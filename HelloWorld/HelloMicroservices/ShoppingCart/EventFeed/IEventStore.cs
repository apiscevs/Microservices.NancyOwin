using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public interface IEventStore
    {
        IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber);
        void Raise(string eventName, object content);
    }
}
