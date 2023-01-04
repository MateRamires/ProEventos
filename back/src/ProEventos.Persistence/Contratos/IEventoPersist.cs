using System.Threading.Tasks;
using ProEventos.Domain;

namespace ProEventos.Persistence.Contratos
{
    public interface IEventoPersist
    {
         //EVENTOS
         Task<Evento[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false); //Estamos adicionando na aula 223, esse novo parametro userId, pois independente se estamos pegando os temas por Id, ou por tema, nos sempre iremos pegar os eventos do usuario especifico que esta logado.
         Task<Evento[]> GetAllEventosAsync(int userId, bool includePalestrantes = false);
         Task<Evento> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false);

    }
}