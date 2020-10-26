using DbEngDatabaseConnectorPostgresine.DAL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.DAL
{
	[Serializable]
	public class DbFeatureItem
	{
		public NpgsqlConnection Connection
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
				if (Feature.Attributes["ID".ToLower()] != null)
				{
					return Feature.Attributes["ID".ToLower()].ValueLong;
				}
				else
				{
					return 0L;
				}
			}
			set
			{
				Feature.Attributes["ID".ToLower()].Value = value;
			}
		}

		public DbFeatureItem(DbFeature feature)
		{
			Feature = feature;
		}

		public void Delete()
		{
			FeatureClass.DeleteFeature(Feature);
		}

		public void Insert()
		{
			DbFeatureClass arg_10_0 = FeatureClass;
			DbFeature feature = Feature;
			arg_10_0.InsertFeature(feature);
			Feature = feature;
		}

		public void Update()
		{
			DbFeatureClass arg_10_0 = FeatureClass;
			DbFeature feature = Feature;
			arg_10_0.UpdateFeature(feature);
			Feature = feature;
		}
	}
}
