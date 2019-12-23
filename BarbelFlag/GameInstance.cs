using System.Collections.Generic;
using System.Collections;


namespace BarbelFlag
{
    public class GlobalSetting
    {
        public int WinScore = 1000;
        public int MemberCount = 3;
        public int FlagCount = 5;
        public int FlagTicksToCapture = 60000;
    }

    public enum GameStatus
    {
        Initial = 0,
        End = 100
    }

    public class GameInstance
    {
        protected Queue messageQueue;

        public GameStatus Status { get; private set; } 

        protected List<Flag> flags;
        protected GlobalSetting globalSetting;
        protected Dictionary<int, CharacterBase> characters;
        protected Team teamCiri;
        protected Team teamEredin;


        public GameInstance()
        {
            Reset();
        }

        public void Reset()
        {
            LoadGlobalSetting();
            InitCharacters();
            InitTeams();
            InitFlags();
            InitMessageQueue();
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
                flags.Add(new Flag(i, globalSetting.FlagTicksToCapture, this));
            }
        }

        protected void InitMessageQueue()
        {
            messageQueue = new Queue();
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

            if (msgInitCharacter.Faction == TeamFaction.Ciri)
            {
                if (teamCiri.Members.Count >= globalSetting.MemberCount)
                {
                    return new AnswerInitCharacter
                    {
                        Code = ErrorCode.TeamMemberCountLimit,
                        UserId = msgInitCharacter.UserId
                    };
                }
                teamCiri.AddMember(msgInitCharacter.UserId, character);
            }
            else
            {
                if (teamEredin.Members.Count >= globalSetting.MemberCount)
                {
                    return new AnswerInitCharacter
                    {
                        Code = ErrorCode.TeamMemberCountLimit,
                        UserId = msgInitCharacter.UserId
                    };
                }
                teamEredin.AddMember(msgInitCharacter.UserId, character);
            }

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
            var msgAddScore = (MessageAddScore)message;

            var team = teamCiri;
            if (team.Faction != msgAddScore.Faction) team = teamEredin;

            var scoreBefore = team.Score;
            team.AddScore();

            return new AnswerAddScore
            {
                ScoreBefore = scoreBefore,
                ScoreAfter = team.Score
            };
        }

        public void EnqueueMessage(MessageBase message)
        {
            messageQueue.Enqueue(message);
        }

        public void Update()
        {
            //FIXME: concurrency
            while (messageQueue.Count > 0)
            {
                var message = (MessageBase)messageQueue.Dequeue();

                switch (message.MsgType)
                {
                    case MessageType.InitCharacter:
                        HandleInitCharacter(message);
                        return;
                    case MessageType.StartCapture:
                        HandleStartCapture(message);
                        return;
                    case MessageType.AddScore:
                        HandleAddScore(message);
                        return;
                }
            }
        }
    }
}
