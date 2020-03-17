namespace CSCAutomateLib
{
    public class ContestRequest
    {
        public string Name { get; set; }
        public string ChallengeDescription { get; set; }
        public string HasPrizes { get; set; }
        public string Type { get; set; } = "Collection";
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string CustomCss { get; set; }
        public string TemplateSelection { get; set; }
        public string SelfRegistrationEnabled { get; set; }
        public string CreatedBy { get; set; }
        public string MicrosoftAccountSponsor { get; set; }
        public string Country { get; set; }
        public string Mstpid { get; set; }
        public string Eou { get; set; }
        public string AccountType { get; set; }
        public string TimeZone { get; set; }
        public string AllowTeams { get; set; }
        public string Teams { get; set; }
        public string Mpnid { get; set; }
        public string ParticipantType { get; set; }
        public string CollectionID { get; set; }
        public string CollectionName { get; set; }
        public string CollectionUrl { get; set; }
    }
}
