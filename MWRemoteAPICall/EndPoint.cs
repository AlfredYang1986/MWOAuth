using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MWRemoteAPICall
{
    class EndPointFactory
    {
        static Object createHandler(string handlerName)
        {
            Type t = Type.GetType(@"MWRemoteAPICall." + handlerName);   
            ConstructorInfo constructor = t.GetConstructor(Type.EmptyTypes);
            return constructor.Invoke(null);
        }

        static public EndPoint CreateInstance(string name, string method, string handler)
        {
            if (method.ToLower() == "get")
            {
                return new GetEndPoint(name) { handler = createHandler(handler) as EndPointHandler };
            }
            else
            {
                return new PostEndPoint(name) { handler = createHandler(handler) as EndPointHandler };
            }
        }
    }

    abstract class EndPoint
    {
        protected string _name;
        public string name { get { return _name; } }
        protected string _method;
        public string method { get { return _method; } }
        public EndPointHandler handler { get; set; }

        abstract public object invokes(string url, EndPointParameters ps, Func<string, Object> func);
    }

    class GetEndPoint : EndPoint
    {
        public GetEndPoint(string n) { _name = n; }
        override public object invokes(string url, EndPointParameters ps, Func<string, Object> func)
        {
            return handler.invokes(EPPHelper.urlProtocol(url, name, ps), func);
        }
    }

    class PostEndPoint : EndPoint
    {
        public PostEndPoint(string n) { _name = n; }
        override public object invokes(string url, EndPointParameters ps, Func<string, Object> func)
        {
            return handler.invokes(EPPHelper.postProtocol(url, name, ps), func);
        }
    }
}