using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    //
	// A set of parsed constraints for a type parameter
	//
    public class TypeArguments
    {

        public List<ITypeReference> ToTypeReferences(InterningProvider intern)
        {
        List<ITypeReference> l = new List<ITypeReference>();
        foreach (var fe in args)
            l.Add(fe as ITypeReference);
        return l;
        }
        List<FullNamedExpression> args;
      //  TypeSpec[] atypes;

        public TypeArguments(params FullNamedExpression[] types)
        {
            this.args = new List<FullNamedExpression>(types);
        }

        public void Add(FullNamedExpression type)
        {
            args.Add(type);
        }


        public int Count
        {
            get
            {
                return args.Count;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return false;
            }
        }
        public List<FullNamedExpression> Arguments
        {
            get { return args; }
        }
        public string GetSignatureForError()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; ++i)
            {
                var expr = args[i];
                if (expr != null)
                    sb.Append(expr.GetSignatureForError());

                if (i + 1 < Count)
                    sb.Append(',');
            }

            return sb.ToString();
        }
    }
}
