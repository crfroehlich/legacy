using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.Sched;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceImport
    {
        public static void getImportDefs( ICswResources CswResources, CswNbtImportWcf.ImportDefsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
            ret.Data = Importer.getDefinitionNames().ToString();
        }

        public static void getImportJobs( ICswResources CswResources, CswNbtImportWcf.ImportJobsReturn ret, object parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );
            foreach( CswNbtImportDataJob Job in Importer.getJobs() )
            {
                ret.Data.Add( Job );
            }
        }

        public static void getImportStatus( ICswResources CswResources, CswNbtImportWcf.ImportStatusReturn ret, CswNbtImportWcf.ImportStatusRequest parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            if( Int32.MinValue != parms.JobId )
            {
                CswNbtImportDataJob Job = new CswNbtImportDataJob( CswNbtResources, parms.JobId );
                ret.Data.DateEnded = Job.DateEnded;
                Job.getStatus( out ret.Data.RowsDone,
                               out ret.Data.RowsTotal,
                               out ret.Data.RowsError,
                               out ret.Data.ItemsDone,
                               out ret.Data.ItemsTotal );
            }
        }

        public static void uploadImportData( ICswResources CswResources, CswNbtImportWcf.ImportDataReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            ret.JobId = Importer.storeData( parms.PostedFile.FileName, path, parms.ImportDefName, parms.Overwrite );
        }


        public static void uploadImportDefinition( ICswResources CswResources, CswWebSvcReturn ret, CswNbtImportWcf.ImportFileParams parms )
        {
            CswNbtResources CswNbtResources = (CswNbtResources) CswResources;
            CswNbtImporter Importer = new CswNbtImporter( CswNbtResources );

            // Write uploaded file to temp dir
            CswTempFile myTempFile = new CswTempFile( CswResources );
            string path = myTempFile.saveToTempFile( parms.PostedFile.InputStream, DateTime.Now.Ticks + "_" + parms.PostedFile.FileName );
            Importer.storeDefinition( path, parms.ImportDefName );
        }

        public static void startCAFImport( ICswResources CswResources, CswWebSvcReturn Ret, CswNbtImportWcf.StartImportParams Params )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;

            if( Params.ImportDefName.Equals( "CAF" ) )
            {
                // Enable the CAFImport rule
                CswTableUpdate TableUpdate = _CswNbtResources.makeCswTableUpdate( "enableCafImportRule", "scheduledrules" );
                DataTable DataTable = TableUpdate.getTable( "where rulename = '" + CswEnumNbtScheduleRuleNames.CAFImport + "'" );
                if( DataTable.Rows.Count > 0 )
                {
                    DataTable.Rows[0]["disabled"] = CswConvert.ToDbVal( false );
                    TableUpdate.update( DataTable );
                }
            }
        }


    } // class CswNbtWebServiceImport

} // namespace ChemSW.Nbt.WebServices

