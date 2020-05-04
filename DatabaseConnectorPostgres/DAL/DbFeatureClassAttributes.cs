
using DatabaseConnectorPostgres.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureClassAttributes : IEnumerable
	{
		private List<DbFeatureClassAttribute> _internalFeatureClassAttributeList;
		private DbConnection _connection;
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
				bool flag = index > checked(this.Count - 1);
				DbFeatureClassAttribute result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this._internalFeatureClassAttributeList[index];
				}
				return result;
			}
		}
		public int Count
		{
			get
			{
				return this._internalFeatureClassAttributeList.Count;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)this._internalFeatureClassAttributeList.GetEnumerator();
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
		public static DbFeatureClassAttributes GetFeatureClassAttributes(ref DbConnection refConnection, string tableName)
		{
			return new DbFeatureClassAttributes(ref refConnection, tableName);
		}
		public DbFeatureClassAttributes(ref DbConnection refConnection, string tableName)
		{
			this._internalFeatureClassAttributeList = new List<DbFeatureClassAttribute>();
			this._connection = null;
			this._tableName = "";
			this._connection = refConnection;
			this._tableName = tableName;
			this.InitFeatureClassAttributeList();
		}
		private void InitFeatureClassAttributeList()
		{
			this._internalFeatureClassAttributeList.Clear();
			using (DbHelper.DbDataReader dbDataReader = new DbHelper.DbDataReader(ref this._connection, string.Format("SELECT table_catalog, column_name, data_type,CASE WHEN is_nullable = 'YES' THEN 0 ELSE 1 end As nullable from information_schema.columns where table_name = '{0}'", this._tableName)))
			{
				while (dbDataReader.Read())
				{
					DbFeatureClassAttribute item = new DbFeatureClassAttribute(dbDataReader.GetString(1), dbDataReader.GetString(2), dbDataReader.GetBoolean(3), 0L, 0L);
					this._internalFeatureClassAttributeList.Add(item);
				}
			}
		}
		public DbFeatureClassAttribute CreateAttribute(string name, DbFeatureClassAttribute.DataTypes dataType, bool nullable, long length = 0L, long precision = 0L)
		{
			bool flag = length == 0L;
			if (flag)
			{
				bool flag2 = dataType == DbFeatureClassAttribute.DataTypes.type_int;
				if (flag2)
				{
					length = 10L;
				}
				else
				{
					bool flag3 = dataType == DbFeatureClassAttribute.DataTypes.type_nvarchar;
					if (flag3)
					{
						length = 255L;
					}
					else
					{
						bool flag4 = dataType == DbFeatureClassAttribute.DataTypes.type_serial;
						if (flag4)
						{
							length = 10L;
						}
					}
				}
			}
			DbFeatureClassAttribute dbFeatureClassAttribute = new DbFeatureClassAttribute(name, dataType, nullable, length, precision);
			string alterTableCreateColumnString = DbSqlStringBuilder.GetAlterTableCreateColumnString(this._tableName, dbFeatureClassAttribute);
			bool flag5 = !DbHelper.DbSqlExecuter.Execute(_connection, alterTableCreateColumnString);
			DbFeatureClassAttribute result;
			if (flag5)
			{
				result = null;
			}
			else
			{
				this._internalFeatureClassAttributeList.Add(dbFeatureClassAttribute);
				result = dbFeatureClassAttribute;
			}
			return result;
		}
		public void DropAttribute(string attributeName)
		{
			this.DropAttribute(this[attributeName]);
		}
		public void DropAttribute(DbFeatureClassAttribute ftClassAttribute)
		{
			string alterTableDropColumnString = DbSqlStringBuilder.GetAlterTableDropColumnString(this._tableName, ftClassAttribute.Name);
			bool flag = DbHelper.DbSqlExecuter.Execute(_connection, alterTableDropColumnString);
			if (flag)
			{
				this._internalFeatureClassAttributeList.Remove(ftClassAttribute);
			}
		}
	}
}
