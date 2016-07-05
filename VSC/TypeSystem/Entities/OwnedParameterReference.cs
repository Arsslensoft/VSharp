using System;

namespace VSC.TypeSystem.Implementation
{
    sealed class OwnedParameterReference : ISymbolReference
    {
        readonly IMemberReference memberReference;
        readonly int index;
		
        public OwnedParameterReference(IMemberReference member, int index)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            this.memberReference = member;
            this.index = index;
        }
		
        public ISymbol Resolve(ITypeResolveContext context)
        {
            IParameterizedMember member = memberReference.Resolve(context) as IParameterizedMember;
            if (member != null && index >= 0 && index < member.Parameters.Count)
                return member.Parameters[index];
            else
                return null;
        }
    }
}