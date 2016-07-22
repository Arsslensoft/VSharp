using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono.CSharp
{

    internal class AsmCodeDef : IComparable
    {


        public int CompareTo(object obj)
        {
            AsmCodeDef acd = obj as AsmCodeDef;
            if (acd.Line == Line && acd.Col == Col)
                return 0;

            if (acd.Line > Line)
                return -1;

            if (acd.Line == Line && acd.Col > Col)
                return -1;


            if (acd.Line < Line)
                return 1;

            if (acd.Line == Line && acd.Col < Col)
                return 1;

            // The orders are equivalent.
            return 0;
        }
        public int Line;
        public int Col;
        public int File;
        public string AsmCode;
        public SimpleAssign AssignInstruction;
    }



    public class AssemblerErrorManager
    {
        public static int BaseAsmErrorCode = 3660;
        public static void Error(int code, string err, Report ec)
        {
            ec.Error(code, err);
        }
        public static void AsmGenError(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode, "Failed to export asm code " + msg, ec);
        }
        public static void AsmGenErrorParam(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode + 1, "Failed to export asm code, parameter error " + msg, ec);
        }
        public static void AsmGenErrorConst(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode + 2, "Constant could not be used as output an operand " + msg, ec);
        }
        public static void AsmGenErrorRegister(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode + 3, "Platform mismatch please use proper registers for corresponding platform. " + msg, ec);
        }
        public static void AsmGenErrorRegisterTypeMismatch(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode + 5, "Register type mismatch. " + msg, ec);
        }

        public static void AsmGenErrorResolveAsmParameter(Report ec, string msg = "")
        {
            Error(BaseAsmErrorCode + 7, "Failed to resolve asm parameter. " + msg, ec);
        }
    }


    public enum NativeTypeAttribute : byte
    {
        None ,
        Array,
        Pointer,
        Unknown
        

    }
    public enum NativeInstructionType : byte
    {
        RegisterAssign,
        RegisterLoad,
        InlineAsm,
    }
    public enum NativeTypes : byte
    {
        BOOL,
       BYTE,
       UBYTE,

       SHORT,
        INT,
        LONG,

        USHORT,
        UINT,
        ULONG,

        FLOAT,
        DOUBLE,
        VOID,
        POINTER,
        STRING,
        UNKNOWN





    }
    public class NativeTypeBridge
    {
        public static string GetNativeType(NativeTypes nativ)
        {
            switch (nativ)
            {
                case  NativeTypes.BOOL:
                case NativeTypes.BYTE:
                    return "char";
                case NativeTypes.UBYTE:
                    return "unsigned char";
                case NativeTypes.SHORT:
                    return "short";
                case NativeTypes.INT:
                    return "long";
                case NativeTypes.LONG:
                    return "long long";
                case NativeTypes.USHORT:
                    return "unsigned short";
                case NativeTypes.UINT:
                    return "unsigned long";
                case NativeTypes.ULONG:
                    return "unsigned long long";
                case NativeTypes.VOID:
                    return "void";
                case NativeTypes.DOUBLE:
                    return "double";
                case NativeTypes.FLOAT :
                    return "float";
                case NativeTypes.POINTER:
                    return "HANDLE";
                default:
                    return null;
              


            }
        }
        public static string SetTypeAttribute(string ctype, NativeTypeAttribute nta)
        {
            if (nta == NativeTypeAttribute.Pointer || nta == NativeTypeAttribute.Array)
                return ctype + "*";
           else return ctype;

        }
        public static string GenerateParameter(string type, string param)
        {
            return type + " " + param;
        }
      


    }
    public class NativeCompileMessage
    {
        public int LineNumber { get; set; }

        public int CharNumber { get; set; }

        public string Message { get; set; }
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Project { get; set; }
        public enum MessageTypes : byte
        {
            Info = 0,
            Note = 1,
            Warning = 2,
            Error = 3
        };

        public MessageTypes Type { get; set; }
        public bool Compile;
        public NativeCompileMessage(int lineNumber, int charNumber, string message, MessageTypes type, string file)
        {
            Compile = false;
            LineNumber = lineNumber;
            CharNumber = charNumber;
            Message = message;
            Type = type;
            FileName = file;
            Code = "ALASM";
            Project = "";
        }
        public NativeCompileMessage(int lineNumber, int charNumber, string message, MessageTypes type)
        {
            Compile = false;
            LineNumber = lineNumber;
            CharNumber = charNumber;
            Message = message;
            Type = type;
            FileName = "";
            Code = "ALASM";
            Project = "";
        }
        public NativeCompileMessage(int lineNumber, int charNumber, string message, MessageTypes type, string file, bool com)
        {

            Compile = com;
            LineNumber = lineNumber;
            CharNumber = charNumber;
            Message = message;
            Type = type;
            FileName = file;
            Code = "ALASM";
            Project = "";
        }
        public NativeCompileMessage(int lineNumber, int charNumber, string message, MessageTypes type, bool com)
        {
            Compile = com;
            LineNumber = lineNumber;
            CharNumber = charNumber;
            Message = message;
            Type = type;
            Code = "ALASM";
            Project = "";
        }
        public NativeCompileMessage(int lineNumber, int charNumber, string message, MessageTypes type, string file, bool com, string code, string project)
        {

            Compile = com;
            LineNumber = lineNumber;
            CharNumber = charNumber;
            Message = message;
            Type = type;
            FileName = file;
            Code = code;
            Project = project;
        }


    }

   public class NativeBridge
    {
       public static string AddInclude(string include)
       {
           return "#include "+include;
       }
       public static string GetDeclaration()
       {
           return "#define DLL_EXPORT __declspec(dllexport)";
       }
       public static string GenerateMethodHeader(string name, string type = "void")
       {
          return "DLL_EXPORT "+type + " "+name + "(";
         
        
          


       }
    
       public static string GetRegister(Register reg, string varname, List<string> ptrvar, Dictionary<string,string> ovrvar)
       {

           if (ptrvar.Contains(varname))
               return string.Format("\"={0}\" ( *" + varname + " )",  reg.Id);
           else if(ovrvar.ContainsKey(varname))
               return string.Format("\"={0}\" ( " + ovrvar[varname] + " )", reg.Id);
           else
               return string.Format("\"={0}\" ( " + varname + " )", reg.Id);


           //if(isvarpointer)
           //return  string.Format("asm(\"movl  %%{0},%0;\": \"={1}\" ( *" + varname + " ) );",reg.Name.ToLower(),reg.Id);
           //else return string.Format("asm(\"movl  %%{0},%0;\": \"={1}\" ( " + varname + " )    );", reg.Name.ToLower(),reg.Id);
       }
       public static string AssignRegister(Register reg, string varname, List<string> ptrvar, Dictionary<string, string> ovrvar)
       {
           if (ptrvar.Contains(varname))
               return string.Format("\"{1}\" ( *{0} )", varname, reg.Id);
          else if(ovrvar.ContainsKey(varname))
                          return string.Format("\"{1}\" ( {0} )", ovrvar[varname], reg.Id);
           else
               return string.Format("\"{1}\" ( {0} )", varname, reg.Id);
       //  return string.Format( "asm(\"movl %0, %%{0};\":  : \"{2}\" ( {1} )  );",reg.Name.ToLower(),varname,reg.Id);
       }



    }
}
