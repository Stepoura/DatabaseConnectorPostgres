using DatabaseConnectorPostgres.Exceptions;
using DbEngDatabaseConnectorPostgresine.DAL;
using Npgsql;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureClasses : IEnumerable
	{
		private List<DbFeatureClass> _internalFeatureClassList;
		private NpgsqlConnection _connection;
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
				bool flag = index > checked(Count - 1);
				DbFeatureClass result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = _internalFeatureClassList[index];
				}
				return result;
			}
		}
		public int Count
		{
			get
			{
				return _internalFeatureClassList.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _internalFeatureClassList.GetEnumerator();
		}

		public static async Task<DbFeatureClasses> GetFeatureClasses(NpgsqlConnection refConnection)
		{
			var dbFeatureClasses = await BuildDbFeatureClassesAsync(refConnection);
			return dbFeatureClasses;
		}

		async public static Task<DbFeatureClasses> BuildDbFeatureClassesAsync(NpgsqlConnection refConnection)
		{
			DbFeatureClasses dbDbFeatureClasses = new DbFeatureClasses(refConnection);
			await dbDbFeatureClasses.InitFeatureClassListAsync();
			return dbDbFeatureClasses;
		}

		public DbFeatureClasses(NpgsqlConnection refConnection)
		{
			_internalFeatureClassList = new List<DbFeatureClass>();
			_connection = null;
			_connection = refConnection;
		}

		private async Task InitFeatureClassListAsync()
		{
            _internalFeatureClassList.Clear();
            string selectString = DbSqlStringBuilder.GetSelectString("information_schema.tables", new string[]
            {
                "table_name"
            }, "table_schema = 'public' and table_type = 'BASE TABLE' ORDER BY table_schema,table_name", "");

			await using (var cmd = new NpgsqlCommand(selectString, _connection))
			await using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
				{
					DbFeatureClass item = new DbFeatureClass(_connection, reader.GetString(0));
					_internalFeatureClassList.Add(item);
				}
        }

		public DbFeatureClass CreateFeatureClass(string featureClassName)
		{
			//string createTableString = DbSqlStringBuilder.GetCreateTableString(featureClassName, new DbFeatureClassAttribute("ID".ToLower(), DbFeatureClassAttribute.DataTypes.type_serial, false, 0L, 0L));
			//bool flag = !DbHelper.DbSqlExecuter.Execute(this._connection, createTableString);
			//DbFeatureClass result;
			//if (flag)
			//{
			//	result = null;
			//}
			//else
			//{
			//	DbFeatureClass dbFeatureClass = new DbFeatureClass(this._connection, featureClassName);
			//	this._internalFeatureClassList.Add(dbFeatureClass);
			//	result = dbFeatureClass;
			//}
			//return result;
			return null;
		}

		public void DropFeatureClass(string featureClassName)
		{
			DropFeatureClass(this[featureClassName]);
		}

		public void DropFeatureClass(DbFeatureClass ftClass)
		{
			//string dropTableString = DbSqlStringBuilder.GetDropTableString(ftClass.Name);
			//bool flag = DbHelper.DbSqlExecuter.Execute(this._connection, dropTableString);
			//if (flag)
			//{
			//	this._internalFeatureClassList.Remove(ftClass);
			//}
		}
	}
}
