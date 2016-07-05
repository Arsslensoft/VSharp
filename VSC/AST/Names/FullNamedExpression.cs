using System.Threading.Tasks;
using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class FullNamedExpression : Expression
    {
        public virtual ITypeReference ToTypeReference(InterningProvider intern)
        {
            return this as ITypeReference;
        }
    }

    // <summary>
	//   This class is used to "construct" the type during a typecast
	//   operation.
	// </summary>
    //
    // Holds additional type specifiers like ?, *, []
    //


    //
    // Better name would be DottenName
    //
}
