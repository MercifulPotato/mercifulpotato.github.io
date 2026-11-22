namespace ObserverMagazine.Web.Services;

public static class GlobalExceptionLogging
{
    public const string CategoryName = "ObserverMagazine.Web.UnhandledExceptions";

    public static void Register(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(CategoryName);

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            logger.LogCritical(args.ExceptionObject as Exception,
                "Unhandled exception (runtime terminating: {IsTerminating})", args.IsTerminating);

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            logger.LogError(args.Exception, "Unobserved exception in a background task");
            args.SetObserved();
        };
    }
}
