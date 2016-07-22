using IKVM.Reflection;
using IKVM.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono.CSharp
{
    public class AsmParameter
    {
        public IKVM.Reflection.Type ParameterType { get; set; }
        public string CType { get; set; }

        public string ParameterName {get;set;}
        public Expression ParameterExpr { get; set; }
        public bool IsOutParameter { get; set; }


        public Universe Univ { get; set; }

        public AsmParameter(string name, Expression expr, bool isoutparam,Universe univ)
        {
            Univ = univ;
            IsOutParameter = isoutparam;
            ParameterExpr = expr;
            ParameterName = name;
            ParameterType = GetTypeFromVarName(expr);
     
        }

        public IKVM.Reflection.Type GetTypeFromVarName(Expression expr)
        {
            string type = GetTypeFromExprAsString(expr);
            if (type.Contains("*"))
                return GetAlType(type).MakePointerType();
            else if (type.Contains("["))
                return GetAlType(type).MakeArrayType();
            else return GetAlType(type);

        }
        // Al type String
        public string GetTypeFromExprAsString(Expression expr)
        {
            var field = expr as FieldExpr;
            var prop = expr as PropertyExpr;
            var cnst = expr as ConstantExpr;
            var locvar = expr as LocalVariableReference;
            var param = expr as ParameterReference;

            if (field != null)
            {

                return field.Spec.MemberType.ToString();

            }
            else if (prop != null)
            {
                // array - pointer - none

                return prop.Type.ToString();
            }
            else if (locvar != null)
            {

                return locvar.local_info.Type.ToString();
            }
            else if (cnst != null)
            {
                // array - pointer - none

                return cnst.constant.MemberType.ToString();
            }
            else if (param != null)
            {
                // array - pointer - none

                return param.Parameter.Type.ToString();
            }



            // CONST MANIPULATION
            var strc = expr as StringConstant;
            var bytec = expr as ByteConstant;

            var intc = expr as IntConstant;
            var uintc = expr as UIntConstant;
            var shortc = expr as ShortConstant;
            var ushortc = expr as UShortConstant;
            var longc = expr as LongConstant;
            var ulongc = expr as ULongConstant;

            var dblc = expr as DoubleConstant;
            var floatc = expr as FloatConstant;

            var boolc = expr as BoolConstant;


            if (intc != null)
                return "integer";
            else if (uintc != null)
                return "uinteger";

            else if (shortc != null)
                return "shortint";
            else if (ushortc != null)
                return "ushortint";

            else if (longc != null)
                return "longint";
            else if (ulongc != null)
                return "ulongint";

            else if (bytec != null)
                return "byte";
            else if (boolc != null)
                return "bool";
            else if (strc != null)
                return "string";

            else if (floatc != null)
                return "float";
            else if (dblc != null)
                return "real";


            return null;

        }
        // altype string -> IKVMTYPE
        public IKVM.Reflection.Type GetAlType(string type)
        {

            switch (type.Replace("[]", "").Replace("*", ""))
            {
                case "ushortint":
                    return Univ.System_UInt16;
                case "uinteger":
                    return Univ.System_UInt32;
                case "ulongint":
                    return Univ.System_UInt64;

                case "shortint":
                    return Univ.System_Int16;
                case "integer":
                    return Univ.System_Int32;
                case "longint":
                    return Univ.System_Int64;


                case "single":
                case "float":
                    return Univ.System_Single;

                case "real":
                    return Univ.System_Double;

                case "string":
                    return Univ.System_String;

                case "sbyte":
                case "byte":
                    return Univ.System_Byte;

                case "bool":
                    return Univ.System_Int32;



                default:
                    return Univ.System_IntPtr;


            }
        }
        // Al type -> C TYPE
        public string GetCType(string type)
        {
            type = type.Replace("[]", "").Replace("*", "");
            switch (type)
            {
                case "ushortint":
                    return "unsigned short";
                case "uinteger":
                    return "unsigned long int";
                case "ulongint":
                    return "unsigned long long int";

                case "shortint":
                    return "short";
                case "integer":
                    return "long int";
                case "longint":
                    return "long long int";


                case "single":
                case "float":
                    return "float";

                case "real":
                    return "double";

                case "string":
                    return "char*";

                case "sbyte":
                case "byte":
                    return "char";

                case "bool":
                    return "int";



                default:
                    return "void*";


            }
        }
        public string GetPointerType(string type)
        {
            if (type.Contains("[]"))
                return "*" + GetPointerType(type.Replace("[]", ""));
            else if (type.Contains("*"))
                return "*" + GetPointerType(type.Remove(type.IndexOf('*'), 1));
            else if (IsOutParameter)
                return "*";
            else return "";
        }

        public bool Resolve(Report rc)
        {
            try
            {
                string type = GetTypeFromExprAsString(ParameterExpr);
              
                CType =   GetCType(type)+GetPointerType(type);
                return true;
            }
            catch
            {
                AssemblerErrorManager.AsmGenErrorResolveAsmParameter(rc, " Parameter " + ParameterName);
                return false;

            }


        }

        public void Emit(NativeEmitContext nec, Report ec, bool isfirst = false)
        {
            if (Resolve(ec))
            {
                if (!isfirst)
                    nec.EmitInLine(", " + NativeTypeBridge.GenerateParameter(CType, ParameterName));
                else nec.EmitInLine(NativeTypeBridge.GenerateParameter(CType, ParameterName));
            }
        }


    }

    public enum OperandType : byte
    {
        REGISTER_LOAD = 0,
        REGISTER_ASSIGN = 1
    }
    public class Operand
    {
        public OperandType OType { get; set; }
        public AsmParameter Parameter { get; set; }
        public Register Reg { get; set; }

        public Operand(OperandType ot, AsmParameter param, Register reg)
        {
            Reg = reg;
            OType = ot;
            Parameter = param;
        }

    }

    public class InlineAsmDeclaration
    {
        public string MethodName { get; set; }
        public List<Operand> InputOperands { get; set; }
        public List<Operand> OutputOperands { get; set; }

        public List<Register> InputRegisters { get; set; }
        public List<Register> OutputRegisters { get; set; }

        public string AsmCode { get; set; }
        public Dictionary<string, string> OverrideVars { get; set; }
        public List<AsmParameter> Parameters { get; set; }
        public List<string> OutputVars { get; set; }

        // Error Location
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public Location SrcLocation { get; set; }

        public InlineAsmDeclaration(Location sloc)
        {
            MethodName = GenerateAsmName();
            InputOperands = new List<Operand>();
            OutputOperands = new List<Operand>();

            InputRegisters = new List<Register>();
            OutputRegisters = new List<Register>();

            Parameters = new List<AsmParameter>();
            OutputVars = new List<string>();
            OverrideVars = new Dictionary<string, string>();
       
            SrcLocation = sloc;
            StartLine = 0;
            EndLine = 0;
        }

        public static int AsmCallId = -1;
        public static string GenerateAsmName()
        {
            AsmCallId++;
            return "al_call_asm_" + AsmCallId.ToString();
        }

        public void Emit(NativeEmitContext nec, Report ec)
        {

            nec.EmitInLine(NativeBridge.GenerateMethodHeader(MethodName));
            byte i = 0;
            foreach (AsmParameter param in Parameters)
            {
                param.Emit(nec, ec, i == 0);
                i++;
            }
            nec.EmitInLine(")");
            nec.EmitCode("{");
            string inop = "", outop = "";
            // IO Operands
            foreach (Operand ninst in InputOperands)
                   inop += ", " + NativeBridge.AssignRegister(ninst.Reg, ninst.Parameter.ParameterName, OutputVars, OverrideVars);
               
              foreach (Operand ninst in OutputOperands)    
                    outop += ", " + NativeBridge.GetRegister(ninst.Reg, ninst.Parameter.ParameterName, OutputVars, OverrideVars);

          
            // Remove ,
            if (!string.IsNullOrEmpty(inop))
                inop = inop.Remove(0, 1);

            if (!string.IsNullOrEmpty(outop))
                outop = outop.Remove(0, 1);

            // Normalize Asm Code
            StringBuilder sb = new StringBuilder();
            AsmCode = AsmCode.Trim().Replace("\r\n", "");
            foreach (string inst in AsmCode.Split(';'))
                if(!string.IsNullOrEmpty(inst))
                sb.AppendLine("\"" + inst.Trim() + ";\"");


            StartLine = nec.CurrentLine;
            nec.EmitCode("");

            nec.EmitInLine("asm(");
            nec.EmitInLine(sb.ToString());
            nec.EmitInLine(" ");
            // only input
            if (!string.IsNullOrEmpty(inop) && string.IsNullOrEmpty(outop))
                nec.EmitInLine(": :" + inop);
            else if (string.IsNullOrEmpty(inop) && !string.IsNullOrEmpty(outop)) // only output
                nec.EmitInLine(": " + outop);
            else if (!string.IsNullOrEmpty(inop) && !string.IsNullOrEmpty(outop)) // both
                nec.EmitInLine(": " + outop + " :" + inop);

            nec.EmitInLine(");");

            nec.EmitCode("");
            EndLine = nec.CurrentLine;
            nec.EmitCode("}");
            nec.EmitCode(" ");
        }

    }

    public class NativeCodeDeclaration
    {
       
        public List<InlineAsmDeclaration> AssemblyDeclarations { get; set; }
        public List<string> Includes { get; set; }
        public List<string> EmitedDeclarations { get; set; }
        public NativeCodeDeclaration()
        {
            Includes = new List<string>();
            AssemblyDeclarations = new List<InlineAsmDeclaration>();
            EmitedDeclarations = new List<string>();
        }
        public void Emit(string file, Report ec)
        {
            try
            {
                using (StreamWriter str = new StreamWriter(file, false))
                {
                    NativeEmitContext nec = new NativeEmitContext(str);


                    // Includes
                    foreach (string inc in Includes)
                        nec.EmitCode(NativeBridge.AddInclude(inc));

                    // DLL EXPORT
                    nec.EmitCode(NativeBridge.GetDeclaration());

                    // Assembler Declarations
                    foreach (InlineAsmDeclaration adcl in AssemblyDeclarations)
                        adcl.Emit(nec, ec);


                }
            }
            catch (Exception ex)
            {
                AssemblerErrorManager.AsmGenError(ec, ". " + ex.Message);
            }
        }
    }

    public class AssemblerGenerator
    {
        public static string DllName = "Al.native.dll";
        static object GetValueFromExpr(Expression expr)
        {
            // Cast
            var strc = expr as StringConstant;
            var bytec = expr as ByteConstant;
            var sbytec = expr as SByteConstant;
            var intc = expr as IntConstant;
            var uintc = expr as UIntConstant;
            var shortc = expr as ShortConstant;
            var ushortc = expr as UShortConstant;
            var longc = expr as LongConstant;
            var ulongc = expr as ULongConstant;

            var dblc = expr as DoubleConstant;
            var floatc = expr as FloatConstant;
            var decc = expr as DecimalConstant;

            var charc = expr as CharConstant;
            var boolc = expr as BoolConstant;


            // String
            if (strc != null)
                return strc.Value;


            // Int
            if (intc != null)
                return intc.Value;

            // Uint
            if (uintc != null)
                return uintc.Value;

            // Short
            if (shortc != null)
                return shortc.Value;

            // UShort
            if (ushortc != null)
                return ushortc.Value;

            // Long
            if (longc != null)
                return longc.Value;


            // Ulong
            if (ulongc != null)
                return ulongc.Value;

            // Double
            if (dblc != null)
                return dblc.Value;

            // Float
            if (floatc != null)
                return floatc.Value;
            // Byte
            if (bytec != null)
                return bytec.Value;



            // Sbyte
            if (sbytec != null)
                return sbytec.Value;



            // Char
            if (charc != null)
                return charc.Value;


            // Boolean
            if (boolc != null)
                return boolc.Value;




            return null;
        }

        internal static MethodBuilder GenerateMethod(EmitContext ec, InlineAsmDeclaration decl, TypeBuilder tb, Universe univ)
        {

            //   string name = "MessageBox";
            List<IKVM.Reflection.Type> param = new List<IKVM.Reflection.Type>();
            foreach (AsmParameter pdcl in decl.Parameters)
            {
                if (pdcl.IsOutParameter)
                    param.Add(pdcl.ParameterType.MakePointerType());
                else
                    param.Add(pdcl.ParameterType);
            }
            // return type
            IKVM.Reflection.Type retype = univ.System_Void;

          
            // declr
            MethodBuilder mb = tb.DefinePInvokeMethod(decl.MethodName, DllName, IKVM.Reflection.MethodAttributes.PinvokeImpl | IKVM.Reflection.MethodAttributes.Static | IKVM.Reflection.MethodAttributes.Public, IKVM.Reflection.CallingConventions.Standard, retype, param.ToArray(), System.Runtime.InteropServices.CallingConvention.StdCall, System.Runtime.InteropServices.CharSet.Ansi);
            // param name

            for (int i = 0; i < param.Count; i++)
            {
                ParameterAttributes pa = ParameterAttributes.None;
                if (decl.Parameters[i].IsOutParameter)
                    pa = ParameterAttributes.Out;
               

            ParameterBuilder pb =    mb.DefineParameter(i + 1, pa, decl.Parameters[i].ParameterName);

           }
            // out var

      //     tb.CreateType();
            return mb;
        }
        internal static bool EmitStackCall(EmitContext ec, MethodBuilder mb, InlineAsmDeclaration decl, Dictionary<string, LocalBuilder> lbdef)
        {
            int i = -1;
            foreach (AsmParameter np in decl.Parameters)
            {
                i++;
                //if (pdcl.ptype == ParameterType.Pointer)
                //{
                //    ec.Report.Error(3665, "ASM: Pointers are not supported in asm use iasm instead");
                //    return false ;
                //}
                // Emit push eval stack
                // Local Vars

                if (lbdef.ContainsKey(np.ParameterName))
                {
                    LocalBuilder lb = lbdef[np.ParameterName];
                    int idx = lb.LocalIndex;
                  
                    EmitStack(ec, idx, false, np.IsOutParameter);
                    continue;
                }
                // LocalVariableReference
                var lvar = np.ParameterExpr as LocalVariableReference;
                if (lvar != null)
                {
                   
                    int idx = lvar.local_info.builder.LocalIndex;
                    EmitStack(ec, idx, false, np.IsOutParameter);
                    continue;
                }

                // Parameters
                var par = np.ParameterExpr as ParameterReference;
                if (par != null)
                {
                   
                    int idx = par.Parameter.Index;
                    EmitStack(ec, idx, true, np.IsOutParameter);
                    continue;
                }
                // Field
                var field = np.ParameterExpr as FieldExpr;

                if (field != null)
                {
                
                    EmitStack(ec, field, np.ParameterName, np.IsOutParameter);
                    continue;
                }
                // Property
                var prop = np.ParameterExpr as PropertyExpr;
                if (prop != null)
                {

                    EmitStack(ec, prop, np.ParameterName, np.IsOutParameter);
                    continue;
                }
                // Constant
                var cnst = np.ParameterExpr as ConstantExpr;
                if (cnst != null)
                {

                    EmitStack(ec, cnst, np.ParameterName);
                    continue;
                }

                // CONST MANIP
                var strc = np.ParameterExpr as StringConstant;
                var bytec = np.ParameterExpr as ByteConstant;
                var intc = np.ParameterExpr as IntConstant;
                var uintc = np.ParameterExpr as UIntConstant;
                var shortc = np.ParameterExpr as ShortConstant;
                var ushortc = np.ParameterExpr as UShortConstant;
                var longc = np.ParameterExpr as LongConstant;
                var ulongc = np.ParameterExpr as ULongConstant;

                var dblc = np.ParameterExpr as DoubleConstant;
                var floatc = np.ParameterExpr as FloatConstant;

                var boolc = np.ParameterExpr as BoolConstant;

                if (intc != null)
                {
                    EmitStack(ec, intc.Value);
                    continue;
                }
                if (uintc != null)
                {
                    EmitStack(ec, (int)uintc.Value);
                    continue;
                }
                if (shortc != null)
                {
                    EmitStack(ec, (int)shortc.Value);
                    continue;
                }
                if (ushortc != null)
                {
                    EmitStack(ec, (int)ushortc.Value);
                    continue;
                }
                if (bytec != null)
                {
                    EmitStack(ec, (int)bytec.Value);
                    continue;
                }
                if (boolc != null)
                {
                    EmitStack(ec, boolc.Value ? 1 : 0);
                    continue;
                }

                if (longc != null)
                {
                    EmitStack(ec, longc.Value);
                    continue;
                }
                if (ulongc != null)
                {
                    EmitStack(ec, (long)ulongc.Value);
                    continue;
                }


                if (dblc != null)
                {
                    EmitStack(ec, dblc.Value);
                    continue;
                }
                if (floatc != null)
                {
                    EmitStack(ec, floatc.Value);
                    continue;
                }
                if (strc != null)
                {
                    EmitStack(ec, strc.Value);
                    continue;
                }
                // Not supported
                ec.Report.Error(3665, "ASM: parameter not supported ");
                return false;




            }

          

            ec.Emit(OpCodes.Call, mb);

            ////   EmitReturn(ec, decl);
            return true;
        }




        internal static bool EmitStack(EmitContext ec, PropertyExpr property, string name, bool isadr)
        {

            if (property.PropertyInfo.Get == null)
            {
                ec.Report.Error(3664, "Asm code : the property " + name + " does not have a get accessor or does not exist");
                return false;
            }
            if (isadr)
            {
                ec.Report.Error(3664, "Asm code : the property " + name + " could not be used as POPARG");
                return false;

            }
            ec.Emit(OpCodes.Call, property.PropertyInfo.Get);
            //ec.Emit(OpCodes.Stloc_0);
            //ec.Emit(OpCodes.Ldloc_0);
            return true;
        }
        public static bool EmitStack(EmitContext ec, FieldExpr field, string name, bool isadr = false)
        {
            if (field.Spec == null)
            {
                ec.Report.Error(3664, "Asm code : the field " + name + " does not have a get accessor or does not exist");
                return false;
            }

            if (!isadr)
                ec.Emit(OpCodes.Ldsfld, field.Spec);
            else
                ec.Emit(OpCodes.Ldsflda, field.Spec);
            return true;
        }
        public static bool EmitStack(EmitContext ec, ConstantExpr constant, string name)
        {
            if (constant == null)
            {
                ec.Report.Error(3664, "Asm code : the field " + name + " does not have a get accessor or does not exist");
                return false;
            }


            object val = GetValueFromExpr(constant.constant.Value);
            switch (constant.constant.MemberType.ToString().ToLower())
            {

                case "string":
                    ec.Emit(OpCodes.Ldstr, (string)val);
                    break;

                case "ushortint":
                case "uinteger":
                case "shortint":
                case "integer":
                    ec.ig.Emit(OpCodes.Ldc_I4, (int)val);
                    break;

                case "longint":
                    ec.ig.Emit(OpCodes.Ldc_I8, (long)val);
                    break;

                case "single":
                case "float":
                    ec.Emit(OpCodes.Ldc_R4, (float)val);
                    break;

                case "real":
                    ec.Emit(OpCodes.Ldc_R8, (double)val);
                    break;

                case "byte":

                    ec.Emit(OpCodes.Ldc_I4, (byte)val);
                    break;
                default:
                    return false;
            }
            //     ec.Emit(OpCodes.Ldsfld, constant.constant);
            return true;
        }
        public static bool EmitStack(EmitContext ec, int index, bool isarg, bool isadr, bool store = false)
        {
            if (!store)
            {
                if (isarg)
                {
                    if (isadr)
                    {
                        if (index < 256)
                        {
                            ec.Emit(OpCodes.Ldarga_S);
                            ec.ig.code.Write((byte)index);
                        }
                        else
                        {
                            ec.Emit(OpCodes.Ldarga);
                            ec.ig.code.Write(index);
                        }
                        return true;
                    }
                    switch (index)
                    {
                        case 0:
                            ec.Emit(OpCodes.Ldarg_0);
                            break;
                        case 1:
                            ec.Emit(OpCodes.Ldarg_1);
                            break;
                        case 2:
                            ec.Emit(OpCodes.Ldarg_2);
                            break;
                        case 3:
                            ec.Emit(OpCodes.Ldarg_3);
                            break;
                        default:
                            if(index < 256){
                            ec.Emit(OpCodes.Ldarg_S);
                            ec.ig.code.Write((byte)index);
                               }
                        else
                        {
                            ec.Emit(OpCodes.Ldarg);
                            ec.ig.code.Write(index);
                        }
                            break;
                    }

                }
                else
                {
                    if (isadr)
                    {
                        if(index < 256){
                        ec.Emit(OpCodes.Ldloca_S);
                        ec.ig.code.Write((byte)index);
                                 }
                        else
                        {
                            ec.Emit(OpCodes.Ldloca);
                            ec.ig.code.Write(index);
                        }
                        return true;
                    }

                    switch (index)
                    {
                        case 0:
                            ec.Emit(OpCodes.Ldloc_0);
                            break;
                        case 1:
                            ec.Emit(OpCodes.Ldloc_1);
                            break;
                        case 2:
                            ec.Emit(OpCodes.Ldloc_2);
                            break;
                        case 3:
                            ec.Emit(OpCodes.Ldloc_3);
                            break;
                        default:
                            if(index < 256){
                                ec.Emit(OpCodes.Ldloc_S);
                                ec.ig.code.Write((byte)index);
                         
                            }
                        else
                        {
                        
                            ec.Emit(OpCodes.Ldloc);
                            ec.ig.code.Write(index);
                        }
                            break;
                    }
                }


                return true;
            }
            else
            {
                if (isarg)
                {
                    if (index < 256)
                    {
                        ec.Emit(OpCodes.Starg_S);
                        ec.ig.code.Write((byte)index);
                    }
                    else
                    {
                        ec.Emit(OpCodes.Starg);
                        ec.ig.code.Write(index);
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 0:
                            ec.Emit(OpCodes.Stloc_0);
                            break;
                        case 1:
                            ec.Emit(OpCodes.Stloc_1);
                            break;
                        case 2:
                            ec.Emit(OpCodes.Stloc_2);
                            break;
                        case 3:
                            ec.Emit(OpCodes.Stloc_3);
                            break;
                        default:
                            if(index < 256){
                                ec.Emit(OpCodes.Stloc_S);
                                ec.ig.code.Write((byte)index);
                                    }
                        else
                        {
                        
                           ec.Emit(OpCodes.Stloc);
                            ec.ig.code.Write(index);
                        }
                        
                            break;
                    }
                }
                return true;
            }
        }

        // CONST MANIPULATION
        public static bool EmitStack(EmitContext ec, int val)
        {
            ec.ig.Emit(OpCodes.Ldc_I4, val);

            return true;
        }
        public static bool EmitStack(EmitContext ec, float val)
        {
            ec.ig.Emit(OpCodes.Ldc_R4, val);

            return true;
        }
        public static bool EmitStack(EmitContext ec, long val)
        {
            ec.ig.Emit(OpCodes.Ldc_I8, val);

            return true;
        }
        public static bool EmitStack(EmitContext ec, double val)
        {
            ec.ig.Emit(OpCodes.Ldc_R8, val);

            return true;
        }
        public static bool EmitStack(EmitContext ec, string val)
        {
            ec.ig.Emit(OpCodes.Ldstr, val);

            return true;
        }


        public static bool Compile(string infile, string outfile, out string outerr, bool is32bits = false, bool intelsyntax = true)
        {
            ProcessStartInfo pi = new ProcessStartInfo();
            string isyn = "";
            if (intelsyntax)
                isyn = "-masm=intel ";

            if (!is32bits)
                pi.Arguments = "-shared "+isyn+"-o \"" + outfile + "\" \"" + infile + "\"";
            else
                pi.Arguments = "-shared " + isyn + "-m32 -o \"" + outfile + "\" \"" + infile + "\"";

            pi.FileName = AppDomain.CurrentDomain.BaseDirectory + @"MinGW64\bin\x86_64-w64-mingw32-gcc.exe";
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardError = true;
            pi.RedirectStandardOutput = true;

            Process p = Process.Start(pi);

            p.StartInfo = pi;
            p.Start();



            p.WaitForExit();
            outerr = p.StandardError.ReadToEnd();
            //string outp = p.StandardOutput.ReadToEnd();

            int exitCode = p.ExitCode;
            p.Close();

            if (exitCode == 0)
                return true;
            else
                return false;
        }
    }

}
