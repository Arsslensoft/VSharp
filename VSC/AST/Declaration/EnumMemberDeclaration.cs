using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using System.Linq;
namespace VSC.AST { 
	public class EnumMemberDeclaration : Declaration {
 			public OptAttributes _opt_attributes;
			public Identifier _identifier;
			public Expression _expression;


			[Rule("<enum member declaration> ::= <opt attributes> <Identifier>")]
			public EnumMemberDeclaration(OptAttributes _OptAttributes,Identifier _Identifier)
				{
				_opt_attributes = _OptAttributes;
				_identifier = _Identifier;
				}
			[Rule("<enum member declaration> ::= <opt attributes> <Identifier> '=' <expression>")]
			public EnumMemberDeclaration(OptAttributes _OptAttributes,Identifier _Identifier, Semantic _symbol60,Expression _Expression)
				{
				_opt_attributes = _OptAttributes;
				_identifier = _Identifier;
				_expression = _Expression;
				}


            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                
                UnresolvedFieldSpec field = new UnresolvedFieldSpec(rc.currentTypeDefinition,_identifier._Identifier);
                field.Region = field.BodyRegion = rc.MakeRegion(this);
                rc.ConvertAttributes(field.Attributes,_opt_attributes._Attributes);

                if (rc.currentTypeDefinition.TypeParameters.Count == 0)
                    field.ReturnType = rc.currentTypeDefinition;
                else
                {
                    ITypeReference[] typeArgs = new ITypeReference[rc.currentTypeDefinition.TypeParameters.Count];
                    for (int i = 0; i < typeArgs.Length; i++)
                    {
                        typeArgs[i] = TypeParameterReference.Create(SymbolKind.TypeDefinition, i);
                    }
                    field.ReturnType = rc.interningProvider.Intern(new ParameterizedTypeReference(rc.currentTypeDefinition, typeArgs));
                }
                field.Accessibility = Accessibility.Public;
                field.IsStatic = true;
                if (_expression != null)
                    field.ConstantValue = rc.ConvertConstantValue(field.ReturnType, _expression);
                else
                {
                    UnresolvedFieldSpec prevField = rc.currentTypeDefinition.Members.LastOrDefault() as UnresolvedFieldSpec;
                    if (prevField == null || prevField.ConstantValue == null)
                        field.ConstantValue = rc.ConvertConstantValue(field.ReturnType, new IntConstant(0, position));
                    else
                        field.ConstantValue = rc.interningProvider.Intern(new IncrementConstantValue(prevField.ConstantValue));
                 
                }

                rc.currentTypeDefinition.Members.Add(field);
                field.ApplyInterningProvider(rc.interningProvider);
                return true;
            } 
}
}
