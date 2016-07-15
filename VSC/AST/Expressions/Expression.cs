using System;
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
    public abstract class Expression : IAstNode, IResolveExpression, IEmitExpression
     {
         public ResolveResult Result;
        protected ITypeReference type;
        protected Location loc;

        public ITypeReference Type
        {
            get { return type; }
            set { type = value; }
        }
        public IAstNode ParentNode { get; set; }

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

        public virtual Expression DoResolve(ResolveContext rc)
        {
            return this;
        }

        public virtual ResolveResult GetResolveResult(ResolveContext rc)
        {
            return Result;
        }
        public bool EmitToStack(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public bool EmitFromStack(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public bool Emit(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public  IConstantValue ConvertConstantValue(ITypeReference targetType)
        {
            return ConvertConstantValue(targetType, this, CompilerContext.InternProvider);
        }

        protected IConstantValue ConvertConstantValue(
             ITypeReference targetType, Expression expression,
             InterningProvider interningProvider)
        {

            Constant c = expression as Constant;
            if (c == null)
                return new ErrorConstantValue(targetType);

            // cast to the desired type
            return interningProvider.Intern(new ConstantCast(targetType, c, true));
        }
         public virtual IConstantValue BuilConstantValue(bool isAttributeConstant)
         {
             return this as IConstantValue;
             
         }
    }
}
