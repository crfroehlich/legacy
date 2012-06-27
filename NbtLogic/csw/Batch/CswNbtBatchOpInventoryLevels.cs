﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Batch
{
    public class CswNbtBatchOpInventoryLevels : ICswNbtBatchOp
    {
        private CswNbtResources _CswNbtResources;
        private NbtBatchOpName _BatchOpName = NbtBatchOpName.InventoryLevel;

        public CswNbtBatchOpInventoryLevels( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        /// <summary>
        /// Returns the percentage of the task that is complete
        /// </summary>
        public Double getPercentDone( CswNbtObjClassBatchOp BatchNode )
        {
            Double ret = 0;
            if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.InventoryLevel )
            {
                ret = 100;
            }
            return ret;
        } // getPercentDone()

        /// <summary>
        /// Create a new batch operation to handle recalcution of inventory levels
        /// </summary>
        public CswNbtObjClassBatchOp makeBatchOp( CswPrimaryKey PrevLocation, CswPrimaryKey CurrentLocation )
        {
            CswNbtObjClassBatchOp BatchNode = null;
            if( null != PrevLocation && null != CurrentLocation && Int32.MinValue != PrevLocation.PrimaryKey && Int32.MinValue != CurrentLocation.PrimaryKey )
            {
                InventoryLevelsBatchData BatchData = new InventoryLevelsBatchData( PrevLocation, CurrentLocation );
                BatchNode = CswNbtBatchManager.makeNew( _CswNbtResources, _BatchOpName, BatchData );
            }
            return BatchNode;
        } // makeBatchOp()

        
        /// <summary>
        /// Run the next iteration of this batch operation
        /// </summary>
        public void runBatchOp( CswNbtObjClassBatchOp BatchNode )
        {
            try
            {
                if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.InventoryLevel )
                {
                    BatchNode.start();

                    InventoryLevelsBatchData BatchData = BatchNode.BatchData.Text;
                    CswNbtSdInventoryLevelMgr Mgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
                    Mgr.changeLocationOfLocation( BatchData.FromLocation, BatchData.ToLocation );
                    BatchNode.finish();
                    BatchNode.postChanges( false );
                } // if( BatchNode != null && BatchNode.OpNameValue == NbtBatchOpName.FutureNodes )
            }
            catch( Exception ex )
            {
                if( BatchNode != null ) BatchNode.error( ex );
            }
        } // runBatchOp()


        #region InventoryLevelsBatchData

        // This internal class is specific to this batch operation
        private class InventoryLevelsBatchData
        {
            public InventoryLevelsBatchData( CswPrimaryKey PrevLocation, CswPrimaryKey CurrentLocation )
            {
                FromLocation = PrevLocation;
                ToLocation = CurrentLocation;
            }

            public CswPrimaryKey FromLocation;
            public CswPrimaryKey ToLocation;

            public static implicit operator InventoryLevelsBatchData( string item )
            {
                JObject Obj = CswConvert.ToJObject( item );
                CswPrimaryKey PrevLocation = new CswPrimaryKey();
                PrevLocation.FromString( CswConvert.ToString( Obj["FromLocation"] ) );
                CswPrimaryKey CurrentLocation = new CswPrimaryKey();
                CurrentLocation.FromString( CswConvert.ToString( Obj["ToLocation"] ) );
                return new InventoryLevelsBatchData( PrevLocation, CurrentLocation );
            }

            public static implicit operator string( InventoryLevelsBatchData item )
            {
                return item.ToString();
            }

            public override string ToString()
            {
                JObject Obj = new JObject();
                Obj["FromLocation"] = FromLocation.ToString();
                Obj["ToLocation"] = ToLocation.ToString();
                return Obj.ToString();
            }
        } // class InventoryLevelsBatchData

        #endregion InventoryLevelsBatchData


    } // class CswNbtBatchOpInventoryLevels
} // namespace ChemSW.Nbt.Batch