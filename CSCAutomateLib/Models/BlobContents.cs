using System;
using System.Collections.Generic;
using System.Text;

namespace CSCAutomateLib.Models
{
    public class BlobContents
    {
        public ContestRequest BaseInputs { get; set; }
        public IList<ContestResponse> Contests { get; set; }
    }
}
