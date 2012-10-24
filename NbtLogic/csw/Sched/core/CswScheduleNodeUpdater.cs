using System;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropertySets;


namespace ChemSW.Nbt.Sched
{
    public class CswScheduleNodeUpdater
    {

        private CswNbtResources _CswNbtResources = null;
        public CswScheduleNodeUpdater( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public void update( CswNbtNode CswNbtNodeSchedualable, string StatusMessage )
        {
            ICswNbtPropertySetScheduler SchedulerNode = CswNbtPropSetCaster.AsPropertySetScheduler( CswNbtNodeSchedualable );
            //case 25702 - add comment:
            if( false == String.IsNullOrEmpty( StatusMessage ) )
            {
                SchedulerNode.RunStatus.AddComment( StatusMessage );
            }

            DateTime AfterDate = SchedulerNode.NextDueDate.DateTimeValue;
            if( SchedulerNode.WarningDays.Value > 0 )
            {
                // case 27930 - Prevent redundant activity within the warning days window
                AfterDate = AfterDate.AddDays( SchedulerNode.WarningDays.Value ); 
            }
            DateTime CandidateNextDueDate = SchedulerNode.DueDateInterval.getNextOccuranceAfter( AfterDate );
            if( _finalDueDateHasPassed( CandidateNextDueDate.Date, SchedulerNode.FinalDueDate.DateTimeValue.Date ) )
            {
                CandidateNextDueDate = DateTime.MinValue;
            }

            SchedulerNode.NextDueDate.DateTimeValue = CandidateNextDueDate;
            CswNbtNodeSchedualable.postChanges( false );

        }//update() 

        private bool _finalDueDateHasPassed( DateTime CandidateNextDueDate, DateTime FinalDueDate )
        {
            return FinalDueDate != DateTime.MinValue &&
                ( FinalDueDate < DateTime.Now.Date ||
                CandidateNextDueDate > FinalDueDate );
        }


    }//CswScheduleNodeUpdater

}//namespace ChemSW.MtSched


