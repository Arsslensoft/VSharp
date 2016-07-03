namespace VSC.AST {

    /*
        public class Switch : LoopStatement
        {
            // structure used to hold blocks of keys while calculating table switch
            sealed class LabelsRange : IComparable<LabelsRange>
            {
                public readonly long min;
                public long max;
                public readonly List<long> label_values;

                public LabelsRange (long value)
                {
                    min = max = value;
                    label_values = new List<long> ();
                    label_values.Addition (value);
                }

                public LabelsRange (long min, long max, ICollection<long> values)
                {
                    this.min = min;
                    this.max = max;
                    this.label_values = new List<long> (values);
                }

                public long Range {
                    get {
                        return max - min + 1;
                    }
                }

                public bool AddValue (long value)
                {
                    var gap = value - min + 1;
                    // Ensure the range has > 50% occupancy
                    if (gap > 2 * (label_values.Count + 1) || gap <= 0)
                        return false;

                    max = value;
                    label_values.Addition (value);
                    return true;
                }

                public int CompareTo (LabelsRange other)
                {
                    int nLength = label_values.Count;
                    int nLengthOther = other.label_values.Count;
                    if (nLengthOther == nLength)
                        return (int) (other.min - min);

                    return nLength - nLengthOther;
                }
            }

            sealed class DispatchStatement : Statement
            {
                readonly Switch body;

                public DispatchStatement (Switch body)
                {
                    this.body = body;
                }

                protected override void CloneTo (CloneContext clonectx, Statement target)
                {
                    throw new NotImplementedException ();
                }

                protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
                {
                    return false;
                }

                protected override void DoEmit (EmitContext ec)
                {
                    body.EmitDispatch (ec);
                }
            }

            class MissingBreak : Statement
            {
                readonly SwitchLabel label;

                public MissingBreak (SwitchLabel sl)
                {
                    this.label = sl;
                    this.loc = sl.loc;
                }

                public bool FallOut { get; set; }

                protected override void DoEmit (EmitContext ec)
                {
                }

                protected override void CloneTo (CloneContext clonectx, Statement target)
                {
                }

                protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
                {
                    if (FallOut) {
                        fc.Report.Error (8070, loc, "Control cannot fall out of switch statement through final case label `{0}'",
                            label.GetSignatureForError ());
                    } else {
                        fc.Report.Error (163, loc, "Control cannot fall through from one case label `{0}' to another",
                            label.GetSignatureForError ());
                    }
                    return true;
                }
            }

            public Expression Expr;

            //
            // Mapping of all labels to their SwitchLabels
            //
            Dictionary<long, SwitchLabel> labels;
            Dictionary<string, SwitchLabel> string_labels;
            List<SwitchLabel> case_labels;

            List<Tuple<GotoCase, Constant>> goto_cases;
            List<DefiniteAssignmentBitSet> end_reachable_das;

            /// <summary>
            ///   The governing switch type
            /// </summary>
            public TypeSpec SwitchType;

            Expression new_expr;

            SwitchLabel case_null;
            SwitchLabel case_default;

            Label defaultLabel, nullLabel;
            VariableReference value;
            ExpressionStatement string_dictionary;
            FieldExpr switch_cache_field;
            ExplicitBlock block;
            bool end_reachable;

            //
            // Nullable Types support
            //
            Nullable.Unwrap unwrap;

            public Switch (Expression e, ExplicitBlock block, Location l)
                : base (block)
            {
                Expr = e;
                this.block = block;
                loc = l;
            }

            public SwitchLabel ActiveLabel { get; set; }

            public ExplicitBlock Block {
                get {
                    return block;
                }
            }

            public SwitchLabel DefaultLabel {
                get {
                    return case_default;
                }
            }

            public bool IsNullable {
                get {
                    return unwrap != null;
                }
            }

            public bool IsPatternMatching {
                get {
                    return new_expr == null && SwitchType != null;
                }
            }

            public List<SwitchLabel> RegisteredLabels {
                get {
                    return case_labels;
                }
            }

            public VariableReference ExpressionValue {
                get {
                    return value;
                }
            }

            //
            // Determines the governing type for a switch.  The returned
            // expression might be the expression from the switch, or an
            // expression that includes any potential conversions to
            //
            static Expression SwitchGoverningType (ResolveContext rc, Expression expr, bool unwrapExpr)
            {
                switch (expr.Type.BuiltinType) {
                case BuiltinTypeSpec.Type.Byte:
                case BuiltinTypeSpec.Type.SByte:
                case BuiltinTypeSpec.Type.UShort:
                case BuiltinTypeSpec.Type.Short:
                case BuiltinTypeSpec.Type.UInt:
                case BuiltinTypeSpec.Type.Int:
                case BuiltinTypeSpec.Type.ULong:
                case BuiltinTypeSpec.Type.Long:
                case BuiltinTypeSpec.Type.Char:
                case BuiltinTypeSpec.Type.String:
                case BuiltinTypeSpec.Type.Bool:
                    return expr;
                }

                if (expr.Type.IsEnum)
                    return expr;

                //
                // Try to find a *user* defined implicit conversion.
                //
                // If there is no implicit conversion, or if there are multiple
                // conversions, we have to report an error
                //
                Expression converted = null;
                foreach (TypeSpec tt in rc.Module.PredefinedTypes.SwitchUserTypes) {

                    if (!unwrapExpr && tt.IsNullableType && expr.Type.IsNullableType)
                        break;

                    var restr = Convert.UserConversionRestriction.ImplicitOnly |
                        Convert.UserConversionRestriction.ProbingOnly;

                    if (unwrapExpr)
                        restr |= Convert.UserConversionRestriction.NullableSourceOnly;

                    var e = Convert.UserDefinedConversion (rc, expr, tt, restr, Location.Null);
                    if (e == null)
                        continue;

                    //
                    // Ignore over-worked ImplicitUserConversions that do
                    // an implicit conversion in addition to the user conversion.
                    // 
                    var uc = e as UserCast;
                    if (uc == null)
                        continue;

                    if (converted != null){
    //					rc.Report.ExtraInformation (loc, "(Ambiguous implicit user defined conversion in previous ");
                        return null;
                    }

                    converted = e;
                }
                return converted;
            }

            public static TypeSpec[] CreateSwitchUserTypes (ModuleContainer module, TypeSpec nullable)
            {
                var types = module.Compiler.BuiltinTypes;

                // LAMESPEC: For some reason it does not contain bool which looks like csc bug
                TypeSpec[] stypes = new[] {
                    types.SByte,
                    types.Byte,
                    types.Short,
                    types.UShort,
                    types.Int,
                    types.UInt,
                    types.Long,
                    types.ULong,
                    types.Char,
                    types.String
                };

                if (nullable != null) {

                    Array.Resize (ref stypes, stypes.Length + 9);

                    for (int i = 0; i < 9; ++i) {
                        stypes [10 + i] = nullable.MakeGenericType (module, new [] { stypes [i] });
                    }
                }

                return stypes;
            }

            public void RegisterLabel (BlockContext rc, SwitchLabel sl)
            {
                case_labels.Addition (sl);

                if (sl.IsDefault) {
                    if (case_default != null) {
                        sl.Error_AlreadyOccurs (rc, case_default);
                    } else {
                        case_default = sl;
                    }

                    return;
                }

                if (sl.Converted == null)
                    return;

                try {
                    if (string_labels != null) {
                        string string_value = sl.Converted.GetValue () as string;
                        if (string_value == null)
                            case_null = sl;
                        else
                            string_labels.Addition (string_value, sl);
                    } else {
                        if (sl.Converted.IsNull) {
                            case_null = sl;
                        } else {
                            labels.Addition (sl.Converted.GetValueAsLong (), sl);
                        }
                    }
                } catch (ArgumentException) {
                    if (string_labels != null)
                        sl.Error_AlreadyOccurs (rc, string_labels[(string) sl.Converted.GetValue ()]);
                    else
                        sl.Error_AlreadyOccurs (rc, labels[sl.Converted.GetValueAsLong ()]);
                }
            }
		
            //
            // This method emits code for a lookup-based switch statement (non-string)
            // Basically it groups the cases into blocks that are at least half full,
            // and then spits out individual lookup opcodes for each block.
            // It emits the longest blocks first, and short blocks are just
            // handled with direct compares.
            //
            void EmitTableSwitch (EmitContext ec, Expression val)
            {
                if (labels != null && labels.Count > 0) {
                    List<LabelsRange> ranges;
                    if (string_labels != null) {
                        // We have done all hard work for string already
                        // setup single range only
                        ranges = new List<LabelsRange> (1);
                        ranges.Addition (new LabelsRange (0, labels.Count - 1, labels.Keys));
                    } else {
                        var element_keys = new long[labels.Count];
                        labels.Keys.CopyTo (element_keys, 0);
                        Array.Sort (element_keys);

                        //
                        // Build possible ranges of switch labes to reduce number
                        // of comparisons
                        //
                        ranges = new List<LabelsRange> (element_keys.Length);
                        var range = new LabelsRange (element_keys[0]);
                        ranges.Addition (range);
                        for (int i = 1; i < element_keys.Length; ++i) {
                            var l = element_keys[i];
                            if (range.AddValue (l))
                                continue;

                            range = new LabelsRange (l);
                            ranges.Addition (range);
                        }

                        // sort the blocks so we can tackle the largest ones first
                        ranges.Sort ();
                    }

                    Label lbl_default = defaultLabel;
                    TypeSpec compare_type = SwitchType.IsEnum ? EnumSpec.GetUnderlyingType (SwitchType) : SwitchType;

                    for (int range_index = ranges.Count - 1; range_index >= 0; --range_index) {
                        LabelsRange kb = ranges[range_index];
                        lbl_default = (range_index == 0) ? defaultLabel : ec.DefineLabel ();

                        // Optimize small ranges using simple equality check
                        if (kb.Range <= 2) {
                            foreach (var key in kb.label_values) {
                                SwitchLabel sl = labels[key];
                                if (sl == case_default || sl == case_null)
                                    continue;

                                if (sl.Converted.IsZeroInteger) {
                                    val.EmitBranchable (ec, sl.GetILLabel (ec), false);
                                } else {
                                    val.Emit (ec);
                                    sl.Converted.Emit (ec);
                                    ec.Emit (OpCodes.Beq, sl.GetILLabel (ec));
                                }
                            }
                        } else {
                            // TODO: if all the keys in the block are the same and there are
                            //       no gaps/defaults then just use a range-check.
                            if (compare_type.BuiltinType == BuiltinTypeSpec.Type.Long || compare_type.BuiltinType == BuiltinTypeSpec.Type.ULong) {
                                // TODO: optimize constant/I4 cases

                                // check block range (could be > 2^31)
                                val.Emit (ec);
                                ec.EmitLong (kb.min);
                                ec.Emit (OpCodes.Blt, lbl_default);

                                val.Emit (ec);
                                ec.EmitLong (kb.max);
                                ec.Emit (OpCodes.Bgt, lbl_default);

                                // normalize range
                                val.Emit (ec);
                                if (kb.min != 0) {
                                    ec.EmitLong (kb.min);
                                    ec.Emit (OpCodes.Sub);
                                }

                                ec.Emit (OpCodes.Conv_I4);	// assumes < 2^31 labels!
                            } else {
                                // normalize range
                                val.Emit (ec);
                                int first = (int) kb.min;
                                if (first > 0) {
                                    ec.EmitInt (first);
                                    ec.Emit (OpCodes.Sub);
                                } else if (first < 0) {
                                    ec.EmitInt (-first);
                                    ec.Emit (OpCodes.Addition);
                                }
                            }

                            // first, build the list of labels for the switch
                            int iKey = 0;
                            long cJumps = kb.Range;
                            Label[] switch_labels = new Label[cJumps];
                            for (int iJump = 0; iJump < cJumps; iJump++) {
                                var key = kb.label_values[iKey];
                                if (key == kb.min + iJump) {
                                    switch_labels[iJump] = labels[key].GetILLabel (ec);
                                    iKey++;
                                } else {
                                    switch_labels[iJump] = lbl_default;
                                }
                            }

                            // emit the switch opcode
                            ec.Emit (OpCodes.Switch, switch_labels);
                        }

                        // mark the default for this block
                        if (range_index != 0)
                            ec.MarkLabel (lbl_default);
                    }

                    // the last default just goes to the end
                    if (ranges.Count > 0)
                        ec.Emit (OpCodes.Br, lbl_default);
                }
            }
		
            public SwitchLabel FindLabel (Constant value)
            {
                SwitchLabel sl = null;

                if (string_labels != null) {
                    string s = value.GetValue () as string;
                    if (s == null) {
                        if (case_null != null)
                            sl = case_null;
                        else if (case_default != null)
                            sl = case_default;
                    } else {
                        string_labels.TryGetValue (s, out sl);
                    }
                } else {
                    if (value is NullLiteral) {
                        sl = case_null;
                    } else {
                        labels.TryGetValue (value.GetValueAsLong (), out sl);
                    }
                }

                if (sl == null || sl.SectionStart)
                    return sl;

                //
                // Always return section start, it simplifies handling of switch labels
                //
                for (int idx = case_labels.IndexOf (sl); ; --idx) {
                    var cs = case_labels [idx];
                    if (cs.SectionStart)
                        return cs;
                }
            }

            protected override bool DoFlowAnalysis (FlowAnalysisContext fc)
            {
                Expr.FlowAnalysis (fc);

                var prev_switch = fc.SwitchInitialDefinitiveAssignment;
                var InitialDefinitiveAssignment = fc.DefiniteAssignment;
                fc.SwitchInitialDefinitiveAssignment = InitialDefinitiveAssignment;

                block.FlowAnalysis (fc);

                fc.SwitchInitialDefinitiveAssignment = prev_switch;

                if (end_reachable_das != null) {
                    var sections_das = DefiniteAssignmentBitSet.And (end_reachable_das);
                    InitialDefinitiveAssignment |= sections_das;
                    end_reachable_das = null;
                }

                fc.DefiniteAssignment = InitialDefinitiveAssignment;

                return case_default != null && !end_reachable;
            }

            public override bool Resolve (BlockContext ec)
            {
                Expr = Expr.Resolve (ec);
                if (Expr == null)
                    return false;

                //
                // LAMESPEC: User conversion from non-nullable governing type has a priority
                //
                new_expr = SwitchGoverningType (ec, Expr, false);

                if (new_expr == null) {
                    if (Expr.Type.IsNullableType) {
                        unwrap = Nullable.Unwrap.Create (Expr, false);
                        if (unwrap == null)
                            return false;

                        //
                        // Unwrap + user conversion using non-nullable type is not allowed but user operator
                        // involving nullable Expr and nullable governing type is
                        //
                        new_expr = SwitchGoverningType (ec, unwrap, true);
                    }
                }

                Expression switch_expr;
                if (new_expr == null) {
                    if (ec.Module.Compiler.Settings.Version != LanguageVersion.Experimental) {
                        if (Expr.Type != InternalType.ErrorType) {
                            ec.Report.Error (151, loc,
                                "A switch expression of type `{0}' cannot be converted to an integral type, bool, char, string, enum or nullable type",
                                Expr.Type.GetSignatureForError ());
                        }

                        return false;
                    }

                    switch_expr = Expr;
                    SwitchType = Expr.Type;
                } else {
                    switch_expr = new_expr;
                    SwitchType = new_expr.Type;
                    if (SwitchType.IsNullableType) {
                        new_expr = unwrap = Nullable.Unwrap.Create (new_expr, true);
                        SwitchType = Nullable.NullableInfo.GetUnderlyingType (SwitchType);
                    }

                    if (SwitchType.BuiltinType == BuiltinTypeSpec.Type.Bool && ec.Module.Compiler.Settings.Version == LanguageVersion.ISO_1) {
                        ec.Report.FeatureIsNotAvailable (ec.Module.Compiler, loc, "switch expression of boolean type");
                        return false;
                    }

                    if (block.Statements.Count == 0)
                        return true;

                    if (SwitchType.BuiltinType == BuiltinTypeSpec.Type.String) {
                        string_labels = new Dictionary<string, SwitchLabel> ();
                    } else {
                        labels = new Dictionary<long, SwitchLabel> ();
                    }
                }

                var constant = switch_expr as Constant;

                //
                // Don't need extra variable for constant switch or switch with
                // only default case
                //
                if (constant == null) {
                    //
                    // Store switch expression for comparison purposes
                    //
                    value = switch_expr as VariableReference;
                    if (value == null && !HasOnlyDefaultSection ()) {
                        var current_block = ec.CurrentBlock;
                        ec.CurrentBlock = Block;
                        // Create temporary variable inside switch scope
                        value = TemporaryVariableReference.Create (SwitchType, ec.CurrentBlock, loc);
                        value.Resolve (ec);
                        ec.CurrentBlock = current_block;
                    }
                }

                case_labels = new List<SwitchLabel> ();

                Switch old_switch = ec.Switch;
                ec.Switch = this;
                var parent_los = ec.EnclosingLoopOrSwitch;
                ec.EnclosingLoopOrSwitch = this;

                var ok = Statement.Resolve (ec);

                ec.EnclosingLoopOrSwitch = parent_los;
                ec.Switch = old_switch;

                //
                // Check if all goto cases are valid. Needs to be done after switch
                // is resolved because goto can jump forward in the scope.
                //
                if (goto_cases != null) {
                    foreach (var gc in goto_cases) {
                        if (gc.Item1 == null) {
                            if (DefaultLabel == null) {
                                Goto.Error_UnknownLabel (ec, "default", loc);
                            }

                            continue;
                        }

                        var sl = FindLabel (gc.Item2);
                        if (sl == null) {
                            Goto.Error_UnknownLabel (ec, "case " + gc.Item2.GetValueAsLiteral (), loc);
                        } else {
                            gc.Item1.Label = sl;
                        }
                    }
                }

                if (!ok)
                    return false;

                if (constant == null && SwitchType.BuiltinType == BuiltinTypeSpec.Type.String && string_labels.Count > 6) {
                    ResolveStringSwitchMap (ec);
                }

                //
                // Anonymous storey initialization has to happen before
                // any generated switch dispatch
                //
                block.InsertStatement (0, new DispatchStatement (this));

                return true;
            }

            bool HasOnlyDefaultSection ()
            {
                for (int i = 0; i < block.Statements.Count; ++i) {
                    var s = block.Statements[i] as SwitchLabel;

                    if (s == null || s.IsDefault)
                        continue;

                    return false;
                }

                return true;
            }

            public override Reachability MarkReachable (Reachability rc)
            {
                if (rc.IsUnreachable)
                    return rc;

                base.MarkReachable (rc);

                block.MarkReachableScope (rc);

                if (block.Statements.Count == 0)
                    return rc;

                SwitchLabel constant_label = null;
                var constant = new_expr as Constant;

                if (constant != null) {
                    constant_label = FindLabel (constant) ?? case_default;
                    if (constant_label == null) {
                        block.Statements.RemoveAt (0);
                        return rc;
                    }
                }

                var section_rc = new Reachability ();
                SwitchLabel prev_label = null;

                for (int i = 0; i < block.Statements.Count; ++i) {
                    var s = block.Statements[i];
                    var sl = s as SwitchLabel;

                    if (sl != null && sl.SectionStart) {
                        //
                        // Section is marked already via goto case
                        //
                        if (!sl.IsUnreachable) {
                            section_rc = new Reachability ();
                            continue;
                        }

                        if (constant_label != null && constant_label != sl)
                            section_rc = Reachability.CreateUnreachable ();
                        else if (section_rc.IsUnreachable) {
                            section_rc = new Reachability ();
                        } else {
                            if (prev_label != null) {
                                sl.SectionStart = false;
                                s = new MissingBreak (prev_label);
                                s.MarkReachable (rc);
                                block.Statements.Insert (i - 1, s);
                                ++i;
                            }
                        }

                        prev_label = sl;
                    }

                    section_rc = s.MarkReachable (section_rc);
                }

                if (!section_rc.IsUnreachable && prev_label != null) {
                    prev_label.SectionStart = false;
                    var s = new MissingBreak (prev_label) {
                        FallOut = true
                    };

                    s.MarkReachable (rc);
                    block.Statements.Addition (s);
                }

                //
                // Reachability can affect parent only when all possible paths are handled but
                // we still need to run reachability check on switch body to check for fall-through
                //
                if (case_default == null && constant_label == null)
                    return rc;

                //
                // We have at least one local exit from the switch
                //
                if (end_reachable)
                    return rc;

                return Reachability.CreateUnreachable ();
            }

            public void RegisterGotoCase (GotoCase gotoCase, Constant value)
            {
                if (goto_cases == null)
                    goto_cases = new List<Tuple<GotoCase, Constant>> ();

                goto_cases.Addition (Tuple.Create (gotoCase, value));
            }

            //
            // Converts string switch into string hashtable
            //
            void ResolveStringSwitchMap (ResolveContext ec)
            {
                FullNamedExpression string_dictionary_type;
                if (ec.Module.PredefinedTypes.Dictionary.Define ()) {
                    string_dictionary_type = new TypeExpression (
                        ec.Module.PredefinedTypes.Dictionary.TypeSpec.MakeGenericType (ec,
                            new [] { ec.BuiltinTypes.String, ec.BuiltinTypes.Int }),
                        loc);
                } else if (ec.Module.PredefinedTypes.Hashtable.Define ()) {
                    string_dictionary_type = new TypeExpression (ec.Module.PredefinedTypes.Hashtable.TypeSpec, loc);
                } else {
                    ec.Module.PredefinedTypes.Dictionary.Resolve ();
                    return;
                }

                var ctype = ec.CurrentMemberDefinition.Parent.PartialContainer;
                Field field = new Field (ctype, string_dictionary_type,
                    Modifiers.STATIC | Modifiers.PRIVATE | Modifiers.COMPILER_GENERATED,
                    new MemberName (CompilerGeneratedContainer.MakeName (null, "f", "switch$map", ec.Module.CounterSwitchTypes++), loc), null);
                if (!field.Define ())
                    return;
                ctype.AddField (field);

                var init = new List<Expression> ();
                int counter = -1;
                labels = new Dictionary<long, SwitchLabel> (string_labels.Count);
                string value = null;

                foreach (SwitchLabel sl in case_labels) {

                    if (sl.SectionStart)
                        labels.Addition (++counter, sl);

                    if (sl == case_default || sl == case_null)
                        continue;

                    value = (string) sl.Converted.GetValue ();
                    var init_args = new List<Expression> (2);
                    init_args.Addition (new StringLiteral (ec.BuiltinTypes, value, sl.Location));

                    sl.Converted = new IntConstant (ec.BuiltinTypes, counter, loc);
                    init_args.Addition (sl.Converted);

                    init.Addition (new CollectionElementInitializer (init_args, loc));
                }
	
                Arguments args = new Arguments (1);
                args.Addition (new Argument (new IntConstant (ec.BuiltinTypes, init.Count, loc)));
                Expression initializer = new NewInitialize (string_dictionary_type, args,
                    new CollectionOrObjectInitializers (init, loc), loc);

                switch_cache_field = new FieldExpr (field, loc);
                string_dictionary = new SimpleAssign (switch_cache_field, initializer.Resolve (ec));
            }

            void DoEmitStringSwitch (EmitContext ec)
            {
                Label l_initialized = ec.DefineLabel ();

                //
                // Skip initialization when value is null
                //
                value.EmitBranchable (ec, nullLabel, false);

                //
                // Check if string dictionary is initialized and initialize
                //
                switch_cache_field.EmitBranchable (ec, l_initialized, true);
                using (ec.With (BuilderContext.Options.OmitDebugInfo, true)) {
                    string_dictionary.EmitStatement (ec);
                }
                ec.MarkLabel (l_initialized);

                LocalTemporary string_switch_variable = new LocalTemporary (ec.BuiltinTypes.Int);

                ResolveContext rc = new ResolveContext (ec.MemberContext);

                if (switch_cache_field.Type.IsGeneric) {
                    Arguments get_value_args = new Arguments (2);
                    get_value_args.Addition (new Argument (value));
                    get_value_args.Addition (new Argument (string_switch_variable, Argument.AType.Out));
                    Expression get_item = new Invocation (new MemberAccess (switch_cache_field, "TryGetValue", loc), get_value_args).Resolve (rc);
                    if (get_item == null)
                        return;

                    //
                    // A value was not found, go to default case
                    //
                    get_item.EmitBranchable (ec, defaultLabel, false);
                } else {
                    Arguments get_value_args = new Arguments (1);
                    get_value_args.Addition (new Argument (value));

                    Expression get_item = new ElementAccess (switch_cache_field, get_value_args, loc).Resolve (rc);
                    if (get_item == null)
                        return;

                    LocalTemporary get_item_object = new LocalTemporary (ec.BuiltinTypes.Object);
                    get_item_object.EmitAssign (ec, get_item, true, false);
                    ec.Emit (OpCodes.Brfalse, defaultLabel);

                    ExpressionStatement get_item_int = (ExpressionStatement) new SimpleAssign (string_switch_variable,
                        new Cast (new TypeExpression (ec.BuiltinTypes.Int, loc), get_item_object, loc)).Resolve (rc);

                    get_item_int.EmitStatement (ec);
                    get_item_object.Release (ec);
                }

                EmitTableSwitch (ec, string_switch_variable);
                string_switch_variable.Release (ec);
            }

            //
            // Emits switch using simple if/else comparison for small label count (4 + optional default)
            //
            void EmitShortSwitch (EmitContext ec)
            {
                MethodSpec equal_method = null;
                if (SwitchType.BuiltinType == BuiltinTypeSpec.Type.String) {
                    equal_method = ec.Module.PredefinedMembers.StringEqual.Resolve (loc);
                }

                if (equal_method != null) {
                    value.EmitBranchable (ec, nullLabel, false);
                }

                for (int i = 0; i < case_labels.Count; ++i) {
                    var label = case_labels [i];
                    if (label == case_default || label == case_null)
                        continue;

                    var constant = label.Converted;

                    if (constant == null) {
                        label.Label.EmitBranchable (ec, label.GetILLabel (ec), true);
                        continue;
                    }

                    if (equal_method != null) {
                        value.Emit (ec);
                        constant.Emit (ec);

                        var call = new CallEmitter ();
                        call.EmitPredefined (ec, equal_method, new Arguments (0));
                        ec.Emit (OpCodes.Brtrue, label.GetILLabel (ec));
                        continue;
                    }

                    if (constant.IsZeroInteger && constant.Type.BuiltinType != BuiltinTypeSpec.Type.Long && constant.Type.BuiltinType != BuiltinTypeSpec.Type.ULong) {
                        value.EmitBranchable (ec, label.GetILLabel (ec), false);
                        continue;
                    }

                    value.Emit (ec);
                    constant.Emit (ec);
                    ec.Emit (OpCodes.Beq, label.GetILLabel (ec));
                }

                ec.Emit (OpCodes.Br, defaultLabel);
            }

            void EmitDispatch (EmitContext ec)
            {
                if (IsPatternMatching) {
                    EmitShortSwitch (ec);
                    return;
                }

                if (value == null) {
                    //
                    // Constant switch, we've already done the work if there is only 1 label
                    // referenced
                    //
                    int reachable = 0;
                    foreach (var sl in case_labels) {
                        if (sl.IsUnreachable)
                            continue;

                        if (reachable++ > 0) {
                            var constant = (Constant) new_expr;
                            var constant_label = FindLabel (constant) ?? case_default;

                            ec.Emit (OpCodes.Br, constant_label.GetILLabel (ec));
                            break;
                        }
                    }

                    return;
                }

                if (string_dictionary != null) {
                    DoEmitStringSwitch (ec);
                } else if (case_labels.Count < 4 || string_labels != null) {
                    EmitShortSwitch (ec);
                } else {
                    EmitTableSwitch (ec, value);
                }
            }

            protected override void DoEmit (EmitContext ec)
            {
                //
                // Setup the codegen context
                //
                Label old_end = ec.LoopEnd;
                Switch old_switch = ec.Switch;

                ec.LoopEnd = ec.DefineLabel ();
                ec.Switch = this;

                defaultLabel = case_default == null ? ec.LoopEnd : case_default.GetILLabel (ec);
                nullLabel = case_null == null ? defaultLabel : case_null.GetILLabel (ec);

                if (value != null) {
                    ec.Mark (loc);

                    var switch_expr = new_expr ?? Expr;
                    if (IsNullable) {
                        unwrap.EmitCheck (ec);
                        ec.Emit (OpCodes.Brfalse, nullLabel);
                        value.EmitAssign (ec, switch_expr, false, false);
                    } else if (switch_expr != value) {
                        value.EmitAssign (ec, switch_expr, false, false);
                    }


                    //
                    // Next statement is compiler generated we don't need extra
                    // nop when we can use the statement for sequence point
                    //
                    ec.Mark (block.StartLocation);
                    block.IsCompilerGenerated = true;
                } else {
                    new_expr.EmitSideEffect (ec);
                }

                block.Emit (ec);

                // Restore context state. 
                ec.MarkLabel (ec.LoopEnd);

                //
                // Restore the previous context
                //
                ec.LoopEnd = old_end;
                ec.Switch = old_switch;
            }

            protected override void CloneTo (CloneContext clonectx, Statement t)
            {
                Switch target = (Switch) t;

                target.Expr = Expr.Clone (clonectx);
                target.Statement = target.block = (ExplicitBlock) block.Clone (clonectx);
            }
		
            public override object Accept (StructuralVisitor visitor)
            {
                return visitor.Visit (this);
            }

            public override void AddEndDefiniteAssignment (FlowAnalysisContext fc)
            {
                if (case_default == null && !(new_expr is Constant))
                    return;

                if (end_reachable_das == null)
                    end_reachable_das = new List<DefiniteAssignmentBitSet> ();

                end_reachable_das.Addition (fc.DefiniteAssignment);
            }

            public override void SetEndReachable ()
            {
                end_reachable = true;
            }
        }
    */
	
   
   
}