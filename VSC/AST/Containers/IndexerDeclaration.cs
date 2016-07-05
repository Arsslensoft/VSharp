using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    [Serializable]
    public class IndexerDeclaration : PropertyOrIndexer
    {
        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE |
            Modifiers.VIRTUAL |
            Modifiers.SEALED |
            Modifiers.OVERRIDE |
            Modifiers.EXTERN |
            Modifiers.ABSTRACT;

        const Modifiers AllowedInterfaceModifiers =
            Modifiers.NEW;


        readonly ParametersCompiled parameters;
        public IndexerDeclaration(TypeContainer parent, FullNamedExpression type, MemberName name, Modifiers mod, ParametersCompiled parameters, VSharpAttributes attrs)
            : base(parent, type, mod,
                parent is InterfaceDeclaration ? AllowedInterfaceModifiers : AllowedModifiers,
                name, attrs)
        {
            this.parameters = parameters;
        }


        #region Properties

        AParametersCollection Parameters
        {
            get
            {
                return parameters;
            }
        }

        public ParametersCompiled ParameterInfo
        {
            get
            {
                return parameters;
            }
        }

        #endregion
    }
}