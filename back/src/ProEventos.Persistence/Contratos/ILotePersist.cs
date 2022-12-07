using System.Threading.Tasks;
using ProEventos.Domain;

namespace ProEventos.Persistence.Contratos
{
    public interface ILotePersist
    {
        /// <summary>
        /// Metodo get que retornara uma lista de lote por eventoId.
        /// </summary>
        /// <param name="eventoId">Codigo chave da tabela Evento</param>
        /// <returns>Array de lotes</returns>
         Task<Lote[]> GetLotesByEventoIdAsync(int eventoId);
         /// <summary>
         /// Metodo get que retornara apenas um lote
         /// </summary>
         /// <param name="eventoId">Codigo chave da tabela Evento</param>
         /// <param name="id">Codigo chave do meu lote</param>
         /// <returns>Apenas 1 lote</returns>
         Task<Lote> GetLoteByIdsAsync(int eventoId, int id);

    }
}