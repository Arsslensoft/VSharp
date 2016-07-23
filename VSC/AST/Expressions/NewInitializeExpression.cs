using System.Collections.Generic;

namespace VSC.AST
{
    public class NewInitializeExpression : NewExpression
    {
        CollectionOrObjectInitializers initializers;
        public NewInitializeExpression(FullNamedExpression requested_type, Arguments arguments, CollectionOrObjectInitializers initializers, Location l)
            : base(requested_type, arguments, l)
        {
            this.initializers = initializers;
        }

        public CollectionOrObjectInitializers Initializers
        {
            get
            {
                return initializers;
            }
        }


        public override VSC.AST.Expression DoResolve(VSC.TypeSystem.Resolver.ResolveContext rc)
        {
            if (_resolved)
                return this;

            if (!ResolveCommon(rc))
                return null;

            string[] argumentNames;
            Expression[] rarguments = Arguments.GetArguments(out argumentNames);
            initializers = (CollectionOrObjectInitializers)initializers.DoResolve(rc);
            if (initializers == null)
                return null;

            return ResolveObjectCreation(rc, loc, ResolvedType, rarguments, argumentNames, false, initializers.Initializers);
        }
    }
}