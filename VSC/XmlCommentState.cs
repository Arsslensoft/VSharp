namespace VSC
{
    public enum XmlCommentState
    {
        // comment is allowed in this state.
        Allowed,
        // comment is not allowed in this state.
        NotAllowed,
        // once comments appeared when it is NotAllowed, then the
        // state is changed to it, until the state is changed to
        // .Allowed.
        Error
    }
}