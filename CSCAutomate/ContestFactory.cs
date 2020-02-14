using System;
using System.Collections.Generic;

namespace CSCAutomate
{
    class ContestFactory
    {
        public static ContestRequest Default => new ContestRequest
        {
            Name = $"Automation Test {DateTime.Now.ToString("MMMM")}",
            ChallengeDescription = $"Testing Challenge Automation",
            Type = "Growth",
            CollectionID = null,
            CollectionUrl = null,
            CollectionName = null,
            Country = "United States",
            ParticipantType = "Customer",
            Mstpid = "636852",
            Mpnid = null,
            TimeZone = "Dateline Standard Time",
            StartDateStr = "01-31-2020 00:00:01",
            EndDateStr = "02-03-2020 23:59:59",
            SelfRegistrationEnabled = "true",
            CustomCss = @"body {
                    font-family: SegoeUI, sans-serif;
                    background-color: #ffffff;
                    }
                    header {
                    background-color: #ffffff;
                    color:#262626;
                    }
                    header .logo:before {
                    content: url('.. / .. / images / microsoft - theme0.png');
                    }",
            TemplateSelection = "theme0",
            MicrosoftAccountSponsor = "Frank Sposaro",
            Eou = "USA - Northeast",
            AccountType = "Strategic Commercial",
            Teams = "Team1,Team2",
            AllowTeams = "true",
            HasPrizes = "false",
            CreatedBy = "Frank Sposaro"
        };

        public static List<string> Collections = new List<string>
        {
            "https://docs.microsoft.com/en-us/users/drfrank/collections/zy8fd58jq1yn3",
            "https://docs.microsoft.com/en-us/users/drfrank/collections/704b82r0m6zn0",
            "https://docs.microsoft.com/en-us/users/drfrank/collections/584uq7y8ggnrr"
        };
    }
}
