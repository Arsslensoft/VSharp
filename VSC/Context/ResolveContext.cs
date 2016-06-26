using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.Context
{

    public class BlockContext : ResolveContext
    {

    }
    public class SymbolResolveContext
    {

        internal UsingScope usingScope;
        VSharpUnresolvedTypeDefinition currentTypeDefinition;
        UnresolvedMethodSpec currentMethod;

        InterningProvider interningProvider = new SimpleInterningProvider();
        internal VSharpUnresolvedFile unresolvedFile;
        public VSharpUnresolvedFile UnresolvedFile
        {
            get { return unresolvedFile; }
        }
        /// <summary>
        /// Gets/Sets the interning provider to use.
        /// The default value is a new <see cref="SimpleInterningProvider"/> instance.
        /// </summary>
        public InterningProvider InterningProvider
        {
            get { return interningProvider; }
            set
            {
                if (interningProvider == null)
                    throw new ArgumentNullException();
                interningProvider = value;
            }
        }


        public SymbolResolveContext(string filename)
        {
            this.unresolvedFile = new VSharpUnresolvedFile();
            this.unresolvedFile.FileName = filename;
            this.usingScope = unresolvedFile.RootUsingScope;
        }


        DomRegion MakeRegion(Location start, Location end)
        {
            return new DomRegion(unresolvedFile.FileName, start.Line, start.Column, end.Line, end.Column);
        }


        #region Types
       //public ITypeReference ConvertTypeReference(Semantic type, NameLookupMode lookupMode = NameLookupMode.Type)
       // {
       //     return type.ToTypeReference(lookupMode, interningProvider);
       // }
        #endregion
        //DomRegion MakeRegion(Semantic node)
        //{
        //    if (node == null || node.IsNull)
        //        return DomRegion.Empty;
        //    else
        //        return MakeRegion(GetStartLocationAfterAttributes(node), node.position);
        //}

        //internal static Location GetStartLocationAfterAttributes(Semantic node)
        //{
        //    AstNode child = node.FirstChild;
        //    // Skip attributes and comments between attributes for the purpose of
        //    // getting a declaration's region.
        //    while (child != null && (child is AttributeSection || child.NodeType == NodeType.Whitespace))
        //        child = child.NextSibling;
        //    return (child ?? node).StartLocation;
        //}

       
    }
   public class ResolveContext
    {
       internal VSharpResolver _resolver;
     
       public VSharpResolver Resolver { get { return _resolver; } }
       public void CreateResolver(ICompilation c)
       {
           _resolver = new VSharpResolver(c);
       }
    

       public ResolveContext()
       {

       }
       public ResolveContext(ICompilation comp)
       {
           _resolver = new VSharpResolver(comp);
       }




    }
}
