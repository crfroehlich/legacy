using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropBarcode : CswNbtNodeProp
    {
        public const string AutoSignal = "[auto]";

        public static implicit operator CswNbtNodePropBarcode( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsBarcode;
        }

        public CswNbtNodePropBarcode( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _BarcodeSubField = ( (CswNbtFieldTypeRuleBarCode) _FieldTypeRule ).BarcodeSubField;
            _SequenceNumberSubField = ( (CswNbtFieldTypeRuleBarCode) _FieldTypeRule ).SequenceNumberSubField;
            _Sequence = CswNbtMetaDataNodeTypeProp.Sequence;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _BarcodeSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => Barcode, x => setBarcodeValueOverride( CswConvert.ToString( x ), true ) ) );
            _SubFieldMethods.Add( _SequenceNumberSubField.Name, new Tuple<Func<dynamic>, Action<dynamic>>( () => SequenceNumber, null ) );
        } //CswNbtNodePropBarcode()

        private CswNbtSubField _BarcodeSubField;
        private CswNbtSubField _SequenceNumberSubField;
        private CswNbtObjClassDesignSequence _Sequence;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }//Empty

        public string Barcode
        {
            get
            {
                return GetPropRowValue( _BarcodeSubField );
            }
        }//Barcode

        public Int32 SequenceNumber
        {
            get
            {
                return CswConvert.ToInt32( GetPropRowValue( _SequenceNumberSubField ) );
            }
        }//SequenceNumber

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        /// <summary>
        /// Sets Barcode to the next sequence value
        /// </summary>
        public bool setBarcodeValue( bool OverrideExisting = false )
        {
            bool Succeeded = false;
            if( ( Barcode.Trim() == string.Empty || OverrideExisting ) && null != _Sequence )
            {
                string value = _Sequence.getNext();
                Succeeded = setBarcodeValueOverride( value, false );
            }
            return Succeeded;
        }

        /// <summary>
        /// Resets SequenceNumber to the numeric portion of the Barcode
        /// </summary>
        public void resetSequenceNumber()
        {
            // Fix missing sequence number
            if( null != _Sequence )
            {
                Int32 ThisSeqValue = _Sequence.deformatSequence( Barcode );
                SetPropRowValue( _SequenceNumberSubField, ThisSeqValue );
            }
        }

        /// <summary>
        /// Sets Barcode to the provided value.  
        /// This allows manually overriding automatically generated sequence values.  Use carefully.
        /// </summary>
        /// <param name="SeqValue">Value to set for Barcode</param>
        /// <param name="ResetSequence">True if the sequence needs to be reset to this value 
        /// (set true if the value was not just generated from the sequence)</param>
        public bool setBarcodeValueOverride( string SeqValue, bool ResetSequence )
        {
            bool Succeeded = SetPropRowValue( _BarcodeSubField, SeqValue );
            if( null != _Sequence )
            {
                Int32 ThisSeqValue = _Sequence.deformatSequence( SeqValue );
                Succeeded = ( Succeeded && SetPropRowValue( _SequenceNumberSubField, ThisSeqValue ) );
                Gestalt = SeqValue;
                if( ResetSequence )
                {
                    // Keep the sequence up to date
                    _Sequence.reSync( CswNbtFieldTypeRuleBarCode.SequenceNumberColumn, ThisSeqValue );
                }
            }
            return Succeeded;
        }

        override public void onBeforeUpdateNodePropRowLogic()
        {
            if( false == _Node.IsTemp )
            {
                setBarcodeValue();
            }
        }//onBeforeUpdateNodePropRow()

        public override void Copy( CswNbtNodePropData Source )
        {
            if( false == _Node.IsTemp )
            {
                setBarcodeValue();
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_SequenceNumberSubField.ToXmlNodeName( true )] = SequenceNumber;
            ParentObject[_BarcodeSubField.ToXmlNodeName( true )] = Barcode;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            _setPropVals( CswTools.XmlRealAttributeName( PropRow[_BarcodeSubField.ToXmlNodeName()].ToString() ) );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_BarcodeSubField.ToXmlNodeName( true )] )
            {
                _setPropVals( JObject[_BarcodeSubField.ToXmlNodeName( true )].ToString() );
            }
        }

        private void _setPropVals( string ProspectiveBarcode )
        {
            if( ProspectiveBarcode != string.Empty )
            {
                setBarcodeValueOverride( ProspectiveBarcode, false );
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Barcode );
        }

        //public override bool onBeforeSetDefault()
        //{
        //    return HasDefaultValue( false ) && getDefaultValue( false, false ).AsBarcode.Barcode != AutoSignal;
        //}

    }//CswNbtNodePropQuantity

}//namespace 
