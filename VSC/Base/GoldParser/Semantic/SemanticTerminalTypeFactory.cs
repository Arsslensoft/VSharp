using System;
using System.Diagnostics;
using System.Reflection;

namespace VSC.Base.GoldParser.Semantic {
	/// <summary>
	/// The factory for terminals of the semantic token type implementation.
	/// </summary>
	/// <typeparam name="TBase">The base type of the semantic token.</typeparam>
	/// <typeparam name="TOutput">The <see cref="SemanticToken"/> descendant instantiated by this factory.</typeparam>
	public class SemanticTerminalTypeFactory<TBase, TOutput>: SemanticTerminalFactory<TBase, TOutput> where TBase: SemanticToken where TOutput: TBase {
		private readonly SemanticTerminalTypeFactoryHelper<TBase>.Activator<TOutput> activator;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemanticTerminalTypeFactory&lt;TBase, TOutput&gt;"/> class. This is mainly for internal use.
		/// </summary>
		public SemanticTerminalTypeFactory() {
			ConstructorInfo constructor = typeof(TOutput).GetConstructor(new[] {typeof(string)});
			if (constructor == null) {
				constructor = typeof(TOutput).GetConstructor(Type.EmptyTypes);
				if (constructor == null) {
					throw new InvalidOperationException("No matching constructor found");
				}
			}
			activator = SemanticTerminalTypeFactoryHelper<TBase>.CreateActivator(this, constructor);
			Debug.Assert(activator != null);
		}

		protected override TOutput Create(string text) {
			return activator(text);
		}
	}
}