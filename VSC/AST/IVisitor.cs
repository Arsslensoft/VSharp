namespace VSC.AST
{
    public interface IVisitor
    {
        void Visit(CompilationSourceFile node);
        void Visit(TypeContainer node);
        void Visit(MemberContainer node);
        void Visit(PackageContainer node);
        void Visit(ImportPackage node);
        void Visit(ImportPackageAlias node);


        void Visit(Expression node);
        void Visit(MemberAccess node);
        void Visit(QualifiedAlias node);
        void Visit(SimpleName node);
        void Visit(ComposedType node);
        void Visit(TypeExpression node);


    }
}