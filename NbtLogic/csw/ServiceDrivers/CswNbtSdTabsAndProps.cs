using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Statistics;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdTabsAndProps
    {

        private readonly CswNbtResources _CswNbtResources;

        private readonly bool _IsMultiEdit;
        private readonly bool _ConfigMode;
        private string HistoryTabPrefix = "history_";
        private CswNbtStatisticsEvents _CswNbtStatisticsEvents;

        public CswNbtSdTabsAndProps( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents = null, bool Multi = false, bool ConfigMode = false )
        {
            _CswNbtResources = CswNbtResources;

            _IsMultiEdit = Multi;
            _ConfigMode = ConfigMode;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
        }

        public JObject getTabs( string NodeId, string NodeKey, CswDateTime Date, string filterToPropId )
        {
            JObject Ret = new JObject();
            JObject Tabs = new JObject();
            Ret["tabs"] = Tabs;
            Ret["node"] = new JObject();
            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( NodeId, NodeKey, Date );
            if( null != Node )
            {
                TabOrderModifier = 0;
                CswNbtMetaDataNodeType NodeType = Node.getNodeType();
                Int32 NodeTypeId = Int32.MinValue;
                if( filterToPropId != string.Empty )
                {
                    CswPropIdAttr PropId = new CswPropIdAttr( filterToPropId );
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                    if( null != Prop )
                    {
                        NodeTypeId = Prop.NodeTypeId;
                        foreach( CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout EditLayout in Prop.getEditLayouts().Values )
                        {
                            CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( EditLayout.TabId );
                            if(
                                    ( _ConfigMode || Tab.TabName != CswNbtMetaData.IdentityTabName ) && (
                                        ( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, NodeType ) ||
                                          _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, NodeType, Tab ) ) &&
                                        (
                                         _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Edit, NodeType, Node.NodeId ) &&
                                         _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, Prop, Tab )
                                        )
                                    )
                               )
                            {
                                _makeTabObj( Tabs, Tab, false );
                                break;
                            }
                        }
                    }
                }
                else
                {
                    NodeTypeId = Node.NodeTypeId;
                    foreach( CswNbtMetaDataNodeTypeTab Tab in from _Tab in _CswNbtResources.MetaData.getNodeTypeTabs( Node.NodeTypeId )
                                                              orderby _Tab.TabOrder, _Tab.TabName
                                                              where ( ( _ConfigMode || _Tab.TabName != CswNbtMetaData.IdentityTabName ) &&
                                                                    (
                                                                        (
                                                                            _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.View, Node.getNodeType(), _Tab ) ||
                                                                            _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, Node.getNodeType() )
                                                                        )
                                                                        &&
                                                                        (
                                                                  //Case 29843: Exlude "empty" tabs from the UI
                                                                            _ConfigMode || _Tab.HasProps
                                                                        )
                                                                    ) )
                                                              select _Tab )
                    {
                        _makeTabObj( Tabs, Tab, _canEditLayout() );
                    }

                    // History tab
                    if( false == _ConfigMode &&
                        false == _IsMultiEdit &&
                        Date.IsNull &&
                        NodeType.AuditLevel != Audit.CswEnumAuditLevel.NoAudit &&
                        CswConvert.ToBoolean( _CswNbtResources.DoAuditing ) )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NodeType ) )
                        {
                            _makeTabObj( Tabs, Int32.MaxValue, "history", "History", false, Int32.MinValue.ToString() );
                        }
                    }

                    Ret["node"]["canEditLayout"] = _canEditLayout();
                    Ret["node"]["nodename"] = Node.NodeName;
                } // if-else( filterToPropId != string.Empty )
                Ret["node"]["nodetypeid"] = NodeTypeId;
                Ret["node"]["isFavorite"] = Node.isFavorite();
                Ret["node"]["nodetypename"] = NodeType.NodeTypeName;
            }
            return Ret;
        } // getTabs()

        private Int32 TabOrderModifier = 0;

        private void _makeTabObj( JObject ParentObj, CswNbtMetaDataNodeTypeTab Tab, bool CanEditLayout )
        {
            if( null != Tab )
            {
                if( _ConfigMode || Tab.getNodeTypePropsByDisplayOrder().Any() )
                {
                    _makeTabObj( ParentObj, Tab.TabOrder, Tab.TabId.ToString(), Tab.TabName, CanEditLayout, Tab.DesignNodeId.ToString() );
                }
            }
        }

        private void _makeTabObj( JObject ParentObj, Int32 TabOrder, string Id, string Name, bool CanEditLayout, string TabNodeId )
        {
            // case 24250
            // This mechanism correctly orders all tabs even with redundant tab order values,
            // as long as the tabs are added in order
            while( ParentObj[( TabOrder + TabOrderModifier ).ToString()] != null )
            {
                TabOrderModifier++;
            }
            string RealTabOrder = ( TabOrder + TabOrderModifier ).ToString();

            ParentObj[RealTabOrder] = new JObject();
            ParentObj[RealTabOrder]["id"] = Id;
            ParentObj[RealTabOrder]["name"] = Name;
            ParentObj[RealTabOrder]["canEditLayout"] = CanEditLayout;
            ParentObj[RealTabOrder]["tabnodeid"] = TabNodeId;
        }


        /// <summary>
        /// Create a new temp node according to NodeTypeId.
        /// </summary>
        public CswNbtNode getAddNode( Int32 NodeTypeId, string RelatedNodeId, Action<CswNbtNode> After )
        {
            CswNbtNode Ret = null;
            CswNbtMetaDataNodeType NodeType = null;
            if( NodeTypeId != Int32.MinValue )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                if( null != NodeType )
                {
                    if( false == _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType ) )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Insufficient permission to create a new " + NodeType.NodeTypeName, "User " + _CswNbtResources.CurrentNbtUser.Username + " does not have Create permission for " + NodeType.NodeTypeName );
                    }
                    else
                    {
                        Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, IsTemp: true, OnAfterMakeNode: delegate( CswNbtNode NewNode )
                        {
                            if( null != After )
                            {
                                After( NewNode );
                            }
                            CswPrimaryKey RelatedNodePk = CswConvert.ToPrimaryKey( RelatedNodeId );
                            CswNbtNode RelatedNode = _CswNbtResources.Nodes[RelatedNodePk];
                            if( null != RelatedNode )
                            {
                                foreach( CswNbtNodePropRelationship Relationship in from _Prop
                                    in NewNode.Properties
                                                                                    where _Prop.getFieldTypeValue() == CswEnumNbtFieldType.Relationship &&
                                                                                            ( _Prop.AsRelationship.TargetMatches( RelatedNode.getNodeType() ) ||
                                                                                            _Prop.AsRelationship.TargetMatches( RelatedNode.getObjectClass() ) )
                                                                                    select _Prop )
                                {
                                    Relationship.RelatedNodeId = RelatedNodePk;
                                }
                            }
                        } );
                    }
                }
            }
            return Ret;
        }

        /// <summary>
        /// Create a new temp node according to NodeType.
        /// </summary>
        public CswNbtNode getAddNodeAndPostChanges( CswNbtMetaDataNodeType NodeType, Action<CswNbtNode> After, bool IgnorePermissions = false )
        {
            CswNbtNode Ret = null;
            if( null != NodeType )
            {
                if( IgnorePermissions || _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType ) )
                {
                    Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, After, IsTemp: true );
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Insufficient permission to create a new " + NodeType.NodeTypeName, "User " + _CswNbtResources.CurrentNbtUser.Username + " does not have Create permission for " + NodeType.NodeTypeName );
                }
            }
            return Ret;
        }

        /// <summary>
        /// Fetch or create a node, and return a JObject for all properties in a given tab
        /// </summary>
        public JObject getProps( string NodeId, string NodeKey, string TabId, Int32 NodeTypeId, string filterToPropId, string RelatedNodeId, bool ForceReadOnly, CswDateTime Date = null )
        {
            JObject Ret = new JObject();

            CswPropIdAttr FilterPropIdAttr = null;
            if( false == string.IsNullOrEmpty( filterToPropId ) )
            {
                FilterPropIdAttr = new CswPropIdAttr( filterToPropId );
            }

            if( TabId == "history" )
            {
                CswNbtNode Node = _CswNbtResources.getNode( NodeId, NodeKey, Date );
                if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, Node.getNodeType() ) )
                {
                    _getAuditHistoryGridProp( Ret, Node );
                }
                // Case 31931 -- We should be returning information about the node!
                Ret["node"] = _getNodeProps( Node );
            }
            else
            {
                CswEnumNbtLayoutType LayoutType = CswEnumNbtLayoutType.LayoutTypeForEditMode( _CswNbtResources.EditMode );

                CswNbtNode Node;
                if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add && false == CswTools.IsPrimaryKey( CswConvert.ToPrimaryKey( NodeId ) ) )
                {
                    Node = getAddNode( NodeTypeId, RelatedNodeId, null );
                }
                else
                {
                    Node = _CswNbtResources.getNode( NodeId, NodeKey, Date );
                }
                return getProps( Node, TabId, FilterPropIdAttr, LayoutType, ForceReadOnly, Date );

            } // if-else( TabId.StartsWith( HistoryTabPrefix ) )
            return Ret;
        } // getProps()

        /// <summary>
        /// Fetch or create a node, and return a JObject for all properties in the identity tab
        /// </summary>
        public JObject getIdentityTabProps( CswPrimaryKey NodeId, string filterToPropId, string RelatedNodeId, bool ForceReadOnly = false, CswDateTime Date = null )
        {
            JObject Ret = new JObject();

            CswNbtNode Node = _CswNbtResources.Nodes[NodeId];
            CswNbtMetaDataNodeType NodeType = Node.getNodeType();
            if( null != NodeType )
            {
                CswNbtMetaDataNodeTypeTab IdentityTab = NodeType.getIdentityTab();
                Ret = getProps( NodeId.ToString(), null, IdentityTab.TabId.ToString(), NodeType.NodeTypeId, filterToPropId, RelatedNodeId, ForceReadOnly, Date );
                Ret["tab"] = new JObject();
                Ret["tab"]["tabid"] = IdentityTab.TabId;
            }
            return Ret;
        } // getIdentityTabProps()

        /// <summary>
        /// Get props of a Node instance
        /// </summary>
        public JObject getProps( CswNbtNode Node, string TabId, CswPropIdAttr FilterPropIdAttr, CswEnumNbtLayoutType LayoutType, bool ForceReadOnly = false, CswDateTime Date = null )
        {
            JObject Ret = new JObject();
            Ret["node"] = new JObject();
            JObject Properties = new JObject();
            Ret["properties"] = Properties;

            if( Node != null )
            {
                // case 30765 - this must be done here in order to prepare the property for export to the UI (e.g. setting 'Hidden' correctly)
                foreach( CswNbtNodePropWrapper Prop in Node.Properties )
                {
                    Prop.TriggerOnBeforeRender();
                }

                Ret["node"] = _getNodeProps( Node );
                CswNbtMetaDataNodeType NodeType = Node.getNodeType();

                if( TabId.StartsWith( HistoryTabPrefix ) )
                {
                    if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, Node.getNodeType() ) )
                    {
                        _getAuditHistoryGridProp( Properties, Node );
                    }
                }
                else
                {
                    Int32 TabIdPk = CswConvert.ToInt32( TabId );
                    IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeTypeId, TabIdPk, LayoutType, Date );

                    if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add ||
                        _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType ) )
                    {
                        CswNbtNodePropColl PropColl = Node.Properties;

                        IEnumerable<CswNbtMetaDataNodeTypeProp> CswNbtMetaDataNodeTypeProps = Props as CswNbtMetaDataNodeTypeProp[] ?? Props.ToArray();
                        bool TabHasAnyEditableProp = CswNbtMetaDataNodeTypeProps.Any( Prop => Prop.IsSaveable );

                        CswNbtMetaDataNodeTypeTab IdentityTab = Node.getNodeType().getIdentityTab();
                        if( TabIdPk != IdentityTab.TabId && CswEnumNbtLayoutType.Add != LayoutType )
                        {
                            IEnumerable<CswNbtMetaDataNodeTypeProp> IdentityProps = _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Node.NodeTypeId, IdentityTab.TabId, LayoutType, Date );
                            IEnumerable<CswNbtMetaDataNodeTypeProp> IdentityTabProps = Props as CswNbtMetaDataNodeTypeProp[] ?? IdentityProps.ToArray();
                            TabHasAnyEditableProp = TabHasAnyEditableProp || IdentityTabProps.Any( Prop => Prop.IsSaveable );
                        }
                        bool HasEditableProps = false == ForceReadOnly && TabHasAnyEditableProp;
                        Ret["node"]["showeditbutton"] = HasEditableProps;

                        if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add && false == HasEditableProps )
                        {
                            //Case 29531 - There are no props on the Add layout, so just save the node - the client will skip the add dialog
                            Node.PromoteTempToReal();
                        }
                        else
                        {
                            IEnumerable<CswNbtMetaDataNodeTypeProp> FilteredProps = ( from _Prop in CswNbtMetaDataNodeTypeProps
                                                                                      where PropColl != null
                                                                                      where _showProp( LayoutType, _Prop, FilterPropIdAttr, TabIdPk, Node, HasEditableProps )
                                                                                      select _Prop ).OrderByDescending( Prop => Prop.PropName != "Save" ).ThenBy( Prop => Prop );


                            foreach( CswNbtMetaDataNodeTypeProp Prop in FilteredProps )
                            {
                                _addProp( Properties, Node, Prop, CswConvert.ToInt32( TabId ), ForceReadOnly, LayoutType );
                            }
                        }
                    }
                } // if(Node != null)
            }
            return Ret;
        }

        private bool _showProp( CswEnumNbtLayoutType LayoutType, CswNbtMetaDataNodeTypeProp Prop, CswPropIdAttr FilterPropIdAttr, Int32 TabId, CswNbtNode Node, bool HasEditableProps )
        {
            bool Ret = Prop.ShowProp( LayoutType, Node, TabId, _ConfigMode, HasEditableProps ) &&
                 ( Prop.IsSaveProp ||
                 ( LayoutType != CswEnumNbtLayoutType.Add ||
                //Case 24023: Exclude buttons on Add (Except the Save button)
                   Prop.getFieldTypeValue() != CswEnumNbtFieldType.Button ) &&
                 ( FilterPropIdAttr == null || Prop.PropId == FilterPropIdAttr.NodeTypePropId ) );

            return Ret;
        }

        /// <summary>
        /// Returns JObject for a single property and its conditional properties
        /// </summary>
        public JObject getSingleProp( string NodeId, string NodeKey, string PropIdFromJson, Int32 NodeTypeId, string NewPropJson )
        {
            JObject Ret = new JObject();
            JObject PropObj = new JObject();

            CswNbtNode Node = _CswNbtResources.getNode( NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
            if( null == Node &&
                ( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add || _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Temp ) &&
                NodeTypeId != Int32.MinValue )
            {
                Node = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, IsTemp: true );
            }

            if( Node != null )
            {
                // for prop filters, update node prop value but don't save the change
                JObject PropJson = CswConvert.ToJObject( NewPropJson, true, "NewPropJson" );
                CswNbtSdNode NodeAction = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
                NodeAction.addSingleNodeProp( Node, PropJson, null );
                // Some property values are reset by business logic, which needs to be emulated here
                // ...but can we do that in a way that doesn't call WriteNode logic?
                Node.ObjClass.beforeWriteNode( IsCopy : false, OverrideUniqueValidation: true, Creating : false );

                // case 30765 - this must be done here in order to prepare the property for export to the UI (e.g. setting 'Hidden' correctly)
                // We need to do this prop as well as all conditional props
                foreach( CswNbtNodePropWrapper p in Node.Properties )
                {
                    p.TriggerOnBeforeRender();
                }

                CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropIdFromJson );
                CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );
                if( Prop.FirstEditLayout != null )
                {
                    _addProp( PropObj, Node, Prop, Prop.FirstEditLayout.TabId );
                }
            }
            if( PropObj.HasValues )
            {
                Ret = (JObject) PropObj.Properties().First().Value;
            }

            return Ret;
        } // getProp()

        private void _addProp( JObject ParentObj, CswNbtNode Node, CswNbtMetaDataNodeTypeProp Prop, Int32 TabId, bool ForceReadOnly = false, CswEnumNbtLayoutType LayoutType = null )
        {
            if( null == LayoutType )
            {
                LayoutType = CswEnumNbtLayoutType.LayoutTypeForEditMode( _CswNbtResources.EditMode );
            }
            CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout = Prop.getLayout( LayoutType, TabId );
            if( _ConfigMode ||
                ( false == Node.Properties[Prop].Hidden &&
                  ( false == ForceReadOnly || Prop.getFieldTypeValue() != CswEnumNbtFieldType.Button ) ) ) // Hide buttons when ForceReadOnly is true
            {
                JProperty JpProp = makePropJson( Node.NodeId, Prop, Node.Properties[Prop], Layout, ForceReadOnly, Node.Locked );
                ParentObj.Add( JpProp );
                JObject PropObj = (JObject) JpProp.Value;

                // Handle conditional properties
                JObject SubPropsObj = new JObject();
                JProperty SubPropsJProp = new JProperty( "subprops", SubPropsObj );
                PropObj.Add( SubPropsJProp );
                bool HasSubProps = false;
                foreach( CswNbtMetaDataNodeTypeProp FilterProp in _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( Prop.NodeTypeId, Layout.TabId, LayoutType ) )
                {
                    if( Int32.MinValue != FilterProp.FilterNodeTypePropId && FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                    {
                        HasSubProps = true;
                        CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout FilterPropLayout = FilterProp.getLayout( LayoutType, TabId );
                        JProperty JPFilterProp = makePropJson( Node.NodeId, FilterProp, Node.Properties[FilterProp], FilterPropLayout, ForceReadOnly, Node.Locked );
                        SubPropsObj.Add( JPFilterProp );
                        JObject FilterPropXml = (JObject) JPFilterProp.Value;

                        // Hide those for whom the filter doesn't match
                        // (but we need the XML node to be there to store the value, for client-side changes)
                        FilterPropXml["display"] = FilterProp.CheckFilter( Node ).ToString().ToLower();

                    } // if( FilterProp.FilterNodeTypePropId == Prop.FirstPropVersionId )
                } // foreach( CswNbtMetaDataNodeTypeProp FilterProp in Tab.NodeTypePropsByDisplayOrder )
                PropObj["hassubprops"] = HasSubProps;
                PropObj["issaveprop"] = Prop.IsSaveProp;
            } // if( false == Node.Properties[Prop].Hidden )
        } // addProp()

        private Dictionary<Int32, Collection<Int32>> _DisplayRowsAndCols = new Dictionary<Int32, Collection<Int32>>();

        public static Int32 getUniqueRow( Int32 ProposedRow, Int32 Column, Dictionary<Int32, Collection<Int32>> RowsAndColumns = null )
        {
            RowsAndColumns = RowsAndColumns ?? new Dictionary<int, Collection<int>>();
            Int32 Ret = ProposedRow;
            if( Ret < 1 )
            {
                Ret = 1;
            }
            if( Column < 1 )
            {
                Column = 1;
            }

            if( false == RowsAndColumns.ContainsKey( Column ) )
            {
                RowsAndColumns.Add( Column, new Collection<Int32> { Ret } );
            }
            else if( false == RowsAndColumns[Column].Contains( Ret ) )
            {
                RowsAndColumns[Column].Add( Ret );
            }
            else
            {
                Ret = getUniqueRow( Ret + 1, Column, RowsAndColumns );
            }
            return Ret;
        }

        public JProperty makePropJson( CswPrimaryKey NodeId, CswNbtMetaDataNodeTypeProp Prop, CswNbtNodePropWrapper PropWrapper, CswNbtMetaDataNodeTypeLayoutMgr.NodeTypeLayout Layout, bool ForceReadOnly = false, bool NodeLocked = false )
        {
            CswPropIdAttr PropIdAttr = null;
            PropIdAttr = new CswPropIdAttr( NodeId, Prop.PropId );

            JObject PropObj = new JObject();
            JProperty ret = new JProperty( "prop_" + PropIdAttr, PropObj );
            CswEnumNbtFieldType FieldType = Prop.getFieldTypeValue();
            PropObj["id"] = PropIdAttr.ToString();
            PropObj["propnodeid"] = Prop.DesignNodeId.ToString();
            PropObj["name"] = Prop.PropNameWithQuestionNo;
            if( null != PropWrapper )
            {
                PropObj["helptext"] = PropWrapper[CswEnumNbtPropertyAttributeName.HelpText];
            }
            else
            {
                PropObj["helptext"] = Prop.HelpText;
            }
            PropObj["fieldtype"] = FieldType.ToString();
            PropObj["ocpname"] = Prop.getObjectClassPropName();
            Int32 DisplayRow = getUniqueRow( Layout.DisplayRow, Layout.DisplayColumn, _DisplayRowsAndCols );

            if( Prop.IsSaveProp )
            {
                DisplayRow = _CswNbtResources.MetaData.NodeTypeLayout.getCurrentMaxDisplayRow( Prop.NodeTypeId, Layout.TabId, Layout.LayoutType );
                if( Int32.MinValue != Prop.ObjectClassPropId && null != Prop.getObjectClassProp() && Prop.getObjectClassProp().PropName == CswNbtObjClass.PropertyName.Save )
                {
                    DisplayRow = getUniqueRow( DisplayRow + 1, Layout.DisplayColumn, _DisplayRowsAndCols );
                }
            }
            bool ReadOnly = ( null != PropWrapper && CswConvert.ToBoolean( PropWrapper[CswEnumNbtPropertyAttributeName.Required] ) );

            PropObj["displayrow"] = DisplayRow;
            PropObj["displaycol"] = Layout.DisplayColumn;

            PropObj["tabgroup"] = Layout.TabGroup;
            PropObj["required"] = ReadOnly;
            PropObj["copyable"] = Prop.IsCopyable();

            bool ShowPropertyName = ( false == ( FieldType == CswEnumNbtFieldType.Image ||
                                                 FieldType == CswEnumNbtFieldType.Button ||
                                                 FieldType == CswEnumNbtFieldType.ReportLink ||
                                                 ( FieldType == CswEnumNbtFieldType.Grid &&
                                                   PropWrapper.AsGrid.GridMode == CswEnumNbtGridPropMode.Full ) ) );

            PropObj["showpropertyname"] = ShowPropertyName;

            CswNbtMetaDataNodeTypeTab Tab = null;
            if( CswEnumNbtLayoutType.LayoutTypeForEditMode( _CswNbtResources.EditMode ) == CswEnumNbtLayoutType.Edit &&
                Layout.TabId != Int32.MinValue )
            {
                Tab = _CswNbtResources.MetaData.getNodeTypeTab( Layout.TabId );
            }

            if( PropWrapper != null )
            {
                PropObj["helptext"] = PropWrapper[CswEnumNbtPropertyAttributeName.HelpText];   // case 29342

                CswNbtMetaDataNodeType NodeType = Prop.getNodeType();

                CswEnumNbtNodeTypePermission permission = CswEnumNbtNodeTypePermission.Edit;
                if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                {
                    //case 31503
                    permission = CswEnumNbtNodeTypePermission.Create;
                }

                if( //Case 29142: Buttons are never "readonly"--defer entirely to the Object Class to decide whether they are visible
                    FieldType == CswEnumNbtFieldType.Button ||
                    ( false == ForceReadOnly &&
                      ( _CswNbtResources.Permit.isPropWritable( permission, Prop, Tab, PropWrapper ) &&
                        _CswNbtResources.Permit.isNodeWritable( permission, NodeType, NodeId ) &&
                        ( _CswNbtResources.Permit.canNodeType( permission, NodeType ) ||
                          _CswNbtResources.Permit.canTab( permission, NodeType, Tab ) ) ) ) )
                {
                    PropObj["readonly"] = false;
                }
                else
                {
                    PropObj["readonly"] = true;

                    // case 29095
                    PropObj["canoverride"] = ( false == ForceReadOnly &&
                                               false == Prop.ServerManaged &&
                                               false == NodeLocked &&                                        // case 29440
                                               false == _ConfigMode &&                                       // case 29484
                                               FieldType != CswEnumNbtFieldType.PropertyReference &&
                                               FieldType != CswEnumNbtFieldType.Static &&
                                               CswEnumNbtNodeEditMode.Edit == _CswNbtResources.EditMode &&   // case 29484
                                               _CswNbtResources.CurrentNbtUser.IsAdministrator() );
                }

                PropObj["gestalt"] = PropWrapper.Gestalt.Replace( "\"", "&quot;" );

                // case 30702
                if( null != Prop.Date && Int32.MinValue == PropWrapper.JctNodePropId )
                {
                    PropObj["helptext"] = "[No Audit Data Available]";
                }

                PropObj["highlight"] = PropWrapper.AuditChanged.ToString().ToLower();
                PropWrapper.ToJSON( PropObj );
            }
            return ret;
        } // makePropJson()

        /// <summary>
        /// Create a JObject with information about the provided Node.
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        private JObject _getNodeProps( CswNbtNode Node )
        {
            JObject Ret = new JObject();

            if( CswTools.IsPrimaryKey( Node.NodeId ) )
            {
                Ret["nodeid"] = Node.NodeId.ToString();
                Ret["nodelink"] = Node.NodeLink;
                Ret["nodename"] = Node.NodeName;
                Ret["nodetypeid"] = Node.NodeTypeId;
                Ret["isFavorite"] = Node.isFavorite();
                if( null != Node.RelationalId && CswTools.IsPrimaryKey( Node.RelationalId ) )
                {
                    Ret["relationalid"] = Node.RelationalId.PrimaryKey.ToString();
                }
            }

            return Ret;
        }//_getNodeProps()

        public void _getAuditHistoryGridProp( JObject ParentObj, CswNbtNode Node )
        {
            if( Node != null )
            {
                JObject AuditProperty = new JObject();
                ParentObj["properties"] = AuditProperty;
                JObject PropObj = new JObject();
                ParentObj["properties"]["prop_" + Node.NodeId.ToString() + "_audit"] = PropObj;

                PropObj["name"] = "Audit History";
                PropObj["helptext"] = string.Empty;
                PropObj["fieldtype"] = "AuditHistoryGrid";
                PropObj["displayrow"] = 1;
                PropObj["displaycol"] = 1;
                PropObj["required"] = false;
                PropObj["readonly"] = true;
                PropObj["id"] = Node.NodeId.ToString() + "_audit";
                PropObj["showpropertyname"] = false;
            }
        } // _getAuditHistoryGridProp()

        public bool moveProp( string PropIdAttr, Int32 TabId, Int32 NewRow, Int32 NewColumn )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue && NewRow > 0 && NewColumn > 0 )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    Prop.updateLayout( CswEnumNbtLayoutType.LayoutTypeForEditMode( _CswNbtResources.EditMode ), false, TabId, NewRow, NewColumn );
                    ret = true;
                }
            } // _canEditLayout()
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // moveProp()

        public bool removeProp( string PropIdAttr, Int32 TabId )
        {
            bool ret = false;
            if( _canEditLayout() )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                Int32 NodeTypePropId = PropId.NodeTypePropId;
                if( NodeTypePropId != Int32.MinValue )
                {
                    CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
                    if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add && Prop.IsRequired && false == Prop.HasDefaultValue() )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, Prop.PropName + " may not be removed", Prop.PropName + " is required and has no unique value, and therefore cannot be removed from 'Add' layouts" );
                    }
                    Prop.removeFromLayout( CswEnumNbtLayoutType.LayoutTypeForEditMode( _CswNbtResources.EditMode ), TabId );
                    ret = true;
                }
            } // _canEditLayout()
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "You do not have permission to configure layout", _CswNbtResources.CurrentNbtUser.Username + " tried to change property layout without administrative or Design privileges" );
            }
            return ret;
        } // removeProp()

        private CswNbtNode _addNode( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, Action<CswNbtNode> After, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            CswEnumNbtNodeEditMode prevEditMode = _CswNbtResources.EditMode;
            _CswNbtResources.EditMode = CswEnumNbtNodeEditMode.Add;

            CswNbtNode Ret = Node;
            RetNbtNodeKey = null;
            CswNbtActQuotas QuotaAction = new CswNbtActQuotas( _CswNbtResources );
            CswNbtActQuotas.Quota Quota = QuotaAction.CheckQuotaNT( NodeType );
            if( Quota.HasSpace )
            {
                CswNbtNodeKey nodekey = null;
                Action<CswNbtNode> After2 = delegate( CswNbtNode NewNode )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType ) ||
                            _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Create, NodeType, NodeTypeTab ) ||
                            _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Create, NodeType, NewNode.NodeId ) )
                        {
                            nodekey = _saveProp( NewNode, PropsObj, View, NodeTypeTab, true );
                        }
                        if( null != After )
                        {
                            After( NewNode );
                        }
                    };

                if( null == Ret || false == CswTools.IsPrimaryKey( Ret.NodeId ) )
                {
                    Ret = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, After2 );
                }
                else
                {
                    After2( Ret );
                }
                RetNbtNodeKey = nodekey;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Quota Exceeded", "You have used all of your purchased quota, and must purchase additional quota space in order to add" );
            }

            _CswNbtResources.EditMode = prevEditMode;
            return Ret;
        }

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, Action<CswNbtNode> After, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _addNode( NodeType, null, PropsObj, out RetNbtNodeKey, After, View, NodeTypeTab );
        }

        public CswNbtNode addNode( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, JObject PropsObj, out CswNbtNodeKey RetNbtNodeKey, Action<CswNbtNode> After, CswNbtView View = null, CswNbtMetaDataNodeTypeTab NodeTypeTab = null )
        {
            return _addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, After, View, NodeTypeTab );
        }

        public JObject saveProps( CswPrimaryKey NodePk, Int32 TabId, JObject PropsObj, Int32 NodeTypeId, CswNbtView View, bool IsIdentityTab, bool setIsTempToFalse = true )
        {
            JObject ret = new JObject();
            if( PropsObj.HasValues )
            {
                CswNbtNodeKey RetNbtNodeKey = null;
                bool AllSucceeded = false;
                CswNbtNode Node = null;
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                CswNbtMetaDataNodeTypeTab NodeTypeTab = null;
                if( false == IsIdentityTab )
                {
                    NodeTypeTab = _CswNbtResources.MetaData.getNodeTypeTab( TabId );
                }
                else if( null != NodeType )
                {
                    NodeTypeTab = NodeType.getIdentityTab();
                }

                if( null == NodeType && null != NodeTypeTab )
                {
                    NodeType = NodeTypeTab.getNodeType();
                }
                if( null != NodeType )
                {
                    Node = _CswNbtResources.Nodes.GetNode( NodePk );
                    switch( _CswNbtResources.EditMode )
                    {
                        case CswEnumNbtNodeEditMode.Temp:
                            if( null != Node )
                            {
                                addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, null, View, NodeTypeTab );
                            }
                            else
                            {
                                Node = addNode( NodeType, null, PropsObj, out RetNbtNodeKey, null, View, NodeTypeTab );
                            }
                            break;
                        case CswEnumNbtNodeEditMode.Add:
                            if( null != Node )
                            {
                                addNode( NodeType, Node, PropsObj, out RetNbtNodeKey, null, View, NodeTypeTab );
                            }
                            else
                            {
                                Node = addNode( NodeType, null, PropsObj, out RetNbtNodeKey, null, View, NodeTypeTab );
                            }

                            AllSucceeded = ( null != Node );
                            break;
                        default:
                            if( null != Node )
                            {
                                if( _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, NodeType, NodeTypeTab ) ||
                                    _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.Edit, NodeType ) ||
                                    _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Edit, NodeType, Node.NodeId ) )
                                {
                                    if( Node.PendingUpdate )
                                    {
                                        CswNbtActUpdatePropertyValue Act = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                                        Act.UpdateNode( Node, false );
                                    }
                                    RetNbtNodeKey = _saveProp( Node, PropsObj, View, NodeTypeTab );
                                    if( null != RetNbtNodeKey )
                                    {
                                        AllSucceeded = true;
                                    }
                                }
                            }
                            break;
                    } //switch( _CswNbtResources.EditMode )
                    if( AllSucceeded && null != RetNbtNodeKey )
                    {
                        string RetNodeKey = RetNbtNodeKey.ToString();
                        ret["nodekey"] = RetNodeKey;
                    } //if( AllSucceeded && null != RetNbtNodeKey )
                    else
                    {
                        string ErrString;
                        if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                        {
                            ErrString = "Attempt to Add failed.";
                        }
                        else
                        {
                            ErrString = "Prop updates failed";
                        }
                        ret["result"] = ErrString;
                    } //else
                } //if( null != NodeType && null != NodeTypeTab )

                // Good opportunity to force an update on the node
                if( Node != null )
                {
                    CswNbtActUpdatePropertyValue ActUpdatePropVal = new CswNbtActUpdatePropertyValue( _CswNbtResources );
                    ActUpdatePropVal.UpdateNode( Node, true );
                    if( Node.IsTemp && setIsTempToFalse )
                    {
                        Node.PromoteTempToReal();
                    }
                    else
                    {
                        Node.postChanges( false );
                    }
                    ret["result"] = "Succeeded";
                    //If we're Adding, NodeName won't be valid until now.
                    ret["nodename"] = Node.NodeName;
                    ret["nodelink"] = Node.NodeLink;
                    ret["nodeid"] = Node.NodeId.ToString();
                    ret["action"] = _determineAction( Node.ObjClass.ObjectClass.ObjectClass );
                }
            } // if( PropsObj.HasValues )

            return ret;
        } // saveProps()

        private string _determineAction( CswEnumNbtObjectClass objectClass )
        {
            CswEnumNbtButtonAction ret;
            switch( objectClass )
            {
                case CswEnumNbtObjectClass.FeedbackClass:
                    ret = CswEnumNbtButtonAction.loadView;
                    break;
                default:
                    ret = CswEnumNbtButtonAction.refresh;
                    break;
            }
            return ret.ToString();
        }

        /// <summary>
        /// Directly save a node and its properties. Useful in Wizards and Actions when you have already done your own validation.
        ///<para>WARNING: Don't call this method unless you have rolled your own validation.</para>
        /// </summary>
        public void saveNodeProps( CswNbtNode Node, JObject PropsObj )
        {
            _saveProp( Node, PropsObj, null, null );
        }

        private CswNbtNodeKey _saveProp( CswNbtNode Node, JObject PropsObj, CswNbtView View, CswNbtMetaDataNodeTypeTab Tab, bool ForceUpdate = false )
        {
            CswNbtNodeKey Ret = null;
            if( Node != null )
            {
                CswNbtSdNode NodeAction = new CswNbtSdNode( _CswNbtResources, _CswNbtStatisticsEvents );
                NodeAction.addNodeProps( Node, PropsObj, Tab );

                /* Case 8517 - this sets sequences that have setvalonadd = 0 */
                Node.setSequenceValues();

                ICswNbtTree Tree;
                if( View != null )
                {
                    // Get the nodekey of this node in the current view
                    Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
                if( Ret == null )
                {
                    // Make a nodekey from the default view
                    View = Node.getNodeType().CreateDefaultView();
                    View.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( Node.NodeId );
                    Tree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
                    Ret = Tree.getNodeKeyByNodeId( Node.NodeId );
                }
            }
            return Ret;
        }

        public void copyPropValues( CswNbtNode SourceNode, CswCommaDelimitedString CopyNodeIds, CswCommaDelimitedString CopyPropIds )
        {
            if( null != SourceNode && CopyNodeIds.Count > 1 && CopyPropIds.Count > 0 )
            {
                Collection<CswNbtNode> CopyToNodes = new Collection<CswNbtNode>();
                foreach( string CopyToNodeId in CopyNodeIds )
                {
                    if( string.Compare( CopyToNodeId, SourceNode.NodeId.ToString(), StringComparison.OrdinalIgnoreCase ) != 0 )
                    {
                        CswNbtNode Node = _CswNbtResources.Nodes[CopyToNodeId];
                        if( null != Node &&
                            Node.NodeTypeId == SourceNode.NodeTypeId &&
                            _CswNbtResources.Permit.isNodeWritable( CswEnumNbtNodeTypePermission.Edit, SourceNode.getNodeType(), Node.NodeId ) )
                        {
                            CopyToNodes.Add( Node );
                        }
                    }
                }
                foreach( CswNbtNode CopyToNode in CopyToNodes )
                {
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in CopyPropIds.Select( PropIdAttr => new CswPropIdAttr( PropIdAttr ) )
                                                                                    .Select( PropId => _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId ) ) )
                    {
                        CopyToNode.Properties[NodeTypeProp].copy( SourceNode.Properties[NodeTypeProp] );
                    }
                    CopyToNode.postChanges( ForceUpdate: false );
                } // foreach( string NodeIdStr in CopyNodeIds )
            }
        }


        public JArray getPropertiesForLayoutAdd( string NodeId, string NodeKey, string NodeTypeId, string TabId, CswEnumNbtLayoutType LayoutType )
        {
            JArray ret = new JArray();

            CswNbtMetaDataNodeType NodeType = null;

            if( false == string.IsNullOrEmpty( NodeTypeId ) )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            }
            else if( false == string.IsNullOrEmpty( TabId ) )
            {
                CswNbtMetaDataNodeTypeTab Tab = _CswNbtResources.MetaData.getNodeTypeTab( CswConvert.ToInt32( TabId ) );
                if( null != Tab )
                {
                    NodeType = Tab.getNodeType();
                }
            }
            else
            {
                CswNbtNode CopyFromNode = _CswNbtResources.getNode( NodeId, NodeKey, new CswDateTime( _CswNbtResources ) );
                if( null != CopyFromNode )
                {
                    NodeType = CopyFromNode.getNodeType();
                }
            }

            if( NodeType != null )
            {
                IEnumerable<CswNbtMetaDataNodeTypeProp> Props = _CswNbtResources.MetaData.NodeTypeLayout.getPropsNotInLayout( NodeType, CswConvert.ToInt32( TabId ), LayoutType );
                if( LayoutType == CswEnumNbtLayoutType.Edit )//Case 31740 - For Edit Layout, include all props for Identity tab's sake.
                {
                    Props = Props.Concat( _CswNbtResources.MetaData.NodeTypeLayout.getPropsInLayout( NodeType.NodeTypeId, CswConvert.ToInt32( TabId ), LayoutType ) );
                }
                foreach( CswNbtMetaDataNodeTypeProp Prop in from Prop in Props
                                                            orderby Prop.PropNameWithQuestionNo
                                                            select Prop )
                {
                    // case 24179
                    if( Prop.getFieldType().IsLayoutCompatible( LayoutType ) && false == Prop.IsSaveProp )
                    {
                        JObject ThisPropObj = new JObject();
                        ThisPropObj["propid"] = Prop.PropId.ToString();
                        ThisPropObj["propname"] = Prop.PropNameWithQuestionNo.ToString();
                        ThisPropObj["hidden"] = ( LayoutType == CswEnumNbtLayoutType.Edit && Prop.FirstEditLayout == null ).ToString().ToLower();
                        ret.Add( ThisPropObj );
                    }
                }
            } // if( NodeType != null )
            return ret;
        } // getPropertiesForLayoutAdd()


        public bool addPropertyToLayout( string PropId, string TabId, CswEnumNbtLayoutType LayoutType )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropId ) );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( LayoutType, Prop.NodeTypeId, Prop, false, CswConvert.ToInt32( TabId ), Int32.MinValue, Int32.MinValue );
            return true;
        } // addPropertyToLayout()

        public bool ClearPropValue( string PropIdAttr, bool IncludeBlob )
        {
            bool ret = false;
            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            if( Int32.MinValue != PropId.NodeId.PrimaryKey )
            {
                CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
                CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];
                PropWrapper.ClearValue();
                Node.postChanges( false );
                if( IncludeBlob )
                {
                    PropWrapper.ClearBlob();
                }
                ret = true;
            }
            return ret;
        } // ClearPropValue()


        private bool _canEditLayout()
        {
            return ( _CswNbtResources.Permit.can( CswEnumNbtActionName.Design ) ); //|| _CswNbtResources.CurrentNbtUser.IsAdministrator() );
        }

        public JObject getObjectClassButtons( string ObjectClassId )
        {
            JObject Buttons = new JObject();

            CswNbtMetaDataObjectClass Oc = _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( ObjectClassId ) );
            foreach( CswNbtMetaDataObjectClassProp Prop in Oc.getObjectClassProps() )
            {
                CswNbtMetaDataFieldType Type = Prop.getFieldType();
                if( Type.FieldType == CswEnumNbtFieldType.Button )
                {
                    string propName = "button_" + Prop.PropId;
                    Buttons[propName] = new JObject();
                    Buttons[propName]["id"] = Prop.PropId;
                    Buttons[propName]["name"] = Prop.PropName;
                }
            }

            return Buttons;
        }

    } // class CswNbtSdTabsAndProps

} // namespace ChemSW.Nbt.WebServices
