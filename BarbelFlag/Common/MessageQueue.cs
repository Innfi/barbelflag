using System.Collections.Generic;
using System.Collections;


namespace BarbelFlag
{
    public class MessageQueue
    {
        //FIXME: concurrency
        protected Queue messageQueue;

        public int Count { get { return messageQueue.Count; } }

        public MessageQueue()
        {
            messageQueue = new Queue();
        }

        public void EnqueueMessage(MessageBase message)
        {
            messageQueue.Enqueue(message);

            int dummy = 1;
        }

        public MessageBase Dequeue()
        {
            return (MessageBase)messageQueue.Dequeue();
        }
    }
}
