﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30041_Vendors : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30041; }
        }

        public override void update()
        {
            // Scheduled rule for CAFImports
            _CswNbtSchemaModTrnsctn.createScheduledRule( CswEnumNbtScheduleRuleNames.CAFImport, CswEnumRecurrence.NHours, 1 );

            // CAF bindings definitions for Vendors

            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, 1, "vendors", "Vendor" );                                                

            // Binding
            ImpMgr.importBinding( "accountno", "Account No", "" );
            ImpMgr.importBinding( "city", "City", "" );
            ImpMgr.importBinding( "contactname", "Contact Name", "" );
            ImpMgr.importBinding( "fax", "Fax", "" );
            ImpMgr.importBinding( "phone", "Phone", "" );
            ImpMgr.importBinding( "state", "State", "" );
            ImpMgr.importBinding( "street1", "Street1", "" );
            ImpMgr.importBinding( "street2", "Street2", "" );
            ImpMgr.importBinding( "vendorid", "Legacy Id", "" );
            ImpMgr.importBinding( "vendorname", "Vendor Name", "" );
            ImpMgr.importBinding( "zip", "Zip", "" );

            // Relationship
            // none

            ImpMgr.finalize();
        } // update()

    } // class CswUpdateSchema_02F_Case30041_Vendors

}//namespace ChemSW.Nbt.Schema