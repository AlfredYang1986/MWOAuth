using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace MWAuthorizationServer
{
    public class MWWebHTTPBehaviorHelper : WebHttpBehavior
    {
        
        
            protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
            {
                bool isRequestWrapped = this.IsRequestWrapped(operationDescription.Behaviors.Find<WebInvokeAttribute>());
                IDispatchMessageFormatter originalFormatter = base.GetRequestDispatchFormatter(operationDescription, endpoint);
                if (isRequestWrapped)
                {
                    return new MyFormUrlEncodedAwareFormatter(
                        operationDescription,
                        originalFormatter,
                        this.GetQueryStringConverter(operationDescription));
                }
                else
                {
                    return originalFormatter;
                }
            }

            private bool IsRequestWrapped(WebInvokeAttribute wia)
            {
                WebMessageBodyStyle bodyStyle;
                if (wia.IsBodyStyleSetExplicitly)
                {
                    bodyStyle = wia.BodyStyle;
                }
                else
                {
                    bodyStyle = this.DefaultBodyStyle;
                }

                return bodyStyle == WebMessageBodyStyle.Wrapped || bodyStyle == WebMessageBodyStyle.WrappedRequest;
            }

            class MyFormUrlEncodedAwareFormatter : IDispatchMessageFormatter
            {
                const string FormUrlEncodedContentType = "application/x-www-form-urlencoded";
                OperationDescription operation;
                IDispatchMessageFormatter originalFormatter;
                QueryStringConverter queryStringConverter;
                public MyFormUrlEncodedAwareFormatter(OperationDescription operation, IDispatchMessageFormatter originalFormatter, QueryStringConverter queryStringConverter)
                {
                    this.operation = operation;
                    this.originalFormatter = originalFormatter;
                    this.queryStringConverter = queryStringConverter;
                }

                public void DeserializeRequest(Message message, object[] parameters)
                {
                    if (IsFormUrlEncodedMessage(message))
                    {
                        XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
                        bodyReader.ReadStartElement("Binary");
                        byte[] bodyBytes = bodyReader.ReadContentAsBase64();
                        string body = Encoding.UTF8.GetString(bodyBytes);
                        NameValueCollection pairs = HttpUtility.ParseQueryString(body);
                        Dictionary<string, string> values = new Dictionary<string, string>();
                        foreach (var key in pairs.AllKeys)
                        {
                            values.Add(key, pairs[key]);
                        }

                        foreach (var part in this.operation.Messages[0].Body.Parts)
                        {
                            if (values.ContainsKey(part.Name))
                            {
                                string value = values[part.Name];
                                parameters[part.Index] = this.queryStringConverter.ConvertStringToValue(value, part.Type);
                            }
                            else
                            {
                                parameters[part.Index] = GetDefaultValue(part.Type);
                            }
                        }
                    }
                    else
                    {
                        this.originalFormatter.DeserializeRequest(message, parameters);
                    }
                }

                public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
                {
                    throw new NotSupportedException("This is a request-only formatter");
                }

                private static bool IsFormUrlEncodedMessage(Message message)
                {
                    object prop;
                    if (message.Properties.TryGetValue(WebBodyFormatMessageProperty.Name, out prop))
                    {
                        if (((WebBodyFormatMessageProperty)prop).Format == WebContentFormat.Raw)
                        {
                            if (message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out prop))
                            {
                                if (((HttpRequestMessageProperty)prop).Headers[HttpRequestHeader.ContentType].StartsWith(FormUrlEncodedContentType))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }

                private static object GetDefaultValue(Type type)
                {
                    if (type.IsValueType)
                    {
                        return Activator.CreateInstance(type);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        
    }
}
