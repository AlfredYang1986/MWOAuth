//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using System.ComponentModel;
//using System.ServiceModel;
//using System.ServiceProcess;
//using System.Configuration;
//using System.Configuration.Install;

//namespace MWAuthorizationServer
//{
//    class MWUrlEncodedService : ServiceBase
//    {
//         public ServiceHost serviceHost = null;
//         public MWUrlEncodedService()
//    {
//        // Name the Windows Service
//        ServiceName = "UrlEncodedHelperService";
//    }

//    public static void Main()
//    {
//        ServiceBase.Run(new MWUrlEncodedService());
//    }
//    // Start the Windows service.
//    protected override void OnStart(string[] args)
//    {
//            //string baseAddress = "http://localhost:1296/Service.svc";//"http://" + Environment.MachineName + ":8000/Service";
//            //ServiceHost host = new ServiceHost(typeof(MWAuthorizationImpl), new Uri(baseAddress));
//            //host.AddServiceEndpoint(typeof(IMWAuthorizationServer), new WebHttpBinding(), "").Behaviors.Add(new MWWebHTTPBehaviorHelper());
//            //host.Open();
//            //Console.WriteLine("Host opened");
        
//        if (serviceHost != null)
//        {
//            serviceHost.Close();
//        }

//        // Create a ServiceHost for the CalculatorService type and 
//        // provide the base address.
//        serviceHost = new ServiceHost(typeof(MWAuthorizationImpl));

//        // Open the ServiceHostBase to create listeners and start 
//        // listening for messages.
//        serviceHost.Open();
//    }
//    protected override void OnStop()
//    {
//        if (serviceHost != null)
//        {
//            serviceHost.Close();
//            serviceHost = null;
//        }
//    }
//    }
//}
