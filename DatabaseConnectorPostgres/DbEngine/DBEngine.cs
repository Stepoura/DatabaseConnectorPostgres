using DatabaseConnectorPostgres.DAL;
using DbEngDatabaseConnectorPostgresine.DAL;
using Devart.Data.PostgreSql;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DatabaseConnectorPostgres.DbEngine
{
    public class DbEngine : IDisposable
    {

        private string DatabaseName
        {
            get
            {
                return "postgres";
            }
        }

        private static DbEngine _instance = null;
        private DbFeatureClasses _featureClasses;
        private bool _installComplete;
        private DbFeature _SettingsFeature;
        private bool disposedValue;

        public static DbEngine Instance
        {
            get
            {
                bool flag = _instance == null;
                if (flag)
                {
                    _instance = new DbEngine();
                }
                return _instance;
            }
        }

        public DbFeature SettingsFeature
        {
            get
            {
                bool flag = _SettingsFeature == null || _SettingsFeature.Attributes == null;
                if (flag)
                {
                    InitFeatureClasses();
                    _SettingsFeature = FeatureClasses["settings"].GetFeature(1L);
                }
                return _SettingsFeature;
            }
        }

        public long Version
        {
            get
            {
                long result;
                try
                {
                    result = SettingsFeature.Attributes["db_version"].ValueLong;
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
                this.SettingsFeature.Attributes["db_version"].Value = value;
                DbFeatureClass arg_36_0 = SettingsFeature.FeatureClass;
                DbFeature settingsFeature = SettingsFeature;
                arg_36_0.UpdateFeature(ref settingsFeature);
            }
        }

        private DbEngine()
        {
            _featureClasses = null;
            _installComplete = false;
            _SettingsFeature = null;
            ConnectToDatabase();
            Install();
        }

        public DbConnection Connection
        {
            get
            {
                PgSqlConnection pgSqlConnection = new PgSqlConnection(this.ConnectionString);
                pgSqlConnection.UserId = "postgres";
                pgSqlConnection.Password = "pg";
                pgSqlConnection.Host = "localhost";
                pgSqlConnection.Database = DatabaseName;
                pgSqlConnection.Unicode = true;
                return pgSqlConnection;

            }
        }

        private string ConnectionString
        {
            get
            {
                return string.Format("Default Command Timeout=0;Force IPv4=true", new object[0]);
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

        private void ConnectToDatabase()
        {
            Connection.Open();
            InitFeatureClasses();
            bool flag = this._featureClasses != null && _featureClasses.Count > 0;
            if (flag)
            {
                _installComplete = true;
            }
            else
            {
                RunDatabaseUpdate();
                Reload();
                Connection.Close();
                Connection.Dispose();
                _installComplete = true;
            }
        }

        private void InitFeatureClasses()
        {
            DbConnection connection = this.Connection;
            this._featureClasses = DbFeatureClasses.GetFeatureClasses(ref connection);
        }

        public void Install()
        {
            bool installComplete = this.InstallComplete;
            if (!installComplete)
            {
                this.Connection.Open();
                this.InitFeatureClasses();
                DbFeatureClass dbFeatureClass = this.FeatureClasses.CreateFeatureClass("settings");
                dbFeatureClass.Attributes.CreateAttribute("db_version", DbFeatureClassAttribute.DataTypes.type_int, true, 0L, 0L);
                DbFeature dbFeature = dbFeatureClass.CreateFeature();
                dbFeature.Attributes["db_version"].Value = 0;
                dbFeatureClass.InsertFeature(ref dbFeature);
                this.RunDatabaseUpdate();
                this.Reload();
                this.Connection.Close();
                this.Connection.Dispose();
            }
        }

        private void RunDatabaseUpdate()
        {
            DbEngine dbEngine = this;
            DbStructureUpdater dbStructureUpdater = new DbStructureUpdater(ref dbEngine);
            dbStructureUpdater.Run();
        }

        private void Reload()
        {
            _SettingsFeature = null;
        }

        public void Refresh()
        {
            InitFeatureClasses();
        }

        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
