using System.Collections.Generic;

namespace CSCAutomateLib
{
    public class ContestCollection
    {
        public string Id { get; set; }
        public string ContestId { get; set; }
        public string CollectionID { get; set; }
        public List<CollectionModule> ContestCollectionModulesList { get; set; }
    }
}
