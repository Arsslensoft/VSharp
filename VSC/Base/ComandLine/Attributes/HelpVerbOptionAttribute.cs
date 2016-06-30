using System;
using System.Reflection;
using VSC.Base.CommandLine.Extensions;

using VSC.Base.CommandLine.Infrastructure;


namespace VSC.Base.CommandLine
{
    /// <summary>
    /// Indicates the instance method that must be invoked when it becomes necessary show your help screen.
    /// The method signature is an instance method with that accepts and returns a <see cref="System.String"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HelpVerbOptionAttribute : BaseOptionAttribute
    {
        private const string DefaultHelpText = "Display more information on a specific command.";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.HelpVerbOptionAttribute"/> class.
        /// Although it is possible, it is strongly discouraged redefine the long name for this option
        /// not to disorient your users.
        /// </summary>
        public HelpVerbOptionAttribute()
            : this("help")
        {
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.HelpVerbOptionAttribute"/> class
        /// with the specified long name. Use parameter less constructor instead.
        /// </summary>
        /// <param name="longName">Help verb option alternative name.</param>
        /// <remarks>
        /// It's highly not recommended change the way users invoke help. It may create confusion.
        /// </remarks>
        public HelpVerbOptionAttribute(string longName)
            : base(null, longName)
        {
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Help verb command do not support short name by design.
        /// </summary>
        public override char? ShortName
        {
            get { return null; }
            internal set { throw new InvalidOperationException(SR.InvalidOperationException_DoNotUseShortNameForVerbCommands); }
        }

        /// <summary>
        /// Help verb command like ordinary help option cannot be mandatory by design.
        /// </summary>
        public override bool Required
        {
            get { return false; }
            set { throw new InvalidOperationException(SR.InvalidOperationException_DoNotSetRequiredPropertyForVerbCommands); }
        }

        internal static void InvokeMethod(
            object target,
            Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo,
            string verb,
            out string text)
        {
            text = null;
            var method = helpInfo.Left;
            if (!CheckMethodSignature(method))
            {
                throw new MemberAccessException(
                    SR.MemberAccessException_BadSignatureForHelpVerbOptionAttribute.FormatInvariant(method.Name));
            }

            text = (string)method.Invoke(target, new object[] { verb });
        }

        private static bool CheckMethodSignature(MethodInfo value)
        {
            if (value.ReturnType == typeof(string) && value.GetParameters().Length == 1)
            {
                return value.GetParameters()[0].ParameterType == typeof(string);
            }

            return false;
        }
    }
}