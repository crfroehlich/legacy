using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!


            #region DOGWOOD


            _addRelationalNodeId( CswEnumDeveloper.SS, 29311 );


            #endregion DOGWOOD

        }//Update()        



        #region DOGWOOD Methods

        private void _addRelationalNodeId( CswEnumDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "relationalid" ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodes", "relationalid", "Foreign key to relational-model copy of this node", false, false );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "relationaltable" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "relationaltable", "Table of relational-model copy of this node", false, false, 50 );
            }

            _resetBlame();
        } // _addRelationalNodeId()

        #endregion DOGWOOD Methods

 


    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


