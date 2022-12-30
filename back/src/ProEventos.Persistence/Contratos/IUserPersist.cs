using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProEventos.Domain.Identity;

namespace ProEventos.Persistence.Contratos
{
    public interface IUserPersist : IGeralPersist //Nos outros persists nos nao estavamos herdando de Geral, entao la no service na camada de application, nos tinhamos que injetar tanto o geralPersist quanto o eventoPersist. Agora, como estamos herdando de geral, so iremos precisar injetar o userPersist la no userService (que ainda nao criamos).
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);

    }
}