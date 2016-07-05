﻿using System;
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
                    CompilerContext.report.Error(1, arg.loc, "Attribute named argument expression must be a constant");

            return il;
        }
        public IList<KeyValuePair<string, IConstantValue>> ToNamedCtorArgs()
        {
            List<KeyValuePair<string, IConstantValue>> il = new List<KeyValuePair<string, IConstantValue>>();
            foreach (NamedArgument arg in args)
                if (arg.CtorArgument &&arg.Expr is IConstantValue)
                    il.Add(new KeyValuePair<string, IConstantValue>(arg.Name, arg.Expr as IConstantValue));
                else if(arg.CtorArgument)
                    CompilerContext.report.Error(1, arg.loc, "Attribute named argument expression must be a constant");

            return il;
        }
        public IList<IConstantValue> ToPositionalArgs()
        {
            List<IConstantValue> il = new List<IConstantValue>();
            foreach (var arg in args)
                if (arg.Expr is IConstantValue)
                    il.Add(arg.Expr as IConstantValue);
                else
                    CompilerContext.report.Error(1, arg.loc, "Attribute argument expression must be a constant");

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
