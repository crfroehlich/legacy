﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd
    {
        public static int CurrentReleaseIteration;
        public static char CurrentReleaseIdentifier;

        private CswNbtResources _CswNbtResources = null;

        public CswSchemaScriptsProd( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion( 2, 'L', 42 );

            // Set the global variable values
            CurrentReleaseIdentifier = _getNextReleaseIdentifier();
            CurrentReleaseIteration = _getNextSuperCycle( CurrentReleaseIdentifier );

            #region Populate Scripts

            Collection<ICswSchemaScripts> AllScripts = new Collection<ICswSchemaScripts>()
                {
                    new CswSchemaScriptsMagnolia()
                };

            // DDL
            foreach( ICswSchemaScripts ScriptColl in AllScripts )
            {
                foreach( CswUpdateSchemaTo Script in ScriptColl._DDLScripts() )
                {
                    _addVersionedScript( Script );
                }
            }
            // MetaData
            foreach( ICswSchemaScripts ScriptColl in AllScripts )
            {
                foreach( CswUpdateSchemaTo Script in ScriptColl._MetaDataScripts() )
                {
                    _addVersionedScript( Script );
                }
            }

            // This is the MakeMissingNodeTypeProps script. If you have a script which contains OC changes, put it before this script.
            _addVersionedScript( new RunAlways_MakeMissingNodeTypePropsProps() );

            // Schema
            foreach( ICswSchemaScripts ScriptColl in AllScripts )
            {
                foreach( CswUpdateSchemaTo Script in ScriptColl._SchemaScripts() )
                {
                    _addVersionedScript( Script );
                }
            }

            #endregion Populate Scripts

            #region Calculate the Latest Version

            _setLatestVersion( CswNbtResources );

            #endregion Calculate the Latest Version

            #region Before Scripts
            // Before scripts that always run.
            _addRunBeforeScript( new RunBeforeEveryExecutionOfUpdater_02SQL() );
            #endregion Before Scripts

            #region After Script
            // After scripts that always run.
            _addRunAfterScript( new RunAfterEveryExecutionOfUpdater_01() );
            _addRunAfterScript( new RunAfterEveryExecutionOfUpdater_02AuditSql() );
            _addRunAfterScript( new RunAfterEveryExecutionOfUpdater_DBViews() );
            #endregion After Script

        }//ctor

        private void _setLatestVersion( CswNbtResources CswNbtResources )
        {
            _LatestVersion = _MinimumVersion;

            if( CurrentVersion( CswNbtResources ) != _MinimumVersion )
            {
                _LatestVersion = CurrentVersion( CswNbtResources );
            }

            foreach( KeyValuePair<CswSchemaVersion, CswSchemaUpdateDriver> Pair in _UpdateDrivers.Where( Pair => false == Pair.Value.AlwaysRun ).Where( Pair => _LatestVersion == _MinimumVersion ||
                                                                                                                    ( _LatestVersion.ReleaseIteration < Pair.Key.ReleaseIteration ) ) )
            {
                _LatestVersion = Pair.Key;
            }

        }//_setLatestVersion()

        #region ICswSchemaScripts

        private CswSchemaVersion _LatestVersion = null;
        public CswSchemaVersion LatestVersion
        {
            get { return ( _LatestVersion ); }
        }

        private CswSchemaVersion _MinimumVersion = null;
        public CswSchemaVersion MinimumVersion
        {
            get { return ( _MinimumVersion ); }
        }

        public CswSchemaVersion CurrentVersion( CswNbtResources CswNbtResources )
        {
            return ( new CswSchemaVersion( CswNbtResources.ConfigVbls.getConfigVariableValue( "schemaversion" ) ) );
        }

        public CswSchemaVersion TargetVersion( CswNbtResources CswNbtResources )
        {
            CswSchemaVersion ret = null;
            CswSchemaVersion myCurrentVersion = CurrentVersion( CswNbtResources );
            if( myCurrentVersion == MinimumVersion )
                ret = new CswSchemaVersion( LatestVersion.CycleIteration, LatestVersion.ReleaseIdentifier, 1 );
            else
                ret = new CswSchemaVersion( myCurrentVersion.CycleIteration, myCurrentVersion.ReleaseIdentifier, myCurrentVersion.ReleaseIteration + 1 );
            return ret;
        }

        private Int32 _CurrentIdx = Int32.MinValue;
        public CswSchemaUpdateDriver Next( CswNbtResources CswNbtResources )
        {
            CswSchemaUpdateDriver ReturnVal = null;

            if( _UpdateDrivers.Count > ( _CurrentIdx + 1 ) )
            {
                if( Int32.MinValue == _CurrentIdx )
                {
                    _CurrentIdx = 0;
                }
                else
                {
                    _CurrentIdx++;
                }

                KeyValuePair<CswSchemaVersion, CswSchemaUpdateDriver> CurrentItem = _UpdateDrivers.ElementAt( _CurrentIdx );
                ReturnVal = CurrentItem.Value;
            }

            return ReturnVal;
        }//Next()

        public CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion]
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;

                if( _UpdateDrivers.ContainsKey( CswSchemaVersion ) )
                {
                    ReturnVal = _UpdateDrivers[CswSchemaVersion];
                }

                return ( ReturnVal );
            }
        }

        public void stampSchemaVersion( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswNbtResources.ConfigVbls.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }

        #endregion

        #region Versioned scripts

        private CswSchemaVersion _makeNextSchemaVersion()
        {
            char ReleaseIdentifier = _getNextReleaseIdentifier();
            int SuperCycle = _getNextSuperCycle( ReleaseIdentifier );
            int ReleaseIteration = _getReleaseIteration( _CswNbtResources );

            return ( new CswSchemaVersion( SuperCycle, ReleaseIdentifier, _getCountOfRunOnceScripts( _UpdateDrivers ) + ReleaseIteration ) );
        }//_makeNextSchemaVersion()

        private int _getReleaseIteration( CswNbtResources CswNbtResources )
        {
            int Return = 0;

            if( CurrentVersion( CswNbtResources ) == _MinimumVersion )
            {
                Return = 1;
            }
            else
            {
                Return = CurrentVersion( CswNbtResources ).ReleaseIteration + 1;
            }

            return Return;
        }//_getReleaseIteration()

        private char _getNextReleaseIdentifier()
        {
            char Return = _MinimumVersion.ReleaseIdentifier;
            if( 'Y' != Return )
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWY".ToCharArray(); //No X or Z
                List<char> Chars = new List<char>( alpha );
                int ReleaseIdInt = Chars.IndexOf( Return );
                ReleaseIdInt++;
                Return = Chars[ReleaseIdInt];
            }
            else
            {
                Return = 'A';
            }

            return Return;
        }//_getNextReleaseIdentifier()

        private int _getNextSuperCycle( char ReleaseIdentifier )
        {
            int Return = _MinimumVersion.CycleIteration;
            if( 'Y' == ReleaseIdentifier )
            {
                Return = _MinimumVersion.CycleIteration + 1;
            }
            return Return;
        }//_getNextSuperCycle()

        private Int32 _getCountOfRunOnceScripts( Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> Dictionary )
        {
            return Dictionary.Count( Pair => false == Pair.Value.AlwaysRun );
        }

        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        private void _addVersionedScript( CswUpdateSchemaTo UpdateTo )
        {
            // Instance the UpdateDriver
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );

            CswSchemaUpdateDriver.CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( _CswNbtResources );
            if( false == CswSchemaUpdateDriver.AlreadyRun() || CswSchemaUpdateDriver.AlwaysRun )
            {
                if( false == CswSchemaUpdateDriver.AlwaysRun )
                {
                    CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
                    CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.ScriptName;
                    if( false == _isDuplicateScript( CswSchemaUpdateDriver ) )
                    {
                        _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );
                    }
                }
                else
                {
                    CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', 0 );
                    CswSchemaUpdateDriver.Description = "Run Always Script: " + CswSchemaUpdateDriver.ScriptName;
                    _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );
                }
            }
        }

        /// <summary>
        /// Returns true if a script with the same ScriptName was already added to _UpdateDrivers
        /// </summary>
        /// <param name="CswSchemaUpdateDriver"></param>
        /// <returns></returns>
        private bool _isDuplicateScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            return _UpdateDrivers.Values.Any( UpdateDriver => UpdateDriver.ScriptName == CswSchemaUpdateDriver.ScriptName );
        }

        #endregion

        #region Run-always scripts

        private List<CswSchemaUpdateDriver> _RunBeforeScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunBeforeScripts
        {
            get
            {
                return ( _RunBeforeScripts );
            }
        }

        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return ( _RunAfterScripts );
            }

        }

        private void _addRunBeforeScript( CswUpdateSchemaTo UpdateTo, string Description = null )
        {
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description += Description ?? string.Empty;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
            }
        }

        private void _addRunAfterScript( CswUpdateSchemaTo UpdateTo, string Description = null )
        {
            CswSchemaUpdateDriver CswSchemaUpdateDriver = new CswSchemaUpdateDriver( UpdateTo );
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 99, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description += Description ?? string.Empty;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }
        }

        #endregion

    }//CswScriptCollections
}//ChemSW.Nbt.Schema