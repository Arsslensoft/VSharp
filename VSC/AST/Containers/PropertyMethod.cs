using VSC.TypeSystem;

namespace VSC.AST
{
    public class PropertyMethod : MethodOrOperator
    {
        const Modifiers AllowedModifiers =
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;
        public PropertyMethod(PropertyOrIndexer method, Modifiers modifiers, MemberName name,ParametersCompiled parameters,VSharpAttributes attrs)
            : base(method.Parent, method.ReturnType, modifiers,AllowedModifiers, name,parameters, attrs )
        {
       
        }
        public PropertyMethod(PropertyOrIndexer method, ITypeReference returnType,Modifiers modifiers, MemberName name,ParametersCompiled parameters, VSharpAttributes attrs)
            : base(method.Parent, returnType, modifiers, AllowedModifiers, name, parameters, attrs)
        {

        }
    }
}