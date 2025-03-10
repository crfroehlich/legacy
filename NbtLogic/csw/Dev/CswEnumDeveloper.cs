﻿using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.csw.Dev
{
    /// <summary>
    /// Template for new CswDeveloper class
    /// </summary>
    public sealed class CswEnumDeveloper : IEquatable<CswEnumDeveloper>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
                                                                   {
                                                                       { NBT, NBT },
                                                                       { AE, AE },
                                                                       { BV, BV },
                                                                       { CF, CF },
                                                                       { CM, CM },
                                                                       { DH, DH },
                                                                       { MB, MB },
                                                                       { MO, MO },
                                                                       { PG, PG },
                                                                       { SS, SS }
                                                                   };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public CswEnumDeveloper( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumDeveloper( string Val )
        {
            return new CswEnumDeveloper( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( CswEnumDeveloper item )
        {
            return item.Value;
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion Internals

        #region Enum members

        /// <summary> NBT (System Script) </summary>
        public const string NBT = "NBT (System Script)";

        /// <summary> Adeesha </summary>
        public const string AE = "Adeesha Ekanayake";
        /// <summary> Brendan </summary>
        public const string BV = "Brendan Vavra";
        /// <summary> Christopher </summary>
        public const string CF = "Christopher Froehlich";
        /// <summary> Colleen </summary>
        public const string CM = "Colleen Muldowney";
        /// <summary> David </summary>
        public const string DH = "The Honey Badger";
        /// <summary> Matt </summary>
        public const string MB = "Matt Bischoff";
        /// <summary> Matt O </summary>
        public const string MO = "Matt Obetz";
        /// <summary> Phil </summary>
        public const string PG = "Phil Glaser";
        /// <summary> Steve </summary>
        public const string SS = "Steve Salter";

        #endregion Enum members

        #region IEquatable (CswDeveloper)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumDeveloper ft1, CswEnumDeveloper ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumDeveloper ft1, CswEnumDeveloper ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumDeveloper ) )
            {
                return false;
            }
            return this == (CswEnumDeveloper) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumDeveloper obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (CswDeveloper)

    };
}
