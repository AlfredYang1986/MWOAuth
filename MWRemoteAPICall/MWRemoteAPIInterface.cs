using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWRemoteAPICall
{
    public class MWRemoteAPIFactory
    {
        static private MWRemoteAPIInterface _impl = new MWRemoteAPI();

        static public MWRemoteAPIInterface Instance()
        {
            return _impl;
        }
    }

    public interface MWRemoteAPIInterface : IDisposable
    {
        object invokes(string serviceName, string epName, EndPointParameters ps, Func<string, Object> func = null);
    }
}