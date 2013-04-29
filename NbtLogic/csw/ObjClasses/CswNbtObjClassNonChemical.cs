using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassNonChemical : CswNbtPropertySetMaterial
    {
        #region Base

        /// <summary>
        /// Ctor
        /// </summary>
        public CswNbtObjClassNonChemical( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) { }

        /// <summary>
        /// Implicit cast of Node to Object Class
        /// </summary>
        public static implicit operator CswNbtObjClassNonChemical( CswNbtNode Node )
        {
            CswNbtObjClassNonChemical ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.NonChemicalClass ) )
            {
                ret = (CswNbtObjClassNonChemical) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// Object Class
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass ); }
        }

        /// <summary>
        /// Cast a Request Item PropertySet back to an Object Class
        /// </summary>
        public static CswNbtObjClassNonChemical fromPropertySet( CswNbtPropertySetMaterial PropertySet )
        {
            return PropertySet.Node;
        }

        /// <summary>
        /// Cast a the Object Class as a PropertySet
        /// </summary>
        public static CswNbtPropertySetMaterial toPropertySet( CswNbtObjClassNonChemical ObjClass )
        {
            return ObjClass;
        }

        #endregion Base

        #region Enums

        public new sealed class PropertyName : CswNbtPropertySetMaterial.PropertyName
        {
            //NonChemical-specific properties go here
        }

        #endregion Enums

        #region Inherited Events

        public override void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation ) {}

        public override void afterPropertySetWriteNode() {}

        public override void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes = false ) {}

        public override void afterPropertySetDeleteNode() {}

        public override void afterPropertySetPopulateProps() {}

        public override bool onPropertySetButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        public override void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) {}

        #endregion Inherited Events

        #region Custom Logic

        //NonChemical Materials don't have an expiration interval, so their containers won't have an expiration date defined.
        public override DateTime getDefaultExpirationDate()
        {
            return DateTime.MinValue;
        }

        #endregion Custom Logic

        #region ObjectClass-specific properties

        //NonChemcial-specific properties go here

        #endregion ObjectClass-specific properties

    }//CswNbtObjClassNonChemical

}//namespace ChemSW.Nbt.ObjClasses