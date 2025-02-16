using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EF_tutorial
{
    internal class Program
    {
        // Klasę User traktujemy tylko jako model danych, bez logiki bazy
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }

            public User(string name, string email)
            {
                Name = name;
                Email = email;
            }
        }

        // Kontekst bazy danych - konfiguracja połączenia
        public class WorkshopContext : DbContext
        {
            public DbSet<User> Users { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string server = "localhost"; 
                string database = "workshop";
                string user = "root";
                string password = "";
                optionsBuilder.UseMySql($"server={server};database={database};user={user};password={password};",
                    new MySqlServerVersion(new Version(10, 5, 9)));
            }
        }

        // obsługa operacji CRUD na User
        public class UserRepository
        {
            private readonly WorkshopContext _context;

            public UserRepository(WorkshopContext context)
            {
                _context = context;
            }

            public void CreateUser(User user)
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                Console.WriteLine("Użytkownik został dodany do bazy danych.");
            }

            public void UpdateUser(User user)
            {
                _context.Users.Update(user);
                _context.SaveChanges();
                Console.WriteLine("Użytkownik został zaktualizowany.");
            }

            public void DeleteUser(int userId)
            {
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    Console.WriteLine("Użytkownik został usunięty.");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono użytkownika.");
                }
            }

            public User ReadUser(int userId)
            {
                return _context.Users.Find(userId);
            }

            public List<User> GetAllUsers()
            {
                return new List<User>(_context.Users);
            }
        }

        static void Main(string[] args)
        {
            using var context = new WorkshopContext();
            var userRepo = new UserRepository(context);

            //var user = new User(2, "Jan Kowalski", "jan@kowy.pl");
            //Przykład dodania użytkownika --> userRepo.CreateUser(user);
            // Przykład pobrania użytkownika z bazy do kodu -->  var user = userRepo.ReadUser(1);
            // -->  if (user != null)
            // -->     Console.WriteLine($"Użytkownik: {user.Name}, {user.Email}");
            //Przykład zaktualizowania użytkownika --> user.Name = "Nowe Imię";
            // --> userRepo.UpdateUser(user1);



            //wypisanie wszystkich użytkowników z bazy


            var NewUser = new User(getString("Podaj imię: "), getString("Podaj email: "));
            userRepo.CreateUser(NewUser);

            var AllUsers = userRepo.GetAllUsers();
            foreach (var user in AllUsers)
            {
                Console.WriteLine($"imię: {user.Name} \t email: {user.Email}");
            }
        }

        private static string getString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
