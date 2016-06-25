using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RegisterOperation : Semantic {
 			public Register _register;
			public RegisterBinaryOperator _register_binary_operator;
			public RegisterUnaryOperator _register_unary_operator;
            public Register _rregister;
            public Register _lregister;
			[Rule("<Register Operation> ::= <Register> <Register Binary Operator> <Register>")]
			public RegisterOperation(Register _lRegister,RegisterBinaryOperator _RegisterBinaryOperator,Register _rRegister)
				{
				_lregister = _lRegister;
				_register_binary_operator = _RegisterBinaryOperator;
				_rregister = _rRegister;
				}
			[Rule("<Register Operation> ::= <Register Unary Operator> <Register>")]
			public RegisterOperation(RegisterUnaryOperator _RegisterUnaryOperator,Register _Register)
				{
				_register_unary_operator = _RegisterUnaryOperator;
				_register = _Register;
				}
}
}
