using System;
using System.Diagnostics;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
//using Serilog.Sinks.Console;

namespace ConsoleApp_AllTrash_Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                           .Enrich.FromLogContext()
                           .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {CorrelationId}] {Message:lj}{NewLine}{Exception}")
                           .CreateLogger();

            // WithTracerProvider();

            // WithoutTracerProvider();

            // WithTracerProviderContext();

            // WithActivityContext();

            LightweighCorrelationId();


            Log.CloseAndFlush();
        }

        private static void WithTracerProvider()
        {
            Log.Information("App started");
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                                .AddSource("ConsoleApp11")
                                .AddSource("ConsoleApp22")
                                .AddConsoleExporter()
                                .Build();

            var activitySource = new ActivitySource("ConsoleApp11");

            var task1 = Task.Run(async () =>
            {
                using var activity = activitySource.StartActivity("Task1");
                // activity?.SetBaggage("CorrelationId", activity?.Context.TraceId.ToString());
                activity?.SetBaggage("CorrelationId", activity?.Context.TraceId.ToString());
                activity?.SetParentId(Activity.Current?.Id);
                activity?.AddTag("Method", "DoWork1");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));

                Log.Information("Task1 started");
                Console.WriteLine("CorrelationId " + activity?.GetBaggageItem("CorrelationId"));
                await Task.Delay(1000);
                activity?.SetBaggage("CorrelationId", "trololo");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));
                Log.Information("Task1 finished");
            });

            var task2 = Task.Run(async () =>
            {
                using var activity = activitySource.StartActivity("Task2");
                activity?.SetCustomProperty("CorrelationId", activity?.Context.TraceId.ToString());
                activity?.SetParentId(Activity.Current?.Id);
                activity?.AddTag("Method", "DoWork2");
                LogContext.PushProperty("CorrelationId", activity?.GetCustomProperty("CorrelationId"));

                Log.Information("Task2 started");
                Console.WriteLine("CorrelationId " + activity?.GetCustomProperty("CorrelationId"));
                await Task.Delay(1000);
                Log.Information("Task2 finished");
            });

            Task.WaitAll(task1, task2);


        }

        private static void WithTracerProviderContext()
        {
            Log.Information("App started");
            var tracerProvider = Sdk.CreateTracerProviderBuilder()
                                .AddSource("ConsoleApp11")
                                .AddSource("ConsoleApp22")
                                .AddConsoleExporter()
                                .Build();

            var activitySource = new ActivitySource("ConsoleApp11");

            var task1 = Task.Run(async () =>
            {
                using var activity = activitySource.StartActivity("Task1");
                activity?.SetBaggage("CorrelationId", Activity.Current?.Id);
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));

                Log.Information("Task1 started");
                await Task.Delay(1000);
                activity?.SetBaggage("CorrelationId", "trololo");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));
                Log.Information("Task1 finished");
            });

            var task2 = Task.Run(async () =>
            {
                using var activity = activitySource.StartActivity("Task2");
                activity?.SetCustomProperty("CorrelationId", Activity.Current?.Id);
                LogContext.PushProperty("CorrelationId", activity?.GetCustomProperty("CorrelationId"));

                Log.Information("Task2 started");
                await Task.Delay(1000);
                Log.Information("Task2 finished");
            });

            Task.WaitAll(task1, task2);
        }

        private static void WithActivityContext()
        {
            var task1 = Task.Run(async () =>
            {
                using var activity = new Activity("Task1");
                activity.Start();
                activity?.SetBaggage("CorrelationId", Activity.Current?.Id);
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));

                Log.Information("Task1 started");
                await Task.Delay(1000);
                activity?.SetBaggage("CorrelationId", "trololo");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));
                Log.Information("Task1 finished");
            });

            var task2 = Task.Run(async () =>
            {
                using var activity = new Activity("Task2");
                activity.Start();
                activity?.SetCustomProperty("CorrelationId", Activity.Current?.Id);
                LogContext.PushProperty("CorrelationId", activity?.GetCustomProperty("CorrelationId"));

                Log.Information("Task2 started");
                await Task.Delay(1000);
                Log.Information("Task2 finished");
            });

            Task.WaitAll(task1, task2);
        }

        private static void LightweighCorrelationId()
        {
            var task1 = Task.Run(async () =>
            {
                using var activity = new Activity("");
                activity.Start();
                LogContext.PushProperty("CorrelationId", Activity.Current?.Id);

                Log.Information("Task1 started");
                await Task.Delay(1000);
                Log.Information("Task1 finished");
            });

            var task2 = Task.Run(async () =>
            {
                using var activity = new Activity("");
                activity.Start();
                LogContext.PushProperty("CorrelationId", Activity.Current?.Id);

                Log.Information("Task2 started");
                await Task.Delay(1000);
                Log.Information("Task2 finished");
            });

            Task.WaitAll(task1, task2);
        }

        private static void WithoutTracerProvider()
        {
            Log.Information("App started");

            var task1 = Task.Run(async () =>
            {
                using var activity = new Activity("Task1");
                // activity?.SetBaggage("CorrelationId", activity?.Context.TraceId.ToString());
                activity?.SetBaggage("CorrelationId", Guid.NewGuid().ToString());
                activity?.SetParentId(Activity.Current?.Id);
                activity?.AddTag("Method", "DoWork1");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));

                Log.Information("Task1 started");
                Console.WriteLine("CorrelationId " + activity?.GetBaggageItem("CorrelationId"));
                await Task.Delay(1000);
                activity?.SetBaggage("CorrelationId", "trololo");
                LogContext.PushProperty("CorrelationId", activity?.GetBaggageItem("CorrelationId"));
                Log.Information("Task1 finished");
            });

            var task2 = Task.Run(async () =>
            {
                using var activity = new Activity("Task2");
                activity?.SetCustomProperty("CorrelationId", Guid.NewGuid().ToString());
                activity?.SetParentId(Activity.Current?.Id);
                activity?.AddTag("Method", "DoWork2");
                LogContext.PushProperty("CorrelationId", activity?.GetCustomProperty("CorrelationId"));

                Log.Information("Task2 started");
                Console.WriteLine("CorrelationId " + activity?.GetCustomProperty("CorrelationId"));
                await Task.Delay(1000);
                Log.Information("Task2 finished");
            });

            Task.WaitAll(task1, task2);
        }
    }
}