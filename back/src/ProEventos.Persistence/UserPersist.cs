using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain.Identity;
using ProEventos.Persistance.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class UserPersist : GeralPersist, IUserPersist //Alem daquela alteracao na interface, aqui tambem precisamos herdar da interface E TAMBEM do geralPersist
    {
        private readonly ProEventosContext context;

        public UserPersist(ProEventosContext context) : base(context) //esse base(context) tambem eh necessario para funcionar essa nova forma de heranca que estamos fazendo.
        {
            this.context = context;
            
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await context.Users.ToListAsync();
        }
        
        public async Task<User> GetUserByIdAsync(int id)
        {
           return await context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
             return await context.Users.SingleOrDefaultAsync(user => user.UserName == userName.ToLower());
        }
        

    }
}