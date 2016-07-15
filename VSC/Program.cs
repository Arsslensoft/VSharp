using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.Base;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC
{
    class Program
    {
        static void Parse(string file)
        {
            Stopwatch st = new Stopwatch();
            st.Start();

            
            SeekableStreamReader ssr = new SeekableStreamReader(File.OpenRead(file), Encoding.UTF8);
            SourceFile sf = new SourceFile(Path.GetFileName(file), file, 1);
            ModuleContext mctx = new ModuleContext(new Context.CompilerContext(new CompilerSettings()));
            CompilationSourceFile csf = new CompilationSourceFile(mctx, sf);
            ParserSession ps = new ParserSession();
        
 
            //  Tokenizer cs = new Tokenizer(ssr,csf,ps,mc.Compiler.Report);
            VSharpParser vp = new VSharpParser(ssr, csf, csf.Compiler.Report, ps,mctx);
            vp.parse();


            // ResolveScope
            IProjectContent pc = new VSharpProjectContent();
            pc = pc.AddOrUpdateFiles(csf);
            pc = pc.AddAssemblyReferences(MinimalCorlib.Instance);
            var c = pc.CreateCompilation();
            
            ResolveContext rc = new ResolveContext(c,csf.Compiler.Report);
            csf.DoResolve(rc);
            st.Stop();
            Console.WriteLine(st.Elapsed);
        }
        static void Main(string[] args)
        {
           
            foreach (string g in Directory.GetFiles(@"C:\Users\Arsslen\Desktop\AST", "*.vs", SearchOption.AllDirectories))
                Parse(g);
            Console.Read();
        }
    }
}
