using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSC.TypeSystem;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.AST
{


    public abstract class CompilerGeneratedContainer : ClassOrStructDeclaration
    {
        protected CompilerGeneratedContainer(TypeContainer parent, MemberName name, Modifiers mod, CompilationSourceFile f)
            : this(parent, name, mod, TypeKind.Class,f)
        {
        }
        protected CompilerGeneratedContainer(PackageContainer parent, MemberName name, Modifiers mod, CompilationSourceFile f)
            : this(parent, name, mod, TypeKind.Class, f)
        {
        }
        protected CompilerGeneratedContainer(PackageContainer parent, MemberName name, Modifiers mod, TypeKind kind, CompilationSourceFile f)
            : base(parent, mod, mod, name, null, Location.Null, kind, f)
        {
            ModFlags = mod | Modifiers.COMPILER_GENERATED | Modifiers.SEALED;
            IsSealed = true;
            IsSynthetic = true;


        }
        protected CompilerGeneratedContainer(TypeContainer parent, MemberName name, Modifiers mod, TypeKind kind, CompilationSourceFile f)
            : base(parent, mod,mod,name,null, Location.Null, kind, f)
        {
            ModFlags = mod | Modifiers.COMPILER_GENERATED | Modifiers.SEALED;
            IsSealed = true;
            IsSynthetic = true;


        }
        protected static MemberName MakeMemberName(MemberContainer host, string name, int unique_id, TypeParameters tparams, Location loc)
        {
            string host_name = host == null ? null : host is InterfaceMemberContainer ? ((InterfaceMemberContainer)host).GetFullName(host.MemberName) : host.MemberName.Name;
            string tname = MakeName(host_name, "c", name, unique_id);
            TypeParameters args = null;
            if (tparams != null)
            {
                args = new TypeParameters(tparams.Count);

                // Type parameters will be filled later when we have TypeContainer
                // instance, for now we need only correct arity to create valid name
                for (int i = 0; i < tparams.Count; ++i)
                    args.Add((UnresolvedTypeParameterSpec)null);
            }

            return new MemberName(tname, args, loc);
        }

        public static string MakeName(string host, string typePrefix, string name, int id)
        {
            return "<" + host + ">" + typePrefix + "__" + name + id.ToString("X");
        }

      
    }
    //
    // Anonymous type container
    //
    public class AnonymousTypeClass : CompilerGeneratedContainer
    {
        public const string ClassNamePrefix = "<>__AnonType";
        public const string SignatureForError = "anonymous type";

        readonly IList<AnonymousTypeParameter> parameters;

        private AnonymousTypeClass(CompilationSourceFile f, MemberName name, IList<AnonymousTypeParameter> parameters, Location loc)
            : base(f.RootPackage, name,  Modifiers.PUBLIC, f)
        {
            this.parameters = parameters;
        }

        public static AnonymousTypeClass Create(TypeContainer parent, IList<AnonymousTypeParameter> parameters, Location loc)
        {
            string name = ClassNamePrefix + parent.Module.CounterAnonymousTypes++;

            ParametersCompiled all_parameters;
            TypeParameters tparams = null;
            SimpleName[] t_args;

            if (parameters.Count == 0)
            {
                all_parameters = ParametersCompiled.EmptyReadOnlyParameters;
                t_args = null;
            }
            else
            {
                t_args = new SimpleName[parameters.Count];
                tparams = new TypeParameters();
                Parameter[] ctor_params = new Parameter[parameters.Count];
                for (int i = 0; i < parameters.Count; ++i)
                {
                    AnonymousTypeParameter p = parameters[i];
                    for (int ii = 0; ii < i; ++ii)
                    {
                        if (parameters[ii].Name == p.Name)
                        {
                            parent.Compiler.Report.Error(833, parameters[ii].Location,
                                "`{0}': An anonymous type cannot have multiple properties with the same name",
                                    p.Name);

                            p = new AnonymousTypeParameter(null, "$" + i.ToString(), p.Location);
                            parameters[i] = p;
                            break;
                        }
                    }

                    t_args[i] = new SimpleName("<" + p.Name + ">__T", p.Location);
                    tparams.Add(new UnresolvedTypeParameterSpec(SymbolKind.TypeDefinition, i, p.Location, t_args[i].Name));
                    ctor_params[i] = new Parameter(t_args[i], p.Name, ParameterModifier.None, null, p.Location);
                }

                all_parameters = new ParametersCompiled(ctor_params);
            }

            //
            // Create generic anonymous type host with generic arguments
            // named upon properties names
            //
            AnonymousTypeClass a_type = new AnonymousTypeClass(parent.UnresolvedFile as CompilationSourceFile, new MemberName(name, tparams, loc), parameters, loc);

            ConstructorDeclaration c = new ConstructorDeclaration(a_type, name, Modifiers.PUBLIC | Modifiers.DEBUGGER_HIDDEN,
                null, all_parameters, loc);
            c.Block = new ToplevelBlock(parent.Module.Compiler, c.ParameterInfo, loc);

            // 
            // Create fields and constructor body with field initialization
            //
            bool error = false;
            for (int i = 0; i < parameters.Count; ++i)
            {
                AnonymousTypeParameter p = parameters[i];

                FieldDeclaration f = new FieldDeclaration(a_type, t_args[i], Modifiers.PRIVATE | Modifiers.READONLY | Modifiers.DEBUGGER_HIDDEN,
                    new MemberName("<" + p.Name + ">", p.Location), null);

                c.Block.AddStatement(new StatementExpression(
                    new SimpleAssign(new MemberAccess(new SelfReference(p.Location), f.Name),
                       new SimpleName(all_parameters[i].Name, p.Location))));

                ToplevelBlock get_block = new ToplevelBlock(parent.Module.Compiler, p.Location);
                get_block.AddStatement(new Return(
                    new MemberAccess(new SelfReference(p.Location), f.Name), p.Location));

                PropertyDeclaration prop = new PropertyDeclaration(a_type, t_args[i], Modifiers.PUBLIC,
                    new MemberName(p.Name, p.Location), null);
                prop.Getter = new GetterDeclaration(prop, 0, null, p.Location);
                (prop.Getter as GetterDeclaration).Block = get_block;
                a_type.AddMember(prop);
            }

            a_type.AddMember(c);
            return a_type;
        }

        public override string GetSignatureForError()
        {
            return SignatureForError;
        }

        public CompilationSourceFile GetCompilationSourceFile()
        {
            return UnresolvedFile as CompilationSourceFile;
        }

        public IList<AnonymousTypeParameter> Parameters
        {
            get
            {
                return parameters;
            }
        }
    }
}
