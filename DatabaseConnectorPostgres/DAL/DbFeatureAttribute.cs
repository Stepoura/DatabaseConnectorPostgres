using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureAttribute : DbFeatureClassAttribute
	{
		private bool _needsUpdate;
		private object _value;
		public Type ValueType
		{
			get
			{
				return Value.GetType();
			}
		}

		public bool NeedsUpdate
		{
			get
			{
				return _needsUpdate;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = RuntimeHelpers.GetObjectValue(value);
				_needsUpdate = true;
			}
		}

		public long ValueLong
		{
			get
			{
				bool flag = Value == null || !Versioned.IsNumeric(RuntimeHelpers.GetObjectValue(Value));
				long result;
				if (flag)
				{
					result = 0L;
				}
				else
				{
					result = Conversions.ToLong(Value);
				}
				return result;
			}
		}

		public int ValueInt
		{
			get
			{
				bool flag = Value == null || !Versioned.IsNumeric(RuntimeHelpers.GetObjectValue(Value));
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					result = Conversions.ToInteger(Value);
				}
				return result;
			}
		}

		public string ValueString
		{
			get
			{
				bool flag = Value == null;
				string result;
				if (flag)
				{
					result = string.Empty;
				}
				else
				{
					result = Conversions.ToString(Value);
				}
				return result;
			}
		}

		public void UpdatedExecuted()
		{
			_needsUpdate = false;
		}

		public DbFeatureAttribute(DbFeatureClassAttribute refFeatureClassAttribute, object valValue) : base(refFeatureClassAttribute.Name, refFeatureClassAttribute.DataType, refFeatureClassAttribute.Nullable, 0L, 0L)
		{
			_value = RuntimeHelpers.GetObjectValue(valValue);
		}
	}
}
