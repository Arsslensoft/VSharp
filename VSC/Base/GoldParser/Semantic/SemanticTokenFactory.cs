using System;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Semantic {
	/// <summary>
	/// The abstract base class for all seamntic token factories.
	/// </summary>
	public abstract class SemanticTokenFactory<TBase> where TBase: SemanticToken {
		/// <summary>
		/// Gets a value indicating whether the type created by this factory can vary or not. Typically, all factories but trim factories will return a static output type.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is static output type; otherwise, <c>false</c>.
		/// </value>
		public bool IsStaticOutputType {
			get {
				return RedirectForOutputType == null;
			}
		}

		/// <summary>
		/// Gets the type of the instances created by this factory.
		/// </summary>
		/// <value>The type of the output.</value>
		public abstract Type OutputType {
			get;
		}

		protected internal virtual Symbol RedirectForOutputType {
			get {
				return null;
			}
		}
	}
}