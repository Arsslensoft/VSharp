using System;

namespace VSC
{
    public sealed class FileListAttribute : System.Attribute
    {
        public FileListAttribute(Type concreteType)
        {

        }

        public int MaximumElements { get; set; }
    }
}