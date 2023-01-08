using System.Threading.Tasks;
using ProEventos.Domain;
using ProEventos.Persistence.Models;

namespace ProEventos.Persistence.Contratos
{
    public interface IEventoPersist
    {
         //EVENTOS
         //Task<PageList<Evento>> GetAllEventosByTemaAsync(int userId, PageParams pageParams, string tema, bool includePalestrantes = false); //Estamos adicionando na aula 223, esse novo parametro userId, pois independente se estamos pegando os temas por Id, ou por tema, nos sempre iremos pegar os eventos do usuario especifico que esta logado.
         Task<PageList<Evento>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false);
         Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);

    }
}