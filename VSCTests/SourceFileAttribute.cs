using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using VSC;
using VSC.AST;
using VSC.Base;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSCTests
{
    class SourceFileAttribute : Attribute
    {
        public List<AbstractMessage> Errors { get; set; }

        public SourceFileAttribute(string file)
        {

            SeekableStreamReader ssr = new SeekableStreamReader(File.OpenRead(file), Encoding.UTF8);
            SourceFile sf = new SourceFile(Path.GetFileName(file), file, 1);
            CompilationSourceFile csf = new CompilationSourceFile(new VSC.Context.CompilerContext(new CompilerSettings(),false), sf);
            ParserSession ps = new ParserSession();
            //  Tokenizer cs = new Tokenizer(ssr,csf,ps,mc.Compiler.Report);
            VSharpParser vp = new VSharpParser(ssr, csf, csf.Compiler.Report, ps);
            vp.parse();


            // ResolveScope
            IProjectContent pc = new VSharpProjectContent();
            pc = pc.AddOrUpdateFiles(csf);
            pc = pc.AddAssemblyReferences(MinimalCorlib.Instance);
            var c = pc.CreateCompilation();

            ResolveContext rc = new ResolveContext(c, csf.Compiler.Report);
            csf.Resolve(rc);
            Errors = (csf.Compiler.Report.Printer as ListReportPrinter).Messages;
        }
    }
}
