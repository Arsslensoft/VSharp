package X{
abstract class TEST : S{
public int A{get{}} = "A"; // AUTO IMPLEMENTED HAVE INIT ERROR
public abstract int J{private get;} // abstract cannot have private accessor
public abstract int XI{get{}} // no body marked as asbtract
public string jk{get;set{}} // error both impl
public string rjk{get{}set;} // error both impl
int S.L {
get{

}
}
}
interface S
{
	int L {get;} = "H"; // error interface props cannot have init/only auto impl

}
}