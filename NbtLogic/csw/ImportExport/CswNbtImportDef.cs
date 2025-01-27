﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDef
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDef( CswNbtResources CswNbtResources, string DefinitionName, string SheetName )
        {
            // If incoming sheet name has a $, trim it off    
            SheetName = SheetName.TrimEnd( new char[] { '$' } );

            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbtImportDef_select", CswNbtImportTables.ImportDef.TableName );
            DataTable defTable = defSelect.getTable( "where lower(" + CswNbtImportTables.ImportDef.definitionname + ") = '" + DefinitionName.ToLower() + "'" +
                                                     "  and lower(" + CswNbtImportTables.ImportDef.sheetname + ") = '" + SheetName.ToLower() + "'" );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition: " + DefinitionName + " and sheetname: " + SheetName, "Could not find a matching record in " + CswNbtImportTables.ImportDef.TableName );
            }
        }

        public CswNbtImportDef( CswNbtResources CswNbtResources, Int32 ImportDefinitionId )
        {
            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbtImportDef_select", CswNbtImportTables.ImportDef.TableName );
            DataTable defTable = defSelect.getTable( CswNbtImportTables.ImportDef.importdefid, ImportDefinitionId );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition id: " + ImportDefinitionId, "Could not find a matching record in " + CswNbtImportTables.ImportDef.TableName );
            }
        }

        public CswNbtImportDef( CswNbtResources CswNbtResources, DataRow DefinitionRow )
        {
            _finishConstructor( CswNbtResources, DefinitionRow );
        }

        private void _finishConstructor( CswNbtResources CswNbtResources, DataRow DefinitionRow )
        {
            _CswNbtResources = CswNbtResources;
            if( null != DefinitionRow )
            {
                _row = DefinitionRow;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition", "CswNbtImportDef was passed a null DefinitionRow" );
            }
            _loadBindings();
        }

        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDef.importdefid] ); }
        }
        public string DefinitionName
        {
            get { return _row[CswNbtImportTables.ImportDef.definitionname].ToString(); }
        }
        public string SheetName
        {
            get { return _row[CswNbtImportTables.ImportDef.sheetname].ToString(); }
        }
        public Int32 SheetOrder
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDef.sheetorder] ); }
        }

        public CswNbtImportDefBindingCollection Bindings = new CswNbtImportDefBindingCollection();
        public SortedList<Int32, CswNbtImportDefOrder> ImportOrder = new SortedList<int, CswNbtImportDefOrder>();
        public Collection<CswNbtImportDefRelationship> RowRelationships = new Collection<CswNbtImportDefRelationship>();

       

        /// <summary>
        /// Loads import definition bindings from the database
        /// </summary>
        private void _loadBindings()
        {
            if( Int32.MinValue != ImportDefinitionId )
            {
                // Order
                CswTableSelect OrderSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_order_select", CswNbtImportTables.ImportDefOrder.TableName );
                DataTable OrderDataTable = OrderSelect.getTable( CswNbtImportTables.ImportDefOrder.importdefid, ImportDefinitionId );
                foreach( DataRow OrderRow in OrderDataTable.Rows )
                {
                    this.ImportOrder.Add( CswConvert.ToInt32( OrderRow[CswNbtImportTables.ImportDefOrder.importorder] ), new CswNbtImportDefOrder( _CswNbtResources, OrderRow ) );
                }

                // Bindings
                CswTableSelect BindingsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_bindings_select", CswNbtImportTables.ImportDefBindings.TableName );
                DataTable BindingsDataTable = BindingsSelect.getTable( CswNbtImportTables.ImportDefBindings.importdefid, ImportDefinitionId );
                foreach( DataRow BindingRow in BindingsDataTable.Rows )
                {
                    Bindings.Add( new CswNbtImportDefBinding( _CswNbtResources, BindingRow ) );
                }

                // Row Relationships
                CswTableSelect RelationshipsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_rel_select", CswNbtImportTables.ImportDefRelationships.TableName );
                DataTable RelationshipsDataTable = RelationshipsSelect.getTable( CswNbtImportTables.ImportDefRelationships.importdefid, ImportDefinitionId );
                foreach( DataRow RelRow in RelationshipsDataTable.Rows )
                {
                    RowRelationships.Add( new CswNbtImportDefRelationship( _CswNbtResources, RelRow ) );
                }
            }
        } // _loadBindings()


        public static Dictionary<string, Int32> addDefinitionEntriesFromExcel( CswNbtResources CswNbtResources, string ImportDefinitionName, DataTable OrderDataTable, DataTable DefDataTable )
        {
            Dictionary<string, Int32> ret = new Dictionary<string, Int32>();
            CswTableUpdate importDefinitionUpdate = CswNbtResources.makeCswTableUpdate( "CswNbtImportDef_addDefinitionEntries_Update", CswNbtImportTables.ImportDef.TableName );
            DataTable importDefinitionTable = importDefinitionUpdate.getTable();
            Int32 i = 1;

            // First we get the sheet and sheetorder
            string SheetName = string.Empty;
            Int32 SheetOrder = Int32.MinValue;

            if( null != DefDataTable )
            {
                SheetName = DefDataTable.Rows[0]["sheetname"].ToString();
                SheetOrder = CswConvert.ToInt32( DefDataTable.Rows[0]["sheetorder"] );
            }

            foreach( DataRow OrderRow in OrderDataTable.Rows )
            {
                SheetName = OrderRow["sheetname"].ToString();
                if( false == string.IsNullOrEmpty( SheetName ) &&
                    false == ret.ContainsKey( SheetName ) )
                {

                    DataRow[] ExistingRows = importDefinitionTable.Select( "sheetname = '" + SheetName + "' and definitionname = '" + ImportDefinitionName + "'" );
                    if( ExistingRows.Length > 0 )
                    {
                        ret.Add( SheetName, CswConvert.ToInt32( ExistingRows[0][CswNbtImportTables.ImportDef.PkColumnName] ) );
                    }
                    else
                    {
                        DataRow defrow = importDefinitionTable.NewRow();
                        defrow[CswNbtImportTables.ImportDef.definitionname] = ImportDefinitionName;
                        defrow[CswNbtImportTables.ImportDef.sheetname] = SheetName;
                        defrow[CswNbtImportTables.ImportDef.sheetorder] = Int32.MinValue != SheetOrder ? SheetOrder : i;
                        i++;
                        importDefinitionTable.Rows.Add( defrow );
                        ret.Add( SheetName, CswConvert.ToInt32( defrow[CswNbtImportTables.ImportDef.PkColumnName] ) );
                    }
                }
            } // foreach
            importDefinitionUpdate.update( importDefinitionTable );
            return ret;
        } // _addDefinitionEntriesFromExcel();

        public static Dictionary<string, Int32> addDefinitionEntries( CswNbtResources CswNbtResources, string ImportDefinitionName, DataTable DefDataTable )
        {
            Dictionary<string, Int32> ret = new Dictionary<string, Int32>();
            CswTableUpdate importDefinitionUpdate = CswNbtResources.makeCswTableUpdate( "CswNbtImportDef_addDefinitionEntries_Update", CswNbtImportTables.ImportDef.TableName );

            //this is a hack, and the fact that we can even do this makes me sad
            importDefinitionUpdate._DoledOutTables.Add( DefDataTable );
            importDefinitionUpdate.update( DefDataTable );

            CswTableSelect importDefSelect = CswNbtResources.makeCswTableSelect( "CswNbtImportDef_addDefinitionEntries_Select", CswNbtImportTables.ImportDef.TableName );

            DataTable importDefTable = importDefSelect.getTable("where definitionname = '" + ImportDefinitionName + "'");

            foreach( DataRow DefRow in importDefTable.Rows )
            {
                ret.Add( DefRow["sheetname"].ToString(), Convert.ToInt32(DefRow["importdefid"]) );
            }

            return ret;
        } // _addDefinitionEntries();

        /// <summary>
        /// Check for existence of a definition entry in a schema script before adding bindings.
        /// </summary>
        /// <param name="CswNbtSchemaModTrnsctn"></param>
        /// <param name="ImportDefinitionName"></param>
        /// <returns></returns>
        public static bool checkForDefinitionEntries( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, string ImportDefinitionName )
        {
            bool Ret = false;

            CswTableSelect importDefSelect = CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswNbtImportDef_findDefinitionEntry", CswNbtImportTables.ImportDef.TableName );
            DataTable importDefTable = importDefSelect.getTable( "where definitionname = '" + ImportDefinitionName + "'" );
            if( importDefTable.Rows.Count > 0 )
            {
                Ret = true;
            }

            return Ret;
        }

    } // class CswNbtImportDef
} // namespace
