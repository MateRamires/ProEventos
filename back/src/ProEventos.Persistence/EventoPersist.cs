using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Persistance.Contextos;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Persistence
{
    public class EventoPersist : IEventoPersist
    {
        private readonly ProEventosContext _context;

        public EventoPersist(ProEventosContext context)
        {
            _context = context;

        }

        public async Task<Evento[]> GetAllEventosAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);

            if(includePalestrantes)
            {
                query = query
                    .Include(e => e.PalestrantesEventos)
                    .ThenInclude(pe => pe.Palestrante); //ThenInclude foi usado aqui pois, primeiro deve ser pego os PalestrantesEventos dentro de Evento, e ai dentro de PalestranteEvento, pegue os palestrantes
            }

            query = query.AsNoTracking().OrderBy(e => e.Id); //Ordenará a query pelo ID.

            return await query.ToArrayAsync(); //E por fim retornar a query.
        }

        public async Task<Evento[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);

            if(includePalestrantes)
            {
                query = query
                    .Include(e => e.PalestrantesEventos)
                    .ThenInclude(pe => pe.Palestrante); 
            }

            query = query.AsNoTracking().OrderBy(e => e.Id)
                .Where(e => e.Tema.ToLower().Contains(tema.ToLower())); //Antes de retornarmos estamos fazendo um Where. A logica dentro desse Where é a seguinte: a cada evento que tiver (e), procure o tema (e.Tema), converte para lowerCase (ToLower()) e analisa se ele contem o tema que foi passado no parametro e converte ele para lowerCase tambem.

            return await query.ToArrayAsync(); 
        }

        public async Task<Evento> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);

            if (includePalestrantes)
            {
                query = query
                    .Include(e => e.PalestrantesEventos)
                    .ThenInclude(pe => pe.Palestrante);
            }

            query = query.AsNoTracking().OrderBy(e => e.Id) //AsNoTracking resolve um possivel erro de tracking.
                         .Where(e => e.Id == eventoId);

            return await query.FirstOrDefaultAsync();
        }

    }
}