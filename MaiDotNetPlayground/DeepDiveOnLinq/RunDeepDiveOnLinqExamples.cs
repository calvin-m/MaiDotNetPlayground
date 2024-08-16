namespace MaiDotNetPlayground.DeepDiveOnLinq;

public static class RunDeepDiveOnLinqExamples
{
    public static void Run()
    {
        // --- code block A ---
        // IEnumerable<int> e = GetValues();
        // using IEnumerator<int> enumerator = e.GetEnumerator();
        // while(enumerator.MoveNext())
        // {
        //     int i = enumerator.Current;
        //     Console.WriteLine(i);
        // }
/*
The "foreach" keyword would be lowered to the above "code block A".
The "using" statement itself is a higher construct, 
which under the hood, it will be lowered to a try-finally construct:

try
{
    while (enumerator.MoveNext())
    {
        ....
    }
}
finally
{
    enumerator?.Dispose();
}
*/
        foreach(int i in GetValues())
        {
            Console.WriteLine(i);
        }
    }

/*
Iterators allow you are basically a form of a co-routine 
so it's not you enter the method you do everything and you leave. 
It's you enter the method you do some stuff, you leave, you come back, 
you do some more stuff, you leave, you come back, you do some more stuff and so on,
which is referred to a sort of resumption and suspension.
*/
    static IEnumerable<int> GetValues()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }

}
