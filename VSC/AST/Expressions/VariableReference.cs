using System;
using System.Globalization;
using VSC.Base;
using VSC.TypeSystem;

namespace VSC.AST
{
    /// <summary>
    /// Represents a local variable or parameter.
    /// </summary>
    public class LocalVariableExpression : VariableReference
    {
        readonly IVariable variable;

        public LocalVariableExpression(IVariable variable, Location l)

        {
            loc = l;
            this._resolved = true;
            this.ResolvedType = UnpackTypeIfByRefParameter(variable);
            this.variable = variable;
        }

        static IType UnpackTypeIfByRefParameter(IVariable variable)
        {
            if (variable == null)
                throw new ArgumentNullException("variable");
            IType type = variable.Type;
            if (type.Kind == TypeKind.ByReference)
            {
                IParameter p = variable as IParameter;
                if (p != null && (p.IsRef || p.IsOut))
                    return ((ByReferenceType)type).ElementType;
            }
            return type;
        }

        public IVariable Variable
        {
            get { return variable; }
        }

        public bool IsParameter
        {
            get { return variable is IParameter; }
        }

        public override bool IsCompileTimeConstant
        {
            get { return variable.IsConst; }
        }

        public override object ConstantValue
        {
            get { return IsParameter ? null : variable.ConstantValue; }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[LocalVariable {0}]", variable);
        }



        public override string Name
        {
            get { return variable.Name; }
        }
    }
    public abstract class VariableReference : Expression
    {
        public abstract string Name { get; }
    }
}