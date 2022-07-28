namespace HomeLogging
{
    public interface ILoggerWrapperConfig
    {
        bool AddDateTimeToFilesNames { get; set; }
        string ConfigName { get; set; }
        string ErrorFileName { get; set; }
        string InfoFileName { get; set; }
        string PathToLogFiles { get; set; }
        string TraceFileName { get; set; }
    }
}