using DatabaseConnectorPostgres.DbEngine;
using DatabaseConnectorPostgres.Enum;
using DatabaseConnectorPostgres.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace ServerApp.Classes
{
    public static class Database
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static bool IsOnline()
        {
            try
            {
                using (var db = DbEngine.Instance.Connection)
                {
                    var user = User.GetAll(db);
                    if (user.Count > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch(Exception ex)
            {
                log.Error(ex.ToString());
            }
            return false;
        }

        public static IList<User> GetUsers()
        {
            try
            {
                using (var db = DbEngine.Instance.Connection)
                {
                    return User.GetAll(db);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
            return new List<User>();
        }

        internal static EnumUser AddUser(string username, string password, bool isAdmin)
        {

            try
            {
                using (var db = DbEngine.Instance.Connection)
                {
                    KeyValuePair<EnumUser, User> user =  User.CreateUser(db, username, password, isAdmin);
                    return user.Key;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
            return EnumUser.FAILED;
        }

        internal static bool DeleteUser(long id)
        {
            try
            {
                using (var db = DbEngine.Instance.Connection)
                {
                    var delete = User.Get(db, id);
                    delete.Delete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
            return false;
        }
    }
}
