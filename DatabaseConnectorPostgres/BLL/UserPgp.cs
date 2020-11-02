using Cinchoo.PGP;
using DatabaseConnectorPostgres.DAL;
using DbEngDatabaseConnectorPostgresine.DAL;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorPostgres.BLL
{
    public class UserPgp : DbFeatureItem
    {
        private const string TABLE_NAME = "pgpkeys";

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

        public int UserId
        {
            get
            {
                return Feature.Attributes["user_id"].ValueInt;
            }
            set
            {
                Feature.Attributes["user_id"].Value = value;
            }
        }

        public string PgpKeyPublic
        {
            get
            {
                return Feature.Attributes["pgp_public_key"].ValueString;
            }
            set
            {
                Feature.Attributes["pgp_public_key"].Value = value;
            }
        }

        public string PgpKeyPrivate
        {
            get
            {
                return Feature.Attributes["pgp_private_key"].ValueString;
            }
            set
            {
                Feature.Attributes["pgp_private_key"].Value = value;
            }
        }

        private static async Task<UserPgp> CreateNew(NpgsqlConnection connection)
        {
            DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
            return new UserPgp(dbFeatureClass.CreateFeature());
        }

        private static bool CheckIfFolderExists(int userId)
        {
            string pathPublicKey = string.Format(@"PgpKeys/{0}/pub.asc", userId);
            string pathPrivateKey = string.Format(@"PgpKeys/{0}/pri.asc", userId);

            bool Folderexists = Directory.Exists(string.Format(@"PgpKeys/{0}/", userId));
            bool PubExists = File.Exists(pathPublicKey);
            bool PrivExists = File.Exists(pathPrivateKey);
            if (!Folderexists || !PubExists || !PrivExists)
            {
                Directory.CreateDirectory(string.Format(@"PgpKeys/{0}/", userId));
                using (FileStream fs = File.Create(pathPublicKey))
                {
                }
                using (FileStream fs = File.Create(pathPrivateKey))
                {
                }
                CheckIfFolderExists(userId);
            }
            return true;
        }

        public static async Task<KeyValuePair<EnumPgp, UserPgp>> CreatePgpKeys(NpgsqlConnection connection, User user)
        {
            UserPgp newPgp = await CreateNew(connection);
            using (ChoPGPEncryptDecrypt pgp = new ChoPGPEncryptDecrypt())
            {
                string filenamePublic = string.Format(@"PgpKeys/{0}/pub.asc", user.Id);
                string filenamePrivate = string.Format(@"PgpKeys/{0}/pri.asc", user.Id);

                if (CheckIfFolderExists(user.Id)){
                    pgp.GenerateKey(filenamePublic, filenamePrivate, user.Email, user.Password);
                    newPgp.UserId = user.Id;
                    newPgp.PgpKeyPrivate = filenamePrivate;
                    newPgp.PgpKeyPublic = filenamePublic;
                    await newPgp.Insert();
                    return new KeyValuePair<EnumPgp, UserPgp>(EnumPgp.SUCCESS, newPgp);
                }
            }
            return new KeyValuePair<EnumPgp, UserPgp>(EnumPgp.FAILED, newPgp);
        }

        public static async Task<KeyValuePair<EnumPgp, UserPgp>> Get(NpgsqlConnection connection, int userId)
        {
            DbFeatureClass dbFeatureClass = await DbFeatureClass.BuildDbFeatureClassAsync(connection, TABLE_NAME);
            List<DbFeature> features = await dbFeatureClass.GetFeatures(string.Format("user_id = '{0}'", userId), "");
            int count = features.Count;
            if (count == 0)
            {
                return new KeyValuePair<EnumPgp, UserPgp>(EnumPgp.FAILED, null);
            }
            if (count != 1)
            {
                return new KeyValuePair<EnumPgp, UserPgp>(EnumPgp.MULTIPLE_FOUND, null);
            }

            return new KeyValuePair<EnumPgp, UserPgp>(EnumPgp.SUCCESS, new UserPgp(features[0]));
        }

        public UserPgp(DbFeature feature) : base(feature)
        {
        }
    }

    public enum EnumPgp
    {
        SUCCESS,
        FAILED,
        MULTIPLE_FOUND
    }
}
