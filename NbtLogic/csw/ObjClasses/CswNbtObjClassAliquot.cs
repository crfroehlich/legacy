using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassAliquot : CswNbtObjClass
    {
        public static string QuantityPropertyName { get { return "Quantity"; } }
        //public string IncrementPropertyName { get { return "Increment"; } }
        public static string BarcodePropertyName { get { return "Barcode"; } }
        public static string LocationPropertyName { get { return "Location"; } }
        public static string SamplePropertyName { get { return "Sample"; } }
        public static string ParentAliquotPropertyName { get { return "Parent Aliquot"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassAliquot( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassAliquot
        /// </summary>
        public static implicit operator CswNbtObjClassAliquot( CswNbtNode Node )
        {
            CswNbtObjClassAliquot ret = null;
            if( _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.AliquotClass ) )
            {
                ret = (CswNbtObjClassAliquot) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

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
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropQuantity Quantity { get { return ( _CswNbtNode.Properties[QuantityPropertyName].AsQuantity ); } }
        //public CswNbtNodePropText Increment { get { return ( _CswNbtNode.Properties[IncrementPropertyName].AsText ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[BarcodePropertyName].AsBarcode ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[LocationPropertyName].AsLocation ); } }
        public CswNbtNodePropRelationship Sample { get { return ( _CswNbtNode.Properties[SamplePropertyName].AsRelationship ); } }
        public CswNbtNodePropRelationship ParentAliquot { get { return ( _CswNbtNode.Properties[ParentAliquotPropertyName].AsRelationship ); } }

        #endregion

    }//CswNbtObjClassAliquot

}//namespace ChemSW.Nbt.ObjClasses
