using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public enum GameStatus
    {
        Initial = 0,
        End = 100
    }

    public class GameInstance
    {
        public GameStatus Status { get; private set; }

        protected Dictionary<int, CharacterBase> characters;
        protected Team team1;
        protected Team team2;


        public GameInstance()
        {
            characters = new Dictionary<int, CharacterBase>();
            team1 = new Team(new Team.Initializer
            {
                TeamID = 1
            });
            team2 = new Team(new Team.Initializer
            {
                TeamID = 2
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
            characters.Add(msgInitCharacter.UserId, character);

            if (msgInitCharacter.TeamId == 1)
            {
                team1.AddMember(msgInitCharacter.UserId, character);
            }
            else
            {
                team2.AddMember(msgInitCharacter.UserId, character);
            }

            return new AnswerInitCharacter
            {
                Code = ErrorCode.Ok,
                UserId = 1,
                Character = character,
                TeamId = msgInitCharacter.TeamId,
            };
        }

        protected AnswerBase HandleLoadTeam(MessageBase message)
        {
            var msgLoadTeam = (MessageLoadTeam)message;

            var members = team1.Members;
            if (msgLoadTeam.TeamId != 1) members = team2.Members;

            return new AnswerLoadTeam
            {
                Code = ErrorCode.Ok,
                TeamMembers = members
            };
        }
    }
}
