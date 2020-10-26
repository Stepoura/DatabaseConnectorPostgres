using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
			type_serial,
			type_boolean
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
				return _name;
			}
		}
		public DbFeatureClassAttribute.DataTypes DataType
		{
			get
			{
				return _dataType;
			}
		}
		public bool Nullable
		{
			get
			{
				return _nullable;
			}
		}

		public long Length
		{
			get
			{
				return _length;
			}
		}

		public long Precision
		{
			get
			{
				return _precision;
			}
		}

		public bool PrimaryKey
		{
			get
			{
				return Name.Equals("ID".ToLower());
			}
		}

		public DbFeatureClassAttribute(string valName, DataTypes valDataType, bool valNullable, long valLength = 0L, long valPrecision = 0L)
		{
			_name = valName;
			_dataType = valDataType;
			_nullable = valNullable;
			_length = valLength;
			_precision = valPrecision;
		}

		public DbFeatureClassAttribute(string valName, string valDataType, int valNullable, long valLength = 0L, long valPrecision = 0L) : this(valName, GetDataType(valDataType), GetBool(valNullable), valLength, valPrecision)
		{
		}

        private static bool GetBool(int valNullable)
        {
			return valNullable != 0;
		}

        private static DataTypes GetDataType(string dataTypeName)
		{
            switch (dataTypeName.ToLower())
            {
				case "character varying":
					return DataTypes.type_nvarchar;
				case "timestamp without time zone":
					return DataTypes.type_datetime;
				case "numeric":
					return DataTypes.type_decimal;
				case "integer":
					return DataTypes.type_int;
				case "serial":
					return DataTypes.type_serial;
				case "boolean":
					return DataTypes.type_boolean;
				default:
					return DataTypes.type_unknown;
			}
		}
	}
}
