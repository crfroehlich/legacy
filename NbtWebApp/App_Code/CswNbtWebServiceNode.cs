﻿using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Statistics;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNode
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtStatisticsEvents _CswNbtStatisticsEvents;
        public CswNbtWebServiceNode( CswNbtResources CswNbtResources, CswNbtStatisticsEvents CswNbtStatisticsEvents )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtStatisticsEvents = CswNbtStatisticsEvents;
        }

        public CswPrimaryKey CopyNode( CswPrimaryKey NodePk )
        {
            CswNbtNode OriginalNode = _CswNbtResources.Nodes[NodePk];

            CswNbtNode NewNode = null;
            CswNbtActCopyNode CswNbtActCopyNode = new CswNbtActCopyNode( _CswNbtResources );
            switch( OriginalNode.ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass:
                    NewNode = CswNbtActCopyNode.CopyEquipmentNode( OriginalNode );
                    break;
                case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass:
                    NewNode = CswNbtActCopyNode.CopyEquipmentAssemblyNode( OriginalNode );
                    break;
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass:
                    NewNode = CswNbtActCopyNode.CopyInspectionTargetNode( OriginalNode );
                    break;
                default:
                    NewNode = CswNbtActCopyNode.CopyNode( OriginalNode );
                    break;
            }

            if( NewNode != null )
            {
                _CswNbtStatisticsEvents.OnCopyNode( OriginalNode, NewNode );
            }
            return NewNode.NodeId;
        }

        public bool DeleteNode( CswPrimaryKey NodePk )
        {
            bool ret = false;

            CswNbtNode NodeToDelete = _CswNbtResources.Nodes[NodePk];
            NodeToDelete.delete();
            ret = true;

            return ret;
        }

        public bool doObjectClassButtonClick( CswPropIdAttr PropId )
        {
            bool RetSuccess;

            if( Int32.MinValue == PropId.NodeTypePropId ||
                    null == PropId.NodeId ||
                        Int32.MinValue == PropId.NodeId.PrimaryKey )
            {
                throw new CswDniException( ErrorType.Error, "Cannot execute a button click without valid parameters.", "Attempted to call DoObjectClassButtonClick with invalid NodeId and NodeTypePropId." );
            }

            CswNbtNode Node = _CswNbtResources.Nodes.GetNode( PropId.NodeId );
            if( null == Node )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid node with the provided parameters.", "No node exists for NodePk " + PropId.NodeId.ToString() + "." );
            }

            CswNbtMetaDataNodeTypeProp NodeTypeProp = Node.NodeType.getNodeTypeProp( PropId.NodeTypePropId );
            if( null == NodeTypeProp )
            {
                throw new CswDniException( ErrorType.Error, "Cannot find a valid property with the provided parameters.", "No property exists for NodeTypePropId " + PropId.NodeTypePropId.ToString() + "." );
            }

            CswNbtObjClass NbtObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, Node.ObjectClass.ObjectClassId, Node );
            NbtObjClass.onButtonClick( NodeTypeProp );
            RetSuccess = true;

            return RetSuccess;
        }

    } // class CswNbtWebServiceNode

} // namespace ChemSW.Nbt.WebServices
