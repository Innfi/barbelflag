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
        public Dictionary<int, GameClient> TeamMembers;
        public int Score;

        public AnswerLoadTeam()
        {
            MsgType = MessageType.LoadTeam;
        }
    }
}
