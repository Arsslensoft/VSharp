using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    //
	// A set of parsed constraints for a type parameter
	//
    public class TypeArguments : IList<ITypeReference>
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




        public int IndexOf(ITypeReference item)
        {
            return args.IndexOf((FullNamedExpression)item);
        }

        public void Insert(int index, ITypeReference item)
        {
            args.Insert(index, (FullNamedExpression)item);
        }

        public void RemoveAt(int index)
        {
            args.RemoveAt(index);
        }

        public ITypeReference this[int index]
        {
            get { return (ITypeReference)args[index]; }
            set { args[index] = (FullNamedExpression)value; }
        }

        public void Add(ITypeReference item)
        {
            args.Add((FullNamedExpression)item);
        }

        public void Clear()
        {
            args.Clear();
        }

        public bool Contains(ITypeReference item)
        {
            return args.Contains((FullNamedExpression)item);
        }

        public void CopyTo(ITypeReference[] array, int arrayIndex)
        {
            args.Cast<ITypeReference>().ToList().CopyTo(array, arrayIndex);
        }
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ITypeReference item)
        {
            return args.Remove((FullNamedExpression)item);
        }

        public IEnumerator<ITypeReference> GetEnumerator()
        {
            return args.Cast<ITypeReference>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return args.GetEnumerator();
        }


   
    }
}
