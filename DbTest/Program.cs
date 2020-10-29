using DatabaseConnectorPostgres.BLL;
using DatabaseConnectorPostgres.DbEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using static DatabaseConnectorPostgres.BLL.User;

namespace DbTest
{
    class Program
    {
        private static readonly string name = "nils";

        [STAThread]
        static async Task Main(string[] args)
        {
            await InsertUserAsync();
        }

        private static async Task ListAllYtAsync()
        {
            using (var db = await DbEngine.Instance())
            {
                var users = await YoutubeVideo.GetAll(db.Connection);
                if (users.Value.Count > 0)
                {
                    foreach (var entry in users.Value)
                    {
                        Console.WriteLine("YtVideo: " + entry.Title + "(Id: " + entry.ID + ")" + ", Url " + entry.URL);
                    }
                }
                else
                {
                    Console.WriteLine("No YtVideos found");
                }
                Console.Write("\n");
            }
        }

        private static async Task InsertUserAsync()
        {
            using (var db = await DbEngine.Instance())
            {
                KeyValuePair<EnumUser, User> newUser = await CreateUser(db.Connection, "blub1", "test1234", true);
                if (newUser.Key == EnumUser.SUCCESS)
                {
                    Console.WriteLine("User created!");
                }
                else
                {
                    Console.WriteLine("User exists");
                }
                Console.Write("\n");
            }
        }

        private static async Task ListAllUserAsync()
        {
            using (var db = await DbEngine.Instance())
            {
                var users = await User.GetAll(db.Connection);
                if (users.Count > 0)
                {
                    foreach (var entry in users)
                    {
                        Console.WriteLine("User: " + entry.UserName + "(Id: " + entry.ID + ")" + ", is Admin: " + entry.IsAdmin.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No Users found");
                }
                Console.Write("\n");
            }
        }

        private static async Task GetUser()
        {
            using (var db = await DbEngine.Instance())
            {
                var user = await User.Get(db.Connection, name);
                if (user.Key == User.EnumUser.SUCCESS)
                {
                    Console.WriteLine("User: " + user.Value.UserName + "(Id: " + user.Value.ID + ")" + ", is Admin: " + user.Value.IsAdmin.ToString());
                }
                else
                {
                    Console.WriteLine(user.Key.ToString());
                }

            }
        }

        private static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
