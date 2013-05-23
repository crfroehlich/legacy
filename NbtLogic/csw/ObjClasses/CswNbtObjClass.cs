using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public abstract class CswNbtObjClass
    {
        //protected CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        protected CswNbtNode _CswNbtNode = null;
        protected CswNbtResources _CswNbtResources = null;

        private bool canSave( string TabId )
        {
            Int32 TabIdInt = CswConvert.ToInt32( TabId );
            bool Ret = false;
            if( null != this.Node )
            {
                switch( _CswNbtResources.EditMode )
                {
                    case CswEnumNbtNodeEditMode.Temp:
                    case CswEnumNbtNodeEditMode.Add:
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Create, this.NodeType ) )
                        {
                            Ret = true;
                        }
                        break;
                    case CswEnumNbtNodeEditMode.EditInPopup:
                    case CswEnumNbtNodeEditMode.Edit:
                        if( TabIdInt > 0 )
                        {
                            CswNbtMetaDataNodeTypeTab Tab = this.NodeType.getNodeTypeTab( TabIdInt );
                            if( null != Tab )
                            {
                                Ret = _CswNbtResources.Permit.canTab( CswEnumNbtNodeTypePermission.Edit, this.NodeType, Tab );
                            }
                        }
                        else
                        {
                            Ret = _CswNbtResources.Permit.canAnyTab( CswEnumNbtNodeTypePermission.Edit, this.NodeType );
                        }
                        break;
                }
            }
            return Ret;
        }

        /// <summary>
        /// Constructor for when we have a node instance
        /// </summary>
        public CswNbtObjClass( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
        {
            _CswNbtNode = CswNbtNode;
            _CswNbtResources = CswNbtResources;
        }//ctor()

        /// <summary>
        /// Post node property changes to the database
        /// </summary>
        /// <param name="ForceUpdate">If true, an update will happen whether properties have been modified or not</param>
        public void postChanges( bool ForceUpdate ) //bz# 5446
        {
            _CswNbtNode.postChanges( ForceUpdate );
        }//postChanges()

        public abstract CswNbtMetaDataObjectClass ObjectClass { get; }
        public abstract void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation );
        public abstract void afterCreateNode();
        public abstract void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation );
        public abstract void afterWriteNode();
        public abstract void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false );
        public abstract void afterDeleteNode();

        public void triggerAfterPopulateProps()
        {
            //We don't have a context for which Tab is going to render, but we can eliminate the base conditions for displaying the Save button here.
            //if( null != this.Node && false == canSave( TabId : Int32.MinValue ) )
            //{
            //    Save.setHidden( value : true, SaveToDb : false );
            //}
            afterPopulateProps();
        }
        protected abstract void afterPopulateProps();

        private void _onButtonClickSaveProps( string TabId, NbtButtonData ButtonData )
        {
            Int32 TabIdAsInt = CswConvert.ToInt32( TabId );
            JObject SelectedTab = null;
            if( null != ButtonData.PropsToSave && ButtonData.PropsToSave.HasValues )
            {
                SelectedTab = CswConvert.ToJObject( ButtonData.PropsToSave[TabId] );
            }
            if( TabIdAsInt > 0 || ( null != SelectedTab && SelectedTab.HasValues ) )
            {
                if( canSave( TabId ) )
                {
                    CswNbtSdTabsAndProps Sd = new CswNbtSdTabsAndProps( _CswNbtResources );

                    Sd.saveProps( this.NodeId, TabIdAsInt, SelectedTab, this.NodeTypeId, null, false );
                    ButtonData.PropsToReturn = Sd.getProps( NodeId.ToString(), null, TabId, NodeTypeId, null, null, null, null, null, ForceReadOnly : false );
                    ButtonData.Action = CswEnumNbtButtonAction.refresh;
                    if( ButtonData.NodeIds.Count > 1 && ButtonData.PropIds.Count > 0 )
                    {
                        CswNbtObjClassBatchOp Batch = Sd.copyPropValues( this.Node, ButtonData.NodeIds, ButtonData.PropIds );
                        if( null != Batch )
                        {
                            ButtonData.Action = CswEnumNbtButtonAction.batchop;
                            ButtonData.Data["batch"] = Batch.Node.NodeLink;
                        }
                    }

                }
            }
        }

        public bool triggerOnButtonClick( NbtButtonData ButtonData )
        {
            bool Ret = false;
            if( null != ButtonData )
            {
                Collection<Int32> TabIds = new Collection<int>();
                if( null != ButtonData.TabIds )
                {
                    TabIds = ButtonData.TabIds.ToIntCollection( ExcludeMinVal: true, ExcludeDuplicates: true );
                }

                if( TabIds.Count > 0 )
                {
                    foreach( Int32 TabId in TabIds )
                    {
                        _onButtonClickSaveProps( CswConvert.ToString( TabId ), ButtonData );
                    }
                }
                else
                {
                    if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                    {
                        //Client-side, we are defining a tabid as EditMode + '_tab'. This isn't great, but it's what we've got right now.
                        _onButtonClickSaveProps( CswEnumNbtNodeEditMode.Add + "_tab", ButtonData );
                    }
                }
                if( _CswNbtResources.EditMode == CswEnumNbtNodeEditMode.Add )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.refreshonadd;
                }
                
                if( null != ButtonData.NodeTypeProp && ButtonData.NodeTypeProp.IsSaveProp )
                {
                    Ret = true;
                }
                else
                {
                    Ret = onButtonClick( ButtonData );
                }
            }
            return Ret;
        }

        protected abstract bool onButtonClick( NbtButtonData ButtonData );

        public abstract void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship );

        public virtual CswNbtNode CopyNode( Action<CswNbtNode> OnCopy = null )
        {
            CswNbtNode CopiedNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    NewNode.copyPropertyValues( Node );
                    if( null != OnCopy )
                    {
                        OnCopy( NewNode );
                    }
                } );
            return CopiedNode;
        }

        public abstract class PropertyName
        {
            public const string Save = "Save";
        }

        public virtual CswNbtNodePropButton Save
        {
            get
            {
                CswNbtNodePropButton Ret = Node.Properties[PropertyName.Save];

                return Ret;
            }
        }

        public Int32 NodeTypeId { get { return _CswNbtNode.NodeTypeId; } }
        public CswNbtMetaDataNodeType NodeType { get { return _CswNbtResources.MetaData.getNodeType( _CswNbtNode.NodeTypeId ); } }
        public string NodeName { get { return _CswNbtNode.NodeName; } }
        public CswPrimaryKey NodeId { get { return _CswNbtNode.NodeId; } }
        public CswNbtNode Node { get { return _CswNbtNode; } }
        public bool IsDemo { get { return _CswNbtNode.IsDemo; } set { _CswNbtNode.IsDemo = value; } }
        public bool IsTemp { get { return _CswNbtNode.IsTemp; } set { _CswNbtNode.IsTemp = value; } }

        public CswPrimaryKey RelationalId
        {
            get { return _CswNbtNode.RelationalId; }
            set { _CswNbtNode.RelationalId = value; }
        }

        public class NbtButtonData
        {
            public NbtButtonData( CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            {
                Data = new JObject();
                Action = CswEnumNbtButtonAction.Unknown;

                Debug.Assert( null != CswNbtMetaDataNodeTypeProp, "CswNbtMetaDataNodeTypeProp is null." );
                if( null == CswNbtMetaDataNodeTypeProp )
                {
                    throw new CswDniException( "Property is unknown." );
                }
                NodeTypeProp = CswNbtMetaDataNodeTypeProp;
            }
            public void clone( NbtButtonData DataToCopy )
            {
                if( null != DataToCopy )
                {
                    if( null != DataToCopy.Action )
                    {
                        Action = DataToCopy.Action;
                    }
                    if( null != DataToCopy.SelectedText )
                    {
                        SelectedText = DataToCopy.SelectedText;
                    }
                    if( null != DataToCopy.Data )
                    {
                        Data = DataToCopy.Data;
                    }
                    if( null != DataToCopy.Message )
                    {
                        Message = DataToCopy.Message;
                    }
                }
            }

            public CswEnumNbtButtonAction Action = CswEnumNbtButtonAction.nothing;
            public string SelectedText = string.Empty;
            public CswNbtMetaDataNodeTypeProp NodeTypeProp = null;
            public JObject Data = new JObject();
            public JObject PropsToSave = new JObject();
            public JObject PropsToReturn = new JObject();
            public CswCommaDelimitedString TabIds = new CswCommaDelimitedString();
            public string Message = string.Empty;
            public CswCommaDelimitedString NodeIds = new CswCommaDelimitedString();
            public CswCommaDelimitedString PropIds = new CswCommaDelimitedString();
        }

        // For validating object class casting
        protected static bool _Validate( CswNbtNode Node, CswEnumNbtObjectClass TargetObjectClass )
        {
            if( Node == null )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid node", "CswNbtObjClass._Validate was given a null node as a parameter" );
            }

            if( !( Node.getObjectClass().ObjectClass == TargetObjectClass ) )
            {
                throw ( new CswDniException( CswEnumErrorType.Error, "Invalid cast", "Can't cast current object class as " + TargetObjectClass.ToString() + "; Current object class is " + Node.getObjectClass().ObjectClass.ToString() ) );
            }
            return true;
        }

    }//CswNbtObjClass

}//namespace ChemSW.Nbt.ObjClasses
