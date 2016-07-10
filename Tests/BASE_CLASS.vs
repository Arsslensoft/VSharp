package X{
    class V{}
    interface C : C // circular
    {}
    class A : Std.Object,CA,C{ // error name does not exist

    }
    interface  IKAL{}
    struct S
    {
    }
struct K : S,Std.IDisposable{} // error is not an interface
  
    class AAA : FAA // circular
        {
            
        }

        class BAA : AAA// circular
        {
            
        }

        class CAA : BAA// circular
        {
            
        } 

    class DAA : CAA// circular
        {
            
        } 
    class EAA : DAA// circular
        {
            
        } 
    class FAA : EAA// circular
        {
            
        } 

sealed class H{}
class SEALED : H{} // error sealed class

class FIRST : IKAL, V{} // error base class is not first

class DUP: IKAL,IKAL{} // duplicate
class MUL : V,FIRST{} // multiple
public class Y : V {} // accessibity
public class INHER : Std.Delegate // error special class
{
}
public static class Stat 
{
		int a; // error instance member
		
}
class HYU<T1>
{
  class JU : T1 // error derive from type parameter
	{}

	public override bool Equals(Std.Object obj) // warning does not define gethashcode
		{}

}
struct AAAAH{
ARS k;

}
struct ARS
{
AAAAH  h;
}
}