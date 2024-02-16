namespace FpScala.Exercises.Action.Imperative;

using static Console;

public static class UserCreationApp
{
    public static User ReadUser()
    {
        WriteLine("What's your name?");
        var name = ReadLine();
        WriteLine("What's your date of birth? [dd-mm-yyyy]");
        var dateOfBirth = DateOnly.ParseExact(ReadLine(), "dd-MM-yyyy");
        var now = DateTime.Now;
        var user = new User(name, dateOfBirth, now);
        WriteLine($"User is ${user}");
        return user;
    }

    public static bool ReadSubscribeToMailingList()
    {
        WriteLine("Would like to subscribe to our mailing list? [Y/N]");
        var line = ReadLine();
        return ParseYesNo(line);
    }

    public static bool ParseYesNo(string? line) =>
        line switch
        {
            "Y" => true,
            "N" => false,
            _ => throw new ArgumentException($"""Expected "Y" or "N" but received {line}""")
        };
}