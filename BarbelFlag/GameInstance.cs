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


        public GameInstance()
        {
            characters = new Dictionary<int, CharacterBase>();
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

            return new AnswerInitCharacter
            {
                Code = ErrorCode.Ok,
                UserId = 1,
                Character = character,
                TeamId = 0,
            };
        }
    }
}
