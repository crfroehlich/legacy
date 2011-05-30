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
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_021_04 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_021.Purpose, "clean up test data" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_021 _CswTstCaseRsrc_021 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_021_04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_021 = (CswTstCaseRsrc_021) CswTstCaseRsrc;

        }//ctor


        public void update()
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();
            _CswNbtSchemaModTrnsctn.dropTable( _CswTstCaseRsrc_021.ArbitraryTableName_01 );
            _CswNbtSchemaModTrnsctn.dropTable( CswAuditMetaData.makeAuditTableName( _CswTstCaseRsrc_021.ArbitraryTableName_01 ) );
            _CswTstCaseRsrc_021.restoreAuditSetting(); 
        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
