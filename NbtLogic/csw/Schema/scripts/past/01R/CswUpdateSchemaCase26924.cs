﻿
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26924
    /// </summary>
    public class CswUpdateSchemaCase26924 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.netquantity_enforced, "When set to 1, users cannot dispense any quantity greater than the parent container's netquantity.", "1", IsSystem: false );
        }//Update()

    }//class CswUpdateSchemaCase26924

}//namespace ChemSW.Nbt.Schema