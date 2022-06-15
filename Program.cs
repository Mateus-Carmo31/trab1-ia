using MapPathfinder;

static class Program
{
    public static void Main()
    {
        var app = new Application();
        app.Init();

        while(app.isRunning())
        {
            app.MainLoop();
        }

        app.Close();
    }
}
