using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MWAuthorizationServer;

namespace MWAuthorizationServer
{
    class UnitTest_MWRefreshToken
    {
        public static void SendRequest(string uri, string method, string contentType, string body)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);
            req.Method = method;
            if (!String.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            if (body != null)
            {
                byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
                req.GetRequestStream().Write(bodyBytes, 0, bodyBytes.Length);
                req.GetRequestStream().Close();
            }

            HttpWebResponse resp;
            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException e)
            {
                resp = (HttpWebResponse)e.Response;
            }
            Console.WriteLine("HTTP/{0} {1} {2}", resp.ProtocolVersion, (int)resp.StatusCode, resp.StatusDescription);
            foreach (string headerName in resp.Headers.AllKeys)
            {
                Console.WriteLine("{0}: {1}", headerName, resp.Headers[headerName]);
            }
            Console.WriteLine();
            Console.WriteLine(new StreamReader(resp.GetResponseStream()).ReadToEnd());
            Console.WriteLine();
            Console.WriteLine("  *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*  ");
            Console.WriteLine();
            System.Console.Read();
        }

        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:1296/Service.svc";//"http://" + Environment.MachineName + ":8000/Service";
            ServiceHost host = new ServiceHost(typeof(MWAuthorizationImpl), new Uri(baseAddress));
            host.AddServiceEndpoint(typeof(IMWAuthorizationServer), new WebHttpBinding(), "").Behaviors.Add(new MWWebHTTPBehaviorHelper());
            host.Open();
            Console.WriteLine("Host opened");

            //SendRequest(baseAddress + "/access_token", "POST", "application/json", "{\"x\":22,\"y\":33}");
            SendRequest(baseAddress + "/access_token", "POST", "application/x-www-form-urlencoded", "grant_type=refresh_token&client_id=yS2JzRYo0Ui9wO5KHvA3V%2bkwWapVxdUw&refresh_token=A/w8iKMt0UjaGeawrj1ZeQnF7wXiHFqz");
            //SendRequest(baseAddress + "/Add", "POST", "application/json", "{\"x\":22,\"z\":33}");
            //SendRequest(baseAddress + "/Add", "POST", "application/x-www-form-urlencoded", "x=22&z=33");

            //SendRequest(baseAddress + "/Concat", "POST", "application/json", "{\"text1\":\"hello\",\"text2\":\" world\"}");
            //SendRequest(baseAddress + "/Concat", "POST", "application/x-www-form-urlencoded", "text1=hello&text2=%20world");
            //SendRequest(baseAddress + "/Concat", "POST", "application/json", "{\"text1\":\"hello\",\"text9\":\" world\"}");
            //SendRequest(baseAddress + "/Concat", "POST", "application/x-www-form-urlencoded", "text1=hello&text9=%20world");
            
        }
    }
}
