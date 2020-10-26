using DatabaseConnectorPostgres.Exceptions;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DatabaseConnectorPostgres.DAL
{
	public class DbSqlStringBuilder
	{

		private static string GetSqlElements(List<string> list)
		{
			return Strings.Join(list.ToArray(), ",");
		}

		public static string GetSelectLastIdString(string tableName)
		{
			return string.Format("SELECT max(id) AS LastID FROM {0}", tableName);
		}

		public static string GetUpdateRowString(string tableName, DbFeatureAttributes attributes, string where)
		{
			List<string> list = new List<string>();
			try
			{
				foreach(var entry in attributes)
				{
					DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)entry;
					bool primaryKey = dbFeatureAttribute.PrimaryKey;
					if (!primaryKey)
					{
						bool needsUpdate = dbFeatureAttribute.NeedsUpdate;
						if (needsUpdate)
						{
							list.Add(string.Format("{0} = '{1}'", dbFeatureAttribute.Name, dbFeatureAttribute.ValueString));
						}
					}
				}
			}
			catch
			{
				throw new GetUpdateRowStringException();
			}
			string str = string.Format("UPDATE {0} SET {1}", tableName, GetSqlElements(list));
			return str + string.Format(" WHERE {0}", where);
		}

		public static string GetInsertRowString(string tableName, DbFeatureAttributes attributes)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();

			try
			{
				foreach (var entry in attributes)
				{
					DbFeatureAttribute dbFeatureAttribute = (DbFeatureAttribute)entry;
					bool primaryKey = dbFeatureAttribute.PrimaryKey;
					if (!primaryKey)
					{
						list.Add(string.Format("{0}", dbFeatureAttribute.Name));
						bool flag = dbFeatureAttribute.DataType == DbFeatureClassAttribute.DataTypes.type_datetime;
						if (flag)
						{
							list2.Add(string.Format("'{0}'", Conversions.ToDate(dbFeatureAttribute.Value).ToString("yyyy-MM-dd HH:mm:ss")));
						}
						else
						{
							bool flag2 = dbFeatureAttribute.DataType == DbFeatureClassAttribute.DataTypes.type_int;
							if (flag2)
							{
								bool flag3 = string.IsNullOrWhiteSpace(dbFeatureAttribute.ValueString);
								if (flag3)
								{
									list2.Add(string.Format("{0}", "NULL"));
								}
								else
								{
									list2.Add(string.Format("{0}", dbFeatureAttribute.ValueString));
								}
							}
							else
							{
								list2.Add(string.Format("'{0}'", dbFeatureAttribute.ValueString));
							}
						}
					}
				}
			}
			catch
			{
				throw new GetInsertRowStringException();
			}
			return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, GetSqlElements(list), GetSqlElements(list2));
		}
		public static string GetDeleteRowString(string tableName, string where)
		{
			string str = string.Format("DELETE FROM {0}", tableName);
			return str + string.Format(" WHERE {0}", where);
		}
		public static string GetCreateTableString(string tableName, DbFeatureClassAttribute firstAttribute)
		{
			return string.Format("CREATE TABLE {0} ({1})", tableName, DbSqlStringBuilder.BuildAttributeDefinitionString(firstAttribute));
		}
		private static string BuildAttributeDefinitionString(DbFeatureClassAttribute valAttribute)
		{
			string arg = "";
			switch (valAttribute.DataType)
			{
				case DbFeatureClassAttribute.DataTypes.type_datetime:
					arg = "timestamp without time zone";
					break;
				case DbFeatureClassAttribute.DataTypes.type_decimal:
					arg = string.Format("numeric({0},{1})", valAttribute.Length, valAttribute.Precision);
					break;
				case DbFeatureClassAttribute.DataTypes.type_int:
					arg = "integer";
					break;
				case DbFeatureClassAttribute.DataTypes.type_nvarchar:
					arg = string.Format("character varying({0})", valAttribute.Length);
					break;
				case DbFeatureClassAttribute.DataTypes.type_serial:
					arg = string.Format("serial", valAttribute.Length);
					break;
			}
			return string.Format("{0} {1} {2} ", valAttribute.Name, arg, RuntimeHelpers.GetObjectValue(Interaction.IIf(valAttribute.Nullable, "null", "not null")));
		}
		public static string GetAlterTableCreateColumnString(string tableName, DbFeatureClassAttribute valAttribute)
		{
			string str = string.Format("ALTER TABLE {0} ", tableName);
			return str + string.Format("ADD COLUMN {0}", BuildAttributeDefinitionString(valAttribute));
		}
		public static string GetAlterTableDropColumnString(string tableName, string columnName)
		{
			string str = string.Format("ALTER TABLE {0} ", tableName);
			return str + string.Format("DROP COLUMN {0} ", columnName);
		}
		public static string GetDropTableString(string tableName)
		{
			return string.Format("DROP TABLE {0}", tableName);
		}
		public static string GetSelectString(string tableName, string[] attributes, string where = "", string order = "")
		{
			string text = string.Format("SELECT {0} FROM {1}", BuildAttributesString(attributes), tableName);
			bool flag = where.Length > 0;
			if (flag)
			{
				text += string.Format(" WHERE {0}", where);
			}
			bool flag2 = order.Length > 0;
			if (flag2)
			{
				text += string.Format(" ORDER BY {0}", order);
			}
			return text;
		}
		private static string BuildAttributesString(string[] attributes)
		{
			bool flag = attributes != null && attributes.Length > 0;
			string result;
			if (flag)
			{
				result = string.Join(", ", attributes);
			}
			else
			{
				result = "*";
			}
			return result;
		}
	}
}
