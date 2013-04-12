using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CCPro Module
    /// </summary>
    public class CswNbtModuleRuleCCPro : CswNbtModuleRule
    {
        public CswNbtModuleRuleCCPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.CCPro; } }
        public override void OnEnable() { }
        public override void OnDisable() { }

    } // class CswNbtModuleCCPro
}// namespace ChemSW.Nbt
