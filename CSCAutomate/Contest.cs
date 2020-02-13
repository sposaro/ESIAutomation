using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace CSCAutomate
{
    class Contest
    {
        public string name { get; set; }
        public string challengeDescription { get; set; }
        public string hasPrizes { get; set; }
        public string type { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string customCss { get; set; }
        public string templateSelection { get; set; }
        public string selfRegistrationEnabled { get; set; }
        public string createdBy { get; set; }
        public string microsoftAccountSponsor { get; set; }
        public string country { get; set; }
        public string mstpid { get; set; }
        public string eou { get; set; }
        public string accountType { get; set; }
        public string timeZone { get; set; }
        public string allowTeams { get; set; }
        public string teams { get; set; }
        public string mpnid { get; set; }
        public string participantType { get; set; }
        public string collectionID { get; set; }
        public string collectionName { get; set; }
        public string collectionUrl { get; set; }
    }
}
