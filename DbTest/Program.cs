using DatabaseConnectorPostgres.BLL;
using DatabaseConnectorPostgres.DbEngine;
using DbTest.Helper;
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
        private static string name = "";

        [STAThread]
        static async Task Main(string[] args)
        {
            for(int i = 0; i < 100; i++)
            {
                await InsertUserAsync();
                await Task.Delay(300);
            }
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
            Random rand = new Random(DateTime.Now.Second); // we need a random variable to select names randomly
            RandomName nameGen = new RandomName(rand); // create a new instance of the RandomName class
            name = nameGen.Generate(Sex.Male, 1); // generate a male name, with one middal name.

            using (var db = await DbEngine.Instance())
            {
                KeyValuePair<EnumUser, User> newUser = await CreateUser(db.Connection, name, "test1234", true);
                if (newUser.Key == EnumUser.SUCCESS)
                {
                    await InsertPgpAsync();
                    Console.WriteLine("User created!");
                }
                else
                {
                    Console.WriteLine("User exists");
                }
                Console.Write("\n");
            }
        }

        private static async Task InsertPgpAsync()
        {
            var user = await GetUser();
            using (var db = await DbEngine.Instance())
            {
                KeyValuePair<EnumPgp, UserPgp> newPgp = await UserPgp.CreatePgpKeys(db.Connection, user);
                if (newPgp.Key == EnumPgp.SUCCESS)
                {
                    Console.WriteLine("Pgp created!");
                    var details = await UserPgp.Get(db.Connection, user.Id);
                }
                else
                {
                    Console.WriteLine("Pgp exists");
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

        private static async Task<User> GetUser()
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
                return user.Value;
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
