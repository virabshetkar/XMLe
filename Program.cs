using XMLe.Commands;

namespace XMLe;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = new AppCommand();
        await app.Parse(args).InvokeAsync();
    }
}
