using System;

namespace VSC.Base
{
	/// <summary>
	/// Platform-specific code.
	/// </summary>
	public static class Platform
	{
		public static StringComparer FileNameComparer {
			get {
				switch (Environment.OSVersion.Platform) {
					case PlatformID.Unix:
					case PlatformID.MacOSX:
						return StringComparer.Ordinal;
					default:
						return StringComparer.OrdinalIgnoreCase;
				}
			}
		}
	}
}
