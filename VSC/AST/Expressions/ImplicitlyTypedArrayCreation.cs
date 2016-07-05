namespace VSC.AST
{
    class ImplicitlyTypedArrayCreation : ArrayCreation
    {
        public ImplicitlyTypedArrayCreation(ComposedTypeSpecifier rank, ArrayInitializer initializers, Location loc)
            : base(null, rank, initializers, loc)
        {
        }

        public ImplicitlyTypedArrayCreation(ArrayInitializer initializers, Location loc)
            : base(null, initializers, loc)
        {
        }

    }
}