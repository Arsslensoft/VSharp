﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
   
     /// <remarks>
    ///   Base class for expressions
    /// </remarks>
    public abstract class Expression : IAstNode, IResolve
    {
  
        protected ITypeReference type;
        protected Location loc;

        public ITypeReference Type
        {
            get { return type; }
            set { type = value; }
        }


        public Location Location
        {
            get { return loc; }
        }

        public virtual bool IsNull
        {
            get
            {
                return false;
            }
        }

        public virtual string GetSignatureForError()
        {
            return type.ToString();
        }
        public virtual void AcceptVisitor(IVisitor vis)
        {
            vis.Visit(this);
        }

        public virtual bool Resolve(ResolveContext rc)
        {
            return true;
        }
        public virtual object DoResolve(ResolveContext rc)
        {
            return this;
        }
       
    }
    /// <summary>
	///   This is just a base class for expressions that can
	///   appear on statements (invocations, object creation,
	///   assignments, post/pre increment and decrement).  The idea
	///   being that they would support an extra Emition interface that
	///   does not leave a result on the stack.
	/// </summary>
    public abstract class ExpressionStatement : Expression
    {

    }
 
}
