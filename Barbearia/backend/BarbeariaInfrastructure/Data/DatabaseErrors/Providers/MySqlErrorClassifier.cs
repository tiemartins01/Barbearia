using BarbeariaCore.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BarbeariaInfrastructure.Data.DatabaseErrors.Providers;

public sealed class MySqlErrorClassifier
    : IDatabaseErrorClassifier
{
    public bool IsUniqueViolation(
        DbUpdateException exception,
        string? constraintName = null)
    {
        if (exception.InnerException is not MySqlException mysql)
            return false;

        if (mysql.ErrorCode != MySqlErrorCode.DuplicateKeyEntry)
            return false;

        if (string.IsNullOrWhiteSpace(constraintName))
            return true;

        return mysql.Message.Contains(
            constraintName,
            StringComparison.OrdinalIgnoreCase);
    }
}

//PostgreSQL
//→ erro unique?
//→ constraint correta?

//SQL Server
//→ erro unique?
//→ constraint correta?

//MySQL
//→ erro unique?
//→ constraint correta?
