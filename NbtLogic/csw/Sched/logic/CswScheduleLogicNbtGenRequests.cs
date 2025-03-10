using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNbtGenRequests: ICswScheduleLogic
    {
        public string RuleName
        {
            get { return ( CswEnumNbtScheduleRuleNames.GenRequest ); }
        }

        //Determine the number of recurring requests that need to be processed and return that value
        public Int32 getLoadCount( ICswResources CswResources )
        {
            Int32 LoadCount = 0;
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( NbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtActRequesting ActRequesting = new CswNbtActRequesting( NbtResources );
                CswNbtView AllRecurringRequests = ActRequesting.getDueRecurringRequestItemsView();
                ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( AllRecurringRequests, RequireViewPermissions: false, IncludeSystemNodes: false, IncludeHiddenNodes: false );
                LoadCount = Tree.getChildNodeCount();
            }
            return LoadCount;
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
        }//initScheduleLogicDetail()

        public void threadCallBack( ICswResources CswResources )
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Running;

            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            _CswNbtResources.AuditContext = "Scheduler Task: " + RuleName;

            if( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus )
            {
                try
                {
                    if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                    {
                        Int32 RequestsLimit = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.generatorlimit.ToString() ) );
                        if( RequestsLimit <= 0 )
                        {
                            RequestsLimit = 1;
                        }

                        CswNbtActRequesting ActRequesting = new CswNbtActRequesting( _CswNbtResources );
                        CswNbtView AllRecurringRequests = ActRequesting.getDueRecurringRequestItemsView();
                        ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( AllRecurringRequests, RequireViewPermissions: false, IncludeSystemNodes: false, IncludeHiddenNodes: false );

                        Int32 TotalRequestsProcessed = 0;
                        string RequestDescriptions = string.Empty;
                        Int32 TotatRequests = Tree.getChildNodeCount();

                        for( Int32 ChildN = 0; ( ChildN < TotatRequests && TotalRequestsProcessed < RequestsLimit ) && ( CswEnumScheduleLogicRunStatus.Stopping != _LogicRunStatus ); ChildN++ )
                        {
                            string Description = "";
                            try
                            {
                                Tree.goToNthChild( ChildN );
                                CswNbtObjClassRequestItem CurrentRequestItem = Tree.getNodeForCurrentPosition();
                                if( _doesRequestItemCopyNow( CurrentRequestItem ) )
                                {
                                    Description = CurrentRequestItem.Description.StaticText;
                                    CswNbtObjClassRequest RecurringRequest = _CswNbtResources.Nodes[CurrentRequestItem.Request.RelatedNodeId];
                                    if( null != RecurringRequest )
                                    {
                                        CswNbtObjClassUser Requestor = _CswNbtResources.Nodes[RecurringRequest.Requestor.RelatedNodeId];
                                        if( null != Requestor )
                                        {
                                            CswNbtObjClassRequestItem CopiedRequestItem = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CurrentRequestItem.NodeTypeId, delegate( CswNbtNode NewNode )
                                            {
                                                // We'd get all of this for free if we used copyNode, 
                                                // but then we'd have to manually do as much work in the other direction:
                                                // un-hiding properties, etc.
                                                CswNbtActRequesting ThisUserAct = new CswNbtActRequesting( _CswNbtResources, Requestor );
                                                CswNbtObjClassRequest UsersCartNode = ThisUserAct.getCurrentRequestNode();
                                                if( null != UsersCartNode )
                                                {
                                                    CswNbtObjClassRequestItem NewRequestItem = NewNode;
                                                    // Most importantly, put the new request item in the current cart
                                                    NewRequestItem.Request.RelatedNodeId = UsersCartNode.NodeId;

                                                    NewRequestItem.Requestor.RelatedNodeId = CurrentRequestItem.Requestor.RelatedNodeId;
                                                    NewRequestItem.Material.RelatedNodeId = CurrentRequestItem.Material.RelatedNodeId;
                                                    NewRequestItem.Material.CachedNodeName = CurrentRequestItem.Material.CachedNodeName;
                                                    NewRequestItem.InventoryGroup.RelatedNodeId = CurrentRequestItem.InventoryGroup.RelatedNodeId;
                                                    NewRequestItem.Location.SelectedNodeId = CurrentRequestItem.Location.SelectedNodeId;
                                                    NewRequestItem.Location.CachedPath = CurrentRequestItem.Location.CachedPath;
                                                    NewRequestItem.Comments.CommentsJson = CurrentRequestItem.Comments.CommentsJson;
                                                    NewRequestItem.Type.Value = CurrentRequestItem.Type.Value;

                                                    if( CurrentRequestItem.Type.Value == CswNbtObjClassRequestItem.Types.MaterialSize )
                                                    {
                                                        NewRequestItem.Size.RelatedNodeId = CurrentRequestItem.Size.RelatedNodeId;
                                                        NewRequestItem.Size.CachedNodeName = CurrentRequestItem.Size.CachedNodeName;
                                                        NewRequestItem.SizeCount.Value = CurrentRequestItem.SizeCount.Value;
                                                    }
                                                    else
                                                    {
                                                        NewRequestItem.Quantity.Quantity = CurrentRequestItem.Quantity.Quantity;
                                                        NewRequestItem.Quantity.CachedUnitName = CurrentRequestItem.Quantity.CachedUnitName;
                                                        NewRequestItem.Quantity.UnitId = CurrentRequestItem.Quantity.UnitId;
                                                    }
                                                    NewRequestItem.Status.Value = CswNbtObjClassRequestItem.Statuses.Pending;

                                                    NewRequestItem.postChanges( ForceUpdate: false );

                                                    CurrentRequestItem.NextReorderDate.DateTimeValue = CswNbtPropertySetSchedulerImpl.getNextDueDate( CurrentRequestItem.Node, CurrentRequestItem.NextReorderDate, CurrentRequestItem.RecurringFrequency, ForceUpdate: true );
                                                    CurrentRequestItem.postChanges( ForceUpdate: false );
                                                }
                                            } );
                                        }
                                        RequestDescriptions += CurrentRequestItem.Description + "; ";
                                    }
                                }
                                Tree.goToParentNode();
                            } // if ~( not null, is recurring and is due)
                            catch( Exception Exception )
                            {
                                string Message = "Unable to create recurring request " + Description + ", due to the following exception: " + Exception.Message;
                                RequestDescriptions += Message;
                                _CswNbtResources.logError( new CswDniException( Message ) );

                            } //catch
                            finally
                            {
                                TotalRequestsProcessed += 1;
                            }
                        } //iterate requests
                        
                        string StatusMessage = "No Recurring Requests found to process";
                        if( TotalRequestsProcessed > 0 )
                        {
                            StatusMessage = TotalRequestsProcessed.ToString() + " requests processed: " + RequestDescriptions;
                        }
                        _CswScheduleLogicDetail.StatusMessage = StatusMessage;
                        _LogicRunStatus = CswEnumScheduleLogicRunStatus.Succeeded; //last line
                    }
                }//try
                catch( Exception Exception )
                {
                    _CswScheduleLogicDetail.StatusMessage = "CswScheduleLogicNbtGenRequests::threadCallBack() exception: " + Exception.Message + "; " + Exception.StackTrace;
                    _CswNbtResources.logError( new CswDniException( _CswScheduleLogicDetail.StatusMessage ) );
                    _LogicRunStatus = CswEnumScheduleLogicRunStatus.Failed;
                }//catch
            }//if we're not shutting down
        }//threadCallBack()

        public void stop()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Stopping;
        }

        public void reset()
        {
            _LogicRunStatus = CswEnumScheduleLogicRunStatus.Idle;
        }

        private bool _doesRequestItemCopyNow( CswNbtObjClassRequestItem CurrentRequestItem )
        {
            //A lot of these checks seem excessive...
            return null != CurrentRequestItem && // The Request Item isn't null
                   CurrentRequestItem.IsRecurring.Checked == CswEnumTristate.True && // This is actually a recurring request
                   false == CurrentRequestItem.RecurringFrequency.Empty && // The recurring frequency has been defined
                   ( CurrentRequestItem.RecurringFrequency.RateInterval.RateType != CswEnumRateIntervalType.Hourly || // Recurring on any frequency other than hourly
                   ( CurrentRequestItem.NextReorderDate.DateTimeValue.Date <= DateTime.Today && // Recurring no more than once per hour
                     DateTime.Now.AddHours( 1 ).Subtract( CurrentRequestItem.NextReorderDate.DateTimeValue ).Hours >= 1 ) );
        }
    }//CswScheduleLogicNbtGenRequests

}//namespace ChemSW.Nbt.Sched
