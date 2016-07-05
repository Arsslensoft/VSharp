using System;

namespace VSC.TypeSystem.Implementation
{
    public sealed class ParameterReference : ISymbolReference
    {
        readonly ITypeReference type;
        readonly string name;
        readonly DomRegion region;
        readonly bool isRef, isOut, isParams, isOptional;
        readonly object defaultValue;
		
        public ParameterReference(ITypeReference type, string name, DomRegion region, bool isRef, bool isOut, bool isParams, bool isOptional, object defaultValue)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");
            this.type = type;
            this.name = name;
            this.region = region;
            this.isRef = isRef;
            this.isOut = isOut;
            this.isParams = isParams;
            this.isOptional = isOptional;
            this.defaultValue = defaultValue;
        }

        public ISymbol Resolve(ITypeResolveContext context)
        {
            return new ParameterSpec(type.Resolve(context), name, region: region, isRef: isRef, isOut: isOut, isParams: isParams, isOptional: isOptional, defaultValue: defaultValue);
        }
    }
}