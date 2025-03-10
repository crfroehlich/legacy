﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Menus
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "getSearchMenuItems" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceSearchMenu.CswNbtSearchMenuReturn getSearchMenuItems( bool UniversalSearchOnly )
        {
            CswNbtWebServiceSearchMenu.CswNbtSearchMenuReturn Ret = new CswNbtWebServiceSearchMenu.CswNbtSearchMenuReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceSearchMenu.CswNbtSearchMenuReturn, bool>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceSearchMenu.GetSearchMenuItems,
                ParamObj: UniversalSearchOnly
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "initImpersonate" )]
        [Description( "" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceMainMenu.CswNbtMainMenuReturn initImpersonate()
        {
            CswNbtWebServiceMainMenu.CswNbtMainMenuReturn Ret = new CswNbtWebServiceMainMenu.CswNbtMainMenuReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceMainMenu.CswNbtMainMenuReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceMainMenu.initializeImpersonate,
                ParamObj: null
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}
