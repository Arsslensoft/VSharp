using System;
using System.Collections;
using System.IO;
using System.Reflection;
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
	public class Semantic : SemanticToken, ICloneable
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

        /// <summary>
        /// Clone the object, and returning a reference to a cloned object.
        /// </summary>
        /// <returns>Reference to the new cloned 
        /// object.</returns>
        public object Clone()
        {
            //First we create an instance of this specific type.
            object newObject = Activator.CreateInstance(this.GetType());

            //We get the array of fields for the new type instance.
            FieldInfo[] fields = newObject.GetType().GetFields();

            int i = 0;

            foreach (FieldInfo fi in this.GetType().GetFields())
            {
                //We query if the fiels support the ICloneable interface.
                System.Type ICloneType = fi.FieldType.GetInterface("ICloneable", true);

                if (ICloneType != null)
                {
                    //Getting the ICloneable interface from the object.
                    ICloneable IClone = (ICloneable)fi.GetValue(this);

                    //We use the clone method to set the new value to the field.
                    fields[i].SetValue(newObject, IClone.Clone());
                }
                else
                {
                    // If the field doesn't support the ICloneable 
                    // interface then just set it.
                    fields[i].SetValue(newObject, fi.GetValue(this));
                }

                //Now we check if the object support the 
                //IEnumerable interface, so if it does
                //we need to enumerate all its items and check if 
                //they support the ICloneable interface.
                System.Type IEnumerableType = fi.FieldType.GetInterface
                                ("IEnumerable", true);
                if (IEnumerableType != null)
                {
                    //Get the IEnumerable interface from the field.
                    IEnumerable IEnum = (IEnumerable)fi.GetValue(this);

                    //This version support the IList and the 
                    //IDictionary interfaces to iterate on collections.
                    System.Type IListType = fields[i].FieldType.GetInterface
                                        ("IList", true);
                    System.Type IDicType = fields[i].FieldType.GetInterface
                                        ("IDictionary", true);

                    int j = 0;
                    if (IListType != null)
                    {
                        //Getting the IList interface.
                        IList list = (IList)fields[i].GetValue(newObject);

                        foreach (object obj in IEnum)
                        {
                            //Checking to see if the current item 
                            //support the ICloneable interface.
                            ICloneType = obj.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                //If it does support the ICloneable interface, 
                                //we use it to set the clone of
                                //the object in the list.
                                ICloneable clone = (ICloneable)obj;

                                list[j] = clone.Clone();
                            }

                            //NOTE: If the item in the list is not 
                            //support the ICloneable interface then in the 
                            //cloned list this item will be the same 
                            //item as in the original list
                            //(as long as this type is a reference type).

                            j++;
                        }
                    }
                    else if (IDicType != null)
                    {
                        //Getting the dictionary interface.
                        IDictionary dic = (IDictionary)fields[i].
                                            GetValue(newObject);
                        j = 0;

                        foreach (DictionaryEntry de in IEnum)
                        {
                            //Checking to see if the item 
                            //support the ICloneable interface.
                            ICloneType = de.Value.GetType().
                                GetInterface("ICloneable", true);

                            if (ICloneType != null)
                            {
                                ICloneable clone = (ICloneable)de.Value;

                                dic[de.Key] = clone.Clone();
                            }
                            j++;
                        }
                    }
                }
                i++;
            }
            return newObject;
        }
    }
}
