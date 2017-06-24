using Autofac;
using BotAuth.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac.Integration.WebApi;


namespace InsuranceBoT
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);          

        }
    }
}
