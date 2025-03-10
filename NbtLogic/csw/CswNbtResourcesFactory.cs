using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    /// <summary>
    /// A collection of useful resources for NBT business logic.
    /// </summary>
    public class CswNbtResourcesFactory
    {
        /// <summary>
        /// Create a new CswNbtResources using same initialization parameters as an existing one
        /// Does not copy data or access id
        /// </summary>
        public static CswNbtResources makeCswNbtResources( CswNbtResources OtherResources )
        {
            return makeCswNbtResources( OtherResources.AppType,
                                        OtherResources.SetupVbls.SetupMode,
                                        OtherResources.ExcludeDisabledModules,
                                        OtherResources.CswSuperCycleCache,
                                        OtherResources.PooledConnectionState,
                                        null, OtherResources.CswLogger );
        }

        /// <summary>
        /// Create a new CswNbtResources
        /// </summary>
        public static CswNbtResources makeCswNbtResources( CswEnumAppType AppType, CswEnumSetupMode SetupMode, bool ExcludeDisabledModules, ICswSuperCycleCache CswSuperCycleCache = null, ChemSW.RscAdo.CswEnumPooledConnectionState PooledConnectionState = RscAdo.CswEnumPooledConnectionState.Open, ICswResources CswResourcesMaster = null, ICswLogger CswLogger = null )
        {
            if( null == CswSuperCycleCache )
            {
                CswSuperCycleCache = new CswSuperCycleCacheDefault();
            }

            CswSetupVbls SetupVbls = new CswSetupVbls( SetupMode );
            CswDbCfgInfo ConfigInfo = new CswDbCfgInfo( SetupMode );
            
            CswNbtResources ReturnVal = new CswNbtResources( AppType, SetupVbls, ConfigInfo, ExcludeDisabledModules, CswSuperCycleCache, CswResourcesMaster, CswLogger );
            ReturnVal.SetDbResources( PooledConnectionState );

            ////bz # 9896: This events must only be assigned when we first instance the class;
            ////if we also assign them to cached resources, we get duplicate events occuring :-(
            //CswNbtMetaDataEvents CswNbtMetaDataEvents = new CswNbtMetaDataEvents( ReturnVal );
            //ReturnVal.OnMakeNewNodeType += new CswNbtResources.NewNodeTypeEventHandler( CswNbtMetaDataEvents.OnMakeNewNodeType );
            //ReturnVal.OnCopyNodeType += new CswNbtResources.CopyNodeTypeEventHandler( CswNbtMetaDataEvents.OnCopyNodeType );
            //ReturnVal.OnMakeNewNodeTypeProp += new CswNbtResources.NewNodeTypePropEventHandler( CswNbtMetaDataEvents.OnMakeNewNodeTypeProp );
            //ReturnVal.OnEditNodeTypePropName += new CswNbtResources.EditPropNameEventHandler( CswNbtMetaDataEvents.OnEditNodeTypePropName );
            //ReturnVal.OnDeleteNodeTypeProp += new CswNbtResources.DeletePropEventHandler( CswNbtMetaDataEvents.OnDeleteNodeTypeProp );
            //ReturnVal.OnEditNodeTypeName += new CswNbtResources.EditNodeTypeNameEventHandler( CswNbtMetaDataEvents.OnEditNodeTypeName );

            return ( ReturnVal );
        }
    } // CswNbtResourcesFactory

}//ChemSW.NbtResources
