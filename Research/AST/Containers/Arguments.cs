using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Context;
using VSC.TypeSystem;

namespace VSC.AST
{
    	//
	// Argument expression used for invocation
	//
    public class Argument
    {
        public enum AType : byte
        {
            None = 0,
            Ref = 1,			// ref modifier used
            Out = 2,			// out modifier used
            Default = 3,		// argument created from default parameter value
            DynamicTypeName = 4,	// System.Type argument for dynamic binding
            ExtensionType = 5,	// Instance expression inserted as the first argument

            // Conditional instance expression inserted as the first argument
            ExtensionTypeConditionalAccess = 5 | ConditionalAccessFlag,

            ConditionalAccessFlag = 1 << 7
        }

        public readonly AType ArgType;
        public Expression Expr;
        public readonly Location loc;
        public Argument(Expression expr, AType type, Location l)
        {
            this.Expr = expr;
            this.ArgType = type;
            this.loc = l;
        }

        public Argument(Expression expr, Location l)
        {
            this.Expr = expr; this.loc = l;
        }

        #region Properties

        public bool IsByRef
        {
            get { return ArgType == AType.Ref || ArgType == AType.Out; }
        }

        public bool IsDefaultArgument
        {
            get { return ArgType == AType.Default; }
        }

        public bool IsExtensionType
        {
            get
            {
                return (ArgType & AType.ExtensionType) == AType.ExtensionType;
            }
        }

        public ParameterModifier Modifier
        {
            get
            {
                switch (ArgType)
                {
                    case AType.Out:
                        return ParameterModifier.Out;

                    case AType.Ref:
                        return ParameterModifier.Ref;

                    default:
                        return ParameterModifier.None;
                }
            }
        }


        #endregion
    }
    public class MovableArgument : Argument
    {
        public MovableArgument(Argument arg, Location l)
            : this(arg.Expr, arg.ArgType,arg.loc)
        {
        }

        protected MovableArgument(Expression expr, AType modifier, Location l)
            : base(expr, modifier, l)
        {
        }

    }
    public class NamedArgument : MovableArgument
    {
        public readonly string Name;
        public readonly bool CtorArgument = true;

        public NamedArgument(string name, Location loc, Expression expr)
            : this(name, loc, expr, AType.None)
        {
            CtorArgument = false;
        }

        public NamedArgument(string name, Location loc, Expression expr, AType modifier)
            : base(expr, modifier, loc)
        {
            this.Name = name;
  
        }

        public Location Location
        {
            get { return loc; }
        }
    }
    public class Arguments
    {
        sealed class ArgumentsOrdered : Arguments
        {
            readonly List<MovableArgument> ordered;

            public ArgumentsOrdered(Arguments args)
                : base(args.Count)
            {
                AddRange(args);
                ordered = new List<MovableArgument>();
            }

            public void AddOrdered(MovableArgument arg)
            {
                ordered.Add(arg);
            }
        }

        // Try not to add any more instances to this class, it's allocated a lot
      internal  List<Argument> args;

        public Arguments(int capacity)
        {
            args = new List<Argument>(capacity);
        }

        private Arguments(List<Argument> args)
        {
            this.args = args;
        }

        public void Add(Argument arg)
        {
            args.Add(arg);
        }

        public void AddRange(Arguments args)
        {
            this.args.AddRange(args.args);
        }
        public IList<KeyValuePair<string, IConstantValue>> ToNamedArgs()
        {
            List<KeyValuePair<string, IConstantValue>> il = new List<KeyValuePair<string, IConstantValue>>();
            foreach (NamedArgument arg in args)
                if (!arg.CtorArgument && arg.Expr is IConstantValue)
                    il.Add(new KeyValuePair<string, IConstantValue>(arg.Name, arg.Expr as IConstantValue));
                else if(!arg.CtorArgument)
                    CompilerContext.report.Error(0, arg.loc, "Attribute named argument expression must be a constant");

            return il;
        }
        public IList<KeyValuePair<string, IConstantValue>> ToNamedCtorArgs()
        {
            List<KeyValuePair<string, IConstantValue>> il = new List<KeyValuePair<string, IConstantValue>>();
            foreach (NamedArgument arg in args)
                if (arg.CtorArgument &&arg.Expr is IConstantValue)
                    il.Add(new KeyValuePair<string, IConstantValue>(arg.Name, arg.Expr as IConstantValue));
                else if(arg.CtorArgument)
                    CompilerContext.report.Error(0, arg.loc, "Attribute named argument expression must be a constant");

            return il;
        }
        public IList<IConstantValue> ToPositionalArgs()
        {
            List<IConstantValue> il = new List<IConstantValue>();
            foreach (var arg in args)
                if (arg.Expr is IConstantValue)
                    il.Add(arg.Expr as IConstantValue);
                else
                    CompilerContext.report.Error(0, arg.loc, "Attribute argument expression must be a constant");

            return il;
        }
        //
        // At least one argument is named argument
        //
        public bool HasNamed
        {
            get
            {
                foreach (Argument a in args)
                {
                    if (a is NamedArgument)
                        return true;
                }

                return false;
            }
        }
        public void RemoveAt(int index)
        {
            args.RemoveAt(index);
        }
        public int Count
        {
            get { return args.Count; }
        }
        public Argument this[int index]
        {
            get { return args[index]; }
            set { args[index] = value; }
        }
    }
}
