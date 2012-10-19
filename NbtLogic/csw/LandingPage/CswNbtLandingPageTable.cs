﻿using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.LandingPage
{
    public class CswNbtLandingPageTable
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtLandingPageTable( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public LandingPageData getLandingPageItems( LandingPageData.Request Request )
        {
            LandingPageData Items = new LandingPageData();
            DataTable LandingPageTable = _getLandingPageTable( Request.RoleId, Request.ActionId );
            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                CswNbtLandingPageItem Item = CswNbtLandingPageItemFactory.makeLandingPageItem( _CswNbtResources, LandingPageRow["componenttype"].ToString() );
                Item.setItemDataForUI( LandingPageRow );
                Items.LandingPageItems.Add( Item.ItemData );
            }
            return Items;
        }

        private DataTable _getLandingPageTable( string RoleId, string ActionId )
        {
            Int32 PkRoleId = _getRoleIdPk( RoleId );
            CswTableSelect LandingPageSelect = _CswNbtResources.makeCswTableSelect( "LandingPageSelect", "landingpage" );
            string WhereClause;
            if( false == String.IsNullOrEmpty( ActionId ) )
            {
                WhereClause = "where for_actionid = '" + ActionId + "'";
            }
            else
            {
                WhereClause = "where for_roleid = '" + PkRoleId.ToString() + "' and for_actionid is null";
            }
            Collection<OrderByClause> OrderBy = new Collection<OrderByClause>();
            OrderBy.Add( new OrderByClause( "display_row", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "display_col", OrderByType.Ascending ) );
            OrderBy.Add( new OrderByClause( "landingpageid", OrderByType.Ascending ) );
            DataTable LandingPageTable = LandingPageSelect.getTable( WhereClause, OrderBy );
            return LandingPageTable;
        }

        private Int32 _getRoleIdPk( string RoleId )
        {
            if( RoleId == string.Empty || false == _CswNbtResources.CurrentNbtUser.IsAdministrator() )
            {
                RoleId = _CswNbtResources.CurrentNbtUser.RoleId.ToString();
            }
            CswPrimaryKey RolePk = new CswPrimaryKey();
            RolePk.FromString( RoleId );
            return RolePk.PrimaryKey;
        }

        public void addLandingPageItem( LandingPageData.Request Request )
        {            
            CswNbtLandingPageItem Item = CswNbtLandingPageItemFactory.makeLandingPageItem( _CswNbtResources, Request.Type );
            Item.setItemDataForDB( Request );
            Item.saveToDB();
        }

        public void moveLandingPageItem( LandingPageData.Request Request )
        {
            if( Request.LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "MoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", Request.LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    DataRow LandingPageRow = LandingPageTable.Rows[0];
                    LandingPageRow["display_row"] = CswConvert.ToDbVal( Request.NewRow );
                    LandingPageRow["display_col"] = CswConvert.ToDbVal( Request.NewColumn );
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }

        public void deleteLandingPageItem( LandingPageData.Request Request )
        {
            if( Request.LandingPageId != Int32.MinValue )
            {
                CswTableUpdate LandingPageUpdate = _CswNbtResources.makeCswTableUpdate( "RemoveLandingPageItem", "landingpage" );
                DataTable LandingPageTable = LandingPageUpdate.getTable( "landingpageid", Request.LandingPageId );
                if( LandingPageTable.Rows.Count > 0 )
                {
                    foreach( DataRow LandingPageRow in LandingPageTable.Rows )
                    {
                        LandingPageRow.Delete();
                    }
                    LandingPageUpdate.update( LandingPageTable );
                }
            }
        }
    }
}
