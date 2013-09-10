using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropTimeInterval : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropTimeInterval( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsTimeInterval;
        }

        public CswNbtNodePropTimeInterval( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            //_RateInterval = new CswRateInterval(CswNbtNodePropData.Gestalt);   //this should be backwards compatible...
            if( CswNbtNodePropData.ClobData.ToString() != string.Empty )
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml( CswNbtNodePropData.ClobData.ToString() );
                _RateInterval = new CswRateInterval( _CswNbtResources, XmlDoc.FirstChild );
            }
            else
            {
                _RateInterval = new CswRateInterval( _CswNbtResources );
            }
            _IntervalSubField = ( (CswNbtFieldTypeRuleTimeInterval) _FieldTypeRule ).IntervalSubField;
            _StartDateSubField = ( (CswNbtFieldTypeRuleTimeInterval) _FieldTypeRule ).StartDateSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _IntervalSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => RateInterval, x => RateInterval.ReadJson( CswConvert.ToJObject( x ) ) ) );
            _SubFieldMethods.Add( _StartDateSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => getStartDate(), null ) );
        }

        private CswNbtSubField _IntervalSubField;
        private CswNbtSubField _StartDateSubField;

        override public bool Empty
        {
            get
            {
                return ( string.IsNullOrEmpty( Gestalt ) );
            }
        }
        
        private CswRateInterval _RateInterval;
        public CswRateInterval RateInterval
        {
            get
            {
                return _RateInterval;
            }
            set
            {
                _RateInterval = value;
                SetPropRowValue( _IntervalSubField.Column, value.ToString() );
                SetPropRowValue( _StartDateSubField.Column, value.getFirst() );
                Gestalt = value.ToString();
                ClobData = value.ToXmlString();
            }
        }


        public DateTime getNextOccuranceAfter( DateTime AfterDateTime )
        {
            return _RateInterval.getNext( AfterDateTime );
        }//getNextOccuranceAfter()

        public DateTime getLastOccuranceBefore( DateTime BeforeDateTime )
        {
            return _RateInterval.getPrevious( BeforeDateTime );
        }//getLastOccuranceBefore()


        public DateTime getStartDate()
        {
            return _RateInterval.getFirst();
        }//getStartDate()

        public Int32 getMaximumWarningDays()
        {
            return _RateInterval.getMaximumWarningDays();
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }



        //private string _ElemName_Rateinterval = "Rateinterval";

        public override void ToJSON( JObject ParentObject )
        {
            JObject IntervalObj = new JObject();
            ParentObject[_IntervalSubField.ToXmlNodeName()] = IntervalObj;
            //IntervalObj["text"] = RateInterval.ToString();
            RateInterval.ToJson( IntervalObj );
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //RateInterval = new CswRateInterval( PropRow[_IntervalSubField.ToXmlNodeName()].ToString() );
            if( PropRow.Table.Columns.Contains( _IntervalSubField.ToXmlNodeName() ) )
            {
                string IntervalXmlAsString = CswTools.XmlRealAttributeName( PropRow[_IntervalSubField.ToXmlNodeName()].ToString() );
                XmlDocument Doc = new XmlDocument();
                XmlNode IntervalNode = CswXmlDocument.SetDocumentElement( Doc, _IntervalSubField.ToXmlNodeName() );
                IntervalNode.InnerXml = IntervalXmlAsString.Trim();

                CswRateInterval NewRateInterval = new CswRateInterval( _CswNbtResources );
                NewRateInterval.ReadXml( IntervalNode );
                // Setting RateInterval triggers the change to the property value -- don't skip this step
                RateInterval = NewRateInterval;
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            CswRateInterval NewRateInterval = new CswRateInterval( _CswNbtResources );
            NewRateInterval.ReadJson( (JObject) JObject[_IntervalSubField.ToXmlNodeName()] );
            // Setting RateInterval triggers the change to the property value -- don't skip this step
            RateInterval = NewRateInterval;
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtPropColumn.Gestalt, RateInterval.ToString() );
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
