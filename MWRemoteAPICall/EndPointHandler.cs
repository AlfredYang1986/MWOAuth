using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MWRemoteAPICall
{
    interface EndPointHandler
    {
        Object invokes(HttpWebResponse response, Func<string, Object> func);
    }

    class defaultEndPointHandler : EndPointHandler
    {
        public Object invokes(HttpWebResponse response, Func<String, Object> func)
        {
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return streamReader.ReadToEnd().ToString();
            }
        }
    }

    class JsonEndPointhandler : EndPointHandler
    {
        public Object invokes(HttpWebResponse response, Func<String, Object> func)
        {
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return func(streamReader.ReadToEnd().ToString());
            }
        }
    }

    class XmlEndPointhandler : EndPointHandler
    {
        public Object invokes(HttpWebResponse response, Func<String, Object> func)
        {
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                var xDoc = new XmlDocument();
                xDoc.LoadXml(result);
                return func(xDoc.InnerText);
            }
        }
    }
}
