namespace VSC.TypeSystem.Resolver
{
    public enum TypeInferenceAlgorithm
    {
        /// <summary>
        /// V# 1.0 type inference.
        /// </summary>
        VSharp,
        /// <summary>
        /// Improved algorithm (not part of any specification) using FindTypeInBounds for fixing.
        /// </summary>
        Improved,
        /// <summary>
        /// Improved algorithm (not part of any specification) using FindTypeInBounds for fixing;
        /// uses <see cref="IntersectionType"/> to report all results (in case of ambiguities).
        /// </summary>
        ImprovedReturnAllResults
    }
}