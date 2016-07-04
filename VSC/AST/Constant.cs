using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{
    public abstract class Constant : Expression, IConstantValue
    {

        public virtual ResolveResult Resolve(VSharpResolver resolver)
        {
            return null;
        }


        public ResolveResult Resolve(ITypeResolveContext context)
        {
            var csContext = (VSharpTypeResolveContext)context;
            if (context.CurrentAssembly != context.Compilation.MainAssembly)
            {
                // The constant needs to be resolved in a different compilation.
                IProjectContent pc = context.CurrentAssembly as IProjectContent;
                if (pc != null)
                {
                    ICompilation nestedCompilation = context.Compilation.SolutionSnapshot.GetCompilation(pc);
                    if (nestedCompilation != null)
                    {
                        var nestedContext = MapToNestedCompilation(csContext, nestedCompilation);
                        ResolveResult rr = Resolve(new VSharpResolver(nestedContext));
                        return MapToNewContext(rr, context);
                    }
                }
            }
            // Resolve in current context.
            return Resolve(new VSharpResolver(csContext));
        }

        VSharpTypeResolveContext MapToNestedCompilation(VSharpTypeResolveContext context, ICompilation nestedCompilation)
        {
            var nestedContext = new VSharpTypeResolveContext(nestedCompilation.MainAssembly);
            if (context.CurrentUsingScope != null)
            {
                nestedContext = nestedContext.WithUsingScope(context.CurrentUsingScope.UnresolvedUsingScope.Resolve(nestedCompilation));
            }
            if (context.CurrentTypeDefinition != null)
            {
                nestedContext = nestedContext.WithCurrentTypeDefinition(nestedCompilation.Import(context.CurrentTypeDefinition));
            }
            return nestedContext;
        }

        static ResolveResult MapToNewContext(ResolveResult rr, ITypeResolveContext newContext)
        {
            if (rr is TypeOfResolveResult)
            {
                return new TypeOfResolveResult(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    ((TypeOfResolveResult)rr).ReferencedType.ToTypeReference().Resolve(newContext));
            }
            else if (rr is ArrayCreateResolveResult)
            {
                ArrayCreateResolveResult acrr = (ArrayCreateResolveResult)rr;
                return new ArrayCreateResolveResult(
                    acrr.Type.ToTypeReference().Resolve(newContext),
                    MapToNewContext(acrr.SizeArguments, newContext),
                    MapToNewContext(acrr.InitializerElements, newContext));
            }
            else if (rr.IsCompileTimeConstant)
            {
                return new ConstantResolveResult(
                    rr.Type.ToTypeReference().Resolve(newContext),
                    rr.ConstantValue
                );
            }
            else
            {
                return new ErrorResolveResult(rr.Type.ToTypeReference().Resolve(newContext));
            }
        }

        static ResolveResult[] MapToNewContext(IList<ResolveResult> input, ITypeResolveContext newContext)
        {
            if (input == null)
                return null;
            ResolveResult[] output = new ResolveResult[input.Count];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = MapToNewContext(input[i], newContext);
            }
            return output;
        }


        static readonly NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

        protected Constant(Location loc)
        {
            this.loc = loc;
        }

        override public string ToString()
        {
            return this.GetType().Name + " (" + GetValueAsLiteral() + ")";
        }

        /// <summary>
        ///  This is used to obtain the actual value of the literal
        ///  cast into an object.
        /// </summary>
        public abstract object GetValue();

        public abstract long GetValueAsLong();

        public abstract string GetValueAsLiteral();


        public abstract bool IsDefaultValue
        {
            get;
        }

        public abstract bool IsNegative
        {
            get;
        }

        //
        // When constant is declared as literal
        //
        public virtual bool IsLiteral
        {
            get { return false; }
        }

        public virtual bool IsOneInteger
        {
            get { return false; }
        }

     
        public virtual bool IsZeroInteger
        {
            get { return false; }
        }

    }
    public abstract class IntegralConstant : Constant
    {
        protected IntegralConstant(ITypeReference type, Location loc)
            : base(loc)
        {
            this.type = type;
       
        }

        public override string GetValueAsLiteral()
        {
            return GetValue().ToString();
        }

        public abstract Constant Increment();
    }
    public class BoolConstant : Constant {
		public readonly bool Value;

		
		public BoolConstant ( bool val, Location loc)
			: base (loc)
		{
            this.type = KnownTypeReference.Boolean;

			Value = val;
		}

		public override object GetValue ()
		{
			return (object) Value;
		}

		public override string GetValueAsLiteral ()
		{
			return Value ? "true" : "false";
		}
		public override long GetValueAsLong ()
		{
			return Value ? 1 : 0;
		}
		public override bool IsDefaultValue {
			get {
				return !Value;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}
	
		public override bool IsZeroInteger {
			get { return Value == false; }
		}

	}
	public class ByteConstant : IntegralConstant
	{
		public readonly byte Value;


		public ByteConstant (byte v, Location loc)
			: base (KnownTypeReference.Byte, loc)
		{
			Value = v;
		}

	
		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
			return new ByteConstant (checked ((byte)(Value + 1)), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

		public override bool IsNegative {
			get {
				return false;
			}
		}

		public override bool IsZeroInteger {
			get { return Value == 0; }
		}



	}
	public class CharConstant : Constant {
		public readonly char Value;

		public CharConstant (char v, Location loc)
			: base (loc)
		{
			this.type = KnownTypeReference.Char;

			Value = v;
		}

		
		static string descape (char c)
		{
			switch (c){
			case '\a':
				return "\\a"; 
			case '\b':
				return "\\b"; 
			case '\n':
				return "\\n"; 
			case '\t':
				return "\\t"; 
			case '\v':
				return "\\v"; 
			case '\r':
				return "\\r"; 
			case '\\':
				return "\\\\";
			case '\f':
				return "\\f"; 
			case '\0':
				return "\\0"; 
			case '"':
				return "\\\""; 
			case '\'':
				return "\\\'"; 
			}
			return c.ToString ();
		}

		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override string GetValueAsLiteral ()
		{
			return "\"" + descape (Value) + "\"";
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}

		public override bool IsZeroInteger {
			get { return Value == '\0'; }
		}



	}
	public class SByteConstant : IntegralConstant
	{
		public readonly sbyte Value;


		public SByteConstant ( sbyte v, Location loc)
			: base (KnownTypeReference.SByte, loc)
		{
			Value = v;
		}

	

		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
		    return new SByteConstant (checked((sbyte)(Value + 1)), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		
		
		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

	

	}
	public class ShortConstant : IntegralConstant {
		public readonly short Value;

	

		public ShortConstant (short v, Location loc)
			: base (KnownTypeReference.Int16, loc)
		{
			Value = v;
		}

	

		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
			return new ShortConstant (checked((short)(Value + 1)), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}
		
		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

	
	}
	public class UShortConstant : IntegralConstant
	{
		public readonly ushort Value;

		public UShortConstant ( ushort v, Location loc)
			: base (KnownTypeReference.UInt16, loc)
		{
			Value = v;
		}

	
		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}
	
		public override Constant Increment ()
		{
			return new UShortConstant ( checked((ushort)(Value + 1)), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		
	
		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

	
	}
	public class IntConstant : IntegralConstant
	{
		public readonly int Value;

		public IntConstant (int v, Location loc)
			: base (KnownTypeReference.Int32, loc)
		{
			Value = v;
		}


		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
			return new IntConstant (checked(Value + 1), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}
		
		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

	
	
	}
	public class UIntConstant : IntegralConstant {
		public readonly uint Value;

		public UIntConstant ( uint v, Location loc)
			: base (KnownTypeReference.UInt32, loc)
		{
			Value = v;
		}

	
		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
			return new UIntConstant (checked(Value + 1), loc);
		}
	
		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

	

	}
	public class LongConstant : IntegralConstant {
		public readonly long Value;

		
		public LongConstant (long v, Location loc)
			: base (KnownTypeReference.Int64, loc)
		{
			Value = v;
		}

	
		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return Value;
		}

		public override Constant Increment ()
		{
			return new LongConstant (checked(Value + 1), loc);
		}
		
		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

		public override bool IsZeroInteger {
			get { return Value == 0; }
		}
	}
	public class ULongConstant : IntegralConstant {
		public readonly ulong Value;


		public ULongConstant (ulong v, Location loc)
			: base (KnownTypeReference.UInt64, loc)
		{
			Value = v;
		}

	
		public override object GetValue ()
		{
			return Value;
		}

		public override long GetValueAsLong ()
		{
			return (long) Value;
		}

		public override Constant Increment ()
		{
			return new ULongConstant (checked(Value + 1), loc);
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}
		
		public override bool IsOneInteger {
			get {
				return Value == 1;
			}
		}		

		public override bool IsZeroInteger {
			get { return Value == 0; }
		}

	}
	public class FloatConstant : Constant {
		//
		// Store constant value as double because float constant operations
		// need to work on double value to match JIT
		//
		public readonly double DoubleValue;

	
		public FloatConstant (double v, Location loc)
			: base (loc)
		{
			this.type = KnownTypeReference.Single;


			DoubleValue = v;
		}

	
		public float Value {
			get {
				return (float) DoubleValue;
			}
		}

		public override object GetValue ()
		{
			return Value;
		}

		public override string GetValueAsLiteral ()
		{
			return Value.ToString ();
		}

		public override long GetValueAsLong ()
		{
			throw new NotSupportedException ();
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}


	}
	public class DoubleConstant : Constant
	{
		public readonly double Value;

	
		public DoubleConstant ( double v, Location loc)
			: base (loc)
		{
			this.type = KnownTypeReference.Double;
		

			Value = v;
		}



		public override object GetValue ()
		{
			return Value;
		}

		public override string GetValueAsLiteral ()
		{
			return Value.ToString ();
		}

		public override long GetValueAsLong ()
		{
			throw new NotSupportedException ();
		}

		public override bool IsDefaultValue {
			get {
				return Value == 0;
			}
		}

		public override bool IsNegative {
			get {
				return Value < 0;
			}
		}

	

	}
	public class StringConstant : Constant {
	
		public StringConstant (string s, Location loc)
			: base (loc)
		{
			this.type = KnownTypeReference.String;
	

			Value = s;
		}

		protected StringConstant (Location loc)
			: base (loc)
		{
		}

		public string Value { get; protected set; }

		public override object GetValue ()
		{
			return Value;
		}

		public override string GetValueAsLiteral ()
		{
			// FIXME: Escape the string.
			return "\"" + Value + "\"";
		}

		public override long GetValueAsLong ()
		{
			throw new NotSupportedException ();
		}
		
	
		public override bool IsDefaultValue {
			get {
				return Value == null;
			}
		}

		public override bool IsNegative {
			get {
				return false;
			}
		}

		public override bool IsNull {
			get {
				return IsDefaultValue;
			}
		}

	
	}

	//
	// Null constant can have its own type, think of `default (Foo)'
	//
	public class NullConstant : Constant
	{
		public NullConstant (Location loc)
			: base (loc)
		{
	
			this.type = KnownTypeReference.Object;
		}

	
		

		public override object GetValue ()
		{
			return null;
		}

        public override string GetValueAsLiteral()
        {
            return "null";
        }


		public override long GetValueAsLong ()
		{
			throw new NotSupportedException ();
		}

		public override bool IsDefaultValue {
			get { return true; }
		}

		public override bool IsNegative {
			get { return false; }
		}

		public override bool IsNull {
			get { return true; }
		}

		public override bool IsZeroInteger {
			get { return true; }
		}
	}


	//
	// A null constant in a pointer context
	//
	class NullPointer : NullConstant
	{
		public NullPointer (Location loc)
			: base (loc)
		{
		}

	

	
	}


}
