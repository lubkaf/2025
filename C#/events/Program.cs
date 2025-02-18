
using System;

namespace events
{
    public delegate void EventHandler(string message);

    public class Sender
    {
        public event EventHandler EventOccured;
        public void TriggerEvent(string message)
        {
            EventOccured?.Invoke(message);
        }
    }

    public class Receiver
    {
        public void ReactToEvent(string message){
            File.AppendAllText("file.txt", $"{DateTime.Now} Message received: {message}\n");
        }
    }
    public class Program
    {
        static void Main(string[] args)
        {
            Sender sender = new Sender();
            Receiver receiver = new Receiver();

            sender.EventOccured += receiver.ReactToEvent;

            sender.TriggerEvent("Hello world!");
        }
    }
}
