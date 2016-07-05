namespace VSC.AST
{
    /// <summary>
    ///   This is just a base class for expressions that can
    ///   appear on statements (invocations, object creation,
    ///   assignments, post/pre increment and decrement).  The idea
    ///   being that they would support an extra Emition interface that
    ///   does not leave a result on the stack.
    /// </summary>
    public abstract class ExpressionStatement : Expression
    {

    }
}