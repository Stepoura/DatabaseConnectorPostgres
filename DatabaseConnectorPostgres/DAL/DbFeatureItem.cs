using DbEngDatabaseConnectorPostgresine.DAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	[Serializable]
	public abstract class DbFeatureItem
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never), CompilerGenerated]
		private DbFeature _Feature;
		public DbConnection Connection
		{
			get
			{
				return FeatureClass.Connection;
			}
		}
		public DbFeature Feature
		{
			get;
			set;
		}
		public DbFeatureClass FeatureClass
		{
			get
			{
				return Feature.FeatureClass;
			}
		}
		public long ID
		{
			get
			{
				return Feature.Attributes["ID".ToLower()].ValueLong;
			}
			set
			{
				Feature.Attributes["ID".ToLower()].Value = value;
			}
		}
		protected DbFeatureItem(DbConnection connection, string tableName, long id)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(connection, tableName);
			Feature = dbFeatureClass.GetFeature(id);
		}
		protected DbFeatureItem(DbFeature feature)
		{
			Feature = feature;
		}
		public void Delete()
		{
			FeatureClass.DeleteFeature(this.Feature);
		}
		public void Insert()
		{
			DbFeatureClass arg_10_0 = this.FeatureClass;
			DbFeature feature = this.Feature;
			arg_10_0.InsertFeature(ref feature);
			Feature = feature;
		}
		public void Update()
		{
			DbFeatureClass arg_10_0 = this.FeatureClass;
			DbFeature feature = this.Feature;
			arg_10_0.UpdateFeature(ref feature);
			Feature = feature;
		}
	}
}
