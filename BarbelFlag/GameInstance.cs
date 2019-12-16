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
    }

    public enum GameStatus
    {
        Initial = 0,
        End = 100
    }

    public class GameInstance
    {
        public GameStatus Status { get; private set; }
        protected GlobalSetting globalSetting;
        protected Dictionary<int, CharacterBase> characters;
        protected Team teamCiri;
        protected Team teamEredin;


        public GameInstance()
        {
            this.globalSetting = new GlobalSetting
            {
                MemberCount = 100
            };
            characters = new Dictionary<int, CharacterBase>();
            teamCiri = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Ciri
            });
            teamEredin = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Eredin
            });
        }

        public GameInstance(GlobalSetting globalSetting)
        {
            this.globalSetting = globalSetting;
            characters = new Dictionary<int, CharacterBase>();
            teamCiri = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Ciri
            });
            teamEredin = new Team(new Team.Initializer
            {
                Faction = TeamFaction.Eredin
            });
        }

        public void DummyStatusChanger()
        {
            Status = GameStatus.End;
        }

        public AnswerBase HandleMessage(MessageBase message)
        {
            if (message.MsgType == MessageType.InitCharacter)
            {
                return HandleInitCharacter(message);
            }
            else if (message.MsgType == MessageType.LoadTeam)
            {
                return HandleLoadTeam(message);
            }

            return new AnswerInitCharacter();
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
    }
}
