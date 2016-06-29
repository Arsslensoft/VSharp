using System;
using System.Collections.Generic;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using System.Linq;
namespace VSC.AST {
    public class DelegateDeclaration : Declaration
    {
        public VSharpUnresolvedTypeDefinition UnresolvedType;
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public MemberType _member_type;
			public TypeDeclarationName _type_declaration_name;
			public OptFormalParameterList _opt_formal_parameter_list;
			public OptTypeParameterConstraintsClauses _opt_type_parameter_constraints_clauses;
            public OptDocumentation _opt_documentation;
            [Rule("<delegate declaration> ::= <Opt Documentation> <opt attributes> <opt modifiers> delegate <member type> <type declaration name> '(' <opt formal parameter list> ')' <opt type parameter constraints clauses> ';'")]
            public DelegateDeclaration(OptDocumentation _OptDocumentation, OptAttributes _OptAttributes, OptModifiers _OptModifiers, Semantic _symbol87, MemberType _MemberType, TypeDeclarationName _TypeDeclarationName, Semantic _symbol20, OptFormalParameterList _OptFormalParameterList, Semantic _symbol21, OptTypeParameterConstraintsClauses _OptTypeParameterConstraintsClauses, Semantic _symbol31)
            {
                _opt_documentation = _OptDocumentation;
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_member_type = _MemberType;
				_type_declaration_name = _TypeDeclarationName;
				_opt_formal_parameter_list = _OptFormalParameterList;
				_opt_type_parameter_constraints_clauses = _OptTypeParameterConstraintsClauses;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                var td = rc.currentTypeDefinition = UnresolvedType = rc.CreateTypeDefinition(_type_declaration_name._identifier._Identifier);
                td.Kind = TypeKind.Delegate;
                td.Region = rc.MakeRegion(this);
                td.BaseTypes.Add(KnownTypeReference.MulticastDelegate);
                // documentation
                rc.AddDocumentation(td, _opt_documentation);
                rc.ApplyModifiers(td, _opt_modifiers._Modifiers);
                td.IsSealed = true; // delegates are implicitly sealed

                rc.ConvertTypeParameters(td.TypeParameters, _type_declaration_name._opt_type_parameter_list._type_parameters,_opt_type_parameter_constraints_clauses._type_parameter_constraints_clauses, SymbolKind.TypeDefinition);

                ITypeReference returnType = rc.ConvertTypeReference(_member_type, TypeSystem.Resolver.NameLookupMode.Type);
                List<IUnresolvedParameter> parameters = new List<IUnresolvedParameter>();
                if(_opt_formal_parameter_list._formal_parameter_list != null)
                        rc.ConvertParameters(parameters, _opt_formal_parameter_list._formal_parameter_list._fixed_parameters, _opt_formal_parameter_list._formal_parameter_list._parameter_array);
                rc.AddDefaultMethodsToDelegate(td, returnType, parameters);


                List<IUnresolvedAttribute> returnTypeAttributes = new List<IUnresolvedAttribute>();
                rc.ConvertAttributes(returnTypeAttributes, _opt_attributes._ReturnAttributes);
                IUnresolvedMethod invokeMethod = (IUnresolvedMethod)td.Members.Single(m => m.Name == "Invoke");
       
            
                foreach (IUnresolvedAttribute attr in returnTypeAttributes)
                    invokeMethod.ReturnTypeAttributes.Add(attr);
            

                rc.ConvertAttributes(td.Attributes, _opt_attributes._Attributes);
                //rc.currentTypeDefinition = (VSharpUnresolvedTypeDefinition)rc.currentTypeDefinition.DeclaringTypeDefinition;
                rc.currentTypeDefinition = rc.DefaultTypeDefinition;
                td.ApplyInterningProvider(rc.interningProvider);
                return true;
            }
}
}
