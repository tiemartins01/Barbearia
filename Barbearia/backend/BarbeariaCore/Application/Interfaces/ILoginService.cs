using Barbearia.Core.Domain.Entities;
using Barbearia.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barbearia.Core.Interface
{
    public interface ILoginService
    {
        Task<DTOAuthResponse> RealizarLoginAsync(string login, string senha);
        

    }
}
