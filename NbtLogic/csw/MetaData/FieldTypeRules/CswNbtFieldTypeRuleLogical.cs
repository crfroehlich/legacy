using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleLogical : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Checked = CswEnumNbtSubFieldName.Checked;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;


        public CswNbtFieldTypeRuleLogical( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            CheckedSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Checked );
            CheckedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CheckedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            SubFields.add( CheckedSubField );

        }//ctor

        public CswNbtSubField CheckedSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }
        private string _FilterTableAlias = "jnp.";

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            string ValueColumn = _FilterTableAlias + CswNbtSubField.Column.ToString();
            string ReturnVal = "";

            if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
            {
                if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                {
                    ReturnVal = ValueColumn + " = '1' ";
                }
                else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                {
                    ReturnVal = ValueColumn + " = '0' ";
                }
                else
                {
                    ReturnVal = ValueColumn + " is null";
                }
            }
            else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
            {
                if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                {
                    ReturnVal = "(" + ValueColumn + " = '0' or " + ValueColumn + " is null) ";
                }
                else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                {
                    ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " is null) ";
                }
                else
                {
                    ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " = '0') ";
                }
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtFieldTypeRuleLogical.renderViewPropFilter()" );
            }

            return ( ReturnVal );
        }

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            _CswNbtFieldTypeRuleDefault.onSetFk( DesignNTPNode );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Logical );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Logical,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
