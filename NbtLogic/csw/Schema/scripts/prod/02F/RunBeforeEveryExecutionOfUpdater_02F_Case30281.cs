using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02F_Case30281 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30281";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30281; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ExpirationLockedOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationLocked );
            if( null == ExpirationLockedOCP )
            {
                ExpirationLockedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassContainer.PropertyName.ExpirationLocked,
                    FieldType = CswEnumNbtFieldType.Logical,
                    SetValOnAdd = false,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ExpirationLockedOCP, CswEnumTristate.True, CswEnumNbtSubFieldName.Checked );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02F_Case30281
}//namespace ChemSW.Nbt.Schema


