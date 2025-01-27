﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Import
    {
        private HttpContext _Context = HttpContext.Current;
        private static CswWebSvcSessionAuthenticateData.Authentication.Request AuthRequest
        {
            get
            {
                CswWebSvcSessionAuthenticateData.Authentication.Request Ret = new CswWebSvcSessionAuthenticateData.Authentication.Request();
                return Ret;
            }
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get possible import definitions" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportDefsReturn getImportDefs()
        {
            CswNbtImportWcf.ImportDefsReturn ret = new CswNbtImportWcf.ImportDefsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportDefsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportDefs,
                ParamObj: null
                );

            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "GET", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get existing import jobs" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportJobsReturn getImportJobs()
        {
            CswNbtImportWcf.ImportJobsReturn ret = new CswNbtImportWcf.ImportJobsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportJobsReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportJobs,
                ParamObj: null
                );

            SvcDriver.run();
            return ( ret );
        }

        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "uploadImportData?defname={ImportDefName}&overwrite={Overwrite}" )]
        [Description( "Upload Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportDataReturn uploadImportData( string ImportDefName, bool Overwrite )
        {
            CswNbtImportWcf.ImportDataReturn ret = new CswNbtImportWcf.ImportDataReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                CswNbtImportWcf.ImportFileParams parms = new CswNbtImportWcf.ImportFileParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;
                parms.Overwrite = Overwrite;

                var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportDataReturn, CswNbtImportWcf.ImportFileParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceImport.uploadImportData,
                    ParamObj: parms
                    );

                SvcDriver.run();
            }

            return ret;
        }


        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Download Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream downloadImportData( Stream DataStream )
        {
            string Data = new StreamReader( DataStream ).ReadToEnd();
            NameValueCollection FormData = HttpUtility.ParseQueryString( Data );
            string Filename = FormData["filename"];

            CswNbtImportWcf.GenerateSQLReturn Ret = new CswNbtImportWcf.GenerateSQLReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.GenerateSQLReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.downloadImportData,
                ParamObj: Filename
                );

            SvcDriver.run();

            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-Disposition", "attachment; filename=\"" + Filename + "\";" );
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/vnd.ms-excel";
            return Ret.stream;
        }//downloadImportDefinition



        [OperationContract]
        [WebInvoke( Method = "POST", UriTemplate = "uploadImportDefinition?defname={ImportDefName}" )]
        [Description( "Upload Import Data" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn uploadImportDefinition( string ImportDefName )
        {
            CswWebSvcReturn ret = new CswWebSvcReturn();

            if( _Context.Request.Files.Count > 0 )
            {
                CswNbtImportWcf.ImportFileParams parms = new CswNbtImportWcf.ImportFileParams();
                parms.PostedFile = _Context.Request.Files[0];
                parms.ImportDefName = ImportDefName;

                var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.ImportFileParams>(
                    CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                    ReturnObj: ret,
                    WebSvcMethodPtr: CswNbtWebServiceImport.uploadImportDefinition,
                    ParamObj: parms
                    );

                SvcDriver.run();
            }

            return ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Download Import Definition" )]
        [FaultContract( typeof( FaultException ) )]
        public Stream downloadImportDefinition( Stream DataStream )
        {
            string Data = new StreamReader( DataStream ).ReadToEnd();
            NameValueCollection FormData = HttpUtility.ParseQueryString( Data );
            string ImportDefName = FormData["importdefname"];

            CswNbtImportWcf.GenerateSQLReturn Ret = new CswNbtImportWcf.GenerateSQLReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.GenerateSQLReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.downloadImportDefinition,
                ParamObj: ImportDefName
                );

            SvcDriver.run();

            WebOperationContext.Current.OutgoingResponse.Headers.Set( "Content-Disposition", "attachment; filename=\"" + ImportDefName + "bindings.xls\";" );
            WebOperationContext.Current.OutgoingResponse.ContentType = "application/vnd.ms-excel";
            return Ret.stream;
        }//downloadImportDefinition


        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Update a binding definition" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn updateImportDefinition( CswNbtImportWcf.DefinitionUpdateRow[] parms )
        {
            CswWebSvcReturn ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.DefinitionUpdateRow[]>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.updateImportDefinition,
                ParamObj: parms
                );

            SvcDriver.run();

            return ret;
        }



        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get current status of imports" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportStatusReturn getImportStatus( CswNbtImportWcf.JobRequest parms )
        {
            CswNbtImportWcf.ImportStatusReturn ret = new CswNbtImportWcf.ImportStatusReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportStatusReturn, CswNbtImportWcf.JobRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getImportStatus,
                ParamObj: parms
                );

            SvcDriver.run();

            return ret;
        } // getImportStatus()

        [OperationContract]
        [WebInvoke( Method = "POST", ResponseFormat = WebMessageFormat.Json )]
        [Description( "Get current status of imports" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn cancelJob( CswNbtImportWcf.JobRequest parms )
        {
            CswWebSvcReturn ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.JobRequest>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, AuthRequest ),
                ReturnObj: ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.cancelJob,
                ParamObj: parms
                );

            SvcDriver.run();

            return ret;
        } // cancelJob()

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Start import" )]
        [FaultContract( typeof( FaultException ) )]
        public CswWebSvcReturn startImport( CswNbtImportWcf.StartImportParams Params )
        {
            CswWebSvcReturn Ret = new CswWebSvcReturn();

            var SvcDriver = new CswWebSvcDriver<CswWebSvcReturn, CswNbtImportWcf.StartImportParams>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.startCAFImport,
                ParamObj: Params
                );

            SvcDriver.run();

            return Ret;
        }//startImport()


        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Retrieve bindings for current definition" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.ImportBindingsReturn getBindingsForDefinition( string ImportDefName )
        {
            CswNbtImportWcf.ImportBindingsReturn Ret = new CswNbtImportWcf.ImportBindingsReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.ImportBindingsReturn, string>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getBindingsForDefinition,
                ParamObj: ImportDefName
                );

            SvcDriver.run();

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Get list of existing nodes in system" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.DltExistingNodesReturn getExistingNodes()
        {
            CswNbtImportWcf.DltExistingNodesReturn Ret = new CswNbtImportWcf.DltExistingNodesReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.DltExistingNodesReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.getExistingNodes,
                ParamObj: null
                );

            SvcDriver.run();

            return Ret;
        }

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [Description( "Delete existing nodes from the system" )]
        [FaultContract( typeof( FaultException ) )]
        public CswNbtImportWcf.DltExistingNodesReturn deleteExistingNodes()
        {
            CswNbtImportWcf.DltExistingNodesReturn Ret = new CswNbtImportWcf.DltExistingNodesReturn();

            var SvcDriver = new CswWebSvcDriver<CswNbtImportWcf.DltExistingNodesReturn, object>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceImport.deleteExistingNodes,
                ParamObj: null
                );

            SvcDriver.run();

            return Ret;
        }


    }
}
