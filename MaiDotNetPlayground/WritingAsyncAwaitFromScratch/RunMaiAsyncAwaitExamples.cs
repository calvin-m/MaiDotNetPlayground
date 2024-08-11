namespace MaiDotNetPlayground.WritingAsyncAwaitFromScratch;

public class RunMaiAsyncAwaitExamples
{
    public static void Run()
    {
        // https://www.youtube.com/watch?v=R-z2Hv-7nxk
        Console.WriteLine("Hello, Writing async/await from scratch in C# Stephen Toub!");

        AsyncLocal<int> myValue = new(); // ExecutionContext
        List<MaiTask> tasks = new();
        for (int i = 0; i < 100; i++)
        {
            myValue.Value = i; 
            tasks.Add(MaiTask.Run(delegate
            {
                Console.WriteLine(myValue.Value);
                Thread.Sleep(1000);
            })); // Queueing/launching workitem(s) asynchronously.
        }

        //foreach(var t in tasks) t.Wait();

        // Instead of waiting every task above,
        MaiTask.WhenAll(tasks).Wait();
    }
}
