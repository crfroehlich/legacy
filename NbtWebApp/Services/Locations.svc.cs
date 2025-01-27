﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Locations
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Assign specified inventory group to specified locations" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse assignPropsToLocations( CswNbtWebServiceLocationsCis.AssignInventoryGroupData.AssignRequest Request )
        {
            CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse ReturnVal = new CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocationsCis.AssignInventoryGroupResponse, CswNbtWebServiceLocationsCis.AssignInventoryGroupData.AssignRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ReturnVal,
                WebSvcMethodPtr: CswNbtWebServiceLocationsCis.assignPropsToLocations,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( ReturnVal );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Generate a list of Locations for the SI Mobile application" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocations.CswNbtLocationReturn list( bool IsMobile = true )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceLocations.CswNbtLocationReturn Ret = new CswNbtWebServiceLocations.CswNbtLocationReturn();
            CswWebSvcSessionAuthenticateData.Authentication.Request AuthRequest = new CswWebSvcSessionAuthenticateData.Authentication.Request();
            AuthRequest.RequiredModules.Add( CswEnumNbtModuleName.SI );

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocations.CswNbtLocationReturn, bool>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLocations.getLocationsListMobile,
                ParamObj: IsMobile
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Generate a list of Locations for the NBT web application" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocations.CswNbtLocationReturn getLocationsList( string ViewId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtWebServiceLocations.CswNbtLocationReturn Ret = new CswNbtWebServiceLocations.CswNbtLocationReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocations.CswNbtLocationReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLocations.getLocationsList,
                ParamObj: ViewId
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Perform a search of Locations" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtWebServiceLocations.CswNbtLocationReturn searchLocations( CswNbtWebServiceLocations.CswNbtLocationRequest.CswNbtLocationSearch Request )
        {
            CswNbtWebServiceLocations.CswNbtLocationReturn Ret = new CswNbtWebServiceLocations.CswNbtLocationReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtWebServiceLocations.CswNbtLocationReturn, CswNbtWebServiceLocations.CswNbtLocationRequest.CswNbtLocationSearch>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceLocations.searchLocations,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

    }//Locations
}
