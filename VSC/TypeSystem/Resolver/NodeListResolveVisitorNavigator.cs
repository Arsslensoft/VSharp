// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using VSC.AST;
using VSC.Base;


namespace VSC.TypeSystem.Resolver
{
	/// <summary>
	/// <see cref="IResolveVisitorNavigator"/> implementation that resolves a list of nodes.
	/// We will skip all nodes which are not the target nodes or ancestors of the target nodes.
	/// </summary>
	public class NodeListResolveVisitorNavigator : IResolveVisitorNavigator
	{
        readonly Dictionary<IAstNode, ResolveVisitorNavigationMode> dict = new Dictionary<IAstNode, ResolveVisitorNavigationMode>();
		
		/// <summary>
		/// Creates a new NodeListResolveVisitorNavigator that resolves the specified nodes.
		/// </summary>
        public NodeListResolveVisitorNavigator(params IAstNode[] nodes)
            : this((IEnumerable<IAstNode>)nodes)
		{
		}
		
		/// <summary>
		/// Creates a new NodeListResolveVisitorNavigator that resolves the specified nodes.
		/// </summary>
        public NodeListResolveVisitorNavigator(IEnumerable<IAstNode> nodes, bool scanOnly = false)
		{
			if (nodes == null)
				throw new ArgumentNullException("nodes");
			foreach (var node in nodes) {
				dict[node] = scanOnly ? ResolveVisitorNavigationMode.Scan : ResolveVisitorNavigationMode.Resolve;
                for (var ancestor = node.ParentNode; ancestor != null && !dict.ContainsKey(ancestor); ancestor = ancestor.ParentNode)
                {
					dict.Add(ancestor, ResolveVisitorNavigationMode.Scan);
				}
			}
		}
		
		/// <inheritdoc/>
        public virtual ResolveVisitorNavigationMode Scan(IAstNode node)
		{
			ResolveVisitorNavigationMode mode;
			if (dict.TryGetValue(node, out mode)) {
				return mode;
			} else {
				return ResolveVisitorNavigationMode.Skip;
			}
		}

        public virtual void Resolved(IAstNode node, ResolveResult result)
		{
		}
		
		public virtual void ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType)
		{
		}
	}
}
