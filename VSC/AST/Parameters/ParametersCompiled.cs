using System;
using VSC.Context;
using VSC.TypeSystem;

namespace VSC.AST
{
    public class ParametersCompiled : AParametersCollection
    {
        public static readonly ParametersCompiled EmptyReadOnlyParameters = new ParametersCompiled();
        public static readonly ParametersCompiled Undefined = new ParametersCompiled();
        public ParametersCompiled Clone()
        {
            ParametersCompiled p = (ParametersCompiled)MemberwiseClone();

            p.parameters = new Parameter[parameters.Length];
            for (int i = 0; i < Count; ++i)
                p.parameters[i] = this[i].Clone();

            return p;
        }
        private ParametersCompiled()
        {
            parameters = new Parameter[0];
        }
        public void CheckParameters()
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                var name = parameters[i].Name;
                for (int ii = i + 1; ii < parameters.Length; ++ii)
                {
                    if (parameters[ii].Name == name)
                        this[ii].Error_DuplicateName(CompilerContext.report);
                }
            }
        }
        public ParametersCompiled(params Parameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ArgumentException("Use EmptyReadOnlyParameters");

            this.parameters = parameters;
            int count = parameters.Length;

            for (int i = 0; i < count; i++)
                has_params |= parameters[i].IsSelf;
            
        }

        public ParametersCompiled(Parameter[] parameters, bool has_arglist) :
            this(parameters)
        {
            this.has_arglist = has_arglist;
        }

        public Parameter this[int pos]
        {
            get { return (Parameter)parameters[pos]; }
        }


        public static ParametersCompiled CreateImplicitParameter(FullNamedExpression texpr, Location loc)
        {
            return new ParametersCompiled(
                new[] { new Parameter(texpr, "value", ParameterModifier.None, null, loc) });
        }
        public static ParametersCompiled MergeGenerated(CompilerContext ctx, ParametersCompiled userParams, bool checkConflicts, Parameter compilerParams)
        {
            return MergeGenerated(ctx, userParams, checkConflicts,
                new Parameter[] { compilerParams });
        }

        //
        // Use this method when you merge compiler generated parameters with user parameters
        //
        public static ParametersCompiled MergeGenerated(CompilerContext ctx, ParametersCompiled userParams, bool checkConflicts, Parameter[] compilerParams)
        {
            Parameter[] all_params = new Parameter[userParams.Count + compilerParams.Length];
            userParams.FixedParameters.CopyTo(all_params, 0);

        
            int last_filled = userParams.Count;
            int index = 0;
            foreach (Parameter p in compilerParams)
            {
                for (int i = 0; i < last_filled; ++i)
                {
                    while (p.Name == all_params[i].Name)
                    {
                        if (checkConflicts && i < userParams.Count)
                        {
                            ctx.Report.Error(10, userParams[i].Location,
                                "The parameter name `{0}' conflicts with a compiler generated name", p.Name);
                        }
                        p.Name = '_' + p.Name;
                    }
                }
                all_params[last_filled] = p;
              
                ++last_filled;
            }

            ParametersCompiled parameters = new ParametersCompiled(all_params);
            parameters.has_params = userParams.has_params;
            return parameters;
        }

    }
}