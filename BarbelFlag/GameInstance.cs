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
        public int MemberCount = 3;
        public int FlagCount = 5;
    }

    public enum GameStatus
    {
        Initial = 0,
        End = 100
    }

    public class GameInstance
    {
        public GameStatus Status { get; private set; }        
        protected List<Flag> Flags;

        protected GlobalSetting globalSetting;
        protected Dictionary<int, CharacterBase> characters;
        protected Team teamCiri;
        protected Team teamEredin;


        public GameInstance()
        {
            LoadGlobalSetting();
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
            Flags = new List<Flag>();
            for (int i = 0; i < globalSetting.FlagCount; i++)
            {
                Flags.Add(new Flag(i));
            }
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

            character = new CharacterInnfi();        

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

        protected AnswerBase HandleLoadTeam(MessageBase message)
        {
            var msgLoadTeam = (MessageLoadTeam)message;

            var members = teamCiri.Members;
            if (msgLoadTeam.Faction != TeamFaction.Ciri) members = teamEredin.Members;

            return new AnswerLoadTeam
            {
                Code = ErrorCode.Ok,
                TeamMembers = members
            };
        }

        protected AnswerBase HandleGetFlagsStatus(MessageBase message)
        {
            return new AnswerGetFlagsStatus
            {
                Code = ErrorCode.Ok,
                Flags = this.Flags
            };
        }

        protected AnswerBase HandleStartCapture(MessageBase message)
        {
            var msgStartCapture = (MessageStartCapture)message;
            var flagId = msgStartCapture.FlagId;

            var flag = Flags.Find(x => x.FlagId == flagId);

            flag.CaptureStatus = Flag.FlagCaptureStatus.Capturing;

            return new AnswerStartCapture
            {
                Code = ErrorCode.Ok
            };
        }
    }
}
