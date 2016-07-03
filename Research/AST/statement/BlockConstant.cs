namespace VSC.AST {
public class BlockConstant : BlockVariable
	{
		/*public BlockConstant (FullNamedExpression type, LocalVariable li)
			: base (type, li)
		{
		}

		public override void Emit (EmitContext ec)
		{
			// Nothing to emit, not even sequence point
		}

		protected override Expression ResolveInitializer (BlockContext bc, LocalVariable li, Expression initializer)
		{
			initializer = initializer.Resolve (bc);
			if (initializer == null)
				return null;

			var c = initializer as Constant;
			if (c == null) {
				initializer.Error_ExpressionMustBeConstant (bc, initializer.Location, li.Name);
				return null;
			}

			c = c.ConvertImplicitly (li.Type);
			if (c == null) {
				if (TypeSpec.IsReferenceType (li.Type))
					initializer.Error_ConstantCanBeInitializedWithNullOnly (bc, li.Type, initializer.Location, li.Name);
				else
					initializer.Error_ValueCannotBeConverted (bc, li.Type, false);

				return null;
			}

			li.ConstantValue = c;
			return initializer;
		}
		
		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}*/
	}


}