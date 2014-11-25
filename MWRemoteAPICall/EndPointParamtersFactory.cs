using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MWRemoteAPICall
{
    class SearchParameter : EndPointParameters { }
    class MethodParameter: EndPointParameters { }
    class AuthParameter: EndPointParameters { }

    public class ParameterFactory : EndPointParameters
    {
        public static EndPointParameters CreateRequestParamters(EndPointParameters p)
        {
            dynamic para = new EndPointParameters();
            para.Request = p;
            return para;
        }

        public static EndPointParameters CreateSearchParamters(string input, int min, int max, int currentPage, string sortName, string sortMethod)
        {
            dynamic para = new SearchParameter();
            para.input = input.Trim();

            para.conditions = new JArray() as dynamic;
            para.notConditions = new JArray() as dynamic;

            dynamic price = new EndPointParameters();
            price.minPrice = 20;
            price.maxPrice = 10000;

            para.price = price;
            para.currentPage = currentPage;
            para.sortName = sortName;
            para.sortMethod = sortMethod;

            return para;
        }

        public static EndPointParameters CreateMethodParamters(string method_name, string str)
        {
            dynamic para = new MethodParameter();
            para.MessageName = method_name;
            para.Parameters = str;
            return para;
        }

        public static EndPointParameters CreateMethodParamters(string method_name, EndPointParameters p)
        {
            dynamic para = new MethodParameter();
            para.MessageName = method_name;
            para.Parameters = JsonConvert.SerializeObject(p);
            return para;
        }

        public static EndPointParameters CreateAuthParamters(string token, EndPointParameters p)
        {
            dynamic para = new AuthParameter();
            para.Token = token;
            para.RequestString = p;
            return para;
        }

        public static string toJSON(EndPointParameters para)
        {
            return JsonConvert.SerializeObject(para).ToString();
        }
    }
}