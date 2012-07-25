using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

namespace ZdPo
{
    /// <summary>
    /// Manipulate with Admin DataTable
    /// </summary>
    [System.ComponentModel.DataObject]
    public class Admin_Proxy
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Admin_Proxy()
        {
        }

        #region Work with XML file
        /// <summary>
        /// Reag all data from XML data source
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Select, false)]
        public Data_Schema.HDGDataTable GetAllTable()
        {
            Data_Schema.HDGDataTable dt =
                (Data_Schema.HDGDataTable)Utils.DataTableReadFromFile(Utils.CreatePhysicalFileName(Constants.FileName.HDGFileName), new Data_Schema.HDGDataTable());
            if (dt == null)
                dt = new Data_Schema.HDGDataTable();

            return dt;
        }
        /// <summary>
        /// Insert new line to XML table
        /// </summary>
        /// <param name="HDGID"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Insert, false)]
        public void Insert(string HDGID, string line1, string line2)
        {
            if (string.IsNullOrWhiteSpace(HDGID))
                return;
            if (string.IsNullOrWhiteSpace(line1))
                line1 = string.Empty;
            if (string.IsNullOrWhiteSpace(line2))
                line2 = string.Empty;

            Data_Schema.HDGDataTable dt = GetAllTable();

            foreach (Data_Schema.HDGRow rw in dt.Rows)
            {
                if (HDGID == rw.HDGID)
                    return;
            }
            dt.AddHDGRow(HDGID, line1, line2);
            Utils.DataTableWriteToFile(Utils.CreatePhysicalFileName(Constants.FileName.HDGFileName), dt);
        }
        /// <summary>
        /// Delete one line from XML file
        /// </summary>
        /// <param name="HDGID"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Delete, false)]
        public void Delete(string HDGID, string line1, string line2, string original_HDGID)
        {
            if (string.IsNullOrWhiteSpace(original_HDGID))
                return;
            Data_Schema.HDGDataTable dt = GetAllTable();
            Data_Schema.HDGRow rw;
            for (int i = dt.Rows.Count; i > 0; i--)
            {
                rw = (Data_Schema.HDGRow)dt.Rows[i - 1];
                if (original_HDGID == rw.HDGID)
                    dt.Rows.RemoveAt(i - 1);
            }
            Utils.DataTableWriteToFile(Utils.CreatePhysicalFileName(Constants.FileName.HDGFileName), dt);
        }
        /// <summary>
        /// Update one line in XML file
        /// </summary>
        /// <param name="Line1"></param>
        /// <param name="Line2"></param>
        /// <param name="original_HDGID"></param>
        /// <param name="original_Line1"></param>
        /// <param name="original_Line2"></param>
        [System.ComponentModel.DataObjectMethodAttribute(System.ComponentModel.DataObjectMethodType.Update, false)]
        public void Update(string Line1, string Line2, string original_HDGID, string original_Line1, string original_Line2)
        {
            Data_Schema.HDGDataTable dt = GetAllTable();
            if (string.IsNullOrWhiteSpace(Line1))
                Line1 = string.Empty;
            if (string.IsNullOrWhiteSpace(Line2))
                Line2 = string.Empty;
            foreach (Data_Schema.HDGRow rw in dt.Rows)
            {
                if (original_HDGID == rw.HDGID)
                {
                    rw.Line1 = Line1;
                    rw.Line2 = Line2;
                }
            }
            Utils.DataTableWriteToFile(Utils.CreatePhysicalFileName(Constants.FileName.HDGFileName), dt);
        }
        #endregion

        #region Authenticate
        /// <summary>
        /// Authenticite user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool Authenticate(string user, string pwd)
        {
            string admUsr = Utils.ReadConfigurationKey(Constants.AppSetings.AdminUser);
            string admPwd = Utils.ReadConfigurationKey(Constants.AppSetings.AdminPwd);
            if (!((user == admUsr) && (pwd == admPwd)))
                logger.Warn(Constants.LoggerMessages.LogonError, user, pwd);

            return ((user == admUsr) && (pwd == admPwd));
        }
        #endregion
    }
}