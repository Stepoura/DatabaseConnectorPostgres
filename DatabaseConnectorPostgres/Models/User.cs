
using DatabaseConnectorPostgres.DAL;
using DatabaseConnectorPostgres.Enum;
using DatabaseConnectorPostgres.Exceptions;
using DatabaseConnectorPostgres.Hashing;
using DbEngDatabaseConnectorPostgresine.DAL;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace DatabaseConnectorPostgres.Models
{
	public class User : DbFeatureItem
	{
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

		private static bool CheckIfExists(DbConnection connection, string username)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(connection, "users");
			List<DbFeature> features = dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
			return features.Count > 0;
		}


		private User(DbConnection connection, long id) : base(connection, "users", id)
		{
		}

		private User(DbFeature feature) : base(feature)
		{
		}

		private static User CreateNew(DbConnection connection)
		{
			return new User(new DbFeatureClass(connection, "users").CreateFeature());
		}

		public static KeyValuePair<EnumUser, User> CreateUser(DbConnection connection, string username, string password, bool isAdmin)
		{
			if(!CheckIfExists(connection, username))
			{
				User newUser = CreateNew(connection);
				newUser.UserName = username;
				HashingService hashingService = new HashingService();
				newUser.Password = hashingService.CreatePasswordHash(password);
				newUser.IsAdmin = isAdmin;
				newUser.Insert();
				return new KeyValuePair<EnumUser, User>(EnumUser.SUCCESS, newUser);
			}
			else
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.USER_EXISTS, null);
			}
		}

		public static User Get(DbConnection connection, long id)
		{
			User user = new User(connection, id);
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

		public static KeyValuePair<EnumUser, User> Get(DbConnection connection, string username)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(connection, "users");
			List<DbFeature> features = dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
			int count = features.Count;
			if (count == 0)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.USER_NOT_FOUND, null);
			}
			if (count != 1)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.MULTIPLE_USER_FOUND, null);
			}
			return new KeyValuePair<EnumUser, User>(EnumUser.SUCCESS, new User(connection, features[0].ID));
		}

		public static KeyValuePair<EnumUser, User> Get(DbConnection connection, string username, string password)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(connection, "users");
			List<DbFeature> features = dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
			int count = features.Count;
			if (count == 0)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.USER_NOT_FOUND, null);
			}
			if (count != 1)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.MULTIPLE_USER_FOUND, null);
			}
			User user = new User(features[0]);
			HashingService hashingService = new HashingService();
			bool flag = hashingService.ValidatePasswordHash(password, user.Password);
			if (!flag)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.WRONG_PASSWORD, null);
			}
			return new KeyValuePair<EnumUser, User>(EnumUser.SUCCESS, user);
		}

		public static IList<User> GetAll(DbConnection connection)
		{
			DbFeatureClass dbFeatureClass = new DbFeatureClass(connection, "users");
			List<DbFeature> features = dbFeatureClass.GetFeatures();
			List<User> list = new List<User>();

			try
			{
				foreach(var entry in features)
				{
					DbFeature current = entry;
					list.Add(User.Get(current));
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
