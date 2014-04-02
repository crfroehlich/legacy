﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.Services
{
    /// <summary>
    /// Web service for configuration variable management
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ConfigurationVariables 
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Initialize" )]
        [Description( "Initialize the configuration variables page" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtConfigurationVariablesPageReturn Initialize()
        {
            CswNbtConfigurationVariablesPageReturn ret = new CswNbtConfigurationVariablesPageReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtConfigurationVariablesPageReturn, object>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : ret,
                WebSvcMethodPtr : CswNbtWebServiceConfigurationVariables.Initialize,
                ParamObj : null
                );
        
            SvcDriver.run();
            return ( ret );

        }
    }

    public class CswNbtConfigurationVariablesPageReturn : CswWebSvcReturn
    {
        public CswNbtConfigurationVariablesPageReturn()
        {
            Data = new CswNbtDataContractConfigurationVariablesPage();
        }

        [DataMember] public CswNbtDataContractConfigurationVariablesPage Data;
    }

} //namespace NbtWebApp.Services
