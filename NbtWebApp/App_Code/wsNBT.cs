﻿using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.Script.Services;   // supports ScriptService attribute
using ChemSW.Core;
using ChemSW.Config;
using ChemSW.Security;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ChemSW.Nbt.WebServices
{
	/// <summary>
	/// NBT Web service interface
	/// </summary>
	/// 
	[ScriptService]
	[WebService( Namespace = "http://localhost/NbtWebApp" )]
	[WebServiceBinding( ConformsTo = WsiProfiles.BasicProfile1_1 )]
	public class wsNBT : System.Web.Services.WebService
	{
		#region Session and Resource Management

		private CswSessionResourcesNbt _SessionResources;
		private CswNbtResources _CswNbtResources;

		private string _FilesPath
		{
			get
			{
				return ( System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\etc" );
			}
		}

		private void start()
		{
			_SessionResources = new CswSessionResourcesNbt( Context.Application, Context.Session, Context.Request, Context.Response, string.Empty, _FilesPath, SetupMode.Web );
			_CswNbtResources = _SessionResources.CswNbtResources;

		}//start() 

		private void end()
		{
			if( _CswNbtResources != null )
			{
				_CswNbtResources.finalize();
				_CswNbtResources.release();
			}
			if( _SessionResources != null )
				_SessionResources.setCache();
		}

		private string error( Exception ex )
		{
			_CswNbtResources.CswLogger.reportError( ex );
			_CswNbtResources.Rollback();
			return "<error>Error: " + ex.Message + "</error>";
		}

        //never used
        //private string result( string ReturnVal )
        //{
        //    return "<result>" + ReturnVal + "</result>";
        //}

		/// <summary>
		/// Append to QuickLaunch
		/// </summary>
		private void addToQuickLaunch(CswNbtView View)
		{
			const string QuickLaunchViews = CswNbtWebServiceQuickLaunchItems.QuickLaunchViews;
			if( ( View.ViewId > 0 ) || ( View.ViewId <= 0 && View.SessionViewId > 0 ) )
			{
				LinkedList<CswNbtQuickLaunchItem> ViewHistoryList = null;
				if( null == Session[QuickLaunchViews] )
				{
					ViewHistoryList = new LinkedList<CswNbtQuickLaunchItem>();
				}
				else
				{
					ViewHistoryList = (LinkedList<CswNbtQuickLaunchItem>) Session[QuickLaunchViews];
				}
				var ThisView = new CswNbtQuickLaunchItem( View.ViewId, View.ViewName, View.ViewMode );

				if( ViewHistoryList.Contains( ThisView ) )
				{
					ViewHistoryList.Remove( ThisView );
				}
				ViewHistoryList.AddFirst( ThisView );
				Session[QuickLaunchViews] = ViewHistoryList;
			}
		} // addToQuickLaunch()

		#endregion Session and Resource Management

		#region Web Methods


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string authenticate( string AccessId, string UserName, string Password )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				_SessionResources.CswSessionManager.setAccessId( AccessId );
				AuthenticationStatus AuthenticationStatus = _SessionResources.CswSessionManager.Authenticate( UserName, Password, CswWebControls.CswNbtWebTools.getIpAddress() );
				//ReturnVal = result( "<AuthenticationStatus>" + AuthenticationStatus + "</AuthenticationStatus>" );
				ReturnVal = "{ \"AuthenticationStatus\": \"" + AuthenticationStatus + "\" }";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}

			return ( ReturnVal );
		}//authenticate()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string deauthenticate()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				_SessionResources.CswSessionManager.DeAuthenticate();
				ReturnVal = "{ \"Deauthentication\": \"Succeeded\" }";
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		}//deAuthenticate()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getWelcomeItems( string RoleId )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();

				var ws = new CswNbtWebServiceWelcomeItems( _CswNbtResources );
				// Only administrators can get welcome content for other roles
				if( RoleId != string.Empty && _CswNbtResources.CurrentNbtUser.IsAdministrator() )
					ReturnVal = ws.GetWelcomeItems( RoleId );
				else
					ReturnVal = ws.GetWelcomeItems( _CswNbtResources.CurrentNbtUser.RoleId.ToString() );

				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getWelcomeItems()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getQuickLaunchItems()
		{
            var QuickLaunchItems = new XElement( "quicklaunch" );
			try
			{
				start();

				CswPrimaryKey UserId = _CswNbtResources.CurrentNbtUser.UserId;
				var ws = new CswNbtWebServiceQuickLaunchItems( _CswNbtResources, Session );
				if( null != UserId )
				{
					QuickLaunchItems.Add( ws.getQuickLaunchItems( UserId ) );
				}

				end();
			}
			catch( Exception ex )
			{
                QuickLaunchItems = XElement.Parse( error( ex ) );
			}

            return QuickLaunchItems;
		} // getQuickLaunchItems()


		//[WebMethod( EnableSession = true )]
		//public XmlDocument getViews()
		//{
		//    CswTimer Timer = new CswTimer();
		//    string ReturnVal = string.Empty;
		//    try
		//    {
		//        start();
		//        CswNbtWebServiceTreeView ws = new CswNbtWebServiceTreeView( _CswNbtResources );
		//        ReturnVal = ws.getViews();
		//        end();
		//    }
		//    catch( Exception ex )
		//    {
		//        ReturnVal = error( ex );
		//    }
		//    //return ( ReturnVal );
		//    XmlDocument Doc = new XmlDocument();
		//    Doc.LoadXml( ReturnVal );
		//    return Doc;
		//} // getViews()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getViewTree()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceView( _CswNbtResources );
				ReturnVal = ws.getViewTree(Session);
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getViews()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getDashboard()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = ws.getDashboard();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getDashboard()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getHeaderMenu()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = ws.getHeaderMenu();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal );
			return Doc;
		} // getHeaderMenu()		[WebMethod( EnableSession = true )]

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getMainMenu( string ViewNum, string SafeNodeKey )
		{
            XElement ReturnNode = new XElement("return");
		    string t = string.Empty;
            try
            {
                start();
                var ws = new CswNbtWebServiceMainMenu(_CswNbtResources);
                Int32 ViewId = CswConvert.ToInt32(ViewNum);
                if (Int32.MinValue != ViewId || !string.IsNullOrEmpty(SafeNodeKey))
                {
                    ReturnNode = ws.getMenu(ViewId, SafeNodeKey);
                }
                end();
			}
			catch( Exception ex )
			{
                ReturnNode = XElement.Parse( error( ex ) );
			}
            return ReturnNode;
		} // getMainMenu()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string getGrid( string ViewPk, string SafeNodeKey = null )
		{
			var ReturnJson = string.Empty;
            string ParsedNokeKey = wsTools.FromSafeJavaScriptParam( SafeNodeKey );

			try
			{
				start();
				Int32 ViewId = CswConvert.ToInt32( ViewPk );
				if( Int32.MinValue != ViewId )
				{
					CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
					if( null != View )
					{
						CswNbtNodeKey ParentNodeKey = null;
						if( !string.IsNullOrEmpty( ParsedNokeKey ) )
						{
							ParentNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNokeKey );
						}
						var g = new CswNbtWebServiceGrid( _CswNbtResources, View, ParentNodeKey );
						ReturnJson = g.getGrid().ToString();
						addToQuickLaunch( View );
					}
				}
				end();
			}
			catch( Exception Ex )
			{
				ReturnJson = ( error( Ex ) );
			}

			return ReturnJson;
		} // getGrid()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTree( string ViewNum, string IDPrefix, string ViewMode )
		{
		    var TreeNode = new XElement("tree");

			try
			{
				start();
			    Int32 ViewId = CswConvert.ToInt32( ViewNum );
                if( Int32.MinValue != ViewId )
                {
                    CswNbtView View = CswNbtViewFactory.restoreView( _CswNbtResources, ViewId );
                    if( null != View )
                    {
                        var ws = new CswNbtWebServiceTree( _CswNbtResources );
                        TreeNode = ws.getTree( View, IDPrefix, ViewMode );
                        addToQuickLaunch( View );
                    }
                }
			    end();
			}
			catch( Exception ex )
			{
                TreeNode = XElement.Parse( error( ex ) );
			}

            return TreeNode;
		} // getTree()


		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XElement getTabs( string EditMode, string SafeNodeKey, string NodeTypeId )
		{
		    var TabsNode = new XElement("tabs");
            try
            {
                start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if (!string.IsNullOrEmpty(ParsedNokeKey) || EditMode == "AddInPopup")
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    TabsNode = ws.getTabs( RealEditMode, ParsedNokeKey, CswConvert.ToInt32( NodeTypeId ) );
                }
                end();
			}
			catch( Exception ex )
			{
                TabsNode = XElement.Parse( error( ex ) );
			}

            return TabsNode;
		} // getTabs()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getProps( string EditMode, string SafeNodeKey, string TabId, string NodeTypeId )
		{
			XmlDocument ReturnXml = null;
			try
			{
				start();
				string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) || EditMode == "AddInPopup" )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnXml = ws.getProps( RealEditMode, ParsedNokeKey, TabId, CswConvert.ToInt32( NodeTypeId ) );
                }
			    end();
			}
			catch( Exception ex )
			{
				ReturnXml = new XmlDocument();
				ReturnXml.LoadXml( error( ex ) );
			}
			return ReturnXml;
		} // getProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
        public XmlDocument getSingleProp( string EditMode, string SafeNodeKey, string PropId, string NodeTypeId, string NewPropXml )
		{
			XmlDocument ReturnXml = null;
			try
			{
				start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnXml = ws.getSingleProp( RealEditMode, ParsedNokeKey, PropId, CswConvert.ToInt32( NodeTypeId ),
                                                 NewPropXml);
                }
			    end();
			}
			catch( Exception ex )
			{
				ReturnXml = new XmlDocument();
				ReturnXml.LoadXml( error( ex ) );
			}
			return ReturnXml;
		} // getProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public string saveProps( string EditMode, string SafeNodeKey, string NewPropsXml, string NodeTypeId )
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    var ws = new CswNbtWebServiceTabsAndProps(_CswNbtResources);
                    var RealEditMode = (CswNbtWebServiceTabsAndProps.NodeEditMode) Enum.Parse(typeof (CswNbtWebServiceTabsAndProps.NodeEditMode), EditMode);
                    ReturnVal = ws.saveProps( RealEditMode, ParsedNokeKey, NewPropsXml, CswConvert.ToInt32( NodeTypeId ) );
                }
			    end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			return ( ReturnVal );
		} // saveProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Xml )]
		public XmlDocument getAbout()
		{
			string ReturnVal = string.Empty;
			try
			{
				start();
				var ws = new CswNbtWebServiceHeader( _CswNbtResources );
				ReturnVal = ws.makeVersionXml();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal = error( ex );
			}
			//return ( ReturnVal );
			XmlDocument Doc = new XmlDocument();
			Doc.LoadXml( ReturnVal.Replace( "&", "&amp;" ) );
			return Doc;
		} // saveProps()

		[WebMethod( EnableSession = true )]
		[ScriptMethod( ResponseFormat = ResponseFormat.Json )]
        public string DeleteNode( string SafeNodeKey )
		{
			JProperty ReturnVal = new JProperty("Result");
			try
			{
				start();
                string ParsedNokeKey = wsTools.FromSafeJavaScriptParam(SafeNodeKey);
                if( !string.IsNullOrEmpty( ParsedNokeKey ) )
                {
                    CswNbtNodeKey NbtNodeKey = new CswNbtNodeKey( _CswNbtResources, ParsedNokeKey );
                    CswNbtNode NodeToDelete = _CswNbtResources.Nodes[NbtNodeKey.NodeId];
                    NodeToDelete.delete();
                    ReturnVal.Value = "Succeeded";
                }
			    end();
			}
			catch( Exception ex )
			{
                ReturnVal.Value = error( ex );
			}
			return ( ReturnVal.ToString() );
		}

		[WebMethod( EnableSession = true )]
        [ScriptMethod( ResponseFormat = ResponseFormat.Json )]
		public string MoveProp( string PropId, string NewRow, string NewColumn )
		{
            JProperty ReturnVal = new JProperty( "Result" );
			try
			{
				start();
				CswNbtWebServiceTabsAndProps ws = new CswNbtWebServiceTabsAndProps( _CswNbtResources );
				bool ret = ws.moveProp( PropId, CswConvert.ToInt32( NewRow ), CswConvert.ToInt32( NewColumn ) );
				ReturnVal.Value = ret.ToString().ToLower();
				end();
			}
			catch( Exception ex )
			{
				ReturnVal.Value = error( ex );
			}
			return ( ReturnVal.ToString() );
		}

		#endregion Web Methods

	}//wsNBT

} // namespace ChemSW.WebServices
