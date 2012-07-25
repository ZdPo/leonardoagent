using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Data;
using NLog;
using System.Collections;
using System.Drawing;

namespace ZdPo
{
    /// <summary>
    /// This class provide any conversation source XML data to DataTable Data use form Cache or from web server
    /// </summary>
    [System.ComponentModel.DataObject]
    public class Data_Proxy
    {
        #region common variables & create class
        /// <summary>
        /// Target for Logging errors
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// refresh time in sec
        /// </summary>
        private int _refreshTime = 60;
        /// <summary>
        /// Remember history in hours
        /// </summary>
        private int _history = 3;
        /// <summary>
        /// Private referenc to SourceReader Instanc
        /// </summary>
        private SourceReader sourceread = null;
        /// <summary>
        /// Departure data to lock
        /// </summary>
        private static object departureLock = string.Empty;
        /// <summary>
        /// Arrival data to lock
        /// </summary>
        private static object arriveLock= string.Empty;
        /// <summary>
        /// Class constructor, read configurations
        /// </summary>
        public Data_Proxy()
        {
            _refreshTime = Utils.ConvertStringToInt(Utils.ReadConfigurationKey(Constants.AppSetings.HttpRefresh), _refreshTime);
            _history = Utils.ConvertStringToInt(Utils.ReadConfigurationKey(Constants.AppSetings.CacheHistory), _history);
            sourceread = SourceReader.Instance;
        }
        #endregion

        #region Read data from server or cache
        /// <summary>
        /// Read data from source or from cache based on last refresh and availabilty of server
        /// </summary>
        /// <param name="isArrive">true if want read arrive data, false mean departure</param>
        /// <returns></returns>
        internal string ReadSourceData(bool isArrive)
        {
            string filenameTS = Utils.CreatePhysicalFileName(isArrive ? Constants.FileName.ArriveActualTimeStamp : Constants.FileName.DepartureActualTimeStamp);
            DateTime latestTimestamp = Utils.TimeStampRead(filenameTS);
            string filename = Utils.CreatePhysicalFileName(isArrive ? Constants.FileName.ArriveActual : Constants.FileName.DepartureActual);
            string data = "";
            logger.Debug(Constants.LoggerMessages.WebSourceNowWork);
            sourceread.WaitForAvaibleToWorkInstance();
            data = ReadSourceDataWithoutCheck(isArrive);
            if (string.IsNullOrWhiteSpace(data))
            {
                try
                {
                    data = isArrive ? sourceread.ReadArrive() : sourceread.ReadDeparture();
                }
                catch (Exception e)
                {
                    logger.Error(Constants.LoggerMessages.ProxyNotReadSourceDataServer, isArrive ? Constants.CommonConst.TextArrive : Constants.CommonConst.TextDeparture);
                    logger.Debug(e);
                    return "";
                }
                if (Utils.StringWriteToFile(filename, data))
                {
                    Utils.TimeStampWrite(filenameTS, DateTime.Now);
                }
            }
            return data;
        }

        private string ReadSourceDataWithoutCheck(bool isArrive)
        {
            string filenameTS = Utils.CreatePhysicalFileName(isArrive ? Constants.FileName.ArriveActualTimeStamp : Constants.FileName.DepartureActualTimeStamp);
            DateTime latestTimestamp = Utils.TimeStampRead(filenameTS);
            string filename = Utils.CreatePhysicalFileName(isArrive ? Constants.FileName.ArriveActual : Constants.FileName.DepartureActual);
            string data = "";
            latestTimestamp = Utils.TimeStampRead(filenameTS);
            if (latestTimestamp.AddSeconds(_refreshTime) > DateTime.Now)
            {
                logger.Debug(Constants.LoggerMessages.ProxyReadCachedData, filename);
                data = Utils.StringReadFromFile(filename);
            }
            return data;
        }
        #endregion

        #region Propagate data do source data table
        /// <summary>
        /// Return source data of actual read from system
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.ArriveWebDataTable GetAllArrival()
        {
            string fileName = Utils.CreatePhysicalFileName(Constants.FileName.ArriveActualTimeStamp);
            DateTime latestTimestamp = Utils.TimeStampRead(fileName);
            if (latestTimestamp.AddSeconds(_refreshTime) > DateTime.Now)
            {
                Data_Schema.ArriveWebDataTable dataFromCache = ArriveReadHistoryFromFile(false);
                if (dataFromCache != null)
                    return dataFromCache;
            }
            string dataXMLFromWeb = ReadSourceData(true);
            if (!string.IsNullOrWhiteSpace(dataXMLFromWeb))
            {
                return ArriveAppendHistoryToActualReadData(ConvertToArrive(dataXMLFromWeb));
            }
            return new Data_Schema.ArriveWebDataTable();
        }
        /// <summary>
        /// Return source data of actual read from system
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.DepartureWebDataTable GetAllDeparture()
        {
            string fileName = Utils.CreatePhysicalFileName(Constants.FileName.DepartureActualTimeStamp);
            DateTime latestTimestamp = Utils.TimeStampRead(fileName);
            if (latestTimestamp.AddSeconds(_refreshTime) > DateTime.Now)
            {
                Data_Schema.DepartureWebDataTable dataFromCache = DepartureReadHistoryFromFile(false);
                if (dataFromCache != null)
                    return dataFromCache;
            }
            string dataXMLFromWeb = ReadSourceData(false);
            if (!string.IsNullOrWhiteSpace(dataXMLFromWeb))
                return DepartureAppendHistoryToActualReadData(ConvertToDeparture(dataXMLFromWeb));
            return new Data_Schema.DepartureWebDataTable();
        }

        /// <summary>
        /// Convert arrival string to desired DataTable
        /// </summary>
        /// <param name="dataReadFromWebInXMLFormat">Incoming stream</param>
        /// <returns>DataTable or DataTable.Empty when no data to parse or error ocure</returns>
        internal Data_Schema.ArriveWebDataTable ConvertToArrive(string dataReadFromWebInXMLFormat)
        {
            Data_Schema.ArriveWebDataTable tableConvertedFromXML = new Data_Schema.ArriveWebDataTable();
            Data_Schema.ArriveWebRow rowCinvertedFromXML;
            XmlDocument xmlDocumentReadFromService = new XmlDocument();
            xmlDocumentReadFromService.LoadXml(dataReadFromWebInXMLFormat);
            XmlNode xRoot = xmlDocumentReadFromService.DocumentElement;
            XmlNodeList xListContentFromSourceXML = xRoot.SelectNodes("/*/Content");
            string actualDate = DateTime.Now.ToString("dd.MM.yyyy");
            foreach (XmlNode nodeInContentPartXML in xListContentFromSourceXML)
            {
                rowCinvertedFromXML = tableConvertedFromXML.NewArriveWebRow();
                if (nodeInContentPartXML.SelectNodes("DATE_LT").Count > 0)
                    actualDate = nodeInContentPartXML.SelectSingleNode("DATE_LT").InnerText;
                foreach (XmlNode xOneRecordInContentPartXML in nodeInContentPartXML.ChildNodes)
                {
                    switch (xOneRecordInContentPartXML.Name.ToUpper())
                    {
                        case "FLIGHTID":
                            rowCinvertedFromXML.FLIGHTID = Utils.ConvertStringToLong(xOneRecordInContentPartXML.InnerText, 0L);
                            break;
                        case "TIME_LT":
                            rowCinvertedFromXML.TIME_LT = Utils.ConvertHHMM(xOneRecordInContentPartXML.InnerText, rowCinvertedFromXML.TIME_LT);
                            break;
                        case "DATE_LT":
                            rowCinvertedFromXML.TIME_LT = Utils.ConvertDDMMYYY(xOneRecordInContentPartXML.InnerText, rowCinvertedFromXML.TIME_LT);
                            break;
                        case "ICAO_AIR":
                            rowCinvertedFromXML.ICAO_AIR = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "IATAICAO":
                            rowCinvertedFromXML.IATAICAO = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "FLGTNUM":
                            rowCinvertedFromXML.FLGTNUM = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "ORDER":
                            rowCinvertedFromXML.ORDER = Utils.ConvertStringToInt(xOneRecordInContentPartXML.InnerText, 0);
                            break;
                        case "S1":
                            rowCinvertedFromXML.S1 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S1CIT":
                            rowCinvertedFromXML.S1CIT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S2":
                            rowCinvertedFromXML.S2 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S2CIT":
                            rowCinvertedFromXML.S2CIT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "HAL":
                            rowCinvertedFromXML.HAL = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "REMARK":
                            rowCinvertedFromXML.REMARK = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "ETA_LT":
                            rowCinvertedFromXML.ETA_LT = Utils.ConvertHHMM(xOneRecordInContentPartXML.InnerText, DateTime.Now);
                            rowCinvertedFromXML.ETA_LT = Utils.ConvertDDMMYYY(actualDate, rowCinvertedFromXML.ETA_LT);
                            break;
                        case "HDG":
                            rowCinvertedFromXML.HDG = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "AIRCRFT":
                            rowCinvertedFromXML.AIRCRFT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "ATA_LT":
                            rowCinvertedFromXML.ATA_LT = Utils.ConvertHHMM(xOneRecordInContentPartXML.InnerText, DateTime.Now);
                            rowCinvertedFromXML.ATA_LT = Utils.ConvertDDMMYYY(actualDate, rowCinvertedFromXML.ATA_LT);
                            break;
                        case "PARID":
                            rowCinvertedFromXML.PARID = Utils.ConvertStringToLong(xOneRecordInContentPartXML.InnerText, 0L);
                            break;
                        case "STAND":
                            rowCinvertedFromXML.STAND = xOneRecordInContentPartXML.InnerText;
                            break;
                        default:
                            logger.Warn(Constants.LoggerMessages.ProxyXmlNodeName, xOneRecordInContentPartXML.Name, xOneRecordInContentPartXML.InnerText);
                            break;
                    }
                }
               
                tableConvertedFromXML.AddArriveWebRow(rowCinvertedFromXML);
            }

            return tableConvertedFromXML;
        }
        /// <summary>
        /// Convert departure string to desired DataTable
        /// </summary>
        /// <param name="dataReadFromWebInXMLFormat">Incoming stream</param>
        /// <returns>DataTable or DataTable.Empty when no data to parse or error ocure</returns>
        internal Data_Schema.DepartureWebDataTable ConvertToDeparture(string dataReadFromWebInXMLFormat)
        {
            Data_Schema.DepartureWebDataTable tableConvertedFromXML = new Data_Schema.DepartureWebDataTable();
            Data_Schema.DepartureWebRow rowCinvertedFromXML;
            XmlDocument xmlDocumentReadFromService = new XmlDocument();
            xmlDocumentReadFromService.LoadXml(dataReadFromWebInXMLFormat);
            XmlNode xRoot = xmlDocumentReadFromService.DocumentElement;
            XmlNodeList xListContentFromSourceXML = xRoot.SelectNodes("/*/Content");
            string actualDate = DateTime.Now.ToString("dd.MM.yyyy");
            foreach (XmlNode nodeInContentPartXML in xListContentFromSourceXML)
            {
                rowCinvertedFromXML = tableConvertedFromXML.NewDepartureWebRow();
                if (nodeInContentPartXML.SelectNodes("DATE_LT").Count > 0)
                    actualDate = nodeInContentPartXML.SelectSingleNode("DATE_LT").InnerText;
                foreach (XmlNode xOneRecordInContentPartXML in nodeInContentPartXML.ChildNodes)
                {
                    switch (xOneRecordInContentPartXML.Name.ToUpper())
                    {
                        case "FLIGHTID":
                            rowCinvertedFromXML.FLIGHTID = Utils.ConvertStringToLong(xOneRecordInContentPartXML.InnerText, 0L);
                            break;
                        case "TIME_LT":
                            rowCinvertedFromXML.TIME_LT = Utils.ConvertHHMM(xOneRecordInContentPartXML.InnerText, rowCinvertedFromXML.TIME_LT);
                            break;
                        case "DATE_LT":
                            rowCinvertedFromXML.TIME_LT = Utils.ConvertDDMMYYY(xOneRecordInContentPartXML.InnerText, rowCinvertedFromXML.TIME_LT);
                            break;
                        case "ICAO_AIR":
                            rowCinvertedFromXML.ICAO_AIR = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "IATAICAO":
                            rowCinvertedFromXML.IATAICAO = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "FLGTNUM":
                            rowCinvertedFromXML.FLGTNUM = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "ORDER":
                            rowCinvertedFromXML.ORDER = Utils.ConvertStringToInt(xOneRecordInContentPartXML.InnerText, 0);
                            break;
                        case "S1":
                            rowCinvertedFromXML.S1 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S1CIT":
                            rowCinvertedFromXML.S1CIT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S2":
                            rowCinvertedFromXML.S2 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "S2CIT":
                            rowCinvertedFromXML.S2CIT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "HAL":
                            rowCinvertedFromXML.HAL = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "CHIN_F":
                            rowCinvertedFromXML.CHIN_F = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "CHIN_L":
                            rowCinvertedFromXML.CHIN_L = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "REMARK":
                            rowCinvertedFromXML.REMARK = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "GATE1":
                            rowCinvertedFromXML.GATE1 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "GATE2":
                            rowCinvertedFromXML.GATE2 = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "HDG":
                            rowCinvertedFromXML.HDG = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "AIRCRFT":
                            rowCinvertedFromXML.AIRCRFT = xOneRecordInContentPartXML.InnerText;
                            break;
                        case "ATD_LT":
                            rowCinvertedFromXML.ATD_LT = Utils.ConvertHHMM(xOneRecordInContentPartXML.InnerText, DateTime.Now);
                            rowCinvertedFromXML.ATD_LT = Utils.ConvertDDMMYYY(actualDate, rowCinvertedFromXML.ATD_LT);
                            break;
                        case "PARID":
                            rowCinvertedFromXML.PARID = Utils.ConvertStringToLong(xOneRecordInContentPartXML.InnerText, 0L);
                            break;
                        default:
                            logger.Warn(Constants.LoggerMessages.ProxyXmlNodeName, xOneRecordInContentPartXML.Name, xOneRecordInContentPartXML.InnerText);
                            break;
                    }
                }

                tableConvertedFromXML.AddDepartureWebRow(rowCinvertedFromXML);
            }

            return tableConvertedFromXML;
        }

        #endregion

        #region Create table for show data Departure
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.DepartureShowDataTable DepartureData()
        {
            Data_Schema.DepartureShowDataTable tableToShow = new Data_Schema.DepartureShowDataTable();
            Dictionary<long, ShareCodeDeparture> shareCodeDictionary = new Dictionary<long, ShareCodeDeparture>();
            Hashtable ht = new Hashtable();
            Data_Schema.DepartureWebDataTable sourceTableFromWebAndCache = GetAllDeparture();
            if (sourceTableFromWebAndCache == null)
            {
                logger.Error(Constants.LoggerMessages.ProxyWebSourceError, Constants.CommonConst.TextDeparture);
                sourceTableFromWebAndCache = DepartureReadHistoryFromFile(true);
            }
            Data_Schema.DepartureShowRow rowShow;
            foreach (Data_Schema.DepartureWebRow rowSourceTableFromWebAndCache in sourceTableFromWebAndCache.Rows)
            {
                switch (rowSourceTableFromWebAndCache.ORDER)
                {
                    case 1:
                        rowShow = tableToShow.NewDepartureShowRow();
                        if (shareCodeDictionary.ContainsKey(rowSourceTableFromWebAndCache.FLIGHTID))
                            shareCodeDictionary[rowSourceTableFromWebAndCache.FLIGHTID].MASTERROW = rowShow;
                        else
                            shareCodeDictionary.Add(rowSourceTableFromWebAndCache.FLIGHTID, new ShareCodeDeparture(rowShow));
                        tableToShow.AddDepartureShowRow(CopyLineDeparture(rowSourceTableFromWebAndCache, rowShow));
                        break;
                    case 0:
                        tableToShow.AddDepartureShowRow(CopyLineDeparture(rowSourceTableFromWebAndCache, tableToShow.NewDepartureShowRow()));
                        break;
                    case 2:
                        if (shareCodeDictionary.ContainsKey(rowSourceTableFromWebAndCache.PARID))
                        {
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].IATAICAO.Add(rowSourceTableFromWebAndCache.IATAICAO);
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].ICAO_AIR.Add(rowSourceTableFromWebAndCache.ICAO_AIR);
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].FLGTNUM.Add(rowSourceTableFromWebAndCache.FLGTNUM);

                        }
                        else
                            shareCodeDictionary.Add(rowSourceTableFromWebAndCache.PARID, new ShareCodeDeparture(rowSourceTableFromWebAndCache.IATAICAO, rowSourceTableFromWebAndCache.ICAO_AIR, rowSourceTableFromWebAndCache.FLGTNUM));

                        break;
                    default:
                        logger.Warn(Constants.LoggerMessages.ProxyOrderBadValue, Constants.CommonConst.TextDeparture, rowSourceTableFromWebAndCache.PARID);
                        break;
                }
            }
            foreach (ShareCodeDeparture shareCode in shareCodeDictionary.Values)
            {
                if (shareCode.MASTERROW != null)
                {
                    shareCode.MASTERROW.FLGTSHARE = Utils.ConvertListToString(shareCode.FLGTNUM, Constants.CommonConst.SelectorNewLine);
                    shareCode.MASTERROW.ICAO_AIR = Utils.ConvertListToStringICAO(shareCode.MASTERROW.IATAICAO, shareCode.MASTERROW.ICAO_AIR, shareCode.IATAICAO,
                        shareCode.ICAO_AIR, Constants.CommonConst.SelectorDash, Constants.CommonConst.SelectorNewLine);
                }
            }
            tableToShow.AddDepartureShowRow(-1, "", "", "", DateTime.Now, "", "", "", "", "", "", "", "", "", "", "", "", "");
            tableToShow.DefaultView.Sort = "TIME_LT";
            tableToShow.DefaultView.ApplyDefaultSort = true;
            return tableToShow;
        }
        /// <summary>
        /// Return only +/- Time
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.DepartureShowDataTable DepartureData60()
        {
            Data_Schema.DepartureShowDataTable dt = DepartureData();
            Data_Schema.DepartureShowRow row;
            DateTime start = DateTime.Now.AddMinutes(-60);
            DateTime end = DateTime.Now.AddMinutes(60);
            for (int i = dt.Rows.Count; i > 0; i--)
            {
                row = dt.Rows[i - 1] as Data_Schema.DepartureShowRow;
                if ((row.TIME_LT < start) || (row.TIME_LT > end))
                    dt.Rows.RemoveAt(i - 1);
            }
            return dt;
        }
        /// <summary>
        /// Copy one web source row to show destination row
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        internal Data_Schema.DepartureShowRow CopyLineDeparture(Data_Schema.DepartureWebRow source, Data_Schema.DepartureShowRow destination)
        {
            destination.FLIGHTID = source.FLIGHTID;
            destination.TIME_LT = source.TIME_LT;
            destination.ICAO_AIR = source.ICAO_AIR;
            destination.IATAICAO = source.IATAICAO;
            destination.FLGTNUM = source.FLGTNUM;
            destination.S1 = source.S1;
            destination.S1CIT = source.S1CIT;
            if (source.IsS2Null())
                destination.S2 = string.Empty;
            else
                destination.S2 = source.S2;
            if (source.IsS2CITNull())
                destination.S2CIT = string.Empty;
            else
                destination.S2CIT = source.S2CIT;
            if (source.IsHALNull())
                destination.HAL = string.Empty;
            else
                destination.HAL = source.HAL;
            if ((!source.IsCHIN_FNull()) && (!source.IsCHIN_LNull()))
                destination.CHIN = string.Format("{0} - {1}", source.CHIN_F, source.CHIN_L);
            else if (!source.IsCHIN_FNull())
                destination.CHIN = source.CHIN_F;
            else
                destination.CHIN = string.Empty;
            if (source.IsREMARKNull())
                destination.REMARK = string.Empty;
            else
                destination.REMARK = source.REMARK;
            if ((!source.IsGATE1Null()) && (!source.IsGATE2Null()))
                destination.GATE = string.Format("{0} - {1}", source.GATE1, source.GATE2);
            else if (!source.IsGATE1Null())
                destination.GATE = source.GATE1;
            else
                destination.GATE = string.Empty;
            if (source.IsHDGNull())
                destination.HDG = string.Empty;
            else
            {
                destination.HDG = source.HDG;
                destination.HDGDETAIL = HDGDetail(destination.HDG);
            }
            if (!source.IsAIRCRFTNull())
                destination.AIRCRAFT = source.AIRCRFT;
            else
                destination.AIRCRAFT = string.Empty;
            if (source.IsATD_LTNull())
                destination.COLOR = source.TIME_LT >= DateTime.Now ? Color.Black.Name : Color.Red.Name;
            else
                destination.COLOR = Color.Black.Name;
            return destination;
        }
        #endregion

        #region Create table for show data Arrival
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.ArriveShowDataTable ArriveData()
        {
            Data_Schema.ArriveShowDataTable tableToShow = new Data_Schema.ArriveShowDataTable();
            Dictionary<long, ShareCodeArrive> shareCodeDictionary = new Dictionary<long, ShareCodeArrive>();
            Hashtable ht = new Hashtable();
            Data_Schema.ArriveWebDataTable sourceTableFromWebAndCache = GetAllArrival();
            if (sourceTableFromWebAndCache == null)
            {
                logger.Error(Constants.LoggerMessages.ProxyWebSourceError, Constants.CommonConst.TextArrive);
                sourceTableFromWebAndCache = ArriveReadHistoryFromFile(true);
            }
            Data_Schema.ArriveShowRow rowToShow;
            foreach (Data_Schema.ArriveWebRow rowSourceTableFromWebAndCache in sourceTableFromWebAndCache.Rows)
            {
                switch (rowSourceTableFromWebAndCache.ORDER)
                {
                    case 1:
                        rowToShow = tableToShow.NewArriveShowRow();
                        if (shareCodeDictionary.ContainsKey(rowSourceTableFromWebAndCache.FLIGHTID))
                            shareCodeDictionary[rowSourceTableFromWebAndCache.FLIGHTID].MASTERROW = rowToShow;
                        else
                            shareCodeDictionary.Add(rowSourceTableFromWebAndCache.FLIGHTID, new ShareCodeArrive(rowToShow));
                        tableToShow.AddArriveShowRow(CopyLineDeparture(rowSourceTableFromWebAndCache, rowToShow));
                        break;
                    case 0:
                        tableToShow.AddArriveShowRow(CopyLineDeparture(rowSourceTableFromWebAndCache, tableToShow.NewArriveShowRow()));
                        break;
                    case 2:
                        if (shareCodeDictionary.ContainsKey(rowSourceTableFromWebAndCache.PARID))
                        {
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].IATAICAO.Add(rowSourceTableFromWebAndCache.IATAICAO);
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].ICAO_AIR.Add(rowSourceTableFromWebAndCache.ICAO_AIR);
                            shareCodeDictionary[rowSourceTableFromWebAndCache.PARID].FLGTNUM.Add(rowSourceTableFromWebAndCache.FLGTNUM);

                        }
                        else
                            shareCodeDictionary.Add(rowSourceTableFromWebAndCache.PARID, new ShareCodeArrive(rowSourceTableFromWebAndCache.IATAICAO, rowSourceTableFromWebAndCache.ICAO_AIR, rowSourceTableFromWebAndCache.FLGTNUM));

                        break;
                    default:
                        logger.Warn(Constants.LoggerMessages.ProxyOrderBadValue, Constants.CommonConst.TextDeparture, rowSourceTableFromWebAndCache.PARID);
                        break;
                }
            }
            foreach (ShareCodeArrive shareCode in shareCodeDictionary.Values)
            {
                if (shareCode.MASTERROW != null)
                {
                    shareCode.MASTERROW.FLGTSHARE = Utils.ConvertListToString(shareCode.FLGTNUM, Constants.CommonConst.SelectorNewLine);
                    shareCode.MASTERROW.ICAO_AIR = Utils.ConvertListToStringICAO(shareCode.MASTERROW.IATAICAO, shareCode.MASTERROW.ICAO_AIR, shareCode.IATAICAO,
                        shareCode.ICAO_AIR, Constants.CommonConst.SelectorDash, Constants.CommonConst.SelectorNewLine);
                }
            }
            tableToShow.AddArriveShowRow(-1, "", "", "", DateTime.Now, DateTime.Now, DateTime.Now, "", "", "", "", "", "", "", "", "", "", "", "");
            tableToShow.DefaultView.Sort = "TIME_LT";
            tableToShow.DefaultView.ApplyDefaultSort = true;
            return tableToShow;
        }
        /// <summary>
        /// Return only +/- Time
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.ArriveShowDataTable ArriveData60()
        {
            Data_Schema.ArriveShowDataTable arriveTableHours = ArriveData();
            Data_Schema.ArriveShowRow row;
            DateTime startDateTime = DateTime.Now.AddMinutes(-60);
            DateTime endDateTime = DateTime.Now.AddMinutes(60);
            for (int i = arriveTableHours.Rows.Count; i > 0; i--)
            {
                row = arriveTableHours.Rows[i - 1] as Data_Schema.ArriveShowRow;
                if ((row.TIME_LT < startDateTime) || (row.TIME_LT > endDateTime))
                    arriveTableHours.Rows.RemoveAt(i - 1);
            }
            return arriveTableHours;
        }
        /// <summary>
        /// Copy one web source row to show destination row
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        internal Data_Schema.ArriveShowRow CopyLineDeparture(Data_Schema.ArriveWebRow source, Data_Schema.ArriveShowRow destination)
        {
            destination.FLIGHTID = source.FLIGHTID;
            destination.TIME_LT = source.TIME_LT;
            destination.ICAO_AIR = source.ICAO_AIR;
            destination.IATAICAO = source.IATAICAO;
            destination.FLGTNUM = source.FLGTNUM;
            destination.S1 = source.S1;
            destination.S1CIT = source.S1CIT;
            if (source.IsS2Null())
                destination.S2 = string.Empty;
            else
                destination.S2 = source.S2;
            if (source.IsS2CITNull())
                destination.S2CIT = string.Empty;
            else
                destination.S2CIT = source.S2CIT;
            if (source.IsHALNull())
                destination.HAL = string.Empty;
            else
                destination.HAL = source.HAL;
            if (source.IsREMARKNull())
                destination.REMARK = string.Empty;
            else
                destination.REMARK = source.REMARK;
            if (!source.IsETA_LTNull())
                destination.ETA_LT = source.ETA_LT;
            if (source.IsHDGNull())
                destination.HDG = string.Empty;
            else
            {
                destination.HDG = source.HDG;
                destination.HDGDETAIL = HDGDetail(destination.HDG);
            }
            if (source.IsAIRCRFTNull())
                destination.AIRCRAFT = string.Empty;
            else
                destination.AIRCRAFT = source.AIRCRFT;
            if (!source.IsATA_LTNull())
                destination.ATA_LT = source.ATA_LT;
            if (source.IsSTANDNull())
                destination.STAND = "";
            else
                destination.STAND = source.STAND;
            if (source.IsATA_LTNull())
                destination.COLOR = source.TIME_LT >= DateTime.Now ? Color.Black.Name : Color.Red.Name;
            else
                destination.COLOR = Color.Black.Name;
            return destination;
        }
        #endregion

        #region internal structure
        /// <summary>
        /// Create detail for HDG
        /// </summary>
        /// <param name="HDG"></param>
        /// <returns></returns>
        internal string HDGDetail(string HDG)
        {
            if (!string.IsNullOrWhiteSpace(HDG))
            {
                Data_Schema.HDGDataTable HDGTable = (new Admin_Proxy()).GetAllTable();
                foreach (Data_Schema.HDGRow rowHDG in HDGTable)
                {
                    if (HDG == rowHDG.HDGID)
                    {
                        string outs = "";
                        if (!string.IsNullOrWhiteSpace(rowHDG.Line1))
                            outs = rowHDG.Line1;
                        if (!string.IsNullOrWhiteSpace(rowHDG.Line2))
                        {
                            if (outs.Length > 0)
                                outs = string.Format("{0}{1}{2}", outs, Constants.CommonConst.SelectorNewLine, rowHDG.Line2);
                            else
                                outs = rowHDG.Line2;
                        }
                        return outs;
                    }
                }
            }
            return null;
        }

        internal class ShareCodeDeparture
        {
            /// <summary>
            /// Contains reference to show datatable
            /// </summary>
            public Data_Schema.DepartureShowRow MASTERROW = null;
            public List<string> IATAICAO = new List<string>();
            public List<string> ICAO_AIR = new List<string>();
            public List<string> FLGTNUM = new List<string>();
            public ShareCodeDeparture(Data_Schema.DepartureShowRow source)
            {
                MASTERROW = source;
            }
            public ShareCodeDeparture(string iataicao, string icao_air, string flgtnum)
            {
                IATAICAO.Add(iataicao);
                ICAO_AIR.Add(icao_air);
                FLGTNUM.Add(flgtnum);
            }
        }
        internal class ShareCodeArrive
        {
            /// <summary>
            /// Contains reference to show datatable
            /// </summary>
            public Data_Schema.ArriveShowRow MASTERROW = null;
            public List<string> IATAICAO = new List<string>();
            public List<string> ICAO_AIR = new List<string>();
            public List<string> FLGTNUM = new List<string>();
            public ShareCodeArrive(Data_Schema.ArriveShowRow source)
            {
                MASTERROW = source;
            }
            public ShareCodeArrive(string iataicao, string icao_air, string flgtnum)
            {
                IATAICAO.Add(iataicao);
                ICAO_AIR.Add(icao_air);
                FLGTNUM.Add(flgtnum);
            }
        }
        #endregion

        #region Work with history (apend to data actal table and save it)
        /// <summary>
        /// Append history data and new data
        /// </summary>
        /// <param name="actualDataReadFromService">actual read data from service</param>
        /// <returns></returns>
        internal Data_Schema.ArriveWebDataTable ArriveAppendHistoryToActualReadData(Data_Schema.ArriveWebDataTable actualDataReadFromService)
        {
            Data_Schema.ArriveWebDataTable ArriveHistoryDataFromFile;
            lock (arriveLock)
            {
                ArriveHistoryDataFromFile = ArriveReadHistoryFromFile(false);
                if (ArriveHistoryDataFromFile == null)
                {
                    ArriveHistoryDataFromFile = new Data_Schema.ArriveWebDataTable();
                }
                /// delete olders then Hist time stamp
                DateTime minimalDateForRemember = DateTime.Now.AddHours(-_history);
                List<long> ListOfFlightID = actualDataReadFromService.AsEnumerable().Select(Selrow => Selrow.Field<long>("FLIGHTID")).ToList();
                if (ListOfFlightID == null)
                    ListOfFlightID = new List<long>();
                Data_Schema.ArriveWebRow rowForShowOnWeb;
                for (int i = ArriveHistoryDataFromFile.Rows.Count; i > 0; i--)
                {
                    rowForShowOnWeb = ArriveHistoryDataFromFile.Rows[i - 1] as Data_Schema.ArriveWebRow;
                    if (rowForShowOnWeb != null)
                    {
                        if (ListOfFlightID.Contains(rowForShowOnWeb.FLIGHTID))
                            ArriveHistoryDataFromFile.Rows.RemoveAt(i - 1);
                        else if (rowForShowOnWeb.TIME_LT < minimalDateForRemember)
                            ArriveHistoryDataFromFile.Rows.RemoveAt(i - 1);
                    }
                }
                foreach (Data_Schema.ArriveWebRow rowActualReadFromService in actualDataReadFromService.Rows)
                {
                    rowForShowOnWeb = ArriveHistoryDataFromFile.NewArriveWebRow();
                    rowForShowOnWeb.FLIGHTID = rowActualReadFromService.FLIGHTID;
                    rowForShowOnWeb.TIME_LT = rowActualReadFromService.TIME_LT;
                    rowForShowOnWeb.ICAO_AIR = rowActualReadFromService.ICAO_AIR;
                    rowForShowOnWeb.IATAICAO = rowActualReadFromService.IATAICAO;
                    rowForShowOnWeb.PARID = rowActualReadFromService.PARID;
                    rowForShowOnWeb.FLGTNUM = rowActualReadFromService.FLGTNUM;
                    rowForShowOnWeb.S1 = rowActualReadFromService.S1;
                    rowForShowOnWeb.S1CIT = rowActualReadFromService.S1CIT;
                    if (!rowActualReadFromService.IsAIRCRFTNull())
                        rowForShowOnWeb.AIRCRFT = rowActualReadFromService.AIRCRFT;
                    if (!rowActualReadFromService.IsATA_LTNull())
                        rowForShowOnWeb.ATA_LT = rowActualReadFromService.ATA_LT;
                    if (!rowActualReadFromService.IsETA_LTNull())
                        rowForShowOnWeb.ETA_LT = rowActualReadFromService.ETA_LT;
                    if (!rowActualReadFromService.IsHALNull())
                        rowForShowOnWeb.HAL = rowActualReadFromService.HAL;
                    if (!rowActualReadFromService.IsHDGNull())
                        rowForShowOnWeb.HDG = rowActualReadFromService.HDG;
                    if (!rowActualReadFromService.IsORDERNull())
                        rowForShowOnWeb.ORDER = rowActualReadFromService.ORDER;
                    if (!rowActualReadFromService.IsREMARKNull())
                        rowForShowOnWeb.REMARK = rowActualReadFromService.REMARK;
                    if (!rowActualReadFromService.IsS2CITNull())
                        rowForShowOnWeb.S2CIT = rowActualReadFromService.S2CIT;
                    if (!rowActualReadFromService.IsS2Null())
                        rowForShowOnWeb.S2 = rowActualReadFromService.S2;
                    if (!rowActualReadFromService.IsSTANDNull())
                        rowForShowOnWeb.STAND = rowActualReadFromService.STAND;
                    ArriveHistoryDataFromFile.AddArriveWebRow(rowForShowOnWeb);
                }
                string fileName = Utils.CreatePhysicalFileName(Constants.FileName.ArriveShow);
                Utils.DataTableWriteToFile(fileName, ArriveHistoryDataFromFile);
            }
            return ArriveHistoryDataFromFile;
        }
        /// <summary>
        /// Fill historical data
        /// </summary>
        /// <param name="actualDataReadFromService"></param>
        /// <returns></returns>
        internal Data_Schema.DepartureWebDataTable DepartureAppendHistoryToActualReadData(Data_Schema.DepartureWebDataTable actualDataReadFromService)
        {
            Data_Schema.DepartureWebDataTable departureHistoryDataFromFile;
            lock (departureLock)
            {
                departureHistoryDataFromFile = DepartureReadHistoryFromFile(false);
                if (departureHistoryDataFromFile == null)
                    departureHistoryDataFromFile = new Data_Schema.DepartureWebDataTable();
                DateTime minimalDateForRemember = DateTime.Now.AddHours(-_history);
                List<long> ListOfFlightID = actualDataReadFromService.AsEnumerable().Select(Selrow => Selrow.Field<long>("FLIGHTID")).ToList();
                if (ListOfFlightID == null)
                    ListOfFlightID = new List<long>();
                Data_Schema.DepartureWebRow rowForShowOnWeb;
                for (int i = departureHistoryDataFromFile.Rows.Count; i > 0; i--)
                {
                    rowForShowOnWeb = departureHistoryDataFromFile.Rows[i - 1] as Data_Schema.DepartureWebRow;
                    if (rowForShowOnWeb != null)
                    {
                        if (ListOfFlightID.Contains(rowForShowOnWeb.FLIGHTID))
                            departureHistoryDataFromFile.Rows.RemoveAt(i - 1);
                        else if (rowForShowOnWeb.TIME_LT < minimalDateForRemember)
                            departureHistoryDataFromFile.Rows.RemoveAt(i - 1);
                    }
                }
                foreach (Data_Schema.DepartureWebRow rowActualReadFromService in actualDataReadFromService.Rows)
                {
                    rowForShowOnWeb = departureHistoryDataFromFile.NewDepartureWebRow();
                    rowForShowOnWeb.FLIGHTID = rowActualReadFromService.FLIGHTID;
                    rowForShowOnWeb.TIME_LT = rowActualReadFromService.TIME_LT;
                    rowForShowOnWeb.ICAO_AIR = rowActualReadFromService.ICAO_AIR;
                    rowForShowOnWeb.IATAICAO = rowActualReadFromService.IATAICAO;
                    rowForShowOnWeb.FLGTNUM = rowActualReadFromService.FLGTNUM;
                    rowForShowOnWeb.S1 = rowActualReadFromService.S1;
                    rowForShowOnWeb.S1CIT = rowActualReadFromService.S1CIT;
                    rowForShowOnWeb.PARID = rowActualReadFromService.PARID;
                    if (!rowActualReadFromService.IsAIRCRFTNull())
                        rowForShowOnWeb.AIRCRFT = rowActualReadFromService.AIRCRFT;
                    if (!rowActualReadFromService.IsHALNull())
                        rowForShowOnWeb.HAL = rowActualReadFromService.HAL;
                    if (!rowActualReadFromService.IsHDGNull())
                        rowForShowOnWeb.HDG = rowActualReadFromService.HDG;
                    if (!rowActualReadFromService.IsORDERNull())
                        rowForShowOnWeb.ORDER = rowActualReadFromService.ORDER;
                    if (!rowActualReadFromService.IsREMARKNull())
                        rowForShowOnWeb.REMARK = rowActualReadFromService.REMARK;
                    if (!rowActualReadFromService.IsS2CITNull())
                        rowForShowOnWeb.S2CIT = rowActualReadFromService.S2CIT;
                    if (!rowActualReadFromService.IsS2Null())
                        rowForShowOnWeb.S2 = rowActualReadFromService.S2;
                    if (!rowActualReadFromService.IsATD_LTNull())
                        rowForShowOnWeb.ATD_LT = rowActualReadFromService.ATD_LT;
                    if (!rowActualReadFromService.IsGATE1Null())
                        rowForShowOnWeb.GATE1 = rowActualReadFromService.GATE1;
                    if (!rowActualReadFromService.IsGATE2Null())
                        rowForShowOnWeb.GATE2 = rowActualReadFromService.GATE2;
                    if (!rowActualReadFromService.IsCHIN_FNull())
                        rowForShowOnWeb.CHIN_F = rowActualReadFromService.CHIN_F;
                    if (!rowActualReadFromService.IsCHIN_LNull())
                        rowForShowOnWeb.CHIN_L = rowActualReadFromService.CHIN_L;
                    departureHistoryDataFromFile.AddDepartureWebRow(rowForShowOnWeb);
                }
                string fileName = Utils.CreatePhysicalFileName(Constants.FileName.DepartureShow);
                Utils.DataTableWriteToFile(fileName, departureHistoryDataFromFile);
            }
            return departureHistoryDataFromFile;
        }
        /// <summary>
        /// Read data from file
        /// </summary>
        /// <param name="addBlankLineInEmptyTable"></param>
        /// <returns></returns>
        internal Data_Schema.DepartureWebDataTable DepartureReadHistoryFromFile(bool addBlankLineInEmptyTable)
        {
            string fileName=Utils.CreatePhysicalFileName(Constants.FileName.DepartureShow);
            Data_Schema.DepartureWebDataTable history = Utils.DataTableReadFromFile(fileName, new Data_Schema.DepartureWebDataTable()) as Data_Schema.DepartureWebDataTable;
            if ((history == null) && addBlankLineInEmptyTable)
            {
                    history = new Data_Schema.DepartureWebDataTable();
                    history.AddDepartureWebRow(0L, DateTime.MaxValue, "", "", "", "", "", "", "", "", "", "", 0L, 0, "", "", "", "", "", DateTime.MaxValue);
            }
            return history;
        }
        /// <summary>
        /// Read data from file 
        /// </summary>
        /// <param name="addBlankLineInEmptyTable">if add line in blank data table</param>
        /// <returns></returns>
        internal Data_Schema.ArriveWebDataTable ArriveReadHistoryFromFile(bool addBlankLineInEmptyTable)
        {
            string fileName = Utils.CreatePhysicalFileName(Constants.FileName.ArriveShow);
            Data_Schema.ArriveWebDataTable history = Utils.DataTableReadFromFile(fileName, new Data_Schema.ArriveWebDataTable()) as Data_Schema.ArriveWebDataTable;
            if ((history == null) && addBlankLineInEmptyTable)
            {
                history = new Data_Schema.ArriveWebDataTable();
                history.AddArriveWebRow(0L, DateTime.MaxValue, "", "", "", "", "", "", "", "", 0L, 0, "", DateTime.MaxValue, "", "", DateTime.MaxValue, "");
            }
            return history;
        }
        #endregion
 
    }
}