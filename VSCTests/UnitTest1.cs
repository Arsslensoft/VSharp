using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VSC;

namespace VSCTests
{
    [TestClass]
    public class ClassTests
    {
        /*
         E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,17) error VS0178: Type parameter `U' has the `struct' constraint, so it cannot be used as a constraint for `I'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,19) error VS0178: Type parameter `U' has the `struct' constraint, so it cannot be used as a constraint for `J'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,19) error VS0179: `U' and `J' : cannot specify both `class' and `struct' constraint
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,23) error VS0173: Circular constraint dependency involving `P' and `O'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,25) error VS0173: Circular constraint dependency involving `O' and `P'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,27) error VS0181: The type `VSC.AST.A' must have a public parameterless constructor in order to use it as parameter `Q' in the generic type or method `VSC.AST.S`14'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,29) error VS0182: The type `Std.Int16' cannot not be used as a base type for type parameter `R' in type or method `VSC.AST.S`14'.
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,37) error VS0176: Duplicate constraint `Std.Object' for type parameter `V'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,42) error VS0177: Inconsistent accessibility: constraint type `VSC.AST.JK' is less accessible than `VSC.AST.S`14'
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,44) error VS0180: `VSC.AST.SE' A constraint must be an interface,  a type parameter or a non sealed/static class
E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs(17,46) error VS0179: `T' and `Z' : cannot specify both `class' and `struct' constraint
         */ 
        [SourceFile(@"E:\Projects\VSharp\Tests\TPAR_CONSTRAINTS.vs")]
        [TestMethod]
        public void TypeParametersConstraintsCheck()
        {
            List<AbstractMessage> errors = TestManager.RunTests(this);
            Assert.IsTrue(errors.HasCode(173, 17, 25));
            Assert.IsTrue(errors.HasCode(173, 17, 23));
            Assert.IsTrue(errors.HasCode(176, 17, 37));
            Assert.IsTrue(errors.HasCode(177, 17, 42));
            Assert.IsTrue(errors.HasCode(178,17,17));
            Assert.IsTrue(errors.HasCode(178, 17, 19));
            Assert.IsTrue(errors.HasCode(179, 17, 46));
            Assert.IsTrue(errors.HasCode(179, 17, 19));
            Assert.IsTrue(errors.HasCode(180, 17, 44));
            Assert.IsTrue(errors.HasCode(181, 17, 27));
            Assert.IsTrue(errors.HasCode(182, 17, 29));

        }
        /*
         E:\Projects\VSharp\Tests\BASE_CLASS.vs(3,5) error VS0162: Inconsistent accessibility: base interface `X.C' is less accessible than interface `C'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(3,5) error VS0166: Inherited interface `X.C' causes a cycle in the interface hierarchy of `X.C'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(5,26) error VS0006: This name `CA' does not exist in the current context
E:\Projects\VSharp\Tests\BASE_CLASS.vs(5,5) error VS0166: Inherited interface `X.C' causes a cycle in the interface hierarchy of `X.A'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(12,1) error VS0161: Type `X.S' is not an interface
E:\Projects\VSharp\Tests\BASE_CLASS.vs(12,1) error VS0163: `X.S' is a sealed or a static class.
E:\Projects\VSharp\Tests\BASE_CLASS.vs(14,5) error VS0165: Circular base class dependency involving `X.BAA' and `X.AAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(14,5) error VS0162: Inconsistent accessibility: base class `X.FAA' is less accessible than class `X.AAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(19,9) error VS0165: Circular base class dependency involving `X.CAA' and `X.BAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(19,9) error VS0162: Inconsistent accessibility: base class `X.AAA' is less accessible than class `X.BAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(24,9) error VS0165: Circular base class dependency involving `X.DAA' and `X.CAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(24,9) error VS0162: Inconsistent accessibility: base class `X.BAA' is less accessible than class `X.CAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(29,5) error VS0165: Circular base class dependency involving `X.EAA' and `X.DAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(29,5) error VS0162: Inconsistent accessibility: base class `X.CAA' is less accessible than class `X.DAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(33,5) error VS0165: Circular base class dependency involving `X.FAA' and `X.EAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(33,5) error VS0162: Inconsistent accessibility: base class `X.DAA' is less accessible than class `X.EAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(37,5) error VS0165: Circular base class dependency involving `X.AAA' and `X.FAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(37,5) error VS0162: Inconsistent accessibility: base class `X.EAA' is less accessible than class `X.FAA'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(43,1) error VS0163: `X.H' is a sealed or a static class.
E:\Projects\VSharp\Tests\BASE_CLASS.vs(43,1) error VS0162: Inconsistent accessibility: base class `X.H' is less accessible than class `X.SEALED'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(45,1) error VS0160: `FIRST': Base class must be specified as first, `X.V' is not a the first base class
E:\Projects\VSharp\Tests\BASE_CLASS.vs(45,1) error VS0162: Inconsistent accessibility: base class `X.V' is less accessible than class `X.FIRST'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(47,1) error VS0158: Duplicate base class `X.IKAL' for type definition `X.DUP'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(48,1) error VS0159: `MUL': Classes cannot have multiple base classes (`X.FIRST' and `X.V')
E:\Projects\VSharp\Tests\BASE_CLASS.vs(48,1) error VS0162: Inconsistent accessibility: base class `X.FIRST' is less accessible than class `X.MUL'
E:\Projects\VSharp\Tests\BASE_CLASS.vs(49,8) error VS0162: Inconsistent accessibility: base class `X.V' is less accessible than class `X.Y' */
        [SourceFile(@"E:\Projects\VSharp\Tests\BASE_CLASS.vs")]
        [TestMethod]
        public void ClassBaseCheck()
        {
            List<AbstractMessage> errors = TestManager.RunTests(this);

        }

    }
}
