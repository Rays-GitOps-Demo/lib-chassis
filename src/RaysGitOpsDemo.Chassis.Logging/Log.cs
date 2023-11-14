namespace RaysGitOpsDemo.Chassis.Logging
{
    /// <summary>
    /// A static class exposing global operations that can be performed on the logging subsystem.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Immediately flush any and all log entries that have not been dumped into sinks yet.  This should 
        /// usually be called prior to application termination to ensure no long entries are lost.
        /// </summary>
        public static void CloseAndFlush()
        {
            Serilog.Log.CloseAndFlush();
        }
    }
}