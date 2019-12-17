using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class MessageGetFlagsStatus : MessageBase
    {
        public MessageGetFlagsStatus()
        {
            MsgType = MessageType.GetFlagsStatus;
        }
    }

    public class AnswerGetFlagsStatus : AnswerBase
    {
        public List<Flag> Flags;

        public AnswerGetFlagsStatus()
        {
            MsgType = MessageType.GetFlagsStatus;
        }
    }
}
