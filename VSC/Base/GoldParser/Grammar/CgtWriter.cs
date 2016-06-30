using System;
using System.IO;
using System.Text;

namespace VSC.Base.GoldParser.Grammar {
	internal class CgtWriter {
		private readonly BinaryWriter writer;
		private int entryCount;

		public CgtWriter(BinaryWriter writer) {
			if (writer == null) {
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		public void WriteBoolEntry(bool value) {
			WriteEntryType(CgtEntryType.Boolean);
			entryCount--;
			writer.Write(value);
		}

		public void WriteByteEntry(byte value) {
			WriteEntryType(CgtEntryType.Byte);
			entryCount--;
			writer.Write(value);
		}

		public void WriteEmptyEntry() {
			WriteEntryType(CgtEntryType.Empty);
			entryCount--;
		}

		public void WriteHeaderString(string header) {
			if (entryCount != 0) {
				throw new FileLoadException("Header expected");
			}
			WriteString(header);
		}

		public void WriteIntegerEntry(int value) {
			WriteEntryType(CgtEntryType.Integer);
			entryCount--;
			writer.Write(checked((UInt16)value));
		}

		public void WriteNextRecord(CgtRecordType recordType, int entries) {
			if (entryCount != 0) {
				throw new InvalidOperationException("There are entries missing before starting a new record");
			}
			entryCount = entries+1;
			writer.Write((byte)'M');
			writer.Write(checked((UInt16)entryCount));
			WriteByteEntry((byte)recordType);
		}

		public void WriteStringEntry(string value) {
			WriteEntryType(CgtEntryType.String);
			entryCount--;
			WriteString(value);
		}

		private void WriteEntryType(CgtEntryType entryType) {
			if (entryCount <= 0) {
				throw new FileLoadException("No entry pending in this record");
			}
			writer.Write((byte)entryType);
		}

		private void WriteString(string data) {
			if (data == null) {
				throw new ArgumentNullException("data");
			}
			writer.Write(Encoding.Unicode.GetBytes(data));
			writer.Write((UInt16)0);
		}
	}
}
