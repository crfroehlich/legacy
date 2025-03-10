using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30090
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30090 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30090; }
        }

        public override void update()
        {

            Int32 LOLISyncModuleId = _CswNbtSchemaModTrnsctn.Modules.GetModuleId( CswEnumNbtModuleName.LOLISync );
            if( Int32.MinValue == LOLISyncModuleId )
            {
                _CswNbtSchemaModTrnsctn.createModule( "LOLI Sync", CswEnumNbtModuleName.LOLISync, false );
                _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswEnumNbtModuleName.RegulatoryLists, CswEnumNbtModuleName.LOLISync );
            }

        } // update()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30090

}//namespace ChemSW.Nbt.Schema