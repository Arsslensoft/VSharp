﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.Base;

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
            CompilationSourceFile csf = new CompilationSourceFile(new Context.CompilerContext(new CompilerSettings()), sf);
            ParserSession ps = new ParserSession();
            //  Tokenizer cs = new Tokenizer(ssr,csf,ps,mc.Compiler.Report);
            VSharpParser vp = new VSharpParser(ssr, csf, csf.Compiler.Report, ps);
            vp.parse();
            st.Stop();
            Console.WriteLine(st.Elapsed);
        }
        static void Main(string[] args)
        {
            foreach (string g in Directory.GetFiles(@"C:\Users\Arsslen\Desktop\AST", "*.cs", SearchOption.AllDirectories))
                Parse(g);
            Console.Read();
        }
    }
}
