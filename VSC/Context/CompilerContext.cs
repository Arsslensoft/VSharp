using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;

namespace VSC.Context
{
    interface IModuleSupport
    {
        ModuleContext Module { get; set; }
    }


    public class ModuleContext
    {
        public int CounterAnonymousTypes { get; set; }
        public CompilerContext Compiler { get; set; }
      
        public ModuleContext(CompilerContext compiler)
        {
            Compiler = compiler;

        }
    }
    public class CompilerContext
    {
        public static Report report;
        public static InterningProvider InternProvider = new SimpleInterningProvider();
        public CompilerContext(CompilerSettings settings = null,bool console=true)
        {
            Settings = settings;
            if(console)
            report = new Report(this, new ConsoleReportPrinter());
            else report = new Report(this, new ListReportPrinter());
        }
        public Report Report { get { return report; } }
        public CompilerSettings Settings { get; set; }
        public List<SourceFile> all_source_files;

        public SourceFile LookupFile(CompilationSourceFile comp_unit, string name)
        {
            if (all_source_files == null)
                all_source_files = new List<SourceFile>();
           

            string path;
            if (!Path.IsPathRooted(name))
            {
                var loc = comp_unit.SourceFile;
                string root = Path.GetDirectoryName(loc.FullPathName);
                path = Path.GetFullPath(Path.Combine(root, name));
                var dir = Path.GetDirectoryName(loc.Name);
                if (!string.IsNullOrEmpty(dir))
                    name = Path.Combine(dir, name);
            }
            else
                path = name;

         
          foreach(var src in all_source_files)
              if(src.FullPathName == name)
                return src;

          SourceFile retval = new SourceFile(name, path, all_source_files.Count + 1);
            all_source_files.Add(retval);
            return retval;
        }
    }
}
