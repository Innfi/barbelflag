using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class MessageStartCapture : MessageBase
    {
        public int FlagId;

        public MessageStartCapture()
        {
            MsgType = MessageType.StartCapture;
        }
    }

    public class AnswerStartCapture : AnswerBase
    {
        public AnswerStartCapture()
        {
            MsgType = MessageType.StartCapture;
        }
    }
}
