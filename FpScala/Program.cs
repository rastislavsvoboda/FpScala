using FpScala.Exercises.Action.Imperative;

var console = new SystemConsole();
var clock = new SystemClock();
var service = new UserCreationService(console, clock);

var user = service.ReadUser();

// Console.ReadLine();