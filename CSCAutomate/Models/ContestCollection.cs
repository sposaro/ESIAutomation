using System;
using System.Collections.Generic;
using System.Text;

namespace CSCAutomate
{
    public class ContestCollection
    {
        public string id { get; set; }
        public string contestId { get; set; }
        public object collectionID { get; set; }
        public List<object> contestCollectionModulesList { get; set; }
    }
}
