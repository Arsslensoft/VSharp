using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OperatorDeclarator : Semantic {
        public Type _type;
			public OverloadableOperator _overloadable_operator;
			public OptFormalParameterList _opt_formal_parameter_list;
            public string _operator_name;
            public string GetOperatorType()
            {
                if (_overloadable_operator._binary_operator_constant != null)
                    return "";// TODO
                else if (_overloadable_operator._unary_operator_constant != null)
                    return ""; // TODO
                else
                    return VSC.TypeSystem.Resolver.VSharpResolver.GetMetadataName(_overloadable_operator._operator.Name, ref _OperatorType);
            }
            public VSC.TypeSystem.Resolver.OperatorType _OperatorType;
			[Rule("<operator declarator> ::= <type> operator <overloadable operator> '(' <opt formal parameter list> ')'")]
            public OperatorDeclarator(Type _Type, Semantic _symbol119, OverloadableOperator _OverloadableOperator, Semantic _symbol20, OptFormalParameterList _OptFormalParameterList, Semantic _symbol21)
            {
                _opt_formal_parameter_list = _OptFormalParameterList;
                    _type = _Type;
				_overloadable_operator = _OverloadableOperator;
                _operator_name = GetOperatorType();
				}
            [Rule("<operator declarator> ::= implicit operator <type> '(' <opt formal parameter list> ')'")]
            [Rule("<operator declarator> ::= explicit operator <type> '(' <opt formal parameter list> ')'")]
            public OperatorDeclarator(Semantic _symbol106, Semantic _symbol119, Type _Type, Semantic _symbol20, OptFormalParameterList _OptFormalParameterList, Semantic _symbol21)
				{
                    if ( _symbol106.Name == "implicit")
                        _OperatorType = TypeSystem.Resolver.OperatorType.Implicit;
                    else _OperatorType = TypeSystem.Resolver.OperatorType.Explicit;
                    _operator_name = VSC.TypeSystem.Resolver.VSharpResolver.GetMetadataName(_OperatorType);
                    _type = _Type;

                    _opt_formal_parameter_list = _OptFormalParameterList;
				}
}
}
