using System;

namespace CSCAutomate
{
    class ContestTests
    {
        public static Contest TestContest => new Contest
        {
            name = $"Automation Test {DateTime.Now.ToString("MMMM")}",
            challengeDescription = $"Testing Challenge Automation",
            type = "Growth",
            collectionID = null,
            collectionUrl = null,
            collectionName = null,
            country = "United States",
            participantType = "Customer",
            mstpid = "636852",
            mpnid = null,
            timeZone = "Dateline Standard Time",
            StartDateStr = "01-31-2020 00:00:00",
            EndDateStr = "02-03-2020 23:59:59",
            selfRegistrationEnabled = "true",
            customCss = null,
            templateSelection = "theme0",
            microsoftAccountSponsor = "Frank Sposaro",
            eou = "USA - Northeast",
            accountType = "Strategic Commercial",
            teams = "team1, team2",
            allowTeams = "true",
            hasPrizes = "false",
            createdBy = "Dr. Frank Sposaro"
        };
    }
}

/*
 @"body {
                    font-family: SegoeUI, sans-serif;
                    background-color: #ffffff;
                    }
                    header {
                    background-color: #ffffff;
                    color:#262626;
                    }
                    header .logo:before {
                    content: url('.. / .. / images / microsoft - theme0.png');
                    }"


    */
