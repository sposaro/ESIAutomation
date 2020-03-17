using System;
using System.Collections.Generic;
using System.Text;

namespace CSCAutomateLib
{
    public class AdditionalInput
    {
        public bool AutoRecurring { get; set; }
        public bool RequestedVouchers { get; set; }
    }

    public class LearningPath
    {
        public string CollectionName { get; set; }
        public string CollectionUrl { get; set; }

        public string GetCollectionId()
        {
            return CollectionUrl.Substring(CollectionUrl.LastIndexOf('/') + 1);
        }
    }

    public class ChallengeRequest
    {
        public IList<AdditionalInput> AdditionalInputs { get; set; }
        public IList<ContestRequest> BaseInputs { get; set; }
        public IList<LearningPath> LearningPaths { get; set; }
    }
}
