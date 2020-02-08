using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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
using RestSharp;
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

        private int PCUToPercents(int pcu, int maxPcu)
        {
            return pcu / maxPcu;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
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
        private readonly string m_remoteUrl = "/vrageremote/{0}";
        private readonly string m_securityKey = "V7ry55j2i3WTaLBYxDuFtg==";
        private int m_nonce = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        public RestRequest CreateRequest(string resourceLink, Method method,
            params Tuple<string, string>[] queryParams)
        {
            string methodUrl = string.Format(m_remoteUrl, resourceLink);
            RestRequest request = new RestRequest(methodUrl, method);
            string date = DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture);
            request.AddHeader("Date", date);
            m_nonce = new Random().Next(0, int.MaxValue);
            string nonce = m_nonce.ToString();
            StringBuilder message = new StringBuilder();
            message.Append(methodUrl);
            if (queryParams.Length > 0)
            {
                message.Append("?");
            }

            for (int i = 0; i < queryParams.Length; i++)
            {
                var param = queryParams[i];
                request.AddQueryParameter(param.Item1, param.Item2);
                message.AppendFormat("{0}={1}", param.Item1, param.Item2);
                if (i != queryParams.Length - 1)
                {
                    message.Append("&");
                }
            }

            message.AppendLine();
            message.AppendLine(nonce);
            message.AppendLine(date);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message.ToString());

            byte[] key = Convert.FromBase64String(m_securityKey);
            byte[] computedHash;
            using (HMACSHA1 hmac = new HMACSHA1(key))
            {
                computedHash = hmac.ComputeHash(messageBuffer);
            }

            string hash = Convert.ToBase64String(computedHash);

            request.AddHeader("Authorization", string.Format("{0}:{1}", nonce, hash));
            return request;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*ServerManagerWindow smw = new ServerManagerWindow();
            smw.Owner = this;
            smw.Show();
            */
            RestClient client = new RestClient("http://192.168.139.1:8080");
            

            Tuple<string, string>[] _params = new Tuple<string, string>[0];
            //_params[0] = new Tuple<string, string>("","");

            RestRequest request = CreateRequest("api", Method.GET, _params);
            var response= client.Get(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show(response.Content);
            }
        }
    }
}
