  
    package VSC.AST { 
    // class
    public class A{
    public self(int a){}
        
    }
     interface AE{
    }
     interface AA{
    }
    public sealed class SE{}
    public class AEz{}
    public class GA{}
    class JK{}
public struct TEST{}
 public class S<I,J,N,O,P,Q,R,T, U, V, W,X,Y,Z>
            where I:U // error struct
	    where J: class,U
            where N:Q, GA // not convertible
            where O: P // circular
            where P: O // circular
            where Q:class, A, new()
	    where R: Std.Int16
            where T : class
            where U : struct
            where V : Std.Object,Std.Object // error duplicate           
            where X:JK // error accessibility
            where Y:SE // error sealed or static
            where Z:struct,T // both class & struct
        {

        }
    }
