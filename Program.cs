using Spectre.Console;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public static class Program
{
    private static AccountJSON? loggedInUser = null;
    public static void Main(string[] args)
    {
      while (true)
      {
        if (loggedInUser == null)
        {
          ShowMainMenu();
        }
        else
        {
          ShowLoggedInMenu();
        }
      }
    }
      public static void ShowMainMenu()
      {
        var options =  AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
                .AddChoices(new[] 
                {
                    "Log in to account",
                    "Create new account",
                    "Information about project",
                }));
    switch(options)
    {
      case "Log in to account":
        Login();
        break;
      case "Create new account":
        CreateAccount();
        break;
      case "Information about project":
        AnsiConsole.WriteLine("");
        break;
    }
    }
        public static void ShowLoggedInMenu()
    {
        var options = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Welcome {loggedInUser.FirstName}, what would you like to do?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
                .AddChoices(new[] {
                    "Deposit money",
                    "Withdraw money",
                    "Log out",
                }));

        switch (options)
        {
            case "Deposit money":
                Deposit();
                break;
            case "Withdraw money":
                Withdraw();
                break;
            case "Log out":
                LogOut();
                break;
        }
    }


    public static void Login()
    {
        var uid = AnsiConsole.Ask<string>("Enter [green]uid[/]:");
        var password = SHA256Encrypt(AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]password[/]?") .PromptStyle("red").Secret('*')));
        string jsonData = File.ReadAllText("users.json");
        AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

        foreach (var item in ob.accounts)
        {
            if (item.uid == uid && item.password == password) 
            {
              AnsiConsole.WriteLine("Logged in!");
              loggedInUser = item;
              ShowAccountDetails(item);
              return;

            }
        }

        Console.WriteLine("Password or UID are incorrect.");
        // Zamiast return mozemy tutaj dac zapetlenie sie funkcji login, zeby mozna powtorzyc logowanie.
        return;
    }
    public static void LogOut()
    {
        if (loggedInUser != null)
        {
            AnsiConsole.WriteLine($"User {loggedInUser.uid} logged out.");
            loggedInUser = null;
        }
        else
        {
            AnsiConsole.WriteLine("No user is currently logged in.");
        }
    }

//void na tworzenie konta - M
//trzeba zrobić zasadę bo jak są 2 takie same id to nadpisuje. 
    public static void CreateAccount()
    {

      AnsiConsole.WriteLine("Welcome! Please enter the information to create account with us");
      AnsiConsole.WriteLine("Upon entry of each required information, please click Enter");
      
      AnsiConsole.WriteLine("Create User ID");
      var uid = AnsiConsole.Ask<string>("Enter [green]uid[/]:");
      

      AnsiConsole.WriteLine("Create password");
      var password = SHA256Encrypt(AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]password[/]?") .PromptStyle("red").Secret('*')));
    

      AnsiConsole.WriteLine("Please provide us with your first, last name and date of birth.");

      var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
      var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

//naming sucks ;)
        var birthDateString = AnsiConsole.Ask<string>("Enter [green]birth date[/] (format: dd-mm-yyyy):");
        var birthDate = DateTime.ParseExact(birthDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        string birthDateOnly = birthDate.ToString("dd-MM-yyyy");
        
        var balance = AnsiConsole.Ask<decimal>("Enter [green]initial balance[/]:");
        var creationDate = DateTime.Now;
      //var birthDateString = AnsiConsole.Ask<string>("Enter [green]birth date[/] (format: dd-mm-yyyy):");
      //var birthDate = DateTime.ParseExact(birthDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
      //string birthDateOnly = birthDate.ToString("dd-MM-yyyy");
     // Kod trzeba poprawić bo date of birth przy formacie 01-01-03 wyrzuca exception, póki co tylko godzine poprawiam.

    // godzina naprawiona, nie testowałem exceptions - na nowych kontach będzie się wyświetlać ok.

      AnsiConsole.WriteLine("Press enter to continue.");
      Console.ReadKey();

        string jsonData = File.ReadAllText("users.json");
        AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

        // Dodajemy nowe konto do listy, 
        ob.accounts = ob.accounts.Append(new AccountJSON { uid = uid, password = password, FirstName = firstName, LastName = lastName, DateOfBirth = birthDateOnly, Balance = balance, 
        CreationDate = creationDate}).ToArray();
      
      //Nie zmienia .jsona w jedna linie yipiee!!!
      var options = new JsonSerializerOptions
      { 
        WriteIndented = true
      };

      File.WriteAllText("users.json", JsonSerializer.Serialize(ob, options));
      }
    

    public static void ShowAccountDetails(AccountJSON account)
    {
      AnsiConsole.WriteLine("Press Enter.");
      Console.ReadKey();
    
    // Wyświetla informacje o koncie, po tym można pokazać listę opcji.
      AnsiConsole.WriteLine($"UserID: {account.uid}");
      AnsiConsole.WriteLine($"Imię: {account.FirstName}");
      AnsiConsole.WriteLine($"Nazwisko: {account.LastName}");
      AnsiConsole.WriteLine($"Data urodzenia: {account.DateOfBirth}");
      AnsiConsole.WriteLine($"Saldo konta: {account.Balance}");
      AnsiConsole.WriteLine($"Data utworzenia konta: {account.CreationDate}");
      
      // Trzeba dodac do tego pliku json, jakies pola typu saldo konta etc. 
      // Plus listę opcji z których dalej można wybierać, czyli poza info np mamy Listę opcji wyloguj, zrób przelew etc etc. - M
    }
  public static void Deposit()
    {
        if (loggedInUser != null)
        {
            decimal amount = AnsiConsole.Ask<decimal>("Enter amount to deposit:");
            loggedInUser.Balance += amount;

            UpdateUserInJson(loggedInUser);

            AnsiConsole.WriteLine($"Deposit of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
        }
    }
  public static void Withdraw()
    {
        if (loggedInUser != null)
        {
            decimal amount = AnsiConsole.Ask<decimal>("Enter amount to withdraw:");
            if (amount <= loggedInUser.Balance)
            {
                loggedInUser.Balance -= amount;

                UpdateUserInJson(loggedInUser);

                AnsiConsole.WriteLine($"Withdrawal of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
            }
            else
            {
                AnsiConsole.WriteLine("Insufficient funds.");
            }
        }
    }
    public static void UpdateUserInJson(AccountJSON updatedAccount)
    {
        string jsonData = File.ReadAllText("users.json");
        AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

        for (int i = 0; i < ob.accounts.Length; i++)
        {
            if (ob.accounts[i].uid == updatedAccount.uid)
            {
                ob.accounts[i] = updatedAccount;
                break;
            }
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText("users.json", JsonSerializer.Serialize(ob, options));
    }

    static string SHA256Encrypt(string password)
    {
        // Hashowanie hasla.
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
              builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    } 

}

// Wczytanie tabeli Accounts z JSON
public class AccountsJSON
{
  public AccountJSON[] accounts { get; set; } = Array.Empty<AccountJSON>();  
}

// Wczytanie pol uid i password z JSON
public class AccountJSON {
  public string uid { get; set; } = string.Empty;
  public string password { get; set; } = string.Empty;

//Nowe stringi z DateOfBirth będę bawił się poźniej
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string DateOfBirth { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreationDate { get; set; }

  public AccountJSON() { }
}



/* Login info

1.123456 - abc
2.789012 - cba - ten nie działa
3.111222333 - InneHaslo
*/ 