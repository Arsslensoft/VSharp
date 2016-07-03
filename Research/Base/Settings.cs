using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VSC.Base.CommandLine;
using VSC.Base.CommandLine.Text;

namespace VSC
{
    public sealed class FileListAttribute : System.Attribute
    {
        public FileListAttribute(Type concreteType)
        {

        }

        public int MaximumElements { get; set; }
    }
    public enum Platform
    {
        Intel16,
        x86
    }
    public enum Target
    {
        flat,
        tiny,
        vexe,
        obj
    }
   public class CompilerSettings
   {
       public string DocumentationFile { get; set; }
        [Option("tabsize", Required = false, DefaultValue = 8,
HelpText = "Tab size")]
       public int TabSize { get; set; }
       [Option("ovfc", Required = false, DefaultValue = false,
HelpText = "Enable overflow checking")]
       public bool CheckForOverflow { get; set; }

       [Option( "warnlevel", HelpText = "Warning level", DefaultValue =0)]
       public int WarningLevel { get; set; }


       [Option('l', "langver", HelpText = "Language version", DefaultValue = LanguageVersion.Default)]
       public LanguageVersion Version { get; set; }

       [Option('v', "verbose", HelpText = "Print details during execution.",DefaultValue=false)]
         public bool Verbose { get; set; }

       [Option('g', "debug", HelpText = "Debug option", DefaultValue = false)]
       public bool Debug { get; set; }

       [Option('f', "float", HelpText = "Floating point support", DefaultValue = false)]
       public bool FloatingPointEnabled { get; set; }


       [Option('p', "platform", Required = false, DefaultValue = Platform.Intel16,
HelpText = "Platform")]
       public Platform Platform { get; set; }

       [Option('t', "target", Required = false, DefaultValue = Target.flat,
HelpText = "Target output")]
       public Target Target { get; set; }

        [Option( "optimize", Required = false, DefaultValue = false,
   HelpText = "Enable optimizations")]
        public bool Optimize { get; set; }

        [Option('p', "pplevel", Required = false, DefaultValue = 1,
 HelpText = "Preprocessing Level")]
        public int PreprocessLevel { get; set; }

        [Option( "pc", Required = false, DefaultValue = 1,
HelpText = "Parallel Compilation threads")]
        public int ParallelThreads { get; set; }

               [Option('z', "optimizelevel", Required = false, DefaultValue = 0,
   HelpText = "Optimizations Level")]
        public int OptimizeLevel { get; set; }

            [Option("Werror", Required = false, DefaultValue = false,
   HelpText = "Warn as error")]
               public bool WarningsAreErrors { get; set; }

            [OptionArray("dwarne", HelpText = "Warnings as errors.")]
            public int[] WarningsAsErrors { get; set; }
            [OptionArray('w', "dwarn", HelpText = "Disable warnings.")]
            public int[] DisabledWarnings { get; set; }


       [OptionArray('i', "include", HelpText = "Include directory.")]
        public string[] Includes { get; set; }

       [OptionArray( "sym", HelpText = "Symbols.")]
       public string[] Symbols { get; set; }

       [OptionArray('l', "library", HelpText = "Object files directory.")]
       public string[] Libraries { get; set; }

       [OptionArray('s', "source", Required = true, HelpText = "Source files.")]
       public string[] Sources { get; set; }

  
    
       [OptionArray('a', "asm", Required = true,HelpText = "Output assembly file")]
       public string[] AssemblyOutput { get; set; }
       
       [Option("boot", Required = false,  DefaultValue = false,
HelpText = "Bootloader")]
       public bool BootLoader { get; set; }

       [Option("int", Required = false, DefaultValue = false,
HelpText = "Interrupts definition")]
       public bool IsInterrupt { get; set; }



       [OptionArray('o',"out", HelpText = "Output files list.", Required = true)]
       public string[] OutputFiles { get; set; }




       [Option("flow", Required = false, DefaultValue = false,
HelpText = "Do flow analysis")]
       public bool Flow { get; set; }
            [ParserState]
            public IParserState LastParserState { get; set; }
            [HelpOption]
            public string GetUsage()
            {
                var help =          HelpText.AutoBuild(this,
                  (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));

                help.Copyright = "Copyright (c) 2016 Phexon Foundation. All rights reserved";
                help.AdditionalNewLineAfterOption = false;
                help.Heading = "V# Compiler";
                return help;

            }
    }
}
