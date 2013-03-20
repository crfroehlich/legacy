﻿using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28950
    /// </summary>
    public class CswUpdateSchema_02A_Case28950: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28950; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass reportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp instructionsOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Instructions );
            CswNbtMetaDataObjectClassProp sqlOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Sql );

            foreach( CswNbtMetaDataNodeTypeProp instructionsNTP in instructionsOCP.getNodeTypeProps() )
            {
                instructionsNTP.TextAreaColumns = 65;
                instructionsNTP.TextAreaRows = 6;
            }

            foreach( CswNbtMetaDataNodeTypeProp sqlNTP in sqlOCP.getNodeTypeProps() )
            {
                sqlNTP.HelpText = "";
            }

            string txt = "To create a parameterized sql report wrap the parameter name(s) in {} brackets.\n\n";
            txt += "Enter a User property name such as 'username' to have the parameter automatically filled in with the current users value for that property. You may also use the keywords 'nodeid' or 'userid' to have the primary key of the current user filled in as the value for that parameter.";

            foreach( CswNbtObjClassReport reportNode in reportOC.getNodes( false, true, false, true ) )
            {
                reportNode.Instructions.Text = txt;
                reportNode.postChanges( false );
            }

        } // update()

    }//class CswUpdateSchema_02A_Case28950

}//namespace ChemSW.Nbt.Schema