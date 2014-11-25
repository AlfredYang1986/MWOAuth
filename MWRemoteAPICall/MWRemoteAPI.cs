using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWRemoteAPICall
{
    class MWRemoteAPI : MWRemoteAPIInterface
    {
        private IEnumerable<EndPointService> services;
        private EndPointConfigReader reader;

        public void Dispose() { }
        
        public object invokes(string serviceName, string epName, EndPointParameters ps, Func<string, Object> func = null)
        {
            try
            {
                return services.FirstOrDefault(x => x.serviceName == serviceName).invokes(epName, ps, func);
            }
            catch (System.Exception )
            {
                return "not existing";	
            }
        }

        public MWRemoteAPI() 
        {
            reader = new EndPointConfigReader();
            services = reader.initEndPoint("dispatch", "oauth");
        }
    }
}
