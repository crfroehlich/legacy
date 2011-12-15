﻿using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReport : CswNbtObjClass
    {
        public static string RPTFilePropertyName { get { return "RPT File"; } }
        public static string ReportNamePropertyName { get { return "Report Name"; } }
        public static string CategoryPropertyName { get { return "Category"; } }
        public static string ViewPropertyName { get { return "View"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReport( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassReport( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass ); }
        }

        #region Object class specific Events

        public delegate void AfterModifyReportEventHandler();
        public static string AfterModifyReportEventName = "AfterModifyReport";

        #endregion Object class specific Events


        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            // BZ 10048
            Collection<object> AfterModifyReportEvents = _CswNbtResources.CswEventLinker.Trigger( AfterModifyReportEventName );
            foreach( object Handler in AfterModifyReportEvents )
            {
                if( Handler is AfterModifyReportEventHandler )
                    ( (AfterModifyReportEventHandler) Handler )();
            }

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ActionObj )
        {
            if( null != NodeTypeProp ) { /*Do Something*/ }
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropBlob RPTFile
        {
            get
            {
                return ( _CswNbtNode.Properties[RPTFilePropertyName].AsBlob );
            }
        }

        public CswNbtNodePropText ReportName
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportNamePropertyName].AsText );
            }
        }

        public CswNbtNodePropText Category
        {
            get
            {
                return ( _CswNbtNode.Properties[CategoryPropertyName].AsText );
            }
        }

        public CswNbtNodePropViewReference View
        {
            get
            {
                return ( _CswNbtNode.Properties[ViewPropertyName].AsViewReference );
            }
        }

        #endregion

    }//CswNbtObjClassReport

}//namespace ChemSW.Nbt.ObjClasses
