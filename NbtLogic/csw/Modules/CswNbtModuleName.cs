using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module
    /// </summary>
    public sealed class CswNbtModuleName : CswEnum<CswNbtModuleName>
    {
        private CswNbtModuleName( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtModuleName> _All { get { return All; } }
        public static implicit operator CswNbtModuleName( string str )
        {
            CswNbtModuleName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswNbtModuleName Unknown = new CswNbtModuleName( "Unknown" );

        /// <summary>
        /// BioSafety
        /// </summary>
        public static readonly CswNbtModuleName BioSafety = new CswNbtModuleName( "BioSafety" );
        /// <summary>
        /// Control Chart Pro
        /// </summary> 
        public static readonly CswNbtModuleName CCPro = new CswNbtModuleName( "CCPro" );
        /// <summary>
        /// Chemical Inventory
        /// </summary>
        public static readonly CswNbtModuleName CISPro = new CswNbtModuleName( "CISPro" );
        /// <summary>
        /// Mobile
        /// </summary>
        public static readonly CswNbtModuleName Mobile = new CswNbtModuleName( "Mobile" );
        /// <summary>
        /// Instrument Maintenance and Calibration
        /// </summary>
        public static readonly CswNbtModuleName IMCS = new CswNbtModuleName( "IMCS" );
        /// <summary>
        /// NBT Management Application
        /// </summary>
        public static readonly CswNbtModuleName NBTManager = new CswNbtModuleName( "NBTManager" );
        /// <summary>
        /// Site Inspection
        /// </summary>
        public static readonly CswNbtModuleName SI = new CswNbtModuleName( "SI" );
        /// <summary>
        /// Sample Tracking
        /// </summary>
        public static readonly CswNbtModuleName STIS = new CswNbtModuleName( "STIS" );
    
    } // class CswNbtModule
}// namespace ChemSW.Nbt