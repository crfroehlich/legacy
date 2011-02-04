﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using System.Xml;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceView
    {
        public enum ItemType
        {
            Root,
            View,
            //ViewCategory, 
            Category,
            Action,
            Report,
            //ReportCategory, 
            Search,
            RecentView,
            Unknown
        };

        private CswNbtResources _CswNbtResources;
        private const string QuickLaunchViews = "QuickLaunchViews";

        public CswNbtWebServiceView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        //public string getViews()
        //{
        //    string ret = string.Empty;
        //    DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false );
        //    foreach( DataRow ViewRow in ViewDT.Rows )
        //    {
        //        ret += "<view id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\"";
        //        ret += " name=\"" + ViewRow["viewname"].ToString() + "\"";
        //        ret += "/>";
        //    }
        //    return "<views>" + ret + "</views>";
        //}

        public const string ViewTreeSessionKey = "ViewTreeXml";

        // jsTree compatible format
        public string getViewTree( HttpSessionState Session )
        {

            //string ret = string.Empty;
            //DataTable ViewDT = _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser, false, false );
            //foreach( DataRow ViewRow in ViewDT.Rows )
            //{
            //    ret += "<item id=\"" + CswConvert.ToInt32( ViewRow["nodeviewid"] ) + "\" rel=\"" + ViewRow["viewmode"].ToString() + "\" >";
            //    ret += "  <content><name>" + ViewRow["viewname"].ToString() + "</name></content>";
            //    ret += "</item>";
            //}
            //return "<root>" + ret + "</root>";

            XmlDocument TreeXmlDoc;
            if( Session[ViewTreeSessionKey] != null )
            {
                TreeXmlDoc = (XmlDocument) Session[ViewTreeSessionKey];
            }
            else
            {
                TreeXmlDoc = new XmlDocument();
                XmlNode DocRoot = TreeXmlDoc.CreateElement( "root" );
                TreeXmlDoc.AppendChild( DocRoot );

                // Views
                DataTable ViewsTable = _CswNbtResources.ViewSelect.getVisibleViews( "lower(NVL(v.category, v.viewname)), lower(v.viewname)", _CswNbtResources.CurrentNbtUser, false, false );

                foreach( DataRow Row in ViewsTable.Rows )
                {
                    // BZ 10121
                    // This is a performance hit, but since this view list is cached, it's ok
                    CswNbtView CurrentView = new CswNbtView( _CswNbtResources );
                    CurrentView.LoadXml( Row["viewxml"].ToString() );
                    CurrentView.ViewId = CswConvert.ToInt32( Row["nodeviewid"] );

                    _makeViewTreeNode( DocRoot, Row["category"].ToString(), ItemType.View, CurrentView.ViewMode, CurrentView.ViewId, CurrentView.ViewName );
                }

                // Actions
                foreach( CswNbtAction Action in _CswNbtResources.Actions )
                {
                    if( Action.ShowInList &&
                        ( Action.Name != CswNbtActionName.View_By_Location || _CswNbtResources.getConfigVariableValue( "loc_use_images" ) != "0" ) &&
                            ( (CswNbtObjClassUser) _CswNbtResources.CurrentNbtUser ).CheckActionPermission( Action.Name ) )
                    {
                        _makeViewTreeNode( DocRoot, Action.Category, ItemType.Action, NbtViewRenderingMode.Unknown, Action.ActionId, Action.DisplayName );
                    }
                }


                // Reports
                CswNbtMetaDataObjectClass ReportMetaDataObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
                CswNbtView ReportView = ReportMetaDataObjectClass.CreateDefaultView();
                ReportView.ViewName = "CswViewTree.DataBinding.ReportView";
                ICswNbtTree ReportTree = _CswNbtResources.Trees.getTreeFromView( ReportView, true, true, false, false );
                for( int i = 0; i < ReportTree.getChildNodeCount(); i++ )
                {
                    ReportTree.goToNthChild( i );

                    CswNbtObjClassReport ReportNode = CswNbtNodeCaster.AsReport( ReportTree.getNodeForCurrentPosition() );
                    _makeViewTreeNode( DocRoot, ReportNode.Category.Text, ItemType.Report, NbtViewRenderingMode.Unknown, ReportNode.NodeId.PrimaryKey, ReportNode.ReportName.Text );

                    ReportTree.goToParentNode();
                }
                
                Session[ViewTreeSessionKey] = TreeXmlDoc;

            }

            string ret = "<result>" +
                         "  <tree>" + TreeXmlDoc.InnerXml + "</tree>" +
                         "  <types>{ " + _getTypes() + " }</types>" +
                         "</result>";

            return ret;
        } // getViewTree()

        private void _makeViewTreeNode( XmlNode DocRoot, string Category, ItemType Type, NbtViewRenderingMode ViewMode, Int32 Id, string Text )
        {
            XmlNode CategoryNode = _getCategoryNode( DocRoot, Category );
            _makeItemNode( CategoryNode, Type, ViewMode, Id, Text );
        }

        private XmlNode _makeItemNode( XmlNode ParentNode, ItemType Type, NbtViewRenderingMode ViewMode, Int32 Id, string Text )
        {
            XmlNode ItemNode = CswXmlDocument.AppendXmlNode( ParentNode, "item" );
            XmlNode ContentNode = CswXmlDocument.AppendXmlNode( ItemNode, "content" );
            XmlNode NameNode = CswXmlDocument.AppendXmlNode( ContentNode, "name" );
            NameNode.InnerText = Text;

            string rel = Type.ToString().ToLower();
            if( Type == ItemType.View )
                rel = Type.ToString().ToLower() + ViewMode.ToString().ToLower();
            CswXmlDocument.AppendXmlAttribute( ItemNode, "rel", rel );
            CswXmlDocument.AppendXmlAttribute( ItemNode, "id", rel + "_" + Id.ToString() );

            return ItemNode;
        }


        private Int32 _catcount = 0;
        private Dictionary<string, XmlNode> CategoryNodes = new Dictionary<string, XmlNode>();

        private XmlNode _getCategoryNode( XmlNode DocRoot, string Category )
        {
            XmlNode CategoryNode = null;
            if( Category != string.Empty )
            {
                if( CategoryNodes.ContainsKey( Category ) )
                {
                    CategoryNode = CategoryNodes[Category];
                }
                else
                {
                    // Make one
                    _catcount++;
                    CategoryNode = _makeItemNode( DocRoot, ItemType.Category, NbtViewRenderingMode.Unknown, _catcount, Category );
                    CategoryNodes.Add( Category, CategoryNode );
                }
            }
            else
            {
                CategoryNode = DocRoot;
            }
            return CategoryNode;
        } // _getCategoryNode()


        public string _getTypes()
        {
            string ret = "\"default\": \"\"";

            string[] types = { "action", "category", "report", "viewtree", "viewgrid", "viewlist" };
            foreach( string type in types )
            {
                ret += @",""" + type + @""": {
                       ""icon"": {
                         ""image"": ""Images/view/" + type + @".gif""
                       }
                     }";
            }
            return ret;
        }




    } // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
