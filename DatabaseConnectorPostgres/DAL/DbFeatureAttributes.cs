
using DatabaseConnectorPostgres.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureAttributes : IEnumerable
	{
		private List<DbFeatureAttribute> _internalFeatureAttributeList;
		public DbFeatureAttribute this[string attributeName]
		{
			get
			{
				DbFeatureAttribute result;

				try
				{
					foreach(var entry in _internalFeatureAttributeList)
					{
						DbFeatureAttribute current = entry;
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
					throw new GetDbFeatureAttributeException();
				}
				result = null;
				return result;
			}
		}
		public DbFeatureAttribute this[int index]
		{
			get
			{
				bool flag = index > checked(this.Count - 1);
				DbFeatureAttribute result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = this._internalFeatureAttributeList[index];
				}
				return result;
			}
		}
		public int Count
		{
			get
			{
				return this._internalFeatureAttributeList.Count;
			}
		}
		public IEnumerator GetEnumerator()
		{
			return (IEnumerator)this._internalFeatureAttributeList.GetEnumerator();
		}
		public DbFeatureAttributes(List<DbFeatureAttribute> valAttributeList)
		{
			this._internalFeatureAttributeList = valAttributeList;
		}
	}
}
