using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RegisterExpression : Semantic {
 			public Register _register;
			public RegisterTargetExpression _register_target_expression;
			public Expression _expression;
			public RegisterOperation _register_operation;
            public RegisterTargetExpression _fregister_target_expression;
            public RegisterTargetExpression _tregister_target_expression;
			[Rule("<Register Expression> ::= <Register> ':=' <Register Target Expression>")]
			public RegisterExpression(Register _Register, Semantic _symbol30,RegisterTargetExpression _RegisterTargetExpression)
				{
				_register = _Register;
				_register_target_expression = _RegisterTargetExpression;
				}
			[Rule("<Register Expression> ::= <expression> ':=' <Register>")]
			public RegisterExpression(Expression _Expression, Semantic _symbol30,Register _Register)
				{
				_expression = _Expression;
				_register = _Register;
				}
			[Rule("<Register Expression> ::= '+' <Register>")]
			[Rule("<Register Expression> ::= '-' <Register>")]
			public RegisterExpression( Semantic _symbol51,Register _Register)
				{
				_register = _Register;
				}
			[Rule("<Register Expression> ::= <Register> ':=' <Register Operation> '?' <Register Target Expression> ':' <Register Target Expression>")]
			public RegisterExpression(Register _Register, Semantic _symbol30,RegisterOperation _RegisterOperation, Semantic _symbol32,RegisterTargetExpression _tRegisterTargetExpression, Semantic _symbol28,RegisterTargetExpression _fRegisterTargetExpression)
				{
				_register = _Register;
				_register_operation = _RegisterOperation;
				_tregister_target_expression = _tRegisterTargetExpression;
				_fregister_target_expression = _fRegisterTargetExpression;
				}
			[Rule("<Register Expression> ::= <Register Operation>")]
			public RegisterExpression(RegisterOperation _RegisterOperation)
				{
				_register_operation = _RegisterOperation;
				}
}
}
