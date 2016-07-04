namespace VSC.AST {

public class SwitchLabel : Statement
	{
/*		Constant converted;
		Expression label;

		Label? il_label;

		//
		// if expr == null, then it is the default case.
		//
		public SwitchLabel (Expression expr, Location l)
		{
			label = expr;
			loc = l;
		}

		public bool IsDefault {
			get {
				return label == null;
			}
		}

		public Expression Label {
			get {
				return label;
			}
		}

		public Location Location {
			get {
				return loc;
			}
		}

		public Constant Converted {
			get {
				return converted;
			}
			set {
				converted = value; 
			}
		}

		public bool PatternMatching { get; set; }

		public bool SectionStart { get; set; }

		public Label GetILLabel (EmitContext ec)
		{
			if (il_label == null){
				il_label = ec.DefineLabel ();
			}

			return il_label.Value;
		}

		protected override void DoEmit (EmitContext ec)
		{
			ec.MarkLabel (GetILLabel (ec));
		}

		protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
		{
			if (!SectionStart)
				return false;

			fc.BranchDefiniteAssignment (fc.SwitchInitialDefinitiveAssignment);
			return false;
		}

		public override bool Resolve (BlockContext bc)
		{
			if (ResolveAndReduce (bc))
				bc.Switch.RegisterLabel (bc, this);

			return true;
		}

		//
		// Resolves the expression, reduces it to a literal if possible
		// and then converts it to the requested type.
		//
		bool ResolveAndReduce (BlockContext bc)
		{
			if (IsDefault)
				return true;

			var switch_statement = bc.Switch;

			if (PatternMatching) {
				label = new Is (switch_statement.ExpressionValue, label, loc).Resolve (bc);
				return label != null;
			}

			var c = label.ResolveLabelConstant (bc);
			if (c == null)
				return false;

			if (switch_statement.IsNullable && c is NullLiteral) {
				converted = c;
				return true;
			}

			if (switch_statement.IsPatternMatching) {
				label = new Is (switch_statement.ExpressionValue, label, loc).Resolve (bc);
				return true;
			}

			converted = c.ImplicitConversionRequired (bc, switch_statement.SwitchType);
			return converted != null;
		}

		public void Error_AlreadyOccurs (ResolveContext ec, SwitchLabel collision_with)
		{
			ec.Report.SymbolRelatedToPreviousError (collision_with.loc, null);
			ec.Report.Error (152, loc, "The label `{0}' already occurs in this switch statement", GetSignatureForError ());
		}

		protected override void CloneTo (CloneContext clonectx, Statement target)
		{
			var t = (SwitchLabel) target;
			if (label != null)
				t.label = label.Clone (clonectx);
		}

		public override object Accept (StructuralVisitor visitor)
		{
			return visitor.Visit (this);
		}

		public string GetSignatureForError ()
		{
			string label;
			if (converted == null)
				label = "default";
			else
				label = converted.GetValueAsLiteral ();

			return string.Format ("case {0}:", label);
		}
*/	}

}