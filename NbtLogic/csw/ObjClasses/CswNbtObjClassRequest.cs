using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassRequest : CswNbtObjClass
    {

        public sealed class PropertyName
        {
            public const string Requestor = "Requestor";
            public const string Name = "Name";
            public const string SubmittedDate = "Submitted Date";
            public const string CompletedDate = "Completed Date";
        }

        public static implicit operator CswNbtObjClassRequest( CswNbtNode Node )
        {
            CswNbtObjClassRequest ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass ) )
            {
                ret = (CswNbtObjClassRequest) Node.ObjClass;
            }
            return ret;
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassRequest( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
            Requestor.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            if( SubmittedDate.WasModified && DateTime.MinValue != SubmittedDate.DateTimeValue )
            {
                CswNbtView RequestItemsView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClass RequestItemsOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtMetaDataObjectClassProp RiRequestOcp = RequestItemsOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Request );

                CswNbtViewRelationship RequestItemVr = RequestItemsView.AddViewRelationship( RequestItemsOc, false );
                RequestItemsView.AddViewPropertyAndFilter( RequestItemVr,
                                                          RequestItemsOc.getObjectClassProp(
                                                              CswNbtObjClassRequestItem.PropertyName.Status ),
                                                          CswNbtObjClassRequestItem.Statuses.Pending,
                                                          FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                RequestItemsView.AddViewPropertyAndFilter( RequestItemVr, RiRequestOcp, NodeId.PrimaryKey.ToString(), SubFieldName: CswNbtSubField.SubFieldName.NodeID );

                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( RequestItemsView, false, false );
                Int32 RequestItemNodeCount = Tree.getChildNodeCount();
                if( RequestItemNodeCount > 0 )
                {
                    for( Int32 N = 0; N < RequestItemNodeCount; N += 1 )
                    {
                        Tree.goToNthChild( N );
                        CswNbtObjClassRequestItem NodeAsRequestItem = _CswNbtResources.Nodes.GetNode( Tree.getNodeIdForCurrentPosition() );
                        NodeAsRequestItem.Status.Value = CswNbtObjClassRequestItem.Statuses.Submitted;
                        NodeAsRequestItem.postChanges( true );
                        Tree.goToParentNode();
                    }
                }
            }
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtMetaDataObjectClassProp RequestorOcp = ObjectClass.getObjectClassProp( PropertyName.Requestor.ToString() );
            ParentRelationship.View.AddViewPropertyAndFilter( ParentRelationship, RequestorOcp, "me" );

            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Requestor
        {
            get { return _CswNbtNode.Properties[PropertyName.Requestor]; }
        }

        public CswNbtNodePropText Name
        {
            get { return _CswNbtNode.Properties[PropertyName.Name]; }
        }

        public CswNbtNodePropDateTime SubmittedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.SubmittedDate]; }
        }

        public CswNbtNodePropDateTime CompletedDate
        {
            get { return _CswNbtNode.Properties[PropertyName.CompletedDate]; }
        }

        #endregion
    }//CswNbtObjClassRequest

}//namespace ChemSW.Nbt.ObjClasses
