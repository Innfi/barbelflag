using System.Collections.Generic;


namespace BarbelFlag
{
    public enum TeamFaction
    {
        None = 0,
        Ciri = 1,
        Eredin = 2
    };

    public class Team
    {
        public class Initializer
        {
            public GlobalSetting Setting;
            public GameInstance Game;
            public TeamFaction Faction;
        }

        public TeamFaction Faction { get; protected set; }
        public int Score { get; private set; }
        public Dictionary<int, GameClient> Members { get; protected set; }

        protected GlobalSetting globalSetting;
        protected GameInstance gameInstance;


        public Team(Initializer initializer)
        {
            globalSetting = initializer.Setting;
            gameInstance = initializer.Game;
            Faction = initializer.Faction;
            Members = new Dictionary<int, GameClient>();
        }

        public void AddMember(int userId, GameClient client)
        {
            Members.Add(userId, client);
        }

        public void AddScore()
        {
            Score += 10;
        }
    }

    public class MessageMoveCharacter : MessageBase
    {
        public ObjectPosition BeforePos;
        public ObjectPosition TargetPos;
        public int UserId;

        public MessageMoveCharacter()
        {
            MsgType = MessageType.MoveCharacter;
        }
    }

    public class AnswerMoveCharacter : AnswerBase
    {
        public ObjectPosition ResultPos;

        public AnswerMoveCharacter()
        {
            MsgType = MessageType.MoveCharacter;
        }
    }
}
