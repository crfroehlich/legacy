﻿using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceNbtManager
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceNbtManager( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.NBTManager ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use NBT Manager web services if the NBT Manager module is not enabled.", "Attempted to instance CswNbtWebServiceNbtManager, while the NBT Manager module is not enabled." );
            }
        } //ctor

        public JObject getActiveAccessIds()
        {
            JObject RetObj = new JObject();
            JArray CustomerIds = new JArray();
            RetObj["customerids"] = CustomerIds;

            foreach( string AccessId in _CswNbtResources.CswDbCfgInfo.AccessIds )
            {
                if( _CswNbtResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
                {
                    CustomerIds.Add( AccessId );
                }
            }
            return RetObj;
        }

        private void _ValidateAccessId( string AccessId )
        {
            if( string.IsNullOrEmpty( AccessId ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot get Scheduled Rules without a Customer ID.", "getScheduledRulesGrid was called with a null or empty AccessID." );
            }
            if( false == _CswNbtResources.CswDbCfgInfo.ConfigurationExists( AccessId, true ) )
            {
                throw new CswDniException( ErrorType.Error, "The supplied Customer ID " + AccessId + " does not exist or is not enabled.", "No configuration could be loaded for AccessId " + AccessId + "." );
            }
        }

        public JObject getScheduledRulesGrid( string AccessId )
        {
            JObject RetObj;
            _ValidateAccessId( AccessId );

            _CswNbtResources.AccessId = AccessId;
            CswTableSelect ScheduledRulesSelect = _CswNbtResources.makeCswTableSelect( "Scheduledrules_select_on_" + AccessId, "scheduledrules" );
            DataTable ScheduledRulesTable = ScheduledRulesSelect.getTable();

            CswGridData GridData = new CswGridData( _CswNbtResources );
            string TablePkColumn = "scheduledruleid";
            GridData.PkColumn = TablePkColumn;
            GridData.HidePkColumn = true;

            CswCommaDelimitedString ExcludedColumns = new CswCommaDelimitedString()
                                                          {
                                                              "THREADID"
                                                          };
            CswCommaDelimitedString ReadOnlyColumns = new CswCommaDelimitedString()
                                                          {
                                                              "RULENAME",
                                                              "TOTALROGUECOUNT",
                                                              "RUNSTARTTIME",
                                                              "RUNENDTIME",
                                                              "LASTRUN",
                                                              "STATUSMESSAGE"
                                                          };

            foreach( string ColumnName in ExcludedColumns )
            {
                ScheduledRulesTable.Columns.Remove( ColumnName );
            }

            GridData.EditableColumns = new CswCommaDelimitedString();
            foreach( DataColumn Column in ScheduledRulesTable.Columns )
            {
                if( false == ReadOnlyColumns.Contains( Column.ColumnName ) )
                {
                    GridData.EditableColumns.Add( Column.ColumnName );
                }
            }

            RetObj = GridData.DataTableToJSON( ScheduledRulesTable, true );

            return RetObj;
        }

        public bool updateScheduledRule( string AccessId, string DataRow )
        {
            bool RetSuccess = false;

            _ValidateAccessId( AccessId );

            return RetSuccess;
        }

    } // class CswNbtWebServiceNbtManager

} // namespace ChemSW.Nbt.WebServices
