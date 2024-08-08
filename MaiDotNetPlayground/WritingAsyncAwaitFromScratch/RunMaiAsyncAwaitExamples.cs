namespace MaiDotNetPlayground.WritingAsyncAwaitFromScratch;

public class RunMaiAsyncAwaitExamples
{
    public static void Run()
    {
        // https://www.youtube.com/watch?v=R-z2Hv-7nxk
        Console.WriteLine("Hello, Writing async/await from scratch in C# Stephen Toub!");

        for (int i = 0; i < 100; i++)
        {
            int capturedValue = i; // Create a local variable to capture the current value of i
            ThreadPool.QueueUserWorkItem(delegate
            {
                Console.WriteLine(capturedValue);
                Thread.Sleep(1000);
            }); // Queueing/launching workitem(s) asynchronously.
        }

        Console.WriteLine("Finished queueing 100 work items.");
    }
}
