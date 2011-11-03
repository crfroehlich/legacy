﻿

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01K-01
    /// </summary>
    public class CswUpdateSchemaTo01K01 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'K', 01 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

        }//Update()

    }//class CswUpdateSchemaTo01K01

}//namespace ChemSW.Nbt.Schema


