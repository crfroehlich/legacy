﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMobile
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly bool _ForMobile;
        private readonly Int32 _MobilePageSize = 30;

        public CswNbtWebServiceMobile( CswNbtResources CswNbtResources, bool ForMobile )
        {
            _CswNbtResources = CswNbtResources;
            _ForMobile = ForMobile;
            string PageSize = _CswNbtResources.getConfigVariableValue( CswNbtResources.ConfigurationVariables.mobileview_resultlim.ToString() );
            if( CswTools.IsInteger( PageSize ) )
            {
                _MobilePageSize = CswConvert.ToInt32( PageSize );
            }
        }

        #region Get

        private const string PropIdPrefix = "prop_";
        private const string NodeIdPrefix = "nodeid_";


        public JObject getViewsList( string ParentId, ICswNbtUser CurrentUser )
        {
            // All Views
            JObject RetJson = new JObject();
            Collection<CswNbtView> MobileViews = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, CurrentUser, false, _ForMobile, false, NbtViewRenderingMode.Any );
            foreach( CswNbtView MobileView in MobileViews )
            {
                RetJson.Add( new JProperty( MobileView.ViewId.ToString(), MobileView.ViewName ) );
            }

            return RetJson;
        } // Run()

        public JObject getView( string ViewId, ICswNbtUser CurrentUser )
        {
            JObject RetJson = new JObject();

            // Get the full XML for the entire view
            CswNbtViewId NbtViewId = new CswNbtViewId( ViewId );
            if( null != NbtViewId && NbtViewId.isSet() )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( NbtViewId );

                // case 20083
                if( _ForMobile )
                {
                    RetJson.Add( _getSearchNodes( View ) );
                }

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false, _MobilePageSize );

                if( Tree.getChildNodeCount() > 0 )
                {
                    JProperty ParentNode = new JProperty( "nodes" );
                    JObject Nodes = new JObject();
                    ParentNode.Value = Nodes;
                    _runTreeNodesRecursive( Tree, ref Nodes );
                    RetJson.Add( ParentNode );
                }
                else
                {
                    RetJson.Add( new JProperty( "nodes",
                                                new JObject(
                                                    new JProperty( NodeIdPrefix + Int32.MinValue, "No Results" ) ) ) );
                }
            }

            return RetJson;
        } // Run()

        // case 20083 - search options
        private JProperty _getSearchNodes( CswNbtView View )
        {
            JProperty ReturnJson = new JProperty( "searches",
                new JObject(
                    from CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes
                    where View.ContainsNodeType( NodeType )
                    from CswNbtMetaDataNodeTypeProp MetaDataProp in NodeType.NodeTypeProps
                    where MetaDataProp.MobileSearch
                    let PropId = ( MetaDataProp.ObjectClassProp != null ) ? "search_ocp_" + MetaDataProp.ObjectClassPropId : "search_ntp_" + MetaDataProp.PropId
                    select new JProperty( PropId, CswTools.SafeJavascriptParam( MetaDataProp.PropNameWithQuestionNo ) ) ) );
            return ReturnJson;
        } // _getSearchNodes

        private void _runTreeNodesRecursive( ICswNbtTree Tree, ref JObject ParentJsonO )
        {
            for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
            {
                Tree.goToNthChild( c );

                CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
                CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

                JProperty ThisJProp = new JProperty( "subitems" );
                JObject ThisSubItems = new JObject();
                ThisJProp.Value = ThisSubItems;

                _runTreeNodesRecursive( Tree, ref ThisSubItems );

                if( Tree.getNodeShowInTreeForCurrentPosition() )
                {
                    bool RunProps = ( _ForMobile && Tree.getChildNodeCount() == 0 && NodeSpecies.More != ThisNodeKey.NodeSpecies );   // is a leaf
                    ParentJsonO.Add( _getNode( ThisNode, RunProps, ThisNodeKey.NodeSpecies ) );
                } // if( Tree.getNodeShowInTreeForCurrentPosition() )
                else
                {
                    ParentJsonO.Add( ThisSubItems );
                }
                Tree.goToParentNode();
            }
        } // _runTreeNodesRecursive()

        public JObject getNode( string NodeId )
        {
            CswDelimitedString NodeStr = new CswDelimitedString( '_' );
            NodeStr.FromString( NodeId );
            if( NodeStr[0] == "nodeid" )
            {
                NodeStr.RemoveAt( 0 );
            }
            string NodePk = NodeStr.ToString();
            JObject Ret = new JObject();
            Ret.Add( _getNode( NodePk, true ) );
            return Ret;
        }

        private JProperty _getNode( string NodePkStr, bool RunProps = true )
        {
            CswPrimaryKey NodePk = new CswPrimaryKey();
            NodePk.FromString( NodePkStr );
            CswNbtNode ThisNode = _CswNbtResources.Nodes.GetNode( NodePk );
            return _getNode( ThisNode, RunProps );
        }

        private JProperty _getNode( CswNbtNode ThisNode, bool RunProps = true, NodeSpecies ThisNodeSpecies = NodeSpecies.UnKnown )
        {
            JProperty Ret = new JProperty( "No Results" );
            if( null != ThisNode )
            {
                JProperty ThisJProp = new JProperty( "subitems" );
                JObject ThisSubItems = new JObject();
                ThisJProp.Value = ThisSubItems;

                string ThisNodeName = ThisNode.NodeName;
                if( RunProps )
                {
                    _runProperties( ThisNode, ref ThisSubItems );
                }

                Ret = new JProperty( NodeIdPrefix + ThisNode.NodeId );
                JObject NodeProps = new JObject();
                Ret.Value = NodeProps;

                NodeSpecies NodeSpecie = ( ThisNodeSpecies != NodeSpecies.UnKnown ) ? ThisNodeSpecies : ThisNode.NodeSpecies;
                if( NodeSpecies.More == NodeSpecie )
                {
                    ThisNodeName = "Results Truncated at " + _MobilePageSize;
                }

                NodeProps["node_name"] = CswTools.SafeJavascriptParam( ThisNodeName );
                NodeProps["nodetype"] = CswTools.SafeJavascriptParam( ThisNode.NodeType.NodeTypeName );
                NodeProps["objectclass"] = CswTools.SafeJavascriptParam( ThisNode.ObjectClass.ObjectClass.ToString() );
                NodeProps["nodespecies"] = CswTools.SafeJavascriptParam( NodeSpecie.ToString() );
                NodeProps.Add( ThisJProp );

                if( false == string.IsNullOrEmpty( ThisNode.NodeType.IconFileName ) )
                {
                    NodeProps["iconfilename"] = CswTools.SafeJavascriptParam( ThisNode.NodeType.IconFileName );
                }

                _addObjectClassProps( ThisNode, ref NodeProps );

                foreach( CswNbtMetaDataNodeTypeProp MetaDataProp in ThisNode.NodeType.NodeTypeProps
                                                                            .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                            .Where( MetaDataProp => MetaDataProp.MobileSearch ) )
                {
                    if( ( MetaDataProp.ObjectClassProp != null ) )
                    {
                        NodeProps["search_ocp_" + MetaDataProp.ObjectClassPropId] = CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt );
                    }
                    else
                    {
                        NodeProps["search_ntp_" + MetaDataProp.PropId] = CswTools.SafeJavascriptParam( ThisNode.Properties[MetaDataProp].Gestalt );
                    }

                }
            }
            return Ret;
        }

        private static void _addObjectClassProps( CswNbtNode Node, ref JObject NodeProps )
        {
            switch( Node.ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                    {
                        string Location = Node.Properties[CswNbtObjClassInspectionDesign.LocationPropertyName].Gestalt;
                        string DueDate = Node.Properties[CswNbtObjClassInspectionDesign.DatePropertyName].Gestalt;
                        string Status = Node.Properties[CswNbtObjClassInspectionDesign.StatusPropertyName].Gestalt;
                        string Target = Node.Properties[CswNbtObjClassInspectionDesign.TargetPropertyName].Gestalt;
                        NodeProps["location"] = Location;
                        NodeProps["duedate"] = DueDate;
                        NodeProps["status"] = Status;
                        NodeProps["target"] = Target;
                        break;
                    }
            }
        }

        private static void _runProperties( CswNbtNode Node, ref JObject SubItemsJProp )
        {
            Collection<CswNbtMetaDataNodeTypeTab> Tabs = new Collection<CswNbtMetaDataNodeTypeTab>();
            foreach( CswNbtMetaDataNodeTypeTab Tab in Node.NodeType.NodeTypeTabs )
            {
                Tabs.Add( Tab );
            }
            for( Int32 i = 0; i < Tabs.Count; i++ )
            {
                CswNbtMetaDataNodeTypeTab CurrentTab = Tabs[i];

                JProperty TabProp = new JProperty( CurrentTab.TabName );
                SubItemsJProp.Add( TabProp );

                JObject TabObj = new JObject();
                TabProp.Value = TabObj;

                //if( i > 1 )
                //{
                //    CswNbtMetaDataNodeTypeTab PrevTab = Tabs[i - 1];
                //    TabObj.Add( new JProperty( "prevtab", PrevTab.TabName ) );
                //}
                if( i < Tabs.Count - 1 )
                {
                    CswNbtMetaDataNodeTypeTab NextTab = Tabs[i + 1];
                    TabObj.Add( new JProperty( "nexttab", NextTab.TabName ) );
                }

                foreach( CswNbtMetaDataNodeTypeProp Prop in CurrentTab.NodeTypePropsByDisplayOrder
                                                                .Cast<CswNbtMetaDataNodeTypeProp>()
                                                                .Where( Prop => !Prop.HideInMobile &&
                                                                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Password &&
                                                                        Prop.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Grid ) )
                {
                    CswNbtNodePropWrapper PropWrapper = Node.Properties[Prop];


                    JProperty ThisProp = new JProperty( PropIdPrefix + Prop.PropId + "_" + NodeIdPrefix + Node.NodeId );
                    TabObj.Add( ThisProp );

                    JObject ThisPropAttr = new JObject(
                                                new JProperty( "prop_name", CswTools.SafeJavascriptParam( Prop.PropNameWithQuestionNo ) ),
                                                new JProperty( "fieldtype", Prop.FieldType.FieldType.ToString() ),
                                                new JProperty( "gestalt", CswTools.SafeJavascriptParam( PropWrapper.Gestalt ) ),
                                                new JProperty( "ocpname", CswTools.SafeJavascriptParam( PropWrapper.ObjectClassPropName ) )
                                           );
                    if( Node.ReadOnly || Prop.ReadOnly )
                    {
                        ThisPropAttr.Add( new JProperty( "isreadonly", true ) );
                    }
                    PropWrapper.ToJSON( ThisPropAttr );
                    ThisProp.Value = ThisPropAttr;
                }

            }
        }

        // _runProperties()

        #endregion Get

        #region Set

        public bool updateViewProps( string UpdatedViewJson )
        {
            bool Ret = false;
            JObject UpdatedJSON = JObject.Parse( UpdatedViewJson );
            if( null != UpdatedJSON.Property( "nodes" ) )
            {
                // this is a view
                JObject Nodes = (JObject) UpdatedJSON.Property( "nodes" ).Value;
                Ret = _updateViewProps( Nodes );
            }
            else if( null != UpdatedJSON.Property( "node_name" ) )
            {
                // this is a node
                Ret = _updateNodeProps( UpdatedJSON );
            }
            else
            {
                // this is a node collection
                Ret = _updateViewProps( UpdatedJSON );
            }
            return Ret;
        }

        public bool updateNodesProps( string UpdatedNodeJson )
        {
            JObject UpdatedNode = JObject.Parse( UpdatedNodeJson );
            return _updateNodeProps( UpdatedNode );
        } // Run()

        private bool _updateViewProps( JObject UpdatedNode )
        {
            bool Ret = false;
            foreach( JObject Node in UpdatedNode.Properties()
                                                .Select( Prop => (JObject) Prop.Value ) )
            {
                Ret = _updateNodeProps( Node ) || Ret;
            }
            return Ret;
        }

        private bool _updateNodeProps( JObject NodeObj )
        {
            bool Ret = false;
            Collection<JProperty> Props = new Collection<JProperty>();


            if( null != NodeObj.Property( "subitems" ) )
            {
                JObject Tabs = (JObject) NodeObj.Property( "subitems" ).Value;
                foreach( JProperty Prop in from Tab
                                               in Tabs.Properties()
                                           where ( null != Tab.Value )
                                           select (JObject) Tab.Value
                                               into TabProps
                                               from Prop
                                                   in TabProps.Properties()
                                               where ( null != Prop.Value &&
                                                        Prop.Name != "nexttab" )
                                               let PropAtr = (JObject) Prop.Value
                                               where null != PropAtr.Property( "wasmodified" ) &&
                                                     CswConvert.ToBoolean( PropAtr.Property( "wasmodified" ).Value )
                                               select Prop )
                {
                    Props.Add( Prop );
                }
            }


            // post changes once per node, not once per prop            
            Collection<CswNbtNode> NodesToPost = new Collection<CswNbtNode>();

            foreach( JProperty Prop in Props )
            {
                if( null != Prop.Name )
                {
                    string NodePropId = Prop.Name; // ~ "prop_4019_nodeid_nodes_24709"
                    string[] SplitNodePropId = NodePropId.Split( '_' );
                    Int32 NodeTypePropId = CswConvert.ToInt32( SplitNodePropId[1] );
                    CswPrimaryKey NodePk = new CswPrimaryKey( SplitNodePropId[3], CswConvert.ToInt32( SplitNodePropId[4] ) );

                    CswNbtNode Node = _CswNbtResources.Nodes[NodePk];
                    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );

                    //Case 20964. Client needs to know whether the inspection is complete.
                    if( !Ret && Node.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                    {
                        CswNbtMetaDataObjectClassProp Finished = Node.ObjectClass.getObjectClassProp( CswNbtObjClassInspectionDesign.FinishedPropertyName );
                        CswNbtMetaDataObjectClassProp Cancelled = Node.ObjectClass.getObjectClassProp( CswNbtObjClassInspectionDesign.CancelledPropertyName );
                        if( ( MetaDataProp.ObjectClassProp == Finished &&
                            Tristate.True == CswConvert.ToTristate( Prop.Value ) ) ||
                            ( MetaDataProp.ObjectClassProp == Cancelled )
                            )
                        {
                            Ret = true;
                        }
                    }

                    JObject PropObj = (JObject) Prop.Value;

                    Node.Properties[MetaDataProp].ReadJSON( PropObj, null, null );

                    if( !NodesToPost.Contains( Node ) )
                    {
                        NodesToPost.Add( Node );
                    }
                }
            }

            foreach( CswNbtNode Node in NodesToPost )
            {
                Node.postChanges( false );
            }
            return Ret;
        }

        #endregion Set

    } // class CswNbtWebServiceMobileView

} // namespace ChemSW.Nbt.WebServices
