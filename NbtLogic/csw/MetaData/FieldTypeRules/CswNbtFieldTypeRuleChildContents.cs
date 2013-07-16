using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleChildContents : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public static CswEnumNbtPropColumn SequenceNumberColumn = CswEnumNbtPropColumn.Field1_Numeric;

        public CswNbtSubField CachedValueSubField;
        public CswNbtSubField SequenceSubField;
        public CswNbtSubField SequenceNumberSubField;

        public CswNbtFieldTypeRuleChildContents( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );
        }//ctor        

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return false; } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            string ret = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, false );
            return ret;
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string IsFK = CswEnumNbtPropertyAttributeName.IsFK;
            public const string ChildRelationship = CswEnumNbtPropertyAttributeName.ChildRelationship;
            public const string FKType = CswEnumNbtPropertyAttributeName.FKType;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.ChildContents );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ChildContents,
                    Name = AttributeName.IsFK,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Isfk
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.ChildContents,
                Name = AttributeName.ChildRelationship,
                AttributeFieldType = CswEnumNbtFieldType.Relationship,
                Column = CswEnumNbtPropertyAttributeColumn.Fkvalue,
                SubFieldName = CswEnumNbtSubFieldName.NodeID
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.ChildContents,
                    Name = AttributeName.FKType,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Fktype
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
