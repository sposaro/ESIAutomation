using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Threading.Tasks;

namespace CSCAutomateLib
{
    public class QueueApi
    {
        #region "Local Variables"
        private readonly QueueClient queueClient;
        #endregion

        #region "Constructor"
        public QueueApi(Configuration config)
        {
            queueClient = new QueueClient(config.StorageConn1, config.QueueName1);
        }
        #endregion

        #region "Public Methods"
        public async Task<Azure.Response<SendReceipt>> SendQueueMessageAync(string queueMessage)
        {
            //byte[] data = Encoding.ASCII.GetBytes(queueMessage);
            //string queueMessageBase64Encoded = System.Convert.ToBase64String(data);
            //return await queueClient.SendMessageAsync(queueMessageBase64Encoded);
            return await queueClient.SendMessageAsync(queueMessage);
        }

        public async Task<Azure.Response<QueueMessage[]>> ReceiveQueueMessageAync()
        {

            return await queueClient.ReceiveMessagesAsync();
        }


        #endregion
    }
}
