using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;
using ChemSW.Nbt.MetaData;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {
        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceKioskMode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private sealed class Modes
        {
            public const string Move = "move";
            public const string Owner = "owner";
            public const string Transfer = "transfer";
            public const string Dispense = "dispense";
            public const string Dispose = "dispose";
            public static readonly CswCommaDelimitedString All = new CswCommaDelimitedString()
            {
                Move, Owner, Transfer, Dispense, Dispose
            };
        }

        #region Data Contracts

        [DataContract]
        public class KioskModeDataReturn : CswWebSvcReturn
        {
            public KioskModeDataReturn()
            {
                Data = new KioskModeData();
            }
            [DataMember]
            public KioskModeData Data;
        }

        [DataContract]
        public class KioskModeData
        {
            [DataMember]
            public Collection<Mode> AvailableModes = new Collection<Mode>();
            [DataMember]
            public OperationData OperationData;
        }

        [DataContract]
        public class Mode
        {
            [DataMember]
            public string name = string.Empty;
            [DataMember]
            public string imgUrl = string.Empty;
        }

        [DataContract]
        public class ActiveMode
        {
            [DataMember]
            public string field1Name = string.Empty;
            [DataMember]
            public string field2Name = string.Empty;
        }

        [DataContract]
        public class OperationData
        {
            [DataMember]
            public string Mode = string.Empty;
            [DataMember]
            public string ModeStatusMsg = string.Empty;
            [DataMember]
            public bool ModeServerValidated = false;
            [DataMember]
            public Collection<string> Log = new Collection<string>();
            [DataMember]
            public Field Field1;
            [DataMember]
            public Field Field2;
            [DataMember]
            public string LastItemScanned;
        }

        [DataContract]
        public class Field
        {
            [DataMember]
            public string Name = string.Empty;
            [DataMember]
            public string Value = string.Empty;
            [DataMember]
            public string StatusMsg = string.Empty;
            [DataMember]
            public bool ServerValidated = false;
            [DataMember]
            public string SecondValue = string.Empty;

            public NbtObjectClass ExpectedObjClass( string OpMode, int FieldNumber )
            {
                NbtObjectClass Ret = NbtObjectClass.LocationClass;
                OpMode = OpMode.ToLower();
                switch( OpMode )
                {
                    case Modes.Move:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.LocationClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Owner:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Transfer:
                        if( FieldNumber == 1 )
                        {
                            Ret = NbtObjectClass.UserClass;
                        }
                        else
                        {
                            Ret = NbtObjectClass.ContainerClass;
                        }
                        break;
                    case Modes.Dispense:
                        //TODO: dispense container
                        break;
                    case Modes.Dispose:
                        //TODO: dispose container
                        break;
                }
                return Ret;
            }
        }

        #endregion

        public static void GetAvailableModes( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassRole currentUserRoleNode = NbtResources.Nodes.makeRoleNodeFromRoleName( NbtResources.CurrentNbtUser.Rolename );

            KioskModeData kioskModeData = new KioskModeData();

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( Modes.Move ),
                imgUrl = "Images/newicons/KioskMode/Move_code39.png"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( Modes.Owner ),
                imgUrl = "Images/newicons/KioskMode/Owner_code39.png"
            } );
            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( Modes.Transfer ),
                imgUrl = "Images/newicons/KioskMode/Transfer_code39.png"
            } );
            CswNbtPermit permissions = new CswNbtPermit( NbtResources );
            if( permissions.can( CswNbtActionName.DispenseContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Dispense ),
                    imgUrl = "Images/newicons/KioskMode/Dispense_code39.png"
                } );
            }
            if( permissions.can( CswNbtActionName.DisposeContainer ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Dispose ),
                    imgUrl = "Images/newicons/KioskMode/Dispose_code39.png"
                } );
            }

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Reset",
                imgUrl = "Images/newicons/KioskMode/Reset_code39.png"
            } );

            Return.Data = kioskModeData;
        }

        public static void HandleScan( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( _isModeScan( KioskModeData.OperationData.LastItemScanned ) )
            {
                KioskModeData.OperationData.Mode = KioskModeData.OperationData.LastItemScanned;
                _setFields( KioskModeData.OperationData );
            }
            else
            {
                if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field2.Value ) && false == KioskModeData.OperationData.Field2.ServerValidated )
                {
                    NbtObjectClass expectedOC = KioskModeData.OperationData.Field2.ExpectedObjClass( KioskModeData.OperationData.Mode, 2 );
                    bool barcodeValid = _validateBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field2.Value );
                    if( false == barcodeValid )
                    {
                        string itemType = expectedOC.Value.Replace( "Class", "" );
                        KioskModeData.OperationData.Field2.StatusMsg = "Error: " + itemType + " does not exist";
                    }
                    else
                    {
                        KioskModeData.OperationData.Field2.ServerValidated = true;
                        KioskModeData.OperationData.Field2.StatusMsg = "";
                    }
                }
                else if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field1.Value ) && false == KioskModeData.OperationData.Field1.ServerValidated )
                {
                    NbtObjectClass expectedOC = KioskModeData.OperationData.Field2.ExpectedObjClass( KioskModeData.OperationData.Mode, 1 );
                    bool barcodeValid = _validateBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field1.Value );
                    if( false == barcodeValid )
                    {
                        string itemType = expectedOC.Value.Replace( "Class", "" );
                        KioskModeData.OperationData.Field1.StatusMsg = "Error: " + itemType + " does not exist";
                    }
                    else
                    {
                        KioskModeData.OperationData.Field1.ServerValidated = true;
                        KioskModeData.OperationData.Field1.StatusMsg = "";
                        if( expectedOC.Equals( NbtObjectClass.LocationClass ) )
                        {
                            CswNbtObjClassLocation scannedLocation = _getNodeByBarcode( NbtResources, expectedOC, KioskModeData.OperationData.Field2.Value );
                            KioskModeData.OperationData.Field1.SecondValue = " (" + scannedLocation.Name.Text + ")";
                        }
                    }
                }
                else
                {
                    KioskModeData.OperationData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    KioskModeData.OperationData.ModeServerValidated = false;
                }
            }
            Return.Data = KioskModeData;
        }

        public static void CommitOperation( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            OperationData OpData = KioskModeData.OperationData;
            string mode = OpData.Mode.ToLower();
            switch( mode )
            {
                case Modes.Move:
                    CswNbtObjClassContainer containerToMove = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassLocation locationToMoveTo = _getNodeByBarcode( NbtResources, NbtObjectClass.LocationClass, OpData.Field1.Value );
                    containerToMove.MoveContainer( locationToMoveTo.NodeId );
                    containerToMove.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Moved container " + OpData.Field2.Value + " to " + locationToMoveTo.Name.Text + " (" + OpData.Field1.Value + ")" );
                    break;
                case Modes.Owner:
                    CswNbtObjClassContainer containerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassUser newOwnerNode = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value );
                    containerNode.Owner.RelatedNodeId = newOwnerNode.NodeId;
                    containerNode.Owner.RefreshNodeName();
                    containerNode.postChanges( false );
                    OpData.Log.Add( DateTime.Now + " - Changed owner of container " + OpData.Field2.Value + " to " + newOwnerNode.FirstName + " " + newOwnerNode.LastName + " (" + OpData.Field1.Value + ")" );
                    break;
                case Modes.Transfer:
                    CswNbtObjClassContainer containerToTransfer = _getNodeByBarcode( NbtResources, NbtObjectClass.ContainerClass, OpData.Field2.Value );
                    CswNbtObjClassUser newTransferOwner = _getNodeByBarcode( NbtResources, NbtObjectClass.UserClass, OpData.Field1.Value );
                    containerToTransfer.Owner.RelatedNodeId = newTransferOwner.NodeId;
                    containerToTransfer.Owner.RefreshNodeName();
                    containerToTransfer.MoveContainer( newTransferOwner.DefaultLocationId );
                    containerToTransfer.postChanges( false );
                    CswNbtObjClassLocation newLocationNode = NbtResources.Nodes[newTransferOwner.DefaultLocationId];
                    OpData.Log.Add( DateTime.Now + " - Transferred container " + OpData.Field2.Value + " ownership to " + newTransferOwner.FirstName + " " + newTransferOwner.LastName + " (" + OpData.Field1.Value + ") at " + newLocationNode.Name.Text );
                    break;
                case Modes.Dispense:
                    //TODO: dispense container
                    break;
                case Modes.Dispose:
                    //TODO: dispose container
                    break;
            }
            OpData.Field2.Value = string.Empty;
            OpData.Field2.SecondValue = string.Empty;
            OpData.Field2.ServerValidated = false;
            KioskModeData.OperationData = OpData;
            Return.Data = KioskModeData;
        }

        #region Private Methods

        private static void _setFields( OperationData OpData )
        {
            OpData.ModeServerValidated = true;
            OpData.ModeStatusMsg = string.Empty;
            OpData.Field1 = new Field();
            OpData.Field2 = new Field();
            string selectedOp = OpData.Mode.ToLower();
            switch( selectedOp )
            {
                case Modes.Move:
                    OpData.Field1.Name = "Location:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Owner:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Transfer:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Dispense:
                    //TODO: dispense container
                    break;
                case Modes.Dispose:
                    //TODO: dispose container
                    break;
                default:
                    OpData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    OpData.ModeServerValidated = false;
                    break;
            }
        }

        private static bool _validateBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            return childCount > 0 ? true : false; //true if the item with this barcode exists
        }

        private static CswNbtNode _getNodeByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswNbtNode Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeForCurrentPosition();
            }
            return Ret;
        }

        private static CswPrimaryKey _getNodeIdByBarcode( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswPrimaryKey Ret = null;
            ICswNbtTree tree = _getTree( NbtResources, ObjClass, Barcode );
            int childCount = tree.getChildNodeCount();
            if( childCount > 0 )
            {
                tree.goToNthChild( 0 );
                Ret = tree.getNodeIdForCurrentPosition();
            }
            return Ret;
        }

        private static ICswNbtTree _getTree( CswNbtResources NbtResources, NbtObjectClass ObjClass, string Barcode )
        {
            CswNbtMetaDataObjectClass metaDataOC = NbtResources.MetaData.getObjectClass( ObjClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = metaDataOC.getBarcodeProp();
            CswNbtView view = new CswNbtView( NbtResources );
            CswNbtViewRelationship parent = view.AddViewRelationship( metaDataOC, true );
            view.AddViewPropertyAndFilter( parent,
                MetaDataProp: barcodeOCP,
                Value: Barcode,
                SubFieldName: CswNbtSubField.SubFieldName.Barcode,
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals
            );

            ICswNbtTree tree = NbtResources.Trees.getTreeFromView( view, true, false, false );
            return tree;
        }

        private static bool _isModeScan( string ScannedMode )
        {
            bool Ret = false;
            foreach( string mode in Modes.All )
            {
                if( mode.Equals( ScannedMode.ToLower() ) )
                {
                    Ret = true;
                }
            }
            return Ret;
        }

        #endregion
    }

}