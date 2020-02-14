using System;
using System.Collections.Generic;

namespace CSCAutomate
{
    public class ContestResponse
    {
        public List<object> learners { get; set; }
        public ContestCollection contestCollection { get; set; }
        public DateTime adjustedStartDate { get; set; }
        public DateTime adjustedEndDate { get; set; }
        public DateTime adjustedNextProgressUpdateOn { get; set; }
        public int growth { get; set; }
        public string id { get; set; }
        public string contestId { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public string challengeDescription { get; set; }
        public bool hasPrizes { get; set; }
        public object prizes { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public DateTime startDate { get; set; }
        public string startDateStr { get; set; }
        public DateTime endDate { get; set; }
        public string endDateStr { get; set; }
        public object targetValue { get; set; }
        public int numberOfLearners { get; set; }
        public string customCss { get; set; }
        public string templateSelection { get; set; }
        public bool selfRegistrationEnabled { get; set; }
        public DateTime createdOn { get; set; }
        public string createdBy { get; set; }
        public object createdByEmail { get; set; }
        public string microsoftAccountSponsor { get; set; }
        public string country { get; set; }
        public string mstpid { get; set; }
        public string eou { get; set; }
        public string accountType { get; set; }
        public object lastProgressUpdateOn { get; set; }
        public DateTime nextProgressUpdateOn { get; set; }
        public int progressUpdateInterval { get; set; }
        public string timeZone { get; set; }
        public object groupName { get; set; }
        public bool allowTeams { get; set; }
        public string teams { get; set; }
        public object mpnid { get; set; }
        public string participantType { get; set; }
        public object collectionID { get; set; }
        public bool showModuleCompletionDate { get; set; }
        public object collectionName { get; set; }
        public object collectionUrl { get; set; }
    }
}
