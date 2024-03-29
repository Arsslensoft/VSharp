using System;
using System.Globalization;

namespace VSC.TypeSystem
{
	[Serializable]
	public struct DomRegion : IEquatable<DomRegion>
	{
		readonly string fileName;
		readonly int beginLine;
		readonly int endLine;
		readonly int beginColumn;
		readonly int endColumn;
		
		public readonly static DomRegion Empty = new DomRegion();
		
		public bool IsEmpty {
			get {
				return BeginLine <= 0;
			}
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		public int BeginLine {
			get {
				return beginLine;
			}
		}
		
		/// <value>
		/// if the end line is == -1 the end column is -1 too
		/// this stands for an unknwon end
		/// </value>
		public int EndLine {
			get {
				return endLine;
			}
		}
		
		public int BeginColumn {
			get {
				return beginColumn;
			}
		}
		
		/// <value>
		/// if the end column is == -1 the end line is -1 too
		/// this stands for an unknown end
		/// </value>
		public int EndColumn {
			get {
				return endColumn;
			}
		}
		
		public Location Begin {
			get {
				return new Location ( beginLine, beginColumn);
			}
		}
		
		public Location End {
			get {
				return new Location (endLine, endColumn);
			}
		}
		
		public DomRegion (int beginLine, int beginColumn, int endLine, int endColumn) : this (null, beginLine, beginColumn, endLine, endColumn)
		{
		}

		public DomRegion(string fileName, int beginLine, int beginColumn, int endLine, int endColumn)
		{
			this.fileName = fileName;
			this.beginLine   = beginLine;
			this.beginColumn = beginColumn;
			this.endLine     = endLine;
			this.endColumn   = endColumn;
		}
		
		public DomRegion (int beginLine, int beginColumn) : this (null, beginLine, beginColumn)
		{
		}
		
		public DomRegion (string fileName, int beginLine, int beginColumn)
		{
			this.fileName = fileName;
			this.beginLine = beginLine;
			this.beginColumn = beginColumn;
			this.endLine = -1;
			this.endColumn = -1;
		}
		
		public DomRegion (Location begin, Location end) : this (begin.SourceFile.FullPathName, begin, end)
		{
		}
		
		public DomRegion (string fileName, Location begin, Location end)
		{
			this.fileName = fileName;
			this.beginLine = begin.Line;
			this.beginColumn = begin.Column;
            this.endLine = end.Line;
			this.endColumn = end.Column;
		}
		
		public DomRegion (Location begin) : this (null, begin)
		{
		}
		
		public DomRegion (string fileName, Location begin)
		{
			this.fileName = fileName;
			this.beginLine = begin.Line;
			this.beginColumn = begin.Column;
			this.endLine = -1;
			this.endColumn = -1;
		}
		
		/// <remarks>
		/// Returns true, if the given coordinates (line, column) are in the region.
		/// This method assumes that for an unknown end the end line is == -1
		/// </remarks>
		public bool IsInside(int line, int column)
		{
			if (IsEmpty)
				return false;
			return line >= BeginLine &&
				(line <= EndLine   || EndLine == -1) &&
				(line != BeginLine || column >= BeginColumn) &&
				(line != EndLine   || column <= EndColumn);
		}

		public bool IsInside(Location location)
		{
			return IsInside(location.Line, location.Column);
		}

		/// <remarks>
		/// Returns true, if the given coordinates (line, column) are in the region.
		/// This method assumes that for an unknown end the end line is == -1
		/// </remarks>
		public bool Contains(int line, int column)
		{
			if (IsEmpty)
				return false;
			return line >= BeginLine &&
				(line <= EndLine   || EndLine == -1) &&
				(line != BeginLine || column >= BeginColumn) &&
				(line != EndLine   || column < EndColumn);
		}

		public bool Contains(Location location)
		{
            return Contains(location.Line, location.Column);
		}

		public bool IntersectsWith (DomRegion region)
		{
			return region.Begin <= End && region.End >= Begin;
		}

		public bool OverlapsWith (DomRegion region)
		{
			var maxBegin = Begin > region.Begin ? Begin : region.Begin;
			var minEnd = End < region.End ? End : region.End;
			return maxBegin < minEnd;
		}

		public override string ToString()
		{
			return string.Format(
				CultureInfo.InvariantCulture,
				"[DomRegion FileName={0}, Begin=({1}, {2}), End=({3}, {4})]",
				fileName, beginLine, beginColumn, endLine, endColumn);
		}
		
		public override bool Equals(object obj)
		{
			return obj is DomRegion && Equals((DomRegion)obj);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				int hashCode = fileName != null ? fileName.GetHashCode() : 0;
				hashCode ^= beginColumn + 1100009 * beginLine + 1200007 * endLine + 1300021 * endColumn;
				return hashCode;
			}
		}
		
		public bool Equals(DomRegion other)
		{
			return beginLine == other.beginLine && beginColumn == other.beginColumn
				&& endLine == other.endLine && endColumn == other.endColumn
				&& fileName == other.fileName;
		}
		
		public static bool operator ==(DomRegion left, DomRegion right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(DomRegion left, DomRegion right)
		{
			return !left.Equals(right);
		}
	}
}
