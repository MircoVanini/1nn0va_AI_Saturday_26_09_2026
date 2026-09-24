
using DemoDiagnostic;

PrintMenu();

ConsoleKeyInfo keyReaded = Console.ReadKey();
Console.WriteLine();

switch (keyReaded.Key)
{
    case ConsoleKey.D1:
        NullReferenceException();
        break;

    case ConsoleKey.D2:
        OutOfMemoryException();
        break;

    case ConsoleKey.D3:
        InfiniteRecurse();
        break;

    case ConsoleKey.D4:
        RaceCondition();
        break;

    case ConsoleKey.D5:
        DeadLock().GetAwaiter().GetResult();
        break;

    default:
        Console.WriteLine("Wrong key, please try again.");
        break;
}

Console.WriteLine("Hit any key to exit");
Console.ReadKey();

static void NullReferenceException()
{
    // null reference exception example
    var f = new Foo();
    var name = f.Bar.Baz.Name;

    // breakpoint - tracepoint - local variables examples 
    var items = new List<Product>();
    for (int i = 0; i < 50; i++)
    {
        var product = new Product(i, $"{i}" + ((i % 2 == 0) ? "even" : "odd"), 1 * 1.1);
        items.Add(product);
    }

    // linq query example
    var itemsQuery = (from item in items
                      where item.Name.StartsWith("20")
                      where item.Price > 1.0
                      where item.Id % 2 == 1
                      select item).ToList();

    // quick actions example
    //string a = null;
    //Console.WriteLine("Length of a: " + a.Length);
}

static void OutOfMemoryException()
{
    List<Product> products = new List<Product>();
    string answer = "";
    do
    {
        for (int i = 0; i < 1_000; i++)
        {
            products.Add(new Product(i, "product" + i, 0));
        }
        Console.WriteLine("Leak some more? Y/N");
        answer = Console.ReadLine()?.ToUpper() ?? "";

    } while (answer == "Y");
}

static void InfiniteRecurse(int start = 0)
{
    InfiniteRecurse(++start);
}

static void RaceCondition()
{
    Thread t1 = new Thread(IncrementCounter);
    Thread t2 = new Thread(IncrementCounter);

    t1.Start();
    t2.Start();

    t1.Join();
    t2.Join();

    Console.WriteLine($"Counter final Value: {counter}");
}

static void IncrementCounter()
{
    for (int i = 0; i < 1_000_000; i++)
    {
        counter++; //  NOT atomic operation → race Condition
    }
}

static async Task DeadLock()
{
    List<Task> tasks = new List<Task>();

    count = 0;
    for (int i = 0; i < 5; i++)
    {
        tasks.Add(Task.Run(TaskAcquireOne));
        tasks.Add(Task.Run(TaskAcquireTwo));
    }

    await Task.WhenAll(tasks);
}

static async Task TaskAcquireOne()
{
    await Task.Run(() =>
    {
        int i = count++;
        lock (Seller)
        {
            lock (Order)
            {
                Thread.Sleep(1000);
            }
        }
    });
}

static async Task TaskAcquireTwo()
{
    await Task.Run(() =>
    {
        int i = count++;
        lock (Order)
        {
            lock (Seller)
            {
                Thread.Sleep(1000);
            }
        }
    });
}

static void PrintMenu()
{
    // https://patorjk.com/software/taag/#p=display&f=Graffiti&t=Type+Something+&x=none&v=4&h=4&w=80&we=false

    Console.WriteLine(@"

 ███████████                                         
░░███░░░░░███                                        
 ░███    ░███ █████ ████  ███████  ███████ █████ ████
 ░██████████ ░░███ ░███  ███░░███ ███░░███░░███ ░███ 
 ░███░░░░░███ ░███ ░███ ░███ ░███░███ ░███ ░███ ░███ 
 ░███    ░███ ░███ ░███ ░███ ░███░███ ░███ ░███ ░███ 
 ███████████  ░░████████░░███████░░███████ ░░███████ 
░░░░░░░░░░░    ░░░░░░░░  ░░░░░███ ░░░░░███  ░░░░░███ 
                         ███ ░███ ███ ░███  ███ ░███ 
                        ░░██████ ░░██████  ░░██████  
                         ░░░░░░   ░░░░░░    ░░░░░░   

");

    Console.WriteLine("Hello from DemoDiagnostic !");
    Console.WriteLine("");
    Console.WriteLine("Select the case to show...");
    Console.WriteLine("");

    Console.WriteLine("1) Null reference exceptions.");
    Console.WriteLine("2) GC Heap presssure, OOM Exceptions.");
    Console.WriteLine("3) Stack overflow.");
    Console.WriteLine("4) Race condition.");
    Console.WriteLine("5) Dead Lock.");
}

static public partial class Program
{
    static volatile int count = 0;
    static int counter = 0;

    static object Seller = new();
    static object Order = new();
    static ManualResetEvent resetEvent = new ManualResetEvent(false);
};
