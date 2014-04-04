﻿using System;
using System.Net;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Logic.API.DataContracts;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.WebSvc.Logic.API
{
    public abstract class CswNbtWebServiceAPI
    {
        public static string AppPath = string.Empty;

        public static int VersionNo = 1;

        protected CswNbtResources _CswNbtResources;
        protected abstract bool hasPermission( CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return );

        /// <summary>
        /// Based on the input, verifies if the user has permission to continue
        /// </summary>
        public bool hasPermission( CswNbtResources NbtResources, CswEnumNbtNodeTypePermission Permission, CswNbtAPIGenericRequest GenericRequest, CswNbtAPIReturn Return )
        {
            bool ret = false;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( GenericRequest.MetaDataName );
            if( null != NodeType )
            {
                if( NbtResources.Permit.canNodeType( Permission, NodeType, User : NbtResources.CurrentNbtUser ) )
                {
                    ret = true;
                }
                else
                {
                    Return.Status = Return.Status = HttpStatusCode.Forbidden; //Permission denied
                }
            }
            else
            {
                Return.Status = Return.Status = HttpStatusCode.NotFound;
            }
            return ret;
        }

        public static string BuildURI( string MetaDataName, int id = Int32.MinValue )
        {
            //We need to extract the full application URI from the request url
            string appUri = ( HttpContext.Current.Request.Url.IsDefaultPort ) ? HttpContext.Current.Request.Url.Host : HttpContext.Current.Request.Url.Authority;
            appUri = String.Format( "{0}://{1}", HttpContext.Current.Request.Url.Scheme, appUri );
            if( HttpContext.Current.Request.ApplicationPath != "/" )
            {
                appUri += HttpContext.Current.Request.ApplicationPath;
            }

            string ret = appUri + "/api/v" + VersionNo + "/" + MetaDataName;
            if( Int32.MinValue != id )
            {
                ret += "/" + id;
            }
            return ret;
        }

        /// <summary>
        /// Converts property data from a JObject to WCF
        /// See CIS-53051
        /// </summary>
        public CswNbtWcfPropCollection ConvertPropertyData( JObject PropData )
        {
            CswNbtWcfPropCollection ret = new CswNbtWcfPropCollection();

            foreach( JProperty OldProp in PropData.Children() )
            {
                CswNbtWcfProperty NewProp = new CswNbtWcfProperty();
                NewProp.PropId = OldProp.Value["id"].ToString();
                NewProp.PropName = OldProp.Value["name"].ToString();
                NewProp.OriginalPropName = OldProp.Value["ocpname"].ToString();
                foreach( JProperty OldPropValue in OldProp.Value["values"].Children() )
                {
                    NewProp.values[OldPropValue.Name] = OldPropValue.Value.ToString();
                }
                ret.addProperty( NewProp );
            }
            return ret;
        }

        public JObject ConvertWcfPropertyData( CswNbtWcfProperty Prop )
        {
            JObject ret = new JObject();
            ret["id"] = Prop.PropId;
            ret["name"] = Prop.PropName;
            ret["ocpname"] = Prop.OriginalPropName;
            JObject values = new JObject();
            foreach( string subFieldStr in Prop.values.Keys )
            {
                object subFieldVal = Prop.values[subFieldStr];
                string subFieldStrOrig = subFieldStr.Replace( '_', ' ' );
                values[subFieldStrOrig] = subFieldVal.ToString();
            }
            ret["values"] = values;

            return ret;
        }


    }
}