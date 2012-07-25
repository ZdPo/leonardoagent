using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZdPo
{
    /// <summary>
    /// Summary description for Constants
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Set of used file names
        /// </summary>
        public static class FileName
        {
            private const string arr = "Arrive";
            private const string dep = "Departure";
            private const string actual = "Actual";
            private const string show = "Show";
            private const string xmlext = ".xml";
            private const string stampext = ".time";
            /// <summary>
            /// Name of Caech directory
            /// </summary>
            public const string CacheDirectory = "cache";
            /// <summary>
            /// Name of actual read arrive data
            /// </summary>
            public const string ArriveActual = arr + actual + xmlext;
            /// <summary>
            /// Name of time stamp for last Arrive actual read
            /// </summary>
            public const string ArriveActualTimeStamp = arr + actual + stampext;
            /// <summary>
            /// Name of full arrive set contains data to show
            /// </summary>
            public const string ArriveShow = arr + show + xmlext;
            /// <summary>
            /// Name of time stamp for last Arrive show
            /// </summary>
            public const string ArriveShowTimeStamp = arr + show + stampext;
            /// <summary>
            /// Name of actual read departure data 
            /// </summary>
            public const string DepartureActual = dep + actual + xmlext;
            /// <summary>
            /// Name of full departure set contains data to show
            /// </summary>
            public const string DepartureShow = dep + show + xmlext;
            /// <summary>
            /// Name of time stamp for last Departure actual read
            /// </summary>
            public const string DepartureActualTimeStamp = dep + stampext;
            /// <summary>
            /// Name of time stamp for last Departure show
            /// </summary>
            public const string DepartureShowTimeStamp = dep + show + stampext;
            /// <summary>
            /// Name of HDG file
            /// </summary>
            public const string HDGFileName = "HDG" + xmlext;
        }

        /// <summary>
        /// Keys in web.config appSettings
        /// </summary>
        public static class AppSetings
        {
            /// <summary>
            /// URL to arrive source server and page
            /// </summary>
            public const string HttpArrive = "Arrive";
            /// <summary>
            /// URL to departure source server and page
            /// </summary>
            public const string HttpDeparture = "Departure";
            /// <summary>
            /// User name for pages, when missing, or blank not need credentials
            /// </summary>
            public const string HttpUser = "usr";
            /// <summary>
            /// Password to server
            /// </summary>
            public const string HttpPwd = "pwd";
            /// <summary>
            /// Refresh time for naex reading data from server in sec
            /// </summary>
            public const string HttpRefresh = "refresh";
            /// <summary>
            /// Maximal history save in server in hours
            /// </summary>
            public const string CacheHistory = "history";
            /// <summary>
            /// User for connect thru proxy, when empty no user use
            /// </summary>
            public const string ProxyUser ="proxyuser";
            /// <summary>
            /// Password for connect thru proxy
            /// </summary>
            public const string ProxyPwd ="proxypwd";
            /// <summary>
            /// Proxy server address or name, When empty not proxy used
            /// </summary>
            public const string ProxyServer ="proxyserver";
            /// <summary>
            /// Port of proxy server
            /// </summary>
            public const string ProxyPort="proxport";
            /// <summary>
            /// User name for administration server data
            /// </summary>
            public const string AdminUser="adminuser";
            /// <summary>
            /// User password for administration  server data
            /// </summary>
            public const string AdminPwd = "adminpwd";
        }

        /// <summary>
        /// Messages for logging system
        /// </summary>
        public static class LoggerMessages
        {
            #region Working with file
            /// <summary>
            /// File was write sucess [{0}]
            /// </summary>
            public const string WriteFileSucess = "File was write sucess [{0}]";
            /// <summary>
            /// File wasn't write success [{0}]
            /// </summary>
            public const string WriteFileError = "File wasn't write success [{0}]";
            /// <summary>
            /// File wasn't delete from disk [{0}]
            /// </summary>
            public const string DeleteFileError = "File wasn't delete from disk [{0}]";
            /// <summary>
            /// File was delete from disk [{0}]
            /// </summary>
            public const string DeleteFileSuccess = "File was delete from disk [{0}]";
            /// <summary>
            /// File wasn't read success [{0}]
            /// </summary>
            public const string ReadFileError = "File wasn't read success [{0}]";
            /// <summary>
            /// File was read success [{0}]
            /// </summary>
            public const string ReadFileSuccess = "File was read success [{0}]";
            /// <summary>
            /// Desired file doesn't exists [{0}]
            /// </summary>
            public const string FileNotExists = "Desired file doesn't exists [{0}]";
            /// <summary>
            /// Desired Timestamp file doesn't exists [{0}]
            /// </summary>
            public const string TimeStampNotExists = "Desired Timestamp file doesn't exists [{0}]";
            /// <summary>
            /// Timestamp was read success. Value is {1}, from file [{0}]
            /// </summary>
            public const string TimeStampSuccess = "Timestamp was read success. Value is {1}, from file [{0}]";
            /// <summary>
            /// Timestamp wasn't read success [{0}]
            /// </summary>
            public const string TimeStampError = "Timestamp wasn't read success [{0}]";
            #endregion

            #region Working with source
            /// <summary>
            /// Create new SourceReader instance
            /// </summary>
            public const string WebSourceCreate = "Create new SourceReader singletone instance";
            /// <summary>
            /// Desired websource wasn't respond or credential not success [{0}]
            /// </summary>
            public const string WebSourceError = "Desired websource wasn't respond or credential not success [{0}]";
            /// <summary>
            /// System sets to use credentials for user [{0}]
            /// </summary>
            public const string WebSourceUserUse = "System sets to use credentials for user [{0}]";
            /// <summary>
            /// System sets to doesn't use credentials
            /// </summary>
            public const string WebSourceUserNotUse = "System sets to doesn't use credentials";
            /// <summary>
            /// System ServiceReader now work with another task. Need try later.
            /// </summary>
            public const string WebSourceNowWork = "System ServiceReader now work with another task. Need try later.";
            /// <summary>
            /// HEAD return {0} [{1}]
            /// </summary>
            public const string WebSourceHeadDate = "HEAD return {0} [{1}]";
            /// <summary>
            /// Application config doesn't containts any keys. Try read name: [{0}]
            /// </summary>
            public const string AppSettingsNotAnyKey = "Application config doesn't containts any keys. Try read name: [{0}]";
            /// <summary>
            /// Application config doesn't containts keys [{0}]
            /// </summary>
            public const string AppSettingsNotKey = "Application config doesn't containts keys [{0}]";
            /// <summary>
            /// System use proxy server [{0}] and port [{1}]
            /// </summary>
            public const string ProxyUsing = "System use proxy server [{0}] and port [{1}]";
            /// <summary>
            /// System use proxy user [{0}]
            /// </summary>
            public const string ProxyUsingUser = "System use proxy user [{0}]";
            #endregion

            #region Working with Data_Proxy
            /// <summary>
            /// Data_Proxy wasn't read source data from server [{0}]
            /// </summary>
            public const string ProxyNotReadSourceDataServer = "Data_Proxy wasn't read source data from server [{0}]";
            /// <summary>
            /// Try read cached data [{0}]
            /// </summary>
            public const string ProxyReadCachedData = "Try read cached data [{0}]";
            /// <summary>
            /// Source data contain unidentification XML node name / value [{0}] / [{1}]
            /// </summary>
            public const string ProxyXmlNodeName = "Source data contain unidentification XML node name / value [{0}] / [{1}]";
            /// <summary>
            /// Source web table {0} is empty. Try read show data from cache.
            /// </summary>
            public const string ProxyWebSourceError = "Source web table {0} is empty. Try read show data from cache.";
            /// <summary>
            /// {0} data table contains indefined value in ORDER column [{1}]
            /// </summary>
            public const string ProxyOrderBadValue = "{0} data table contains indefined value in ORDER column [{1}]";
            #endregion

            #region logon /logoff
            /// <summary>
            /// Try login to system by username [{0}] / paswword [{1}]
            /// </summary>
            public const string LogonError = "Try login to system by username [{0}] / paswword [{1}]";
            #endregion
        }

        /// <summary>
        /// Set of messages for throw new exception
        /// </summary>
        public static class ExceptionMessages
        {
            /// <summary>
            /// Server don't respond [{0}]
            /// </summary>
            public const string ServerNotRespond = "Server don't respond [{0}]";
            /// <summary>
            /// System ServiceReader now work with another task.
            /// </summary>
            public const string ServerNowWork = "System ServiceReader now work with another task.";
            /// <summary>
            /// Time to wait for unlock is expire.
            /// </summary>
            public const string TimeToWaitIsExpire = "Time to wait for unlock is expire.";
        }

        /// <summary>
        /// Set of uncategorized values
        /// </summary>
        public static class CommonConst
        {
            public const string AppName = "Leonardo";
            /// <summary>
            /// Text for Arrive
            /// </summary>
            public const string TextArrive = "Arrive";
            /// <summary>
            /// Text for Departure
            /// </summary>
            public const string TextDeparture = "Departure";
            /// <summary>
            /// "&nbsp;" + AppName + " - {1}"
            /// </summary>
            public const string TitleFormat = "&nbsp;" + AppName + " - {1}";
            public const string PageNameArrive60 = "Prilety +/- 60";
            public const string PageNameArrive = "Prilety";
            public const string PageNameDeparture60 = "Odlety +/- 60";
            public const string PageNameDeparture = "Odlety";
            public const string PageNameAdmin = "Adminstrace";
            /// <summary>
            /// Sleep time in ms = (5s)
            /// </summary>
            public const int ThredSleepTime = 5000;
            /// <summary>
            /// '\r\n'
            /// </summary>
            public const string SelectorNewLine = "\r\n";
            /// <summary>
            /// ', '
            /// </summary>
            public const string SelectorComma = ", ";
            /// <summary>
            /// ' - '
            /// </summary>
            public const string SelectorDash = " - ";
            /// <summary>
            /// ▲
            /// </summary>
            public const string ArrowUp = "▲";
            /// <summary>
            /// ▼
            /// </summary>
            public const string ArrowDown = "▼";
            /// <summary>
            /// ■ █
            /// </summary>
            public const string Block = "■";

        }

        /// <summary>
        /// Formating and other for WebForms
        /// </summary>
        public static class ShowData
        {
            /// <summary>
            /// Aktualni datum a cas (SEC) : {0:dd.MM.yyyy, HH:mm}
            /// </summary>
            public const string SeparatorFormat = "Aktualni datum a cas (SEC) : {0:dd.MM.yyyy, HH:mm}";
        }
    }
}