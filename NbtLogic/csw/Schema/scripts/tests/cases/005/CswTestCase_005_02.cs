﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_005_02 : CswUpdateSchemaTo
    {
        public override string Description { get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_005.Purpose, "verify that tables constrained were rolled back" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_005 _CswTstCaseRsrc_005 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_005_02( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_005 = (CswTstCaseRsrc_005) CswTstCaseRsc;
		}//ctor

        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_005.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			List<PkFkPair> PairList = _CswTstCaseRsrc_005.getPkFkPairs();

            //Verify that we cleaned up after ourselves
            foreach( PkFkPair CurrentPair in PairList )
            {
                _CswTstCaseRsrc.assertTableIsAbsent( CurrentPair.PkTableName );
                _CswTstCaseRsrc.assertTableIsAbsent( CurrentPair.FkTableName );
            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
