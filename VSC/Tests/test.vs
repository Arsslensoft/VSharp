import Std;
package ConsoleApplication1
{
	public delegate void Handler<T,U,V>(T x, U y, V z) 
							where T:IComparable!<T>
							where U:IComparable!<U>
							where V:IEqual!<T>;
	
	public interface TI<T> where T:class
	{
		T val();
		void Hello();
 	}
    public interface TestInterface
	{
		void Hello();
	}
	public enum  V : int
	{
		A,
		B = 1,
		C
	}
	public struct S{
		public int a;
		public int b;
	void Test(){}	
	}
public union U{
		public int a;
		public int b;
		
	}
    class Program<U> : TestInterface, TI!<U> where U:TestInterface
    {
	public int operator ~>(int a,int b);
	public bool operator ==(int a,int b);
	public bool operator is(int a,VG b);
	public bool operator true(X v);


        public long S = 987;
public const long DA = 0x987ADE;
		public const string SA = "Hello World from me"
					@"\SA"
					"VER\n";
		public const char c = '\xFE';
	void TI!<U>.Hello()
	{

	}
 void TestInterface.Hello()
	{

	}
	[StdCall]
	[return: Default]
        static void Main([Push, Encrypt,Sz] string[] args)
        {
            Console.WriteLine("The peek is " + peek);
            Console.WriteLine(pfile);
            Console.Read();
        }

    }
public void Hi(){}

}
