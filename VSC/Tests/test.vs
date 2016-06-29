		// this method is 
import Std;
// 1
package ConsoleApplication1
{
/* 2 */
	public delegate void Handler<T,U,V>(T x, U y, V z) 
							where T:IComparable!<T>
							where U:IComparable!<U>
							where V:IEqual!<T>;
	// 3
	public interface TI<T> where T:class
	{
	// 4
		T val();
		// 5
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
	//10
	public int operator ~>(int a,int b);
	public bool operator ==(int a,int b);
	public bool operator is(int a,VG b);
	public bool operator true(X v);


        public long S = 987;
public const long DA = 0x987ADE;
//9
		public const string SA = "Hello World from me"
					@"\SA"
					"VER\n";
		public const char c = '\xA';
		//8
	void TI!<U>.Hello()
	{

	}	
	// 6
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
	// 7
public void Hi(){}

}
