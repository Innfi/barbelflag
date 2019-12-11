using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BarbelFlag
{
    public class GlobalSetting
    {
        public int WinScore = 1000;
    }

    public enum GameStatus
    {
        Initial = 0,
        End = 100
    }

    public class GameInstance
    {
        public GameStatus Status { get; private set; }
        public void DummyStatusChanger()
        {
            Status = GameStatus.End;
        }
    }

    public class Team
    {
        public class Initializer
        {
            public GlobalSetting Setting;
            public GameInstance Game;
            public int TeamID;
        }

        public int TeamID { get; protected set; }

        private GlobalSetting globalSetting;
        private GameInstance gameInstance;

        public Team(Initializer initializer)
        {
            globalSetting = initializer.Setting;
            gameInstance = initializer.Game;
            TeamID = initializer.TeamID;
        }

        public int Score { get; private set; }

        public void StartCapture(Flag flag)
        {
            flag.CaptureStatus = Flag.FlagCaptureStatus.Capturing;
        }

        public void DoneCapture(Flag flag)
        {
            flag.OwnerTeamID = TeamID;
            flag.CaptureStatus = Flag.FlagCaptureStatus.Captured;
        }

        public void RaiseScoreDummy()
        {
            Score = globalSetting.WinScore;
            gameInstance.DummyStatusChanger();
        }
    }

    public abstract class CharacterBase
    {
        public int MoveSpeed;
        public int Health;
        public int AutoRange;
        public int AutoDamage;
        public float AutoSpeed;
    }

    public class CharacterMilli : CharacterBase
    {
        public CharacterMilli()
        {
            MoveSpeed = 30;
            Health = 150;
            AutoRange = 20;
            AutoDamage = 10;
            AutoSpeed = 0.5f;
        }
    }

    public class CharacterEnnfi : CharacterBase
    {
        public CharacterEnnfi()
        {
            MoveSpeed = 35;
            Health = 100;
            AutoRange = 30;
            AutoDamage = 10;
            AutoSpeed = 0.7f;
        }
    }

    public class CharacterInnfi : CharacterBase
    {
        public CharacterInnfi()
        {
            MoveSpeed = 25;
            Health = 200;
            AutoRange = 20;
            AutoDamage = 15;
            AutoSpeed = 0.6f;
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

        public FlagCaptureStatus CaptureStatus { get; set; }
        public int OwnerTeamID { get; set; }
        public int Score { get; protected set; }


        public Flag()
        {
            OwnerTeamID = 0;
        }

        public void GenScore()
        {
            Score = 10;
        }
    }

    public abstract class MessageBase
    {

    }

    public class MessageCaptureFlagStart : MessageBase
    {

    }
}
