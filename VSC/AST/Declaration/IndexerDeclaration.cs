using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
namespace VSC.AST {
    public class IndexerDeclaration : Declaration
    {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public IndexerDeclarationName _indexer_declaration_name;
			public OptFormalParameterList _opt_formal_parameter_list;
			public IndexerBody _indexer_body;

			[Rule("<indexer declaration> ::= <opt attributes> <opt modifiers> <member type> <indexer declaration name> '[' <opt formal parameter list> ']' <indexer body>")]
			public IndexerDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,MemberType _MemberType,IndexerDeclarationName _IndexerDeclarationName, Semantic _symbol37,OptFormalParameterList _OptFormalParameterList, Semantic _symbol40,IndexerBody _IndexerBody)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_indexer_declaration_name = _IndexerDeclarationName;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_indexer_body = _IndexerBody;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                UnresolvedPropertySpec p = new UnresolvedPropertySpec(rc.currentTypeDefinition, "Item");
                p.SymbolKind = SymbolKind.Indexer;
                p.Region = rc.MakeRegion(this);
                //p.BodyRegion = MakeBraceRegion();
                // modifiers
                rc.ApplyModifiers(p, _opt_modifiers._Modifiers);
                // return
                p.ReturnType = rc.ConvertTypeReference(_member_type, TypeSystem.Resolver.NameLookupMode.Type);

                //attributes
                rc.ConvertAttributes(p.Attributes, _opt_attributes._Attributes);
               

                // parameters
                if(_opt_formal_parameter_list._formal_parameter_list != null)
                    rc.ConvertParameters(p.Parameters, _opt_formal_parameter_list._formal_parameter_list._fixed_parameters, _opt_formal_parameter_list._formal_parameter_list._parameter_array);

                // explicit interface
                if (_indexer_declaration_name._explicit_interface != null)
                {
                    p.Accessibility = Accessibility.None;
                    p.IsExplicitInterfaceImplementation = true;
                    p.ExplicitInterfaceImplementations.Add(rc.interningProvider.Intern(new MemberReferenceSpec(
                        p.SymbolKind, rc.ConvertTypeReference(_indexer_declaration_name._explicit_interface, TypeSystem.Resolver.NameLookupMode.Type), p.Name, 0, rc.GetParameterTypes(p.Parameters))));
                }


                // extern
                bool isExtern = (_opt_modifiers._Modifiers & TypeSystem.Modifiers.EXTERN) == TypeSystem.Modifiers.EXTERN;


                // accessors
                if ( _indexer_body._accessor_declarations._get_accessor_declaration != null)
                    p.Getter = rc.ConvertAccessor(_indexer_body._accessor_declarations._get_accessor_declaration, p, isExtern);
                if (_indexer_body._accessor_declarations._set_accessor_declaration != null)
                    p.Setter = rc.ConvertAccessor(_indexer_body._accessor_declarations._set_accessor_declaration, p, isExtern);

                // add to resolver
                rc.currentTypeDefinition.Members.Add(p);
                p.ApplyInterningProvider(rc.interningProvider);
                return true;
            }
}
}
