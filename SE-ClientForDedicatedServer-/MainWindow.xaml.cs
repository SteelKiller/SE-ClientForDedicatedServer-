using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using RestSharp;
using SE_ClientForDedicatedServer_.ApiFormatClasses;
using SE_ClientForDedicatedServer_.Settings;
using SE_ClientForDedicatedServer_.Static;
using SE_ClientForDedicatedServer_.Windows;

namespace SE_ClientForDedicatedServer_
{

    internal class PCUConverter : IValueConverter
    {
        private string PCUTextBlock(int pcu)
        {
            int K = pcu / 1000;
            int M = K / 1000;
            int B = M / 1000;
            int T = B / 1000;

            if (T > 0)
                return String.Format("{0} T", T % 1000);
            if (B > 0)
                return String.Format("{0} B", B % 1000);
            if (M > 0)
                return String.Format("{0} M", M % 1000);
            if (K > 0)
                return String.Format("{0} K", K % 1000);

            return pcu.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return PCUTextBlock((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class LastLoginOnlineConverter : IValueConverter
    {
        private string PCUTextBlock(DateTime lastOnline)
        {
            return lastOnline.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date = (DateTime)value;
            int countDays = (DateTime.Now - date).Days;
            if (countDays == 0)
                return "Today";
            if (countDays == 1)
                return $"{countDays} Day Ago";

            return $"{countDays} Days Ago";


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class ListViewData
    {
        public string Name { get; set; }
        public int PCU { get; set; }
        public int GridAmount { get; set; }
    }

    public partial class MainWindow : Window
    {

        private PlayersInformationFormat[] players;
        private GridInformationFormat grids;
        private GridInformationWindow gridInfWindow;
        private DispatcherTimer Timer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex _regex = new Regex("[^0-9]+");
            e.Handled = _regex.IsMatch(e.Text);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServerManagerWindow smw = new ServerManagerWindow();
            smw.Owner = this;
            smw.Show();
            if (smw.server != null)
            {
                MyStatic.Server = smw.server;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            t_DumpRadius.Text = (Math.Round(e.NewValue * 500)).ToString();
        }


        private void t_DumpRadius_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                s_DumpRadius.Value = Convert.ToInt32(t_DumpRadius.Text) / 500;
            }
        }

        private void UpdateCustomAPI()
        {
            string response = MyStatic.CustomApiRequest("GetLastPlayersLoginTime");

            if (!string.IsNullOrEmpty(response))
            {
                players = new JavaScriptSerializer().Deserialize<PlayersInformationFormat[]>(response);
            }
        }

        private void UpdateVrageRemoteAPI()
        {
            string response = MyStatic.VrageRemoteApiRequest("session/grids",Method.GET);

            if (!string.IsNullOrEmpty(response))
            {
                grids = new JavaScriptSerializer().Deserialize<GridInformationFormat>(response);
            }
        }

        private void PlayersGridsRefresh()
        {
            for (int i = 0; i < grids.data.Grids.Length; i++)
            {
                for (int j = 0; j < players.Length; j++)
                {
                    if (grids.data.Grids[i].OwnerSteamId == players[j].UserId)
                    {
                        players[j].PCU += grids.data.Grids[i].PCU;
                        players[j].grids.Add(grids.data.Grids[i]);
                        players[j].GridAmount++;
                        break;
                    }
                }
            }
        }

        private int GetMaxPCU()
        {
            int MaxPcu = 0;
            for (int j = 0; j < players.Length; j++)
            {
                if (players[j].PCU > MaxPcu)
                {
                    MaxPcu = players[j].PCU;
                }
            }
            return MaxPcu;
        }

        private void UpdateLists()
        {
            lv_PlyersActivity.Items.Clear();
            lv_PlayersInfo.Items.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                lv_PlyersActivity.Items.Add(players[i]);
            }

            PlayersGridsRefresh();

            int MaxPcu = GetMaxPCU();

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].PCU != 0 || MaxPcu != 0)
                    players[i].PCUPercents = (double)players[i].PCU / MaxPcu * 100;
                lv_PlayersInfo.Items.Add(players[i]);

            }
        }

        private void ListViewsUpdate()
        {
            UpdateCustomAPI();
            UpdateVrageRemoteAPI();
            if (grids == null)
            {
                MessageBox.Show($"ошибка загрузки данных grids");
                return;
            }
            if (players == null)
            {
                MessageBox.Show($"ошибка загрузки данных players");
                return;
            }
            UpdateLists();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Settings.Settings.LoadSettings();

            gridInfWindow = new GridInformationWindow();

            if ((MyStatic.Server = Settings.Settings.ServerForAutoStart()) == null)
            {

                ServerManagerWindow smw = new ServerManagerWindow();
                smw.Owner = this;
                smw.ShowDialog();
                if (smw.server == null)
                    return;
                MyStatic.Server = smw.server;

            }
            ListViewsUpdate();
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(TimerTick);
            Timer.Interval = new TimeSpan(0,0,3);
            Timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            int LastSelectedIndex = lv_PlayersInfo.SelectedIndex;
            ListViewsUpdate();
            if (LastSelectedIndex >= lv_PlayersInfo.Items.Count)
                LastSelectedIndex = lv_PlayersInfo.Items.Count-1;
            lv_PlayersInfo.SelectedIndex = LastSelectedIndex;
            /*if (gridInfWindow.IsActive)
                gridInfWindow.GridInformationUpdate((_Grids)lv_PlayerGrids.SelectedItem);*/
        }

        private void SetPlayerInformation(PlayersInformationFormat player)
        {
            tb_PlayerName.Text = player.Name;
            tb_PlayerPCU.Text = (string)new PCUConverter().Convert(player.PCU, null, null, null);
            lv_PlayerGrids.ItemsSource = player.grids;
        }

        private void lv_PlayersInfo_Selected(object sender, RoutedEventArgs e)
        {
            if (lv_PlayersInfo.SelectedIndex != -1)
            {
                SetPlayerInformation((PlayersInformationFormat)((ListView)sender).SelectedItem);
            }
        }

        private void lv_PlayerGrids_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lv_PlayerGrids.SelectedItem == null)
                return;
          
            if (!gridInfWindow.IsActive)
                gridInfWindow.Show();
            gridInfWindow.GridInformationUpdate((_Grids)lv_PlayerGrids.SelectedItem);
        }

        private void MenuItem_DumpManager_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_GridManager_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gridInfWindow.CloseWindows();
            Timer.Stop();
        }
    }
}
