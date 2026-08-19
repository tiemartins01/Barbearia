using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public sealed class PostgreSqlErrorClassifier
    : IDatabaseErrorClassifier
{
    public bool IsUniqueViolation(
        Exception exception,
        string? constraintName = null)
    {
        if (exception.InnerException
            is not PostgresException postgres)
        {
            return false;
        }

        if (postgres.SqlState !=
            PostgresErrorCodes.UniqueViolation)
        {
            return false;
        }

        return constraintName is null ||
               postgres.ConstraintName == constraintName;
    }
}