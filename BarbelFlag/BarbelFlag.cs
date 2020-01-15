using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


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

    public class GameLoop
    {
        public delegate int UpdateDelegate();
        protected UpdateDelegate updateDelegate;
        protected Stopwatch watch;
        public int DeltaTime { get; protected set; }
        public bool IsRunning { get; protected set; }
        protected Task task;


        public GameLoop(UpdateDelegate callback)
        {
            DeltaTime = 0;
            updateDelegate = callback;
            watch = new Stopwatch();
            IsRunning = false;
            task = new Task(MainLoop);
        }

        public void Start()
        {
            IsRunning = true;
            task.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            task.Wait();
        }

        public void MainLoop()
        {
            while (IsRunning)
            {
                if (DeltaTime >= 0) Thread.Sleep(DeltaTime);
                DeltaTime = 0;

                watch.Start();

                for (int i = 0; i < 60; i++) updateDelegate();

                watch.Stop();

                var loopElapsed = watch.ElapsedMilliseconds;
                DeltaTime += (int)(1000 - loopElapsed);
            }
        }
    }
}
