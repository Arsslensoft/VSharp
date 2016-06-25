﻿// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using VSC.TypeSystem.Implementation;
using VSC.TypeSystem.Resolver;

namespace VSC.TypeSystem
{
	/// <summary>
	/// Represents a file that was parsed and converted for the type system.
	/// </summary>
	[Serializable]
	public class VSharpUnresolvedFile : FreezableSpec, IUnresolvedFile
	{
		// The 'FastSerializerVersion' attribute on VSharpUnresolvedFile must be incremented when fixing 
		// bugs in the TypeSystemConvertVisitor
		
		string fileName = string.Empty;
		readonly UsingScope rootUsingScope = new UsingScope();
		IList<IUnresolvedTypeDefinition> topLevelTypeDefinitions = new List<IUnresolvedTypeDefinition>();
		IList<IUnresolvedAttribute> assemblyAttributes = new List<IUnresolvedAttribute>();
		IList<IUnresolvedAttribute> moduleAttributes = new List<IUnresolvedAttribute>();
		IList<UsingScope> usingScopes = new List<UsingScope>();
        IList<ErrorMessage> errors = new List<ErrorMessage>();
		Dictionary<IUnresolvedEntity, string> documentation;
		
		protected override void FreezeInternal()
		{
			base.FreezeInternal();
			rootUsingScope.Freeze();
			topLevelTypeDefinitions = FreezableHelper.FreezeListAndElements(topLevelTypeDefinitions);
			assemblyAttributes = FreezableHelper.FreezeListAndElements(assemblyAttributes);
			moduleAttributes = FreezableHelper.FreezeListAndElements(moduleAttributes);
			usingScopes = FreezableHelper.FreezeListAndElements(usingScopes);
		}
		
		public string FileName {
			get { return fileName; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				fileName = value ?? string.Empty;
			}
		}
		
		DateTime? lastWriteTime;
		
		public DateTime? LastWriteTime {
			get { return lastWriteTime; }
			set {
				FreezableHelper.ThrowIfFrozen(this);
				lastWriteTime = value;
			}
		}
		
		public UsingScope RootUsingScope {
			get { return rootUsingScope; }
		}
		
		public IList<ErrorMessage> Errors {
			get { return errors; }
            internal set { errors = (List<ErrorMessage>)value; }
		}
		
		public IList<UsingScope> UsingScopes {
			get { return usingScopes; }
		}
		
		public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return topLevelTypeDefinitions; }
		}
		
		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get { return assemblyAttributes; }
		}
		
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get { return moduleAttributes; }
		}
		
		public void AddDocumentation(IUnresolvedEntity entity, string xmlDocumentation)
		{
			FreezableHelper.ThrowIfFrozen(this);
			if (documentation == null)
				documentation = new Dictionary<IUnresolvedEntity, string>();
			documentation.Add(entity, xmlDocumentation);
		}
		
		public UsingScope GetUsingScope(Location location)
		{
			foreach (UsingScope scope in usingScopes) {
				if (scope.Region.IsInside(location.Line, location.Column))
					return scope;
			}
			return rootUsingScope;
		}
		
		public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(Location location)
		{
			return FindEntity(topLevelTypeDefinitions, location);
		}
		
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(Location location)
		{
			IUnresolvedTypeDefinition parent = null;
			IUnresolvedTypeDefinition type = GetTopLevelTypeDefinition(location);
			while (type != null) {
				parent = type;
				type = FindEntity(parent.NestedTypes, location);
			}
			return parent;
		}
		
		public IUnresolvedMember GetMember(Location location)
		{
			IUnresolvedTypeDefinition type = GetInnermostTypeDefinition(location);
			if (type == null)
				return null;
			return FindEntity(type.Members, location);
		}
		
		static T FindEntity<T>(IList<T> list, Location location) where T : class, IUnresolvedEntity
		{
			// This could be improved using a binary search
			foreach (T entity in list) {
				if (entity.Region.IsInside(location.Line, location.Column))
					return entity;
			}
			return null;
		}
		
		public VSharpTypeResolveContext GetTypeResolveContext(ICompilation compilation, Location loc)
		{
			var rctx = new VSharpTypeResolveContext (compilation.MainAssembly);
			rctx = rctx.WithUsingScope (GetUsingScope (loc).Resolve (compilation));
			var curDef = GetInnermostTypeDefinition (loc);
			if (curDef != null) {
				var resolvedDef = curDef.Resolve (rctx).GetDefinition ();
				if (resolvedDef == null)
					return rctx;
				rctx = rctx.WithCurrentTypeDefinition (resolvedDef);
				
				var curMember = resolvedDef.Members.FirstOrDefault (m => m.Region.FileName == FileName && m.Region.Begin <= loc && loc < m.BodyRegion.End);
				if (curMember != null)
					rctx = rctx.WithCurrentMember (curMember);
			}
			
			return rctx;
		}
		
		public VSharpResolver GetResolver (ICompilation compilation, Location loc)
		{
			return new VSharpResolver (GetTypeResolveContext (compilation, loc));
		}
		
		public string GetDocumentation(IUnresolvedEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			if (documentation == null)
				return null;
			string xmlDoc;
			if (documentation.TryGetValue(entity, out xmlDoc))
				return xmlDoc;
			else
				return null;
		}
		

	}
}