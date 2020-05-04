using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbFeatureClassAttribute
	{
		public enum DataTypes
		{
			type_unknown,
			type_datetime,
			type_decimal,
			type_int,
			type_nvarchar,
			type_serial
		}
		private string _name;
		private DbFeatureClassAttribute.DataTypes _dataType;
		private bool _nullable;
		private long _length;
		private long _precision;
		public string Name
		{
			get
			{
				return this._name;
			}
		}
		public DbFeatureClassAttribute.DataTypes DataType
		{
			get
			{
				return this._dataType;
			}
		}
		public bool Nullable
		{
			get
			{
				return this._nullable;
			}
		}
		public long Length
		{
			get
			{
				return this._length;
			}
		}
		public long Precision
		{
			get
			{
				return this._precision;
			}
		}
		public bool PrimaryKey
		{
			get
			{
				return this.Name.Equals("ID".ToLower());
			}
		}
		public DbFeatureClassAttribute(string valName, DbFeatureClassAttribute.DataTypes valDataType, bool valNullable, long valLength = 0L, long valPrecision = 0L)
		{
			this._name = valName;
			this._dataType = valDataType;
			this._nullable = valNullable;
			this._length = valLength;
			this._precision = valPrecision;
		}
		public DbFeatureClassAttribute(string valName, string valDataType, bool valNullable, long valLength = 0L, long valPrecision = 0L) : this(valName, DbFeatureClassAttribute.GetDataType(valDataType), valNullable, valLength, valPrecision)
		{
		}
		private static DbFeatureClassAttribute.DataTypes GetDataType(string dataTypeName)
		{
			string left = dataTypeName.ToLower();
			DbFeatureClassAttribute.DataTypes result;
			if (Operators.CompareString(left, "timestamp without time zone", false) != 0)
			{
				if (Operators.CompareString(left, "numeric", false) != 0)
				{
					if (Operators.CompareString(left, "integer", false) != 0)
					{
						if (Operators.CompareString(left, "character varying", false) != 0)
						{
							if (Operators.CompareString(left, "serial", false) != 0)
							{
								result = DbFeatureClassAttribute.DataTypes.type_unknown;
							}
							else
							{
								result = DbFeatureClassAttribute.DataTypes.type_serial;
							}
						}
						else
						{
							result = DbFeatureClassAttribute.DataTypes.type_nvarchar;
						}
					}
					else
					{
						result = DbFeatureClassAttribute.DataTypes.type_int;
					}
				}
				else
				{
					result = DbFeatureClassAttribute.DataTypes.type_decimal;
				}
			}
			else
			{
				result = DbFeatureClassAttribute.DataTypes.type_datetime;
			}
			return result;
		}
	}
}
