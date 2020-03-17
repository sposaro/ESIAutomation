using System;
using System.Collections.Generic;

namespace CSCAutomateLib
{
    public class ContestResponse
    {
        public List<Learner> Learners { get; set; }
        public ContestCollection ContestCollection { get; set; }
        public DateTime? AdjustedStartDate { get; set; }
        public DateTime? AdjustedEndDate { get; set; }
        public DateTime AdjustedNextProgressUpdateOn { get; set; }
        public int Growth { get; set; }
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public string ChallengeDescription { get; set; }
        public bool HasPrizes { get; set; }
        public object Prizes { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartDateStr { get; set; }
        public DateTime? EndDate { get; set; }
        public string EndDateStr { get; set; }
        public object TargetValue { get; set; }
        public int NumberOfLearners { get; set; }
        public string CustomCss { get; set; }
        public string TemplateSelection { get; set; }
        public bool SelfRegistrationEnabled { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public object CreatedByEmail { get; set; }
        public string MicrosoftAccountSponsor { get; set; }
        public string Country { get; set; }
        public string Mstpid { get; set; }
        public string Eou { get; set; }
        public string AccountType { get; set; }
        public object LastProgressUpdateOn { get; set; }
        public DateTime NextProgressUpdateOn { get; set; }
        public int ProgressUpdateInterval { get; set; }
        public string TimeZone { get; set; }
        public object GroupName { get; set; }
        public bool AllowTeams { get; set; }
        public string Teams { get; set; }
        public object Mpnid { get; set; }
        public string ParticipantType { get; set; }
        public object CollectionID { get; set; }
        public bool ShowModuleCompletionDate { get; set; }
        public object CollectionName { get; set; }
        public object CollectionUrl { get; set; }
        public object StatusCode { get; set; }
        public object Message { get; set; }
        public Errors Errors { get; set; }
        public string Title { get; set; }
        public string TraceId { get; set; }
    }

    /// <summary>
    /// Errors returned by the CSC API
    /// </summary>
    public class Errors
    {
        public List<string> EndDateStr { get; set; }
    }
}
