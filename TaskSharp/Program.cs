using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Threading.Timer;

namespace TaskSharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Random rnd = new Random();
            var tokenLifetime = rnd.Next(1, 60);
            tokenSource.CancelAfter(tokenLifetime);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine($"Main Thread : {Thread.CurrentThread.ManagedThreadId} Statred");

            var mainTask = Task.Run(() => PrintCounter("main subtask"));
            await mainTask;
            try
            {
                await PrintAllCounters(tokenSource);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"{ex.Message} - Token Lifetime {tokenLifetime}");
            }
            finally
            {
                tokenSource.Dispose();
            }

            Console.WriteLine($"Main Thread : {Thread.CurrentThread.ManagedThreadId} Completed");

            Task.WaitAll();

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milisecond: {stopWatch.Elapsed.Milliseconds}");
        }

        static void PrintCounter(string taskName)
        {
            Console.WriteLine($"Child Thread : {Thread.CurrentThread.ManagedThreadId} Started,TaskName : {taskName}");
            for (int count = 1; count <= 10; count++)
            {
                Console.WriteLine($"count value: {count}");
            }

            Console.WriteLine(
                $"Child Thread : {Thread.CurrentThread.ManagedThreadId} Completed, TaskName : {taskName}");
        }

        static Task PrintAllCounters(CancellationTokenSource tokenSource)
        {
            var tasks = new Task[]
            {
                Task.Factory.StartNew(() => { PrintCounter("first"); }, tokenSource.Token),
                Task.Factory.StartNew(() => { PrintCounter("second"); }, tokenSource.Token),
                Task.Factory.StartNew(() => { PrintCounter("third"); }, tokenSource.Token)
            };

            return Task.WhenAll(tasks);
        }
    }
}