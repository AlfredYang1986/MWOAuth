using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace MWRemoteAPICall
{
    class EPPHelper
    {
        static public HttpWebResponse urlProtocol(string url, string end, EndPointParameters xsl)
        {
            string requestUrl = url + "/" + end + "?" + xsl.ToString();
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                return  request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return null;
            }
        }

        static public HttpWebResponse postProtocol(string url, string end, EndPointParameters xsl)
        {
            string requestUrl = url + "/" + end;

            //Make request
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";

            StreamWriter writer = new StreamWriter(request.GetRequestStream());
            var myJSON = JsonConvert.SerializeObject(xsl).ToString();
            writer.Write(myJSON);
            writer.Close();
            
            return request.GetResponse() as HttpWebResponse;
        }
    }
}