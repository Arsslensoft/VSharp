namespace VSC.TypeSystem
{
    public enum SymbolKind : byte
    {
        None,
        /// <seealso cref="ITypeDefinition"/>
        TypeDefinition,
        /// <seealso cref="IField"/>
        Field,
        /// <summary>
        /// The symbol is a property, but not an indexer.
        /// </summary>
        /// <seealso cref="IProperty"/>
        Property,
        /// <summary>
        /// The symbol is an indexer, not a regular property.
        /// </summary>
        /// <seealso cref="IProperty"/>
        Indexer,
        /// <seealso cref="IEvent"/>
        Event,
        /// <summary>
        /// The symbol is a method which is not an operator/constructor/destructor or accessor.
        /// </summary>
        /// <seealso cref="IMethod"/>
        Method,
        /// <summary>
        /// The symbol is a user-defined operator.
        /// </summary>
        /// <seealso cref="IMethod"/>
        Operator,
        /// <seealso cref="IMethod"/>
        Constructor,
        /// <seealso cref="IMethod"/>
        Destructor,
        /// <summary>
        /// The accessor method for a property getter/setter or event add/remove.
        /// </summary>
        /// <seealso cref="IMethod"/>
        Accessor,
        /// <seealso cref="INamespace"/>
        Namespace,
        /// <summary>
        /// The symbol is a variable, but not a parameter.
        /// </summary>
        /// <seealso cref="IVariable"/>
        Variable,
        /// <seealso cref="IParameter"/>
        Parameter,
        /// <seealso cref="ITypeParameter"/>
        TypeParameter,
    }
}