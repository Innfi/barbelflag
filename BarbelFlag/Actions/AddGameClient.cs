

namespace BarbelFlag
{
    public class MessageAddGameClient : MessageBase
    {
        public GameClient gameClient;

        public MessageAddGameClient()
        {
            MsgType = MessageType.AddGameClient;
        }
    };

    public class AnswerAddGameClient : AnswerBase
    {
        public AnswerAddGameClient()
        {
            MsgType = MessageType.AddGameClient;
        }
    }
}
