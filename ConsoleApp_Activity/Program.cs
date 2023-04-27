using Serilog;
using Serilog.Context;
using System.Diagnostics;
using ColorExtension;
using System.Drawing;

namespace ConsoleApp_Activity
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                           .Enrich.FromLogContext()
                           .WriteTo.Console(outputTemplate:
@"[{Timestamp:HH:mm:ss} {Level:u3}] 
Message: {Message:lj}
CorrelationId: {CorrelationId}
TraceId: {TraceId}
SpanId: {SpanId}
{NewLine}").CreateLogger();

            await LightweighCorrelationId();


            Log.CloseAndFlush();
        }

        private static async Task LightweighCorrelationId()
        {
            var t1 = Deep1("t1", true);

            var t2 = Deep1("t2", false);

            await Task.WhenAll(t1, t2);
        }

        private static async Task Deep1(string taskN, bool colorIt)
        {
            using var activity = new Activity("");
            activity.Start();
            using var _ = LogContext.PushProperty("CorrelationId", Activity.Current?.Id);
            using var __ = LogContext.PushProperty("TraceId", Activity.Current?.TraceId);
            using var ___ = LogContext.PushProperty("SpanId", Activity.Current?.SpanId);

            Log.Logger.BkColor("Deep1 started {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundYellow : ColorExtension.LoggerExtensions.BackgroundBrightYellow);
            SomethingInsideMe(taskN, colorIt);

            await Task.Delay(1000);
            await Deep2(taskN, colorIt);
            Log.Logger.BkColor("Deep1 finished {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundYellow : ColorExtension.LoggerExtensions.BackgroundBrightYellow);
        }

        private static void SomethingInsideMe(string taskN, bool colorIt)
        {
            using var activity = new Activity("");
            activity.Start();
            using var _ = LogContext.PushProperty("CorrelationId", Activity.Current.Id);
            using var __ = LogContext.PushProperty("TraceId", Activity.Current?.TraceId);
            using var ___ = LogContext.PushProperty("SpanId", Activity.Current?.SpanId);

            Log.Logger.BkColor("Inside started {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundRed : ColorExtension.LoggerExtensions.BackgroundBrightRed);
            Log.Logger.BkColor("Inside finished {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundRed : ColorExtension.LoggerExtensions.BackgroundBrightRed);
        }

        private static async Task Deep2(string taskN, bool colorIt)
        {
            Log.Logger.BkColor("Deep2 started {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundYellow : ColorExtension.LoggerExtensions.BackgroundBrightYellow);
            await Task.Delay(1000);
            await Deep3(taskN, colorIt);
            Log.Logger.BkColor("Deep2 finished {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundYellow : ColorExtension.LoggerExtensions.BackgroundBrightYellow);
        }

        private static async Task Deep3(string taskN, bool colorIt)
        {
            using var activity = new Activity("");
            activity.Start();
            using var _ = LogContext.PushProperty("CorrelationId", Activity.Current?.Id);
            using var __ = LogContext.PushProperty("TraceId", Activity.Current?.TraceId);
            using var ___ = LogContext.PushProperty("SpanId", Activity.Current?.SpanId);

            Log.Logger.BkColor("Deep3 started {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundMagenta : ColorExtension.LoggerExtensions.BackgroundBrightMagenta);
            await Task.Delay(1000);
            await Deep4(taskN, colorIt);
            Log.Logger.BkColor("Deep3 finished {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundMagenta : ColorExtension.LoggerExtensions.BackgroundBrightMagenta);
        }

        private static async Task Deep4(string taskN, bool colorIt)
        {
            Log.Logger.BkColor("Deep4 started {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundMagenta : ColorExtension.LoggerExtensions.BackgroundBrightMagenta);
            await Task.Delay(1000);
            Log.Logger.BkColor("Deep4 finished {TaskN}", taskN, colorIt ? ColorExtension.LoggerExtensions.BackgroundMagenta : ColorExtension.LoggerExtensions.BackgroundBrightMagenta);
        }
    }
}