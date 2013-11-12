﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using NbtWebApp.Actions.ChemWatch;

namespace NbtWebApp
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class ChemWatch
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "Initialize" )]
        [Description( "Get the initialization data for the ChemWatch action" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn Initialize( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.Initialize,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetMatchingSuppliers" )]
        [Description( "Get the initialization data for the ChemWatch action" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn GetMatchingSuppliers( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceChemWatch.GetMatchingSuppliers,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "MaterialSearch" )]
        [Description( "Search for Materials" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtChemWatchReturn MaterialSearch( CswNbtChemWatchRequest Request )
        {
            CswNbtChemWatchReturn Ret = new CswNbtChemWatchReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtChemWatchReturn, CswNbtChemWatchRequest>(
                CswWebSvcResourceInitializer : new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj : Ret,
                WebSvcMethodPtr : CswNbtWebServiceChemWatch.MaterialSearch,
                ParamObj : Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }

}