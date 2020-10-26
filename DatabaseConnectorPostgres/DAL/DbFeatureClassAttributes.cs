
using DatabaseConnectorPostgres.Exceptions;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureClassAttributes : IEnumerable
	{
		private List<DbFeatureClassAttribute> _internalFeatureClassAttributeList;
		private NpgsqlConnection _connection;
		private string _tableName;
		public DbFeatureClassAttribute this[string attributeName]
		{
			get
			{
				DbFeatureClassAttribute result;

				try
				{
					foreach (var entry in _internalFeatureClassAttributeList)
					{
						DbFeatureClassAttribute current = entry;
						bool flag = current.Name.Equals(attributeName);
						if (flag)
						{
							result = current;
							return result;
						}
					}
				}
				catch
				{
					throw new DbFeatureClassAttributeException();
				}
				result = null;
				return result;
			}
		}

		public DbFeatureClassAttribute this[int index]
		{
			get
			{
				bool flag = index > checked(Count - 1);
				DbFeatureClassAttribute result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = _internalFeatureClassAttributeList[index];
				}
				return result;
			}
		}

		public int Count
		{
			get
			{
				return _internalFeatureClassAttributeList.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _internalFeatureClassAttributeList.GetEnumerator();
		}

		public string[] ToNameArray()
		{
			List<string> list = new List<string>();
			try
			{
				foreach (var entry in _internalFeatureClassAttributeList)
				{
					DbFeatureClassAttribute current = entry;
					list.Add(current.Name);
				}
			}
			catch
			{
				throw new ToNameArrayException();
			}
			return list.ToArray();
		}

		public static async Task<DbFeatureClassAttributes> GetFeatureClassAttributesAsync(NpgsqlConnection refConnection, string tableName)
		{
			var dbFeartureAttributes = await BuildDbFeatureClassAttributesAsync(refConnection, tableName);
			return dbFeartureAttributes;
		}

		async public static Task<DbFeatureClassAttributes> BuildDbFeatureClassAttributesAsync(NpgsqlConnection refConnection, string tableName)
		{
			DbFeatureClassAttributes dbFeatureClassAttributes = new DbFeatureClassAttributes(refConnection, tableName);
			await dbFeatureClassAttributes.InitFeatureClassAttributeList();
			return dbFeatureClassAttributes;
		}

		public DbFeatureClassAttributes(NpgsqlConnection refConnection, string tableName)
		{
			_internalFeatureClassAttributeList = new List<DbFeatureClassAttribute>();
			_connection = null;
			_tableName = "";
			_connection = refConnection;
			_tableName = tableName;
		}

		private async Task InitFeatureClassAttributeList()
		{
			await using (var cmd = new NpgsqlCommand(string.Format("SELECT table_catalog, column_name, data_type, CASE WHEN is_nullable = 'YES' THEN 0 ELSE 1 end As nullable from information_schema.columns where table_name = '{0}'", _tableName), _connection))
			await using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
                {
					DbFeatureClassAttribute item = new DbFeatureClassAttribute(reader.GetString(1), reader.GetString(2), reader.GetInt16(3), 0L, 0L);
					_internalFeatureClassAttributeList.Add(item);
				}
		}

		public DbFeatureClassAttribute CreateAttribute(string name, DbFeatureClassAttribute.DataTypes dataType, bool nullable, long length = 0L, long precision = 0L)
		{
			//bool flag = length == 0L;
			//if (flag)
			//{
			//	bool flag2 = dataType == DbFeatureClassAttribute.DataTypes.type_int;
			//	if (flag2)
			//	{
			//		length = 10L;
			//	}
			//	else
			//	{
			//		bool flag3 = dataType == DbFeatureClassAttribute.DataTypes.type_nvarchar;
			//		if (flag3)
			//		{
			//			length = 255L;
			//		}
			//		else
			//		{
			//			bool flag4 = dataType == DbFeatureClassAttribute.DataTypes.type_serial;
			//			if (flag4)
			//			{
			//				length = 10L;
			//			}
			//		}
			//	}
			//}
			//DbFeatureClassAttribute dbFeatureClassAttribute = new DbFeatureClassAttribute(name, dataType, nullable, length, precision);
			//string alterTableCreateColumnString = DbSqlStringBuilder.GetAlterTableCreateColumnString(_tableName, dbFeatureClassAttribute);
			//bool flag5 = !DbHelper.DbSqlExecuter.Execute(_connection, alterTableCreateColumnString);
			//DbFeatureClassAttribute result;
			//if (flag5)
			//{
			//	result = null;
			//}
			//else
			//{
			//	_internalFeatureClassAttributeList.Add(dbFeatureClassAttribute);
			//	result = dbFeatureClassAttribute;
			//}
			//return result;
			return null;
		}

		public void DropAttribute(string attributeName)
		{
			DropAttribute(this[attributeName]);
		}

		public void DropAttribute(DbFeatureClassAttribute ftClassAttribute)
		{
			//string alterTableDropColumnString = DbSqlStringBuilder.GetAlterTableDropColumnString(_tableName, ftClassAttribute.Name);
			//bool flag = DbHelper.DbSqlExecuter.Execute(_connection, alterTableDropColumnString);
			//if (flag)
			//{
			//	_internalFeatureClassAttributeList.Remove(ftClassAttribute);
			//}
		}
	}
}
