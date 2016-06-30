using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.Base.GoldParser;
using VSC.Base.GoldParser.Grammar;
using VSC.Base.GoldParser.Parser;
using VSC.Base.GoldParser.Semantic;
using VSC.Context;
using VSC.TypeSystem;
using VSC.TypeSystem.Resolver;

namespace VSC
{
    class K<I> where I:class
    {
      public  class J<L>
        {
            public I S(L k)
            {
                return null;
            }
        }
    }
    class T<H> where H:class
    {
        void J()
        {
            K<H>.J<int> s = new K<H>.J<int>();
            s.S(5);
        }
    }
    class Program
    {
      static  CompiledGrammar grammar = CompiledGrammar.Load(typeof(Program), "VS.egt");
      static SemanticTypeActions<Semantic> actions = new SemanticTypeActions<Semantic>(grammar);
        static void Parse(string file)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
       
           
            var processor = new SemanticProcessor<Semantic>(new ParserReader(file), actions);
            //var processor = new LalrProcessor(new Tokenizer(new ParserReader(file), grammar));

            ParseMessage parseMessage = processor.ParseAll();
            if (parseMessage == ParseMessage.SyntaxError)
            {
             var x =   processor.GetExpectedTokens();
             Console.WriteLine(processor.CurrentToken);

            }

            if (parseMessage == ParseMessage.Accept)
            {
                CompilationUnit cu = processor.CurrentToken as CompilationUnit;
                CompilerContext cctx = new CompilerContext();
                SymbolResolveContext srctx = new SymbolResolveContext(file, cctx);
                cu.Resolve(srctx);
                srctx.AddDefaultType();
                VSharpProjectContent vspc = new VSharpProjectContent();
                IProjectContent pc =   vspc.AddOrUpdateFiles(srctx.unresolvedFile);
                
                ICompilation comp = pc.CreateCompilation();
                ResolveContext rctx = new ResolveContext(comp, cctx);
                cu.DoResolve(rctx);
            }

            st.Stop();
            Console.WriteLine(st.Elapsed);
        }
        static void Main(string[] args)
        {
           
            Parse(@"..\..\Tests\test.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\a.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\c.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\c.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\a.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\a.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\c.vs");
            //Parse(@"C:\Users\Arsslen\Desktop\c.vs");
            Console.Read();
        }
    }
}
