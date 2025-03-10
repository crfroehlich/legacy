﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    public abstract class CswUpdateSchemaTo
    {
        public virtual string Title { get { return "Script: Case " + CaseNo; } }

        public class UnitOfBlame
        {
            public UnitOfBlame()
            {

            }

            public UnitOfBlame( CswEnumDeveloper Dev, Int32 Case )
            {
                Developer = Dev;
                CaseNumber = Case;
            }

            public CswEnumDeveloper Developer;
            public Int32 CaseNumber;
        }

        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        //public abstract CswSchemaVersion SchemaVersion { get; }
        //public abstract string Description { set; get; }

        private string _Description = string.Empty;

        public virtual string Description
        {
            set
            {
                _Description = value;
            }
            get
            {
                string Ret = _Description;
                if( string.IsNullOrEmpty( Ret ) ||
                    ( CaseNo > 0 &&
                      false == Ret.Contains( "Case " + CaseNo ) &&
                      false == Ret.Contains( Title ) ) )
                {
                    if( false == string.IsNullOrEmpty( Ret ) )
                    {
                        Ret += " - ";
                    }
                    Ret += Title;
                }
                return Ret;
            }
        }

        /// <summary>
        /// The logic to execute in each Schema Script
        /// </summary>
        public abstract void update();

        private UnitOfBlame _Blame = null;
        public virtual UnitOfBlame Blame
        {
            get
            {
                UnitOfBlame Ret;
                if( null != _Blame )
                {
                    Ret = _Blame;
                }
                else
                {
                    Ret = new UnitOfBlame( Author, CaseNo );
                }
                return Ret;
            }
            set { _Blame = value; }
        }

        /// <summary>
        /// The author of the script
        /// </summary>
        public abstract CswEnumDeveloper Author { get; }

        /// <summary>
        /// The FogBugz Case number associated with this script
        /// </summary>
        public abstract Int32 CaseNo { get; }

        /// <summary>
        /// A unique identifier for this script
        /// Format: <Release #><Release Letter>_Case<Case #>
        /// </summary>
        private string _ReleaseIteration = CswTools.PadInt( CswSchemaScriptsProd.CurrentReleaseIteration, 2 );
        private char _ReleaseIdentifier = CswSchemaScriptsProd.CurrentReleaseIdentifier;

        public virtual string AppendToScriptName()
        {
            return string.Empty;
        }

        public string ScriptName
        {
            get
            {
                string StringToAppend = AppendToScriptName().Replace( "'", "" );

                // Run always scripts don't have a case number, but they should have a scriptname
                if( CaseNo == 0 && false == string.IsNullOrEmpty( StringToAppend ) )
                {
                    return StringToAppend;
                }

                string Ret = _ReleaseIteration + _ReleaseIdentifier + "_Case" + CaseNo;
                Ret = Ret + StringToAppend;
                return Ret;
            }
        }

        /// <summary>
        /// Whether the script should be run always
        /// </summary>
        public virtual bool AlwaysRun
        {
            get { return false; }
        }

        /// <summary>
        /// Get the Case number as a link to FogBugz
        /// </summary>
        public string getCaseLink()
        {
            string Ret = string.Empty;
            if( CaseNo > 0 )
            {
                Ret = @"<a href=""https://fogbugz.chemswlive.com/default.asp?" + CaseNo + @""">Case " + CaseNo + "</a>";
            }
            else
            {
                Ret = "No case link for case " + CaseNo;
            }
            return Ret;
        }
    }
}
