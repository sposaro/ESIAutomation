﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSCAutomateLib
{
    public class LearningPath
    {
        public string CollectionName { get; set; }
        public string CollectionUrl { get; set; }

        public string GetCollectionId() =>
            CollectionUrl.Substring(CollectionUrl.LastIndexOf('/') + 1);
    }

    public class ChallengeRequest
    {
        public ContestRequest BaseInputs { get; set; }
        public IList<LearningPath> LearningPaths { get; set; }
    }
}
