//using Azure.Storage.Blobs.Models;
//using CSCAutomateLib;
//using Microsoft.Azure.WebJobs;
//using Microsoft.Build.Framework;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace CSCAutomateFunction
//{
//    public static class CronJob
//    {
//        [FunctionName("TimerTriggerCSharp")]
//        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
//        {
//            // Get all blob names
//            BlobApi blobApi = await BlobApi.Instance;
//            Tuple<IList<ContestResponse>, string> tuple = await blobApi.GetAllContestTupleAsync(string.Empty);

//            foreach (BlobItem blob in tuple)
//            {
//                var contests = await blobApi.GetBlobContentsAync<List<ContestResponse>>(blob.Name);

//                foreach (ContestResponse contest in contests)
//                {
//                    // if in past
//                    DateTime endDate = DateTime.Parse(contest.EndDateStr);
//                    if (endDate < DateTime.Now)
//                    {
//                        // Copy update
//                        // create new
//                        // delete
//                    }
//                }
//            }
//        }
//    }
//}
