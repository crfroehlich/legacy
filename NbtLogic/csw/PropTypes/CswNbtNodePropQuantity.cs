using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropQuantity: CswNbtNodeProp
    {
        #region Private Variables

        private CswNbtFieldTypeRuleQuantity _FieldTypeRule;
        private CswNbtSubField _QuantitySubField;
        private CswNbtSubField _UnitNameSubField;
        private CswNbtView _View;
        public static implicit operator CswNbtNodePropQuantity( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsQuantity;
        }

        private CswNbtSubField _UnitIdSubField;

        #endregion

        #region Constructor

        public CswNbtNodePropQuantity( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleQuantity) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _QuantitySubField = _FieldTypeRule.QuantitySubField;
            _UnitNameSubField = _FieldTypeRule.UnitNameSubField;
            _UnitIdSubField = _FieldTypeRule.UnitIdSubField;
        }

        #endregion

        #region Public Properties

        override public bool Empty
        {
            get
            {
                return Required && QuantityOptional ? false == CswTools.IsDouble( Quantity ) : 0 == Gestalt.Length;
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }//Gestalt

        public Int32 Precision
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.NumberPrecision != Int32.MinValue )
                    return _CswNbtMetaDataNodeTypeProp.NumberPrecision;
                else
                    return 6;
            }
        }
        public double MinValue
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.MinValue;
            }
        }
        public double MaxValue
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.MaxValue;
            }
        }

        /// <summary>
        /// When set to true, the MinValue and MaxValue limits are not included in the allowed number range.
        /// </summary>
        public bool ExcludeRangeLimits
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute2 );
            }
        }

        /// <summary>
        /// When set to true, quantity can be blank even if the field is required.
        /// </summary>
        public bool QuantityOptional
        {
            get
            {
                return CswConvert.ToBoolean( _CswNbtMetaDataNodeTypeProp.Attribute1 );
            }
        }

        public Collection<CswNbtNode> UnitNodes
        {
            get
            {
                Collection<CswNbtNode> _UnitNodes = new Collection<CswNbtNode>();

                ICswNbtTree UnitsTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );
                UnitsTree.goToRoot();
                for( int i = 0; i < UnitsTree.getChildNodeCount(); i++ )
                {
                    UnitsTree.goToNthChild( i );
                    _UnitNodes.Add( UnitsTree.getNodeForCurrentPosition() );
                    UnitsTree.goToParentNode();
                }
                return _UnitNodes;
            }
        }

        public double Quantity
        {
            get
            {
                string Value = _CswNbtNodePropData.GetPropRowValue( _QuantitySubField.Column );
                if( CswTools.IsFloat( Value ) )
                    return Convert.ToDouble( Value );
                else
                    return Double.NaN;
            }
            set
            {
                string StringVal = string.Empty;
                if( Double.IsNaN( value ) )
                {
                    if( Required &&
                        false == QuantityOptional &&
                        false == _AllowSetNull )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot save a Quantity without a value if the Property is required.", "Attempted to save the Quantity of a Quantity with an invalid number." );
                    }
                    _CswNbtNodePropData.SetPropRowValue( _QuantitySubField.Column, Double.NaN );
                }
                else
                {
                    string PrecisionString = "";
                    for( int i = 0; i < Precision; i++ )
                    {
                        PrecisionString += "#";
                    }
                    StringVal = Math.Round( value, Precision, MidpointRounding.AwayFromZero ).ToString( "0." + PrecisionString );
                    _CswNbtNodePropData.SetPropRowValue( _QuantitySubField.Column, StringVal );
                }
                SyncGestalt();
            }
        }

        public string CachedUnitName
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _UnitNameSubField.Column );
            }
            set
            {
                if( value != _CswNbtNodePropData.GetPropRowValue( _UnitNameSubField.Column ) )
                {
                    _CswNbtNodePropData.SetPropRowValue( _UnitNameSubField.Column, value );
                    SyncGestalt();
                }
            }
        }

        private bool _AllowSetNull = false;
        /// <summary>
        /// Empty the subfield data on this Prop
        /// </summary>
        public void clearQuantity( bool ForceClear = false )
        {
            _AllowSetNull = ForceClear;
            CachedUnitName = "";
            Quantity = double.NaN;
            UnitId = null;
        }

        public CswPrimaryKey UnitId
        {
            get
            {
                CswPrimaryKey ret = null;
                string StringVal = _CswNbtNodePropData.GetPropRowValue( _UnitIdSubField.Column );
                if( CswTools.IsInteger( StringVal ) )
                    ret = new CswPrimaryKey( TargetTableName, CswConvert.ToInt32( StringVal ) );
                return ret;
            }
            set
            {
                if( value != null )
                {
                    if( value.TableName != TargetTableName )
                    {
                        throw new CswDniException( ErrorType.Error, "Invalid reference", "CswNbtNodePropRelationship.RelatedNodeId requires a primary key from tablename '" + TargetTableName + "' but got one from tablename '" + value.TableName + "' instead." );
                    }
                    if( UnitId != value )
                    {
                        _CswNbtNodePropData.SetPropRowValue( _UnitIdSubField.Column, value.PrimaryKey );
                        CswNbtNode RelatedNode = _CswNbtResources.Nodes[value];
                        if( null != RelatedNode )
                        {
                            CachedUnitName = RelatedNode.NodeName;
                        }
                    }
                }
                else
                {
                    if( Required && false == _AllowSetNull )
                    {
                        throw new CswDniException( ErrorType.Warning, "Cannot save a Quantity without a Unit if the Property is required.", "Attempted to save a Quantity with an invalid UnitId." );
                    }
                    _CswNbtNodePropData.SetPropRowValue( _UnitIdSubField.Column, Int32.MinValue );
                }

                if( WasModified )
                {
                    PendingUpdate = true;
                }
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }



        #endregion

        #region Relationship-esque Helper Functions

        public CswNbtView View
        {
            set
            {
                _View = value;
                if( _View.ViewId != value.ViewId )
                {
                    _View.LoadJson( value.ToJson() );
                }
                _View.save();
            }
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.ViewId.isSet() && _View == null )
                    _View = _CswNbtResources.ViewSelect.restoreView( _CswNbtMetaDataNodeTypeProp.ViewId );
                return _View;
            }
        }

        public Int32 TargetId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.FKValue;
            }
        }

        public Tristate TargetFractional
        {
            get
            {
                Tristate Fractional = Tristate.True;//We want to be able to enter a decimal value if Unit hasn't been selected yet.
                CswNbtObjClassUnitOfMeasure UnitNode = _CswNbtResources.Nodes[UnitId];
                if( UnitNode != null )
                {
                    Fractional = UnitNode.Fractional.Checked;
                }
                return Fractional;
            }
        }

        public NbtViewRelatedIdType TargetType
        {
            get
            {
                NbtViewRelatedIdType ret = NbtViewRelatedIdType.Unknown;
                try
                {
                    ret = (NbtViewRelatedIdType) _CswNbtMetaDataNodeTypeProp.FKType;
                }
                catch( Exception ex )
                {
                    if( !( ex is System.ArgumentException ) )
                        throw ( ex );
                }
                return ret;
            }

        }

        private string TargetTableName
        {
            get
            {
                string ret = "nodes";
                if( TargetId != Int32.MinValue )
                {
                    if( TargetType == NbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetId );
                        if( TargetNodeType != null )
                            ret = TargetNodeType.TableName;
                    }
                }
                return ret;
            }
        }

        public override void SyncGestalt()
        {
            string GestaltValue = _CswNbtNodePropData.GetPropRowValue( _QuantitySubField.Column ) + " " + _CswNbtNodePropData.GetPropRowValue( _UnitNameSubField.Column );
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, GestaltValue );
        }

        #endregion

        #region Serialization Methods

        // ToXml()

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_QuantitySubField.ToXmlNodeName( true )] = ( !Double.IsNaN( Quantity ) ) ? CswConvert.ToString( Quantity ) : string.Empty;

            ParentObject["minvalue"] = MinValue.ToString();
            ParentObject["maxvalue"] = MaxValue.ToString();
            ParentObject["precision"] = Precision.ToString();
            ParentObject["excludeRangeLimits"] = ExcludeRangeLimits.ToString();
            ParentObject["quantityoptional"] = QuantityOptional.ToString();

            ParentObject[_UnitIdSubField.ToXmlNodeName( true )] = default( string );
            if( UnitId != null && Int32.MinValue != UnitId.PrimaryKey )
            {
                ParentObject[_UnitIdSubField.ToXmlNodeName( true )] = UnitId.ToString();
            }
            ParentObject[_UnitNameSubField.ToXmlNodeName( true )] = CachedUnitName;

            ParentObject["nodetypeid"] = default( string );
            if( TargetType == NbtViewRelatedIdType.NodeTypeId )
            {
                ParentObject["nodetypeid"] = TargetId.ToString();
            }

            CswNbtNode RelatedNode = _CswNbtResources.Nodes[UnitId];
            ParentObject["relatednodeid"] = default( string );
            if( null != RelatedNode )
            {
                ParentObject["relatednodeid"] = RelatedNode.NodeId.ToString();
                ParentObject["relatednodelink"] = RelatedNode.NodeLink;
                ParentObject["nodeid"] = RelatedNode.NodeId.ToString();
            }

            ParentObject["fractional"] = TargetFractional.ToString().ToLower();

            if( false == ReadOnly )
            {
                JArray JOptions = new JArray();
                ParentObject["options"] = JOptions;

                foreach( CswNbtNode Node in UnitNodes )
                {
                    JObject JOption = new JObject();
                    if( Node.NodeId != null && Node.NodeId.PrimaryKey != Int32.MinValue )
                    {
                        JOption["id"] = Node.NodeId.ToString();
                        JOption["value"] = Node.NodeName;
                        JOption["fractional"] = Node.Properties[CswNbtObjClassUnitOfMeasure.PropertyName.Fractional].AsLogical.Checked.ToString().ToLower();

                        //Case 29098 - when adding a node, the picklist for UoM has the first item selected, but the selected nodeid is not set - if this is the case, set the related nodeid to the first item
                        if( null == RelatedNode )
                        {
                            RelatedNode = Node;
                            ParentObject["relatednodeid"] = RelatedNode.NodeId.ToString();
                            ParentObject["relatednodelink"] = RelatedNode.NodeLink;
                            ParentObject["nodeid"] = RelatedNode.NodeId.ToString();
                        }
                    }
                    else if( false == Required )
                    {
                        JOption["id"] = "";
                        JOption["value"] = "";
                        JOption["fractional"] = "";
                    }
                    JOptions.Add( JOption );
                }
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string StringVal = CswTools.XmlRealAttributeName( PropRow[_QuantitySubField.ToXmlNodeName()].ToString() );
            if( CswTools.IsFloat( StringVal ) )
                Quantity = Convert.ToDouble( StringVal );
            CachedUnitName = CswTools.XmlRealAttributeName( PropRow[_UnitNameSubField.ToXmlNodeName()].ToString() );

            string NodeId = CswTools.XmlRealAttributeName( PropRow[_UnitIdSubField.ToXmlNodeName()].ToString() );
            if( NodeMap != null && NodeMap.ContainsKey( NodeId.ToLower() ) )
                UnitId = new CswPrimaryKey( "nodes", NodeMap[NodeId.ToLower()] );
            else if( CswTools.IsInteger( NodeId ) )
                UnitId = new CswPrimaryKey( "nodes", CswConvert.ToInt32( NodeId ) );
            else
                UnitId = null;

            if( null != UnitId )
            {
                PendingUpdate = true;
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_QuantitySubField.ToXmlNodeName( true )] )
            {
                Quantity = CswConvert.ToDouble( JObject[_QuantitySubField.ToXmlNodeName( true )].ToString() );
            }
            if( null != JObject[_UnitNameSubField.ToXmlNodeName( true )] )
            {
                CachedUnitName = JObject[_UnitNameSubField.ToXmlNodeName( true )].ToString();
            }

            if( null != JObject[_UnitIdSubField.ToXmlNodeName( true )] )
            {
                string NodePkString = JObject[_UnitIdSubField.ToXmlNodeName( true )].ToString();
                CswPrimaryKey thisUnitId = new CswPrimaryKey();
                bool validPk = thisUnitId.FromString( NodePkString );
                if( false == validPk )
                {
                    thisUnitId.TableName = "nodes";
                    thisUnitId.PrimaryKey = CswConvert.ToInt32( NodePkString );
                }
                if( CswTools.IsPrimaryKey( thisUnitId ) )
                {
                    if( NodeMap != null && NodeMap.ContainsKey( thisUnitId.PrimaryKey ) )
                    {
                        thisUnitId.PrimaryKey = NodeMap[thisUnitId.PrimaryKey];
                    }
                    UnitId = thisUnitId;
                    JObject["destnodeid"] = UnitId.PrimaryKey.ToString();
                    //PendingUpdate = true;
                }
                else
                {
                    UnitId = null;
                }
            }
        }

        #endregion

    }//CswNbtNodePropQuantity

}//namespace 
