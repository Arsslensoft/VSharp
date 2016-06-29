using System;
using System.IO;
using VSC.Base.GoldParser.Semantic;
using VSC.Context;
namespace VSC.AST {

     [Terminal("OctalLiteral")]
    public class OctalTerminal : Semantic
    {
     public OctalTerminal(string text) : base(text) { }
    }
        [Terminal("BinaryLiteral")]
    public class BinaryTerminal : Semantic
    {
            public BinaryTerminal(string text) : base(text) { }
    }
  [Terminal("DecLiteral")]
 public class DecimalTerminal : Semantic
    {
      public DecimalTerminal(string text) : base(text) { }
    }
    [Terminal("HexLiteral")]
    public class HexTerminal : Semantic
    {
        public HexTerminal(string text) : base(text) { }
    }
        [Terminal("RealLiteral")]
    public class FloatTerminal : Semantic
    {
        public FloatTerminal(string text) : base(text) { }
    }
        [Terminal("CharLiteral")]
    public class CharTerminal : Semantic
    {
        public CharTerminal(string text) : base(text) { }
    }
        [Terminal("VerbatimStringLiteral")]
    public class VerbatimStringTerminal : Semantic
    {
        public VerbatimStringTerminal(string text) : base(text) { }
    }
        [Terminal("RegularStringLiteral")]
    public class RegularStringTerminal : Semantic
    {
        public RegularStringTerminal(string text) : base(text) { }
    }
        [Terminal("(Comment)")]
    public class CommentTerminal : Semantic
        {
            public string Comment;
            public CommentTerminal(string com)
            {
                if (com.StartsWith("//"))
                    Comment = com.Remove(0, 2);
                else
                {
                    Comment = com.Remove(0, 2);
                    Comment = Comment.Substring(0, Comment.Length - 2);
                }
            }
         
        }
        [Terminal("Documentation")]
        public class DocumentationTerminal : Semantic
        {
            public string Documentation;
            public DocumentationTerminal(string com)
            {
         
                    Documentation = com.Remove(0, 3);
              
            }

        }
    [Terminal("(EOF)")]
    [Terminal("(Error)")]
    [Terminal("(Whitespace)")]
    [Terminal("(NewLine)")]
    [Terminal("(*/)")]
    [Terminal("(//)")]
    [Terminal("(/*)")]
    [Terminal("-")]
    [Terminal("--")]
    [Terminal("(///)")]
    [Terminal("!")]
    [Terminal("!<")]
    [Terminal("!=")]
    [Terminal("$")]
    [Terminal("$(")]
    [Terminal("%")]
    [Terminal("%=")]
    [Terminal("&")]
    [Terminal("&&")]
    [Terminal("&=")]
    [Terminal("(")]
    [Terminal(")")]
    [Terminal("*")]
    [Terminal("*=")]
    [Terminal(",")]
    [Terminal(".")]
    [Terminal("/")]
    [Terminal("/=")]
    [Terminal(":")]
    [Terminal("::")]
    [Terminal(":=")]
    [Terminal(";")]
    [Terminal("?")]
    [Terminal("?!")]
    [Terminal("?.")]
    [Terminal("??")]
    [Terminal("?[")]
    [Terminal("[")]
    [Terminal("[,")]
    [Terminal("[]")]
    [Terminal("]")]
    [Terminal("^")]
    [Terminal("^=")]
    [Terminal("{")]
    [Terminal("|")]
    [Terminal("||")]
    [Terminal("|=")]
    [Terminal("}")]
    [Terminal("~")]
    [Terminal("~>")]
    [Terminal("~>=")]
    [Terminal("+")]
    [Terminal("++")]
    [Terminal("+=")]
    [Terminal("<")]
    [Terminal("<~")]
    [Terminal("<~=")]
    [Terminal("<<")]
    [Terminal("<<=")]
    [Terminal("<=")]
    [Terminal("=")]
    [Terminal("-=")]
    [Terminal("==")]
    [Terminal("=>")]
    [Terminal(">")]
    [Terminal("->")]
    [Terminal(">=")]
    [Terminal(">>")]
    [Terminal(">>=")]
    [Terminal("abstract")]
    [Terminal("add")]
    [Terminal("as")]
    [Terminal("ASMCodeLiteral")]
    [Terminal("bool")]
    [Terminal("break")]
    [Terminal("byte")]
    [Terminal("case")]
    [Terminal("catch")]
    [Terminal("char")]
    [Terminal("checked")]
    [Terminal("class")]
    [Terminal("const")]
    [Terminal("continue")]
    [Terminal("default")]
    [Terminal("delegate")]
    [Terminal("delete")]
    [Terminal("do")]
    [Terminal("double")]
    [Terminal("else")]
    [Terminal("enum")]
    [Terminal("event")]
    [Terminal("explicit")]
    [Terminal("extern")]
    [Terminal("false")]
    [Terminal("finally")]
    [Terminal("float")]
    [Terminal("for")]
    [Terminal("foreach")]
    [Terminal("get")]
    [Terminal("goto")]
    [Terminal("Identifier")]
    [Terminal("if")]
    [Terminal("implicit")]
    [Terminal("import")]
    [Terminal("in")]
    [Terminal("int")]
    [Terminal("interface")]
    [Terminal("internal")]
    [Terminal("interrupt")]
    [Terminal("is")]
    [Terminal("long")]
    [Terminal("new")]
    [Terminal("null")]
    [Terminal("object")]
    [Terminal("operator")]
    [Terminal("OperatorLiteralBinary")]
    [Terminal("OperatorLiteralUnary")]
    [Terminal("out")]
    [Terminal("override")]
    [Terminal("package")]
    [Terminal("params")]
    [Terminal("partial")]
    [Terminal("private")]
    [Terminal("protected")]
    [Terminal("public")]
    [Terminal("raise")]
    [Terminal("readonly")]
    [Terminal("ref")]
    [Terminal("remove")]
    [Terminal("return")]
    [Terminal("sbyte")]
    [Terminal("sealed")]
    [Terminal("self")]
    [Terminal("set")]
    [Terminal("short")]
    [Terminal("sizeof")]
    [Terminal("stackalloc")]
    [Terminal("static")]
    [Terminal("string")]
    [Terminal("struct")]
    [Terminal("super")]
    [Terminal("supersede")]
    [Terminal("switch")]
    [Terminal("sync")]
    [Terminal("throw")]
    [Terminal("true")]
    [Terminal("try")]
    [Terminal("typeof")]
    [Terminal("uint")]
    [Terminal("ulong")]
    [Terminal("unchecked")]
    [Terminal("union")]
    [Terminal("ushort")]
    [Terminal("using")]
    [Terminal("virtual")]
    [Terminal("void")]
    [Terminal("where")]
    [Terminal("while")]
	public class Semantic : SemanticToken
    {
        string _name = null;
        public static Location TranslateLocation(VSC.Base.GoldParser.Parser.LineInfo li)
        {
            return new Location(new SourceFile(Path.GetFileName(li.SourceFile), li.SourceFile, 0), li.Line, li.Column);
        }
        private Location _loc;
        public Location Location { get { _loc = TranslateLocation(position); return _loc; } }
        public virtual string Name { get { if (_name == null)return symbol.Name; else return _name; } }
        public Semantic() { }
        public Semantic(string text)
        {
            _name = text;
        }
        public virtual void EmitComment(string comment, EmitContext ec)
        {
        }
    }
}
