﻿using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Logic.Labels;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Labels2
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
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get a collection of EPL texts for the selected Targets" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelEpl getLabel( NbtPrintLabel.Request.Get Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelEpl Ret = new CswNbtLabelEpl();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelEpl, NbtPrintLabel.Request.Get>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.getEPLText,
                ParamObj: Request //new NbtPrintLabel.Request.Get { LabelId = PrintLabelId, TargetId = TargetId }
                );

            SvcDriver.run();
            return ( Ret );
        }

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "attempt to register a label printer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelPrinterReg registerLpc( LabelPrinter Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelPrinterReg Ret = new CswNbtLabelPrinterReg();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelPrinterReg, LabelPrinter>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.registerLpc,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
        
        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "retrieve the next job for a label printer" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelJobResponse getNextLpcJob( CswNbtLabelJobRequest Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelJobResponse Ret = new CswNbtLabelJobResponse();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelJobResponse, CswNbtLabelJobRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.nextLabelJob,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "update the state of a label printing job" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtLabelJobUpdateResponse updateLpcJob( CswNbtLabelJobUpdateRequest Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            CswNbtLabelJobUpdateResponse Ret = new CswNbtLabelJobUpdateResponse();
            var SvcDriver = new CswWebSvcDriver<CswNbtLabelJobUpdateResponse, CswNbtLabelJobUpdateRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServicePrintLabels.updateLabelJob,
                ParamObj: Request
                );

            SvcDriver.run();
            return ( Ret );
        }
    }
}
