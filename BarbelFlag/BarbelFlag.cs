﻿using System.Collections.Generic;


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

    public class GameClient
    {
        public int UserId { get; protected set; }
        public CharacterBase Character { get; protected set; }
        public AnswerBase LastAnswer { get; protected set; }

        protected MessageQueue msgQ;

        public GameClient(int userId, MessageQueue queue)
        {
            UserId = userId;
            SetMessageQueue(queue);
        }

        public GameClient(int userId)
        {
            UserId = userId;
        }

        public void SetMessageQueue(MessageQueue queue)
        {
            msgQ = queue;
        }

        public void SendDummyMessage(MessageBase message)
        {
            msgQ.EnqueueMessage(message);
        }

        public void HandleAnswer(AnswerBase answer)
        {
            LastAnswer = answer;

            switch (answer.MsgType)
            {
                case MessageType.InitCharacter:
                    HandleAnswerInitCharacter((AnswerInitCharacter)answer);
                    return;
            }
        }

        protected void HandleAnswerInitCharacter(AnswerInitCharacter answer)
        {
            Character = answer.Character;
        }

        public void ClearLastAnswer()
        {
            //test method
            LastAnswer = null;
        }
    }

    public class MessageAddGameClient : MessageBase
    {
        public GameClient gameClient;

        public MessageAddGameClient()
        {
            MsgType = MessageType.AddGameClient;    
        }
    };

    public class AnswerAddGameClient : AnswerBase
    {
        public AnswerAddGameClient()
        {
            MsgType = MessageType.AddGameClient;
        }
    }

    public class ObjectPosition
    {
        public double PosX;
        public double PosY;
        public double PosZ;

        public ObjectPosition(double x, double y, double z)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
        }
    }
}
