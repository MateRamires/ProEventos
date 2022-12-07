using System;
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
    public async Task<LoteDto> AddEventos(LoteDto model)
    {
       try
        {
            var evento = mapper.Map<Evento>(model); //O model eh um DTO, entao primeiro "convertemos" o DTO para um evento normal.

            geralPersist.Add<Evento>(evento); //Depois adicionamos esse evento normal

            if (await geralPersist.SaveChangesAsync())
            {
                var eventoRetorno = await lotePersist.GetEventoByIdAsync(evento.Id, false); 
                return mapper.Map<LoteDto>(eventoRetorno); //E por ultimo fazemos o contrario, agora transformamos o evento normal em um DTO, para retornarmos o DTO.
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); //Todos os possiveis erros serao tratados aqui.
        }
    }

    public async Task<LoteDto> UpdateEvento(int eventoId, LoteDto model)
    {
        try
        {
            var evento = await lotePersist.GetEventoByIdAsync(eventoId, false);
            if (evento == null) return null;

            model.Id = evento.Id;

            mapper.Map(model, evento);

            geralPersist.Update<Evento>(evento);

            if (await geralPersist.SaveChangesAsync())
            {
                var eventoRetorno = await lotePersist.GetEventoByIdAsync(evento.Id, false); 
                return mapper.Map<LoteDto>(eventoRetorno);
            }
            return null;
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