
namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the FireDb Sync Module
    /// </summary>
    public class CswNbtModuleRuleFireDbSync : CswNbtModuleRule
    {
        public CswNbtModuleRuleFireDbSync( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.FireDbSync; } }
        public override void OnEnable()
        {

        }// OnEnabled

        public override void OnDisable()
        {

        } // OnDisable()

    } // class CswNbtModuleFireDbSync
}// namespace ChemSW.Nbt