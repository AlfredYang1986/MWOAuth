using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MWRemoteAPICall
{
    class EndPointConfigReader
    {
        private string strPath = Environment.GetFolderPath(
            Environment.SpecialFolder.DesktopDirectory) + @"\Endpoint.xml";
        private XDocument _doc;

        public EndPointConfigReader()
        {
            try
            {
                _doc = XDocument.Load(strPath);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        public IEnumerable<EndPointService> initEndPoint(params string[] names)
        {
            return from s in _doc.Descendants(@"service")
                   select new EndPointService(s.Attribute(@"name").Value, s.Attribute(@"url").Value)
                   {
                        endPoints = from ep in s.Descendants(@"endpoint")
                                    select EndPointFactory.CreateInstance(
                                    ep.Attribute(@"name").Value, ep.Attribute(@"method").Value, 
                                    ep.Attribute(@"handler").Value)
                   };
        }
    }
}