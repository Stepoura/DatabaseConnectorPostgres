using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Exceptions;
using DbEngDatabaseConnectorPostgresine.DAL;
using log4net;
using Microsoft.VisualBasic.CompilerServices;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.DbEngine
{
    public class DbEngine: IDisposable
    {

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string DatabaseName
        {
            get
            {
                return "postgres";
            }
        }

        private string UserId
        {
            get
            {
                return "postgres";
            }
        }

        private string Password
        {
            get
            {
                return "pg";
            }
        }

        private string Host
        {
            get
            {
                return "Localhost";
            }
        }

        public DbFeatureClasses FeatureClasses
        {
            get
            {
                return _featureClasses;
            }
        }
        public bool InstallComplete
        {
            get
            {
                return _installComplete;
            }
        }

        private static DbEngine _instance = null;
        private DbFeatureClasses _featureClasses;
        private bool _installComplete;
        private DbFeature _SettingsFeature;
        private NpgsqlConnection _connection;

        public static async Task<DbEngine> Instance()
        {
            bool flag = _instance == null;
            if (flag)
            {
                _instance = await BuildDbEngineAsync();
            }
            return _instance;
        }

        public async Task<DbFeature> SettingsFeature()
        {
            bool flag = _SettingsFeature == null || _SettingsFeature.Attributes == null;
            if (flag)
            {
                await InitFeatureClassesAsync();
                _SettingsFeature = await FeatureClasses["settings"].GetFeature(1L);
            }
            return _SettingsFeature;
        }

        public long Version
        {
            //todo
            get
            {
                long result;
                try
                {
                    result = _SettingsFeature.Attributes["db_version"].ValueLong;
                }
                catch (Exception expr_1F)
                {
                    ProjectData.SetProjectError(expr_1F);
                    result = 0L;
                    ProjectData.ClearProjectError();
                }
                return result;
            }
            private set
            {
                _SettingsFeature.Attributes["db_version"].Value = value;
                DbFeatureClass arg_36_0 = _SettingsFeature.FeatureClass;
                DbFeature settingsFeature = _SettingsFeature;
                arg_36_0.UpdateFeature(settingsFeature);
            }
        }

        async public static Task<DbEngine> BuildDbEngineAsync()
        {
            DbEngine engine = new DbEngine();
            await engine.ConnectToDatabase();
            await engine.Install();
            return engine;
        }

        private DbEngine()
        {
            _featureClasses = null;
            _installComplete = false;
            _SettingsFeature = null;
        }

        private string GetConnectionString()
        {
            StringBuilder dbConnectionString = new StringBuilder();
            dbConnectionString.Append("Host=" + Host + ";");
            dbConnectionString.Append("Username=" + UserId + ";");
            dbConnectionString.Append("Password=" + Password + ";");
            dbConnectionString.Append("Database=" + DatabaseName + ";");
            return dbConnectionString.ToString();
        }

        public NpgsqlConnection Connection
        {
            get
            {
                bool flag = _connection == null;
                if (flag)
                {
                    string connString = GetConnectionString();
                    _connection = new NpgsqlConnection(connString);
                }
                return _connection;
            }
        }

        private async Task ConnectToDatabase()
        {
            var conn = Connection;
            await conn.OpenAsync();
            await InitFeatureClassesAsync();
            bool flag = _featureClasses != null && _featureClasses.Count > 0;
            if (flag)
            {
                _installComplete = true;
            }
            else
            {
                RunDatabaseUpdate();
                Reload();
                await conn.CloseAsync();
                await conn.DisposeAsync();
                _installComplete = true;
            }
        }

        private async Task InitFeatureClassesAsync()
        {
            _featureClasses = await DbFeatureClasses.GetFeatureClasses(Connection);
        }

        public async Task Install()
        {
            bool installComplete = InstallComplete;
            if (!installComplete)
            {
                var conn = Connection;
                await conn.OpenAsync();
                await InitFeatureClassesAsync();
                DbFeatureClass dbFeatureClass = FeatureClasses.CreateFeatureClass("settings");
                dbFeatureClass.Attributes.CreateAttribute("db_version", DbFeatureClassAttribute.DataTypes.type_int, true, 0L, 0L);
                DbFeature dbFeature = dbFeatureClass.CreateFeature();
                dbFeature.Attributes["db_version"].Value = 0;
                dbFeatureClass.InsertFeature(dbFeature);
                RunDatabaseUpdate();
                Reload();
                await conn.CloseAsync();
                await conn.DisposeAsync();
            }
        }

        private void RunDatabaseUpdate()
        {
            DbEngine dbEngine = this;
            DbStructureUpdater dbStructureUpdater = new DbStructureUpdater(dbEngine);
            dbStructureUpdater.Run();
        }

        private void Reload()
        {
            _SettingsFeature = null;
        }

        public async Task Refresh()
        {
            await InitFeatureClassesAsync();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
