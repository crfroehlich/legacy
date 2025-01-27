using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Multi Site Module
    /// </summary>
    public class CswNbtModuleRuleMultiSite: CswNbtModuleRule
    {
        public CswNbtModuleRuleMultiSite( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.MultiSite; } }
        protected override void OnEnable()
        {
            CswNbtMetaDataNodeType siteNT = _CswNbtResources.MetaData.getNodeType( "Site" );
            if( null != siteNT )
            {
                CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );
                QuotasAct.SetQuotaForNodeType( siteNT.NodeTypeId, Int32.MinValue, false );
            }
        }

        protected override void OnDisable()
        {

            CswNbtMetaDataNodeType siteNT = _CswNbtResources.MetaData.getNodeType( "Site" );
            if( null != siteNT )
            {
                CswNbtActQuotas QuotasAct = new CswNbtActQuotas( _CswNbtResources );

                CswNbtView sitesView = new CswNbtView( _CswNbtResources );
                sitesView.AddViewRelationship( siteNT, false );
                ICswNbtTree sitesTree = _CswNbtResources.Trees.getTreeFromView( sitesView, false, true, true );
                int SitesCount = sitesTree.getChildNodeCount();
                if( SitesCount > 1 && false == _CswNbtResources.CurrentNbtUser is CswNbtSystemUser )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot disable the MultiSite Module when multiple Sites exist", SitesCount + " Site nodes exist, cannot disable the MultiSite module" );
                }
                else
                {
                    QuotasAct.SetQuotaForNodeType( siteNT.NodeTypeId, 1, true );
                }
            }

        } // OnDisable()

    } // class CswNbtModuleRuleMultiSite
}// namespace ChemSW.Nbt
