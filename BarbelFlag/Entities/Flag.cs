

namespace BarbelFlag
{
    public class FlagView
    {
        public int FlagId;
        public Flag.FlagCaptureStatus CaptureStatus;
        public TeamFaction OwnerTeamFaction;
    };

    public class Flag
    {
        public enum FlagCaptureStatus
        {
            Initial = 0,
            Capturing = 5,
            Captured = 10
        }

        public int FlagId { get; protected set; }
        protected FlagCaptureStatus CaptureStatus;
        protected TeamFaction OwnerTeamFaction;
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

        public FlagView ToFlagView()
        {
            return new FlagView
            {
                FlagId = FlagId,
                CaptureStatus = CaptureStatus,
                OwnerTeamFaction = OwnerTeamFaction
            };
        }
    }
}
