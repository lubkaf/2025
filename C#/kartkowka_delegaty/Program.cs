namespace Notifier
{
    public class EmailNotifier
    {
        public void SendEmail(string message) => Console.WriteLine($"Email: {message}");  
    }

        public class SMSNotifier
    {
        public void SendSMS(string message) => Console.WriteLine($"SMS: {message}");  
    }

        public class PushNotifier
    {
        public void SendPushNotification(string message) => Console.WriteLine($"Push Notification: {message}");  
    }

    public class NotificationManager
    {
        public delegate void  NofiticationHandler(string message);
        private NofiticationHandler _notificationHandler;

        private readonly Dictionary<string, string> _notificationNames = new Dictionary<string, string>
        {
            { "SendEmail", "Email" },
            { "SendSMS", "SMS" },
            { "SendPushNotification", "Push" }
        };
        public void AddNotificationMethod(NofiticationHandler method)
        {
            string methodName = method.Method.Name;
            string notificationName = _notificationNames.ContainsKey(methodName) ? _notificationNames[methodName] : methodName;
            System.Console.WriteLine($"Dodano powiadomienie {notificationName}");
            _notificationHandler += method;
        }
        public void RemoveNotificationMethod(NofiticationHandler method)
        {
            string methodName = method.Method.Name;
            string notificationName = _notificationNames.ContainsKey(methodName) ? _notificationNames[methodName] : methodName;
            
            System.Console.WriteLine($"Usunięto powiadomienie: {notificationName}");
            _notificationHandler -= method;
        }
        public void SendNotification(string message)
        {
            System.Console.WriteLine($"Wysyłanie powiadomienia: \"{message}\"");

            if (_notificationHandler != null)
            {
                _notificationHandler.Invoke(message);
            }
            else
            {
                Console.WriteLine("Brak powiadomień do wysłania.");
            }
        }

        public void ClearNotificationMethods()
        {
            _notificationHandler = null;
            Console.WriteLine("Wszystkie powiadomienia zostały usunięte.");
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            EmailNotifier emailNotifier = new EmailNotifier();
            SMSNotifier smsNotifier = new SMSNotifier();
            PushNotifier pushNotifier = new PushNotifier();

            NotificationManager notificationManager = new NotificationManager();

            notificationManager.AddNotificationMethod(emailNotifier.SendEmail);
            notificationManager.AddNotificationMethod(smsNotifier.SendSMS);
            notificationManager.AddNotificationMethod(pushNotifier.SendPushNotification);
            System.Console.WriteLine();

            notificationManager.SendNotification("Spotkanie o 15:00");

            System.Console.WriteLine();
            notificationManager.RemoveNotificationMethod(smsNotifier.SendSMS);
            System.Console.WriteLine();
            notificationManager.SendNotification("Nowa wersja aplikacji jest już dostępna");

            System.Console.WriteLine();
            notificationManager.ClearNotificationMethods();
            System.Console.WriteLine();

            notificationManager.SendNotification("Jutro masz urodziny!!!");
        
        
        Console.WriteLine("\nWitaj! Naciśnij dowolny klawisz, aby przejść dalej...");
            Console.ReadKey();

            bool running = true;
            while (running)
            {
                
                Console.Clear();
                Console.WriteLine("Wybierz opcję:");
                Console.WriteLine("1. Dodaj powiadomienie Email");
                Console.WriteLine("2. Dodaj powiadomienie SMS");
                Console.WriteLine("3. Dodaj powiadomienie Push");
                Console.WriteLine("4. Usuń powiadomienie Email");
                Console.WriteLine("5. Usuń powiadomienie SMS");
                Console.WriteLine("6. Usuń powiadomienie Push");
                Console.WriteLine("7. Wyślij powiadomienie/a");
                Console.WriteLine("8. Wyczyść wszystkie powiadomienia");
                Console.WriteLine("9. Zakończ");

                string choice = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(choice))
                {
                    Console.WriteLine("Nieprawidłowy wybór, spróbuj ponownie.");
                    Console.ReadKey();

                    continue;
                }  
                switch (choice)
                {
                    case "1":
                        notificationManager.AddNotificationMethod(emailNotifier.SendEmail);
                        break;
                    case "2":
                        notificationManager.AddNotificationMethod(smsNotifier.SendSMS);
                        break;
                    case "3":
                        notificationManager.AddNotificationMethod(pushNotifier.SendPushNotification);
                        break;
                    case "4":
                        notificationManager.RemoveNotificationMethod(emailNotifier.SendEmail);
                        break;
                    case "5":
                        notificationManager.RemoveNotificationMethod(smsNotifier.SendSMS);
                        break;
                    case "6":
                        notificationManager.RemoveNotificationMethod(pushNotifier.SendPushNotification);
                        break;
                    case "7": 
                        Console.WriteLine("Podaj wiadomość do wysłania:");
                        System.Console.WriteLine();
                        string message = Console.ReadLine();
                        notificationManager.SendNotification(message);
                        break;
                    case "8":
                        notificationManager.ClearNotificationMethods();
                        break;
                    case "9":
                        running = false;
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowy wybór, spróbuj ponownie.");
                        break;
                }
                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                Console.ReadKey();
            }

        }
    }
}