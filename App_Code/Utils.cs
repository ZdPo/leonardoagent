using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NLog;
using System.Xml;
using System.Text;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections.Specialized;

namespace ZdPo
{
    /// <summary>
    /// Set of static function
    /// </summary>
    /// <remarks>Any procedure throw exception. All exception are handled by procedure and details write to log file. Exception from catch are write as debug detail after short error message.</remarks>
    public static class Utils
    {
        #region Common proc
        /// <summary>
        /// Return Refresh time for page
        /// </summary>
        /// <returns></returns>
        public static string RefreshTag()
        {
            return ReadConfigurationKey(Constants.AppSetings.HttpRefresh);
        }
        /// <summary>
        /// for loging information and errors
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Prepare name for file with full path on physical device
        /// </summary>
        /// <param name="file">File name of file in </param>
        /// <returns>Right file name contains full path</returns>
        public static string CreatePhysicalFileName(string file)
        {
            string fn = AppDomain.CurrentDomain.BaseDirectory;
            fn = Path.Combine(fn, Constants.FileName.CacheDirectory);
            fn = Path.Combine(fn, file);
            return fn;
        }
        /// <summary>
        /// Convert string to int. When string doesn't possible convert return defaultInt
        /// </summary>
        /// <param name="source">String to convert</param>
        /// <param name="defaultInt">Default value when convertion not success</param>
        /// <returns>Converted or defaultInt value</returns>
        public static int ConvertStringToInt(string source, int defaultInt)
        {
            int val = defaultInt;
            if (int.TryParse(source, out val))
                return val;
            return defaultInt;
        }
        /// <summary>
        /// Convert string to Long. When string doesn't possible convert return defaultInt
        /// </summary>
        /// <param name="source">String to convert</param>
        /// <param name="defaultInt">Default value when convertion not success</param>
        /// <returns>Converted or defaultInt value</returns>
        public static long ConvertStringToLong(string source, long defaultInt)
        {
            long val = defaultInt;
            if (long.TryParse(source, out val))
                return val;
            return defaultInt;
        }
        /// <summary>
        /// Read configuration file and return right value
        /// </summary>
        /// <param name="name">Name of appSettings key</param>
        /// <returns>value of requestet key or empty value</returns>
        public static string ReadConfigurationKey(string name)
        {
            string returnValue = string.Empty;
            if (ConfigurationManager.AppSettings is NameValueCollection)
            {
                if (!ConfigurationManager.AppSettings.HasKeys())
                {
                    logger.Error(Constants.LoggerMessages.AppSettingsNotAnyKey, name);
                }
                else
                {
                    foreach (string s in ConfigurationManager.AppSettings.Keys)
                    {
                        if (s == name)
                        {
                            returnValue = ConfigurationManager.AppSettings[name];
                        }
                    }
                    if (string.IsNullOrWhiteSpace(returnValue))
                        logger.Error(Constants.LoggerMessages.AppSettingsNotKey, name);
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Convert source string in format HH:MM and merge it to actual date
        /// </summary>
        /// <param name="source">Source string in HH:MM format</param>
        /// <param name="actual">Actual DateTime vaule where change HH:MM</param>
        /// <returns>Merged DateTime</returns>
        public static DateTime ConvertHHMM(string source, DateTime actual)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                Regex rx = new Regex("(0[0-9]|1[0-9]|2[0-3]):([0-5][0-9])");
                Match mx = rx.Match(source);
                if (mx.Success)
                {
                    int hh = ConvertStringToInt(mx.Groups[1].Value, actual.Hour);
                    int mm = ConvertStringToInt(mx.Groups[2].Value, actual.Minute);
                    actual = new DateTime(actual.Year, actual.Month, actual.Day, hh, mm, 0);
                }
            }
            return actual;
        }
        /// <summary>
        /// Convert source string in format DD.MM.YYYY and merge it to actual date
        /// </summary>
        /// <param name="source">Source string in DD.MM.YYYY format</param>
        /// <param name="actual">Actual DateTime vaule where change DD.MM.YYYY</param>
        /// <returns>Merged DateTime</returns>
        public static DateTime ConvertDDMMYYY(string source, DateTime actual)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                Regex rx = new Regex("(0[0-9]|1[0-9]|2[0-9]|3[0-1]).(0[0-9]|1[0-2]).(2[0-9][0-9][0-9])");
                Match mx = rx.Match(source);
                if (mx.Success)
                {
                    int dd = ConvertStringToInt(mx.Groups[1].Value, actual.Day);
                    int mm = ConvertStringToInt(mx.Groups[2].Value, actual.Month);
                    int yyyy = ConvertStringToInt(mx.Groups[3].Value, actual.Year);
                    actual = new DateTime(yyyy, mm, dd, actual.Hour, actual.Minute, 0);
                }
            }
            return actual;
        }
        /// <summary>
        /// Convert List to string separated by selector
        /// </summary>
        /// <param name="src"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string ConvertListToString(List<string> src, string selector)
        {
            StringBuilder sb = new StringBuilder("");
            bool isNext = false;
            if (src is List<string>)
            {
                foreach (string s in src)
                {
                    if (isNext)
                        sb.Append(selector);
                    sb.Append(s);
                    isNext = true;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Convert two list to formated string for show where on start is primary and primary2
        /// </summary>
        /// <param name="primary"></param>
        /// <param name="primary2"></param>
        /// <param name="src"></param>
        /// <param name="src2"></param>
        /// <param name="selectorIn"></param>
        /// <param name="selectorEnd"></param>
        /// <returns></returns>
        public static string ConvertListToStringICAO(string primary, string primary2, List<string> src, List<string> src2, string selectorIn, string selectorEnd)
        {
            List<string> Lines = new List<string>();
            Lines.Add(string.Format("{0}{1}{2}", primary, selectorIn, primary2));
            int len = src.Count < src2.Count ? src2.Count : src.Count;
            for (int i = 0; i < len; i++)
            {
                if (i < src.Count)
                    primary = src[i];
                else
                    primary = "";
                if (i < src2.Count)
                    primary2 = src2[i];
                else
                    primary2 = "";
                if (primary2.Length + primary.Length > 0)
                    Lines.Add(string.Format("{0}{1}{2}", primary, selectorIn, primary2));
            }
            return ConvertListToString(Lines, selectorEnd);
        }
        #endregion

        #region Timestamp
        /// <summary>
        /// Try read time stamp from filename.
        /// </summary>
        /// <param name="filename">Ful path of file name where timestamp saved</param>
        /// <returns>timestamp or DateTime.MinValue, when file does'n exist or timestamp isn't convert</returns>
        public static DateTime TimeStampRead(string filename)
        {
            DateTime dt = new DateTime();
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error(Constants.LoggerMessages.ReadFileError, filename);
                return DateTime.MinValue;
            }
            if (!File.Exists(filename))
            {
                logger.Error(Constants.LoggerMessages.TimeStampNotExists, filename);
                return DateTime.MinValue;
            }
            try
            {
                string Stamp = File.ReadAllText(filename);
                dt = TimeAccessGet(Stamp);
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.TimeStampError, filename);
                logger.Debug(e);
                return DateTime.MinValue;
            }
            if (dt != DateTime.MinValue)
                logger.Debug(Constants.LoggerMessages.TimeStampSuccess, filename, dt);
            else
                logger.Error(Constants.LoggerMessages.TimeStampError, filename);
            return dt;
        }
        /// <summary>
        /// Write When as actual timestamp to file name
        /// </summary>
        /// <param name="filename">Fulpath of file for writing timestamp</param>
        /// <param name="when">timestamp to write</param>
        /// <returns>That writing is success</returns>
        public static bool TimeStampWrite(string filename, DateTime when)
        {
            if (string.IsNullOrEmpty(filename))
            {
                logger.Error(Constants.LoggerMessages.WriteFileError, filename);
                return false;
            }
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    logger.Debug(Constants.LoggerMessages.DeleteFileSuccess, filename);
                }
                catch (Exception ae)
                {
                    logger.Error(Constants.LoggerMessages.DeleteFileError, filename);
                    logger.Debug(ae);
                    return false;
                }
            }
            try
            {
                using (StreamWriter wr = new StreamWriter(filename))
                {
                    wr.WriteLine(when.Ticks.ToString());
                    wr.Close();
                    logger.Debug(Constants.LoggerMessages.WriteFileSucess, filename);
                }
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.WriteFileError, filename);
                logger.Debug(e);
                return false;
            }
            return true;
        }
        #endregion

        #region File write/read
        /// <summary>
        /// Read requested filename and try convert it to NewDataTable. When any problems occure return Null
        /// </summary>
        /// <param name="filename">File to read in full path</param>
        /// <param name="NewDataTable">blank DataTable type</param>
        /// <returns>fill NewDataTable read from file of NULl when reading not success</returns>
        public static object DataTableReadFromFile(string filename, object NewDataTable)
        {
            if (!File.Exists(filename))
            {
                logger.Error(Constants.LoggerMessages.FileNotExists, filename);
                return (object)null;
            }
            try
            {
                ((DataTable)NewDataTable).ReadXml(filename);
                //logger.Debug(Constants.LoggerMessages.ReadFileSuccess, filename);
                return NewDataTable;
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.ReadFileError, filename);
                logger.Debug(e);
            }
            return (object)null;
        }

        /// <summary>
        /// Read requested filename. When any problems occure return Null
        /// </summary>
        /// <param name="filename">File to read in full path</param>
        /// <returns>readed data from file or NULl when reading not success</returns>
        public static string StringReadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                logger.Error(Constants.LoggerMessages.FileNotExists, filename);
                return null;
            }
            string data = "";
            try
            {
                using (TextReader tr = File.OpenText(filename))
                {
                    data = tr.ReadToEnd();
                    tr.Close();
                }
                logger.Debug(Constants.LoggerMessages.ReadFileSuccess, filename);
                return data;
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.ReadFileError, filename);
                logger.Debug(e);
            }
            return null;
        }
        /// <summary>
        /// Write DataTable to cache with defined file name
        /// </summary>
        /// <param name="filename">Full path file name for destination file</param>
        /// <param name="datatable">DataTable to write</param>
        /// <returns>Determines that action is success</returns>
        public static bool DataTableWriteToFile(string filename, DataTable datatable)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    logger.Debug(Constants.LoggerMessages.DeleteFileSuccess, filename);
                }
                catch (Exception e)
                {
                    logger.Error(Constants.LoggerMessages.DeleteFileError, filename);
                    logger.Debug(e);
                }
            }
            try
            {
                datatable.WriteXml(filename);
                logger.Debug(Constants.LoggerMessages.WriteFileSucess, filename);
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.WriteFileError, filename);
                logger.Debug(e);
            }
            return true;
        }

        /// <summary>
        /// Write string to cache with defined file name
        /// </summary>
        /// <param name="filename">Full path file name for destination file</param>
        /// <param name="data">String data to write</param>
        /// <returns>Determines that action is success</returns>
        public static bool StringWriteToFile(string filename, string data)
        {
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                    logger.Debug(Constants.LoggerMessages.DeleteFileSuccess, filename);
                }
                catch (Exception e)
                {
                    logger.Error(Constants.LoggerMessages.DeleteFileError, filename);
                    logger.Debug(e);
                }
            }
            try
            {
                using (TextWriter tw = File.CreateText(filename))
                {
                    tw.WriteLine(data);
                    tw.Close();
                }
                logger.Debug(Constants.LoggerMessages.WriteFileSucess, filename);
            }
            catch (Exception e)
            {
                logger.Error(Constants.LoggerMessages.WriteFileError, filename);
                logger.Debug(e);
            }
            return true;
        }
        #endregion

        #region Private function
        /// <summary>
        /// Convert access info to date time
        /// </summary>
        /// <param name="dne">string read from timestamp file</param>
        /// <returns>Converted data of DateTime.MinValue</returns>
        private static DateTime TimeAccessGet(string dne)
        {
            long tc = 0;
            DateTime dd = DateTime.MinValue;
            if (long.TryParse(dne, out tc))
            {
                dd = new DateTime(tc);
            }
            return dd;
        }

        #endregion

        #region Work with Master and WebForms
        /// <summary>
        /// Set image on ImageAir control
        /// </summary>
        /// <param name="IsArrive"></param>
        /// <param name="ImageAir"></param>
        public static void ShowImage(bool IsArrive, System.Web.UI.WebControls.Image ImageAir)
        {
            ImageAir.ImageUrl = IsArrive ? "~/Images/prilety_aplikace.jpg" : "~/Images/odlety_aplikace.jpg";
            ImageAir.AlternateText = IsArrive ? Constants.CommonConst.TextArrive : Constants.CommonConst.TextDeparture;
        }
        /// <summary>
        /// Create serach TextBox
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public static TextBox CreateBox(string ID, int pixels)
        {
            TextBox S1 = new TextBox();
            S1.ID = "txb" + ID;
            S1.SkinID = "SmallTextBox";
            S1.Width = Unit.Pixel(pixels);
            S1.ViewStateMode = ViewStateMode.Enabled;
            return S1;
        }

        public static string CreateFilter(string Source, string Value, string Expr)
        {
            string format="({0} LIKE '{1}%')";
            if (Value == "FLGTNUM")
                format="({0} LIKE '%{1}%')";
            if (Value.EndsWith("_LT"))
                format = "({0} = '{1}')";
            if (Source.Length > 0)
                format = "{2} AND " + format;
            return string.Format(format, Value, Expr, Source);
        }
        public static string CreateFilterText(string Source, string Value, string Expr)
        {
            string format = "({0}: '{1}*')";
            if (Value == "LET")
                format = "({0}: '*{1}*')";
            if (Value.EndsWith("_LT"))
                format = "({0}: {1})";
            if (Source.Length > 0)
                format = "{2} AND " + format;
            return string.Format(format, Value, Expr, Source);
        }
        #endregion
    }
}