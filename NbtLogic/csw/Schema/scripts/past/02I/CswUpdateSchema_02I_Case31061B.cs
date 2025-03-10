﻿using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31061B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31061; }
        }

        public override string Title
        {
            get { return "Move Link props hrefs from field2 to field1_big"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataFieldType LinkFieldType = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswEnumNbtFieldType.Link );

            string where = "where jctnodepropid in (select jnp.jctnodepropid from jct_nodes_props jnp" +
                         " join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid" +
                         "  where ntp.fieldtypeid = " + LinkFieldType.FieldTypeId + " and jnp.field2 is not null)";

            CswTableUpdate linkTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31057.UpdateLinkPropsHrefCol", "jct_nodes_props" );
            DataTable jct_nodes_props = linkTU.getTable( new CswCommaDelimitedString() { "field1_big,field2" }, string.Empty, Int32.MinValue, where, false );

            foreach( DataRow Row in jct_nodes_props.Rows )
            {
                Row["field1_big"] = Row["field2"];
                Row["field2"] = string.Empty;
            }
            linkTU.update( jct_nodes_props );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema