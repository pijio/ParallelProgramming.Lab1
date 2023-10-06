namespace ParallelProgramming.Lab1.App;

public class Helpers
{
    public static bool EqualsDefaultValue<T>(T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default(T));
    }
}