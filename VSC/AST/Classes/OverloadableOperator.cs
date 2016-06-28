using System;
using VSC.Base.GoldParser.Semantic;
namespace VSC.AST { 
	public class OverloadableOperator : Semantic {
        public UnaryOperatorLiteral _unary_operator_constant;
        public BinaryOperatorLiteral _binary_operator_constant;
        public Semantic _operator;
        [Rule("<overloadable operator> ::= '!'")]
        [Rule("<overloadable operator> ::= '~'")]
        [Rule("<overloadable operator> ::= '++'")]
        [Rule("<overloadable operator> ::= '--'")]
        [Rule("<overloadable operator> ::= true")]
        [Rule("<overloadable operator> ::= false")]
        [Rule("<overloadable operator> ::= '+'")]
        [Rule("<overloadable operator> ::= '-'")]
        [Rule("<overloadable operator> ::= '*'")]
        [Rule("<overloadable operator> ::= '/'")]
        [Rule("<overloadable operator> ::= '%'")]
        [Rule("<overloadable operator> ::= '&'")]
        [Rule("<overloadable operator> ::= '^'")]
        [Rule("<overloadable operator> ::= '|'")]
        [Rule("<overloadable operator> ::= '<<'")]
        [Rule("<overloadable operator> ::= '>>'")]
        [Rule("<overloadable operator> ::= '<~'")]
        [Rule("<overloadable operator> ::= '~>'")]
        [Rule("<overloadable operator> ::= '=='")]
        [Rule("<overloadable operator> ::= '!='")]
        [Rule("<overloadable operator> ::= '<'")]
        [Rule("<overloadable operator> ::= '>'")]
        [Rule("<overloadable operator> ::= '>='")]
        [Rule("<overloadable operator> ::= '<='")]
        [Rule("<overloadable operator> ::= is")]
        public OverloadableOperator(Semantic _symbol10)
        {
            _operator = _symbol10;
        }
        [Rule("<overloadable operator> ::= <Unary Operator Constant>")]
        public OverloadableOperator(UnaryOperatorLiteral _UnaryOperatorConstant)
        {
            _unary_operator_constant = _UnaryOperatorConstant;
        }
        [Rule("<overloadable operator> ::= <Binary Operator Constant>")]
        public OverloadableOperator(BinaryOperatorLiteral _BinaryOperatorConstant)
        {
            _binary_operator_constant = _BinaryOperatorConstant;
        }
}
}
