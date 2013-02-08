﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.WebSvc;
using ChemSW.Nbt.WebServices;

namespace NbtWebApp
{
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class KioskMode
    {
        private HttpContext _Context = HttpContext.Current;

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "GetAvailableModes" )]
        [Description( "Get all available modes for CISPro Kiosk Mode" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceKioskMode.KioskModeDataReturn GetAvailableModes()
        {
            CswNbtWebServiceKioskMode.KioskModeDataReturn Ret = new CswNbtWebServiceKioskMode.KioskModeDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceKioskMode.KioskModeDataReturn, CswNbtWebServiceKioskMode.KioskModeData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceKioskMode.GetAvailableModes,
                ParamObj: null
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "HandleScan" )]
        [Description( "Handle a scanned item" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceKioskMode.KioskModeDataReturn DetermineDisplayProps( CswNbtWebServiceKioskMode.KioskModeData RequestData )
        {
            CswNbtWebServiceKioskMode.KioskModeDataReturn Ret = new CswNbtWebServiceKioskMode.KioskModeDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceKioskMode.KioskModeDataReturn, CswNbtWebServiceKioskMode.KioskModeData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceKioskMode.HandleScan,
                ParamObj: RequestData
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "CommitOperation" )]
        [Description( "Perform the selected operation with the given data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceKioskMode.KioskModeDataReturn CommitOperation( CswNbtWebServiceKioskMode.KioskModeData RequestData )
        {
            CswNbtWebServiceKioskMode.KioskModeDataReturn Ret = new CswNbtWebServiceKioskMode.KioskModeDataReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceKioskMode.KioskModeDataReturn, CswNbtWebServiceKioskMode.KioskModeData>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceKioskMode.CommitOperation,
                ParamObj: RequestData
                );

            SvcDriver.run();
            return ( Ret );
        }
    }

}