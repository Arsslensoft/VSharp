using System;
using System.Diagnostics;
using System.Linq;

namespace VSC
{
 
	//
	//  This is one single source file.
	//

    /// <summary>
	///   Keeps track of the location in the program
	/// </summary>
	///
	/// <remarks>
	///   This uses a compact representation and a couple of auxiliary
	///   structures to keep track of tokens to (file,line and column) 
	///   mappings. The usage of the bits is:
	///   
	///     - 16 bits for "checkpoint" which is a mixed concept of
	///       file and "line segment"
	///     - 8 bits for line delta (offset) from the line segment
	///     - 8 bits for column number.
	///
	///   http://lists.ximian.com/pipermail/mono-devel-list/2004-December/009508.html
	/// </remarks>
	public struct Location : IEquatable<Location>, IComparable<Location>
	{


        SourceFile file;
		public readonly static Location Null = new Location ();

	
        public Location (int row, int column)
            :this(SourceFile.Null,row,column)
        {
           
        }
		public Location (SourceFile file, int row, int column)
		{
            this.file = file;
            line = row;
            this.column = column;
		}

		public static Location operator - (Location loc, int columns)
		{
			return new Location (loc.SourceFile, loc.Line, loc.Column - columns);
		}
        public static bool operator ==(Location loc, Location l)
        {
            return loc.file == l.file && l.Column == loc.Column && l.Line == loc.Line;
        }
        /// <summary>
        /// Compares two text locations.
        /// </summary>
        public static bool operator <(Location left, Location right)
        {
            if (left.line < right.line)
                return true;
            else if (left.line == right.line)
                return left.column < right.column;
            else
                return false;
        }

        /// <summary>
        /// Compares two text locations.
        /// </summary>
        public static bool operator >(Location left, Location right)
        {
            if (left.line > right.line)
                return true;
            else if (left.line == right.Line)
                return left.column > right.column;
            else
                return false;
        }

        /// <summary>
        /// Compares two text locations.
        /// </summary>
        public static bool operator <=(Location left, Location right)
        {
            return !(left > right);
        }

        /// <summary>
        /// Compares two text locations.
        /// </summary>
        public static bool operator >=(Location left, Location right)
        {
            return !(left < right);
        }

        /// <summary>
        /// Compares two text locations.
        /// </summary>
        public int CompareTo(Location other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            else
                return 1;
        }
        public static bool operator !=(Location loc, Location l)
        {
            return loc.file != l.file || l.Column != loc.Column || l.Line != loc.Line;
        }
	

		string FormatLocation (string fileName)
		{

            return fileName + "(" + Line.ToString() + "," + Column.ToString() +")";
				

		}
		
		public override string ToString ()
		{
			return FormatLocation (Name);
		}

		public string ToStringFullName ()
		{
			return FormatLocation (NameFullPath);
		}
		
		/// <summary>
		///   Whether the Location is Null
		/// </summary>
		public bool IsNull {
			get { return line == 0 && column == 0; }
		}

		public string Name {
			get {
				
				return file.Name;
			}
		}
        public SourceFile File
        {
            get { return file; }
        }
		public string NameFullPath {
			get {
                return File.FullPathName;
			}
		}

	


        int column, line;

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int Line
        {
            get { return line; }
        }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        public int Column
        {
            get { return column; }
        }
	

		// The ISymbolDocumentWriter interface is used by the symbol writer to
		// describe a single source file - for each source file there's exactly
		// one corresponding ISymbolDocumentWriter instance.
		//
		// This class has an internal hash table mapping source document names
		// to such ISymbolDocumentWriter instances - so there's exactly one
		// instance per document.
		//
		// This property returns the ISymbolDocumentWriter instance which belongs
		// to the location's source file.
		//
		// If we don't have a symbol writer, this property is always null.
		public SourceFile SourceFile {
			get {
                return File;
			}
		}

		#region IEquatable<Location> Members

		public bool Equals (Location other)
		{
			return this == other;
		}

		#endregion
	}
	
	
}
