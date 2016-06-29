using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {
    public class UnionDeclaration : Declaration
    {
        public VSharpUnresolvedTypeDefinition UnresolvedType;
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public TypeDeclarationName _type_declaration_name;
			public OptClassBase _opt_class_base;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;
			public OptClassMemberDeclarations _opt_class_member_declarations;
			public OptSemicolon _opt_semicolon;
            public Semantic open_brace;
            public OptPartial _opt_partial;
            public OptDocumentation _opt_documentation;
            [Rule("<union declaration> ::= <Opt Documentation> <opt attributes> <opt modifiers> <opt partial> union <type declaration name> <opt class base> <opt type parameter constraints clauses> '{' <opt class member declarations> '}' <opt semicolon>")]
            public UnionDeclaration(OptDocumentation _OptDocumentation, OptAttributes _OptAttributes, OptModifiers _OptModifiers, OptPartial _OptPartial, Semantic _symbol156, TypeDeclarationName _TypeDeclarationName, OptClassBase _OptClassBase, OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses, Semantic _symbol43, OptClassMemberDeclarations _OptClassMemberDeclarations, Semantic _symbol47, OptSemicolon _OptSemicolon)
            {
                _opt_documentation = _OptDocumentation;
                _opt_partial = _OptPartial;
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_type_declaration_name = _TypeDeclarationName;
				_opt_class_base = _OptClassBase;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				_opt_class_member_declarations = _OptClassMemberDeclarations;
				_opt_semicolon = _OptSemicolon;
                open_brace = _symbol43;
				}
            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                if (_opt_partial.ispartial)
                    _opt_modifiers._Modifiers |= TypeSystem.Modifiers.PARTIAL;
                var td = rc.currentTypeDefinition = UnresolvedType = rc.CreateTypeDefinition(this._type_declaration_name._identifier._Identifier);
                td.Region = rc.MakeRegion(this, _opt_semicolon);
                td.BodyRegion = rc.MakeRegion(open_brace, _opt_semicolon);
                rc.ApplyModifiers(td, _opt_modifiers._Modifiers);
                // attributes
                rc.ConvertAttributes(td.Attributes, _opt_attributes._Attributes);
                td.Kind = TypeKind.Union;
                td.IsSealed = true; // unions/structs are implicitly sealed
                // type parameters
                if (this._type_declaration_name._opt_type_parameter_list._type_parameters != null)
                    rc.ConvertTypeParameters(td.TypeParameters, this._type_declaration_name._opt_type_parameter_list._type_parameters, _opt_type_parameter_constraints_clauses._type_parameter_constraints_clauses, SymbolKind.TypeDefinition);

                // base types
                if (_opt_class_base._class_base != null && _opt_class_base._class_base._type_list != null)
                    foreach (var baseType in _opt_class_base._class_base._type_list)
                        td.BaseTypes.Add(rc.ConvertTypeReference(baseType, NameLookupMode.BaseTypeReference));
                // documentation
                rc.AddDocumentation(td, _opt_documentation);
                // members
                if (_opt_class_member_declarations._class_member_declarations != null)
                    foreach (var decl in _opt_class_member_declarations._class_member_declarations)
                        decl.Resolve(rc);

                rc.currentTypeDefinition = rc.DefaultTypeDefinition;
                td.ApplyInterningProvider(rc.interningProvider);
                return true;
            }
}
}
