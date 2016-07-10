using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VSC;

namespace VSCTests
{
   static class TestManager
    {
       public static bool HasCode(this List<AbstractMessage> s, int code, int line,int col)
       {
           foreach (var e in s)
               if (e.Code == code && e.Location.Line == line && e.Location.Column == col)
                   return true;

           return false;
       }
       public static List<AbstractMessage> RunTests(object o)
       {
           Type t = o.GetType();
           foreach (MethodInfo prop in
               t.GetMethods(BindingFlags.Instance | BindingFlags.Public)) 
           {
               if (Attribute.IsDefined(prop, typeof(SourceFileAttribute)))
               {
                   var value = prop.GetCustomAttribute(typeof(SourceFileAttribute)) as SourceFileAttribute;
                   return value.Errors;
               }
           }
           return new List<AbstractMessage>();
       }
    }
}
