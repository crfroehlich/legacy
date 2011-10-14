﻿

using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-02
    /// </summary>
    public class CswUpdateSchemaTo01J02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 02 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            //Case 23809
            foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Roles and Users" ) )
            {
                View.Category = "System";
                View.save();
            }

            //Case 23782
            CswNbtMetaDataNodeType RouteNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Physical Inspection Route" );
            if( null != RouteNt )
            {
                CswNbtMetaDataNodeTypeProp MpGridNtp = RouteNt.getNodeTypeProp( "FE Inspection Points Grid" );
                if( null != MpGridNtp && MpGridNtp.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Grid )
                {
                    if( null != MpGridNtp.ViewId )
                    {
                        CswNbtView MpgView = _CswNbtSchemaModTrnsctn.restoreView( MpGridNtp.ViewId );
                        CswNbtView BackupView = _CswNbtSchemaModTrnsctn.restoreView( MpGridNtp.ViewId );

                        CswNbtMetaDataNodeType PiNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "FE Inspection Point" );
                        if( null != PiNt )
                        {
                            CswNbtMetaDataNodeTypeProp RouteNtp = PiNt.getNodeTypeProp( "Route" );
                            if( null != RouteNtp )
                            {
                                MpgView.Clear();
                                CswNbtViewRelationship RouteRel = MpgView.AddViewRelationship( RouteNt, false );
                                CswNbtViewRelationship InspectPointRel = MpgView.AddViewRelationship( RouteRel, CswNbtViewRelationship.PropOwnerType.Second, RouteNtp, false );

                                CswNbtMetaDataNodeTypeProp BarcodeNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
                                MpgView.AddViewProperty( InspectPointRel, BarcodeNtp );

                                CswNbtMetaDataNodeTypeProp DescNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
                                MpgView.AddViewProperty( InspectPointRel, DescNtp );

                                CswNbtMetaDataNodeTypeProp LocationNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
                                MpgView.AddViewProperty( InspectPointRel, LocationNtp );

                                CswNbtMetaDataNodeTypeProp LastIDateNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                                MpgView.AddViewProperty( InspectPointRel, LastIDateNtp );

                                CswNbtMetaDataNodeTypeProp StatusNtp = PiNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
                                MpgView.AddViewProperty( InspectPointRel, StatusNtp );

                                MpgView.ViewName = BackupView.ViewName;
                                MpgView.ViewMode = BackupView.ViewMode;
                                MpgView.Visibility = BackupView.Visibility;
                                MpgView.VisibilityRoleId = BackupView.VisibilityRoleId;
                                MpgView.VisibilityUserId = BackupView.VisibilityUserId;
                                MpgView.Width = BackupView.Width;
                                MpgView.Category = MpgView.Category;
                                MpgView.save();
                            }
                        }
                    }
                }
            }


            //foreach( CswNbtView View in _CswNbtSchemaModTrnsctn.restoreViews( "Mount Points Grid" ) )
            //{
            //    if( View.ViewMode == NbtViewRenderingMode.Grid && 
            //        View.Visibility == NbtViewVisibility.Property && 
            //        View.Root. )
            //}

        }//Update()

    }//class CswUpdateSchemaTo01J02

}//namespace ChemSW.Nbt.Schema


