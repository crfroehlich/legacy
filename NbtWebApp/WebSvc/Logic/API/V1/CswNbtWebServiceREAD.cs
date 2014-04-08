﻿using System;
using System.Collections.Generic;
using System.Net;
using ChemSW;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using NbtWebApp.Services;
using NbtWebApp.WebSvc.Logic.API.DataContracts;

namespace NbtWebApp.WebSvc.Logic.API
{
    public class CswNbtWebServiceREAD: CswNbtWebServiceAPI
    {
        public const string VERB = "GET";

        #region Non-Static

        public CswNbtWebServiceREAD( CswNbtResources NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        protected override bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            return hasPermission( _CswNbtResources, CswEnumNbtNodeTypePermission.View, GenericRequest, Return );
        }

        public void GetResource( CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtNode Node = _CswNbtResources.Nodes.GetNode( GenericRequest.NodeId );
                    if( null != Node )
                    {
                        Return.NodeId = GenericRequest.NodeId;
                        Return.NodeName = Node.NodeName;
                        Return.NodeType = Node.getNodeType().NodeTypeName;
                        Return.ObjectClass = Node.ObjClass.ObjectClass.ObjectClassName;
                        CswNbtMetaDataPropertySet propSet = Node.getObjectClass().getPropertySet();
                        if( null != propSet )
                        {
                            Return.PropertySet = propSet.Name;
                        }
                        Return.URI = "api/v1/" + Node.getNodeType().NodeTypeName + "/" + GenericRequest.NodeId.PrimaryKey; //TODO: this URI needs to have the "localhost/NbtDev" part

                        CswNbtSdTabsAndProps SdTabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );
                        CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                        //TODO: better way to get property data - we're forcing it to be by tab...we should get ALL properties, regardless of what tab they're on
                        Return.PropertyData = ConvertPropertyData( CswConvert.ToJObject( SdTabsAndProps.getProps( Node, NodeType.getFirstNodeTypeTab().TabId.ToString(), null, CswEnumNbtLayoutType.Edit )["properties"] ) );
                    }
                    else
                    {
                        Return.Status = HttpStatusCode.NotFound;
                    }
                }
                catch( Exception )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }

        }

        public void GetCollection( CswNbtResourceCollection Return, CswNbtAPIGenericRequest GenericRequest )
        {
            if( hasPermission( GenericRequest, Return ) )
            {
                try
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( GenericRequest.MetaDataName );
                    if( null != NodeType )
                    {
                        foreach( KeyValuePair<CswPrimaryKey, string> pair in NodeType.getNodeIdAndNames( false, false ) )
                        {
                            Return.Add( pair.Value, pair.Key, NodeType.NodeTypeName, string.Empty, string.Empty );
                        }
                    }
                    else
                    {
                        Return.Status = HttpStatusCode.NotFound;
                    }
                }
                catch( Exception )
                {
                    Return.Status = HttpStatusCode.InternalServerError;
                }
            }
        }

        #endregion

        #region Static

        public static void GetResource( ICswResources CswResources, CswNbtResource Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceREAD GET = new CswNbtWebServiceREAD( (CswNbtResources) CswResources );
            GET.GetResource( Return, GenericRequest );
        }

        public static void GetCollection( ICswResources CswResources, CswNbtResourceCollection Return, CswNbtAPIGenericRequest GenericRequest )
        {
            CswNbtWebServiceREAD GET = new CswNbtWebServiceREAD( (CswNbtResources) CswResources );
            GET.GetCollection( Return, GenericRequest );
        }

        #endregion
    }
}