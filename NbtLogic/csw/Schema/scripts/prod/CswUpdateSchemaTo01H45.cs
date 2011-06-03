
using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-42
    /// </summary>
    public class CswUpdateSchemaTo01H45 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 45 ); } }
        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H45( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            // Case 21219
            CswNbtView ProblemsOpen = _CswNbtSchemaModTrnsctn.restoreView( "Problems: Open" );
            if( null != ProblemsOpen )
            {
                ProblemsOpen.Root.ChildRelationships.Clear();
            }
            else
            {
                ProblemsOpen = _CswNbtSchemaModTrnsctn.makeView();
                ProblemsOpen.makeNew( "Problems: Open", NbtViewVisibility.Global, null, null, null );
            }
            CswNbtMetaDataNodeType ProblemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Problem" );
            CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
            if( null != ProblemNT )
            {
                CswNbtViewRelationship ProblemRel = ProblemsOpen.AddViewRelationship( ProblemNT, false );
                CswNbtMetaDataNodeTypeProp ClosedNtp = ProblemNT.getNodeTypeProp( CswNbtObjClassProblem.ClosedPropertyName );
                CswNbtMetaDataNodeTypeProp EquipmentNtp = ProblemNT.getNodeTypeProp( CswNbtObjClassProblem.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp StartDateNtp = ProblemNT.getNodeTypeProp( "Start Date" );
                CswNbtMetaDataNodeTypeProp SummaryNtp = ProblemNT.getNodeTypeProp( "Summary" );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = ProblemNT.getNodeTypeProp( "Technician" );

                if( null != StartDateNtp )
                {
                    CswNbtViewProperty StartDateVp = ProblemsOpen.AddViewProperty( ProblemRel, StartDateNtp );
                    ProblemsOpen.AddViewPropertyFilter( StartDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );
                }

                CswNbtViewProperty ClosedVp = ProblemsOpen.AddViewProperty( ProblemRel, ClosedNtp );
                ProblemsOpen.AddViewPropertyFilter( ClosedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                if( null != SummaryNtp )
                {
                    CswNbtViewProperty SummaryVp = ProblemsOpen.AddViewProperty( ProblemRel, SummaryNtp );
                    ProblemsOpen.AddViewPropertyFilter( SummaryVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtViewProperty TechnicianVp = ProblemsOpen.AddViewProperty( ProblemRel, TechnicianNtp );
                ProblemsOpen.AddViewPropertyFilter( TechnicianVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                CswNbtViewProperty EquipmentVp = ProblemsOpen.AddViewProperty( ProblemRel, EquipmentNtp );
                ProblemsOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                if( null != EquipmentNT )
                {
                    CswNbtViewRelationship EquipmentRel = ProblemsOpen.AddViewRelationship( ProblemRel, CswNbtViewRelationship.PropOwnerType.First, EquipmentNtp, false );
                    CswNbtMetaDataNodeTypeProp StatusNtp = EquipmentNT.getNodeTypeProp( CswNbtObjClassEquipment.StatusPropertyName );
                    CswNbtMetaDataNodeTypeProp TypeNtp = EquipmentNT.getNodeTypeProp( CswNbtObjClassEquipment.TypePropertyName );
                    CswNbtMetaDataNodeTypeProp ManufacturerNtp = EquipmentNT.getNodeTypeProp( "Manufacturer" );
                    CswNbtMetaDataNodeTypeProp SerialNoNtp = EquipmentNT.getNodeTypeProp( "Serial No" );

                    CswNbtViewProperty TypeVp = ProblemsOpen.AddViewProperty( EquipmentRel, TypeNtp );
                    ProblemsOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    if( null != SerialNoNtp )
                    {
                        CswNbtViewProperty SerialNoVp = ProblemsOpen.AddViewProperty( EquipmentRel, SerialNoNtp );
                        ProblemsOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                    }

                    if( null != ManufacturerNtp )
                    {
                        CswNbtViewProperty ManufacturerVp = ProblemsOpen.AddViewProperty( EquipmentRel, ManufacturerNtp );
                        ProblemsOpen.AddViewPropertyFilter( ManufacturerVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );
                    }

                    CswNbtViewProperty StatusVp = ProblemsOpen.AddViewProperty( EquipmentRel, StatusNtp );
                    ProblemsOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, "Available", false );
                } // if( null != EquipmentNT )

                ProblemsOpen.save();
            } //if( null != ProblemNT )


            // Case 21221
            CswNbtView TasksOpen = _CswNbtSchemaModTrnsctn.restoreView( "Tasks: Open" );
            if( null != TasksOpen )
            {
                TasksOpen.Root.ChildRelationships.Clear();
            }
            else
            {
                TasksOpen = _CswNbtSchemaModTrnsctn.makeView();
                TasksOpen.makeNew( "Tasks: Open", NbtViewVisibility.Global, null, null, null );
            }
            CswNbtMetaDataNodeType TaskNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Task" );
            if( null != TaskNT )
            {
                CswNbtViewRelationship TaskRel = ProblemsOpen.AddViewRelationship( TaskNT, false );
                CswNbtMetaDataNodeTypeProp CompletedNtp = TaskNT.getNodeTypeProp( CswNbtObjClassTask.CompletedPropertyName );
                CswNbtMetaDataNodeTypeProp EquipmentNtp = TaskNT.getNodeTypeProp( CswNbtObjClassTask.OwnerPropertyName );
                CswNbtMetaDataNodeTypeProp DueDateNtp = TaskNT.getNodeTypeProp( CswNbtObjClassTask.DueDatePropertyName );
                CswNbtMetaDataNodeTypeProp SummaryNtp = TaskNT.getNodeTypeProp( CswNbtObjClassTask.SummaryPropertyName );
                CswNbtMetaDataNodeTypeProp TechnicianNtp = TaskNT.getNodeTypeProp( "Technician" );


                CswNbtViewProperty DueDateVp = TasksOpen.AddViewProperty( TaskRel, DueDateNtp );
                TasksOpen.AddViewPropertyFilter( DueDateVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals, DateTime.MinValue.ToString(), false );

                CswNbtViewProperty CompletedVp = TasksOpen.AddViewProperty( TaskRel, CompletedNtp );
                TasksOpen.AddViewPropertyFilter( CompletedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                CswNbtViewProperty SummaryVp = TasksOpen.AddViewProperty( TaskRel, SummaryNtp );
                TasksOpen.AddViewPropertyFilter( SummaryVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );

                if( null != TechnicianNtp )
                {
                    CswNbtViewProperty TechnicianVp = TasksOpen.AddViewProperty( TaskRel, TechnicianNtp );
                    TasksOpen.AddViewPropertyFilter( TechnicianVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                }

                CswNbtViewProperty EquipmentVp = TasksOpen.AddViewProperty( TaskRel, EquipmentNtp );
                TasksOpen.AddViewPropertyFilter( EquipmentVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                if( null != EquipmentNT )
                {
                    CswNbtViewRelationship EquipmentRel = TasksOpen.AddViewRelationship( TaskRel, CswNbtViewRelationship.PropOwnerType.First, EquipmentNtp, false );
                    CswNbtMetaDataNodeTypeProp StatusNtp = EquipmentNT.getNodeTypeProp( CswNbtObjClassEquipment.StatusPropertyName );
                    CswNbtMetaDataNodeTypeProp TypeNtp = EquipmentNT.getNodeTypeProp( CswNbtObjClassEquipment.TypePropertyName );
                    CswNbtMetaDataNodeTypeProp SerialNoNtp = EquipmentNT.getNodeTypeProp( "Serial No" );

                    CswNbtViewProperty TypeVp = TasksOpen.AddViewProperty( EquipmentRel, TypeNtp );
                    TasksOpen.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

                    if( null != SerialNoNtp )
                    {
                        CswNbtViewProperty SerialNoVp = TasksOpen.AddViewProperty( EquipmentRel, SerialNoNtp );
                        TasksOpen.AddViewPropertyFilter( SerialNoVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Contains, string.Empty, false );
                    }

                    CswNbtViewProperty StatusVp = TasksOpen.AddViewProperty( EquipmentRel, StatusNtp );
                    TasksOpen.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, "Retired", false );
                } // if( null != EquipmentNT )

                TasksOpen.save();
            } //if( null != ProblemNT )


        } // update()

    }//class CswUpdateSchemaTo01H45

}//namespace ChemSW.Nbt.Schema

