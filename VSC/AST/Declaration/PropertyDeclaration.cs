using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
namespace VSC.AST {
    public class PropertyDeclaration : Declaration
    {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
            public MethodDeclarationName _member_declaration_name;
			public AccessorDeclarations _accessor_declarations;
			public OptPropertyInitializer _opt_property_initializer;
			public ExpressionBlock _expression_block;

            [Rule("<property declaration> ::= <opt attributes> <opt modifiers> <member type> <method declaration name> '{' <accessor declarations> '}' <opt property initializer>")]
            public PropertyDeclaration(OptAttributes _OptAttributes, OptModifiers _OptModifiers, MemberType _MemberType, MethodDeclarationName _MemberDeclarationName, Semantic _symbol43, AccessorDeclarations _AccessorDeclarations, Semantic _symbol47, OptPropertyInitializer _OptPropertyInitializer)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_member_declaration_name = _MemberDeclarationName;
				_accessor_declarations = _AccessorDeclarations;
				_opt_property_initializer = _OptPropertyInitializer;
				}
            [Rule("<property declaration> ::= <opt attributes> <opt modifiers> <member type> <method declaration name> <expression block>")]
			public PropertyDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,MethodDeclarationName _MemberDeclarationName,ExpressionBlock _ExpressionBlock)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_member_declaration_name = _MemberDeclarationName;
				_expression_block = _ExpressionBlock;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                string name = "";
                if (_member_declaration_name._type_declaration_name != null)
                    name = _member_declaration_name._type_declaration_name._identifier._Identifier;
                else
                    name = _member_declaration_name._identifier._Identifier;


                UnresolvedPropertySpec p = new UnresolvedPropertySpec(rc.currentTypeDefinition, name);
                p.Region = rc.MakeRegion(this);
                //p.BodyRegion = rc.MakeRegion();
                // modifiers
                rc.ApplyModifiers(p, _opt_modifiers._Modifiers);
                // return
                p.ReturnType = rc.ConvertTypeReference(_member_type, TypeSystem.Resolver.NameLookupMode.Type);

                // attributes
                rc.ConvertAttributes(p.Attributes, _opt_attributes._Attributes);

                // explicit interface
                if (_member_declaration_name._explicit_interface != null)
                {
                    p.Accessibility = Accessibility.None;
                    p.IsExplicitInterfaceImplementation = true;
                    p.ExplicitInterfaceImplementations.Add(rc.interningProvider.Intern(new MemberReferenceSpec(
                        p.SymbolKind, rc.ConvertTypeReference(_member_declaration_name._explicit_interface, TypeSystem.Resolver.NameLookupMode.Type), p.Name)));
                }

                // extern
                bool isExtern = (_opt_modifiers._Modifiers & TypeSystem.Modifiers.EXTERN) == TypeSystem.Modifiers.EXTERN;


                // accessors
                if(_accessor_declarations._get_accessor_declaration != null)
                    p.Getter = rc.ConvertAccessor(_accessor_declarations._get_accessor_declaration, p,  isExtern);
                if(_accessor_declarations._set_accessor_declaration != null)
                    p.Setter = rc.ConvertAccessor(_accessor_declarations._set_accessor_declaration, p, isExtern);

                // add to resolver
                rc.currentTypeDefinition.Members.Add(p);
                p.ApplyInterningProvider(rc.interningProvider);
                return true;
            }
}
}
