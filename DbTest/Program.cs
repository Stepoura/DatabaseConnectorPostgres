using DatabaseConnectorPostgres.DbEngine;
using DatabaseConnectorPostgres.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace DbTest
{
    class Program
    {
        [STAThread]
        static async Task Main(string[] args)
        {
            await ListAllUserAsync();
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
