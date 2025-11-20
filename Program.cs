using xmle.Commands;

namespace xmle;

public class Program
{
    public static async Task Main(string[] args)
    {
        var app = new AppCommand();
        await app.Parse(args).InvokeAsync();
    }
}
