using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class RegisterTargetExpression : Semantic {
 			public Expression _expression;
			public Register _register;

			[Rule("<Register Target Expression> ::= <expression>")]
			public RegisterTargetExpression(Expression _Expression)
				{
				_expression = _Expression;
				}
			[Rule("<Register Target Expression> ::= <Register>")]
			public RegisterTargetExpression(Register _Register)
				{
				_register = _Register;
				}
}
}
