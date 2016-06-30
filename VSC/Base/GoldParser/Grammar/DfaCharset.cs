using System;
using System.Diagnostics;

namespace VSC.Base.GoldParser.Grammar {
	/// <summary>
	/// A grammar charset representation
	/// </summary>
	public sealed class DfaCharset: GrammarObject<DfaCharset> {
		private readonly char[] charsetExcludingSequence;
		private readonly char[] charsetIncludingSequence;
		private readonly char sequenceEnd;
		private readonly char sequenceStart;

		internal DfaCharset(CompiledGrammar owner, int index, string charset): base(owner, index) {
			if (string.IsNullOrEmpty(charset)) {
				throw new ArgumentException("Empty charsets are not supported", "charset");
			}
			charsetIncludingSequence = charset.ToCharArray();
			Array.Sort(charsetIncludingSequence);
			char currentStartChar = charsetIncludingSequence[0];
			char currentChar = currentStartChar;
			sequenceStart = currentStartChar;
			sequenceEnd = currentStartChar;
			int sequenceLength = 1;
			int currentLength = 1;
			for (int i = 1; i < charsetIncludingSequence.Length; i++) {
				currentChar ++;
				if (charsetIncludingSequence[i] == currentChar) {
					currentLength++;
					if (currentLength > sequenceLength) {
						sequenceLength = currentLength;
						sequenceStart = currentStartChar;
						sequenceEnd = currentChar;
					}
				} else {
					currentLength = 1;
					currentStartChar = charsetIncludingSequence[i];
					currentChar = currentStartChar;
				}
			}
			charsetExcludingSequence = new char[charsetIncludingSequence.Length-sequenceLength];
			int charsetIndex = 0;
			foreach (char c in charsetIncludingSequence) {
				if ((c < sequenceStart) || (c > sequenceEnd)) {
					charsetExcludingSequence[charsetIndex++] = c;
				}
			}
			Debug.Assert(charsetIndex == charsetExcludingSequence.Length);
		}

		public char[] CharactersExcludingSequence {
			get {
				return charsetExcludingSequence;
			}
		}

		public char[] CharactersIncludingSequence {
			get {
				return charsetIncludingSequence;
			}
		}

		public char SequenceEnd {
			get {
				return sequenceEnd;
			}
		}

		public int SequenceLength {
			get {
				return (sequenceEnd-sequenceStart)+1;
			}
		}

		public char SequenceStart {
			get {
				return sequenceStart;
			}
		}
	}
}
