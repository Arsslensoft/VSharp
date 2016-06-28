using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;
using System.Linq;
using VSC.TypeSystem;
namespace VSC.AST {
    public class MethodDeclaration : Declaration
    {
 			public MethodHeader _method_header;
			public MethodBodyExpressionBlock _method_body_expression_block;
            public VSC.TypeSystem.Modifiers _Modifiers = TypeSystem.Modifiers.NONE;
			[Rule("<method declaration> ::= <method header> <method body expression block>")]
			public MethodDeclaration(MethodHeader _MethodHeader,MethodBodyExpressionBlock _MethodBodyExpressionBlock)
				{
				_method_header = _MethodHeader;
				_method_body_expression_block = _MethodBodyExpressionBlock;
                _Modifiers = _MethodHeader._opt_modifiers._Modifiers;
              
              if(_MethodHeader._modifiers != null)
                foreach (var m in _MethodHeader._modifiers)
                    _Modifiers |= m._Modifier;

				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                UnresolvedMethodSpec m = null;
                if(_method_header._method_declaration_name._type_declaration_name != null)
                    m = new UnresolvedMethodSpec(rc.currentTypeDefinition, _method_header._method_declaration_name._type_declaration_name._identifier._Identifier);
                else
                    m = new UnresolvedMethodSpec(rc.currentTypeDefinition, _method_header._method_declaration_name._identifier._Identifier);
                rc.currentMethod = m; // required for resolving type parameters
                m.Region = rc.MakeRegion(this);
                m.BodyRegion = rc.MakeRegion(_method_body_expression_block);

                if (rc.InheritsConstraints(this) && _method_header._opt_type_parameter_constraints_clauses._type_parameter_constraints_clauses != null)
                {
                    int index = 0;
                    TypeParameters tpar = null;
                    
                    if (_method_header._method_declaration_name._type_declaration_name != null)
                        tpar = _method_header._method_declaration_name._type_declaration_name._opt_type_parameter_list._type_parameters;
                    else tpar = _method_header._method_declaration_name._opt_type_parameter_list._type_parameters;
        if(tpar != null)
            foreach (var tpDecl in tpar)
                    {
                        var tp = new MethodTypeParameterWithInheritedConstraints(index++, tpDecl.Name);
                        tp.Region = rc.MakeRegion(tpDecl);
                        tp.ApplyInterningProvider(rc.interningProvider);
                        m.TypeParameters.Add(tp);
                    }
                }
                else if(_method_header._method_declaration_name._explicit_interface == null)
                    rc.ConvertTypeParameters(m.TypeParameters, _method_header._method_declaration_name._type_declaration_name._opt_type_parameter_list._type_parameters, _method_header._opt_type_parameter_constraints_clauses._type_parameter_constraints_clauses, SymbolKind.Method);

                // return type
                m.ReturnType = rc.ConvertTypeReference(_method_header._member_type, NameLookupMode.Type);

                // attributes
                rc.ConvertAttributes(m.Attributes, _method_header._opt_attributes._Attributes );
                rc.ConvertAttributes(m.ReturnTypeAttributes, _method_header._opt_attributes._ReturnAttributes);

                // modifiers
                rc.ApplyModifiers(m,_Modifiers);

                // extension method
                if (_method_header._opt_formal_parameter_list._formal_parameter_list != null)
                {
                    var first_param = _method_header._opt_formal_parameter_list._formal_parameter_list._fixed_parameters.FirstOrDefault();
                    if (first_param != null && first_param._opt_parameter_modifier._parameter_modifier != null && first_param._opt_parameter_modifier._parameter_modifier._Modifier == TypeSystem.ParameterModifier.This)
                    {
                        m.IsExtensionMethod = true;
                        rc.currentTypeDefinition.HasExtensionMethods = true;
                    }
                }
               
                // body
                if (_method_body_expression_block._block_or_semicolon != null)
                    m.HasBody = _method_body_expression_block._block_or_semicolon._block_statement != null;
                else m.HasBody = true;

                // parameters
                if(_method_header._opt_formal_parameter_list._formal_parameter_list != null)
                            rc.ConvertParameters(m.Parameters, _method_header._opt_formal_parameter_list._formal_parameter_list._fixed_parameters, _method_header._opt_formal_parameter_list._formal_parameter_list._parameter_array);


                // explicit interface implementation
                if (_method_header._method_declaration_name._explicit_interface != null)
                {
                    m.Accessibility = Accessibility.None;
                    m.IsExplicitInterfaceImplementation = true;
                    m.ExplicitInterfaceImplementations.Add(
                        rc.interningProvider.Intern(new MemberReferenceSpec(
                            m.SymbolKind,
                            rc.ConvertTypeReference(_method_header._method_declaration_name._explicit_interface, NameLookupMode.Type),
                            m.Name, m.TypeParameters.Count, rc.GetParameterTypes(m.Parameters))));
                }

            
                // add to resolver
                rc.currentTypeDefinition.Members.Add(m);
                rc.currentMethod = null;
                m.ApplyInterningProvider(rc.interningProvider);

                return true;
            }
}
}
