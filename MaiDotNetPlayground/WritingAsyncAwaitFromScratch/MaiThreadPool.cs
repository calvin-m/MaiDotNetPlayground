using System.Collections.Concurrent;

namespace MaiDotNetPlayground.WritingAsyncAwaitFromScratch;

public static class MaiThreadPool
{
    private static readonly BlockingCollection<(Action, ExecutionContext?)> s_workItems = new(); 
  /*
There are lots of different data structures that I could use 
but I'm going to use one called BlockingCollection 
and the beauty of BlockingCollection here is that you can store things into it 
It's basically concurrent queue but when I want to take something out 
I will block waiting to take out the thing 
if it's empty and that's what I want my threads to be doing.

All of my threads and my thread pool are going to be trying to take things from this que to process it 
and if there's nothing there I want them to just wait for something to be available
  */
    public static void QueueUserWorkItem(Action action) => s_workItems.Add((action, ExecutionContext.Capture()));

    static MaiThreadPool()
    {
        for(int i=0; i < Environment.ProcessorCount; i++) // Environment.ProcessorCount: 8 in my MacBook Pro
        {
            new Thread( () =>
            {
                while(true)
                {
                    (Action workItem, ExecutionContext? context) = s_workItems.Take();
                    if(context is null)
                    {
                        workItem();
                    }
                    else
                    {
                        ExecutionContext.Run(context, state => ((Action)state!).Invoke(), workItem);
                        // The above code can be rewritten more readably but less efficiently as follow:
                        //ExecutionContext.Run(context, delegate { workItem(); }, null); // Functionally the same but a little less efficient
                        /*
The difference between these lines is

ExecutionContext.Run actually takes a State argument 
and then that State argument is passed into that ContextCallback delegate 
so that delegate is just an action of object basically, just with a different name.

So you can pass State into it. In fact if I I should be able to browse to the definition here
and if I look at context call back all you can see you can see 
it's just a delegate that takes a state object.
This was introduced before action and action of object were added.
So it's a you know a dedicated delegate type. If we were doing it again today this type wouldn't exist.
It would just be action of object.

The reason I said this is for efficiency is because 
this version "delegate { workItem(); }" has what's called a closure 
and it needs to reference this work item that's defined out here.
So there's actually multiple objects being allocated here to be able to capture that work item into some object 
and create a delegate that's been passed in and here I can avoid that.
In fact I can see that it's being avoided and that there's no closure by using the static keyword in

                        */
                    }
                }
            })
            { IsBackground = true }.Start();
            /*
            interestingly dot-net sort of distinguishes two kinds of threads it
11:32
has what are called foreground threads and background threads 
and the only distinction between those is when your
11:37
main method exits do you want your process to wait around for all of your threads that you created to exit as well
11:44
foreground threads it will wait for them 
background threads it won't wait for them uh because I don't want these

threads that are sitting here in a infinite while loop to keep my process alive forever 
I'm just going to say IsBackground = true uh and that way these threads don't keep my process from exiting 

now is that something that might not necessarily be intuitive to someone who came from a Unix world 
who is not going to think about that kind of foreground thread background thread 
and there's also the concept of green threads and native threads In some cultures.

12:14
Yea, frankly it's it's not something that you frequently run into or or that matters 
but since we're sort of looking at implementing the lower level stuff here
            */
        }
    }
}

/*

just we've just implemented our own sort of new threadpool 
now if you were to look at the real net threadpool it's a
12:56
whole lot more code than the what is this 15 lines or 20 lines here almost
13:01
all of the real code goes into two things:
one making it super efficient and
two uh not having a fixed number of threads a lot of the logic is about thread management and increasing and
13:14
decreasing the number of threads that thread pool has over time in order to try and maintain good throughput for
13:20
your application but as I said I'm not worrying about perf.

*/
