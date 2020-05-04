using DatabaseConnectorPostgres.DbEngine;
using DatabaseConnectorPostgres.Enum;
using DatabaseConnectorPostgres.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace DbTest
{
    class Program
    {
        private static User LoggedInUser;

        static void Main(string[] args)
        {
            if (Login())
            {
                ShowMenu();
            }
            else
            {
                Login();
            }
        }

        private static bool Login()
        {
            Console.WriteLine("-------Login--------");
            Console.WriteLine("Enter Username: ");
            string inputName = Console.ReadLine();
            SecureString password = new SecureString();
            Console.WriteLine("Enter password: ");
            ConsoleKeyInfo nextKey = Console.ReadKey(true);

            while (nextKey.Key != ConsoleKey.Enter)
            {
                if (nextKey.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        // erase the last * as well
                        Console.Write(nextKey.KeyChar);
                        Console.Write(" ");
                        Console.Write(nextKey.KeyChar);
                    }
                }
                else
                {
                    password.AppendChar(nextKey.KeyChar);
                    Console.Write("*");
                }
                nextKey = Console.ReadKey(true);
            }
            password.MakeReadOnly();
            Console.Write("\n");
            using (var db = DbEngine.Instance.Connection)
            {
                var user = User.Get(db, inputName, SecureStringToString(password));
                switch (user.Key)
                {
                    case EnumUser.SUCCESS:
                        LoggedInUser = user.Value;
                        return true;
                    case EnumUser.MULTIPLE_USER_FOUND:
                        Console.WriteLine("Multiple Users found");
                        return false;
                    case EnumUser.WRONG_PASSWORD:
                        Console.WriteLine("Wrong Password");
                        return false;
                    case EnumUser.USER_NOT_FOUND:
                        Console.WriteLine("User not found");
                        return false;
                }
                Console.Write("\n");
            }
            Console.WriteLine("Unknown Error");
            return false;
        }

        private static void ShowMenu()
        {
            Console.WriteLine("----------- MENU -----------");

            Console.WriteLine("(1) List all User");
            Console.WriteLine("(2) Insert User");
            Console.WriteLine("(3) Delete User");
            Console.WriteLine("(4) Change Admin Rights");
            Console.Write("Enter: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ListAllUser();
                    break;
                case "2":
                    InsertUser();
                    break;
                case "3":
                    DeleteUser();
                    break;
                case "4":
                    ChangeAdminRights();
                    break;
            }
        }

        private static void ChangeAdminRights()
        {
            if (LoggedInUser.IsAdmin)
            {
                Console.WriteLine("Enter Username: ");
                string inputName = Console.ReadLine();
                using (var db = DbEngine.Instance.Connection)
                {
                    KeyValuePair<EnumUser, User> newUser = User.Get(db, inputName);
                    if (newUser.Key == EnumUser.SUCCESS)
                    {
                        if (newUser.Value.IsAdmin)
                        {
                            newUser.Value.IsAdmin = false;
                            newUser.Value.Update();
                            Console.WriteLine(newUser.Value.UserName + " is no admin anymore!");
                        }
                        else
                        {
                            newUser.Value.IsAdmin = true;
                            newUser.Value.Update();
                            Console.WriteLine(newUser.Value.UserName + " is admin now!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error updating User");
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                Console.WriteLine("No Admin rights!");
            }
            ShowMenu();
        }

        private static void DeleteUser()
        {
            if (LoggedInUser.IsAdmin)
            {
                Console.WriteLine("Enter Username: ");
                string inputName = Console.ReadLine();
                using (var db = DbEngine.Instance.Connection)
                {
                    KeyValuePair<EnumUser, User> newUser = User.Get(db, inputName);
                    if (newUser.Key == EnumUser.SUCCESS)
                    {
                        if (newUser.Value.IsAdmin)
                        {
                            Console.WriteLine("Cannot delete Admins");
                        }
                        else
                        {
                            newUser.Value.Delete();
                            Console.WriteLine(newUser.Value.UserName + " deleted!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error deleting User");
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                Console.WriteLine("No Admin rights!");
            }
            ShowMenu();
        }

        private static void InsertUser()
        {
            if (LoggedInUser.IsAdmin)
            {
                Console.WriteLine("Enter Username: ");
                string inputName = Console.ReadLine();
                Console.WriteLine("Enter Password: ");
                string inputPw = Console.ReadLine();
                using (var db = DbEngine.Instance.Connection)
                {
                    KeyValuePair<EnumUser, User> newUser = User.CreateUser(db, inputName, inputPw);
                    if (newUser.Key == EnumUser.SUCCESS)
                    {
                        newUser.Value.Insert();
                        Console.WriteLine("User created!");
                    }
                    else
                    {
                        Console.WriteLine("User exists");
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                Console.WriteLine("No Admin rights!");
            }
            ShowMenu();
        }

        private static void ListAllUser()
        {
            using (var db = DbEngine.Instance.Connection)
            {
                var users = User.GetAll(db);
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
                ShowMenu();
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
