namespace VSC.TypeSystem
{
    public interface IHasAccessibility
    {
        /// <summary>
        /// Gets the accessibility of this entity.
        /// </summary>
        Accessibility Accessibility { get; }
		
        /// <summary>
        /// Gets a value indicating whether this instance is private.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is private; otherwise, <c>false</c>.
        /// </value>
        bool IsPrivate { get; }
		
        /// <summary>
        /// Gets a value indicating whether this instance is public.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is public; otherwise, <c>false</c>.
        /// </value>
        bool IsPublic { get; }
		
        /// <summary>
        /// Gets a value indicating whether this instance is protected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is protected; otherwise, <c>false</c>.
        /// </value>
        bool IsProtected { get; }
		
        /// <summary>
        /// Gets a value indicating whether this instance is internal.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is internal; otherwise, <c>false</c>.
        /// </value>
        bool IsInternal { get; }
		
        /// <summary>
        /// Gets a value indicating whether this instance is protected or internal.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is protected or internal; otherwise, <c>false</c>.
        /// </value>
        bool IsProtectedOrInternal { get; }
		
    
    }
}