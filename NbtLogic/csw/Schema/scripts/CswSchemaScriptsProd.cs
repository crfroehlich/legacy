﻿using System.Collections.Generic;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd : ICswSchemaScripts
    {
        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd()
        {
            // This is where you manually set to the last version of the previous release (the one currently in production)
            _MinimumVersion = new CswSchemaVersion( 2, 'B', 37 );

            // This is where you add new versions.

            #region CEDAR

            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29428() ) );                    //02C-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29717() ) );                    //02C-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case26561() ) );                    //02C-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29563() ) );                    //02C-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29563B() ) );                   //02C-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29833() ) );                    //02C-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29833B() ) );                   //02C-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29234() ) );                    //02C-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29861() ) );                    //02C-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29905() ) );                    //02C-010
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29680_Constituent() ) );        //02C-011
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29680_Constituent2() ) );       //02C-012
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29680_Constituent3() ) );       //02C-013
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29488() ) );                    //02C-014
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29489() ) );                    //02C-015
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29680_Constituent4() ) );       //02C-016
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29882() ) );                    //02C-017
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29729() ) );                    //02C-018
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29859() ) );                    //02C-019
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29935() ) );                    //02C-020
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29918() ) );                    //02C-021
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case30022() ) );                    //02C-022
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02C_Case29931() ) );                    //02C-023





            #endregion CEDAR

            #region DOGWOOD

            // e.g. _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_CaseXXXXX() ) );            //02C-000  02D-000
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29407() ) );                    //02C-024  02D-001
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30008() ) );                    //02C-025  02D-002
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30010() ) );                    //02C-026  02D-003
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29499() ) );                    //02C-027  02D-004
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29311_Design() ) );             //02C-028  02D-005
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29311_Sequences() ) );          //02C-029  02D-006
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29311_Fixes() ) );              //02C-030  02D-007
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case30126() ) );                    //02C-031  02D-008
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29311_DefaultValue() ) );       //02C-032  02D-009
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchema_02D_Case29311_MoreFixes() ) );          //02C-033  02D-010

            #endregion DOGWOOD

            // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == _MinimumVersion ||
                                                                                        ( _LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) ) )
            {
                _LatestVersion = Version;
            }

            #region Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01() ), RunBeforeEveryExecutionOfUpdater_01.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01M() ), RunBeforeEveryExecutionOfUpdater_01M.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01OC() ), RunBeforeEveryExecutionOfUpdater_01OC.Title );

            #region Dogwood Run Before Scripts

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29833() ), "Case 29833" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29833B() ), "Case 29833B" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29499A() ), "Case 29499A" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29499B() ), "Case 29499B" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02D_Case29311() ), "Case 29311" );

            #endregion

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_MakeMissingNodeTypeProps() ), "MakeMissingNodeTypeProps" );

            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02SQL() ), RunBeforeEveryExecutionOfUpdater_02SQL.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02() ), RunBeforeEveryExecutionOfUpdater_02.Title );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_03() ), RunBeforeEveryExecutionOfUpdater_03.Title );


            #endregion Before Scripts

            #region After Scripts

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), RunAfterEveryExecutionOfUpdater_01.Title );

            #endregion After Scripts

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

        public CswSchemaUpdateDriver Next( CswNbtResources CswNbtResources )
        {
            CswSchemaUpdateDriver ReturnVal = null;

            CswSchemaVersion myCurrentVersion = CurrentVersion( CswNbtResources );
            if( myCurrentVersion == MinimumVersion ||
                ( LatestVersion.CycleIteration == myCurrentVersion.CycleIteration &&
                    LatestVersion.ReleaseIdentifier == myCurrentVersion.ReleaseIdentifier &&
                    LatestVersion.ReleaseIteration > myCurrentVersion.ReleaseIteration ) )
            {
                ReturnVal = _UpdateDrivers[TargetVersion( CswNbtResources )];
            }
            return ( ReturnVal );
        }

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

            return ( new CswSchemaVersion( SuperCycle, ReleaseIdentifier, _UpdateDrivers.Keys.Count + 1 ) );
        }

        private void _addVersionedScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
            CswSchemaUpdateDriver.Description = CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );
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
