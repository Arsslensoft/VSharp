using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Std
{
    public interface IEventHandlerSupport
    {
      
    }
    public class EventHandlerList<T> where T:Delegate
    {
        T reference;
        EventHandlerList<T> next;
      
        public EventHandlerList(T r)
        {
            reference = r;
            next = null;
        }
        public EventHandlerList(T r, EventHandlerList<T> next)
        {
            reference = r;
            this.next = next;
        }

        private EventHandler s;
        public void InvokeAll()
        {
            
        }
    }
}
