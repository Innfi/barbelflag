

namespace BarbelFlag
{
    public enum MessageType
    {
        //None = 0,
        InitCharacter = 1,
        LoadTeam = 2,
        GetFlagsStatus = 3,
        StartCapture = 4,
        AddScore = 5,
    }

    public enum ErrorCode
    {
        Ok = 0,
        UserAlreadyRegistered = 11,
        TeamMemberCountLimit = 12,
        GameNotStarted = 100,
        GameEnd = 200,
        //InvalidMessageType = 999
    }

    public abstract class MessageBase
    {
        public MessageType MsgType { get; protected set; }
    }

    public abstract class AnswerBase
    {
        public MessageType MsgType { get; protected set; }
        public ErrorCode Code;
    }
}
