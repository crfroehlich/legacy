﻿using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsKatsura : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02K_CaseXXXXX()
                    new CswUpdateDDL_02K_Case29311_AddRelationalColumn(),
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02K_CaseXXXXX()
                    new CswUpdateMetaData_02K_Case29311()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02K_CaseXXXXX()
                    new CswUpdateSchema_02K_Case29311_Design(),
                    new CswUpdateSchema_02K_Case29311_Sequences(),
                    new CswUpdateSchema_02K_Case29311_Fixes(),
                    new CswUpdateSchema_02K_Case29311_DefaultValue(),
                    new CswUpdateSchema_02K_Case29311_MoreFixes(),
                    new CswUpdateSchema_02K_Case29314()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsJuniper
}//namespace ChemSW.Nbt.Schema