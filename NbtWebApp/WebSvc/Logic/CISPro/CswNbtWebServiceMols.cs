using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.StructureSearch;
using NbtWebApp.WebSvc.Logic.CISPro;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMols
    {
        #region Ctor

        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMols( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #endregion Ctor

        #region DataContract

        [DataContract]
        public class MolDataReturn: CswWebSvcReturn
        {
            public MolDataReturn()
            {
                Data = new MolData();
            }
            [DataMember]
            public MolData Data;
        }

        [DataContract]
        public class StructureSearchDataReturn: CswWebSvcReturn
        {
            public StructureSearchDataReturn()
            {
                Data = new StructureSearchViewData();
            }
            [DataMember]
            public StructureSearchViewData Data;
        }

        #endregion

        #region Public

        public static void getMolImg( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            string molData = ImgData.molString;
            string nodeId = ImgData.nodeId;
            string base64String = "";

            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            if( String.IsNullOrEmpty( molData ) && false == String.IsNullOrEmpty( nodeId ) ) //if we only have a nodeid, get the mol text from the mol property if there is one
            {
                CswPrimaryKey pk = CswConvert.ToPrimaryKey( nodeId );
                CswNbtNode node = NbtResources.Nodes[pk];
                CswNbtMetaDataNodeTypeProp molNTP = node.getNodeType().getMolProperty();
                if( null != molNTP )
                {
                    molData = node.Properties[molNTP].AsMol.Mol;
                }
            }

            if( false == String.IsNullOrEmpty( molData ) )
            {
                byte[] bytes = CswStructureSearch.GetImage( molData );
                base64String = Convert.ToBase64String( bytes );
            }

            ImgData.molImgAsBase64String = base64String;
            ImgData.molString = molData;
            Return.Data = ImgData;
        }

        public static void RunStructureSearch( ICswResources CswResources, StructureSearchDataReturn Return, StructureSearchViewData StructureSearchData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            string molData = StructureSearchData.molString;
            bool exact = StructureSearchData.exact;

            Dictionary<int, string> results = NbtResources.StructureSearchManager.RunSearch( molData, exact );
            CswNbtView searchView = new CswNbtView( NbtResources );
            searchView.SetViewMode( NbtViewRenderingMode.Table );
            searchView.Category = "Recent";
            searchView.ViewName = "Structure Search Results";

            if( results.Count > 0 )
            {
                CswNbtMetaDataObjectClass materialOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtViewRelationship parent = searchView.AddViewRelationship( materialOC, false );

                foreach( int nodeId in results.Keys )
                {
                    CswPrimaryKey pk = new CswPrimaryKey( "nodes", nodeId );
                    parent.NodeIdsToFilterIn.Add( pk );
                }
            }

            searchView.SaveToCache( false );

            StructureSearchData.viewId = searchView.SessionViewId.ToString();
            StructureSearchData.viewMode = searchView.ViewMode.ToString();
            Return.Data = StructureSearchData;
        }

        public static void SaveMolPropFile( ICswResources CswResources, MolDataReturn Return, MolData ImgData )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;
            CswPropIdAttr PropId = new CswPropIdAttr( ImgData.propId );
            CswNbtMetaDataNodeTypeProp MetaDataProp = NBTResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );

            CswNbtNode Node = NBTResources.Nodes[PropId.NodeId];
            if( null != Node )
            {
                CswNbtNodePropMol molProp = Node.Properties[MetaDataProp];
                if( null != molProp )
                {
                    //Save the mol text to jct_nodes_props
                    CswTableUpdate JctUpdate = NBTResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );

                    if( Int32.MinValue != molProp.JctNodePropId )
                    {
                        DataTable JctTable = JctUpdate.getTable( "jctnodepropid", molProp.JctNodePropId );
                        JctTable.Rows[0]["clobdata"] = ImgData.molString;
                        JctUpdate.update( JctTable );
                    }
                    else
                    {
                        DataTable JctTable = JctUpdate.getEmptyTable();
                        DataRow JRow = JctTable.NewRow();
                        JRow["nodetypepropid"] = CswConvert.ToDbVal( PropId.NodeTypePropId );
                        JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                        JRow["nodeidtablename"] = Node.NodeId.TableName;
                        JRow["clobdata"] = ImgData.molString;
                        JctTable.Rows.Add( JRow );
                        JctUpdate.update( JctTable );
                    }

                    //Save the mol image to blob_data
                    byte[] molImage = CswStructureSearch.GetImage( ImgData.molString );
                    string Href = "";

                    CswNbtWebServiceBinaryData wsBinData = new CswNbtWebServiceBinaryData( NBTResources );
                    wsBinData.saveFile( PropId.ToString(), molImage, "image/jpeg", "Mol.jpeg", out Href );

                    ImgData.href = Href;

                    //case 28364 - calculate fingerprint and save it
                    NBTResources.StructureSearchManager.InsertFingerprintRecord( PropId.NodeId.PrimaryKey, ImgData.molString );
                }
            }

            Return.Data = ImgData;
        }

        public static void ClearMolFingerprint( ICswResources CswResources, MolDataReturn Return, MolData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey pk = new CswPrimaryKey();
            pk.FromString( Request.nodeId );
            NbtResources.StructureSearchManager.DeleteFingerprintRecord( pk.PrimaryKey );
        }

        #endregion
    }

}