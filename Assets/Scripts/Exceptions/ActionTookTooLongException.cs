using System;

[Serializable]
public class ActionTookTooLongException : Exception {
    public ActionTookTooLongException() : base() { }
    public ActionTookTooLongException(string message) : base(message) { }
    public ActionTookTooLongException(string message, Exception inner) : base(message, inner) { }

    // A constructor is needed for serialization when an
    // exception propagates from a remoting server to the client.
    protected ActionTookTooLongException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
