using System;
using System.Collections.Generic;

namespace VSC.Base.GoldParser.Grammar {
	/// <summary>
	/// A group in the grammar, such as a comment block or a block of another programming language.
	/// </summary>
	public class Group: GrammarObject<Group>, IGroup {
		private readonly GroupAdvanceMode advanceMode;
		private readonly GrammarObjectSet<DfaState> allowedDfaStates = new GrammarObjectSet<DfaState>();
		private readonly GroupEndingMode endingMode;
		private readonly string name;
		private readonly GrammarObjectSet<Group> nesting;
		private Symbol container;
		private Symbol end;
		private Symbol start;

		internal Group(CompiledGrammar owner, int index, string name, GroupAdvanceMode advanceMode, GroupEndingMode endingMode): base(owner, index) {
			this.name = name;
			this.advanceMode = advanceMode;
			this.endingMode = endingMode;
			nesting = new GrammarObjectSet<Group>();
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Determines which groups may be (directly) nested inside this group.
		/// </summary>
		/// <value>
		/// The nesting groups.
		/// </value>
		public IEnumerable<Group> NestingGroups {
			get {
				return nesting;
			}
		}

		/// <summary>
		/// Gets the start symbol.
		/// </summary>
		/// <value>
		/// The start symbol.
		/// </value>
		public Symbol StartSymbol {
			get {
				return start;
			}
		}

		internal void Initialize(Symbol container, Symbol start, Symbol end, Group[] nesting) {
			if (container == null) {
				throw new ArgumentNullException("container");
			}
			if (start == null) {
				throw new ArgumentNullException("start");
			}
			if (end == null) {
				throw new ArgumentNullException("end");
			}
			if (nesting == null) {
				throw new ArgumentNullException("nesting");
			}
			if (this.container != null) {
				throw new InvalidOperationException("This group has already been initialized");
			}
			GrammarObjectSet<Symbol> filter = new GrammarObjectSet<Symbol>();
			this.container = container;
			this.start = start;
			this.end = end;
			filter.Set(EndSymbol);
			foreach (Group nested in nesting) {
				this.nesting.Set(nested);
				filter.Set(nested.StartSymbol);
			}
			if (advanceMode == GroupAdvanceMode.Character) {
				foreach (DfaState state in Owner.GetDfaStatesOfSymbols(filter.Contains)) {
					allowedDfaStates.Set(state);
				}
			}
		}

		/// <summary>
		/// Gets the advance mode.
		/// </summary>
		/// <value>
		/// The advance mode.
		/// </value>
		public GroupAdvanceMode AdvanceMode {
			get {
				return advanceMode;
			}
		}

		/// <summary>
		/// Gets the container symbol.
		/// </summary>
		/// <value>
		/// The container symbol.
		/// </value>
		public Symbol ContainerSymbol {
			get {
				return container;
			}
		}

		/// <summary>
		/// Gets the end symbol.
		/// </summary>
		/// <value>
		/// The end symbol.
		/// </value>
		public Symbol EndSymbol {
			get {
				return end;
			}
		}

		/// <summary>
		/// Gets the ending mode.
		/// </summary>
		/// <value>
		/// The ending mode.
		/// </value>
		public GroupEndingMode EndingMode {
			get {
				return endingMode;
			}
		}

		/// <summary>
		/// Determines whether the specified DFA state is used within this group's symbols or not.
		/// </summary>
		public bool IsAllowedDfaState(DfaState state) {
			return (advanceMode == GroupAdvanceMode.Token) || allowedDfaStates.Contains(state);
		}

		/// <summary>
		/// Determines whether the given group may be nested inside of this group.
		/// </summary>
		/// <param name="group">The group to test.</param>
		/// <returns>
		///   <c>true</c> if nesting is allowed; otherwise, <c>false</c>.
		/// </returns>
		public bool IsNestingAllowed(Group group) {
			return nesting[group];
		}
	}
}
