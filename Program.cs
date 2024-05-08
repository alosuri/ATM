using Spectre.Console;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

public static class Program
{
    public static void Main(string[] args)
    {
        var options =  AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to choose an option.)[/]")
                .AddChoices(new[] {
                    "Log in to account",
                    "Create new account",
                    "Informations about project",
                }));
        if (options == "Log in to account")
        {
            Login();
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
            if (item.uid == uid && item.password == password) {
              AnsiConsole.WriteLine("Logged in!");
              ShowAccountDetails();
              return;
            }
        }

        Console.WriteLine("Password or UID are incorrect.");
        // Zamiast return mozemy tutaj dac zapetlenie sie funkcji login, zeby mozna powtorzyc logowanie.
        return;
    }

    public static void ShowAccountDetails() {
      AnsiConsole.WriteLine("Press Enter.");
      Console.ReadKey();
      // Trzeba dodac do tego pliku json, jakies pola typu saldo konta etc. 
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
  public AccountJSON[] accounts { get; set; } = [];  
}

// Wczytanie pol uid i password z JSON
public class AccountJSON {
  public string uid { get; set; } = string.Empty;
  public string password { get; set; } = string.Empty;
}

