
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

namespace DatabaseConnectorPostgres.BLL
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

        public int Id
        {
            get
            {
                return Feature.Attributes["id"].ValueInt;
            }
            set
            {
                Feature.Attributes["id"].Value = value;
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

        public string Email
        {
            get
            {
                return Feature.Attributes["email"].ValueString;
            }
            set
            {
                Feature.Attributes["email"].Value = value;
            }
        }

        private User(DbFeature feature) : base(feature)
        {
        }

        public static async Task<KeyValuePair<EnumUser, User>> CreateUser(NpgsqlConnection connection, string username, string password, bool isAdmin)
        {
            try
            {
                if (!await CheckIfExists(connection, username))
                {
                    User newUser = await CreateNew(connection);
                    newUser.UserName = username;
                    HashingService hashingService = new HashingService();
                    newUser.Password = hashingService.CreatePasswordHash(password);
                    newUser.IsAdmin = isAdmin;
                    newUser.Email = string.Format("{0}@nBeat.com", username).Replace(" ", "");
                    await newUser.Insert();
                    return new KeyValuePair<EnumUser, User>(EnumUser.SUCCESS, newUser);
                }
                else
                {
                    return new KeyValuePair<EnumUser, User>(EnumUser.USER_EXISTS, null);
                }
            }
            catch
            {
                throw new CreateFeatureException();
            }
        }

        private static async Task<User> CreateNew(NpgsqlConnection connection)
        {
            try
            {
                DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
                return new User(dbFeatureClass.CreateFeature());
            }
            catch
            {
                throw new CreateFeatureException();
            }
        }

        private static async Task<bool> CheckIfExists(NpgsqlConnection connection, string username)
        {
            try
            {
                DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
                List<DbFeature> features = await dbFeatureClass.GetFeatures(string.Format("username = '{0}'", username), "");
                return features.Count > 0;
            }
            catch
            {
                throw new GetFeaturesException();
            }
        }

        private static User Get(DbFeature feature)
        {
            try
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
            catch
            {
                throw new GetFeaturesException();
            }
        }

        public static async Task<KeyValuePair<EnumUser, User>> Get(NpgsqlConnection connection, string username)
        {
            try
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
            catch
            {
                throw new GetFeaturesException();
            }
        }

        public static async Task<KeyValuePair<EnumUser, User>> Get(NpgsqlConnection connection, string username, string password)
        {
            try
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
            catch
            {
                throw new GetFeaturesException();
            }
        }

        public static async Task<IList<User>> GetAll(NpgsqlConnection connection)
        {

            DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
            List<DbFeature> features = await dbFeatureClass.GetFeatures();
            List<User> list = new List<User>();

            try
            {
                foreach (var entry in features)
                {
                    DbFeature current = entry;
                    list.Add(Get(current));
                }
            }
            catch
            {
                throw new GetAllFeaturesException();
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
