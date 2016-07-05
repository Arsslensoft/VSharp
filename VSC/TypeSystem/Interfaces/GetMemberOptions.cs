using System;

namespace VSC.TypeSystem
{
    [Flags]
    public enum GetMemberOptions
    {
        /// <summary>
        /// No options specified - this is the default.
        /// Members will be specialized, and inherited members will be included.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Do not specialize the returned members - directly return the definitions.
        /// </summary>
        ReturnMemberDefinitions = 0x01,
        /// <summary>
        /// Do not list inherited members - only list members defined directly on this type.
        /// </summary>
        IgnoreInheritedMembers = 0x02
    }
}