﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Labels;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Labels
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get all Print labels matching this Target Type, selecting the Target Id's Label Format by default" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelList list( NbtPrintLabel.Request.List Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelList Ret = new CswNbtLabelList();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelList, NbtPrintLabel.Request.List>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getLabels,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebGet( UriTemplate = "label/{PrintLabelId}/target/{TargetId}", BodyStyle = WebMessageBodyStyle.Wrapped )]
        [Description( "Get a collection of EPL texts for the selected Targets" )]
        public CswNbtLabelEpl get( string PrintLabelId, string TargetId )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelEpl Ret = new CswNbtLabelEpl();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelEpl, NbtPrintLabel.Request.Get>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getEPLText,
                ParamObj: new NbtPrintLabel.Request.Get { LabelId = PrintLabelId, TargetId = TargetId }
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Create a print job" )]
        public CswNbtPrintJobReturn newPrintJob( NbtPrintLabel.Request.printJob Request )
        {
            CswNbtPrintJobReturn Ret = new CswNbtPrintJobReturn();
            var SvcDriver = new CswWebSvcDriver<CswNbtPrintJobReturn, NbtPrintLabel.Request.printJob>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.newPrintJob,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}
