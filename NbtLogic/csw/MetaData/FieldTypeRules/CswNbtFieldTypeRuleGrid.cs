using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleGrid : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleGrid( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );


            //SubFields.add( CswEnumNbtPropColumn.Field1, "Value" );
            //We are not setting any filter values because there is no search allowed
        }//ctor

        //public CswNbtSubField ValueSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return ( false ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            return ( string.Empty );
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
            public const string WidthInPixels = CswEnumNbtPropertyAttributeName.WidthInPixels;
            public const string View = CswEnumNbtPropertyAttributeName.View;
            public const string DisplayMode = CswEnumNbtPropertyAttributeName.DisplayMode;
            public const string MaximumRows = CswEnumNbtPropertyAttributeName.MaximumRows;
            public const string ShowHeaders = CswEnumNbtPropertyAttributeName.ShowHeaders;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Grid );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Grid,
                    Name = AttributeName.WidthInPixels,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Textareacols
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Grid,
                    Name = AttributeName.View,
                    AttributeFieldType = CswEnumNbtFieldType.ViewReference,
                    Column = CswEnumNbtPropertyAttributeColumn.Nodeviewid,
                    SubFieldName = CswEnumNbtSubFieldName.ViewID
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Grid,
                    Name = AttributeName.DisplayMode,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Extended
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Grid,
                    Name = AttributeName.MaximumRows,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Grid,
                    Name = AttributeName.ShowHeaders,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Attribute1
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }
    }//CswNbtFieldTypeRuleGrid

}//namespace ChemSW.Nbt.MetaData
