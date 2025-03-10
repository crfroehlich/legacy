﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.NbtWebControls
{
    public class CswViewVisibilityEditor : CompositeControl
    {
        private CswNbtResources _CswNbtResources;
        public CswViewVisibilityEditor( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            EnsureChildControls();

            // View Visibility DropDown
            _NewViewVisibilityDropDown.Items.Clear();
            _NewViewVisibilityDropDown.Items.Add( new ListItem( "User:", CswEnumNbtViewVisibility.User.ToString() ) );
            if( _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                _NewViewVisibilityDropDown.Items.Add( new ListItem( "Role:", CswEnumNbtViewVisibility.Role.ToString() ) );
                _NewViewVisibilityDropDown.Items.Add( new ListItem( "Everyone", CswEnumNbtViewVisibility.Global.ToString() ) );

                // Role dropdown
                CswNbtMetaDataObjectClass Role_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );

                CswNbtView RoleView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship RoleRelationship = RoleView.AddViewRelationship( Role_ObjectClass, true );

                string PriorVisibilityValue = _NewViewVisibilityRoleDropDown.SelectedValue;
                _NewViewVisibilityRoleDropDown.Items.Clear();
                ICswNbtTree RoleTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, RoleView, true, false, false );

                RoleTree.goToRoot();
                //RoleTree.goToNthChild(0);
                for( int n = 0; n < RoleTree.getChildNodeCount(); n++ )
                {
                    RoleTree.goToNthChild( n );
                    CswNbtNode Node = RoleTree.getNodeForCurrentPosition();
                    _NewViewVisibilityRoleDropDown.Items.Add( new ListItem( Node.NodeName, Node.NodeId.PrimaryKey.ToString() ) );
                    RoleTree.goToParentNode();
                }
                if( _NewViewVisibilityRoleDropDown.Items.FindByValue( PriorVisibilityValue ) != null )
                    _NewViewVisibilityRoleDropDown.SelectedValue = PriorVisibilityValue;
            }

            // User dropdown
            CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            CswNbtView UserView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship UserRelationship = UserView.AddViewRelationship( User_ObjectClass, true );
            if( !_CswNbtResources.CurrentNbtUser.IsAdministrator() )
                UserRelationship.NodeIdsToFilterIn.Add( _CswNbtResources.CurrentUser.UserId );

            string PriorUserVisibilityValue = _NewViewVisibilityUserDropDown.SelectedValue;
            _NewViewVisibilityUserDropDown.Items.Clear();
            ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, UserView, true, false, false );

            UserTree.goToRoot();
            //UserTree.goToNthChild(0);
            for( int n = 0; n < UserTree.getChildNodeCount(); n++ )
            {
                UserTree.goToNthChild( n );
                CswNbtNode Node = UserTree.getNodeForCurrentPosition();
                _NewViewVisibilityUserDropDown.Items.Add( new ListItem( Node.NodeName, Node.NodeId.PrimaryKey.ToString() ) );
                UserTree.goToParentNode();
            }
            if( _NewViewVisibilityUserDropDown.Items.FindByValue( PriorUserVisibilityValue ) != null )
                _NewViewVisibilityUserDropDown.SelectedValue = PriorUserVisibilityValue;
        }

        private DropDownList _NewViewVisibilityDropDown = null;
        private DropDownList _NewViewVisibilityRoleDropDown = null;
        private DropDownList _NewViewVisibilityUserDropDown = null;
        private CswAutoTable _Table = null;

        public DropDownList _VisibilityDropDown { get { return _NewViewVisibilityDropDown; } }
        public DropDownList _VisibilityRoleDropDown { get { return _NewViewVisibilityRoleDropDown; } }
        public DropDownList _VisibilityUserDropDown { get { return _NewViewVisibilityUserDropDown; } }
        protected override void CreateChildControls()
        {
            _Table = new CswAutoTable();
            this.Controls.Add( _Table );

            _NewViewVisibilityDropDown = new DropDownList();
            _NewViewVisibilityDropDown.ID = "_NewViewVisibilityDropDown";
            _NewViewVisibilityDropDown.CssClass = "selectinput";
            _Table.addControl( 0, 0, _NewViewVisibilityDropDown );

            _NewViewVisibilityRoleDropDown = new DropDownList();
            _NewViewVisibilityRoleDropDown.ID = "_NewViewVisibilityRoleDropDown";
            _NewViewVisibilityRoleDropDown.CssClass = "selectinput";
            _Table.addControl( 0, 1, _NewViewVisibilityRoleDropDown );

            _NewViewVisibilityUserDropDown = new DropDownList();
            _NewViewVisibilityUserDropDown.ID = "_NewViewVisibilityUserDropDown";
            _NewViewVisibilityUserDropDown.CssClass = "selectinput";
            _Table.addControl( 0, 1, _NewViewVisibilityUserDropDown );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                _NewViewVisibilityDropDown.Attributes.Add( "onchange", "Popup_NewView_setViewVisibility('" + _NewViewVisibilityDropDown.ClientID + "','" + _NewViewVisibilityRoleDropDown.ClientID + "','" + _NewViewVisibilityUserDropDown.ClientID + "');" );

                if( _NewViewVisibilityDropDown.SelectedValue != CswEnumNbtViewVisibility.Role.ToString() )
                    _NewViewVisibilityRoleDropDown.Style.Add( HtmlTextWriterStyle.Display, "none" );
                else
                    _NewViewVisibilityRoleDropDown.Style.Add( HtmlTextWriterStyle.Display, "" );

                if( _NewViewVisibilityDropDown.SelectedValue != CswEnumNbtViewVisibility.User.ToString() )
                    _NewViewVisibilityUserDropDown.Style.Add( HtmlTextWriterStyle.Display, "none" );
                else
                    _NewViewVisibilityUserDropDown.Style.Add( HtmlTextWriterStyle.Display, "" );
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }

        public CswEnumNbtViewVisibility SelectedVisibility
        {
            get
            {
                EnsureChildControls();
                //return (NbtViewVisibility) Enum.Parse( typeof( NbtViewVisibility ), _NewViewVisibilityDropDown.SelectedValue );
                return (CswEnumNbtViewVisibility) _NewViewVisibilityDropDown.SelectedValue;
            }
            set
            {
                EnsureChildControls();
                if( value == CswEnumNbtViewVisibility.Property )
                {
                    _NewViewVisibilityDropDown.Items.Clear();
                    _NewViewVisibilityDropDown.Items.Add( new ListItem( "Property", CswEnumNbtViewVisibility.Property.ToString() ) );
                }
                else
                {
                    if( _NewViewVisibilityDropDown.Items.FindByValue( value.ToString() ) != null )
                    {
                        foreach( ListItem Item in _NewViewVisibilityDropDown.Items )
                            Item.Selected = false;
                        _NewViewVisibilityDropDown.SelectedValue = value.ToString();
                    }
                }
            }
        }

        public CswPrimaryKey SelectedRoleId
        {
            get
            {
                EnsureChildControls();
                CswPrimaryKey ret = null;
                if( _NewViewVisibilityRoleDropDown.SelectedItem != null )
                    ret = new CswPrimaryKey( "nodes", CswConvert.ToInt32( _NewViewVisibilityRoleDropDown.SelectedValue ) );
                return ret;
            }
            set
            {
                EnsureChildControls();
                if( _NewViewVisibilityRoleDropDown.Items.FindByValue( value.PrimaryKey.ToString() ) != null )
                {
                    foreach( ListItem Item in _NewViewVisibilityRoleDropDown.Items )
                        Item.Selected = false;
                    _NewViewVisibilityRoleDropDown.SelectedValue = value.PrimaryKey.ToString();
                }
            }
        }

        public CswPrimaryKey SelectedUserId
        {
            get
            {
                EnsureChildControls();
                CswPrimaryKey ret = null;
                if( _NewViewVisibilityUserDropDown.SelectedItem != null )
                    ret = new CswPrimaryKey( "nodes", CswConvert.ToInt32( _NewViewVisibilityUserDropDown.SelectedValue ) );
                return ret;
            }
            set
            {
                EnsureChildControls();
                if( _NewViewVisibilityUserDropDown.Items.FindByValue( value.PrimaryKey.ToString() ) != null )
                {
                    foreach( ListItem Item in _NewViewVisibilityUserDropDown.Items )
                        Item.Selected = false;
                    _NewViewVisibilityUserDropDown.SelectedValue = value.PrimaryKey.ToString();
                }
            }
        }

        public event CswErrorHandler OnError;

        public void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else
                throw ex;
        }
    }
}
