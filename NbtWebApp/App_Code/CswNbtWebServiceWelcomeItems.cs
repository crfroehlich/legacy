﻿using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using System.Xml;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// Webservice for the table of components on the Welcome page
	/// </summary>
	public class CswNbtWebServiceWelcomeItems : CompositeControl
	{
		private CswNbtResources _CswNbtResources;

		public CswNbtWebServiceWelcomeItems( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;

		}

		/// <summary>
		/// Folder Path for Button Images
		/// </summary>
		public static string IconImageRoot = "Images/biggerbuttons";

		/// <summary>
		/// Types of Welcome Page Components
		/// </summary>
		public enum WelcomeComponentType
		{
			/// <summary>
			/// Link to a View, Report, or Action
			/// </summary>
			Link,
			/// <summary>
			/// Search on a View
			/// </summary>
			Search,
			/// <summary>
			/// Static text
			/// </summary>
			Text,
			/// <summary>
			/// Add link for new node
			/// </summary>
			Add
		}


		private DataTable _getWelcomeTable( Int32 RoleId )
		{
			CswTableSelect WelcomeSelect = _CswNbtResources.makeCswTableSelect( "WelcomeSelect", "welcome" );
			string WhereClause = "where roleid = '" + RoleId.ToString() + "'";
			Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
			OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
			OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
			OrderBy.Add( new OrderByClause( "welcomeid", OrderByType.Ascending ) );
			return WelcomeSelect.getTable( WhereClause, OrderBy );
		} // _getWelcomeTable()


		public string GetWelcomeItems( string strRoleId )
		{
			string ret = string.Empty;
			var ReturnXML = new XmlDocument();
			XmlNode WelcomeNode = CswXmlDocument.SetDocumentElement( ReturnXML, "welcome" );

			CswPrimaryKey RolePk = new CswPrimaryKey();
			RolePk.FromString( strRoleId );
			Int32 RoleId = RolePk.PrimaryKey;

			// Welcome components from database
			DataTable WelcomeTable = _getWelcomeTable( RoleId );

			// see BZ 10234
			if( WelcomeTable.Rows.Count == 0 )
			{
				ResetWelcomeItems( strRoleId );
				WelcomeTable = _getWelcomeTable( RoleId );
			}

			foreach( DataRow WelcomeRow in WelcomeTable.Rows )
			{
				XmlNode ItemNode = CswXmlDocument.AppendXmlNode( WelcomeNode, "item" );
				CswXmlDocument.AppendXmlAttribute( ItemNode, "welcomeid", WelcomeRow["welcomeid"].ToString() );

				WelcomeComponentType ThisType = (WelcomeComponentType) Enum.Parse( typeof( WelcomeComponentType ), WelcomeRow["componenttype"].ToString(), true );
				string LinkText = string.Empty;

				switch( ThisType )
				{
					case WelcomeComponentType.Add:
						if( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) != Int32.MinValue )
						{
							CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( WelcomeRow["nodetypeid"] ) );
							if( WelcomeRow["displaytext"].ToString() != string.Empty )
								LinkText = WelcomeRow["displaytext"].ToString();
							else
								LinkText = "Add New " + NodeType.NodeTypeName;
							CswXmlDocument.AppendXmlAttribute( ItemNode, "nodetypeid", WelcomeRow["nodetypeid"].ToString() );
							CswXmlDocument.AppendXmlAttribute( ItemNode, "type", "add_new_nodetype" );
						}
						break;

					case WelcomeComponentType.Link:
                        if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
						{
							CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
							if( null != ThisView && ThisView.IsFullyEnabled() )
							{
                                if( WelcomeRow["displaytext"].ToString() != string.Empty )
                                {
                                    LinkText = WelcomeRow["displaytext"].ToString();
                                }
                                else
                                {
                                    LinkText = ThisView.ViewName;
                                }
								CswXmlDocument.AppendXmlAttribute( ItemNode, "viewid", new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString() );
								CswXmlDocument.AppendXmlAttribute( ItemNode, "viewmode", ThisView.ViewMode.ToString().ToLower() );
								CswXmlDocument.AppendXmlAttribute( ItemNode, "type", "view" );
							}
						}
						if( CswConvert.ToInt32( WelcomeRow["actionid"] ) != Int32.MinValue )
						{
							CswNbtAction ThisAction = _CswNbtResources.Actions[CswConvert.ToInt32( WelcomeRow["actionid"] )];
							if( _CswNbtResources.Permit.can( ThisAction.Name ) )
							{
								if( WelcomeRow["displaytext"].ToString() != string.Empty )
									LinkText = WelcomeRow["displaytext"].ToString();
								else
									LinkText = ThisAction.Name.ToString();
							}
							CswXmlDocument.AppendXmlAttribute( ItemNode, "actionid", WelcomeRow["actionid"].ToString() );
							CswXmlDocument.AppendXmlAttribute( ItemNode, "type", "action" );
						}
						if( CswConvert.ToInt32( WelcomeRow["reportid"] ) != Int32.MinValue )
						{
							CswNbtNode ThisReportNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( WelcomeRow["reportid"] ) )];
							if( WelcomeRow["displaytext"].ToString() != string.Empty )
								LinkText = WelcomeRow["displaytext"].ToString();
							else
								LinkText = ThisReportNode.NodeName;
							CswXmlDocument.AppendXmlAttribute( ItemNode, "reportid", WelcomeRow["reportid"].ToString() );
							CswXmlDocument.AppendXmlAttribute( ItemNode, "type", "report" );
						}
						break;
					case WelcomeComponentType.Search:
						if( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) != Int32.MinValue )
						{
							CswNbtView ThisView = _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ) );
							if( null != ThisView && ThisView.IsSearchable() )
							{
                                if( WelcomeRow["displaytext"].ToString() != string.Empty )
                                {
                                    LinkText = WelcomeRow["displaytext"].ToString();
                                }
                                else
                                {
                                    LinkText = ThisView.ViewName;
                                }
								CswXmlDocument.AppendXmlAttribute( ItemNode, "viewid", new CswNbtViewId( CswConvert.ToInt32( WelcomeRow["nodeviewid"] ) ).ToString() );
								CswXmlDocument.AppendXmlAttribute( ItemNode, "viewmode", ThisView.ViewMode.ToString().ToLower() );
								CswXmlDocument.AppendXmlAttribute( ItemNode, "type", "view" );
							}
						}
						break;

					case WelcomeComponentType.Text:
						LinkText = WelcomeRow["displaytext"].ToString();
						break;

				} // switch( ThisType )

				if( LinkText != string.Empty )
				{
					CswXmlDocument.AppendXmlAttribute( ItemNode, "linktype", WelcomeRow["componenttype"].ToString() );
					if(WelcomeRow["buttonicon"].ToString() != string.Empty)
						CswXmlDocument.AppendXmlAttribute( ItemNode, "buttonicon", IconImageRoot + "/" + WelcomeRow["buttonicon"].ToString() );
					CswXmlDocument.AppendXmlAttribute( ItemNode, "text", LinkText );
					CswXmlDocument.AppendXmlAttribute( ItemNode, "displayrow", WelcomeRow["display_row"].ToString() );
					CswXmlDocument.AppendXmlAttribute( ItemNode, "displaycol", WelcomeRow["display_col"].ToString() );
				}

			} // foreach( DataRow WelcomeRow in WelcomeTable.Rows )

			ret = ReturnXML.InnerXml;
			return ret;

		} // GetWelcomeItems()


		public void ResetWelcomeItems( string strRoleId )
		{
			CswPrimaryKey RolePk = new CswPrimaryKey();
			RolePk.FromString( strRoleId );
			Int32 RoleId = RolePk.PrimaryKey;

			// Reset the contents for this role to factory default
			CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "WelcomeUpdateReset", "welcome" );
			DataTable WelcomeTable = WelcomeUpdate.getTable( "roleid", RoleId );
			for( Int32 i = 0; i < WelcomeTable.Rows.Count; i++ )
			{
				WelcomeTable.Rows[i].Delete();
			}

			CswNbtViewId EquipmentByTypeViewId = new CswNbtViewId();
			CswNbtViewId TasksOpenViewId = new CswNbtViewId();
			CswNbtViewId ProblemsOpenViewId = new CswNbtViewId();
			CswNbtViewId FindEquipmentViewId = new CswNbtViewId();

            Collection<CswNbtView> Views = _CswNbtResources.ViewSelect.getVisibleViews( false );
			foreach( CswNbtView View in Views )
			{
                if( View.ViewName == "All Equipment" )
                    EquipmentByTypeViewId = View.ViewId;
                if( View.ViewName == "Tasks: Open" )
                    TasksOpenViewId = View.ViewId;
                if( View.ViewName == "Problems: Open" )
                    ProblemsOpenViewId = View.ViewId;
                if( View.ViewName == "Find Equipment" )
                    FindEquipmentViewId = View.ViewId;
			}

			Int32 ProblemNodeTypeId = Int32.MinValue;
			Int32 TaskNodeTypeId = Int32.MinValue;
			Int32 ScheduleNodeTypeId = Int32.MinValue;
			Int32 EquipmentNodeTypeId = Int32.MinValue;
			foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
			{
				if( NodeType.NodeTypeName == "Equipment Problem" )
					ProblemNodeTypeId = NodeType.FirstVersionNodeTypeId;
				if( NodeType.NodeTypeName == "Equipment Task" )
					TaskNodeTypeId = NodeType.FirstVersionNodeTypeId;
				if( NodeType.NodeTypeName == "Equipment Schedule" )
					ScheduleNodeTypeId = NodeType.FirstVersionNodeTypeId;
				if( NodeType.NodeTypeName == "Equipment" )
					EquipmentNodeTypeId = NodeType.FirstVersionNodeTypeId;
			}

			// Equipment
			if( FindEquipmentViewId.isSet() )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Search, CswViewListTree.ViewType.View, FindEquipmentViewId.get(), Int32.MinValue, string.Empty, 1, 1, "magglass.gif", RoleId );
			if( EquipmentNodeTypeId != Int32.MinValue )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, EquipmentNodeTypeId, string.Empty, 5, 1, "", RoleId );
			if( EquipmentByTypeViewId.isSet() )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, EquipmentByTypeViewId.get(), Int32.MinValue, "All Equipment", 7, 1, "", RoleId );

			// Problems
			if( ProblemsOpenViewId.isSet())
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, ProblemsOpenViewId.get(), Int32.MinValue, "Problems", 1, 3, "warning.gif", RoleId );
			if( ProblemNodeTypeId != Int32.MinValue )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ProblemNodeTypeId, "Add New Problem", 5, 3, "", RoleId );

			// Schedules and Tasks
			if( TasksOpenViewId.isSet())
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Link, CswViewListTree.ViewType.View, TasksOpenViewId.get(), Int32.MinValue, "Tasks", 1, 5, "clipboard.gif", RoleId );
			if( TaskNodeTypeId != Int32.MinValue )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, TaskNodeTypeId, "Add New Task", 5, 5, "", RoleId );
			if( ScheduleNodeTypeId != Int32.MinValue )
				_AddWelcomeItem( WelcomeTable, WelcomeComponentType.Add, CswViewListTree.ViewType.View, Int32.MinValue, ScheduleNodeTypeId, "Scheduling", 7, 5, "", RoleId );

			WelcomeUpdate.update( WelcomeTable );
		} // ResetWelcomeItems()

		/// <summary>
		/// Adds a welcome component to the welcome page
		/// </summary>
		public void AddWelcomeItem( WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, Int32 PkValue,
									Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, string strRoleId )
		{
			CswPrimaryKey RolePk = new CswPrimaryKey();
			RolePk.FromString( strRoleId );
			Int32 RoleId = RolePk.PrimaryKey;

			CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
			DataTable WelcomeTable = WelcomeUpdate.getEmptyTable();

			_AddWelcomeItem( WelcomeTable, ComponentType, ViewType, PkValue, NodeTypeId, DisplayText, Row, Column, ButtonIcon, RoleId );

			WelcomeUpdate.update( WelcomeTable );
		} // AddWelcomeItem()

		private void _AddWelcomeItem( DataTable WelcomeTable, WelcomeComponentType ComponentType, CswViewListTree.ViewType ViewType, Int32 PkValue,
									  Int32 NodeTypeId, string DisplayText, Int32 Row, Int32 Column, string ButtonIcon, Int32 RoleId )
		{
			if( Row == Int32.MinValue )
			{
				string SqlText = @"select max(display_row) maxcol
									 from welcome
									where display_col = 1
									  and (roleid = " + RoleId.ToString() + @")";
				CswArbitrarySelect WelcomeSelect = _CswNbtResources.makeCswArbitrarySelect( "AddButton_Click_WelcomeSelect", SqlText );
				DataTable WelcomeSelectTable = WelcomeSelect.getTable();
				Int32 MaxRow = 0;
				if( WelcomeSelectTable.Rows.Count > 0 )
				{
					MaxRow = CswConvert.ToInt32( WelcomeSelectTable.Rows[0]["maxcol"] );
					if( MaxRow < 0 ) MaxRow = 0;
				}
				Row = MaxRow + 1;
				Column = 1;
			}

			if( ButtonIcon == "blank.gif" )
				ButtonIcon = string.Empty;

			DataRow NewWelcomeRow = WelcomeTable.NewRow();
			NewWelcomeRow["roleid"] = RoleId;
			NewWelcomeRow["componenttype"] = ComponentType.ToString();
			NewWelcomeRow["display_col"] = Column;
			NewWelcomeRow["display_row"] = Row;

			switch( ComponentType )
			{
				case WelcomeComponentType.Add:
					NewWelcomeRow["nodetypeid"] = CswConvert.ToDbVal( NodeTypeId );
					NewWelcomeRow["buttonicon"] = ButtonIcon;
					NewWelcomeRow["displaytext"] = DisplayText;
					break;
				case WelcomeComponentType.Link:
					switch( ViewType )
					{
						case CswViewListTree.ViewType.View:
							NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( PkValue );
							break;
						case CswViewListTree.ViewType.Action:
							NewWelcomeRow["actionid"] = CswConvert.ToDbVal( PkValue );
							break;
						case CswViewListTree.ViewType.Report:
							NewWelcomeRow["reportid"] = CswConvert.ToDbVal( PkValue );
							break;
						default:
							throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
					}
					NewWelcomeRow["buttonicon"] = ButtonIcon;
					NewWelcomeRow["displaytext"] = DisplayText;
					break;
				case WelcomeComponentType.Search:
					if( ViewType == CswViewListTree.ViewType.View )
					{
						NewWelcomeRow["nodeviewid"] = CswConvert.ToDbVal( PkValue );
						NewWelcomeRow["buttonicon"] = ButtonIcon;
						NewWelcomeRow["displaytext"] = DisplayText;
					}
					else
						throw new CswDniException( "You must select a view", "No view was selected for new Welcome Page Component" );
					break;
				case WelcomeComponentType.Text:
					NewWelcomeRow["displaytext"] = DisplayText;
					break;
			}
			WelcomeTable.Rows.Add( NewWelcomeRow );

		} // _AddWelcomeItem()


		public bool MoveWelcomeItems( string strRoleId, Int32 WelcomeId, Int32 NewRow, Int32 NewColumn )
		{
			bool ret = false;

			CswPrimaryKey RolePk = new CswPrimaryKey();
			RolePk.FromString( strRoleId );
			Int32 RoleId = RolePk.PrimaryKey;

			if( WelcomeId != Int32.MinValue )
			{
				CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
				DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", WelcomeId );
				if( WelcomeTable.Rows.Count > 0 )
				{
					DataRow WelcomeRow = WelcomeTable.Rows[0];
					WelcomeRow["display_row"] = CswConvert.ToDbVal( NewRow );
					WelcomeRow["display_col"] = CswConvert.ToDbVal( NewColumn );
					WelcomeUpdate.update( WelcomeTable );
					ret = true;
				}
			} // if( WelcomeId != Int32.MinValue ) 

			return ret;
		} // MoveWelcomeItems

		public bool DeleteWelcomeItem( string strRoleId, Int32 WelcomeId )
		{
			bool ret = false;

			CswPrimaryKey RolePk = new CswPrimaryKey();
			RolePk.FromString( strRoleId );
			Int32 RoleId = RolePk.PrimaryKey;

			if( WelcomeId != Int32.MinValue )
			{
				CswTableUpdate WelcomeUpdate = _CswNbtResources.makeCswTableUpdate( "AddWelcomeItem_Update", "welcome" );
				DataTable WelcomeTable = WelcomeUpdate.getTable( "welcomeid", WelcomeId );
				if( WelcomeTable.Rows.Count > 0 )
				{
					DataRow WelcomeRow = WelcomeTable.Rows[0];
					WelcomeRow.Delete();
					WelcomeUpdate.update( WelcomeTable );
					ret = true;
				}
			} // if( WelcomeId != Int32.MinValue ) 

			return ret;
		} // MoveWelcomeItems

		public XElement getButtonIconList()
		{
			XElement ret = new XElement( "buttonicons" );

			//ret.Add( new XElement( "icon", new XAttribute( "filename", "blank.gif" ) ) );

			System.IO.DirectoryInfo d = new System.IO.DirectoryInfo( System.Web.HttpContext.Current.Request.PhysicalApplicationPath + CswWelcomeTable.IconImageRoot );
			System.IO.FileInfo[] IconFiles = d.GetFiles();
			foreach( System.IO.FileInfo IconFile in IconFiles )
			{
				if( //IconFile.Name != "blank.gif" &&
					( IconFile.Name.EndsWith( ".gif" ) || IconFile.Name.EndsWith( ".jpg" ) || IconFile.Name.EndsWith( ".png" ) ) )
				{
					ret.Add( new XElement( "icon", new XAttribute( "filename", IconFile.Name ) ) );

					//ListItem IconItem = new ListItem( IconFile.Name, IconFile.Name );
					//IconList.Items.Add( IconItem );
				}
			}
			return ret;
		} // _initButtonIconList()


	} // class CswNbtWebServiceWelcomeItems
} // namespace ChemSW.Nbt.WebServices

