using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWRemoteAPICall
{
    class EndPointService
    {
        private string _name;
        private string _url;

        public EndPointService(string serviceName, string url)
        {
            _name = serviceName;
            _url = url;
        }

        public string serviceName { get{ return _name; } }
        public string servcieURL { get { return _url; } }
        public IEnumerable<EndPoint> endPoints { get; set; }

        public object invokes(string name, EndPointParameters ps, Func<string, Object> func)
        {
            try
            {
                return endPoints.FirstOrDefault(x => x.name == name).invokes(_url, ps, func);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                return "not existing";	
            }
        }
    }
}
