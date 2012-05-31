using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt
{
    /// <summary>
    /// Editing and Display mode for Nodes
    /// </summary>
    public enum NodeEditMode
    {
        /// <summary>
        /// Regular editing
        /// </summary>
        Edit,
        /// <summary>
        /// Adding a new node in a popup
        /// </summary>
        Add,
        /// <summary>
        /// Editing a node in a popup
        /// </summary>
        EditInPopup,
        /// <summary>
        /// Editing fake property values (as in Design mode)
        /// </summary>
        Demo,
        /// <summary>
        /// Displaying values for a print report
        /// </summary>
        PrintReport,
        /// <summary>
        /// Editing the default value of a property (in Design)
        /// </summary>
        DefaultValue,
        /// <summary>
        /// Showing node audit history in a popup
        /// </summary>
        AuditHistoryInPopup,
        /// <summary>
        /// A preview of the node, displayed when hovering
        /// </summary>
        Preview,
        /// <summary>
        /// Properties of a node displayed in a Table Layout
        /// </summary>
        Table,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }; // NodeEditMode
} // namespace ChemSW.Nbt


namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Type of Node 
    /// </summary>
    public enum NodeSpecies
    {
        /// <summary>
        /// Unknown
        /// </summary>
        UnKnown,
        /// <summary>
        /// Regular, run-of-the-mill Node
        /// </summary>
        Plain,
        /// <summary>
        /// Audit Node
        /// </summary>
        Audit,
        /// <summary>
        /// Group
        /// </summary>
        Group,
        /// <summary>
        /// Root Node
        /// </summary>
        Root,
        /// <summary>
        /// More Node
        /// </summary>
        More
    };

    //bz # 5943
    public enum NodeModificationState
    {
        Unknown,
        /// <summary>
        /// Unknown
        /// </summary>
        Empty,
        /// <summary>
        /// The node contains no data
        /// </summary>
        Unchanged,
        /// <summary>
        /// The node and its properties have been read from the database
        /// </summary>
        Modified,
        //Set,
        /// <summary>
        /// The value one of the node's selectors or of its properties has been modified
        /// </summary>
        Posted,
        /// <summary>
        /// The Node's data has been written to the database, but not yet committed
        /// </summary>
        //Committed,
        ///// <summary>
        ///// The data written in the Posted phase has been committed
        ///// </summary>
        ///// 
        Deleted
        /// <summary>
        /// The node has been removed from the database
        /// </summary>

    };


    //public enum NodeState { Insert, Update, Delete, Unchanged };
    public class CswNbtNode //: System.IEquatable<CswNbtNode>
    {
        public delegate void OnSetNodeIdHandler( CswNbtNode Node, CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId );
        public delegate void OnRequestWriteNodeHandler( CswNbtNode Node, bool ForceUpdate, bool IsCopy, bool OverrideUniqueValidation );
        public delegate void OnRequestDeleteNodeHandler( CswNbtNode Node );
        public delegate void OnRequestFillHandler( CswNbtNode Node, DateTime Date );
        public delegate void OnRequestFillFromNodeTypeIdHandler( CswNbtNode Node, Int32 NodeTypeId );
        public event OnSetNodeIdHandler OnAfterSetNodeId = null;
        public event OnRequestWriteNodeHandler OnRequestWriteNode = null;
        public event OnRequestDeleteNodeHandler OnRequestDeleteNode = null;
        public event OnRequestFillHandler OnRequestFill = null;
        public event OnRequestFillFromNodeTypeIdHandler OnRequestFillFromNodeTypeId = null;

        private void OnAfterSetNodeIdHandler( CswPrimaryKey OldNodeId, CswPrimaryKey NewNodeId )
        {
            if( OnAfterSetNodeId != null )
                OnAfterSetNodeId( this, OldNodeId, NewNodeId );
        }

        private CswNbtNodePropColl _CswNbtNodePropColl = null;
        //private ICswNbtObjClassFactory _CswNbtObjClassFactory = null;
        private CswNbtObjClass __CswNbtObjClass = null;
        private CswNbtObjClass _CswNbtObjClass
        {
            get
            {
                if( __CswNbtObjClass == null && _NodeTypeId != Int32.MinValue )
                {
                    __CswNbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, _CswNbtResources.MetaData.getObjectClassByNodeTypeId( _NodeTypeId ), this );
                }
                return __CswNbtObjClass;
            }
        }

        private CswNbtResources _CswNbtResources;
        public CswNbtNode( CswNbtResources CswNbtResources, Int32 NodeTypeId, NodeSpecies NodeSpecies, CswPrimaryKey NodeId, Int32 UniqueId, bool IsDemo = false ) //, ICswNbtObjClassFactory ICswNbtObjClassFactory )
        {
            _CswNbtResources = CswNbtResources;
            _UniqueId = UniqueId;
            _NodeId = NodeId;
            _NodeTypeId = NodeTypeId;
            _CswNbtNodePropColl = new CswNbtNodePropColl( CswNbtResources, this, null ); //, ICswNbtObjClassFactory);
            //_CswNbtObjClassFactory = ICswNbtObjClassFactory; // new CswNbtObjClassFactory(CswNbtResources, this);
            _NodeSpecies = NodeSpecies;
            _IsDemo = IsDemo;
            //if( NodeType != null )
            //    ObjectClassId = NodeType.ObjectClassId;

        }//ctor()

        //private NodeState _NodeState = NodeState.Unchanged;
        //public NodeState 

        #region Core Properties

        //bz # 5908
        //We need this because in c# you can't take the address of an object
        private Int32 _UniqueId = Int32.MinValue;
        public Int32 UniqueId
        {
            get
            {
                return ( _UniqueId );
            }//
        }//UniqueId

        //bz # 5943
        private NodeModificationState _NodeModificationState = NodeModificationState.Unknown;
        public NodeModificationState ModificationState
        {
            get
            {

                if( ( NodeModificationState.Unchanged == _NodeModificationState ||
                       NodeModificationState.Posted == _NodeModificationState ) &&
                    _CswNbtNodePropColl.Modified )
                {
                    _NodeModificationState = NodeModificationState.Modified;
                }

                return ( _NodeModificationState );
            }//get

        }//ModificationState

        private bool _IsDemo = false;
        public bool IsDemo { get { return _IsDemo; } set { _IsDemo = value; } }

        private bool _ReadOnly = false;
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        private bool _Locked = false;
        public bool Locked
        {
            get { return _Locked; }
            set { _Locked = value; }
        }

        //bz # 5943
        //private bool _Modified = false;
        //public bool Modified
        //{
        //    get { return ( _Modified || _CswNbtNodePropColl.Modified ); }
        //    set { _Modified = value; }
        //}//Modified

        //public void clearModifiedFlag()
        //{
        //    _CswNbtNodePropColl.clearModifiedFlag();
        //    _Modified = false;
        //}

        public bool Filled
        {
            get
            {
                return ( _CswNbtNodePropColl.Filled );
            }//get
        }//Filled

        public bool SuspendModifyTracking
        {
            get { return _CswNbtNodePropColl.SuspendModifyTracking; }
            set { _CswNbtNodePropColl.SuspendModifyTracking = value; }
        }

        public bool DisableSave = false;

        public bool New
        {
            get
            {
                return ( _CswNbtNodePropColl.CreatedFromNodeTypeId );
            }
        }//New

        private NodeSpecies _NodeSpecies = NodeSpecies.UnKnown;
        public NodeSpecies NodeSpecies { get { return ( _NodeSpecies ); } }


        private CswPrimaryKey _NodeId = null;
        public CswPrimaryKey NodeId
        {
            get
            {
                return ( _NodeId );
            }//get

            set
            {
                CswPrimaryKey OldNodeId = _NodeId;
                _NodeId = value;

                // fix properties
                Properties._NodePk = _NodeId;

                OnAfterSetNodeIdHandler( OldNodeId, _NodeId );
            }//set

        }//NodeId

        //private CswNbtNodeKey _NodeKey = null;
        //public CswNbtNodeKey NodeKey
        //{
        //    get { return ( _NodeKey ); }

        //    set
        //    {
        //        _NodeKey = value;
        //        if( null != onAfterSetNodeKey )
        //        {
        //            onAfterSetNodeKey( this );
        //        }//
        //    }//set

        //}//NodeKey

        //private Int32 _ParentNodeId = 0;
        //public Int32 ParentNodeId { get { return ( _ParentNodeId ); } set { _ParentNodeId = value; } }

        //private CswNbtNode _ParentNode = null;
        //public CswNbtNode ParentNode 
        //{ 
        //    get { return ( _ParentNode ); } 

        //    set { 

        //        _ParentNode = value;
        //        if( null != _ParentNode.NodeKey && null != NodeKey )
        //        {
        //            NodeKey.ParentNodeKey = _ParentNode.NodeKey;
        //        }
        //    } 
        //}

        private Int32 _NodeTypeId = 0;
        public Int32 NodeTypeId
        {
            get
            {
                return ( _NodeTypeId );
            }
            set
            {
                _NodeTypeId = value;
            }
        }


        //private CswNbtMetaDataNodeType _NodeType = null;
        public CswNbtMetaDataNodeType getNodeType()
        {
            return _CswNbtResources.MetaData.getNodeType( NodeTypeId );
        }

        public CswNbtMetaDataNodeType getNodeTypeLatestVersion()
        {
            return _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeTypeId );
        }

        //private string _NodeTypeName = "";
        //public string NodeTypeName { get { return ( _NodeTypeName ); } set { _NodeTypeName = value; } }

        public Int32 getObjectClassId()
        {
            return getNodeType().ObjectClassId;
        }

        public CswNbtMetaDataObjectClass getObjectClass()
        {
            return _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeId );
        }

        public CswNbtObjClass ObjClass
        {
            get { return _CswNbtObjClass; }
        }

        private void _clear()
        {
            _NodeName = string.Empty;
            _PendingUpdate = false;
        }//clear()

        private string _NodeName = string.Empty;
        public string NodeName
        {
            get { return ( _NodeName ); }
            set
            {
                // case 20781 - only mark modified if we're changing the name, not assigning it from DB
                if( _NodeName != value && _NodeName != string.Empty )
                {
                    //bz # 5943
                    //_Modified = true;
                    _NodeModificationState = NodeModificationState.Modified;
                }
                _NodeName = value;
            }
        }
        private bool _PendingUpdate = false;
        public bool PendingUpdate
        {
            get { return ( _PendingUpdate ); }
            set
            {
                if( _PendingUpdate != value )
                {
                    _PendingUpdate = value;
                    //bz # 5943
                    //_Modified = true;
                    _NodeModificationState = NodeModificationState.Modified;
                }
            }
        }

        private ArrayList _TemplatePropsAl = new ArrayList();
        //private string _NameTemplate = "";
        //public string NameTemplate
        //{
        //    get { return ( _NameTemplate ); }

        //    set
        //    {
        //        _NameTemplate = value;
        //    }

        //}//NameTemplate 

        //public IEnumerable TemplateProps
        //{
        //    get
        //    {
        //        if( NodeType.NameTemplate.Length > 0 && 0 == _TemplatePropsAl.Count )
        //        {
        //            RegexOptions RegExOpts = new RegexOptions();
        //            RegExOpts |= RegexOptions.IgnoreCase;
        //            string TemplateRegEx = @"\[(.*?)\]";
        //            Regex RegEx = new Regex( TemplateRegEx, RegExOpts );
        //            Match RegExMatch = RegEx.Match( _NameTemplate );
        //            while( RegExMatch.Success )
        //            {
        //                string TemplateParameter = RegExMatch.Groups[ 1 ].ToString();
        //                _TemplatePropsAl.Add( TemplateParameter );
        //                RegExMatch = RegExMatch.NextMatch();
        //            }//iterate matches
        //        }//

        //        return ( _TemplatePropsAl );
        //    }//

        //}//TemplateProps



        private string _IconFileName = "";
        public string IconFileName { get { return ( _IconFileName ); } set { _IconFileName = value; } }

        private bool _Selectable = true;
        public bool Selectable { get { return ( _Selectable ); } set { _Selectable = value; } }

        //private bool _ShowInGrid = true;
        //public bool ShowInGrid { get { return ( _ShowInGrid ); } set { _ShowInGrid = value; } }
        private bool _ShowInTree = true;
        public bool ShowInTree { get { return ( _ShowInTree ); } set { _ShowInTree = value; } }

        //private CswNbtView.AddChildrenSetting _AddChildren = CswNbtView.AddChildrenSetting.None;
        //public CswNbtView.AddChildrenSetting AddChildren { get { return ( _AddChildren ); } set { _AddChildren = value; } }

        public CswNbtNodePropColl Properties { get { return ( _CswNbtNodePropColl ); } }

        //private CswNbtViewNode _ViewNode;
        //public CswNbtViewNode ViewNode { get { return _ViewNode; } set { _ViewNode = value; } }

        public override int GetHashCode()
        {
            return this.NodeId.PrimaryKey;
        }
        #endregion

        #region Methods


        //bz # 5943
        public void postChanges( bool ForceUpdate )
        {
            postChanges( ForceUpdate, false, false );
        }

        public void postChanges( bool ForceUpdate, bool IsCopy, bool OverrideUniqueValidation = false )
        {
            if( NodeModificationState.Modified == ModificationState || ForceUpdate )
            {
                if( null == OnRequestWriteNode )
                    throw ( new CswDniException( "There is no write handler" ) );

                bool IsNew = ( this.NodeId == null || this.NodeId.PrimaryKey == Int32.MinValue );
                if( null != _CswNbtObjClass )
                {
                    if( IsNew )
                        _CswNbtObjClass.beforeCreateNode( OverrideUniqueValidation );
                    else
                        _CswNbtObjClass.beforeWriteNode( IsCopy, OverrideUniqueValidation );
                }

                OnRequestWriteNode( this, ForceUpdate, IsCopy, OverrideUniqueValidation );

                if( null != _CswNbtObjClass )
                {
                    if( IsNew )
                        _CswNbtObjClass.afterCreateNode();
                    else
                        _CswNbtObjClass.afterWriteNode();
                }

                _NodeModificationState = NodeModificationState.Posted;

                //reset(); //bz # 6713
                // But see BZ 9650 and BZ 8517
            }
        }//postChanges()

        //bz # 5943
        //public void beforeWriteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.beforeWriteNode();
        //    }//
        //}//beforeWriteNode()

        //bz # 5943
        //public void afterWriteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.afterWriteNode();
        //    }//
        //}//afterWriteNode()

        /// <summary>
        /// Get a tree view of this node, visible to the current user
        /// </summary>
        /// <returns></returns>
        public CswNbtView getViewOfNode()
        {
            CswNbtView Ret = getNodeType().CreateDefaultView();
            Ret.Root.ChildRelationships[0].NodeIdsToFilterIn.Add( NodeId );
            Ret.ViewMode = NbtViewRenderingMode.Tree;
            Ret.Visibility = NbtViewVisibility.User;
            Ret.VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId;
            return Ret;
        }

        //bz # 5943
        public void delete( bool DeleteAllRequiredRelatedNodes = false )
        {
            if( null == OnRequestDeleteNode )
            {
                throw ( new CswDniException( "There is no delete handler" ) );
            }
            CswNbtMetaDataNodeType thisNT = this.getNodeType();
            if( !_CswNbtResources.Permit.can( Security.CswNbtPermit.NodeTypePermission.Delete, thisNT ) )
            {
                throw ( new CswDniException( ErrorType.Warning, "You do not have permission to delete this " + thisNT.NodeTypeName, "User attempted to delete a " + thisNT.NodeTypeName + " without Delete permissions" ) );
            }

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.beforeDeleteNode(DeleteAllRequiredRelatedNodes: DeleteAllRequiredRelatedNodes);
            }

            OnRequestDeleteNode( this );

            if( null != _CswNbtObjClass )
            {
                _CswNbtObjClass.afterDeleteNode();
            }

            _NodeModificationState = NodeModificationState.Deleted;

        }//delete()

        ////bz # 6713
        // But see BZ 8517 and BZ 9650
        //public void reset()
        //{
        //    if( ModificationState == NodeModificationState.Modified )
        //        throw( new CswDniException( "There are pending changes -- reset not allowed" ) );

        //    _clear();
        //    fill();
        //    Properties.clearModifiedFlag();
        //    _NodeModificationState = NodeModificationState.Unchanged;

        //}//reset()

        //bz # 5943
        //public void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClassbeforeDeleteNode();
        //    }//
        //}//beforeDeleteNode()


        //bz # 5943
        //public void afterDeleteNode()
        //{
        //    if( null != _CswNbtObjClass )
        //    {
        //        _CswNbtObjClass.afterDeleteNode();
        //    }//
        //}//afterDeleteNode()


        //bz # 5943

        public void fill( DateTime Date )
        {
            if( null == OnRequestFill )
                throw ( new CswDniException( "There is no fill handler" ) );

            OnRequestFill( this, Date );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//fill() 


        public void fillFromNodeTypeId( Int32 NodeTypeId )
        {
            if( null == OnRequestFillFromNodeTypeId )
                throw ( new CswDniException( "There is no fill handler" ) );

            OnRequestFillFromNodeTypeId( this, NodeTypeId );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//fillFromNodeTypeId()


        public void cancelChanges()
        {
            Int32 NodeTypeId = _NodeTypeId;

            OnRequestFillFromNodeTypeId( this, NodeTypeId );

            _NodeModificationState = NodeModificationState.Unchanged;

        }//cancelChanges()


        /// <summary>
        /// Copies all matching properties (by name and field type) from another node. 
        /// </summary>
        /// <param name="SourceNode">Node from which to copy property values</param>
        public void copyPropertyValues( CswNbtNode SourceNode )
        {
            foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
            {
                foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
                {
                    if( ThisProp.PropName == SourceProp.PropName && ThisProp.getFieldType().FieldType == SourceProp.getFieldType().FieldType )
                    {
                        ThisProp.copy( SourceProp );
                    } // if( ThisProp.PropName == SourceProp.PropName && ThisProp.FieldType == SourceProp.FieldType )
                } // foreach( CswNbtNodePropWrapper ThisProp in this.Properties )
            } // foreach( CswNbtNodePropWrapper SourceProp in SourceNode.Properties )
        } // copyPropertyValues()

        /// <summary>
        /// Sets the values of all relationships whose target matches 
        /// the ParentNode's nodetypeid or objectclassid to the ParentNode's nodeid.
        /// </summary>
        public void RelateToNode( CswNbtNode ParentNode, CswNbtView View )
        {
            CswNbtNodePropWrapper Prop = null;
            // BZ 10372 - Iterate all relationships
            foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                // BZ 8355 - Set relationships on children pointing to parents, not the other way
                if( ViewRelationship.PropOwner == NbtViewPropOwnerType.Second )
                {
                    if( ( ( ViewRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId && ViewRelationship.SecondId == this.NodeTypeId ) ||
                          ( ViewRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId && ViewRelationship.SecondId == this.getObjectClassId() ) ) &&
                        ( ( ViewRelationship.FirstType == NbtViewRelatedIdType.NodeTypeId && ViewRelationship.FirstId == ParentNode.NodeTypeId ) ||
                          ( ViewRelationship.FirstType == NbtViewRelatedIdType.ObjectClassId && ViewRelationship.FirstId == ParentNode.getObjectClassId() ) ) )
                    {
                        if( ViewRelationship.PropType == NbtViewPropIdType.NodeTypePropId )
                            Prop = this.Properties[_CswNbtResources.MetaData.getNodeTypeProp( ViewRelationship.PropId )];
                        else if( ViewRelationship.PropType == NbtViewPropIdType.ObjectClassPropId )
                            Prop = this.Properties[_CswNbtResources.MetaData.getObjectClassProp( ViewRelationship.PropId ).PropName];

                        if( Prop != null )
                        {
                            CswNbtMetaDataFieldType.NbtFieldType FT = Prop.getFieldType().FieldType;
                            if( FT == CswNbtMetaDataFieldType.NbtFieldType.Relationship )
                            {
                                Prop.AsRelationship.RelatedNodeId = ParentNode.NodeId;
                                Prop.AsRelationship.RefreshNodeName();
                            }
                            if( FT == CswNbtMetaDataFieldType.NbtFieldType.Location )
                            {
                                Prop.AsLocation.SelectedNodeId = ParentNode.NodeId;
                                Prop.AsLocation.RefreshNodeName();
                            }
                        }
                    }
                } // if( ViewRelationship.PropOwner == PropOwnerType.Second )
            } // foreach( CswNbtViewRelationship ViewRelationship in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
        } // RelateToNode()

        #endregion Methods


    }//CswNbtNode

}//namespace ChemSW.Nbt.ObjClasses
