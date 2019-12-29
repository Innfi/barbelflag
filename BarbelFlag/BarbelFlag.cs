using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        public Dictionary<int, CharacterBase> Members { get; protected set; }

        protected GlobalSetting globalSetting;
        protected GameInstance gameInstance;


        public Team(Initializer initializer)
        {
            globalSetting = initializer.Setting;
            gameInstance = initializer.Game;
            Faction = initializer.Faction;
            Members = new Dictionary<int, CharacterBase>();
        }

        public void AddMember(int userId, CharacterBase character)
        {
            Members.Add(userId, character);
        }

        public void AddScore()
        {
            Score += 10;
        }
    }

    public class Flag
    {
        public enum FlagCaptureStatus
        {
            Initial = 0,
            Capturing = 5,
            Captured = 10
        }

        public int FlagId { get; protected set; }
        public FlagCaptureStatus CaptureStatus { get; protected set; }
        public TeamFaction OwnerTeamFaction { get; set; }

        protected int tickLimit;
        protected int tickCurrent;
        protected MessageQueue messageQueue;


        public Flag(int flagId, int limit, MessageQueue queue)
        {
            FlagId = flagId;
            OwnerTeamFaction = TeamFaction.None;
            tickLimit = limit;
            tickCurrent = 0;
            messageQueue = queue;
        }

        public void StartCapture(TeamFaction faction)
        {
            CaptureStatus = Flag.FlagCaptureStatus.Capturing;
            OwnerTeamFaction = faction;
        }

        public void Tick()
        {
            tickCurrent += 6000;
            if (tickCurrent < tickLimit) return;

            if (CaptureStatus == FlagCaptureStatus.Capturing)
            {
                CaptureStatus = FlagCaptureStatus.Captured;
            }
            else if (CaptureStatus == FlagCaptureStatus.Captured)
            {
                messageQueue.EnqueueMessage(new MessageAddScore
                {
                    Faction = OwnerTeamFaction
                });
            }

            tickCurrent = 0;
        }
    }       
}
