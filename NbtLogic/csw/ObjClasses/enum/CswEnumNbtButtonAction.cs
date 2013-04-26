using System;
using System.Collections.Generic;
using System.Diagnostics;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Button Actions
    /// </summary>
    public sealed class CswEnumNbtButtonAction : CswEnum<CswEnumNbtButtonAction>
    {
        private CswEnumNbtButtonAction( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtButtonAction> _All { get { return All; } }
        public static implicit operator CswEnumNbtButtonAction( string str )
        {
            CswEnumNbtButtonAction ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtButtonAction Unknown = new CswEnumNbtButtonAction( "Unknown" );

        public static readonly CswEnumNbtButtonAction editprop = new CswEnumNbtButtonAction( "editprop" );
        public static readonly CswEnumNbtButtonAction creatematerial = new CswEnumNbtButtonAction( "creatematerial" );
        public static readonly CswEnumNbtButtonAction dispense = new CswEnumNbtButtonAction( "dispense" );
        public static readonly CswEnumNbtButtonAction move = new CswEnumNbtButtonAction( "move" );
        public static readonly CswEnumNbtButtonAction reauthenticate = new CswEnumNbtButtonAction( "reauthenticate" );
        public static readonly CswEnumNbtButtonAction refresh = new CswEnumNbtButtonAction( "refresh" );
        public static readonly CswEnumNbtButtonAction receive = new CswEnumNbtButtonAction( "receive" );
        public static readonly CswEnumNbtButtonAction request = new CswEnumNbtButtonAction( "request" );
        public static readonly CswEnumNbtButtonAction popup = new CswEnumNbtButtonAction( "popup" );
        public static readonly CswEnumNbtButtonAction landingpage = new CswEnumNbtButtonAction( "landingpage" );
        public static readonly CswEnumNbtButtonAction loadView = new CswEnumNbtButtonAction( "loadview" );
        public static readonly CswEnumNbtButtonAction nothing = new CswEnumNbtButtonAction( "nothing" );
        public static readonly CswEnumNbtButtonAction griddialog = new CswEnumNbtButtonAction( "griddialog" );
        public static readonly CswEnumNbtButtonAction assignivglocation = new CswEnumNbtButtonAction( "assignivglocation" );
    }

}//namespace ChemSW.Nbt.ObjClasses