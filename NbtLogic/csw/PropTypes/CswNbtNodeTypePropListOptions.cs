using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract]
    public class CswNbtNodeTypePropListOptions
    {
        public static char delimiter = ',';
        private CswNbtMetaDataNodeTypeProp _NodeTypeProp;
        private CswNbtResources _CswNbtResources;

        private CswNbtNodeTypePropListOption[] _Options;
        [DataMember]
        public CswNbtNodeTypePropListOption[] Options
        {
            get { return ( _Options ); }
            set { var doesNothing = value; } // because CF told me so
        }

        private void _init()
        {
            if( _NodeTypeProp.IsFK && _NodeTypeProp.FKType == "fkeydefid" )
            {
                CswTableSelect FkeyDefsSelect = _CswNbtResources.makeCswTableSelect( "fetch_fkey_def_sql", "fkey_definitions" );
                DataTable FkeyDefsTable = FkeyDefsSelect.getTable( "fkeydefid", _NodeTypeProp.FKValue );
                string Sql = FkeyDefsTable.Rows[0]["sql"].ToString();

                CswArbitrarySelect ListOptsSelect = _CswNbtResources.makeCswArbitrarySelect( "list_options_query", Sql );
                DataTable ListOptsTable = ListOptsSelect.getTable();

                _Options = new Collection<CswNbtNodeTypePropListOption>();
                _Options.Add( new CswNbtNodeTypePropListOption( "", "" ) );
                foreach( DataRow CurrentRow in ListOptsTable.Rows )
                {
                    _Options.Add( new CswNbtNodeTypePropListOption( CurrentRow[FkeyDefsTable.Rows[0]["ref_column"].ToString()].ToString(),
                                                                            CurrentRow[FkeyDefsTable.Rows[0]["pk_column"].ToString()].ToString() ) );
                }//iterate listopts rows
            }
            else
            {
                CswCommaDelimitedString Opts = new CswCommaDelimitedString();
                Opts.FromString( _NodeTypeProp.ListOptions );
                Override( Opts );
            }
        }

        public CswNbtNodeTypePropListOptions( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            _NodeTypeProp = NodeTypeProp;
            _CswNbtResources = CswNbtResources;
            _init();
        }//ctor

        public CswNbtNodeTypePropListOptions( CswNbtResources CswNbtResources, Int32 NodeTypePropId )
        {
            _NodeTypeProp = CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropId );
            _CswNbtResources = CswNbtResources;
            _init();
        }//ctor

        public void Override( CswCommaDelimitedString CommaDelimitedOptions )
        {
            _Options = new Collection<CswNbtNodeTypePropListOption>();
            if( false == _NodeTypeProp.IsRequired )
            {
                _Options.Add( new CswNbtNodeTypePropListOption( "", "" ) );
            }
            for( int i = 0; i < CommaDelimitedOptions.Count; i += 1 )
            {
                _Options.Add( new CswNbtNodeTypePropListOption( CommaDelimitedOptions[i].Trim(), CommaDelimitedOptions[i].Trim() ) );
            }
        }

        private Collection<CswNbtNodeTypePropListOption> _Options;
        public Collection<CswNbtNodeTypePropListOption> Options { get { return ( _Options ); } set { _Options = value; } }
        
        public void Override( IEnumerable<CswNbtNodeTypePropListOption> NewOptions )
        {
            int iOptionCnt = 0;
            if( false == _NodeTypeProp.IsRequired )
            {
                _Options = new CswNbtNodeTypePropListOption[NewOptions.Count() + 1];
                _Options[0] = new CswNbtNodeTypePropListOption( "", "" );
                iOptionCnt = 1;
            }
            else
            {
                _Options = new CswNbtNodeTypePropListOption[NewOptions.Count()];
            }
            foreach(CswNbtNodeTypePropListOption thisOption in NewOptions)
            {
                _Options[iOptionCnt] = thisOption;
                iOptionCnt += 1;
            }
        }

        public CswNbtNodeTypePropListOption FindByValue( string Value )
        {
            return Options.FirstOrDefault( Option => Option.Value == Value );
        }

        public CswNbtNodeTypePropListOption FindByText( string Text )
        {
            return Options.FirstOrDefault( Option => Option.Text == Text );
        }

        //public override string ToString()
        //{
        //    CswCommaDelimitedString ret = new CswCommaDelimitedString();
        //    foreach( CswNbtNodeTypePropListOption Option in Options.Where( Option => null != Option ) )
        //    {
        //        ret.Add( Option.Text );
        //    }
        //    return ret.ToString();
        //} // ToString()

    }//CswNbtNodeTypePropListOptions

}//namespace ChemSW.Nbt.PropTypes
