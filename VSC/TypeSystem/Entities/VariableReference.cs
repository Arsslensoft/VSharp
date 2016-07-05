using System;

namespace VSC.TypeSystem.Implementation
{
    public sealed class VariableReference : ISymbolReference
    {
        ITypeReference variableTypeReference;
        string name;
        DomRegion region;
        bool isConst;
        object constantValue;
		
        public VariableReference(ITypeReference variableTypeReference, string name, DomRegion region, bool isConst, object constantValue)
        {
            if (variableTypeReference == null)
                throw new ArgumentNullException("variableTypeReference");
            if (name == null)
                throw new ArgumentNullException("name");
            this.variableTypeReference = variableTypeReference;
            this.name = name;
            this.region = region;
            this.isConst = isConst;
            this.constantValue = constantValue;
        }
		
        public ISymbol Resolve(ITypeResolveContext context)
        {
            return new VariableSpec(variableTypeReference.Resolve(context), name, region, isConst, constantValue);
        }
    }
}