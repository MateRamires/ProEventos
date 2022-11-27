using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;

namespace ProEventos.Application
{
    public class EventoService : IEventoService
    {
        private readonly IGeralPersist geralPersist;
        private readonly IEventoPersist eventoPersist;
        private readonly IMapper mapper;
        public EventoService(IGeralPersist geralPersist,
                             IEventoPersist eventoPersist,
                             IMapper mapper)
        {
            this.geralPersist = geralPersist;
            this.eventoPersist = eventoPersist;
            this.mapper = mapper;

        }
    public async Task<EventoDto> AddEventos(EventoDto model)
    {
        return null;
        /*try
        {
            geralPersist.Add<Evento>(model);
            if (await geralPersist.SaveChangesAsync())
            {
                return await eventoPersist.GetEventoByIdAsync(model.Id, false);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message); //Todos os possiveis erros serao tratados aqui.
        }*/
    }

    public async Task<EventoDto> UpdateEvento(int eventoId, EventoDto model)
    {
        return null;
        /*try
        {
            var evento = await eventoPersist.GetEventoByIdAsync(eventoId, false);
            if (evento == null) return null;

            model.Id = evento.Id;

            geralPersist.Update(model);
            if (await geralPersist.SaveChangesAsync())
            {
                return await eventoPersist.GetEventoByIdAsync(model.Id, false);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }*/
    }

    public async Task<bool> DeleteEvento(int eventoId)
    {
        try
        {
            var evento = await eventoPersist.GetEventoByIdAsync(eventoId, false);
            if (evento == null) throw new Exception("Evento para o Delete não foi encontrado!"); //Se o evento nao for encontrado, sera enviado uma excessao com essa mensagem ao controller.

            geralPersist.Delete<Evento>(evento);
            return await geralPersist.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDto[]> GetAllEventosAsync(bool includePalestrantes = false)
    {

        try
        {
            var eventos = await eventoPersist.GetAllEventosAsync(includePalestrantes);
            if (eventos == null) return null;

            var resultado = mapper.Map<EventoDto[]>(eventos);

            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public async Task<EventoDto[]> GetAllEventosByTemaAsync(string tema, bool includePalestrantes = false)
    {
        try
        {
            var eventos = await eventoPersist.GetAllEventosByTemaAsync(tema, includePalestrantes);
            if (eventos == null) return null;

            var resultado = mapper.Map<EventoDto[]>(eventos);

            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EventoDto> GetEventoByIdAsync(int eventoId, bool includePalestrantes = false)
    {
        try
        {
            var evento = await eventoPersist.GetEventoByIdAsync(eventoId, includePalestrantes);
            if (evento == null) return null;
    
            var resultado = mapper.Map<EventoDto>(evento); //Dado um evento, faca o mapeamento com base no EventoDto. (Esse eh o automapper, basta essa linha de codigo que ja substitui o mapeamento de evento para eventoDto manual que fizemos)

            return resultado;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


}
}