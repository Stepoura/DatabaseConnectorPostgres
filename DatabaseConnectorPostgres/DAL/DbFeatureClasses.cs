using DatabaseConnectorPostgres.Exceptions;
using DbEngDatabaseConnectorPostgresine.DAL;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureClasses : IEnumerable
	{
		private List<DbFeatureClass> _internalFeatureClassList;
		private DbConnection _connection;
		public DbFeatureClass this[string featureClassName]
		{
			get
			{
				DbFeatureClass result;

				try
				{
					foreach (var entry in _internalFeatureClassList)
					{
						DbFeatureClass current = entry;
						bool flag = current.Name.Equals(featureClassName);
						if (flag)
						{
							result = current;
							return result;
						}
					}
				}
				catch
				{
					throw new DbFeatureClassException();
				}
				result = null;
				return result;
			}
		}
		public DbFeatureClass this[int index]
		{
			get
			{
				bool flag = index > checked(this.Count - 1);
				DbFeatureClass result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this._internalFeatureClassList[index];
				}
				return result;
			}
		}
		public int Count
		{
			get
			{
				return this._internalFeatureClassList.Count;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)this._internalFeatureClassList.GetEnumerator();
		}
		public static DbFeatureClasses GetFeatureClasses(ref DbConnection refConnection)
		{
			return new DbFeatureClasses(ref refConnection);
		}
		public DbFeatureClasses(ref DbConnection refConnection)
		{
			this._internalFeatureClassList = new List<DbFeatureClass>();
			this._connection = null;
			this._connection = refConnection;
			this.InitFeatureClassList();
		}
		private void InitFeatureClassList()
		{
			this._internalFeatureClassList.Clear();
			string selectString = DbSqlStringBuilder.GetSelectString("information_schema.tables", new string[]
			{
				"table_name"
			}, "table_schema = 'public' and table_type = 'BASE TABLE' ORDER BY table_schema,table_name", "");
			using (DbHelper.DbDataReader dbDataReader = new DbHelper.DbDataReader(ref this._connection, selectString))
			{
				while (dbDataReader.Read())
				{
					DbFeatureClass item = new DbFeatureClass(this._connection, dbDataReader.GetString(0));
					this._internalFeatureClassList.Add(item);
				}
			}
		}
		public DbFeatureClass CreateFeatureClass(string featureClassName)
		{
			string createTableString = DbSqlStringBuilder.GetCreateTableString(featureClassName, new DbFeatureClassAttribute("ID".ToLower(), DbFeatureClassAttribute.DataTypes.type_serial, false, 0L, 0L));
			bool flag = !DbHelper.DbSqlExecuter.Execute(this._connection, createTableString);
			DbFeatureClass result;
			if (flag)
			{
				result = null;
			}
			else
			{
				DbFeatureClass dbFeatureClass = new DbFeatureClass(this._connection, featureClassName);
				this._internalFeatureClassList.Add(dbFeatureClass);
				result = dbFeatureClass;
			}
			return result;
		}
		public void DropFeatureClass(string featureClassName)
		{
			this.DropFeatureClass(this[featureClassName]);
		}
		public void DropFeatureClass(DbFeatureClass ftClass)
		{
			string dropTableString = DbSqlStringBuilder.GetDropTableString(ftClass.Name);
			bool flag = DbHelper.DbSqlExecuter.Execute(this._connection, dropTableString);
			if (flag)
			{
				this._internalFeatureClassList.Remove(ftClass);
			}
		}
	}
}
