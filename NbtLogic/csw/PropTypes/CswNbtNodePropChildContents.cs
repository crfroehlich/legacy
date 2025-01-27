using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropChildContents : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropChildContents( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsChildContents;
        }

        public CswNbtNodePropChildContents( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            // No subfields
        }

        #region Generic Properties

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void SyncGestalt()
        {

        }

        #endregion Generic Properties

        #region Relationship Properties and Functions

        public Int32 RelationshipId
        {
            //get { return _CswNbtMetaDataNodeTypeProp.FKValue; }
            get { return CswConvert.ToInt32( _CswNbtNodePropData[CswNbtFieldTypeRuleChildContents.AttributeName.ChildRelationship] ); }
        }

        public CswEnumNbtViewPropIdType RelationshipType
        {
            get
            {
                //return _CswNbtMetaDataNodeTypeProp.FKType;
                return _CswNbtNodePropData[CswNbtFieldTypeRuleChildContents.AttributeName.FKType];
            }
        }

        private ICswNbtMetaDataProp _getRelationshipProp()
        {
            ICswNbtMetaDataProp RelationshipProp = null;
            if( RelationshipType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                RelationshipProp = _CswNbtResources.MetaData.getNodeTypeProp( RelationshipId );
            }
            else if( RelationshipType == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                RelationshipProp = _CswNbtResources.MetaData.getObjectClassProp( RelationshipId );
            }
            if( RelationshipProp == null )
            {
                throw new CswDniException( "ChildContents RelationshipId is not valid:" + RelationshipId.ToString() );
            }
            return RelationshipProp;
        } // _getRelationshipProp()

        public delegate CswPrimaryKey SetSelectedHandler();
        public SetSelectedHandler SetSelected = null;

        private Dictionary<CswPrimaryKey, string> _getOptions()
        {
            Dictionary<CswPrimaryKey, string> ret = new Dictionary<CswPrimaryKey, string>();

            // Generate a list of all nodes that use this node as the target value of the selected relationship
            ICswNbtMetaDataProp RelationshipProp = _getRelationshipProp();
            if( null != RelationshipProp )
            {
                CswNbtView View = new CswNbtView( _CswNbtResources )
                    {
                        ViewName = "ChildContentsOptions"
                    };
                CswNbtViewRelationship ThisRel = View.AddViewRelationship( this.NodeTypeProp.getNodeType(), false );
                ThisRel.NodeIdsToFilterIn.Add( NodeId );
                View.AddViewRelationship( ThisRel, CswEnumNbtViewPropOwnerType.Second, _getRelationshipProp(), false );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false );
                if( Tree.getChildNodeCount() > 0 )
                {
                    Tree.goToNthChild( 0 );
                    for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                    {
                        Tree.goToNthChild( c );
                        ret.Add( Tree.getNodeIdForCurrentPosition(), Tree.getNodeNameForCurrentPosition() );
                        Tree.goToParentNode();
                    } // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
                } // if( Tree.getChildNodeCount() > 0 )
            } // if( null != RelationshipProp )
            return ret;
        } // _getOptions()

        #endregion Relationship Properties and Functions

        #region Serialization

        public override void ToJSON( JObject ParentObject )
        {
            bool allowAdd = false;
            ParentObject["nodetypeid"] = string.Empty;
            ParentObject["objectclassid"] = string.Empty;

            if( RelationshipType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp RelationshipNTP = _CswNbtResources.MetaData.getNodeTypeProp( RelationshipId );
                CswNbtMetaDataNodeType RelationshipNT = RelationshipNTP.getNodeType();
                if( null != RelationshipNT )
                {
                    ParentObject["nodetypeid"] = RelationshipNT.NodeTypeId;
                    CswNbtMetaDataObjectClass RelationshipOC = RelationshipNT.getObjectClass();
                    if( null != RelationshipOC )
                    {
                        ParentObject["objectclassid"] = RelationshipOC.ObjectClassId;
                        allowAdd = ( RelationshipOC.CanAdd &&
                                     _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, RelationshipNT ) );
                    }
                }
            }
            else if( RelationshipType == CswEnumNbtViewPropIdType.ObjectClassPropId )
            {
                CswNbtMetaDataObjectClassProp RelationshipOCP = _CswNbtResources.MetaData.getObjectClassProp( RelationshipId );
                CswNbtMetaDataObjectClass RelationshipOC = RelationshipOCP.getObjectClass();
                if( null != RelationshipOC )
                {
                    ParentObject["objectclassid"] = RelationshipOC.ObjectClassId;
                    allowAdd = RelationshipOC.CanAdd;
                    if( allowAdd )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in RelationshipOC.getLatestVersionNodeTypes() )
                        {
                            allowAdd = _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, NodeType );
                            if( allowAdd )
                            {
                                break;
                            }
                        }
                    }
                }
            }
            ParentObject["allowadd"] = allowAdd;

            JArray JOptions = new JArray();
            ParentObject["options"] = JOptions;
            ParentObject["relatednodeid"] = string.Empty;
            ParentObject["relatednodename"] = string.Empty;

            Dictionary<CswPrimaryKey, string> Options = _getOptions();
            CswPrimaryKey SelectedNodeId = null;
            if( null != SetSelected )
            {
                SelectedNodeId = SetSelected();
            }
            bool first = true;
            foreach( CswPrimaryKey NodePk in Options.Keys )
            {
                if( first || ( null != NodePk && NodePk == SelectedNodeId ) )
                {
                    // Choose first option by default
                    ParentObject["relatednodeid"] = NodePk.ToString();
                    ParentObject["relatednodename"] = Options[NodePk];
                    first = false;
                }
                if( NodePk != null && NodePk.PrimaryKey != Int32.MinValue )
                {
                    JObject JOption = new JObject();
                    JOption["id"] = NodePk.ToString();
                    JOption["value"] = Options[NodePk];
                    JOptions.Add( JOption );
                }
            }
        } // ToJson()

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // nothing to save
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // nothing to save
        }

        #endregion Serialization

    }//CswNbtNodePropChildContents

}//namespace ChemSW.Nbt.PropTypes
