using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbHelper
	{
		public class DbDataReader : IDisposable
		{
			private bool disposedValue;
			private System.Data.Common.DbDataReader _dataReader;
			private DbConnection _connection;
			private ConnectionState _connectionLastState;
			protected virtual void Dispose(bool disposing)
			{
				bool flag = !disposedValue;
				if (flag)
				{
					if (disposing)
					{
						_dataReader.Dispose();
						bool flag2 = _connectionLastState == ConnectionState.Closed;
						if (flag2)
						{
							_connection.Close();
						}
					}
				}
				disposedValue = true;
			}
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			public DbDataReader(ref DbConnection refConnection, string sqlCmd)
			{
				this._connection = refConnection;
				DbCommand dbCommand = refConnection.CreateCommand();
				dbCommand.CommandText = sqlCmd;
				this._connectionLastState = this._connection.State;
				bool flag = this._connection.State != ConnectionState.Open;
				if (flag)
				{
					this._connection.Open();
				}
				this._dataReader = dbCommand.ExecuteReader();
			}
			public bool Read()
			{
				return _dataReader.Read();
			}
			public string GetString(int columnNumber)
			{
				return _dataReader.GetString(columnNumber);
			}
			public long GetLong(int columnNumber)
			{
				return _dataReader.GetInt64(columnNumber);
			}
			public DateTime GetDateTime(int columnNumber)
			{
				return _dataReader.GetDateTime(columnNumber);
			}
			public double GetDouble(int columnNumber)
			{
				return _dataReader.GetDouble(columnNumber);
			}
			public bool GetBoolean(int columnNumber)
			{
				return _dataReader.GetBoolean(columnNumber);
			}
			public Type GetFieldType(int columnNumber)
			{
				return _dataReader.GetFieldType(columnNumber);
			}
			public object GetObject(int columnNumber)
			{
				bool flag = IsDbNull(columnNumber);
				object result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = _dataReader.GetValue(columnNumber);
				}
				return result;
			}
			public bool IsDbNull(int columnNumber)
			{
				return _dataReader.IsDBNull(columnNumber);
			}
			public string GetString(string columnName)
			{
				return GetString(_dataReader.GetOrdinal(columnName));
			}
			public long GetLong(string columnName)
			{
				return GetLong(_dataReader.GetOrdinal(columnName));
			}
			public DateTime GetDateTime(string columnName)
			{
				return GetDateTime(_dataReader.GetOrdinal(columnName));
			}
			public double GetDouble(string columnName)
			{
				return GetDouble(this._dataReader.GetOrdinal(columnName));
			}
			public bool GetBoolean(string columnName)
			{
				return this.GetBoolean(this._dataReader.GetOrdinal(columnName));
			}
			public Type GetFieldType(string columnName)
			{
				return this.GetFieldType(this._dataReader.GetOrdinal(columnName));
			}
			public object GetObject(string columnName)
			{
				return this.GetObject(this._dataReader.GetOrdinal(columnName));
			}
			public bool IsDbNull(string columnName)
			{
				return this.IsDbNull(this._dataReader.GetOrdinal(columnName));
			}
		}
		public class DbSqlExecuter
		{
			public static bool Execute(DbConnection refConnection, string sqlCmd)
			{
				DbCommand dbCommand = refConnection.CreateCommand();
				DbParameter dbParameter = dbCommand.CreateParameter();
				dbCommand.CommandText = sqlCmd;
				ConnectionState state = refConnection.State;
				bool flag = refConnection.State != ConnectionState.Open;
				if (flag)
				{
					refConnection.Open();
				}
				dbCommand.ExecuteNonQuery();
				bool flag2 = state == ConnectionState.Closed;
				if (flag2)
				{
					refConnection.Close();
				}
				return true;
			}
		}
		public class DbSqlReader
		{
			private static object GetObject(DbConnection refConnection, string sqlCmd)
			{
				DbCommand dbCommand = refConnection.CreateCommand();
				dbCommand.CommandText = sqlCmd;
				ConnectionState state = refConnection.State;
				bool flag = refConnection.State != ConnectionState.Open;
				if (flag)
				{
					refConnection.Open();
				}
				object objectValue = RuntimeHelpers.GetObjectValue(dbCommand.ExecuteScalar());
				bool flag2 = state == ConnectionState.Closed;
				if (flag2)
				{
					refConnection.Close();
				}
				return objectValue;
			}
			public static long GetLong(ref DbConnection refConnection, string sqlCmd)
			{
				return Convert.ToInt64(RuntimeHelpers.GetObjectValue(DbHelper.DbSqlReader.GetObject(refConnection, sqlCmd)));
			}
			public static string GetString(ref DbConnection refConnection, string sqlCmd)
			{
				return Convert.ToString(RuntimeHelpers.GetObjectValue(DbHelper.DbSqlReader.GetObject(refConnection, sqlCmd)));
			}
		}
	}
}
