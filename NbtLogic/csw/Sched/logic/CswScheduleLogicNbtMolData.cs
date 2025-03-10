using System;
using System.Data;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtMolData: ICswScheduleLogic
    {
        #region Properties

        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.MolData ); }
        }

        private CswEnumScheduleLogicRunStatus _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        public CswEnumScheduleLogicRunStatus LogicRunStatus
        {
            get { return ( _LogicRunStatus ); }
        }

        private CswScheduleLogicDetail _CswScheduleLogicDetail = null;
        public CswScheduleLogicDetail CswScheduleLogicDetail
        {
            get { return ( _CswScheduleLogicDetail ); }
        }

        public void initScheduleLogicDetail( CswScheduleLogicDetail LogicDetail )
        {
            _CswScheduleLogicDetail = LogicDetail;
        }

        #endregion Properties

        #region State

        private CswCommaDelimitedString _nodesToUpdate = new CswCommaDelimitedString();

        private void _setLoad( ICswResources CswResources )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DirectStructureSearch ) )
            {
                _nodesToUpdate = _getMolsWithNoCTab( NbtResources );
            }
            else
            {
                _nodesToUpdate = _getNonFingerPrintedMols( NbtResources );
            }
        }

        #endregion State

        #region Scheduler Methods

        //Determine the number of non-fingerprinted Mols that need to be fingerprinted and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            if( _nodesToUpdate.Count == 0 )
            {
                _setLoad( CswResources );
            }
            return _nodesToUpdate.Count;
        }

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    int nodesPerIteration = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumConfigurationVariableNames.NodesProcessedPerCycle ) );
                    int molsProcessed = 0;
                    while( molsProcessed < nodesPerIteration && _nodesToUpdate.Count > 0 && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ) )
                    {
                        int NodeId = CswConvert.ToInt32( _nodesToUpdate[0] );
                        CswPrimaryKey NodePK = new CswPrimaryKey( "nodes", NodeId );

                        if( CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.DirectStructureSearch ) )
                        {
                            _generateCTab( CswNbtResources, NodePK );
                        }
                        else
                        {
                            _generateFingerprint( CswNbtResources, NodePK );
                        }

                        _nodesToUpdate.RemoveAt( 0 );
                        molsProcessed++;
                    }

                    _CswScheduleLogicDetail.StatusMessage = "Completed without error";
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line

                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtMolFingerprints::GetUpdatedItems() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch

            }//if we're not shutting down

        }//threadCallBack()

        private void _generateCTab( CswNbtResources CswNbtResources, CswPrimaryKey NodePK )
        {
            CswNbtSdBlobData sdBlobData = new CswNbtSdBlobData( CswNbtResources );

            CswNbtNode node = CswNbtResources.Nodes.GetNode( NodePK );
            foreach( CswNbtNodePropWrapper molProp in node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.MOL] )
            {
                CswNbtNodePropMol AsMol = molProp.AsMol;
                string href;
                string error;
                string formatted;
                sdBlobData.saveMol( AsMol.getMol(), new CswPropIdAttr( node, AsMol.NodeTypeProp ).ToString(), out href, out formatted, out error );
            }
        }

        private void _generateFingerprint( CswNbtResources CswNbtResources, CswPrimaryKey NodePK )
        {
            CswNbtNode node = CswNbtResources.Nodes.GetNode( NodePK );

            bool hasntBeenInserted = true;
            foreach( CswNbtNodePropWrapper prop in node.Properties[(CswEnumNbtFieldType) CswEnumNbtFieldType.MOL] )
            {
                if( hasntBeenInserted )
                {
                    string errorMsg;
                    CswNbtResources.StructureSearchManager.InsertFingerprintRecord( NodePK.PrimaryKey, prop.AsMol.getMol(), out errorMsg );
                    hasntBeenInserted = false;
                }
            }
        }

        private CswCommaDelimitedString _getMolsWithNoCTab( CswNbtResources NbtResources )
        {
            CswCommaDelimitedString nonCTabedMols = new CswCommaDelimitedString();

            string sql = @"select nodeid from mol_data where ctab is null";
            CswArbitrarySelect arbSelect = NbtResources.makeCswArbitrarySelect( "getNonCTabedMols", sql );
            DataTable nodesToUpdate = arbSelect.getTable();

            foreach( DataRow row in nodesToUpdate.Rows )
            {
                nonCTabedMols.Add( row["nodeid"].ToString() );
            }

            return nonCTabedMols;
        }

        private CswCommaDelimitedString _getNonFingerPrintedMols( CswNbtResources CswNbtResources )
        {
            CswCommaDelimitedString nonFingerprintedMols = new CswCommaDelimitedString();

            string sql = @"select jnp.nodeid from nodetype_props ntp
                                    join jct_nodes_props jnp on ntp.nodetypepropid = jnp.nodetypepropid 
                                        where ntp.fieldtypeid = (select fieldtypeid from field_types ft where ft.fieldtype = 'MOL')
                                            and jnp.clobdata is not null 
                                            and not exists (select nodeid from mol_keys where nodeid = jnp.nodeid)";
            CswArbitrarySelect arbSelect = CswNbtResources.makeCswArbitrarySelect( "getNonFingerprintedMols", sql );

            int lowerBound = 0;
            int upperBound = 500;
            DataTable jctnodesprops = arbSelect.getTable( lowerBound, upperBound, false, false ); //only get up to 500 records to do in a day

            foreach( DataRow row in jctnodesprops.Rows )
            {
                nonFingerprintedMols.Add( row["nodeid"].ToString() );
            }

            return nonFingerprintedMols;
        }

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        #endregion Scheduler Methods

    }//CswScheduleLogicNbtMolFingerpritns

}//namespace ChemSW.Nbt.Sched
