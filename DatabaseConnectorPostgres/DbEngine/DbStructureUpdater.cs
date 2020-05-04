using System;

namespace DatabaseConnectorPostgres.DbEngine
{
    internal class DbStructureUpdater
    {
        private DbEngine _dbEngine;
        public DbStructureUpdater(ref DbEngine refDbEngine)
        {
            _dbEngine = null;
            _dbEngine = refDbEngine;
        }


        internal void Run()
        {
            this.CreateVersion_1();
        }

        private void CreateVersion_1()
        {
            bool flag = this._dbEngine.Version > 0L;
        }
    }
}