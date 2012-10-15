using System;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEnterprisePart : CswNbtObjClass
    {
        public sealed class PropertyName
        {
            public const string GCAS = "GCAS";
            public const string Request = "Request";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassEnterprisePart( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );

        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.EnterprisePartClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEnterprisePart
        /// </summary>
        public static implicit operator CswNbtObjClassEnterprisePart( CswNbtNode Node )
        {
            CswNbtObjClassEnterprisePart ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.EnterprisePartClass ) )
            {
                ret = (CswNbtObjClassEnterprisePart) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
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
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {



            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText GCAS { get { return _CswNbtNode.Properties[PropertyName.GCAS]; } }
        public CswNbtNodePropButton Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }

        #endregion

    }//CswNbtObjClassEnterprisePart

}//namespace ChemSW.Nbt.ObjClasses