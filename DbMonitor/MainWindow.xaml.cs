using DatabaseConnectorPostgres.Enum;
using DatabaseConnectorPostgres.Models;
using log4net;
using ServerApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public MainWindow()
        {
            InitializeComponent();
            InitDb();
        }

        private void InitDb()
        {
            try
            {
                if (Database.IsOnline())
                {
                    lblDbOnline.Background = Brushes.Green;
                    lblDbOnlineText.Content = "Db Online";
                    InitUsers();
                }
                else
                {
                    lblDbOnline.Background = Brushes.Red;
                    lblDbOnlineText.Content = "Db Offline";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }

        private void InitUsers()
        {
            try
            {
                var list = Database.GetUsers();
                listUserInDB.Items.Clear();
                foreach (var entry in list)
                {
                    listUserInDB.Items.Add(entry);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtboxInsertUserUserName.Text.Length >= 4 && txtboxInsertUserPW.Text.Length >= 8)
                {
                    var insert = Database.AddUser(txtboxInsertUserUserName.Text, txtboxInsertUserPW.Text, (bool)chkboxIsAdmin.IsChecked);

                    switch (insert)
                    {
                        case EnumUser.SUCCESS:
                            MessageBox.Show("User added", "Success");
                            txtboxInsertUserUserName.Clear();
                            txtboxInsertUserPW.Clear();
                            chkboxIsAdmin.IsChecked = false;
                            InitUsers();
                            break;
                        case EnumUser.USER_EXISTS:
                            MessageBox.Show("User already exists", "Info");
                            break;
                        default:
                            MessageBox.Show("Error", "Error");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Username must be at least 4 characters long, Password at least 8", "Error");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null && item.IsSelected)
            {
                User user = item.DataContext as User;
                MessageBoxResult result = MessageBox.Show("Do you want to delete User "+ user.UserName + "?", "Delete User", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        var delete = Database.DeleteUser(user.ID);
                        if (delete)
                        {
                            MessageBox.Show(user.UserName + " deleted", "Deleted");
                            InitUsers();
                        }
                        else
                        {
                            MessageBox.Show("Error deleting", "Error");
                        }
                        break;
                }
            }
        }
    }
}
