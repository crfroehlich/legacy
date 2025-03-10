﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Search
{
    [DataContract]
    public class CswNbtSearchFilter : IEquatable<CswNbtSearchFilter>
    {
        public const string BlankValue = "[blank]";

        public CswNbtSearchFilter( JObject FilterObj )
        {
            FromJObject( FilterObj );
        }
        public CswNbtSearchFilter( string inFilterName, CswEnumNbtSearchFilterType inType, string inFilterId, string inFilterValue, Int32 inCount, string inIcon, bool inRemoveable, CswEnumNbtSearchPropOrderSourceType inSource )
        {
            FilterName = inFilterName;
            Type = inType;
            FilterId = inFilterId;
            FilterValue = inFilterValue;
            Count = inCount;
            Icon = inIcon;
            Removeable = inRemoveable;
            Source = inSource;
        }

        [DataMember]
        public string FilterName;
        [DataMember]
        public CswEnumNbtSearchFilterType Type;
        [DataMember]
        public string FilterId;
        [DataMember]
        public Int32 Count;
        [DataMember]
        public string Icon;
        [DataMember]
        public bool Removeable;
        [DataMember]
        public CswEnumNbtSearchPropOrderSourceType Source;
        [DataMember]
        public string FilterValue;

        // for NodeType Filters
        [DataMember]
        public Int32 FirstVersionId;

        // for Object Class Filters
        [DataMember]
        public Int32 ObjectClassId;

        // for PropertySet Filters
        [DataMember]
        public Int32 PropertySetId;

        // for PropVal Filters
        [DataMember]
        public Int32 FirstPropVersionId;
        
        [DataMember]
        public bool UseMoreLink = true;

        #region Serialization

        public JObject ToJObject()
        {
            JObject FilterObj = new JObject();
            FilterObj["filtername"] = FilterName;
            FilterObj["filtertype"] = Type.ToString();
            FilterObj["filterid"] = FilterId;
            FilterObj["count"] = Count;
            FilterObj["icon"] = Icon;
            FilterObj["removeable"] = Removeable;
            FilterObj["usemorelink"] = UseMoreLink;
            FilterObj["source"] = Source.ToString();

            if( FilterValue != string.Empty )
            {
                FilterObj["filtervalue"] = FilterValue;
            }
            else
            {
                FilterObj["filtervalue"] = BlankValue;
            }

            if( Int32.MinValue != FirstVersionId )
            {
                FilterObj["firstversionid"] = FirstVersionId;
            }
            if( Int32.MinValue != FirstPropVersionId )
            {
                FilterObj["firstpropversionid"] = FirstPropVersionId;
            }
            if( Int32.MinValue != ObjectClassId )
            {
                FilterObj["objectclassid"] = ObjectClassId;
            }
            if( Int32.MinValue != PropertySetId )
            {
                FilterObj["propertysetid"] = PropertySetId;
            }

            return FilterObj;
        } // ToJObject()

        public JObject FromJObject( JObject FilterObj )
        {
            FilterName = FilterObj["filtername"].ToString();
            Type = (CswEnumNbtSearchFilterType) FilterObj["filtertype"].ToString();
            FilterId = FilterObj["filterid"].ToString();
            Count = CswConvert.ToInt32( FilterObj["count"] );
            Icon = FilterObj["icon"].ToString();
            Removeable = CswConvert.ToBoolean( FilterObj["removeable"] );
            Source = (CswEnumNbtSearchPropOrderSourceType) FilterObj["source"].ToString();
            FilterValue = FilterObj["filtervalue"].ToString();
            FirstVersionId = CswConvert.ToInt32( FilterObj["firstversionid"] );
            FirstPropVersionId = CswConvert.ToInt32( FilterObj["firstpropversionid"] );
            ObjectClassId = CswConvert.ToInt32( FilterObj["objectclassid"] );
            PropertySetId = CswConvert.ToInt32( FilterObj["propertysetid"] );
            return FilterObj;
        } // FromJObject()

        #endregion Serialization

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtSearchFilter w1, CswNbtSearchFilter w2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( w1, w2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) w1 == null ) || ( (object) w2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( w1.Type == w2.Type &&
                w1.FilterId == w2.FilterId )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtSearchFilter w1, CswNbtSearchFilter w2 )
        {
            return !( w1 == w2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtSearchFilter ) )
                return false;
            return this == (CswNbtSearchFilter) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtSearchFilter obj )
        {
            return this == (CswNbtSearchFilter) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion IEquatable


    } // class CswNbtSearchFilterWrapper
} // namespace ChemSW.Nbt.Search

