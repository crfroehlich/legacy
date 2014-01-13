﻿
namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodePersistStrategyUpdate : ICswNbtNodePersistStrategy
    {
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Strategy used to update pre-existing temp and real nodes.  Will not update unmodified nodes unless forced.
        /// </summary>
        public CswNbtNodePersistStrategyUpdate( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            ForceUpdate = false;
            IsCopy = false;
            OverrideUniqueValidation = false;
            SkipEvents = false;
            AllowAuditing = true;
            Creating = false;
        }

        public bool ForceUpdate { get; set; }
        public bool IsCopy { get; set; }
        public bool OverrideUniqueValidation { get; set; }
        public bool SkipEvents { get; set; }
        public bool AllowAuditing { get; set; }
        public bool Creating { get; set; }


        public void postChanges( CswNbtNode Node )
        {
            if( CswEnumNbtNodeModificationState.Modified == Node.ModificationState || ForceUpdate )
            {
                Node.checkWriter();

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.beforeWriteNode( false );
                }

                Node.requestWrite( ForceUpdate, IsCopy, OverrideUniqueValidation, Creating, AllowAuditing && ( false == Node.IsTemp ), SkipEvents );

                if( null != Node.ObjClass )
                {
                    Node.ObjClass.afterWriteNode();
                }

                Node.setModificationState( CswEnumNbtNodeModificationState.Posted );
            }
        }
    }
}
