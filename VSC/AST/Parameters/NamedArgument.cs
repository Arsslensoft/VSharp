using System;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class NamedArgument : MovableArgument
    {
        /// <summary>
        /// Gets the member to which the parameter belongs.
        /// This field can be null.
        /// </summary>
        public readonly IParameterizedMember Member;

        /// <summary>
        /// Gets the parameter.
        /// This field can be null.
        /// </summary>
        public readonly IParameter Parameter;
		
     public NamedArgument(IParameter parameter, Expression argument, IParameterizedMember member = null)
        :this(parameter.Name,Location.Null ,argument)
		{
		
			this.Member = member;
			this.Parameter = parameter;
	
		}

            public NamedArgument(string parameterName, Expression argument)
         : this(parameterName, Location.Null, argument)
		{
		
	

		}
		



        public readonly string Name;
        public readonly bool CtorArgument = true;

        public NamedArgument(string name, Location loc, Expression expr)
            : this(name, loc, expr, AType.None)
        {
            CtorArgument = false;
        }

        public NamedArgument(string name, Location loc, Expression expr, AType modifier)
            : base(expr, modifier, loc)
        {
            this.Name = name;
  
        }

        public Location Location
        {
            get { return loc; }
        }
    }
}