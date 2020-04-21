using System;

namespace CSCAutomateLib
{
    public class Learner
    {
        public int Rank { get; set; }
        public int? LevelGrowthValue { get; set; }
        public int? PointsGrowthValue { get; set; }
        public string ProgressValue { get; set; }
        public string ProgressPercentage { get; set; }
        public string DisplayNameAndUserName { get; set; }
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string DisplayName { get; set; }
        public string UserPrincipalName { get; set; }
        public string UserName { get; set; }
        public bool HasSeenMicrosoftPrivacyNotice { get; set; }
        public int StartValue { get; set; }
        public int? TargetValue { get; set; }
        public int? CurrentValue { get; set; }
        public int? PercentComplete { get; set; }
        public int? ModuleComplete { get; set; }
        public string Team { get; set; }
        public DateTime? ReportDataLastSyncedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public DateTime? LastProgressUpdateOn { get; set; }
        public DateTime? NextUpdateOn { get; set; }
        public int? Level { get; set; }
        public int? Points { get; set; }
    }
}
