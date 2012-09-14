using System;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Factory for Module Rules
    /// </summary>
    public class CswNbtModuleRuleFactory
    {
        public static CswNbtModuleRule makeModuleRule( CswNbtResources CswNbtResources, CswNbtModuleName ModuleName )
        {
            CswNbtModuleRule ret = null;
            if( CswNbtModuleName.BioSafety == ModuleName )
            {
                ret = new CswNbtModuleRuleBioSafety( CswNbtResources );
            }
            else if( CswNbtModuleName.CCPro == ModuleName )
            {
                ret = new CswNbtModuleRuleCCPro( CswNbtResources );
            }
            else if( CswNbtModuleName.CISPro == ModuleName )
            {
                ret = new CswNbtModuleRuleCISPro( CswNbtResources );
            }
            else if( CswNbtModuleName.IMCS == ModuleName )
            {
                ret = new CswNbtModuleRuleIMCS( CswNbtResources );
            }
            else if( CswNbtModuleName.Mobile == ModuleName )
            {
                ret = new CswNbtModuleRuleMobile( CswNbtResources );
            }
            else if( CswNbtModuleName.NBTManager == ModuleName )
            {
                ret = new CswNbtModuleRuleNBTManager( CswNbtResources );
            }
            else if( CswNbtModuleName.SI == ModuleName )
            {
                ret = new CswNbtModuleRuleSI( CswNbtResources );
            }
            else if( CswNbtModuleName.STIS == ModuleName )
            {
                ret = new CswNbtModuleRuleSTIS( CswNbtResources );
            }
            else
            {
                throw new CswDniException( ErrorType.Error, 
                                           "Unhandled ModuleName: "+ ModuleName.ToString(), 
                                           "CswNbtModuleRuleFactory did not recognize module name: "+ ModuleName.ToString());
            }
            return ret;
        }
    
    } // class CswNbtModuleBioSafety
}// namespace ChemSW.Nbt