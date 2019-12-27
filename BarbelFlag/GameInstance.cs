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
        public GameStatus Status { get; private set; } 
        
        protected GlobalSetting globalSetting;
        protected List<Flag> flags;
        protected Dictionary<int, CharacterBase> characters;
        protected Team teamCiri;
        protected Team teamEredin;

        public MessageQueue GameMsgQueue;


        public GameInstance()
        {
            Reset();
        }

        public void Reset()
        {
            Status = GameStatus.Initial;
            LoadGlobalSetting();
            InitMessageQueue();
            InitCharacters();
            InitTeams();
            InitFlags();            
        }

        protected void LoadGlobalSetting()
        {
            this.globalSetting = new GlobalSetting
            {
                MemberCount = 100
            };
        }

        protected void InitCharacters()
        {
            characters = new Dictionary<int, CharacterBase>();
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
                flags.Add(new Flag(i, globalSetting.FlagTicksToCapture, GameMsgQueue));
            }
        }

        protected void InitMessageQueue()
        {
            GameMsgQueue = new MessageQueue();
        }

        public GameInstance(GlobalSetting globalSetting)
        {
            this.globalSetting = globalSetting;

            InitCharacters();
            InitTeams();
            InitFlags();
        }

        public void DummyStatusChanger()
        {
            Status = GameStatus.End;
        }

        public AnswerBase HandleMessage(MessageBase message)
        {
            switch (message.MsgType)
            {
                case MessageType.InitCharacter:
                    return HandleInitCharacter(message);
                case MessageType.LoadTeam:
                    return HandleLoadTeam(message);
                case MessageType.GetFlagsStatus:
                    return HandleGetFlagsStatus(message);
                case MessageType.StartCapture:
                    return HandleStartCapture(message);
                case MessageType.AddScore:
                    return HandleAddScore(message);
                default:
                    return new AnswerInitCharacter();
            }
        }

        protected AnswerBase HandleInitCharacter(MessageBase message)
        {
            var msgInitCharacter = (MessageInitCharacter)message;

            if (characters.TryGetValue(msgInitCharacter.UserId, out var character))
            {
                return new AnswerInitCharacter
                {
                    Code = ErrorCode.UserAlreadyRegistered
                };
            }

            character = GenCharacter(msgInitCharacter.CharType);

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
            team.AddMember(msgInitCharacter.UserId, character);
            characters.Add(msgInitCharacter.UserId, character);

            return new AnswerInitCharacter
            {
                Code = ErrorCode.Ok,
                UserId = 1,
                Character = character,
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

        protected AnswerBase HandleGetFlagsStatus(MessageBase message)
        {
            return new AnswerGetFlagsStatus
            {
                Code = ErrorCode.Ok,
                Flags = this.flags
            };
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

            if(team.Score >= globalSetting.WinScore) Status = GameStatus.End;

            return new AnswerAddScore
            {
                ScoreBefore = scoreBefore,
                ScoreAfter = team.Score
            };
        }

        public void Update()
        {            
            while (GameMsgQueue.Count > 0)
            {
                var message = (MessageBase)GameMsgQueue.Dequeue();

                HandleMessage(message);
            }
        }

        public void Start()
        {
            Status = GameStatus.Started;
        }
    }
}
