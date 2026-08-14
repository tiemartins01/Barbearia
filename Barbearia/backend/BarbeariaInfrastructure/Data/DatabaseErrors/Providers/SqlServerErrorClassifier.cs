using BarbeariaCore.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BarbeariaInfrastructure.Data.DatabaseErrors.Providers
{
    public sealed class SqlServerErrorClassifier: IDatabaseErrorClassifier
    {
        public bool IsUniqueViolation(
            DbUpdateException exception,
            string? constraintName = null)
        {
            if (exception.InnerException is not SqlException sql)
                return false;

            if (sql.Number is not (2601 or 2627))
                return false;

            if (string.IsNullOrWhiteSpace(constraintName))
                return true;

            return sql.Message.Contains(
                constraintName,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}


//É SqlException?
//↓
//É erro 2601 ou 2627?
//↓
//SIM
//↓
//constraintName foi informado?
//        ↓
//       NÃO → true

//       SIM
//        ↓
//mensagem contém
//"ux_horarios_barbeiro_horario_ativo"?
//        ↓
//     sim → true
//     não → false

