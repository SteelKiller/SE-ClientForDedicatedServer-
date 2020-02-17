using SE_ClientForDedicatedServer_.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SE_ClientForDedicatedServer_.Windows
{
    /// <summary>
    /// Логика взаимодействия для ServerManagerWindow.xaml
    /// </summary>
    public partial class ServerManagerWindow : Window
    {
        public ServerManagerWindow()
        {
            InitializeComponent();
        }
        public ServerConfig server = null;

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^0-9]+");
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void FillServerinformation(ServerConfig server)
        {
            tb_Name.Text = server.Name;
            tb_Port.Text = server.Port.ToString();
            tb_RemoteUrl.Text = server.Address;
            tb_SecurityKey.Text = server.ApiKey;
            cb_AutoConnect.IsChecked = server.AutoConnect;
        }

        private void SetEnableToElements(bool cancel, bool connect, bool edit, bool remove, bool save, bool name, bool port, bool url, bool key, bool autoConnect,bool list)
        {
            b_Cancel.IsEnabled = cancel;
            b_Connect.IsEnabled = connect;
            b_Edit.IsEnabled = edit;
            b_Remove.IsEnabled = remove;
            b_Save.IsEnabled = save;
            tb_Name.IsEnabled = name;
            tb_Port.IsEnabled = port;
            tb_RemoteUrl.IsEnabled = url;
            tb_SecurityKey.IsEnabled = key;
            cb_AutoConnect.IsEnabled = autoConnect;
            lv_ServersList.IsEnabled = list;
        }

        private void ClearTextBoxes()
        {
            tb_Name.Text = "localServer";
            tb_Port.Text = "8080";
            tb_RemoteUrl.Text = "http:\\localhost";
            tb_SecurityKey.Text = "";
        }

        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            int index = lv_ServersList.SelectedIndex;

            if (index == -1)
                return;

            if (index == 0)
            {
                SetEnableToElements(true,false,false,false,true,true,true,true,true,false,false);
                return;
            }

            FillServerinformation((ServerConfig)lv_ServersList.SelectedItem);

            SetEnableToElements(false,true,true,true,false,false,false,false,false,false,true);

        }

        private void b_Edit_Click(object sender, RoutedEventArgs e)
        {
            b_Save.IsEnabled = true;
            SetEnableToElements(true, false, false, false, true, true, true, true, true, true,false);
            UpdateServerList();
        }

        private bool CheckEditsIsFilled()
        {
            if (String.IsNullOrEmpty(tb_Name.Text) || String.IsNullOrEmpty(tb_RemoteUrl.Text) || String.IsNullOrEmpty(tb_SecurityKey.Text))
                return false;
            return true;
        }
        private void UpdateServerList()
        {
            lv_ServersList.ItemsSource = Settings.Settings.GetServerList();
        }

        private void b_Save_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckEditsIsFilled())
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (lv_ServersList.SelectedIndex > 0)
            {
                Settings.Settings.DeleteServer((ServerConfig)lv_ServersList.SelectedItem);
            }

            Settings.Settings.AddServer(new ServerConfig(tb_Name.Text, tb_RemoteUrl.Text, Convert.ToInt32(tb_Port.Text), tb_SecurityKey.Text, (bool)cb_AutoConnect.IsChecked));
            UpdateServerList();
            SetEnableToElements(false, false, false, false, false, false, false, false, false, false,true);
            ClearTextBoxes();
        }

        private void b_Cancel_Click(object sender, RoutedEventArgs e)
        {
            SetEnableToElements(false, false, false, false, false, false, false, false, false, false,true);
            ClearTextBoxes();
            lv_ServersList.SelectedIndex = -1;
        }

        private void b_Connect_Click(object sender, RoutedEventArgs e)
        {
            server = (ServerConfig)lv_ServersList.SelectedItem;
            this.Close();
        }

        private void b_Close_Click(object sender, RoutedEventArgs e)
        {
            server = null;
            this.Close();
        }

        private void b_Remove_Click(object sender, RoutedEventArgs e)
        {
            Settings.Settings.DeleteServer((ServerConfig)lv_ServersList.SelectedItem);
            SetEnableToElements(false, false, false, false, false, false, false, false, false, false,true);
            UpdateServerList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateServerList();
        }
    }
}
