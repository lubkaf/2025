using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace projekt_uprawnienia
{
    public enum Role
    {
        Administrator,
        Manager,
        User
    }

    public enum Permission
    {
        Read,
        Write,
        Delete,
        ManageUsers
    }

    public class User
    {
        public string Username { get; set; }
        public List<Role> Roles { get; set; }

        public User(string username)
        {
            Roles = new List<Role>();
            Username = username;
        }

        public void AddRole(Role role)
        {
            if (!Roles.Contains(role))
            {
                Roles.Add(role);
            }
        }
    }

    public class RBAC
    {
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;

        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<Permission>>
            {
                { Role.Administrator, new List<Permission>{ Permission.Read, Permission.Write, Permission.Delete, Permission.ManageUsers }},
                { Role.Manager, new List<Permission>{ Permission.Read, Permission.Write }},
                { Role.User, new List<Permission>{ Permission.Read } }
            };
        }

        public bool HasPermission(User user, Permission permission)
        {
            foreach (var role in user.Roles)
            {
                if (_rolePermissions.ContainsKey(role) && _rolePermissions[role].Contains(permission))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class PasswordManager
    {
        private const string _passwordFilePath = "userPasswords.txt";
        public static event Action<string, bool> PasswordVerified;

        static PasswordManager()
        {
            if (!File.Exists(_passwordFilePath))
            {
                File.Create(_passwordFilePath).Dispose();
            }
        }

        public static void SavePassword(string username, string password)
        {
            if (File.ReadLines(_passwordFilePath).Any(line => line.Split(',')[0] == username))
            {
                Console.WriteLine($"User {username} already exists.");
                return;
            }

            string hashedPassword = HashPassword(password);
            File.AppendAllText(_passwordFilePath, $"{username},{hashedPassword}\n");
            Console.WriteLine($"User {username} has been registered.");
        }

        public static bool VerifyPassword(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            foreach (var line in File.ReadLines(_passwordFilePath))
            {
                var parts = line.Split(',');
                if (parts[0] == username && parts[1] == hashedPassword)
                {
                    PasswordVerified?.Invoke(username, true);
                    return true;
                }
            }
            PasswordVerified?.Invoke(username, false);
            return false;
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }

    public class GroupChat
    {
        private const string _chatFilePath = "file.txt";

        public static event Action<string, string> MessageLogged;

        public static void AddMessage(string username, string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            string formattedMessage = $"{DateTime.Now} [{username}]: {message}\n";
            File.AppendAllText(_chatFilePath, formattedMessage);

            // Trigger the event after writing to the file
            MessageLogged?.Invoke(username, message);
        }

        public static void DisplayChat()
        {
            if (File.Exists(_chatFilePath))
            {
                Console.WriteLine("Chat History:");
                Console.WriteLine(File.ReadAllText(_chatFilePath));
            }
            else
            {
                Console.WriteLine("No messages in chat yet.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            PasswordManager.PasswordVerified += (username, success) =>
                Console.WriteLine($"User {username} login {(success ? "successful" : "failed")}");

            // Register users and save their passwords
            PasswordManager.SavePassword("AdminUser", "adminPassword");
            PasswordManager.SavePassword("ManagerUser", "managerPassword");
            PasswordManager.SavePassword("NormalUser", "userPassword");

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.Clear();
                Console.WriteLine("=== Login System ===");

                Console.Write("\nEnter username: ");
                string username = Console.ReadLine();

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                if (!PasswordManager.VerifyPassword(username, password))
                {
                    Console.WriteLine("Invalid username or password.");
                    Console.ReadKey();
                    continue;
                }

                var user = new User(username);
                if (username == "AdminUser") user.AddRole(Role.Administrator);
                else if (username == "ManagerUser") user.AddRole(Role.Manager);
                else if (username == "NormalUser") user.AddRole(Role.User);

                var rbacSystem = new RBAC();
                string message = "";

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Logged in as: {username}");
                    if (rbacSystem.HasPermission(user, Permission.Read))
                        Console.WriteLine("1. View chat history");
                    if (rbacSystem.HasPermission(user, Permission.Write))
                        Console.WriteLine("2. Send a message to the chat");
                    Console.WriteLine("3. Logout");
                    Console.WriteLine("4. Exit program");

                    int choice;
                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid choice. Try again.");
                        continue;
                    }

                    switch (choice)
                    {
                        case 1:
                            if (rbacSystem.HasPermission(user, Permission.Read))
                            {
                                GroupChat.DisplayChat();
                            }
                            else
                            {
                                Console.WriteLine("You do not have permission to view the chat.");
                            }
                            break;

                        case 2:
                            if (rbacSystem.HasPermission(user, Permission.Write))
                            {
                                Console.WriteLine("Enter your message:");
                                message = Console.ReadLine();
                                GroupChat.AddMessage(username, message);
                            }
                            else
                            {
                                Console.WriteLine("You do not have permission to send messages.");
                            }
                            break;

                        case 3:
                            Console.WriteLine("Logging out...");
                            break;

                        case 4:
                            exitProgram = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    if (choice == 3 || exitProgram) break;

                    Console.ReadKey();
                }
            }
        }
    }
}
