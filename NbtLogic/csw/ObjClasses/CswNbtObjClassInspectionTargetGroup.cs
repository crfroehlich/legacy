using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInspectionTargetGroup : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassInspectionTargetGroup( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassInspectionTargetGroup
        /// </summary>
        public static implicit operator CswNbtObjClassInspectionTargetGroup( CswNbtNode Node )
        {
            CswNbtObjClassInspectionTargetGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InspectionTargetGroupClass ) )
            {
                ret = (CswNbtObjClassInspectionTargetGroup) Node.ObjClass;
            }
            return ret;
        }

        private void _setDefaultValues()
        {
            if( string.IsNullOrEmpty( Name.Text ) )
            {
                CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );
                CswNbtMetaDataObjectClassProp OwnerOCP = GeneratorOC.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
                CswNbtMetaDataNodeTypeProp OwnerNTP;
                CswNbtMetaDataNodeType OwnerNT;
                //CswNbtMetaDataObjectClass OwnerOC;
                CswNbtNode GeneratorNode;
                //CswNbtObjClassGenerator NewGenerator;

                foreach( CswNbtMetaDataNodeType NodeType in GeneratorOC.getNodeTypes() )
                {
                    OwnerNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
                    if( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType )
                    {
                        OwnerNT = _CswNbtResources.MetaData.getNodeType( OwnerNTP.FKValue );
                        if( null != OwnerNT && OwnerNT == Node.getNodeType() )
                        {
                            GeneratorNode = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeType.NodeTypeId, delegate( CswNbtNode NewGenerator )
                                {
                                    if( null != NewGenerator )
                                    {
                                        //NewGenerator = (CswNbtObjClassGenerator) GeneratorNode;
                                        ( (CswNbtObjClassGenerator) NewGenerator ).Owner.RelatedNodeId = this.NodeId;
                                        ( (CswNbtObjClassGenerator) NewGenerator ).Owner.RefreshNodeName(); // 20959
                                        //GeneratorNode.postChanges( true );
                                    }
                                } );
                        }
                    } //RelatedIdType.NodeTypeId.ToString() == OwnerNTP.FKType
                    //else if( RelatedIdType.ObjectClassId.ToString() == OwnerNTP.FKType )
                }
            }
        }

        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforePromoteNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }//afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _setDefaultValues();
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }

        #endregion

    }//CswNbtObjClassLocation

}//namespace ChemSW.Nbt.ObjClasses
