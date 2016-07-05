using System.Collections.Generic;
using System.Collections;

namespace VSC
{

    static class ArrayComparer
    {
        public static bool IsEqual<T>(T[] array1, T[] array2)
        {
            if (array1 == null || array2 == null)
                return array1 == array2;

            var eq = EqualityComparer<T>.Default;

            for (int i = 0; i < array1.Length; ++i)
            {
                if (!eq.Equals(array1[i], array2[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
    //
    // This class has to be used by parser only, it reuses token
    // details once a file is parsed
    //

    //
    // Indicates whether it accepts XML documentation or not.
    //
}

