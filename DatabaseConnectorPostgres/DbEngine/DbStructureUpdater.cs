using System;

namespace DatabaseConnectorPostgres.DbEngine
{
    internal class DbStructureUpdater
    {
        private DbEngine _dbEngine;
        public DbStructureUpdater(DbEngine refDbEngine)
        {
            _dbEngine = null;
            _dbEngine = refDbEngine;
        }


        internal void Run()
        {
            CreateVersion_1();
        }

        private void CreateVersion_1()
        {
            bool flag = _dbEngine.Version > 0L;
        }
    }
}