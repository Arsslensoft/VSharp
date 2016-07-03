﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.AST
{
    /// <summary>
    /// Default implementation for IUnresolvedParameter.
    /// </summary>
    [Serializable]
    public sealed class Parameter : IUnresolvedParameter, IFreezable, ISupportsInterning
    {

        public Parameter Clone()
        {
            Parameter p = (Parameter)MemberwiseClone();
            if (attributes != null)
                p.attributes = attributes;

            return p;
        }
        public void Error_DuplicateName(Report r)
        {
            r.Error(100, Location, "The parameter name `{0}' is a duplicate", Name);
        }
        public static string GetModifierSignature(ParameterModifier mod)
        {
            switch (mod)
            {
                case ParameterModifier.Out:
                    return "out";
                case ParameterModifier.Params:
                    return "params";
                case ParameterModifier.Ref:
                    return "ref";
                case ParameterModifier.This:
                    return "self";
                default:
                    return "";
            }
        }
        string name = string.Empty;
        ITypeReference type = SpecialTypeSpec.UnknownType;
        IList<IUnresolvedAttribute> attributes;
        IConstantValue defaultValue;
        DomRegion region;
        byte flags;

        public Parameter()
        {
        }
        public Parameter(FullNamedExpression type, string name, ParameterModifier mods, VSharpAttributes attr, Location loc)
            : this(type as ITypeReference, name,loc)
        {
            if ((mods & ParameterModifier.Out) == ParameterModifier.Out)
            {
                IsOut = true;
                CompilerContext.InternProvider.Intern(new ByReferenceTypeReference(this.type));
            }

            if ((mods & ParameterModifier.Ref) == ParameterModifier.Ref)
            {
                IsRef = true;
               CompilerContext.InternProvider.Intern(new ByReferenceTypeReference(this.type));
            }

             if ((mods & ParameterModifier.This) == ParameterModifier.This)
                IsSelf = true;
             if ((mods & ParameterModifier.Params) == ParameterModifier.Params)
                 IsParams = true;


             if (attr != null)
                 foreach (var a in attr.Attrs)
                     this.attributes.Add(a);
        }
        public Parameter(ITypeReference type, string name, Location l)
        {
            loc = l;
            if (type == null)
                throw new ArgumentNullException("type");
            if (name == null)
                throw new ArgumentNullException("name");
            this.type = type;
            this.name = name;
        }

        void FreezeInternal()
        {
            attributes = FreezableHelper.FreezeListAndElements(attributes);
            FreezableHelper.Freeze(defaultValue);
        }
        readonly Location loc;
        public Location Location
        {
            get
            {
                return loc;
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                FreezableHelper.ThrowIfFrozen(this);
                name = value;
            }
        }

        public ITypeReference Type
        {
            get { return type; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                FreezableHelper.ThrowIfFrozen(this);
                type = value;
            }
        }

        public IList<IUnresolvedAttribute> Attributes
        {
            get
            {
                if (attributes == null)
                    attributes = new List<IUnresolvedAttribute>();
                return attributes;
            }
        }

        public IConstantValue DefaultValue
        {
            get { return defaultValue; }
            set
            {
                FreezableHelper.ThrowIfFrozen(this);
                defaultValue = value;
            }
        }

        public DomRegion Region
        {
            get { return region; }
            set
            {
                FreezableHelper.ThrowIfFrozen(this);
                region = value;
            }
        }

        bool HasFlag(byte flag)
        {
            return (this.flags & flag) != 0;
        }
        void SetFlag(byte flag, bool value)
        {
            FreezableHelper.ThrowIfFrozen(this);
            if (value)
                this.flags |= flag;
            else
                this.flags &= unchecked((byte)~flag);
        }
     
        public bool IsFrozen
        {
            get { return HasFlag(1); }
        }

        public void Freeze()
        {
            if (!this.IsFrozen)
            {
                FreezeInternal();
                this.flags |= 1;
            }
        }

        public bool IsRef
        {
            get { return HasFlag(2); }
            set { SetFlag(2, value); }
        }
   
        public bool IsOut
        {
            get { return HasFlag(4); }
            set { SetFlag(4, value); }
        }

        public bool IsParams
        {
            get { return HasFlag(8); }
            set { SetFlag(8, value); }
        }
        public bool IsSelf
        {
            get { return HasFlag(16); }
            set { SetFlag(16, value); }
        }
        public bool IsOptional
        {
            get { return this.DefaultValue != null; }
        }

        int ISupportsInterning.GetHashCodeForInterning()
        {
            unchecked
            {
                int hashCode = 1919191 ^ (flags & ~1);
                hashCode *= 31;
                hashCode += type.GetHashCode();
                hashCode *= 31;
                hashCode += name.GetHashCode();
                if (attributes != null)
                {
                    foreach (var attr in attributes)
                        hashCode ^= attr.GetHashCode();
                }
                if (defaultValue != null)
                    hashCode ^= defaultValue.GetHashCode();
                return hashCode;
            }
        }

        bool ISupportsInterning.EqualsForInterning(ISupportsInterning other)
        {
            // compare everything except for the IsFrozen flag
            Parameter p = other as Parameter;
            return p != null && type == p.type && name == p.name &&
                defaultValue == p.defaultValue && region == p.region && (flags & ~1) == (p.flags & ~1) && ListEquals(attributes, p.attributes);
        }

        static bool ListEquals(IList<IUnresolvedAttribute> list1, IList<IUnresolvedAttribute> list2)
        {
            return (list1 ?? EmptyList<IUnresolvedAttribute>.Instance).SequenceEqual(list2 ?? EmptyList<IUnresolvedAttribute>.Instance);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            if (IsRef)
                b.Append("ref ");
            if (IsOut)
                b.Append("out ");
            if (IsParams)
                b.Append("params ");
            b.Append(name);
            b.Append(':');
            b.Append(type.ToString());
            if (defaultValue != null)
            {
                b.Append(" = ");
                b.Append(defaultValue.ToString());
            }
            return b.ToString();
        }

        static bool IsOptionalAttribute(IType attributeType)
        {
            return attributeType.Name == "OptionalAttribute" && attributeType.Namespace == "System.Runtime.InteropServices";
        }

        public IParameter CreateResolvedParameter(ITypeResolveContext context)
        {
            Freeze();
            if (defaultValue != null)
            {
                return new ResolvedParameterWithDefaultValue(defaultValue, context)
                {
                    Type = type.Resolve(context),
                    Name = name,
                    Region = region,
                    Attributes = attributes.CreateResolvedAttributes(context),
                    IsRef = this.IsRef,
                    IsOut = this.IsOut,
                    IsParams = this.IsParams
                };
            }
            else
            {
                var owner = context.CurrentMember as IParameterizedMember;
                var resolvedAttributes = attributes.CreateResolvedAttributes(context);
                bool isOptional = resolvedAttributes != null && resolvedAttributes.Any(a => IsOptionalAttribute(a.AttributeType));
                return new ParameterSpec(type.Resolve(context), name, owner, region,
                                             resolvedAttributes, IsRef, IsOut, IsParams, isOptional);
            }
        }

        sealed class ResolvedParameterWithDefaultValue : IParameter
        {
            readonly IConstantValue defaultValue;
            readonly ITypeResolveContext context;

            public ResolvedParameterWithDefaultValue(IConstantValue defaultValue, ITypeResolveContext context)
            {
                this.defaultValue = defaultValue;
                this.context = context;
            }

            SymbolKind ISymbol.SymbolKind { get { return SymbolKind.Parameter; } }
            public IParameterizedMember Owner { get { return context.CurrentMember as IParameterizedMember; } }
            public IType Type { get; internal set; }
            public string Name { get; internal set; }
            public DomRegion Region { get; internal set; }
            public IList<IAttribute> Attributes { get; internal set; }
            public bool IsRef { get; internal set; }
            public bool IsOut { get; internal set; }
            public bool IsParams { get; internal set; }
            public bool IsOptional { get { return true; } }
            bool IVariable.IsConst { get { return false; } }

            ResolveResult resolvedDefaultValue;

            public object ConstantValue
            {
                get
                {
                    ResolveResult rr = LazyInit.VolatileRead(ref this.resolvedDefaultValue);
                    if (rr == null)
                    {
                        rr = defaultValue.Resolve(context);
                        LazyInit.GetOrSet(ref this.resolvedDefaultValue, rr);
                    }
                    return rr.ConstantValue;
                }
            }

            public override string ToString()
            {
                return ParameterSpec.ToString(this);
            }

            public ISymbolReference ToReference()
            {
                if (Owner == null)
                    return new ParameterReference(Type.ToTypeReference(), Name, Region, IsRef, IsOut, IsParams, true, ConstantValue);
                return new OwnedParameterReference(Owner.ToReference(), Owner.Parameters.IndexOf(this));
            }
        }
    }

    public abstract class AParametersCollection
    {
        protected bool has_arglist;
        protected bool has_params;

        // Null object pattern
        public Parameter[] parameters;

        public CallingConventions CallingConvention
        {
            get
            {
                return has_arglist ?
                    CallingConventions.VarArgs :
                    CallingConventions.Standard;
            }
        }

        public int Count
        {
            get { return parameters.Length; }
        }

        public ITypeReference ExtensionMethodType
        {
            get
            {
                if (Count == 0)
                    return null;

                return FixedParameters[0].IsSelf?
                    FixedParameters[0].Type : null;
            }
        }
        public bool IsEmpty
        {
            get { return parameters.Length == 0; }
        }
        public Parameter[] FixedParameters
        {
            get
            {
                return parameters;
            }
        }
    }
    public class ParametersCompiled : AParametersCollection
    {
        public static readonly ParametersCompiled EmptyReadOnlyParameters = new ParametersCompiled();
        public static readonly ParametersCompiled Undefined = new ParametersCompiled();
        public ParametersCompiled Clone()
        {
            ParametersCompiled p = (ParametersCompiled)MemberwiseClone();

            p.parameters = new Parameter[parameters.Length];
            for (int i = 0; i < Count; ++i)
                p.parameters[i] = this[i].Clone();

            return p;
        }
        private ParametersCompiled()
        {
            parameters = new Parameter[0];
        }
        public void CheckParameters()
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                var name = parameters[i].Name;
                for (int ii = i + 1; ii < parameters.Length; ++ii)
                {
                    if (parameters[ii].Name == name)
                        this[ii].Error_DuplicateName(CompilerContext.report);
                }
            }
        }
        public ParametersCompiled(params Parameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("Use EmptyReadOnlyParameters");

            this.parameters = parameters;
            int count = parameters.Length;

            for (int i = 0; i < count; i++)
                has_params |= parameters[i].IsSelf;
            
        }

        public ParametersCompiled(Parameter[] parameters, bool has_arglist) :
            this(parameters)
        {
            this.has_arglist = has_arglist;
        }

        public Parameter this[int pos]
        {
            get { return (Parameter)parameters[pos]; }
        }


        public static ParametersCompiled CreateImplicitParameter(FullNamedExpression texpr, Location loc)
        {
            return new ParametersCompiled(
                new[] { new Parameter(texpr, "value", ParameterModifier.None, null, loc) });
        }
        public static ParametersCompiled MergeGenerated(CompilerContext ctx, ParametersCompiled userParams, bool checkConflicts, Parameter compilerParams)
        {
            return MergeGenerated(ctx, userParams, checkConflicts,
                new Parameter[] { compilerParams });
        }

        //
        // Use this method when you merge compiler generated parameters with user parameters
        //
        public static ParametersCompiled MergeGenerated(CompilerContext ctx, ParametersCompiled userParams, bool checkConflicts, Parameter[] compilerParams)
        {
            Parameter[] all_params = new Parameter[userParams.Count + compilerParams.Length];
            userParams.FixedParameters.CopyTo(all_params, 0);

        
            int last_filled = userParams.Count;
            int index = 0;
            foreach (Parameter p in compilerParams)
            {
                for (int i = 0; i < last_filled; ++i)
                {
                    while (p.Name == all_params[i].Name)
                    {
                        if (checkConflicts && i < userParams.Count)
                        {
                            ctx.Report.Error(316, userParams[i].Location,
                                "The parameter name `{0}' conflicts with a compiler generated name", p.Name);
                        }
                        p.Name = '_' + p.Name;
                    }
                }
                all_params[last_filled] = p;
              
                ++last_filled;
            }

            ParametersCompiled parameters = new ParametersCompiled(all_params);
            parameters.has_params = userParams.has_params;
            return parameters;
        }

    }
}
