using SE_ClientForDedicatedServer_.ApiFormatClasses;
using SE_ClientForDedicatedServer_.Static;
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
using System.Windows.Shapes;

namespace SE_ClientForDedicatedServer_.Windows
{
    /// <summary>
    /// Логика взаимодействия для GridInformationWindow.xaml
    /// </summary>
    public partial class GridInformationWindow : Window
    {
        private bool IsDumpCoords = false;
        private _Grids grid;
        private bool CloseWindow = false;
        public GridInformationWindow()
        {
            InitializeComponent();
        }

        public void GridInformationUpdate(_Grids grid)
        {
            this.grid = grid;
            tb_Name.Text = grid.DisplayName;
            tb_Position.Text = grid.Position.Get;
            tb_GridSize.Text = grid.GridSize;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsDumpCoords = (bool)rb_MoveToDump.IsChecked;
            tb_GPS.IsEnabled = !IsDumpCoords;
        }

        private void b_Move_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tb_GPS.Text) && !IsDumpCoords)
            {
                MessageBox.Show("Fill GPS coords");
                return;
            }

            if(String.IsNullOrEmpty(Settings.Settings.DumpGPS) && IsDumpCoords)
            {
                MessageBox.Show("Fill Dump GPS coords");
                return;
            }

            Tuple<string, string>[] _params = new Tuple<string, string>[3];
            _params[0] = new Tuple<string, string>("EntityId", grid.EntityId.ToString());
            _params[1] = new Tuple<string, string>("clearOwner", "false");

            if(!IsDumpCoords)
                _params[2] = new Tuple<string, string>("GPS", tb_GPS.Text);
            else
                _params[2] = new Tuple<string, string>("GPS", Settings.Settings.DumpGPS);

            MyStatic.CustomApiRequest("moveGridTo", _params);
        }

        private void b_Delete_Click(object sender, RoutedEventArgs e)
        {
            Tuple<string, string>[] _params = new Tuple<string, string>[1];
            _params[0] = new Tuple<string, string>("entityId", grid.EntityId.ToString());
            MyStatic.VrageRemoteApiRequest("session/grids/"+ grid.EntityId.ToString(), RestSharp.Method.DELETE);
        }

        private void b_PowerOff_Click(object sender, RoutedEventArgs e)
        {

        }

        public void CloseWindows()
        {
            CloseWindow = true;
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {          
            e.Cancel = true && !CloseWindow;
            this.Hide();
        }
    }
}
