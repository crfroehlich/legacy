using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUnitOfMeasure : CswNbtObjClass
    {
        public static string NamePropertyName { get { return "Name"; } }
        public static string UnitTypePropertyName { get { return "Unit Type"; } }
        public static string ConversionFactorPropertyName { get { return "Conversion Factor"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass ); }
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

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
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

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ActionObj )
        {
            if( null != NodeTypeProp ) { /*Do Something*/ }
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        public CswNbtNodePropScientific ConversionFactor
        {
            get
            {
                return ( _CswNbtNode.Properties[ConversionFactorPropertyName].AsScientific );
            }
        }

        public CswNbtNodePropRelationship UnitType
        {
            get
            {
                return ( _CswNbtNode.Properties[UnitTypePropertyName].AsRelationship );
            }
        }
        #endregion

    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
