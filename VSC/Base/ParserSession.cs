using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VSC.AST;

namespace VSC.Base
{
    public class ParserSession
    {
        MD5 md5;

        public readonly char[] StreamReaderBuffer = new char[SeekableStreamReader.DefaultReadAheadSize * 2];
        public readonly Dictionary<char[], string>[] Identifiers = new Dictionary<char[], string>[Tokenizer.MaxIdentifierLength + 1];
        public readonly List<Parameter> ParametersStack = new List<Parameter>(4);
        public readonly char[] IDBuilder = new char[Tokenizer.MaxIdentifierLength];
        public readonly char[] NumberBuilder = new char[Tokenizer.MaxNumberLength];

    //    public LocationsBag LocationsBag { get; set; }
        public bool UseJayGlobalArrays { get; set; }
        public LocatedToken[] LocatedTokens { get; set; }

        public MD5 GetChecksumAlgorithm()
        {
            return md5 ?? (md5 = MD5.Create());
        }
    }
}
