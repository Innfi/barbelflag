using System.Collections.Generic;


namespace BarbelFlag
{
    public class MessageLoadTeam : MessageBase
    {
        public MessageLoadTeam()
        {
            MsgType = MessageType.LoadTeam;
        }

        public TeamFaction Faction;
    }

    public class AnswerLoadTeam : AnswerBase
    {
        public Dictionary<int, CharacterBase> TeamMembers;

        public AnswerLoadTeam()
        {
            MsgType = MessageType.LoadTeam;
        }
    }
}
