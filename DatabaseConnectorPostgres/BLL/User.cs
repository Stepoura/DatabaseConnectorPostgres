﻿
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

		public static async Task<KeyValuePair<EnumUser, User>> Get(NpgsqlConnection connection, string username)
		{
			DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
			List<DbFeature> features = await dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
			int count = features.Count;
			if (count == 0)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.USER_NOT_FOUND, null);
			}
			if (count != 1)
			{
				return new KeyValuePair<EnumUser, User>(EnumUser.MULTIPLE_USER_FOUND, null);
			}

			return new KeyValuePair<EnumUser, User>(EnumUser.SUCCESS, new User(features[0]));
		}

		public static async Task<KeyValuePair<EnumUser, User>> Get(NpgsqlConnection connection, string username, string password)
		{
			DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
			List<DbFeature> features = await dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
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

		public enum EnumUser
		{
			SUCCESS,
			USER_EXISTS,
			FAILED,
			USER_NOT_FOUND,
			MULTIPLE_USER_FOUND,
			WRONG_PASSWORD
		}

	}
}
