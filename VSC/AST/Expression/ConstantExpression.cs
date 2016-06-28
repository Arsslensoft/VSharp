using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using VSC.Base;
using VSC.Base.GoldParser.Parser;
using VSC.Base.GoldParser.Semantic;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;
namespace VSC.AST {

    /// <summary>
    /// This static class involves helper methods that use strings.
    /// </summary>
    public static class StringHelper
    {
        // --------------------------------------------------------------------------------
        /// <summary>
        /// Converts a V# literal string into a normal string.
        /// </summary>
        /// <param name="source">Source V# literal string.</param>
        /// <returns>
        /// Normal string representation.
        /// </returns>
        // --------------------------------------------------------------------------------
        public static string StringFromVSharpLiteral(string source)
        {
            StringBuilder sb = new StringBuilder(source.Length);
            int pos = 0;
            while (pos < source.Length)
            {
                char c = source[pos];
                if (c == '\\')
                {
                    // --- Handle escape sequences
                    pos++;
                    if (pos >= source.Length) throw new ArgumentException("Missing escape sequence");
                    switch (source[pos])
                    {
                        // --- Simple character escapes
                        case '\'': c = '\''; break;
                        case '\"': c = '\"'; break;
                        case '\\': c = '\\'; break;
                        case '0': c = '\0'; break;
                        case 'a': c = '\a'; break;
                        case 'b': c = '\b'; break;
                        case 'f': c = '\f'; break;
                        case 'n': c = ' '; break;
                        case 'r': c = ' '; break;
                        case 't': c = '\t'; break;
                        case 'v': c = '\v'; break;
                        case 'x':
                            // --- Hexa escape (1-4 digits)
                            StringBuilder hexa = new StringBuilder(10);
                            pos++;
                            if (pos >= source.Length)
                                throw new ArgumentException("Missing escape sequence");
                            c = source[pos];
                            if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                            {
                                hexa.Append(c);
                                pos++;
                                if (pos < source.Length)
                                {
                                    c = source[pos];
                                    if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                                    {
                                        hexa.Append(c);
                                        pos++;
                                        if (pos < source.Length)
                                        {
                                            c = source[pos];
                                            if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                                              (c >= 'A' && c <= 'F'))
                                            {
                                                hexa.Append(c);
                                                pos++;
                                                if (pos < source.Length)
                                                {
                                                    c = source[pos];
                                                    if (Char.IsDigit(c) || (c >= 'a' && c <= 'f') ||
                                                      (c >= 'A' && c <= 'F'))
                                                    {
                                                        hexa.Append(c);
                                                        pos++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            c = (char)Int32.Parse(hexa.ToString(), NumberStyles.HexNumber);
                            pos--;
                            break;
                        case 'u':
                            // Unicode hexa escape (exactly 4 digits)
                            pos++;
                            if (pos + 3 >= source.Length)
                                throw new ArgumentException("Unrecognized escape sequence");
                            try
                            {
                                uint charValue = UInt32.Parse(source.Substring(pos, 4),
                                  NumberStyles.HexNumber);
                                c = (char)charValue;
                                pos += 3;
                            }
                            catch (SystemException)
                            {
                                throw new ArgumentException("Unrecognized escape sequence");
                            }
                            break;
                        case 'U':
                            // Unicode hexa escape (exactly 8 digits, first four must be 0000)
                            pos++;
                            if (pos + 7 >= source.Length)
                                throw new ArgumentException("Unrecognized escape sequence");
                            try
                            {
                                uint charValue = UInt32.Parse(source.Substring(pos, 8),
                                  NumberStyles.HexNumber);
                                if (charValue > 0xffff)
                                    throw new ArgumentException("Unrecognized escape sequence");
                                c = (char)charValue;
                                pos += 7;
                            }
                            catch (SystemException)
                            {
                                throw new ArgumentException("Unrecognized escape sequence");
                            }
                            break;
                        default:
                            throw new ArgumentException("Unrecognized escape sequence");
                    }
                }
                pos++;
                sb.Append(c);
            }
            return sb.ToString();
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// Converts a V# verbatim literal string into a normal string.
        /// </summary>
        /// <param name="source">Source V# literal string.</param>
        /// <returns>
        /// Normal string representation.
        /// </returns>
        // --------------------------------------------------------------------------------
        public static string StringFromVerbatimLiteral(string source)
        {
            StringBuilder sb = new StringBuilder(source.Length);
            int pos = 0;
            while (pos < source.Length)
            {
                char c = source[pos];
                if (c == '\"')
                {
                    // --- Handle escape sequences
                    pos++;
                    if (pos >= source.Length) throw new ArgumentException("Missing escape sequence");
                    if (source[pos] == '\"') c = '\"';
                    else throw new ArgumentException("Unrecognized escape sequence");
                }
                pos++;
                sb.Append(c);
            }
            return sb.ToString();
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// Converts a V# literal string into a normal character..
        /// </summary>
        /// <param name="source">Source V# literal string.</param>
        /// <returns>
        /// Normal char representation.
        /// </returns>
        // --------------------------------------------------------------------------------
        public static char CharFromVSharpLiteral(string source)
        {
            string result = StringFromVSharpLiteral(source);
            if (result.Length != 1)
                throw new ArgumentException("Invalid char literal");
            return result[0];
        }
    }
    public class LiteralExpression : Expression
    {
        public ConstantExpression ConstantExpr { get; set; }
        public string GetSuffix(string value, bool hex = false)
        {
            string suffix = "";
            foreach (char c in value.ToLower().ToCharArray())
                if (c == 'u' || c == 'l' || c == 'd' || (c == 'f' && !hex))
                    suffix += c;
             
                

            return suffix;

        }
        public bool IsUnsigned(string suffix)
        {
            return suffix.Contains("u");
        }
        public bool IsLong(string suffix)
        {
            return suffix.Contains("l");
        }
        public bool IsFloat(string suffix)
        {
            return suffix.Contains("f");
        }
        public bool IsDouble(string suffix)
        {
            return suffix.Contains("d");
        }
        public string GetString(string value, bool verbatim = false)
        {
            if (verbatim)
                return StringHelper.StringFromVerbatimLiteral(value.Substring(1, value.Length - 2));
            else return StringHelper.StringFromVSharpLiteral(value.Substring(1, value.Length - 2));
        }
    }
    public class PrimitiveConstantExpression : ConstantExpression
    {
        public PrimitiveConstantExpression(ITypeReference tref, object val, LineInfo pos) : base(tref,val,pos)
        {
      
        }

        public override ResolveResult Resolve(VSharpResolver resolver)
        {
            return new ConstantResolveResult(Type.Resolve(resolver.CurrentTypeResolveContext), Value);
        }

    }
    public class ConstantExpression : Expression, IConstantValue
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

        public MultilineStringLiteral _multiline_string_constant;
        public CharacterLiteral _character_constant;
        public IntegralLiteral _integral_constant;
        public BooleanLiteral _boolean_constant;
        public NullLiteral _null_constant;
        public FloatLiteral _float_constant;

       
   
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
