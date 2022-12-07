using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class LoteService : ILoteService
    {
        private readonly IGeralPersist geralPersist;
        private readonly ILotePersist lotePersist;
        private readonly IMapper mapper;
        public LoteService(IGeralPersist geralPersist,
                           ILotePersist lotePersist,
                           IMapper mapper)
        {
            this.geralPersist = geralPersist;
            this.lotePersist = lotePersist;
            this.mapper = mapper;

        }
    public async Task AddLote(int eventoId, LoteDto model)
    {
       try
        {
            var lote = mapper.Map<Lote>(model); 
            lote.EventoId = eventoId;

            geralPersist.Add<Lote>(lote); 

            await geralPersist.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); 
        }
    }

    public async Task<LoteDto[]> SaveLotes(int eventoId, LoteDto[] models)
    {
        try
        {
            var lotes = await lotePersist.GetLotesByEventoIdAsync(eventoId);
            if (lotes == null) return null;

            foreach (var model in models)
            {
                if(model.Id == 0) {
                    await AddLote(eventoId, model);
                }
                else {

                    var lote = lotes.FirstOrDefault(lote => lote.Id == model.Id);
                    model.EventoId = eventoId;

                    mapper.Map(model, lote);

                    geralPersist.Update<Lote>(lote);

                    await geralPersist.SaveChangesAsync();
                }
            }
            
            var loteRetorno = await lotePersist.GetLotesByEventoIdAsync(eventoId); 
            return mapper.Map<LoteDto[]>(loteRetorno);
            
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteLote(int eventoId, int loteId)
    {
        try
        {
            var lote = await lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote == null) throw new Exception("Lote para o Delete não foi encontrado!"); //Se o evento nao for encontrado, sera enviado uma excessao com essa mensagem ao controller.

            geralPersist.Delete<Lote>(lote);
            return await geralPersist.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<LoteDto[]> GetLotesByEventoIdAsync(int eventoId)
    {
        try
        {
            var lotes = await lotePersist.GetLotesByEventoIdAsync(eventoId);
            if (lotes == null) return null;

            var resultado = mapper.Map<LoteDto[]>(lotes);

            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<LoteDto> GetLoteByIdsAsync(int eventoId, int loteId)
    {
        try
        {
            var lote = await lotePersist.GetLoteByIdsAsync(eventoId, loteId);
            if (lote == null) return null;
    
            var resultado = mapper.Map<LoteDto>(lote); 

            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


}
}