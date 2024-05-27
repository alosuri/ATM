using Spectre.Console;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Globalization;

// ShowAccountsDetails - Przenioslem te funkcje do ShowLoggedInMenu. - Rafał

public static class Program
{
  private static AccountJSON? loggedInUser = null;
  public static void Main(string[] args)
  {
    while (true)
    {
      if (loggedInUser == null)
      {
        Console.Clear();
        ShowMainMenu();
      }
      else
      {
        Console.Clear();
        ShowLoggedInMenu();
      }
    }
  }
  public static void ShowMainMenu()
  {
    var panel = new Panel("Tutaj trzeba dac jakis tekst na powitanie, ale na chwile obecna nie mam pomyslu.");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel);

    var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\nWhat would you like to do?")
            .PageSize(3)
            .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
            .AddChoices(new[]
            {
                    "Log in to account",
                    "Create new account",
                    "Information about project",
            }));
    switch (options)
    {
      case "Log in to account":
        Console.Clear();
        Login();
        break;
      case "Create new account":
        Console.Clear();
        CreateAccount();
        break;
      case "Information about project":
        AnsiConsole.WriteLine("");
        break;
    }
  }
  public static void ShowLoggedInMenu()
  {
    Console.Clear();


    var panel = new Panel($"[springgreen3_1]Name:[/] {loggedInUser?.FirstName + ' ' + loggedInUser?.LastName}\n[springgreen2]UID:[/] {loggedInUser?.Uid}\n[cyan3]Birth date:[/] {loggedInUser?.DateOfBirth}\n[darkturquoise]Account created on:[/] {loggedInUser?.CreationDate}\n\n[turquoise2]Account balance:[/] {loggedInUser?.Balance} ZŁ");
    panel.Header = new PanelHeader("[green3_1] Account details [/]");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(2, 2, 2, 2);

    AnsiConsole.Write(panel);
    var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title($"\nWelcome {loggedInUser?.FirstName}, what would you like to do?")
            .PageSize(10)
            .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
            .AddChoices(new[] {
                    "Deposit money",
                    "Withdraw money",
          "Transfer money",
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
      case "Transfer money":
        Transfer();
        break;
      case "Log out":
        LogOut();
        break;
    }
  }


  public static void Login()
  {
    var panel = new Panel("Please enter your [green]User ID[/] and [green]password[/] to access your account.");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel);
    Console.Write("\n");

    var uid = AnsiConsole.Ask<string>("Enter [green]user ID[/]:");
    var password = SHA256Encrypt(AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]password[/]?").PromptStyle("red").Secret('*')));
    string jsonData = File.ReadAllText("users.json");
    AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

    foreach (var item in ob.accounts)
    {
      if (item.Uid == uid && item.Password == password)
      {
        AnsiConsole.WriteLine("Logged in!");
        loggedInUser = item;
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
      Console.Clear();
      AnsiConsole.WriteLine($"User {loggedInUser.Uid} logged out.");
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
    string jsonData = File.ReadAllText("users.json");
    AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

    var panel = new Panel("Welcome! Please enter the information to create account with us.\nUpon entry of each required information, please click Enter.\nPlease provide us with your first, last name and date of birth.");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel);
    Console.Write("\n");


    var firstName = AnsiConsole.Ask<string>("Enter [green]first name[/]:");
    var lastName = AnsiConsole.Ask<string>("Enter [green]last name[/]:");

    //naming sucks ;)
    var birthDateString = AnsiConsole.Ask<string>("Enter [green]birth date[/] (format: dd-mm-yyyy):");
    var birthDate = DateTime.ParseExact(birthDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
    string birthDateOnly = birthDate.ToString("dd-MM-yyyy");

    var balance = AnsiConsole.Ask<decimal>("Enter [green]initial balance[/]:");
    var creationDate = DateTime.Now;

    var password = SHA256Encrypt(AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]password[/]:").PromptStyle("red").Secret('*')));


    //var birthDateString = AnsiConsole.Ask<string>("Enter [green]birth date[/] (format: dd-mm-yyyy):");
    //var birthDate = DateTime.ParseExact(birthDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
    //string birthDateOnly = birthDate.ToString("dd-MM-yyyy");
    // Kod trzeba poprawić bo date of birth przy formacie 01-01-03 wyrzuca exception, póki co tylko godzine poprawiam.

    // godzina naprawiona, nie testowałem exceptions - na nowych kontach będzie się wyświetlać ok.

    Random code = new();
    string uid = Convert.ToString(firstName[0]) + Convert.ToString(lastName[0]) + Convert.ToString(code.Next(10000, 99999));

    Console.Clear();

    var panel2 = new Panel($"Your user ID: {uid}");
    panel2.Border = BoxBorder.Rounded;
    panel2.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel2);

    foreach (var item in ob.accounts)
    {
      if (item.Uid == uid)
      {
        uid = Convert.ToString(firstName[0]) + Convert.ToString(lastName[0]) + Convert.ToString(code.Next(10000, 99999));
      }
    }


    AnsiConsole.WriteLine("\nPress enter to continue.");
    Console.ReadKey();

    // Dodajemy nowe konto do listy, 
    ob.accounts = ob.accounts.Append(new AccountJSON
    {
      Uid = uid,
      Password = password,
      FirstName = firstName,
      LastName = lastName,
      DateOfBirth = birthDateOnly,
      Balance = balance,
      CreationDate = creationDate
    }).ToArray();

    //Nie zmienia .jsona w jedna linie yipiee!!!
    var options = new JsonSerializerOptions
    {
      WriteIndented = true
    };

    File.WriteAllText("users.json", JsonSerializer.Serialize(ob, options));
  }

  public static void Deposit()
  {
    if (loggedInUser != null)
    {
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to deposit:");
      loggedInUser.Balance += amount;

      UpdateUserInJson(loggedInUser);

      // To się chyba nie wyświetla przez Console.Clear(), ale zmęczony jestem, więc zrobię to później 👍👍👍👍
      AnsiConsole.WriteLine($"Deposit of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
    }
  }
  public static void Withdraw()
  {
    if (loggedInUser != null)
    {
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to withdraw:");
      if (amount <= loggedInUser.Balance)
      {
        loggedInUser.Balance -= amount;

        UpdateUserInJson(loggedInUser);

        // To się chyba nie wyświetla przez Console.Clear(), ale zmęczony jestem, więc zrobię to później 👍👍👍👍
        AnsiConsole.WriteLine($"Withdrawal of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
      }
      else
      {
        AnsiConsole.WriteLine("Insufficient funds.");
      }
    }
  }

  public static void Transfer()
  {
    if (loggedInUser != null)
    {
      string uid = AnsiConsole.Ask<string>("\nEnter the recipient's UID:");
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to transfer:");
      if (amount <= loggedInUser.Balance)
      {
        string jsonData = File.ReadAllText("users.json");
        AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);

        foreach (var item in ob.accounts)
        {
          if (item.Uid == uid)
          {
            item.Balance += amount;
            loggedInUser.Balance -= amount;

            UpdateUserInJson(loggedInUser);
            UpdateUserInJson(item);

            break;
          }
        }
        // To się chyba nie wyświetla przez Console.Clear(), ale zmęczony jestem, więc zrobię to później 👍👍👍👍
        AnsiConsole.WriteLine($"Transfer of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
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
      if (ob.accounts[i].Uid == updatedAccount.Uid)
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
public class AccountJSON
{
  public string Uid { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;

  //Nowe stringi z DateOfBirth będę bawił się poźniej
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string DateOfBirth { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreationDate { get; set; }

  public AccountJSON() { }
}