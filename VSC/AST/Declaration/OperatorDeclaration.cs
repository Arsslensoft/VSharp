using System;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {
    public class OperatorDeclaration : Declaration
    {
 			public OptAttributes _opt_attributes;
			public OptModifiers _opt_modifiers;
			public OperatorDeclarator _operator_declarator;
			public MethodBodyExpressionBlock _method_body_expression_block;

			[Rule("<operator declaration> ::= <opt attributes> <opt modifiers> <operator declarator> <method body expression block>")]
			public OperatorDeclaration(OptAttributes _OptAttributes,OptModifiers _OptModifiers,OperatorDeclarator _OperatorDeclarator,MethodBodyExpressionBlock _MethodBodyExpressionBlock)
				{
				_opt_attributes = _OptAttributes;
				_opt_modifiers = _OptModifiers;
				_operator_declarator = _OperatorDeclarator;
				_method_body_expression_block = _MethodBodyExpressionBlock;
				}

            public override bool Resolve(Context.SymbolResolveContext rc)
            {
                string opname = _operator_declarator._operator_name;

                UnresolvedMethodSpec m = new UnresolvedMethodSpec(rc.currentTypeDefinition, opname);
                m.SymbolKind = SymbolKind.Operator;
                m.Region = rc.MakeRegion(this);
                m.BodyRegion = rc.MakeRegion(this._method_body_expression_block);

                // return type
                m.ReturnType = rc.ConvertTypeReference(_operator_declarator._type, NameLookupMode.Type);
            // attributes
                rc.ConvertAttributes(m.Attributes, _opt_attributes._Attributes);
                rc.ConvertAttributes(m.ReturnTypeAttributes, _opt_attributes._ReturnAttributes);

                // modifiers
                rc.ApplyModifiers(m, _opt_modifiers._Modifiers);
                // body
                if (_method_body_expression_block._block_or_semicolon != null)
                    m.HasBody = _method_body_expression_block._block_or_semicolon._block_statement != null;
                else m.HasBody = true;

                //parameters

          if(_operator_declarator._opt_formal_parameter_list._formal_parameter_list != null)
                rc.ConvertParameters(m.Parameters, _operator_declarator._opt_formal_parameter_list._formal_parameter_list._fixed_parameters, _operator_declarator._opt_formal_parameter_list._formal_parameter_list._parameter_array);

                // add to resolver
                rc.currentTypeDefinition.Members.Add(m);
                m.ApplyInterningProvider(rc.interningProvider);
      
                return true;
            }
}
}
