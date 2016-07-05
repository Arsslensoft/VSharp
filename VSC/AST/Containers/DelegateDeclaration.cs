using System;
using System.Collections.Generic;
using System.Linq;
using VSC.TypeSystem;

namespace VSC.AST
{
    public sealed class DelegateDeclaration : TypeDeclaration
    {
        const Modifiers MethodModifiers = Modifiers.PUBLIC | Modifiers.VIRTUAL;

        const Modifiers AllowedModifiers =
            Modifiers.NEW |
            Modifiers.PUBLIC |
            Modifiers.PROTECTED |
            Modifiers.INTERNAL |
            Modifiers.PRIVATE;


        FullNamedExpression ReturnType;
        readonly ParametersCompiled parameters;
        public DelegateDeclaration(TypeContainer parent, FullNamedExpression type, Modifiers mod_flags, MemberName name, ParametersCompiled param_list,
            VSharpAttributes attrs, CompilationSourceFile file)
            : base(parent, mod_flags | Modifiers.SEALED, AllowedModifiers, name, attrs, name.Location, TypeKind.Delegate,file)

        {
            BaseTypes.Add(KnownTypeReference.MulticastDelegate);
            this.ReturnType = type;
            parameters      = param_list;
            List<IUnresolvedParameter> uparameters = new List<IUnresolvedParameter>();
            foreach (Parameter p in param_list.parameters)
                uparameters.Add(p);


            AddDefaultMethodsToDelegate(type as ITypeReference, uparameters);


            if (attrs != null)
            {
                IUnresolvedMethod invokeMethod = (IUnresolvedMethod)Members.Single(m => m.Name == "Invoke");
                foreach (IUnresolvedAttribute attr in attrs.Attrs.Where(x => x.ExplicitTarget == "ret").ToList())
                    invokeMethod.ReturnTypeAttributes.Add(attr);
            }
            
        }
        static readonly IUnresolvedParameter delegateObjectParameter = MakeParameter(KnownTypeReference.Object, "object");
        static readonly IUnresolvedParameter delegateIntPtrMethodParameter = MakeParameter(KnownTypeReference.IntPtr, "method");
        static IUnresolvedParameter MakeParameter(ITypeReference type, string name)
        {
            Parameter p = new Parameter(type, name, Location.Null);
            p.Freeze();
            return p;
        }
        public void AddDefaultMethodsToDelegate(ITypeReference returnType, IEnumerable<IUnresolvedParameter> parameters)
        {
        
            if (returnType == null)
                throw new ArgumentNullException("returnType");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            DomRegion region = Region;
            region = new DomRegion(region.FileName, region.BeginLine, region.BeginColumn); // remove end position

            MethodOrOperator invoke = new MethodOrOperator(this,"Invoke");
            invoke.Accessibility = Accessibility.Public;
            invoke.IsSynthetic = true;
            foreach (var p in parameters)
                invoke.Parameters.Add(p);
            invoke.ReturnType = returnType;
            invoke.Region = region;
            this.Members.Add(invoke);

            MethodOrOperator ctor = new MethodOrOperator(this, ".ctor");
            ctor.SymbolKind = SymbolKind.Constructor;
            ctor.Accessibility = Accessibility.Public;
            ctor.IsSynthetic = true;
            ctor.Parameters.Add(delegateObjectParameter);
            ctor.Parameters.Add(delegateIntPtrMethodParameter);
            ctor.ReturnType = this;
            ctor.Region = region;
            this.Members.Add(ctor);
        }

    }
}