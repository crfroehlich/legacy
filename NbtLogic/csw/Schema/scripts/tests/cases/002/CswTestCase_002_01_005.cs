﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_002_01_005 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'T', 005 ); } }

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_002.Purpose, "add and throw" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_002 _CswTstCaseRsrc_002 = null;
        public CswTestCase_002_01_005( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_002 = new CswTstCaseRsrc_002( _CswNbtSchemaModTrnsctn );

        }//ctor



        public void update()
        {

            _CswNbtSchemaModTrnsctn.addColumn( _CswTstCaseRsrc_002.TestColumnNameOne, DataDictionaryColumnType.Value, 20, 0, "foo", "test column", string.Empty, string.Empty, false, false, false, string.Empty, false, DataDictionaryPortableDataType.String, false, false, _CswTstCaseRsrc_002.TestTableName, DataDictionaryUniqueType.None, false, string.Empty );

            throw ( new ChemSW.Exceptions.CswDniExceptionIgnoreDeliberately() );//this will cause a rollback, so we can test that the column add was rolled back in the next script in the 002 series


        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
