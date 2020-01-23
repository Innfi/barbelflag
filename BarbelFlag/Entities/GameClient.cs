using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbelFlag
{
    public class GameClient
    {
        public int UserId { get; protected set; }
        public CharacterBase Character { get; protected set; }
        public AnswerBase LastAnswer { get; protected set; }

        protected MessageQueue msgQ;

        public GameClient(int userId, MessageQueue queue)
        {
            UserId = userId;
            SetMessageQueue(queue);
        }

        public GameClient(int userId)
        {
            UserId = userId;
        }

        public void SetMessageQueue(MessageQueue queue)
        {
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

    public class ObjectPosition
    {
        public double PosX;
        public double PosY;
        public double PosZ;

        public ObjectPosition(double x, double y, double z)
        {
            PosX = x;
            PosY = y;
            PosZ = z;
        }

        public override bool Equals(object obj)
        {
            var rhs = (ObjectPosition)obj;

            return (PosX == rhs.PosX && PosY == rhs.PosY && PosZ == rhs.PosZ);
        }
    }
}
