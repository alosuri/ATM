using Spectre.Console;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Globalization;

// Zrobione:
// Dodałem tabelkę z historią transakcji - Rafał
// Naprawa systemu logowania (Pokazuje, że hasło lub login jest zły) - Rafał
// Proceed/Resign przy wpłacie/wypłacie/przelewach - Rafał 
// Wyjście z aplikacji w menu głównym - Rafał
// W TS From/To wyświetla teraz użytkowników w dobrej kolejnosci - Mikołaj
// Exception na datę postawione w pętli. - Mikołaj
// Transfery nie przechodzą jeśli UID nie istnieje, wyświetla wiadomość i daje użytkownikowi opcje aby zrezygnować albo kontynouwać (użyłem funkcji z loginu) - Mikołaj
// Informacje o projekcie - Oliwia
// Potwierdzenie czy na pewno chcesz stworzyć konto. - Rafał
// Błąd z historią transakcji (wyświetlało się czy napewno chcesz wykonać przelew) - Rafał
// BUG - możesz sobie sam wysłać pieniądze transferem - Rafał

// Wydaje mi się że niektóre kolorki znikneły ( Mikołaj) - U mnie działa 👍👍👍👍👍 - Rafał

// Do zrobienia:
// TODO: Ten błąd z ob. Ale to można go nie wyświetlać po prostu chyba. // Błąd z ob na sam koniec zróbmy bo mogę po prostu wygenerować blokowanie z VS ale dużo tego doda (wiec na ostatni commit idealnie) (MIkołaj)


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
    var panel = new Panel("Welcome to the banking app. Log in to the transaction service or create a new account.");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel);

    var options = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\nWhat would you like to do?")
            .PageSize(4)
            .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
            .AddChoices(new[]
            {
                    "Log in to account",
                    "Create new account",
                    "Information about project",
          "Exit"
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
        Console.Clear();
        ShowProjectInfo();
        break;
      case "Exit":
        Console.Clear();
        System.Environment.Exit(1);
        break;
    }
  }
 public static void ShowProjectInfo()
{
    var panel = new Panel("The aim of the project is to create a simple banking application that allows users to create bank accounts, log in to their accounts, check their balance, check history of transactions, and perform basic financial operations such as deposits and withdrawals,transactions.\n\nCreated by: [cyan1]Rafal Suchorski, Mikolaj Szymanowicz, Oliwia Szaforz[/]");
    panel.Border = BoxBorder.Rounded;
    panel.Padding = new Padding(1, 1, 1, 1);
    AnsiConsole.Write(panel);

    AnsiConsole.Markup("\n[blue]Press any key to return to the main menu.[/]");
    Console.ReadKey();
    Console.Clear();
    ShowMainMenu(); 
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
                    "Transaction history",
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
      case "Transaction history":
        TransHistory();
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

    var options = AnsiConsole.Prompt(
      new SelectionPrompt<string>()
        .Title("\n[red]Password or UID are incorrect.[/]")
        .PageSize(3)
        .MoreChoicesText("[grey](Choose one of options.)[/]")
        .AddChoices(new[] {
        "Retry",
        "Back"
        }));

    switch (options)
    {
      case "Retry":
        Console.Clear();
        Login();
        break;
      case "Back":
        Console.Clear();
        ShowMainMenu();
        break;
    }
    return;
  }

  public static void LogOut()
  {
    if (loggedInUser != null)
    {
      Console.Clear();
      AnsiConsole.WriteLine($"User {loggedInUser.Uid} logged out.");
      loggedInUser = null;
      Console.ReadKey();// rozwiazuje problem braku informacji - kazdy console read key zaaplikowany ma taka sama funkcje - nulyfikuje problem z C.Clear()
    }
    else
    {
      AnsiConsole.Markup("[blue]No user is currently logged in. Press any key to continue...[/]");
      Console.ReadKey();// rozwiazuje problem braku informacji - kazdy console read key zaaplikowany ma taka sama funkcje - nulyfikuje problem z C.Clear()
    }
  }
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

    string birthDateString;
    DateTime birthDate;
  do
  {
    try
    {
        birthDateString = AnsiConsole.Ask<string>("Enter [green]birth date[/] (format: dd-MM-yyyy):");
        birthDate = DateTime.ParseExact(birthDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        break;
    }
    catch (FormatException)
      {
        AnsiConsole.Markup("[red]Invalid date format.[/]\n[blue]Press any button to try again.\n[/]");
        Console.ReadKey();
      }
    } 
      while (true);

    string birthDateOnly = birthDate.ToString("dd-MM-yyyy");

    var balance = AnsiConsole.Ask<decimal>("Enter [green]initial balance[/]:");
    var creationDate = DateTime.Now;

    var password = SHA256Encrypt(AnsiConsole.Prompt(new TextPrompt<string>("Enter [green]password[/]:").PromptStyle("red").Secret('*')));


    Random code = new();
    string uid = Convert.ToString(firstName[0]) + Convert.ToString(lastName[0]) + Convert.ToString(code.Next(10000, 99999));

	var confirm = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title($"\nAre you sure you want to create an account?")
            .PageSize(3)
            .AddChoices(new[] {
                    "Proceed",
                    "Resign"
            }));

    switch (confirm)
    {
      case "Proceed":
		// Dodajemy nowe konto do listy, 
		ob.accounts = ob.accounts.Append(new AccountJSON
		{
		  Uid = uid,
		  Password = password,
		  FirstName = firstName,
		  LastName = lastName,
		  DateOfBirth = birthDateOnly,
		  Balance = balance,
		  CreationDate = creationDate,
		  Transactions = new List<Transaction>()


		}).ToArray();

		//Nie zmienia .jsona w jedna linie yipiee!!!
		var options = new JsonSerializerOptions
		{
		  WriteIndented = true
		};

    	File.WriteAllText("users.json", JsonSerializer.Serialize(ob, options));
        break;
      case "Resign":
		Console.Clear();
        ShowMainMenu();
        break;
    }


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
  }

  public static void Deposit()
  {
    if (loggedInUser != null)
    {
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to deposit:");
      var options = AnsiConsole.Prompt(
      new SelectionPrompt<string>()
        .Title("\n[green]Are you sure?[/]")
        .PageSize(3)
        .MoreChoicesText("[grey](Choose one of options.)[/]")
        .AddChoices(new[] {
        "Proceed",
        "Resign"
        }));

      switch (options)
      {
        case "Proceed":
          loggedInUser.Balance += amount;

          loggedInUser.Transactions.Add(new Transaction { Type = "Deposit", Amount = amount, Date = DateTime.Now }); // Funckja dla transakcji


          UpdateUserInJson(loggedInUser);



          // To się chyba nie wyświetla przez Console.Clear(), ale zmęczony jestem, więc zrobię to później 👍👍👍👍
          AnsiConsole.WriteLine($"Deposit of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
          Console.ReadKey();
          AnsiConsole.Markup("[blue]Press any button to return to main menu[/]");
          break;
        case "Resign":
          Console.Clear();
          ShowLoggedInMenu();
          break;
      }



    }
  }
  public static void Withdraw()
  {
    if (loggedInUser != null)
    {
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to withdraw:");
      var options = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
      .Title("\n[green]Are you sure?[/]")
      .PageSize(3)
      .MoreChoicesText("[grey](Choose one of options.)[/]")
      .AddChoices(new[] {
        "Proceed",
        "Resign"
      }));

      switch (options)
      {
        case "Proceed":
          if (amount <= loggedInUser.Balance)
          {
            loggedInUser.Balance -= amount;

            loggedInUser.Transactions.Add(new Transaction { Type = "Withdraw", Amount = amount, Date = DateTime.Now }); // Funkcja Transakcji

            UpdateUserInJson(loggedInUser);


            // To się chyba nie wyświetla przez Console.Clear(), ale zmęczony jestem, więc zrobię to później 👍👍👍👍 - naprawione 2 linie nizej
            AnsiConsole.WriteLine($"Withdrawal of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
            Console.ReadKey();// rozwiazuje problem
            AnsiConsole.Markup("[blue]Press any button to return to main menu[/]");
          }
          else
          {
            AnsiConsole.WriteLine("Insufficient funds.");
            Console.ReadKey();// rozwiazuje problem braku informacji - kazdy console read key zaaplikowany ma taka sama funkcje - nulyfikuje problem z C.Clear()
		  	AnsiConsole.Markup("[blue]Press any button to return to main menu[/]");
          }

          break;
        case "Resign":
          Console.Clear();
          ShowLoggedInMenu();
          break;
      }

    }
  }




  public static void Transfer()
  {

    if (loggedInUser != null)
    {

      string uid = AnsiConsole.Ask<string>("\nEnter the recipient's UID:");
      decimal amount = AnsiConsole.Ask<decimal>("\nEnter amount to transfer:");
      var options = AnsiConsole.Prompt(
      new SelectionPrompt<string>()
        .Title("\n[green]Are you sure?[/]")
        .PageSize(3)
        .MoreChoicesText("[grey](Choose one of options.)[/]")
        .AddChoices(new[] {
        "Proceed",
        "Resign"
        }));

      switch (options)
      {
        case "Proceed":
          if (amount <= loggedInUser.Balance)
          {
            string jsonData = File.ReadAllText("users.json");
            AccountsJSON? ob = JsonSerializer.Deserialize<AccountsJSON>(jsonData);
            
            foreach (var item in ob.accounts)
            {
              if (item.Uid == uid && item.Uid != loggedInUser.Uid)
              {
                item.Balance += amount;
                loggedInUser.Balance -= amount;

                item.Transactions.Add(new Transaction { RecipientFrom = loggedInUser.Uid, RecipientTo = item.Uid, Type = "Received", Amount = amount, Date = DateTime.Now });
                loggedInUser.Transactions.Add(new Transaction { RecipientFrom = loggedInUser.Uid, RecipientTo = item.Uid, Type = "Transfer", Amount = amount, Date = DateTime.Now });

                UpdateUserInJson(loggedInUser);
                UpdateUserInJson(item);

                AnsiConsole.WriteLine($"Transfer of {amount} PLN successful. Current balance: {loggedInUser.Balance} PLN");
                AnsiConsole.Markup("[blue]Press any button to return to main menu.[/]");
                Console.ReadKey();
                
                ShowLoggedInMenu();

				return;
              }
            }
          
                
                var IncorrectUid = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("\n[red] This UID is incorrect, please try again.[/]")
                .PageSize(3)
                .MoreChoicesText("[grey](Choose one of options.)[/]")
                .AddChoices(new[] {
                "Retry",
                "Back"
                }));

                  switch (IncorrectUid)
                {
                    case "Retry":
                    // usunąłem console clear btw.
					// spoko
                    Transfer();
                    break;

                    case "Back":
                    Console.Clear();
                    ShowLoggedInMenu();
                    break;
                }
              }     
          

          else
            {
              var IncorrectUid = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("\n[red]Insufficient funds, please try again.[/]")
                .PageSize(3)
                .AddChoices(new[] {
                "Retry",
                "Back"
                }));

                  switch (IncorrectUid)
                {
                    case "Retry":
                    // usunąłem console clear btw.
					// spoko
                    Transfer();
                    break;

                    case "Back":
                    Console.Clear();
                    ShowLoggedInMenu();
                    break;
                }
            }
          break;
        
        case "Resign":
          Console.Clear();
          ShowLoggedInMenu();
          break;
      }
    }
  }
  

 

// Historia transakcji + tabela
  public static void TransHistory()
  {
    if (loggedInUser != null)
    {
      Console.Clear();
      AnsiConsole.WriteLine("\nTransaction History:");


      if (loggedInUser.Transactions.Count > 0)
      {
        var table = new Table();
        table.Border(TableBorder.Rounded);
        AnsiConsole.Live(table)
        .Start(ctx =>
        {
          table.AddColumn(new TableColumn("From/To"));
          ctx.Refresh();
          Thread.Sleep(50);
          table.AddColumn(new TableColumn("Type"));
          ctx.Refresh();
          Thread.Sleep(50);
          table.AddColumn(new TableColumn("Amount"));
          ctx.Refresh();
          Thread.Sleep(50);
          table.AddColumn(new TableColumn("Date"));
          ctx.Refresh();
          Thread.Sleep(50);

          foreach (var transaction in loggedInUser.Transactions) // Zamieniłem Recipient From/To bo było odwrócone
          {
            if (transaction.Type == "Transfer") table.AddRow($"[red]{transaction.RecipientFrom} > {transaction.RecipientTo}[/]", $"[red]{transaction.Type}[/]", $"[red]{transaction.Amount} PLN[/]", $"[red]{transaction.Date}[/]");

            else if (transaction.Type == "Withdraw") table.AddRow($"[red]-[/]", $"[red]{transaction.Type}[/]", $"[red]{transaction.Amount} PLN[/]", $"[red]{transaction.Date}[/]");

            else if (transaction.Type == "Deposit") table.AddRow($"[green]-[/]", $"[green]{transaction.Type}[/]", $"[green]{transaction.Amount} PLN[/]", $"[green]{transaction.Date}[/]");

            else if (transaction.Type == "Received") table.AddRow($"[green]{transaction.RecipientFrom} > {transaction.RecipientTo}[/]", $"[green]{transaction.Type}[/]", $"[green]{transaction.Amount} PLN[/]", $"[green]{transaction.Date}[/]");

            ctx.Refresh();
            Thread.Sleep(50);
          }

        });

      }

      else
      {
        AnsiConsole.Markup("[gray]No data.[/]\n");
      }

      AnsiConsole.Markup("[blue]Press any button to return to main menu.[/]");
      Console.ReadKey(); // rozwiazuje problem z wyswietlaniem historii transakcji

    }
  }


//Update pliku .json
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
// klasa pomocnicza do transakcji
public class Transaction
{
  public required string Type { get; set; } // Dawało wartość null dlatego "required"
  public decimal Amount { get; set; }
  public DateTime Date { get; set; }
  public string RecipientTo { get; set; } = string.Empty;
  public string RecipientFrom { get; set; } = string.Empty;
}

// Wczytanie tabeli Accounts z JSON
public class AccountsJSON
{
  public AccountJSON[] accounts { get; set; } = Array.Empty<AccountJSON>();
}

// Wczytanie pol uid i password z JSON
public class AccountJSON
{
  public List<Transaction> Transactions { get; set; } = new List<Transaction>();
  public string Uid { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string DateOfBirth { get; set; } = string.Empty;
  public decimal Balance { get; set; }
  public DateTime CreationDate { get; set; }
}
