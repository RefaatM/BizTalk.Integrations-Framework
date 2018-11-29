using DE.DAXFSA.Framework.Core.Tracing;
using Insight.Database;
using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DE.DAXFSA.Framework.BizTalk.Configuration
{
    /// <summary>
    /// BizTalk configuration provider class.
    /// </summary>
    /// <remarks>
    /// The implementation uses the singleton pattern to prevent querying
    /// the BizTalk Management database multiple times.
    /// </remarks>
    public sealed class BizTalkConfig
    {
        #region Constants

        private const string BTS_ADMIN_REGISTRY_KEY = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration";
        private const string BTS_MGMT_DB_SERVER_REGISTRY_KEY = "MgmtDBServer";
        private const string BTS_MGMT_DB_NAME_REGISTRY_KEY = "MgmtDBName";
        private const string CONNECTION_STRING_FORMAT = "Data Source={0};Initial Catalog={1};Integrated Security=True";
        private const string GET_DATABASES_SQL = @"SELECT SubscriptionDBServerName, SubscriptionDBName, BamDBServerName, BamDBName, TrackingDBServerName, TrackingDBName FROM adm_Group";

        #endregion Constants

        #region Fields

        private string btsMgmtConnectionString;
        private string btsMsgBoxConnectionString;
        private string btsBamConnectionString;
        private string btsTrackingConnectionString;

        // allocate an instance of this class
        // since the constructor of the class is private, all
        // class users need to obtain an instance via the Instance property
        private static readonly BizTalkConfig instance = new BizTalkConfig();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Private constructor.
        /// </summary>
        private BizTalkConfig()
        {
            // register INsight database SQL provider
            Insight.Database.SqlInsightDbProvider.RegisterProvider();
            // initialize class
            Initialize();
        }

        #endregion Constructor

        #region Instance

        /// <summary>
        /// Gets an instance of the class.
        /// </summary>
        public static BizTalkConfig Instance
        {
            get { return instance; }
        }

        #endregion Instance

        #region Properties

        /// <summary>
        /// Gets the BizTalk Management connection string.
        /// </summary>
        public string ManagementConnectionString
        {
            get { return btsMgmtConnectionString; }
        }

        /// <summary>
        /// Gets the BizTalk MessageBox connection string.
        /// </summary>
        public string MsgBoxConnectionString
        {
            get { return btsMsgBoxConnectionString; }
        }

        /// <summary>
        /// Gets the BizTalk BAM connection string.
        /// </summary>
        public string BAMConnectionString
        {
            get { return btsBamConnectionString; }
        }

        /// <summary>
        /// Gets the BizTalk Tracking connection string.
        /// </summary>
        public string TrackingConnectionString
        {
            get { return btsTrackingConnectionString; }
        }

        #endregion Properties

        #region Helpers

        /// <summary>
        /// Reads the BizTalk configuration settings from the
        /// registry and the Management database.
        /// </summary>
        private void Initialize()
        {
            var callToken = TraceProvider.Logger.TraceIn();
            try
            {
                TraceProvider.Logger.TraceInfo("Retrieving BizTalk administration settings from registry...");
                // the BizTalk management database server and database names
                // are stored in the registry
                RegistryKey key = Registry.LocalMachine.OpenSubKey(BTS_ADMIN_REGISTRY_KEY);
                if (key != null)
                {
                    string bizTalkMgmtDBServer = key.GetValue(BTS_MGMT_DB_SERVER_REGISTRY_KEY) as string;
                    string bizTalkMgmtDBName = key.GetValue(BTS_MGMT_DB_NAME_REGISTRY_KEY) as string;
                    if (String.IsNullOrEmpty(bizTalkMgmtDBServer) == false &&
                        String.IsNullOrEmpty(bizTalkMgmtDBName) == false)
                    {
                        TraceProvider.Logger.TraceInfo("BizTalk ManagementDB Server: {0}, Database Name: {1}", bizTalkMgmtDBServer, bizTalkMgmtDBName);
                        // get the management db connection string
                        this.btsMgmtConnectionString = String.Format(CONNECTION_STRING_FORMAT, bizTalkMgmtDBServer, bizTalkMgmtDBName);
                        TraceProvider.Logger.TraceInfo("Retrieving additional BizTalk database settings...");
                        // using the management db connection string query for the other databases
                        // database instace
                        SqlConnectionStringBuilder database = new SqlConnectionStringBuilder(this.btsMgmtConnectionString);
                        dynamic databasesInfo = database.Connection().QuerySql(GET_DATABASES_SQL, Parameters.Empty).SingleOrDefault();
                        if (databasesInfo != null)
                        {
                            this.btsMsgBoxConnectionString = String.Format(CONNECTION_STRING_FORMAT, databasesInfo.SubscriptionDBServerName, databasesInfo.SubscriptionDBName);
                            this.btsBamConnectionString = String.Format(CONNECTION_STRING_FORMAT, databasesInfo.BamDBServerName, databasesInfo.BamDBName);
                            this.btsTrackingConnectionString = String.Format(CONNECTION_STRING_FORMAT, databasesInfo.TrackingDBServerName, databasesInfo.TrackingDBName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken);
            }
        }

        #endregion Helpers
    }
}