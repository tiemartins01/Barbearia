namespace BarbeariaTests.Helpers;

internal static class ReflectionHelper
{
    public static void SetPrivateProperty<T>(object target, string propertyName, T value)
    {
        var property = target.GetType().GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Propriedade {propertyName} não encontrada.");
        property.SetValue(target, value);
    }
}
