using VSC.Context;
namespace VSC.AST {

public interface INamedBlockVariable
	{
		Block Block { get; }
		bool IsParameter { get; }
		Location Location { get; }
	}

}