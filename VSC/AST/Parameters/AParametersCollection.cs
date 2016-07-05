using VSC.TypeSystem;

namespace VSC.AST
{
    public abstract class AParametersCollection
    {
        protected bool has_arglist;
        protected bool has_params;

        // Null object pattern
        public Parameter[] parameters;

        public CallingConventions CallingConvention
        {
            get
            {
                return has_arglist ?
                    CallingConventions.VarArgs :
                    CallingConventions.Standard;
            }
        }

        public int Count
        {
            get { return parameters.Length; }
        }

        public ITypeReference ExtensionMethodType
        {
            get
            {
                if (Count == 0)
                    return null;

                return FixedParameters[0].IsSelf?
                    FixedParameters[0].Type : null;
            }
        }
        public bool IsEmpty
        {
            get { return parameters.Length == 0; }
        }
        public Parameter[] FixedParameters
        {
            get
            {
                return parameters;
            }
        }
    }
}