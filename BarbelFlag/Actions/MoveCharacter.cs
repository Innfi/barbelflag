using System;


namespace BarbelFlag
{
    public class MessageMoveCharacter : MessageBase
    {
        public ObjectPosition BeforePos;
        public ObjectPosition TargetPos;
        public int UserId;

        public MessageMoveCharacter()
        {
            MsgType = MessageType.MoveCharacter;
        }
    }

    public class AnswerMoveCharacter : AnswerBase
    {
        public ObjectPosition ResultPos;

        public AnswerMoveCharacter()
        {
            MsgType = MessageType.MoveCharacter;
        }
    }
}
