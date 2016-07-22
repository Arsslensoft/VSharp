
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono.CSharp
{
    public class NativeEmitContext
    {
        public StreamWriter Writer { get; set; }
        public int CurrentLine { get; set; }
        public NativeEmitContext(StreamWriter sw)
        {
            Writer = sw;
            CurrentLine = 0;

        }
        public virtual void EmitCode(string code)
        {
            Writer.WriteLine(code);
            CurrentLine++;
        }
        public virtual void EmitInLine(string code)
        {
            Writer.Write(code);
        }

    }
  public  abstract class Register
    {


 
      public string Name { get; set; }
      public bool Is64BitRegister { get; set; }
      public string Id { get; set; }
      public NativeTypes NType {get;set;}


    
     
    }
  
  public class EAX : Register
  {
      public EAX()
      {
         
          Name = "EAX";
          Id = "a";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
      }

     


  }
  public class EBX : Register
  {
      public EBX()
      {
       
          Id = "b";
          Name = "EBX";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
   
      }

  }
  public class ECX : Register
  {
      public ECX()
      {
        
          Id = "c";
          Name = "ECX";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
        
      }

   


  }
  public class EDX : Register
  {
      public EDX()
      {
        
          Id = "d";
          Name = "EDX";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
     
      }

   

  }
  public class ESI : Register
  {
      public ESI()
      {
        
          Id = "S";
          Name = "ESI";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
        
      }




  }
  public class EDI : Register
  {
      public EDI()
      {
        
          Id = "D";
          Name = "EDI";
          Is64BitRegister = false;
          NType = NativeTypes.UINT;
    
      }



  }

  public class RAX : Register
  {
      public RAX()
      {
         
          Id = "a";
          Name = "RAX";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
  public class RBX : Register
  {
      public RBX()
      {
      
          Id = "b";
          Name = "RBX";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
  public class RCX : Register
  {
      public RCX()
      {
       
          Id = "c";
          Name = "RCX";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
  public class RDX : Register
  {
      public RDX()
      {
        
          Id = "d";
          Name = "RDX";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
  public class RSI : Register
  {
      public RSI()
      {
       
          Id = "S";
          Name = "RSI";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }

  public class RDI : Register
  {
      public RDI()
      {
        
          Id = "D";
          Name = "RDI";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }

  public class GBL : Register
  {
      public GBL()
      {

          Id = "g";
          Name = "GBL";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
  public class MEM : Register
  {
      public MEM()
      {

          Id = "m";
          Name = "MEM";
          Is64BitRegister = true;
          NType = NativeTypes.ULONG;

      }



  }
}
