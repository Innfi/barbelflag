using System.Collections.Generic;
using System.Collections;


namespace BarbelFlag
{
    public class GlobalSetting
    {
        public int WinScore = 100;
        public int MemberCount = 3;
        public int FlagCount = 5;
        public int FlagTicksToCapture = 60000;
    }

    public enum GameStatus
    {
        Initial = 0,
        Started = 10,
        End = 100
    }

    public class GameInstance
    {
        public GameStatus Status { get; protected set; } 
        
        protected GlobalSetting globalSetting;
        protected List<Flag> flags;
        protected Team teamCiri;
        protected Team teamEredin;

        public MessageQueue MsgQ { get; protected set; }

        protected Dictionary<int, GameClient> gameClients;


        public GameInstance()
        {
            LoadGlobalSetting();
            Reset();
        }

        public GameInstance(GlobalSetting globalSetting)
        {
            this.globalSetting = globalSetting;
            Reset();
        }

        public void Reset()
        {
            Status = GameStatus.Initial;
            
            InitMessageQueue();
            InitTeams();
            InitFlags();
            InitGameClients();
        }

        protected void LoadGlobalSetting()
        {
            this.globalSetting = new GlobalSetting
            {
                MemberCount = 100
            };
        }

        protected void InitTeams()
        {
            teamCiri = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Ciri
            });
            teamEredin = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Eredin
            });
        }

        protected void InitFlags()
        {
            flags = new List<Flag>();
            for (int i = 0; i < globalSetting.FlagCount; i++)
            {
                flags.Add(new Flag(i, globalSetting.FlagTicksToCapture, MsgQ));
            }
        }

        protected void InitMessageQueue()
        {
            MsgQ = new MessageQueue();
        }

        protected void InitGameClients()
        {
            gameClients = new Dictionary<int, GameClient>();
        }

        public void EnqueueMessage(MessageBase message)
        {
            MsgQ.EnqueueMessage(message);
        }

        public int Update()
        {
            foreach (var flag in flags) flag.Tick();

            while (MsgQ.Count > 0)
            {
                var message = (MessageBase)MsgQ.Dequeue();

                var answer = HandleMessage(message);
                TrySendAnswerToGameClient(message.SenderUserId, answer);
            }

            return 0;
        }

        protected AnswerBase HandleMessage(MessageBase message)
        {
            switch (message.MsgType)
            {
                case MessageType.AddGameClient:
                    return HandleAddGameClient(message);
                case MessageType.InitCharacter:
                    return HandleInitCharacter(message);
                case MessageType.LoadTeam:
                    return HandleLoadTeam(message);
                case MessageType.GetFlagViews:
                    return HandleGetFlagViews(message);
                case MessageType.StartCapture:
                    return HandleStartCapture(message);
                case MessageType.AddScore:
                    return HandleAddScore(message);
                default:
                    return new AnswerBase { Code = ErrorCode.InvalidMessageType };
            }
        }

        protected AnswerBase HandleAddGameClient(MessageBase message)
        {
            var msg = (MessageAddGameClient)message;
            var gameClient = msg.gameClient;
            gameClient.SetMessageQueue(MsgQ);

            gameClients.Add(gameClient.UserId, gameClient);

            return new AnswerAddGameClient
            {
                Code = ErrorCode.Ok
            };
        }

        protected AnswerBase HandleInitCharacter(MessageBase message)
        {
            var msgInitCharacter = (MessageInitCharacter)message;

            if (!gameClients.TryGetValue(msgInitCharacter.UserId, out var gameClient))
            {
                return new AnswerInitCharacter
                {
                    Code = ErrorCode.GameClientNotExist
                };
            }

            if (gameClient.Character != null)
            {
                return new AnswerInitCharacter
                {
                    Code = ErrorCode.UserAlreadyRegistered
                };
            }

            var team = teamCiri;
            if (msgInitCharacter.Faction != TeamFaction.Ciri) team = teamEredin;

            if (team.Members.Count >= globalSetting.MemberCount)
            {
                return new AnswerInitCharacter
                {
                    Code = ErrorCode.TeamMemberCountLimit,
                    UserId = msgInitCharacter.UserId
                };
            }

            team.AddMember(gameClient.UserId, gameClient);

            return new AnswerInitCharacter
            {
                Code = ErrorCode.Ok,
                UserId = msgInitCharacter.UserId,
                Character = GenCharacter(msgInitCharacter.CharType),
                Faction = msgInitCharacter.Faction,
            };
        }

        protected CharacterBase GenCharacter(CharacterType type)
        {
            switch (type)
            {
                case CharacterType.Innfi:
                    return new CharacterInnfi();
                case CharacterType.Ennfi:
                    return new CharacterEnnfi();
                case CharacterType.Milli:
                    return new CharacterMilli();
                default:
                    return new CharacterInnfi();
            }
        }

        protected AnswerBase HandleLoadTeam(MessageBase message)
        {
            var msgLoadTeam = (MessageLoadTeam)message;

            var team = teamCiri;
            if (msgLoadTeam.Faction != TeamFaction.Ciri) team = teamEredin;

            return new AnswerLoadTeam
            {
                Code = ErrorCode.Ok,
                TeamMembers = team.Members,
                Score = team.Score
            };
        }

        protected AnswerBase HandleGetFlagViews(MessageBase message)
        {
            return new AnswerGetFlagViews
            {
                Code = ErrorCode.Ok,
                FlagViews = ToFlagViews()
            };
        }

        protected List<FlagView> ToFlagViews()
        {
            var views = new List<FlagView>();

            foreach (var flag in flags) views.Add(flag.ToFlagView());

            return views;
        }

        protected AnswerBase HandleStartCapture(MessageBase message)
        {
            if (Status != GameStatus.Started)
            {
                return new AnswerStartCapture
                {
                    Code = ErrorCode.GameNotStarted
                };
            }

            var msgStartCapture = (MessageStartCapture)message;
            var flagId = msgStartCapture.FlagId;

            var flag = flags.Find(x => x.FlagId == flagId);
            flag.StartCapture(msgStartCapture.Faction);

            return new AnswerStartCapture
            {
                Code = ErrorCode.Ok
            };
        }

        protected AnswerBase HandleAddScore(MessageBase message)
        {
            if (Status == GameStatus.End)
            {
                return new AnswerAddScore
                {
                    Code = ErrorCode.GameEnd
                };
            }

            var msgAddScore = (MessageAddScore)message;

            var team = teamCiri;
            if (team.Faction != msgAddScore.Faction) team = teamEredin;

            var scoreBefore = team.Score;
            team.AddScore();

            if (team.Score >= globalSetting.WinScore) Status = GameStatus.End;

            return new AnswerAddScore
            {
                ScoreBefore = scoreBefore,
                ScoreAfter = team.Score
            };
        }

        protected void TrySendAnswerToGameClient(int userId, AnswerBase answer)
        {
            if (userId <= 0) return;

            if (!gameClients.TryGetValue(userId, out var client)) return;

            client.HandleAnswer(answer);
        }

        public void Start()
        {
            Status = GameStatus.Started;
        }

        public void AddClient(GameClient client)
        {
            gameClients.Add(client.UserId, client);
        }
    }
}
