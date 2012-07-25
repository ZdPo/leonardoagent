using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace ZdPo
{
    /// <summary>
    /// Class read data from specified web server. Used as singletone class.
    /// </summary>
    /// <remarks>All procedure work with web server and return data or throw WebException. Any error, debug messages are write to log file before system throw exception from this class.</remarks>
    public class SourceReader
    {
        #region private variables
        /// <summary>
        /// URL for reading arrive table
        /// </summary>
        private string arrive = "";
        /// <summary>
        /// URL for reading departure table
        /// </summary>
        private string departure = "";
        /// <summary>
        /// user for authentification
        /// </summary>
        private string user = "";
        /// <summary>
        /// password for authentification
        /// </summary>
        private string pwd = "";
        /// <summary>
        /// logger class for centralized logging
        /// </summary>
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// singletone class variable
        /// </summary>
        private static SourceReader _instance = null;
        /// <summary>
        /// information that another process now working or not
        /// </summary>
        private static bool _isWork = false;
        /// <summary>
        /// Name or IP address for proxy server
        /// </summary>
        private static string proxyServer = "";
        /// <summary>
        /// Port number for proxy server
        /// </summary>
        private static int proxyPort =80;
        /// <summary>
        /// User for proxy
        /// </summary>
        private static string proxyUser="";
        /// <summary>
        /// Password for proxy
        /// </summary>
        private static string proxyPwd = "";
        #endregion

        #region Constructor & instance read
        /// <summary>
        /// Read configuration
        /// </summary>
        private SourceReader()
        {
            IWork();
            logger.Debug(Constants.LoggerMessages.WebSourceCreate);
            arrive = Utils.ReadConfigurationKey(Constants.AppSetings.HttpArrive);
            departure = Utils.ReadConfigurationKey(Constants.AppSetings.HttpDeparture);
            user = Utils.ReadConfigurationKey(Constants.AppSetings.HttpUser);
            if (string.IsNullOrWhiteSpace(user))
                logger.Info(Constants.LoggerMessages.WebSourceUserNotUse);
            else
                logger.Info(Constants.LoggerMessages.WebSourceUserUse, user);
            pwd = Utils.ReadConfigurationKey(Constants.AppSetings.HttpPwd);
            proxyServer = Utils.ReadConfigurationKey(Constants.AppSetings.ProxyServer);
            proxyUser = Utils.ReadConfigurationKey(Constants.AppSetings.ProxyUser);
            proxyPwd = Utils.ReadConfigurationKey(Constants.AppSetings.ProxyPwd);
            proxyPort = Utils.ConvertStringToInt(Utils.ReadConfigurationKey(Constants.AppSetings.ProxyPort), 8080);
            INotWork();
        }

        /// <summary>
        /// Return instance of this singletone class
        /// </summary>
        public static SourceReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    WaitForAvaibleToWork();
                    _instance = new SourceReader();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Inform that system now work or not
        /// </summary>
        public bool IsWork { get { return _isWork; } }

        /// <summary>
        /// Switch to work
        /// </summary>
        private void IWork() { _isWork = true; }
        /// <summary>
        /// Swtich to not work
        /// </summary>
        private void INotWork() { _isWork = false; }
        /// <summary>
        /// System wait for when is avaible for next work
        /// </summary>
        public static void WaitForAvaibleToWork()
        {
            logger.Debug(Constants.LoggerMessages.WebSourceNowWork);
            int loop = 2;
            while (_isWork)
            {
                System.Threading.Thread.Sleep(Constants.CommonConst.ThredSleepTime);
                loop--;
                if (loop <= 0)
                {
                    logger.Debug(Constants.ExceptionMessages.TimeToWaitIsExpire);
                    break;
                }
            }
        }
        public void WaitForAvaibleToWorkInstance()
        {
            WaitForAvaibleToWork();
        }
        #endregion

        #region Read data to memory stream
        /// <summary>
        /// Test and reurn memory stream from Arrive server.
        /// Throw exception when server not respond
        /// </summary>
        /// <exception cref="WebException"></exception>
        /// <returns>Data read from server</returns>
        public string ReadArrive()
        {
            if (IsArriveServerAlive())
            {
                return ReadService(arrive);
            }
            else
            {
                logger.Warn(Constants.ExceptionMessages.ServerNotRespond, arrive);
                throw new WebException(string.Format(Constants.ExceptionMessages.ServerNotRespond, arrive));
            }
        }
        /// <summary>
        /// Test and reurn memory stream from Departure server.
        /// Throw exception when server not respond
        /// </summary>
        /// <exception cref="WebException"></exception>
        /// <returns></returns>
        public string ReadDeparture()
        {
            if (IsDepartureServerAlive())
            {
                return ReadService(departure);
            }
            else
            {
                logger.Warn(Constants.ExceptionMessages.ServerNotRespond, departure);
                throw new WebException(string.Format(Constants.ExceptionMessages.ServerNotRespond, departure));
            }
        }
        /// <summary>
        /// Internal method.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal string ReadService(string name)
        {
            if (IsWork)
            {
                logger.Warn(Constants.LoggerMessages.WebSourceNowWork);
                throw new WebException(Constants.ExceptionMessages.ServerNowWork);
            }
            IWork();
            using (WebClient wc = new WebClient())
            {
                //wc.Proxy = WebRequest.GetSystemWebProxy();
                if (!string.IsNullOrWhiteSpace(proxyServer))
                {
                    WebProxy transferProxy = new WebProxy(proxyServer, proxyPort);
                    transferProxy.BypassProxyOnLocal = true;
                    logger.Debug(Constants.LoggerMessages.ProxyUsing, proxyServer, proxyPort);
                    if ((!string.IsNullOrWhiteSpace(proxyUser)) && (!string.IsNullOrWhiteSpace(proxyPwd))) // user & pwd must be defined
                    {
                        transferProxy.Credentials = new NetworkCredential(proxyUser, proxyPwd);
                        logger.Debug(Constants.LoggerMessages.ProxyUsingUser, proxyUser);
                    }
                    wc.Proxy = transferProxy;
                }
                wc.Encoding = Encoding.UTF8;
                wc.UseDefaultCredentials = true;
                if (!string.IsNullOrWhiteSpace(user))
                {
                    wc.Credentials = new NetworkCredential(user, pwd);
                    logger.Debug(Constants.LoggerMessages.WebSourceUserUse, user);
                }
                string data = "";
                try
                {
                    data = wc.DownloadString(name);
                    INotWork();
                }
                catch (Exception e)
                {
                    INotWork();
                    logger.Error(Constants.LoggerMessages.WebSourceError, name);
                    logger.Debug(e);
                    throw new WebException(string.Format(Constants.LoggerMessages.WebSourceError, name), e);
                }
                return data;
            }
        }

        #endregion

        #region Test server to alive
        /// <summary>
        /// Test that server with arrival table is alive or not
        /// </summary>
        /// <returns></returns>
        public bool IsArriveServerAlive()
        {
            return true; // IsServerAlive(arrive);
        }
        /// <summary>
        /// Test that server with departure table is alive or not
        /// </summary>
        /// <returns></returns>
        public bool IsDepartureServerAlive()
        {
            return true; // IsServerAlive(departure);
        }

        /// <summary>
        /// Internal funcion for testing that server is response for HEAD
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        internal bool IsServerAlive(string Url)
        {
            if (IsWork)
            {
                logger.Warn(Constants.LoggerMessages.WebSourceNowWork);
                return false;
            }
            IWork();
            string Message = string.Empty;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Credentials = new NetworkCredential(user, pwd);
            request.Proxy = WebRequest.GetSystemWebProxy();
            request.Method = "HEAD";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    logger.Debug(Constants.LoggerMessages.WebSourceHeadDate, response.LastModified, Url);
                }
            }
            catch (WebException ex)
            {
                Message += ((Message.Length > 0) ? "\n" : "") + ex.Message;
                logger.Debug(ex);
            }
            INotWork();
            return (Message.Length == 0);
        }
        #endregion
    }
}