using System;

[Serializable]
public class PlayerOutOfActionsException : Exception
{
    public PlayerOutOfActionsException() : base() { }
    public PlayerOutOfActionsException(string message) : base(message) { }
    public PlayerOutOfActionsException(string message, Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected PlayerOutOfActionsException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
