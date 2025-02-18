

namespace delegates
{
    public delegate void LogHandler(string message);

    public interface ILogger
    {
        void Log(string message);
    }

    public class FileLogger : ILogger
    {
        public void Log(string message)
        {
            try
            {
                File.AppendAllText("file.txt", $"[{DateTime.Now}]:{message}\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            try
            {
                System.Console.WriteLine($"[KONSOLA]: {message}");
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
    public class LogManager{
        public LogHandler? LogMethods;

        public void AddLogMethod(LogHandler handler){
            if (LogMethods != null && LogMethods.GetInvocationList().Contains(handler)){
                System.Console.WriteLine("Metoda jest już dodana");
            }
            else{
                LogMethods += handler;
                System.Console.WriteLine("Dodano metode");
            }
        }
        public void RemoveLogMethod(LogHandler handler){
            if(LogMethods == null || !LogMethods.GetInvocationList().Contains(handler)){
                System.Console.WriteLine("Taka metoda nie została dodana");
            }
            else{
                LogMethods -= handler;
                System.Console.WriteLine("usunięto metodę");
            }
        }
        public void SendLog(string message)
        {
            if (LogMethods == null)
            {
                System.Console.WriteLine("brak metod do wysłania");
            }
            else
            {
                foreach (var handler in LogMethods.GetInvocationList())
                {
                    try
                    {
                        handler.DynamicInvoke(message);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                }
            }
        }

        public void MethodList(){
            if (LogMethods == null){
                System.Console.WriteLine("Nie dodano żandej metody");
            }
            else{
                foreach(var handler in LogMethods.GetInvocationList())
                {
                    System.Console.WriteLine($"- {handler.Method.Name}");
                }
            }
        }

    }
    public class Program
    {
        public static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("Menu logowania:");
            System.Console.WriteLine("1) Dodaj logowanie do pliku");
            System.Console.WriteLine("2) Dodaj logowanie na konsolę");
            System.Console.WriteLine("3) Usuń logowanie do pliku");
            System.Console.WriteLine("4) Usuń logownaie na konsolę");
            System.Console.WriteLine("5) Wyślij log");
            System.Console.WriteLine("6) Wyświetl aktywne metody logowania");
            System.Console.WriteLine("7) Wyjdź");
        }
        public static int ValidIntInput(string prompt)
        {
            System.Console.WriteLine(prompt);
            string input = Console.ReadLine();
            int result;
            while (!int.TryParse(input, out result))
            {
                Console.WriteLine("Nieprawidłowe dane. Spróbuj ponownie");
                input = Console.ReadLine();
            }
            return result;
        }
        public static string GetLogMessage()
        {
            System.Console.WriteLine("Podaj treść logu");
            return Console.ReadLine();
        }
        static void Main(string[] args)
        {
            FileLogger fileLogger = new FileLogger();
            ConsoleLogger consoleLogger = new ConsoleLogger();
            var logManager = new LogManager();


            while (true)
            {
                try
                {
                    ShowMenu();
                    int choice = ValidIntInput("Podaj wybór: ");

                    switch (choice){
                        case 1:
                            logManager.AddLogMethod(fileLogger.Log);
                            break;
                        case 2:
                            logManager.AddLogMethod(consoleLogger.Log);
                            break;
                        case 3:
                            logManager.RemoveLogMethod(fileLogger.Log);
                            break;
                        case 4:
                            logManager.RemoveLogMethod(consoleLogger.Log);
                            break;
                        case 5:
                            string message = GetLogMessage();
                            logManager.SendLog(message);
                            break;
                        case 6:
                            logManager.MethodList();
                            break;
                        case 7:
                            Environment.Exit(0);
                            break;
                        default:
                            System.Console.WriteLine("Nieprawidłowy wybór, spróbuj ponownie");
                            break;
                    }
                    System.Console.WriteLine("Kliknij dowolny przycisk aby kontunuować...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}