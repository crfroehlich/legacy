using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceReport
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly CswNbtNode _reportNode = null;

        private HttpContext _Context = HttpContext.Current;

        #region WCF Data Objects

        /// <summary>
        /// Return Object for Reports, which inherits from CswWebSvcReturn
        /// </summary>
        [DataContract]
        public class ReportReturn : CswWebSvcReturn
        {
            public ReportReturn()
            {
                Data = new ReportData();
            }
            [DataMember]
            public ReportData Data;
        }

        [DataContract]
        public class ReportData
        {
            [DataMember]
            public string reportFormat = string.Empty;
            [DataMember]
            public string nodeId = string.Empty;
            [DataMember]
            public string gridJSON = string.Empty;
            [DataMember]
            public Stream stream = null;
            [DataMember]
            public Collection<ReportParam> reportParams = new Collection<ReportParam>();

            [DataMember]
            public Collection<string> controlledParams
            {
                get
                {
                    return new Collection<string> { CswNbtObjClassReport.ControlledParams.NodeId, CswNbtObjClassReport.ControlledParams.RoleId, CswNbtObjClassReport.ControlledParams.UserId };
                }
                private set
                {
                    Collection<string> noSetterAllowed = value;
                }
            }

            [DataMember]
            public bool doesSupportCrystal = false;

            [DataContract]
            public class ReportParam
            {
                [DataMember]
                public string name = string.Empty;
                [DataMember]
                public string value = string.Empty;
            }
            
            private Dictionary<string, string> _reportParamsDictionary = null;
            public Dictionary<string, string> ReportParamDictionary
            {
                get
                {
                    if( null == _reportParamsDictionary )
                    {
                        _reportParamsDictionary = new Dictionary<string, string>();
                        foreach( ReportParam param in reportParams )
                        {
                            _reportParamsDictionary.Add( param.name,param.value );
                        }
                    }
                    return _reportParamsDictionary;
                }
            }

        }

        #endregion WCF Data Objects

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources, CswNbtNode reportNode )
        {
            _CswNbtResources = CswNbtResources;
            _reportNode = reportNode;
        }

        public CswNbtWebServiceReport( CswNbtResources CswNbtResources, CswNbtNode reportNode, HttpContext Context )
        {
            _CswNbtResources = CswNbtResources;
            _reportNode = reportNode;
            _Context = Context;
        }

        // Need to double " in string values
        private static string _csvSafe( string str )
        {
            return str.Replace( "\"", "\"\"" );
        }

        public static void runReport( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            JObject ret = new JObject();
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            DataTable rptDataTbl = _getReportTable( CswResources, Return, reportParams );
            CswNbtGrid cg = new CswNbtGrid( NbtResources );
            ret = cg.DataTableToJSON( rptDataTbl );
            reportParams.gridJSON = ret.ToString();
            Return.Data = reportParams;
        }

        public static void runReportCSV( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            DataTable rptDataTbl = _getReportTable( CswResources, Return, reportParams );
            reportParams.stream = wsTools.ReturnCSVStream( rptDataTbl );
            Return.Data = reportParams;
        }

        private static DataTable _getReportTable( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData reportParams )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            DataTable rptDataTbl = new DataTable();
            CswPrimaryKey pk = CswConvert.ToPrimaryKey( reportParams.nodeId );
            if( CswTools.IsPrimaryKey( pk ) )
            {
                CswNbtObjClassReport reportNode = NbtResources.Nodes[pk];
                if( string.Empty != reportNode.SQL.Text )
                {
                    string ReportSql = "";
                    //Case 30293: We are not trying to solve all of the (usability) issues with SQL Reporting today;
                    //rather, we just want to return friendlier errors when SQL faults occur
                    try
                    {
                        ReportSql = CswNbtObjClassReport.ReplaceReportParams( reportNode.SQL.Text, reportParams.ReportParamDictionary );
                        CswArbitrarySelect cswRptSql = NbtResources.makeCswArbitrarySelect( "report_sql", ReportSql );
                        rptDataTbl = cswRptSql.getTable();
                        if( string.IsNullOrEmpty( rptDataTbl.TableName ) && null != reportNode )
                        {
                            rptDataTbl.TableName = reportNode.ReportName.Text;
                        }
                    }
                    catch( CswSqlException CswException )
                    {
                        CswDniException NewException = new CswDniException( CswEnumErrorType.Warning, "SQL Execution failed with error: " + CswException.OracleError, "Could not execute SQL: {" + CswException.Sql + "}", CswException );
                        //CswException.Add( NewException );
                        throw NewException;
                    }
                    catch( Exception Ex )
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Invalid SQL.", "Could not execute SQL: {" + ReportSql + "}", Ex );
                    }
                }
                else
                {
                    throw ( new CswDniException( "Report has no SQL to run!" ) );
                }
            }
            return rptDataTbl;
        }

        public static Collection<CswNbtWebServiceReport.ReportData.ReportParam> FormReportParamsToCollection( NameValueCollection FormData )
        {
            Collection<CswNbtWebServiceReport.ReportData.ReportParam> reportParams = new Collection<CswNbtWebServiceReport.ReportData.ReportParam>();
            foreach( string key in FormData.AllKeys )
            {
                if( false == key.Equals( "reportid" ) ) //reportid is a special case and is used above
                {
                    CswNbtWebServiceReport.ReportData.ReportParam param = new CswNbtWebServiceReport.ReportData.ReportParam();
                    param.name = key;
                    param.value = FormData[key];
                    reportParams.Add( param );
                }
            }
            return reportParams;
        }

        public static void getReportInfo( ICswResources CswResources, CswNbtWebServiceReport.ReportReturn Return, CswNbtWebServiceReport.ReportData Request )
        {
            CswNbtResources NBTResources = (CswNbtResources) CswResources;
            CswPrimaryKey pk = new CswPrimaryKey();
            pk = CswConvert.ToPrimaryKey( Request.nodeId );
            if( CswTools.IsPrimaryKey( pk ) )
            {
                CswNbtObjClassReport reportNode = NBTResources.Nodes[pk];

                Request.doesSupportCrystal = ( false == reportNode.RPTFile.Empty );

                Request.reportParams = new Collection<ReportData.ReportParam>();
                foreach( var paramPair in reportNode.ExtractReportParams( NBTResources.Nodes[NBTResources.CurrentNbtUser.UserId] ) )
                {
                    ReportData.ReportParam paramObj = new ReportData.ReportParam();
                    paramObj.name = paramPair.Key;
                    paramObj.value = paramPair.Value;
                    Request.reportParams.Add( paramObj );
                }
            }
            Return.Data = Request;
        }

    } // class CswNbtWebServiceReport
} // namespace ChemSW.Nbt.WebServices