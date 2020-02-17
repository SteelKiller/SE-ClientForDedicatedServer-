using RestSharp;
using SE_ClientForDedicatedServer_.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SE_ClientForDedicatedServer_.Static
{
    public class MyStatic
    {

        //TELEPORT ARGS
        //Tuple<string, string>[] _params = new Tuple<string, string>[3];
        //_params[0] = new Tuple<string, string>("EntityId", "92415783242895879");
        //_params[1] = new Tuple<string, string>("clearOwner","false");
        //_params[2] = new Tuple<string, string>("GPS", "GPS:SteelKiller #2:24653.28:139987.92:-116475.34:");
        public static ServerConfig Server;

        public static readonly string m_remoteUrlCustomAPI = "/customapi/{0}";
        public static readonly string m_remoteUrlVrageRemote = "/vrageremote/v1/{0}";
        //private static string m_securityKey = "V7ry55j2i3WTaLBYxDuFtg==";
        public static RestRequest CreateRequest(string m_remoteUrl, string m_securityKey, string resourceLink, Method method,
            params Tuple<string, string>[] queryParams)
        {
            int m_nonce = 0;
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


        public static string CustomApiRequest(string RequestURL,params Tuple<string, string>[] param)
        {
            if (Server == null)
                return "";
            RestClient client = new RestClient($"{Server.Address}:{Server.Port + 1}");
            RestRequest request = MyStatic.CreateRequest(MyStatic.m_remoteUrlCustomAPI, Server.ApiKey, RequestURL, Method.GET, param);
            var response = client.Get(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return "";
            return response.Content;
        }

        public static string VrageRemoteApiRequest(string RequestURL, Method method, params Tuple<string, string>[] param)
        {
            if (Server == null)
                return "";
            RestClient client = new RestClient($"{Server.Address}:{Server.Port}");

            RestRequest request = MyStatic.CreateRequest(MyStatic.m_remoteUrlVrageRemote, Server.ApiKey, RequestURL, method, param);          
            var response = client.Execute(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                return "";
            return response.Content;
        }


    }
}
