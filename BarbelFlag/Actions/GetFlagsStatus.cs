using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class MessageGetFlagViews : MessageBase
    {
        public MessageGetFlagViews()
        {
            MsgType = MessageType.GetFlagViews;
        }
    }

    public class AnswerGetFlagViews : AnswerBase
    {
        public List<FlagView> FlagViews;

        public AnswerGetFlagViews()
        {
            MsgType = MessageType.GetFlagViews;
        }
    }
}
