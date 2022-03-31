namespace Pawelsberg.Tavli.Model.Extensions;

public static class ControlFlowExtensions
{
    public static T For<T>(this T initValue, Func<T, bool> condition, Func<T, T> iterator)
    {
        return condition(initValue) ? For(iterator(initValue), condition, iterator) : initValue;
    }
}

