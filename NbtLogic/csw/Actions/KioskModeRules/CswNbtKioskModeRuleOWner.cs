﻿using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions.KioskModeRules.OperationClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Search;

namespace ChemSW.Nbt.Actions.KioskMode
{
    public class CswNbtKioskModeRuleOwner: CswNbtKioskModeRule
    {
        public CswNbtKioskModeRuleOwner( CswNbtResources NbtResources )
            : base( NbtResources )
        {
            _CswNbtResources = NbtResources;
        }

        public override void SetFields( ref OperationData OpData )
        {
            base.SetFields( ref OpData );
            OpData.Field1.Name = "User:";
            OpData.Field2.Name = "Item:";
        }

        public override void ValidateFieldOne( ref OperationData OpData )
        {
            bool IsValid = _validateUser( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldOne( ref OpData );
            }
        }

        public override void ValidateFieldTwo( ref OperationData OpData )
        {
            bool IsValid = _validateItem( ref OpData );
            if( IsValid )
            {
                base.ValidateFieldTwo( ref OpData );
            }
        }

        public override void CommitOperation( ref OperationData OpData )
        {
            bool succeeded = false;
            CswNbtObjClassUser newOwner = _CswNbtResources.Nodes[OpData.Field1.NodeId];

            CswNbtNode node = _CswNbtResources.Nodes[OpData.Field2.NodeId];
            CswNbtMetaDataNodeType NodeType = node.getNodeType();
            string itemName = NodeType.NodeTypeName;
            string statusMsg = null;

            ICswNbtKioskModeOwnerable AsOwnerable = (ICswNbtKioskModeOwnerable) node.ObjClass;
            if( AsOwnerable.CanUpdateOwner( out statusMsg ) )
            {
                AsOwnerable.UpdateOwner( newOwner );
                succeeded = true;
            }
            
            if( null != node && succeeded )
            {
                node.postChanges( false );
                OpData.Log.Add( DateTime.Now + " - Changed owner of " + itemName + " " + OpData.Field2.Value + " to " + newOwner.Username + " (" + OpData.Field1.Value + ")" );
                base.CommitOperation( ref OpData );
            }
            else
            {
                statusMsg = statusMsg ?? "You do not have permission to edit " + itemName + " (" + OpData.Field2.Value + ")";
                OpData.Field2.StatusMsg = statusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + statusMsg );
            }
        }

        #region Private Functions

        private bool _validateUser( ref OperationData OpData )
        {
            bool ret = false;
            ICswNbtTree tree = _getTree( CswEnumNbtObjectClass.UserClass, OpData.Field1.Value, true );
            if( tree.getChildNodeCount() > 0 )
            {
                tree.goToNthChild( 0 );
                CswNbtObjClassUser usernode = tree.getNodeForCurrentPosition();
                if( null == usernode.DefaultLocationId )
                {
                    OpData.Field1.StatusMsg = "User with barcode: " + OpData.Field1.Value + " does not have a Location set and cannot be used as a Transfer target ";
                    OpData.Field1.ServerValidated = false;
                    OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
                }
                else
                {
                    OpData.Field1.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                ret = true;
            }
            }
            else
            {
                OpData.Field1.StatusMsg = "Could not find a User with barcode " + OpData.Field1.Value;
                OpData.Field1.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + OpData.Field1.StatusMsg );
            }
            return ret;
        }

        private bool _validateItem( ref OperationData OpData )
        {
            bool ret = false;

            CswNbtSearch search = new CswNbtSearch( _CswNbtResources )
            {
                SearchTerm = OpData.Field2.Value,
                SearchType = CswEnumSqlLikeMode.Exact
            };
            ICswNbtTree tree = search.Results();

            int childCount = tree.getChildNodeCount();
            for( int i = 0; i < childCount; i++ )
            {
                tree.goToNthChild( i );
                CswNbtNode node = tree.getNodeForCurrentPosition();
                CswNbtMetaDataNodeType nodeType = node.getNodeType();
                CswNbtMetaDataNodeTypeProp barcodeProp = (CswNbtMetaDataNodeTypeProp) nodeType.getBarcodeProperty();

                if( null != barcodeProp )
                {
                    string barcodeValue = node.Properties[barcodeProp].AsBarcode.Barcode;
                    string ObjClass = node.ObjClass.ObjectClass.ObjectClass;

                    if( string.Equals( barcodeValue, OpData.Field2.Value, StringComparison.CurrentCultureIgnoreCase ) )
                    {
                        if( ObjClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentAssemblyClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                ret = true;
            }

                        if( ObjClass == CswEnumNbtObjectClass.EquipmentClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.EquipmentClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }

                        if( ObjClass == CswEnumNbtObjectClass.ContainerClass )
                        {
                            OpData.Field2.FoundObjClass = CswEnumNbtObjectClass.ContainerClass;
                            OpData.Field2.NodeIdStr = tree.getNodeIdForCurrentPosition().ToString();
                            ret = true;
                        }
                    }
                }
                tree.goToParentNode();
            }

            if( string.IsNullOrEmpty( OpData.Field2.FoundObjClass ) )
            {
                string StatusMsg = "";
                bool first = true;
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
                {
                    StatusMsg = CswEnumNbtObjectClass.ContainerClass.Replace( "Class", "" );
                    first = false;
                }
                if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.IMCS ) )
                {
                    if( first )
                    {
                        StatusMsg = CswEnumNbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
            else
            {
                        StatusMsg += ", " + CswEnumNbtObjectClass.EquipmentClass.Replace( "Class", "" );
                    }
                    StatusMsg += " or " + CswEnumNbtObjectClass.EquipmentAssemblyClass.Replace( "Class", "" );
                }
                StatusMsg = "Could not find " + StatusMsg + " with barcode " + OpData.Field2.Value;

                OpData.Field2.StatusMsg = StatusMsg;
                OpData.Field2.ServerValidated = false;
                OpData.Log.Add( DateTime.Now + " - ERROR: " + StatusMsg );
            }

            return ret;
        }

        #endregion
    }
}
