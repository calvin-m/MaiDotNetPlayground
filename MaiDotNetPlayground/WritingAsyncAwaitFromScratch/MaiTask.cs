using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace MaiDotNetPlayground.WritingAsyncAwaitFromScratch;

public class MaiTask
{
    private readonly object _lock = new();
    private bool _completed;
    private Exception? _exception;
    private Action? _continuation;
    private ExecutionContext? _context;

    public struct Awaiter(MaiTask t) : INotifyCompletion
    {
        public Awaiter GetAwaiter() => this;
        public bool IsCompleted => t.IsCompleted;
        public void OnCompleted(Action continuation) => t.ContinueWith(continuation);

        public void GetResult() => t.Wait();
    }

    public Awaiter GetAwaiter() => new(this);
    public bool IsCompleted
    {
        get
        {
            lock (_lock)
            {
                return _completed;
            }
        }
    }
    public void SetResult() => Complete(null);
    public void SetException(Exception exception) => Complete(exception);
    private void Complete(Exception? exception)
    {
        lock (_lock)
        {
            if (_completed) throw new InvalidOperationException("Stop messing up my code!");

            _completed = true;
            _exception = exception;

            if (_continuation is not null)
            {
                MaiThreadPool.QueueUserWorkItem(delegate
                {
                    if (_context is null)
                    {
                        _continuation();
                    }
                    else
                    {
                        ExecutionContext.Run(_context, (object? state) => ((Action)state!).Invoke(), _continuation);
                    }
                });
            }
        }
    }
    public void Wait()
    {
        /*
So I need to be able to block and anytime you want to kind of synchronously block waiting for something 
you need some sort of synchronization primitive 
in this case I'm going to use a manual reset event.

so I'm going to wait for this manual reset event but only if I create one.
and so I'm only going to create one if this task hasn't yet completed if it's already completed 
there's nothing for me to wait for.

So if it hasn't completed then I actually instantiate this 
and now I need to Signal this ManualResetEvent
to become in a signal State that anyone will waiting on it, will wake up when this task completes.
How do I do?
by doing  "ContinueWith(mres.Set)" 
so now I'm implementing Wait() in terms of ContinueWith by saying when this task completes,
hook up a delegate that will invoke "ManualResetEventSlim.Set()" which will then cause 
this "mres?.Wait()" to wake up.

ManualResetEventSlim is literally the slim, lighter Wait() version of ManualResetEvent 
and because you're not going to be waiting long 
it would be appropriate to use the light / the diet-coke version.

It's actually appropriate to use the diet coke version in 99% of situations 
and better to use the diet coke version.
In this case the ManualResetEvent is just a very thin wrapper around the OS's the kernels equivalent primitive.
 and that means that every time I do any operation on it 
 I'm kind of paying a fair amount of overhead to dive down into the kernel.

ManualResetEventSlim is a much lighter weight version of it 
that's all implemented up in user code in .Net world,
basically just in terms of  monitors which is what lock is also built on top of.

The only time it's less appropriate to use it is if you actually need one of those kernel level things 
which you typically only need if you're doing something more esoteric with Wait Handles in a broader.
        */
        ManualResetEventSlim? mres = null;
        lock (_lock)
        {
            if (!_completed)
            {
                mres = new ManualResetEventSlim();
                ContinueWith(mres.Set);
            }
        }

        mres?.Wait();

        if (_exception is not null)
        {
            // throw _exception;
            /*
            If You tab an existing exception object that has previously been thrown,
            that exception contains a stack trace 
            it contains some what's referred to as the Watson bucket 
            which contains sort of aggregatable information about where that exception came from,
            for use in postmortem debugging and Diagnostics.

            When I rethrow exception like "throw _exception", that's going to overwrite all of that information 
            so I kind of don't want to do that. 
            One common way around that and that was the only way around that 
            when task initially hit the scene and .NET framework 4.0, was to wrap it in another exception. 

            */

            //throw new AggregateException("", _exception);
            /*
            so you might wrap this in and have like an inner exception.
            now throwing this will populate this "new Exception(...)" stack trace 
            this "_exception" will still be available as the inner exception and it won't be touched 
            so all of the stack Trace will stay in place 
            */
            ExceptionDispatchInfo.Throw(_exception);
            /*
Since task was introduced, something that was very useful for
Await is another sort of pretty lowlevel type called ExceptionDispatchInfo.
The name doesn't really matter but what this does is it takes
that exception and it throws it, but rather than overwriting the current stack trace,
it appends the current stack trace and so for anyone who's looked at a an exception 
that's propagated through multiple Awaits, you might be used to seeing a bit of a stack trace 
and then a little dotted line that says 
continue that or original throw location and then more stack Trace 
Every time this exception is getting rethrown up the call stack, up the asynchronous call stack,
 more state is being appended to that stack and that's all handled via this.
            */
        }
    }
    public MaiTask ContinueWith(Action action)
    {
        MaiTask t = new();

        Action callback = () =>
        {
            try
            {
                action();
            }
            catch(Exception e)
            {
                t.SetException(e);
                return;
            }

            t.SetResult();
        };

        lock (_lock)
        {
            if (_completed)
            {
                MaiThreadPool.QueueUserWorkItem(callback);
            }
            else
            {
                _continuation = callback;
                _context = ExecutionContext.Capture();
            }

        }
        return t;
    }

    public MaiTask ContinueWith(Func<MaiTask> action)
    {
        MaiTask t = new();

        Action callback = () =>
        {
            try
            {
                MaiTask next = action();
                next.ContinueWith(delegate
                {
                    if(next._exception is not null)
                    {
                        t.SetException(next._exception);
                    }
                    else
                    {
                        t.SetResult();
                    }
                });
            }
            catch(Exception e)
            {
                t.SetException(e);
                return;
            }
        };

        lock (_lock)
        {
            if (_completed)
            {
                MaiThreadPool.QueueUserWorkItem(callback);
            }
            else
            {
                _continuation = callback;
                _context = ExecutionContext.Capture();
            }

        }
        return t;
    }

    public static MaiTask Run(Action action)
    {
        MaiTask t = new();

        MaiThreadPool.QueueUserWorkItem(() =>
        {
            try
            {
                action();
            }
            catch(Exception e)
            {
                t.SetException(e);
                return;
            }

            t.SetResult();
        });
        return t;
    }

    public static MaiTask WhenAll(List<MaiTask> tasks)
    {
        MaiTask t = new();

        if(tasks.Count == 0)
        {
            t.SetResult();
        }
        else
        {
            int remaining = tasks.Count;
            Action continuation = () =>
            {
                /*
These "tasks" are doing they might all be completing at the same time or not 
and if they were to both complete at approximately the same time 
this continuation two different threads might be trying to decrement this value 
and if they each tried to do it without any synchronization 
their operations might sort of stomp on each other 
and we might lose some of the decrements which would be a big problem 
because we wouldn't know when we actually hit zero.

So I'm using this lightweight synchronization mechanism to ensure that 
all of the decrements are tracked and that only the one that is actually 
the last one to complete performs this work because as we saw if I dive into this 
if multiple of them think that they're the last one and they try 
and both complete it it's going to fail.

And you said lightweight synchronization method as opposed to trying to do some locking around 
that which I suppose you could have done

Totally I could have had it taken a lock here but this is one place where it's really simple and
straightforward to use basically the lowest level synchronization primitive that I have available to me 
which is a lock-free Interlocked operation.
                */

                if(Interlocked.Decrement(ref remaining) == 0)
                {
                    // TODO: exceptions
                    t.SetResult();
                }
            };

            foreach (var task in tasks)
            {
                task.ContinueWith(continuation);
            }
        }
        return t;
    }

    public static MaiTask Delay(int timeout)
    {
        MaiTask t = new();
        new Timer( _ => t.SetResult()).Change(timeout, -1);
        return t;
    }

    public static MaiTask Iterate(IEnumerable<MaiTask> tasks)
    {
        MaiTask t = new();

        IEnumerator<MaiTask> e = tasks.GetEnumerator();

        void MoveNext()
        {
            try
            {
                if(e.MoveNext())
                {
                    MaiTask next = e.Current;
                    next.ContinueWith(MoveNext);
                    return;
                }
            }
            catch(Exception e)
            {
                t.SetException(e);
                return;
            }
            
            t.SetResult();
        }

        MoveNext();

        return t;
    }
}
