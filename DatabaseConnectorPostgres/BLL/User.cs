
using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Exceptions;
using DatabaseConnectorPostgres.Hashing;
using DbEngDatabaseConnectorPostgresine.DAL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.Models
{
	public class User : DbFeatureItem
	{
		private const string TABLE_NAME = "users";


		public string UserName
		{
			get
			{
				return Feature.Attributes["username"].ValueString;
			}
			set
			{
				Feature.Attributes["username"].Value = value;
			}
		}

		public bool IsAdmin
		{
			get
			{
				bool.TryParse(Feature.Attributes["is_admin"].ValueString, out bool result);
				return result;
			}
			set
			{
				if (value)
				{
					Feature.Attributes["is_admin"].Value = true;
				}
				else
				{
					Feature.Attributes["is_admin"].Value = false;
				}
			}
		}

		public string Password
		{
			get
			{
				return Feature.Attributes["password"].ValueString;
			}
			set
			{
				Feature.Attributes["password"].Value = value;
			}
		}

		private User(DbFeature feature) : base(feature)
		{
		}

		public static User Get(DbFeature feature)
		{
			User user = new User(feature);
			bool flag = user.Feature == null;
			User result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = user;
			}
			return result;
		}

		public static async Task<IList<User>> GetAll(NpgsqlConnection connection)
		{

			DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
			List<DbFeature> features = await dbFeatureClass.GetFeatures();
			List<User> list = new List<User>();

			try
			{
				foreach(var entry in features)
				{
					DbFeature current = entry;
					list.Add(Get(current));
				}
			}
			catch
			{
				throw new GetAllUserException();
			}
			return list;
		}

	}
}
