namespace VSC.TypeSystem.Resolver
{
    public enum DynamicInvocationType {
        /// <summary>
        /// The invocation is a normal invocation ( 'a(b)' ).
        /// </summary>
        Invocation,

        /// <summary>
        /// The invocation is an indexing ( 'a[b]' ).
        /// </summary>
        Indexing,

        /// <summary>
        /// The invocation is an object creation ( 'new a(b)' ). Also used when invoking a base constructor ( ' : base(a) ' ) and chaining constructors ( ' : this(a) ').
        /// </summary>
        ObjectCreation,
    }
}