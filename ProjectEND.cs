using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PersonalFinanceSystem
{
    public class User
    {
        public string Username { get; private set; }
        public string NationalId { get; private set; }

        public User(string username, string nationalId)
        {
            Username = username;
            NationalId = nationalId;
        }
    }

    public abstract class Transaction
    {
        public decimal Amount { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        protected Transaction(decimal amount, DateTime date, string categoryName, string description)
        {
            Amount = amount;
            Date = date;
            CategoryName = categoryName;
            Description = description;
        }
    }

    public class Income : Transaction
    {
        public Income(decimal amount, DateTime date, string categoryName, string description)
            : base(amount, date, categoryName, description)
        {
            Type = "Income";
        }
    }

    public class Expense : Transaction
    {
        public Expense(decimal amount, DateTime date, string categoryName, string description)
            : base(amount, date, categoryName, description)
        {
            Type = "Expense";
        }
    }

    public class Category
    {
        public string Name { get; set; }

        public Category(string name)
        {
            Name = name;
        }
    }

    public class FinanceManager
    {
        private List<User> users;
        private List<Transaction> transactions;
        private List<Category> categories;
        private User activeUser;
        private const string UsersFile = "users.json";
        private const string TransactionsFile = "transactions.json";
        private const string CategoriesFile = "categories.json";

        public FinanceManager()
        {
            users = LoadUsers();
            transactions = LoadTransactions();
            categories = LoadCategories();
            if (users.Count > 0 && activeUser == null)
            {
                activeUser = users[0];
            }
        }

        private List<User> LoadUsers()
        {
            if (!File.Exists(UsersFile))
            {
                return new List<User>();
            }
            string json = File.ReadAllText(UsersFile);
            var result = JsonConvert.DeserializeObject<List<User>>(json);
            if (result == null)
            {
                return new List<User>();
            }
            return result;
        }

        private void SaveUsers()
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(UsersFile, json);
        }

        private List<Transaction> LoadTransactions()
        {
            if (!File.Exists(TransactionsFile))
            {
                return new List<Transaction>();
            }
            string json = File.ReadAllText(TransactionsFile);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            var data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json, settings);
            if (data == null)
            {
                data = new List<Dictionary<string, object>>();
            }

            List<Transaction> loadedTransactions = new List<Transaction>();
            foreach (var item in data)
            {
                try
                {
                    string type;
                    if (item.ContainsKey("Type"))
                    {
                        type = item["Type"].ToString();
                    }
                    else
                    {
                        type = "Unknown";
                    }

                    decimal amount;
                    if (item.ContainsKey("Amount"))
                    {
                        amount = decimal.Parse(item["Amount"].ToString());
                    }
                    else
                    {
                        amount = 0;
                    }

                    DateTime date;
                    if (item.ContainsKey("Date"))
                    {
                        date = DateTime.Parse(item["Date"].ToString());
                    }
                    else
                    {
                        date = DateTime.Now;
                    }

                    string categoryName;
                    if (item.ContainsKey("CategoryName"))
                    {
                        categoryName = item["CategoryName"].ToString();
                    }
                    else
                    {
                        categoryName = "";
                    }

                    string description;
                    if (item.ContainsKey("Description"))
                    {
                        description = item["Description"].ToString();
                    }
                    else
                    {
                        description = "";
                    }
                    if (type == "Income")
                    {
                        loadedTransactions.Add(new Income(amount, date, categoryName, description));
                    }
                    else if (type == "Expense")
                    {
                        loadedTransactions.Add(new Expense(amount, date, categoryName, description));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading transaction: " + ex.Message);
                }
            }
            return loadedTransactions;
        }

        private void SaveTransactions()
        {
            string json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
            File.WriteAllText(TransactionsFile, json);
        }

        private List<Category> LoadCategories()
        {
            if (!File.Exists(CategoriesFile))
            {
                return new List<Category>
                {
                    new Category("ghaza"),
                    new Category("haml va naghl"),
                    new Category("ghaboz"),
                    new Category("kharid"),
                    new Category("sargham"),
                    new Category("kar")
                };
            }
            string json = File.ReadAllText(CategoriesFile);
            var result = JsonConvert.DeserializeObject<List<Category>>(json);
            if (result == null)
            {
                return new List<Category>();
            }
            return result;
        }

        private void SaveCategories()
        {
            string json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            File.WriteAllText(CategoriesFile, json);
        }

        public void AddUser(string username, string nationalId)
        {
            foreach (var user in users)
            {
                if (user.Username == username || user.NationalId == nationalId)
                {
                    Console.WriteLine("karbari ba in nam ya code melli vojood darad!");
                    return;
                }
            }
            users.Add(new User(username, nationalId));
            activeUser = users[users.Count - 1];
            SaveUsers();
            Console.WriteLine("karbar ezafe shod!");
        }

        public void ChangeActiveUser(string username)
        {
            foreach (var user in users)
            {
                if (user.Username == username)
                {
                    activeUser = user;
                    Console.WriteLine("karbar faal taghir kard be: " + username);
                    return;
                }
            }
            Console.WriteLine("karbari ba in nam yaft nashod!");
        }

        public void DisplayUsers()
        {
            if (users.Count == 0)
            {
                Console.WriteLine("hic karbari nist!");
                return;
            }
            Console.WriteLine("list karbaran :");
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine((i + 1) + ". nam: " + users[i].Username + ", code melli: " + users[i].NationalId);
            }
        }

        public void AddIncome(decimal amount, DateTime date, string categoryName, string description)
        {
            if (activeUser == null)
            {
                Console.WriteLine("hic karbar faal nist!");
                return;
            }
            if (!CategoryExists(categoryName))
            {
                Console.WriteLine("dastebandi nameotabar ast!");
                return;
            }
            transactions.Add(new Income(amount, date, categoryName, description));
            SaveTransactions();
            Console.WriteLine("daramad sabt shod baraye: " + activeUser.Username);
        }

        public void AddExpense(decimal amount, DateTime date, string categoryName, string description)
        {
            if (activeUser == null)
            {
                Console.WriteLine("hic karbar faal nist!");
                return;
            }
            if (!CategoryExists(categoryName))
            {
                Console.WriteLine("dastebandi name'tabar ast!");
                return;
            }
            transactions.Add(new Expense(amount, date, categoryName, description));
            SaveTransactions();
            Console.WriteLine("kharj sabt shod baraye: " + activeUser.Username);
        }

        public void EditOrDeleteTransaction(int index, decimal newAmount = 0, string newDescription = null)
        {
            if (index < 0 || index >= transactions.Count)
            {
                Console.WriteLine("shomare tarakonesh eshtebahe ast!");
                return;
            }
            if (newAmount > 0 || newDescription != null)
            {
                if (newAmount > 0)
                {
                    transactions[index].Amount = newAmount;
                }
                else
                {
                    transactions[index].Amount = transactions[index].Amount;
                }
                if (newDescription != null)
                {
                    transactions[index].Description = newDescription;
                }
                else
                {
                    transactions[index].Description = transactions[index].Description;
                }
                Console.WriteLine("tarakonesh virayesh shod!");
            }
            else
            {
                transactions.RemoveAt(index);
                Console.WriteLine("tarakonesh hazf shod!");
            }
            SaveTransactions();
        }

        public void DisplayTransactions()
        {
            if (transactions.Count == 0)
            {
                Console.WriteLine("hic tarakoneshi nist!");
                return;
            }
            Console.WriteLine("list tarakoneshha:");
            for (int i = 0; i < transactions.Count; i++)
            {
                Console.WriteLine(i + ". no: " + transactions[i].Type + ", mablagh: " + transactions[i].Amount + ", tarikh: " + transactions[i].Date + ", daste: " + transactions[i].CategoryName + ", tozihat: " + transactions[i].Description);
            }
        }

        public void AddCategory(string name)
        {
            if (!CategoryExists(name))
            {
                categories.Add(new Category(name));
                SaveCategories();
                Console.WriteLine("dastebandi ezafe shod!");
            }
            else
            {
                Console.WriteLine("in dastebandi vojood darad!");
            }
        }

        private bool CategoryExists(string name)
        {
            foreach (var cat in categories)
            {
                if (cat.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public void DisplayCategories()
        {
            if (categories.Count == 0)
            {
                Console.WriteLine("hic dastebandi nist!");
                return;
            }
            Console.WriteLine("list dastebandiha:");
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + categories[i].Name);
            }
        }

        public void MonthlySummary(int year, int month)
        {
            decimal totalIncome = 0;
            decimal totalExpense = 0;
            foreach (var t in transactions)
            {
                if (t.Date.Year == year && t.Date.Month == month)
                {
                    if (t.Type == "Income")
                    {
                        totalIncome += t.Amount;
                    }
                    else
                    {
                        totalExpense += t.Amount;
                    }
                }
            }
            Console.WriteLine("khalase mah " + month + "/" + year + ":");
            Console.WriteLine("daramad: " + totalIncome + " toman");
            Console.WriteLine("kharj: " + totalExpense + " toman");
            Console.WriteLine("mande: " + (totalIncome - totalExpense) + " toman");
            Console.WriteLine("namudar sade (har * = 1 million toman):");
            Console.WriteLine("daramad: " + new string('*', (int)(totalIncome / 1000000)));
            Console.WriteLine("kharj: " + new string('*', (int)(totalExpense / 1000000)));
        }

        public void CategoryReport(int year, int month)
        {
            var totals = new Dictionary<string, decimal>();
            foreach (var t in transactions)
            {
                if (t.Date.Year == year && t.Date.Month == month && t.Type == "Expense")
                {
                    if (totals.ContainsKey(t.CategoryName))
                    {
                        totals[t.CategoryName] += t.Amount;
                    }
                    else
                    {
                        totals[t.CategoryName] = t.Amount;
                    }
                }
            }
            Console.WriteLine("gozaresh dastebandi " + month + "/" + year + ":");
            foreach (var pair in totals)
            {
                Console.WriteLine("daste: " + pair.Key + ", majmo: " + pair.Value + " toman");
                Console.WriteLine("namudar: " + new string('*', (int)(pair.Value / 1000000)));
            }
        }

        public void DailyBalance(int year, int month, int day)
        {
            decimal balance = 0;
            foreach (var t in transactions)
            {
                if (t.Date.Year == year && t.Date.Month == month && t.Date.Day == day)
                {
                    if (t.Type == "Income")
                    {
                        balance += t.Amount;
                    }
                    else
                    {
                        balance -= t.Amount;
                    }
                }
            }
            Console.WriteLine("mande roz " + day + "/" + month + "/" + year + ": " + balance + " toman");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FinanceManager manager = new FinanceManager();

            while (true)
            {
                Console.WriteLine("=== sistem modiriat mali ===");
                Console.WriteLine("1. modiriat karbaran");
                Console.WriteLine("2. modiriat tarakoneshha");
                Console.WriteLine("3. modiriat dastebandiha");
                Console.WriteLine("4. gozareshgiri");
                Console.WriteLine("5. khoroj");
                Console.WriteLine("=======================");
                Console.Write("gheire ro entekhab kon: ");

                string input = Console.ReadLine();
                if (input == "1")
                {
                    Console.WriteLine("\n--- modiriat karbaran ---");
                    Console.WriteLine("1. ezafe kardan karbar");
                    Console.WriteLine("2. taghir karbar faal");
                    Console.WriteLine("3. namayesh karbaran");
                    Console.Write("gheire: ");
                    string userChoice = Console.ReadLine();
                    if (userChoice == "1")
                    {
                        Console.Write("nam karbari: ");
                        string username = Console.ReadLine();
                        Console.Write("code melli: ");
                        string nationalId = Console.ReadLine();
                        manager.AddUser(username, nationalId);
                    }
                    else if (userChoice == "2")
                    {
                        Console.Write("nam karbari jadid: ");
                        string newUsername = Console.ReadLine();
                        manager.ChangeActiveUser(newUsername);
                    }
                    else if (userChoice == "3")
                    {
                        manager.DisplayUsers();
                    }
                }
                else if (input == "2")
                {
                    Console.WriteLine("\n--- modiriat tarakoneshha ---");
                    Console.WriteLine("1. sabt daramad");
                    Console.WriteLine("2. sabt kharj");
                    Console.WriteLine("3. virayesh/hazf tarakonesh");
                    Console.WriteLine("4. namayesh tarakoneshha");
                    Console.Write("gheire: ");
                    string transChoice = Console.ReadLine();
                    if (transChoice == "1")
                    {
                        Console.Write("mablagh: "); decimal amount = decimal.Parse(Console.ReadLine());
                        Console.Write("tarikh (YYYY-MM-DD): "); DateTime date = DateTime.Parse(Console.ReadLine());
                        Console.Write("dastebandi: "); string cat = Console.ReadLine();
                        Console.Write("tozihat: "); string desc = Console.ReadLine();
                        manager.AddIncome(amount, date, cat, desc);
                    }
                    else if (transChoice == "2")
                    {
                        Console.Write("mablagh: "); decimal amount = decimal.Parse(Console.ReadLine());
                        Console.Write("tarikh (YYYY-MM-DD): "); DateTime date = DateTime.Parse(Console.ReadLine());
                        Console.Write("dastebandi: "); string cat = Console.ReadLine();
                        Console.Write("tozihat: "); string desc = Console.ReadLine();
                        manager.AddExpense(amount, date, cat, desc);
                    }
                    else if (transChoice == "3")
                    {
                        Console.Write("shomare tarakonesh: "); int index = int.Parse(Console.ReadLine());
                        Console.Write("mablagh jadid (0 baraye adam taghir): "); decimal newAmount = decimal.Parse(Console.ReadLine());
                        Console.Write("tozihat jadid (khaali baraye adam taghir): "); string newDesc = Console.ReadLine();
                        manager.EditOrDeleteTransaction(index, newAmount, newDesc);
                    }
                    else if (transChoice == "4")
                    {
                        manager.DisplayTransactions();
                    }
                }
                else if (input == "3")
                {
                    Console.WriteLine("\n--- modiriat dastebandiha ---");
                    Console.WriteLine("1. namayesh dastebandiha");
                    Console.WriteLine("2. ezafe kardan dastebandi");
                    Console.Write("gheire: ");
                    string catChoice = Console.ReadLine();
                    if (catChoice == "1")
                    {
                        manager.DisplayCategories();
                    }
                    else if (catChoice == "2")
                    {
                        Console.Write("nam dastebandi: "); string name = Console.ReadLine();
                        manager.AddCategory(name);
                    }
                }
                else if (input == "4")
                {
                    Console.WriteLine("\n--- gozareshgiri ---");
                    Console.WriteLine("1. khalase mahane");
                    Console.WriteLine("2. gozaresh dastebandi");
                    Console.WriteLine("3. mande rozane");
                    Console.Write("gheire: ");
                    string reportChoice = Console.ReadLine();
                    Console.Write("saal: "); int year = int.Parse(Console.ReadLine());
                    Console.Write("mah: "); int month = int.Parse(Console.ReadLine());
                    if (reportChoice == "1")
                    {
                        manager.MonthlySummary(year, month);
                    }
                    else if (reportChoice == "2")
                    {
                        manager.CategoryReport(year, month);
                    }
                    else if (reportChoice == "3")
                    {
                        Console.Write("roz: "); int day = int.Parse(Console.ReadLine());
                        manager.DailyBalance(year, month, day);
                    }
                }
                else if (input == "5")
                {
                    Console.WriteLine("khoroj...");
                    break;
                }
                else
                {
                    Console.WriteLine("gheire eshtebahe ast!");
                }
            }
        }
    }
}