using System;
using VSC.Base.GoldParser.Parser;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
namespace VSC.AST {
    public class ConstantExpression : Expression
    {


        public MultilineStringLiteral _multiline_string_constant;
        public CharacterLiteral _character_constant;
        public IntegralLiteral _integral_constant;
        public BooleanLiteral _boolean_constant;
        public NullLiteral _null_constant;
        public FloatLiteral _float_constant;

        [Rule("<Constant Expression> ::= <Multiline String Constant>")]
        public ConstantExpression(MultilineStringLiteral _MultilineStringConstant)
        {
            _multiline_string_constant = _MultilineStringConstant;
        }
        [Rule("<Constant Expression> ::= <Character Constant>")]
        public ConstantExpression(CharacterLiteral _CharacterConstant)
        {
            _character_constant = _CharacterConstant;
        }
        [Rule("<Constant Expression> ::= <Integral Constant>")]
        public ConstantExpression(IntegralLiteral _IntegralConstant)
        {
            _integral_constant = _IntegralConstant;
        }
        [Rule("<Constant Expression> ::= <Boolean Constant>")]
        public ConstantExpression(BooleanLiteral _BooleanConstant)
        {
            _boolean_constant = _BooleanConstant;
        }
        [Rule("<Constant Expression> ::= <Null Constant>")]
        public ConstantExpression(NullLiteral _NullConstant)
        {
            _null_constant = _NullConstant;
        }
        [Rule("<Constant Expression> ::= <Float Constant>")]
        public ConstantExpression(FloatLiteral _FloatConstant)
        {
            _float_constant = _FloatConstant;
        }
        public ConstantExpression(ITypeReference tref,object val, LineInfo pos)
        {
            type = tref;
            value = val;
            position = pos;
        }

        override public string ToString()
        {
            return this.GetType().Name + " (" + Value.ToString() + ")";
        }
        public virtual object GetValue()
        {
            return value;
        }
        readonly ITypeReference type;
        readonly object value;

        public ITypeReference Type
        {
            get { return type; }
        }

        public object Value
        {
            get { return value; }
        }
    }
}
