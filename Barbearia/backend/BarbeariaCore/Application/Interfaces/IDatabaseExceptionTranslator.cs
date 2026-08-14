using Microsoft.EntityFrameworkCore;

namespace BarbeariaCore.Application.Interfaces
{
    public interface IDatabaseErrorClassifier
    {
        bool IsUniqueViolation(
            DbUpdateException exception,
            string? constraintName = null);
    }
}
