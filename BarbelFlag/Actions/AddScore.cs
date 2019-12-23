using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class MessageAddScore : MessageBase
    {
        public TeamFaction Faction;

        public MessageAddScore()
        {
            MsgType = MessageType.AddScore;
        }
    }

    public class AnswerAddScore : AnswerBase
    {
        public int ScoreAfter;
        public int ScoreBefore;

        public AnswerAddScore()
        {
            MsgType = MessageType.AddScore;
        }
    }
}
