﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.DB;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd : ICswSchemaScripts
    {

        public CswSchemaScriptsProd( CswNbtResources CswNbtResources )
        {
            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion( 2, 'D', 23 );

            #region Eucalyptus

            //// EUCALYPTUS 'Before' Scripts
            //_addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case29700() ) );       //02E-001
            //_addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case30123() ) );       //02E-002
            //_addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case29701() ) );       //02E-003
            //_addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02E_Case30347() ) );       //02E-004

            //// EUCALYPTUS Scripts
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30014() ) );                        //02E-008
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30222() ) );                        //02E-009
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case29847() ) );                        //02E-010
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30123() ) );                        //02E-011
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30370() ) );                        //02E-012
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30360() ) );                        //02E-013
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30339_UserProfilex2() ) );          //02E-014
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30300() ) );                        //02E-015
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30440() ) );                        //02E-016
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30445() ) );                        //02E-017
            //_addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02E_Case30360() ) );                        //02E-018



            #endregion
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case27883() ) );                    //02E-021 //02F-010

            // FOXGLOVE 'Before' Scripts
            _addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30281() ) );           //02F-001 
            _addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30251() ) );           //02F-002 
            _addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30251B() ) );          //02F-003
            _addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case30082_UserCache() ) ); //02F-004

            // This is the MakeMissingNodeTypeProps script. If you have a script which contains OC changes, put it before this script.
            _addVersionedScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps() ) );

            // FOXGLOVE Scripts
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30281() ) );                            //02F-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case28998() ) );                            //02F-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29973() ) );                            //02F-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29191() ) );                            //02F-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29542() ) );                            //02F-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case29438() ) );                            //02F-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30082_UserCache() ) );                  //02F-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30197() ) );                            //02F-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02F_Case30417() ) );                            //02F-013


            #region Calculate the Latest Version
            _LatestVersion = _MinimumVersion;
            if( CurrentVersion( CswNbtResources ) == _MinimumVersion )
            {
                // Then we count because no scripts have been run
                foreach( CswSchemaVersion Version in _UpdateDrivers.Keys )
                {
                    if( _LatestVersion == _MinimumVersion || ( _LatestVersion.CycleIteration == Version.CycleIteration && _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier && _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) )
                    {
                        if( false == Version.ToString().Contains( "#" ) )
                        {
                            _LatestVersion = Version;
                        }
                    }
                }
            }
            else
            {
                // Then we go into the update_history table and count the number of scripts that have been run for this release cycle
                CswTableSelect ts = CswNbtResources.makeCswTableSelect( "GetLatestVersion", "update_history" );
                DataTable dt = ts.getTable( "where version like '" + _SuperCycle + _ReleaseIdentifier + "%" + "' and succeeded = 1" );
                if( dt.Rows.Count > 0 )
                {
                    _LatestVersion = dt.Rows.Count > _UpdateDrivers.Count ? new CswSchemaVersion( _SuperCycle, _ReleaseIdentifier, dt.Rows.Count ) : new CswSchemaVersion( _SuperCycle, _ReleaseIdentifier, dt.Rows.Count + ( _UpdateDrivers.Count - dt.Rows.Count ) );
                }
            }

            #endregion Calculate the Latest Version

            #region Before Scripts

            // We now only have two before scripts that always run
                _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02F_Case27883() ), RunBeforeEveryExecutionOfUpdater_02F_Case27883.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02SQL() ), RunBeforeEveryExecutionOfUpdater_02SQL.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_03() ), RunBeforeEveryExecutionOfUpdater_03.Title );

            #endregion Before Scripts

            #region After Script

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), RunAfterEveryExecutionOfUpdater_01.Title );

            #endregion After Script

        }//ctor

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

            if( _UpdateDriversToRun.Count > ( _CurrentIdx + 1 ) )
            {
                if( Int32.MinValue == _CurrentIdx )
                {
                    _CurrentIdx = 0;
                }
                else
                {
                    _CurrentIdx++;
                }

                KeyValuePair<CswSchemaVersion, CswSchemaUpdateDriver> CurrentItem = _UpdateDriversToRun.ElementAt( _CurrentIdx );
                ReturnVal = CurrentItem.Value;
            }

            return ReturnVal;
        }//Next()

        public CswSchemaUpdateDriver this[CswSchemaVersion CswSchemaVersion]
        {
            get
            {
                CswSchemaUpdateDriver ReturnVal = null;

                if( _UpdateDriversToRun.ContainsKey( CswSchemaVersion ) )
                {
                    ReturnVal = _UpdateDriversToRun[CswSchemaVersion];
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

        private int _SuperCycle;
        private char _ReleaseIdentifier;
        CswSchemaVersion _makeNextSchemaVersion()
        {
            int SuperCycle = _MinimumVersion.CycleIteration;
            char ReleaseIdentifier = _MinimumVersion.ReleaseIdentifier;
            if( 'Y' != ReleaseIdentifier )
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWY".ToCharArray(); //No X or Z
                List<char> Chars = new List<char>( alpha );
                int ReleaseIdInt = Chars.IndexOf( ReleaseIdentifier );
                ReleaseIdInt++;
                ReleaseIdentifier = Chars[ReleaseIdInt];
            }
            else
            {
                SuperCycle = _MinimumVersion.CycleIteration + 1;
                ReleaseIdentifier = 'A';
            }

            // Set these so we can use them when determining the LatestVersion
            _SuperCycle = SuperCycle;
            _ReleaseIdentifier = ReleaseIdentifier;
            return ( new CswSchemaVersion( SuperCycle, ReleaseIdentifier, _getCountOfRunOnceScripts( _UpdateDrivers ) + 1 ) );
        }

        private Int32 _getCountOfRunOnceScripts( Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> Dictionary )
        {
            Int32 Ret = 0;
            foreach( var Pair in Dictionary )
            {
                if( false == Pair.Key.ToString().Contains( "#" ) )
                {
                    Ret++;
                }
            }

            return Ret;
        }

        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDriversToRun = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDriversToRun { get { return _UpdateDriversToRun; } }
        public void addVersionedScriptsToRun( CswNbtResources CswNbtResources )
        {
            foreach( KeyValuePair<CswSchemaVersion, CswSchemaUpdateDriver> Pair in _UpdateDrivers )
            {
                Pair.Value.CswNbtSchemaModTrnsctn = new CswNbtSchemaModTrnsctn( CswNbtResources );
                if( false == Pair.Value.AlreadyRun() || Pair.Value.AlwaysRun )
                {
                    if( false == Pair.Value.AlwaysRun )
                    {
                        CswSchemaVersion NextSchemaVersion = TargetVersion( CswNbtResources );
                        NextSchemaVersion.ReleaseIteration = NextSchemaVersion.ReleaseIteration + _getCountOfRunOnceScripts( _UpdateDriversToRun );
                        Pair.Value.SchemaVersion = NextSchemaVersion;
                        Pair.Value.Description = Pair.Value.ScriptName;
                    }
                    _UpdateDriversToRun.Add( Pair.Value.SchemaVersion, Pair.Value );

                }
            }

        }//addVersionedScriptsToRun()

        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        private void _addVersionedScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            if( false == CswSchemaUpdateDriver.AlwaysRun )
            {
                CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
                CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
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

        private void _addRunBeforeScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
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

        private void _addRunAfterScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 99, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description = Description;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }

        }

        #endregion

    }//CswScriptCollections
}//ChemSW.Nbt.Schema
