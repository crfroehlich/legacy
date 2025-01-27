﻿using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchManager
    {
        /// <summary>
        /// If an operation affects this number of nodes, run as a batch operation instead
        /// </summary>
        public static Int32 getBatchThreshold( CswNbtResources CswNbtResources )
        {
            Int32 ret = CswConvert.ToInt32( CswNbtResources.ConfigVbls.getConfigVariableValue( "batchthreshold" ) );
            if( Int32.MinValue == ret )
            {
                ret = 10;
            }
            return ret;
        }

        /// <summary>
        /// Restore an existing batch row from the database
        /// </summary>
        public static CswNbtObjClassBatchOp restore( CswNbtResources CswNbtResources, CswPrimaryKey BatchId )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( BatchId != null && BatchId.PrimaryKey != Int32.MinValue )
            {
                CswNbtNode Node = CswNbtResources.Nodes[BatchId];
                if( Node != null )
                {
                    BatchNode = Node;
                }
            }
            return BatchNode;
        } // restore()

        /// <summary>
        /// Makes a new batch operation instance in the database
        /// </summary>
        public static CswNbtObjClassBatchOp makeNew( CswNbtResources CswNbtResources,
                                                     CswEnumNbtBatchOpName BatchOpName,
                                                     string BatchData,
                                                     CswPrimaryKey UserId = null,
                                                     Double Priority = Double.NaN )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass );
            if( BatchOpOC != null )
            {
                CswNbtMetaDataNodeType BatchOpNT = BatchOpOC.getNodeTypes().First();
                if( BatchOpNT != null )
                {
                    CswNbtNode Node = CswNbtResources.Nodes.makeNodeFromNodeTypeId( BatchOpNT.NodeTypeId );
                    BatchNode = Node;

                    BatchNode.BatchData.Text = BatchData;
                    BatchNode.CreatedDate.DateTimeValue = DateTime.Now;
                    BatchNode.OpName.Text = BatchOpName.ToString();
                    if( false == Double.IsNaN( Priority ) )
                    {
                        BatchNode.Priority.Value = Priority;
                    }
                    BatchNode.Status.Value = CswEnumNbtBatchOpStatus.Pending.ToString();
                    BatchNode.User.RelatedNodeId = UserId ?? CswNbtResources.CurrentNbtUser.UserId;

                    BatchNode.postChanges( true );
                }
            }
            return BatchNode;
        } // makeNew()

        public static void runNextBatchOp( CswNbtResources CswNbtResources )
        {
            ICswNbtTree BatchOpTree = _getPendingBatchOpsTree( CswNbtResources );

            if( BatchOpTree.getChildNodeCount() > 0 )
            {
                BatchOpTree.goToNthChild( 0 );
                CswNbtNode Node = BatchOpTree.getNodeForCurrentPosition();
                CswNbtObjClassBatchOp BatchNode = Node;

                CswEnumNbtBatchOpName OpName = BatchNode.OpNameValue;
                ICswNbtBatchOp op = null;
                if( OpName == CswEnumNbtBatchOpName.BulkEdit )
                {
                    op = new CswNbtBatchOpBulkEdit( CswNbtResources );
                }
                if( OpName == CswEnumNbtBatchOpName.FutureNodes )
                {
                    op = new CswNbtBatchOpFutureNodes( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.MultiEdit )
                {
                    op = new CswNbtBatchOpMultiEdit( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.MultiButtonClick )
                {
                    op = new CswNbtBatchOpMultiButtonClick( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.InventoryLevel )
                {
                    op = new CswNbtBatchOpInventoryLevels( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.MultiDelete )
                {
                    op = new CswNbtBatchOpMultiDelete( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.SyncLocation )
                {
                    op = new CswNbtBatchOpSyncLocation( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.MobileMultiOpUpdates )
                {
                    op = new CswNbtBatchOpMobileMultiOpUpdates( CswNbtResources );
                }
                else if( OpName == CswEnumNbtBatchOpName.Receiving )
                {
                    op = new CswNbtBatchOpReceiving( CswNbtResources );
                }

                // New batch ops go here
                // else if( OpName == NbtBatchOpName.NEWNAME ) 
                if( null != op )
                {

                    CswNbtNode UserNode = CswNbtResources.Nodes[BatchNode.User.RelatedNodeId];
                    if( null != UserNode )
                    {
                        CswNbtObjClassUser UserOC = UserNode;
                        if( null != UserOC )
                        {
                            CswNbtResources.AuditContext = "Batch Op: " + BatchNode.OpNameValue;
                            CswNbtResources.AuditFirstName = UserOC.FirstName;
                            CswNbtResources.AuditLastName = UserOC.LastName;
                            CswNbtResources.AuditUsername = UserOC.Username;
                        }
                    }

                    op.runBatchOp( BatchNode );
                }
            }
        }

        public static Int32 getBatchNodeCount( CswNbtResources CswNbtResources )
        {
            ICswNbtTree BatchOpTree = _getPendingBatchOpsTree( CswNbtResources );
            return BatchOpTree.getChildNodeCount();
        }

        private static ICswNbtTree _getPendingBatchOpsTree( CswNbtResources CswNbtResources )
        {
            CswNbtMetaDataObjectClass BatchOpOC = CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BatchOpClass );
            CswNbtMetaDataObjectClassProp StatusOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.Status );
            CswNbtMetaDataObjectClassProp PriorityOCP = BatchOpOC.getObjectClassProp( CswNbtObjClassBatchOp.PropertyName.Priority );


            CswNbtView NextBatchOpView = new CswNbtView( CswNbtResources );
            CswNbtViewRelationship BatchVR = NextBatchOpView.AddViewRelationship( BatchOpOC, false );
            CswNbtViewProperty StatusVP = NextBatchOpView.AddViewProperty( BatchVR, StatusOCP );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumNbtBatchOpStatus.Completed.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumNbtBatchOpStatus.Error.ToString() );
            NextBatchOpView.AddViewPropertyFilter( StatusVP, FilterMode: CswEnumNbtFilterMode.NotEquals, Value: CswEnumNbtBatchOpStatus.Unknown.ToString() );
            CswNbtViewProperty PriorityVP = NextBatchOpView.AddViewProperty( BatchVR, PriorityOCP );
            NextBatchOpView.setSortProperty( PriorityVP, CswEnumNbtViewPropertySortMethod.Descending );

            ICswNbtTree BatchOpTree = CswNbtResources.Trees.getTreeFromView( NextBatchOpView, false, true, false );
            return BatchOpTree;
        }

    } // class CswNbtBatchManager
} // namespace ChemSW.Nbt.Batch
