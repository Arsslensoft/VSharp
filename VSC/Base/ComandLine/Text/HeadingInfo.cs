using System;
using System.IO;
using System.Reflection;
using System.Text;

using VSC.Base.CommandLine.Infrastructure;



namespace VSC.Base.CommandLine.Text
{
    /// <summary>
    /// Models the heading part of an help text.
    /// You can assign it where you assign any <see cref="System.String"/> instance.
    /// </summary>
    public class HeadingInfo
    {
        private readonly string _programName;
        private readonly string _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.Text.HeadingInfo"/> class
        /// specifying program name.
        /// </summary>
        /// <param name="programName">The name of the program.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="programName"/> is null or empty string.</exception>
        public HeadingInfo(string programName)
            : this(programName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VSC.Base.CommandLine.Text.HeadingInfo"/> class
        /// specifying program name and version.
        /// </summary>
        /// <param name="programName">The name of the program.</param>
        /// <param name="version">The version of the program.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="programName"/> is null or empty string.</exception>
        public HeadingInfo(string programName, string version)
        {
            Assumes.NotNullOrEmpty(programName, "programName");

            _programName = programName;
            _version = version;
        }

        /// <summary>
        /// Gets the default heading instance.
        /// The title is retrieved from <see cref="AssemblyTitleAttribute"/>,
        /// or the assembly short name if its not defined.
        /// The version is retrieved from <see cref="AssemblyInformationalVersionAttribute"/>,
        /// or the assembly version if its not defined.
        /// </summary>
        public static HeadingInfo Default
        {
            get
            {
                var titleAttribute = ReflectionHelper.GetAttribute<AssemblyTitleAttribute>();
                string title = titleAttribute == null
                    ? ReflectionHelper.AssemblyFromWhichToPullInformation.GetName().Name
                    : Path.GetFileNameWithoutExtension(titleAttribute.Title);
                var versionAttribute = ReflectionHelper.GetAttribute<AssemblyInformationalVersionAttribute>();
                string version = versionAttribute == null
                    ? ReflectionHelper.AssemblyFromWhichToPullInformation.GetName().Version.ToString()
                    : versionAttribute.InformationalVersion;
                return new HeadingInfo(title, version);
            }
        }

        /// <summary>
        /// Converts the heading to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="VSC.Base.CommandLine.Text.HeadingInfo"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the heading.</returns>
        public static implicit operator string(HeadingInfo info)
        {
            return info.ToString();
        }

        /// <summary>
        /// Returns the heading as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the heading.</returns>
        public override string ToString()
        {
            bool isVersionNull = string.IsNullOrEmpty(_version);
            var builder = new StringBuilder(_programName.Length +
                (!isVersionNull ? _version.Length + 1 : 0));
            builder.Append(_programName);
            if (!isVersionNull)
            {
                builder.Append(' ');
                builder.Append(_version);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <param name="writer">The target <see cref="System.IO.TextWriter"/> derived type.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="writer"/> is null.</exception>
        public void WriteMessage(string message, TextWriter writer)
        {
            Assumes.NotNullOrEmpty(message, "message");
            Assumes.NotNull(writer, "writer");

            var builder = new StringBuilder(_programName.Length + message.Length + 2);
            builder.Append(_programName);
            builder.Append(": ");
            builder.Append(message);
            writer.WriteLine(builder.ToString());
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter to standard output stream.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        public void WriteMessage(string message)
        {
            WriteMessage(message, Console.Out);
        }

        /// <summary>
        /// Writes out a string and a new line using the program name specified in the constructor
        /// and <paramref name="message"/> parameter to standard error stream.
        /// </summary>
        /// <param name="message">The <see cref="System.String"/> message to write.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="message"/> is null or empty string.</exception>
        public void WriteError(string message)
        {
            WriteMessage(message, Console.Error);
        }
    }
}
