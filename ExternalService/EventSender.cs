using Abstractions;
using System;

namespace ExternalService
{   
    public class EventSender : IEventSender
    {
        public void SendEvent(object e)
        {
            Console.WriteLine(e);
        }
    }
}
