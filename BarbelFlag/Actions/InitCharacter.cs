using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class MessageInitCharacter : MessageBase
    {
        public MessageInitCharacter()
        {
            MsgType = MessageType.InitCharacter;
        }
        public int UserId;
        public CharacterType CharType;
        public TeamFaction Faction;
    }

    public class AnswerInitCharacter : AnswerBase
    {
        public AnswerInitCharacter()
        {
            MsgType = MessageType.InitCharacter;
        }

        public int UserId;
        public CharacterBase Character;
        public TeamFaction Faction;
    }
}
