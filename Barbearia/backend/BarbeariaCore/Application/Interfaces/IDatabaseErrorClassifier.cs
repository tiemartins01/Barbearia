namespace BarbeariaCore.Application.Interfaces
{
    public interface IDatabaseErrorClassifier
    {
        bool IsUniqueViolation(
            Exception exception,
            string? constraintName = null);
    }
}