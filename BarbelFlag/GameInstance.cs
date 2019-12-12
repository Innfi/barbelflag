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
        public void DummyStatusChanger()
        {
            Status = GameStatus.End;
        }

        public AnswerBase HandleMessage(MessageBase message)
        {
            return new AnswerInitCharacter
            {
                Code = ErrorCode.Ok,
                UserId = 1,
                Character = new CharacterInnfi(),
                TeamId = 0,
            };
        }
    }
}
