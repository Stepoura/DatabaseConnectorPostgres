using DatabaseConnectorPostgres.Exceptions;
using DbEngDatabaseConnectorPostgresine.DAL;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeature
	{
		public long ID
		{
			get
			{
                if (Attributes["ID".ToLower()] != null)
                {
					return Attributes["ID".ToLower()].ValueLong;
				}
                else
                {
					return 0L;
                }			
			}
		}

		public DbFeatureClass FeatureClass { get; }
		public DbFeatureAttributes Attributes { get; }

		public bool IsInserted
		{
			get
			{
				return ID > 0L;
			}
		}

		public bool NeedsUpdate
		{
			get
			{
				bool result;

				try
				{
					foreach(var entry in Attributes)
					{
						DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)entry;
						bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
						if (needsUpdate)
						{
							result = true;
							return result;
						}
					}
				}
				catch
				{
					throw new NeedsUpdateException();
				}
				result = false;
				return result;
			}
		}

		public DbFeature(ref DbFeatureClass refFeatureClass, List<DbFeatureAttribute> valAttributeList)
		{
			FeatureClass = refFeatureClass;
			Attributes = new DbFeatureAttributes(valAttributeList);
		}
		public void Insert()
		{
			DbFeatureClass arg_0B_0 = this.FeatureClass;
			DbFeature dbFeature = this;
			arg_0B_0.InsertFeature(ref dbFeature);
		}
		public void Update()
		{
			DbFeatureClass arg_0B_0 = this.FeatureClass;
			DbFeature dbFeature = this;
			arg_0B_0.UpdateFeature(ref dbFeature);
		}
		public void Delete()
		{
			FeatureClass.DeleteFeature(this);
		}
	}
}
