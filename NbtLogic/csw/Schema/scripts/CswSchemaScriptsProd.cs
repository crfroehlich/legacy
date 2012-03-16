﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Core;

//using ChemSW.RscAdo;
//using ChemSW.TblDn;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsProd : ICswSchemaScripts
    {
        //private CswNbtResources _CswNbtResources;

        private Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> _UpdateDrivers = new Dictionary<CswSchemaVersion, CswSchemaUpdateDriver>();
        public Dictionary<CswSchemaVersion, CswSchemaUpdateDriver> UpdateDrivers { get { return _UpdateDrivers; } }

        public CswSchemaScriptsProd() //CswNbtResources CswNbtResources )
        {
            //_CswNbtResources = CswNbtResources;

            // This is where you manually set to the last version of the previous release
            _MinimumVersion = new CswSchemaVersion( 1, 'L', 21 );

            // This is where you add new versions.
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M01() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M02() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M03() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M04() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M05() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M06() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M07() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M08() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M09() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M10() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M11() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M12() ) );
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaTo01M13() ) ) ;
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24988() ) );       //14
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25322() ) );       //15
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24914() ) );       //16
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25226() ) );       //17
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24515() ) );       //18
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25381() ) );       //19
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25290() ) );       //20
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25377() ) );       //21
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24777() ) );       //22
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25374() ) );       //23
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25374B() ) );      //24
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24520() ) );       //25
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25426() ) );       //26
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase25466() ) );      //27
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24988Epilogue01() ) ); //28
            _addVersionedScript( new CswSchemaUpdateDriver( new CswUpdateSchemaCase24988Epilogue02() ) ); //29


            // This automatically detects the latest version
            _LatestVersion = _MinimumVersion;
            foreach( CswSchemaVersion Version in _UpdateDrivers.Keys.Where( Version => _LatestVersion == _MinimumVersion ||
                                                                                        ( _LatestVersion.CycleIteration == Version.CycleIteration &&
                                                                                            _LatestVersion.ReleaseIdentifier == Version.ReleaseIdentifier &&
                                                                                            _LatestVersion.ReleaseIteration < Version.ReleaseIteration ) ) )
            {
                _LatestVersion = Version;
            }



            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_01() ), "Arbitrary DML" );
            _addRunBeforeScript( new CswSchemaUpdateDriver( new RunBeforeEveryExecutionOfUpdater_02() ), "Shell-out script" );

            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_01() ), "Arbitrary DML" );
            _addRunAfterScript( new CswSchemaUpdateDriver( new RunAfterEveryExecutionOfUpdater_02() ), "Shell-out script" );

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

            }//get

        }//indexer

        public void stampSchemaVersion( CswNbtResources CswNbtResources, CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswNbtResources.ConfigVbls.setConfigVariableValue( "schemaversion", CswSchemaUpdateDriver.SchemaVersion.ToString() ); ;
        }//stampSchemaVersion()


        #endregion



        #region Versioned scripts
  
        CswSchemaVersion _makeNextSchemaVersion()
        {
            int SuperCycle = _MinimumVersion.CycleIteration;
            char ReleaseIdentifier = _MinimumVersion.ReleaseIdentifier;
            if( 'Z' != ReleaseIdentifier )
            {
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
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

        }//_makeNextSchemaVersion()


        private void _addVersionedScript( CswSchemaUpdateDriver CswSchemaUpdateDriver )
        {
            CswSchemaUpdateDriver.SchemaVersion = _makeNextSchemaVersion();
            CswSchemaUpdateDriver.Description = "Update to schema version " + CswSchemaUpdateDriver.SchemaVersion.ToString(); //we do this in prod scripts because test scripts have a different dispensation for description
            _UpdateDrivers.Add( CswSchemaUpdateDriver.SchemaVersion, CswSchemaUpdateDriver );

        }//addReleaseDmlDriver() 


        #endregion

        #region Run-always scripts

        //Run before
        private List<CswSchemaUpdateDriver> _RunBeforeScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunBeforeScripts
        {
            get
            {
                return ( _RunBeforeScripts );
            }
        }//RunBeforeScripts


        private void _addRunBeforeScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunBeforeScripts.Count );
            CswSchemaUpdateDriver.Description = "Run before script: " + Description;
            if( false == _RunBeforeScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunBeforeScripts.Add( CswSchemaUpdateDriver );
            }
        }//


        //Run after
        private List<CswSchemaUpdateDriver> _RunAfterScripts = new List<CswSchemaUpdateDriver>();
        public List<CswSchemaUpdateDriver> RunAfterScripts
        {
            get
            {
                return ( _RunAfterScripts );
            }

        }//RunBeforeScripts
        private void _addRunAfterScript( CswSchemaUpdateDriver CswSchemaUpdateDriver, string Description )
        {
            CswSchemaUpdateDriver.SchemaVersion = new CswSchemaVersion( 0, '#', _RunAfterScripts.Count );
            CswSchemaUpdateDriver.Description = "Run after script: " + Description;
            if( false == _RunAfterScripts.Contains( CswSchemaUpdateDriver ) )
            {
                _RunAfterScripts.Add( CswSchemaUpdateDriver );
            }

        }//
        #endregion
    }//CswScriptCollections

}//ChemSW.Nbt.Schema
