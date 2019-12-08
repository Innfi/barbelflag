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
        }

        private GlobalSetting globalSetting;
        private GameInstance gameInstance;

        public Team(Initializer initializer)
        {
            globalSetting = initializer.Setting;
            gameInstance = initializer.Game;
        }

        public int Score { get; private set; }

        public void RaiseScoreDummy()
        {
            Score = globalSetting.WinScore;
            gameInstance.DummyStatusChanger();
        }
    }

    //public class Field
    //{
    //}


    public static class BarbelFlagMain
    {
        public static void Main()
        {

        }
    }
}
