using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Regulatory Lists Module
    /// </summary>
    public class CswNbtModuleRuleRegulatoryLists : CswNbtModuleRule
    {
        public CswNbtModuleRuleRegulatoryLists( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.RegulatoryLists; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.CISPro );
            }

            //Show the following Material properties
            //   Regulatory Lists
            int MaterialOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( MaterialOC_Id ) )
            {
                _CswNbtResources.Modules.AddPropToFirstTab( NodeTypeId, CswNbtObjClassChemical.PropertyName.RegulatoryLists );
            }

        }

        public override void OnDisable()
        {
            //Hide the following Material properties
            //   Regulatory Lists
            int MaterialOC_Id = _CswNbtResources.MetaData.getObjectClassId( CswEnumNbtObjectClass.ChemicalClass );
            foreach( int NodeTypeId in _CswNbtResources.MetaData.getNodeTypeIds( MaterialOC_Id ) )
            {
                _CswNbtResources.Modules.HideProp( NodeTypeId, CswNbtObjClassChemical.PropertyName.RegulatoryLists );
            }

        } // OnDisable()

    } // class CswNbtModuleRegulatoryLists
}// namespace ChemSW.Nbt
