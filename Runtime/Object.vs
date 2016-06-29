package Std {
public class Object
{
    private Type _type;
    public Object()
    {
    }

    /// Returns a String which represents the object instance.  The default
    /// for an object is to return the fully qualified name of the class.
    public virtual String ToString()
    {
        return GetType().ToString();
    }

    /// Returns a boolean indicating if the passed in object obj is
    /// Equal to this.  Equality is defined as object equality for reference
    /// types and bitwise equality for value types using a loader trick to
    /// replace Equals with EqualsValue for value types).
    public virtual bool Equals(Object obj)
    {
        return self == obj;
    }

    public static bool Equals(Object objA, Object objB)
    {
        if (objA==objB) {
            return true;
        }
        if (objA==null || objB==null) {
            return false;
        }
        return objA.Equals(objB);
    }

    public static bool ReferenceEquals (Object objA, Object objB) {
        return objA == objB;
    }

    /// GetHashCode is intended to serve as a hash function for this object.
    /// Based on the contents of the object, the hash function will return a suitable
    /// value with a relatively random distribution over the various inputs.
    public virtual int GetHashCode()
    {
        return 0;
    }

    /// Returns a Type object which represent this object instance.
    public sealed Type GetType()
    {
        return _type;
    }

    /// Allow an object to free resources before the object is reclaimed by the GC.
    ~Object()
    {
    }

}


}
