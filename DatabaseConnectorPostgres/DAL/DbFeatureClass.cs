
using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Exceptions;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DbEngDatabaseConnectorPostgresine.DAL
{
	public class DbFeatureClass
	{
		private NpgsqlConnection _connection;

		public string Name { get; }
		public DbFeatureClassAttributes Attributes { get; private set; }
		public NpgsqlConnection Connection
		{
			get
			{
				return _connection;
			}
		}

		async public static Task<DbFeatureClass> BuildDbFeatureClassAsync(NpgsqlConnection refConnection, string tableName)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(refConnection, tableName);
			await dbFeatureClass.InitFeatureClassAsync();
			return dbFeatureClass;
		}

		public DbFeatureClass(NpgsqlConnection refConnection, string valName)
		{
			_connection = null;
			Name = "";
			Attributes = null;
			_connection = refConnection;
			Name = valName;
		}
		private async Task InitFeatureClassAsync()
		{
			await InitAttribbutesAsync();
		}

		private async Task InitAttribbutesAsync()
		{
			Attributes = await DbFeatureClassAttributes.GetFeatureClassAttributesAsync(_connection, Name);
		}


		public async Task<DbFeature> GetFeature(long id)
		{
			List<DbFeature> features = await GetFeatures(string.Format("id = {0}", id), "");
			bool flag = features != null && features.Count == 1;
			DbFeature result;
			if (flag)
			{
				result = features[0];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public async Task<List<DbFeature>> GetFeatures(string whereStatement = "", string orderStatement = "")
		{
			List<DbFeature> list = new List<DbFeature>();
			string selectString = DbSqlStringBuilder.GetSelectString(Name, Attributes.ToNameArray(), whereStatement, orderStatement);

			await using (var cmd = new NpgsqlCommand(selectString, _connection))
			await using (var reader = await cmd.ExecuteReaderAsync())
				while (await reader.ReadAsync())
				{
					List<DbFeatureAttribute> list2 = new List<DbFeatureAttribute>();

					try
					{
						IEnumerator enumerator = Attributes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							DbFeatureClassAttribute dbFeatureClassAttribute = (DbFeatureClassAttribute)enumerator.Current;

							var x = dbFeatureClassAttribute.Name;

							DbFeatureAttribute item = new DbFeatureAttribute(dbFeatureClassAttribute, reader.GetValue(dbFeatureClassAttribute.Name));
							list2.Add(item);
						}
					}
					catch
					{
						throw new GetFeaturesException();
					}
					List<DbFeature> arg_B0_0 = list;
					DbFeatureClass dbFeatureClass = this;
					arg_B0_0.Add(new DbFeature(dbFeatureClass, list2));
				}
            return list;
		}
		public long GetFeaturesCount()
		{
			//todo
			return 0;
			//string selectString = DbSqlStringBuilder.GetSelectString(Name, new string[]
			//{
			//	"count(id)"
			//}, "", "");
			//return DbHelper.DbSqlReader.GetLong(_connection, selectString);
		}
		public DbFeature CreateFeature()
		{
			List<DbFeatureAttribute> list = new List<DbFeatureAttribute>();
			try
			{
				IEnumerator enumerator = Attributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DbFeatureClassAttribute dbFeatureClassAttribute = (DbFeatureClassAttribute)enumerator.Current;
					DbFeatureAttribute item = new DbFeatureAttribute(dbFeatureClassAttribute, "");
					list.Add(item);
				}
			}
			catch
			{
				throw new CreateFeatureException();
			}

			DbFeatureClass dbFeatureClass = this;
			return new DbFeature(dbFeatureClass, list);
		}
		public void InsertFeature(DbFeature feature)
		{
			//bool flag = !feature.IsInserted;
			//if (flag)
			//{
			//	string insertRowString = DbSqlStringBuilder.GetInsertRowString(this.Name, feature.Attributes);
			//	bool flag2 = this._connection.State != ConnectionState.Open;
			//	if (flag2)
			//	{
			//		this._connection.Open();
			//	}
			//	bool flag3 = DbHelper.DbSqlExecuter.Execute(this._connection, insertRowString);
			//	if (flag3)
			//	{
			//		string selectLastIdString = DbSqlStringBuilder.GetSelectLastIdString(this.Name);
			//		feature.Attributes["ID".ToLower()].Value = DbHelper.DbSqlReader.GetLong(this._connection, selectLastIdString);
			//		try
			//		{
			//			IEnumerator enumerator = feature.Attributes.GetEnumerator();
			//			while (enumerator.MoveNext())
			//			{
			//				DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)enumerator.Current;
			//				bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
			//				if (needsUpdate)
			//				{
			//					dbFeatureAttribute.UpdatedExecuted();
			//				}
			//			}
			//		}
			//		catch (Exception ex)
			//		{
			//			Console.WriteLine(ex);
			//			throw new InsertFeatureException();
			//		}
			//	}
			//	this._connection.Close();
			//}
		}
		public void InsertFeatures(List<DbFeature> features)
		{
			bool flag = features != null;
			if (flag)
			{
				try
				{
					List<DbFeature>.Enumerator enumerator = features.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DbFeature current = enumerator.Current;
						InsertFeature(current);
					}
				}
				catch
				{
					throw new InsertFeaturesException();
				}
			}
		}
		public void UpdateFeature(DbFeature feature)
		{
			//bool flag = feature.IsInserted && feature.NeedsUpdate;
			//if (flag)
			//{
			//	string updateRowString = DbSqlStringBuilder.GetUpdateRowString(this.Name, feature.Attributes, string.Format("id = {0}", feature.ID));
			//	bool flag2 = DbHelper.DbSqlExecuter.Execute(this._connection, updateRowString);
			//	if (flag2)
			//	{
			//		try
			//		{
			//			IEnumerator enumerator = feature.Attributes.GetEnumerator();
			//			while (enumerator.MoveNext())
			//			{
			//				DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)enumerator.Current;
			//				bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
			//				if (needsUpdate)
			//				{
			//					dbFeatureAttribute.UpdatedExecuted();
			//				}
			//			}
			//		}
			//		catch
			//		{
			//			throw new UpdateFeatureException();
			//		}
			//	}
			//}
		}
		public void UpdateFeatures(List<DbFeature> features)
		{
			bool flag = features != null;
			if (flag)
			{
				try
				{
					List<DbFeature>.Enumerator enumerator = features.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DbFeature current = enumerator.Current;
						UpdateFeature(current);
					}
				}
				catch
				{
					throw new UpdateFeaturesException();
				}
			}
		}
		public void DeleteFeature(DbFeature feature)
		{
			//bool isInserted = feature.IsInserted;
			//if (isInserted)
			//{
			//	string deleteRowString = DbSqlStringBuilder.GetDeleteRowString(Name, string.Format("id = {0}", feature.ID));
			//	DbHelper.DbSqlExecuter.Execute(_connection, deleteRowString);
			//}
		}
		public void DeleteFeatures(List<DbFeature> features)
		{
			bool flag = features != null;
			if (flag)
			{

				try
				{
					List<DbFeature>.Enumerator enumerator = features.GetEnumerator();
					while (enumerator.MoveNext())
					{
						DbFeature current = enumerator.Current;
						DeleteFeature(current);
					}
				}
				catch
				{
					throw new DeleteFeaturesException();
				}

			}
		}
	}
}
