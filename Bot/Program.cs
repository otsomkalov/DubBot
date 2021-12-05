namespace Bot;

public static class Program
{
    public static Task Main(string[] args)
    {
        return CreateWebHostBuilder(args)
            .Build()
            .RunAsync();
    }

    private static IHostBuilder CreateWebHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}