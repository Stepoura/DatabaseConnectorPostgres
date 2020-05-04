
using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace DbEngDatabaseConnectorPostgresine.DAL
{
	public class DbFeatureClass: IDisposable
	{
		private DbConnection _connection;

		public string Name { get; }
		public DbFeatureClassAttributes Attributes { get; private set; }
		public DbConnection Connection
		{
			get
			{
				return _connection;
			}
		}
		public DbFeatureClass(DbConnection refConnection, string valName)
		{
			this._connection = null;
			this.Name = "";
			this.Attributes = null;
			this._connection = refConnection;
			this.Name = valName;
			this.InitFeatureClass();
		}
		private void InitFeatureClass()
		{
			InitAttribbutes();
		}
		private void InitAttribbutes()
		{
			Attributes = DbFeatureClassAttributes.GetFeatureClassAttributes(ref _connection, this.Name);
		}
		public DbFeature GetFeature(long id)
		{
			List<DbFeature> features = this.GetFeatures(string.Format("id = {0}", id), "");
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
		public List<DbFeature> GetFeatures(string whereStatement = "", string orderStatement = "")
		{
			List<DbFeature> list = new List<DbFeature>();
			string selectString = DbSqlStringBuilder.GetSelectString(this.Name, this.Attributes.ToNameArray(), whereStatement, orderStatement);
			using (DbHelper.DbDataReader dbDataReader = new DbHelper.DbDataReader(ref this._connection, selectString))
			{
				while (dbDataReader.Read())
				{
					List<DbFeatureAttribute> list2 = new List<DbFeatureAttribute>();

					try
					{
						IEnumerator enumerator = this.Attributes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							DbFeatureClassAttribute dbFeatureClassAttribute = (DbFeatureClassAttribute)enumerator.Current;
							DbFeatureAttribute item = new DbFeatureAttribute(ref dbFeatureClassAttribute, RuntimeHelpers.GetObjectValue(dbDataReader.GetObject(dbFeatureClassAttribute.Name)));
							list2.Add(item);
						}
					}
					catch
					{
						throw new GetFeaturesException();
					}
					List<DbFeature> arg_B0_0 = list;
					DbFeatureClass dbFeatureClass = this;
					arg_B0_0.Add(new DbFeature(ref dbFeatureClass, list2));
				}
			}
			return list;
		}
		public long GetFeaturesCount()
		{
			string selectString = DbSqlStringBuilder.GetSelectString(this.Name, new string[]
			{
				"count(id)"
			}, "", "");
			return DbHelper.DbSqlReader.GetLong(ref this._connection, selectString);
		}
		public DbFeature CreateFeature()
		{
			List<DbFeatureAttribute> list = new List<DbFeatureAttribute>();
			try
			{
				IEnumerator enumerator = this.Attributes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DbFeatureClassAttribute dbFeatureClassAttribute = (DbFeatureClassAttribute)enumerator.Current;
					DbFeatureAttribute item = new DbFeatureAttribute(ref dbFeatureClassAttribute, "");
					list.Add(item);
				}
			}
			catch
			{
				throw new CreateFeatureException();
			}

			DbFeatureClass dbFeatureClass = this;
			return new DbFeature(ref dbFeatureClass, list);
		}
		public void InsertFeature(ref DbFeature feature)
		{
			bool flag = !feature.IsInserted;
			if (flag)
			{
				string insertRowString = DbSqlStringBuilder.GetInsertRowString(this.Name, feature.Attributes);
				bool flag2 = this._connection.State != ConnectionState.Open;
				if (flag2)
				{
					this._connection.Open();
				}
				bool flag3 = DbHelper.DbSqlExecuter.Execute(this._connection, insertRowString);
				if (flag3)
				{
					string selectLastIdString = DbSqlStringBuilder.GetSelectLastIdString(this.Name);
					feature.Attributes["ID".ToLower()].Value = DbHelper.DbSqlReader.GetLong(ref this._connection, selectLastIdString);
					try
					{
						IEnumerator enumerator = feature.Attributes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)enumerator.Current;
							bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
							if (needsUpdate)
							{
								dbFeatureAttribute.UpdatedExecuted();
							}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						throw new InsertFeatureException();
					}
				}
				this._connection.Close();
			}
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
						this.InsertFeature(ref current);
					}
				}
				catch
				{
					throw new InsertFeaturesException();
				}
			}
		}
		public void UpdateFeature(ref DbFeature feature)
		{
			bool flag = feature.IsInserted && feature.NeedsUpdate;
			if (flag)
			{
				string updateRowString = DbSqlStringBuilder.GetUpdateRowString(this.Name, feature.Attributes, string.Format("id = {0}", feature.ID));
				bool flag2 = DbHelper.DbSqlExecuter.Execute(this._connection, updateRowString);
				if (flag2)
				{
					try
					{
						IEnumerator enumerator = feature.Attributes.GetEnumerator();
						while (enumerator.MoveNext())
						{
							DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)enumerator.Current;
							bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
							if (needsUpdate)
							{
								dbFeatureAttribute.UpdatedExecuted();
							}
						}
					}
					catch
					{
						throw new UpdateFeatureException();
					}
				}
			}
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
						this.UpdateFeature(ref current);
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
			bool isInserted = feature.IsInserted;
			if (isInserted)
			{
				string deleteRowString = DbSqlStringBuilder.GetDeleteRowString(this.Name, string.Format("id = {0}", feature.ID));
				DbHelper.DbSqlExecuter.Execute(this._connection, deleteRowString);
			}
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
						this.DeleteFeature(current);
					}
				}
				catch
				{
					throw new DeleteFeaturesException();
				}

			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
