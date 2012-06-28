﻿using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Nbt.csw.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace NbtLogic.Test
{
    [TestClass]
    public class CswNbtActDispenseContainerTest
    {
        #region Setup and Teardown

        private TestData TestData = null;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            TestData.DeleteTestNodes();
        }

        #endregion

        [TestMethod]
        public void dispenseSourceContainerTestInvalidDispenseType()
        {
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "gal", 2.64172052, -1, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            string InvalidDispenseType = "Receive";
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            try
            {
                JObject obj = wiz.dispenseSourceContainer( InvalidDispenseType, "5", LiterNode.NodeId.ToString() );
                Assert.Fail( "Exception should have been thrown." );
            }
            catch( Exception e )
            {
                Assert.AreNotEqual( InvalidDispenseType, "Add", e.Message );
                Assert.AreNotEqual( InvalidDispenseType, "Waste", e.Message );
            }
        }

        [TestMethod]
        public void dispenseSourceContainerTestAdd()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            JObject obj = wiz.dispenseSourceContainer( DispenseType, ".5", LiterNode.NodeId.ToString() );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseSourceContainerTestAddBasicConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode MilliliterNode = TestData.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, 3, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "500", MilliliterNode.NodeId.ToString() );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseSourceContainerTestAddWeightToVolumeConversion()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.createMaterialNode( "Chemical", "Liquid", .1 );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, LiterNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode GramNode = TestData.createUnitOfMeasureNode( "Weight", "g", 1.0, 3, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "50", GramNode.NodeId.ToString() );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseSourceContainerTestAddVolumeToWeightConversion()
        {
            double Expected = 1.0;
            CswNbtNode KilogramNode = TestData.createUnitOfMeasureNode( "Weight", "kg", 1.0, 0, Tristate.True );
            CswNbtNode ChemicalNode = TestData.createMaterialNode( "Chemical", "Liquid", .1 );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 0.5, KilogramNode, ChemicalNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Add";
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            JObject obj = wiz.dispenseSourceContainer( DispenseType, "5", LiterNode.NodeId.ToString() );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseSourceContainerTestWaste()
        {
            double Expected = 1.0;
            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseType = "Waste";
            JObject obj = wiz.dispenseSourceContainer( DispenseType, ".5", LiterNode.NodeId.ToString() );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseIntoChildContainersTestWasteOne()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"numOfContainers\":\"0\", \"quantityToDispense\":\"0.5\", \"unitId\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseIntoChildContainersTestWasteTwo()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, 3, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 2.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"numOfContainers\":\"0\", \"quantityToDispense\":\"0.5\", \"unitId\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseRow2 = "{ \"numOfContainers\":\"0\", \"quantityToDispense\":\"500\", \"unitId\":\"" + MilliliterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "," + DispenseRow2 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseIntoChildContainersTestDispenseOne()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 1.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"numOfContainers\":\"1\", \"quantityToDispense\":\"0.5\", \"unitId\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 1, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseIntoChildContainersTestDispenseTwo()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode MilliliterNode = TestData.createUnitOfMeasureNode( "Volume", "Milliliters", 1.0, 3, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 2.0, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"numOfContainers\":\"1\", \"quantityToDispense\":\"0.5\", \"unitId\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseRow2 = "{ \"numOfContainers\":\"1\", \"quantityToDispense\":\"500\", \"unitId\":\"" + MilliliterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "," + DispenseRow2 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 2, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        [TestMethod]
        public void dispenseIntoChildContainersTestDispenseMany()
        {
            double Expected = 1.0;
            CswNbtMetaDataNodeType ContainerNT = TestData.CswNbtResources.MetaData.getNodeType( "Container" );
            string ContainerNodeTypeId = ContainerNT.NodeTypeId.ToString();

            CswNbtNode LiterNode = TestData.createUnitOfMeasureNode( "Volume", "Liters", 1.0, 0, Tristate.True );
            CswNbtNode ContainerNode = TestData.createContainerNode( "Container", 2.5, LiterNode );
            CswNbtActDispenseContainer wiz = new CswNbtActDispenseContainer( TestData.CswNbtResources, ContainerNode.NodeId.ToString() );
            string DispenseRow1 = "{ \"numOfContainers\":\"3\", \"quantityToDispense\":\"0.5\", \"unitId\":\"" + LiterNode.NodeId.ToString() + "\" }";
            string DispenseGrid = "[" + DispenseRow1 + "]";

            JObject obj = wiz.dispenseIntoChildContainers( ContainerNodeTypeId, DispenseGrid );

            Assert.AreEqual( Expected, _getNewSourceContainerQuantity( ContainerNode.NodeId ) );
            Assert.AreEqual( 3, _getNewContainerCount( ContainerNode.NodeId ) );
        }

        #region Private Helper Methods

        private double _getNewSourceContainerQuantity( CswPrimaryKey SourceContainerId )
        {
            CswNbtNode UpdatedContainerNode = TestData.CswNbtResources.Nodes.GetNode( SourceContainerId );
            CswNbtObjClassContainer NodeAsContianer = UpdatedContainerNode;
            return NodeAsContianer.Quantity.Quantity;
        }

        private int _getNewContainerCount( CswPrimaryKey SourceContainerId )
        {
            int NewContainerCount = 0;
            CswNbtMetaDataObjectClass ContainerOc = TestData.CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            IEnumerator CurrentNodes = TestData.CswNbtResources.Nodes.GetEnumerator();
            while( CurrentNodes.MoveNext() )
            {
                DictionaryEntry dentry = (DictionaryEntry) CurrentNodes.Current;
                CswNbtNode CurrentNode = (CswNbtNode) dentry.Value;
                if( CurrentNode.ObjClass.ObjectClass == ContainerOc )
                {
                    CswNbtObjClassContainer NewContainer = CurrentNode;
                    if( NewContainer.SourceContainer.RelatedNodeId == SourceContainerId )
                    {
                        NewContainerCount++;
                    }
                }
            }
            return NewContainerCount;
        }

        #endregion

    }
}
