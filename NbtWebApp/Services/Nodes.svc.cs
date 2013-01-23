﻿using System.ComponentModel;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.WebServices;
using ChemSW.WebSvc;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp
{
    
    [DataContract]
    public class NodeResponse : CswWebSvcReturn
    {
        public NodeResponse()
        {
            Data = new NodeSelect.Response();
        }
            
        [DataMember]
        public NodeSelect.Response Data = null;
    }
    
    /// <summary>
    /// WCF Web Methods for View operations
    /// </summary>
    [ServiceBehavior( IncludeExceptionDetailInFaults = true )]
    [ServiceContract( Namespace = "NbtWebApp" )]
    [AspNetCompatibilityRequirements( RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed )]
    public class Nodes
    {
        private HttpContext _Context = HttpContext.Current;

        /// <summary>
        /// 
        /// </summary>
        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Generate a Node Select" )]
        public NodeResponse get( NodeSelect.Request Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            NodeResponse Ret = new NodeResponse();
            var GetViewDriverType = new CswWebSvcDriver<NodeResponse, NodeSelect.Request>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNode.getNodes,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        //[OperationContract]
        //[WebInvoke( Method = "POST" )]
        //[FaultContract( typeof( FaultException ) )]
        //[Description( "Get the options for a relationship property" )]
        //public NodeSelect.Response getRelationshipOpts( NodeSelect.PropertyView Request )
        //{
        //    //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
        //    NodeSelect.Response Ret = new NodeSelect.Response();
        //    var GetViewDriverType = new CswWebSvcDriver<NodeSelect.Response, NodeSelect.PropertyView>(
        //        CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
        //        ReturnObj: Ret,
        //        WebSvcMethodPtr: CswNbtWebServiceNode.getRelationshipOpts,
        //        ParamObj: Request
        //        );

        //    GetViewDriverType.run();
        //    return ( Ret );
        //}

        [OperationContract]
        [WebInvoke( Method = "POST" )]
        [FaultContract( typeof( FaultException ) )]
        [Description( "Get all sizes related to a given node" )]
        public NodeResponse getSizes( CswNbtNode.Node Request )
        {
            //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
            NodeResponse Ret = new NodeResponse();
            var GetViewDriverType = new CswWebSvcDriver<NodeResponse, CswNbtNode.Node>(
                CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
                ReturnObj: Ret,
                WebSvcMethodPtr: CswNbtWebServiceNode.getSizes,
                ParamObj: Request
                );

            GetViewDriverType.run();
            return ( Ret );
        }

        //[OperationContract]
        //[WebInvoke( Method = "POST" )]
        //[FaultContract( typeof( FaultException ) )]
        //[Description( "Get the node link for a given node" )]
        //public NodeSelect.NodeLinkResponse getNodeLink( NodeSelect.PropertyView Request )
        //{
        //    //delegate has to be static because you can't create an instance yet: you don't have resources until the delegate is actually called
        //    NodeSelect.NodeLinkResponse Ret = new NodeSelect.NodeLinkResponse();
        //    var GetViewDriverType = new CswWebSvcDriver<NodeSelect.NodeLinkResponse, NodeSelect.PropertyView>(
        //        CswWebSvcResourceInitializer: new CswWebSvcResourceInitializerNbt( _Context, null ),
        //        ReturnObj: Ret,
        //        WebSvcMethodPtr: CswNbtWebServiceNode.getNodeLink,
        //        ParamObj: Request
        //        );

        //    GetViewDriverType.run();
        //    return ( Ret );
        //}
    }
}
