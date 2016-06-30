using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

using VSC.Base.GoldParser.Grammar;

namespace VSC.Base.GoldParser.Semantic {
	public class SemanticNonterminalTypeFactory<TBase, TOutput>: SemanticNonterminalFactory<TBase, TOutput> where TBase: SemanticToken where TOutput: TBase {
		private readonly SemanticNonterminalTypeFactoryHelper<TBase>.Activator<TOutput> activator;
		private readonly ReadOnlyCollection<Type> inputTypes;

		public SemanticNonterminalTypeFactory(MethodBase methodBase, int[] parameterMapping, int handleCount, Type baseTokenType) {
			if (methodBase == null) {
				throw new ArgumentNullException("methodBase");
			}
			if (parameterMapping == null) {
				throw new ArgumentNullException("parameterMapping");
			}
			if (baseTokenType == null) {
				throw new ArgumentNullException("baseTokenType");
			}
			ParameterInfo[] parameters = methodBase.GetParameters();
			if (parameterMapping.Length != parameters.Length) {
				throw new ArgumentException("The parameter mapping must have exactly as many items as the "+methodBase.MemberType+" has parameters", "parameterMapping");
			}
			int requiredHandles = 0;
			foreach (int i in parameterMapping) {
				if (i >= 0) {
					requiredHandles++;
				}
			}
			if (handleCount < requiredHandles) {
				throw new ArgumentOutOfRangeException("handleCount");
			}
			Type[] inputTypeBuilder = new Type[handleCount];
			for (int i = 0; i < handleCount; i++) {
				inputTypeBuilder[i] = baseTokenType;
			}
			foreach (ParameterInfo parameter in parameters) {
				int tokenIndex = parameterMapping[parameter.Position];
				if (tokenIndex != -1) {
					inputTypeBuilder[tokenIndex] = parameter.ParameterType;
				}
			}
			inputTypes = Array.AsReadOnly(inputTypeBuilder);
			activator = SemanticNonterminalTypeFactoryHelper<TBase>.CreateActivator(this, methodBase, parameterMapping);
			Debug.Assert(activator != null);
		}

		public override ReadOnlyCollection<Type> InputTypes {
			get {
				return inputTypes;
			}
		}

		public override TOutput Create(Rule rule, IList<TBase> tokens) {
			Debug.Assert((tokens != null) && (tokens.Count == inputTypes.Count));
			return activator(tokens);
		}
	}
}
