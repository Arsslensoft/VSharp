using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {
    public class EnumDeclaration : Declaration
    {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
            public Identifier _identifier;
			public OptEnumBase _opt_enum_base;
			public OptEnumMemberDeclarations _opt_enum_member_declarations;
			public OptSemicolon _opt_semicolon;
            public Semantic open_brace;
			[Rule("<enum declaration> ::= <opt attributes> <opt modifiers> enum <Identifier> <opt enum base> '{' <opt enum member declarations> '}' <opt semicolon>")]
            public EnumDeclaration(OptAttributes _OptAttributes, OptModifiers _OptModifiers, Semantic _symbol92, Identifier _Identifier, OptEnumBase _OptEnumBase, Semantic _symbol43, OptEnumMemberDeclarations _OptEnumMemberDeclarations, Semantic _symbol47, OptSemicolon _OptSemicolon)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
                _identifier = _Identifier;
				_opt_enum_base = _OptEnumBase;
                open_brace = _symbol43;
				_opt_enum_member_declarations = _OptEnumMemberDeclarations;
				_opt_semicolon = _OptSemicolon;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                var td = rc.currentTypeDefinition = rc.CreateTypeDefinition(this._identifier._Identifier);
                td.Region = rc.MakeRegion(this, _opt_semicolon);
                td.BodyRegion = rc.MakeRegion(open_brace, _opt_semicolon);
                rc.ApplyModifiers(td, _opt_modifiers._Modifiers);


                 td.Kind = TypeKind.Enum;
               
                // attributes
                rc.ConvertAttributes(td.Attributes, _opt_attributes._Attributes);

              
                // base types
                if (_opt_enum_base._type != null)
                    td.BaseTypes.Add(rc.ConvertTypeReference(_opt_enum_base._type, NameLookupMode.BaseTypeReference));

                // members
                if (_opt_enum_member_declarations._enum_member_declarations != null)
                    foreach (var decl in _opt_enum_member_declarations._enum_member_declarations)
                        decl.Resolve(rc);

                rc.currentTypeDefinition = rc.DefaultTypeDefinition;
                td.ApplyInterningProvider(rc.interningProvider);
             
                return true;
            }
}
}
