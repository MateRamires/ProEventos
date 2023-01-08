using System;
using System.Threading.Tasks;
using AutoMapper;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos;
using ProEventos.Domain;
using ProEventos.Persistence.Contratos;
using ProEventos.Persistence.Models;

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
        public async Task<EventoDto> AddEventos(int userId, EventoDto model)
        {
            try
            {
                var evento = mapper.Map<Evento>(model); //O model eh um DTO, entao primeiro "convertemos" o DTO para um evento normal.
                evento.UserId = userId; //no modulo 13, nos criamos uma nova coluna para evento que eh o userId, e agora estamos falando que o userId desse evento, eh o user que esta logado.

                geralPersist.Add<Evento>(evento); //Depois adicionamos esse evento normal


                if (await geralPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);
                    return mapper.Map<EventoDto>(eventoRetorno); //E por ultimo fazemos o contrario, agora transformamos o evento normal em um DTO, para retornarmos o DTO.
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); //Todos os possiveis erros serao tratados aqui.
            }
        }

        public async Task<EventoDto> UpdateEvento(int userId, int eventoId, EventoDto model)
        {
            try
            {
                var evento = await eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) return null;

                model.Id = evento.Id;
                model.UserId = userId;

                mapper.Map(model, evento);

                geralPersist.Update<Evento>(evento);

                if (await geralPersist.SaveChangesAsync())
                {
                    var eventoRetorno = await eventoPersist.GetEventoByIdAsync(userId, evento.Id, false);
                    return mapper.Map<EventoDto>(eventoRetorno);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                var evento = await eventoPersist.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null) throw new Exception("Evento para o Delete não foi encontrado!"); //Se o evento nao for encontrado, sera enviado uma excessao com essa mensagem ao controller.

                geralPersist.Delete<Evento>(evento);
                return await geralPersist.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<PageList<EventoDto>> GetAllEventosAsync(int userId, PageParams pageParams, bool includePalestrantes = false)
        {

            try
            {
                var eventos = await eventoPersist.GetAllEventosAsync(userId, pageParams, includePalestrantes);
                if (eventos == null) return null;

                var resultado = mapper.Map<PageList<EventoDto>>(eventos);

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<EventoDto> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await eventoPersist.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                if (evento == null) return null;

                var resultado = mapper.Map<EventoDto>(evento); //Dado um evento, faca o mapeamento com base no EventoDto. (Esse eh o automapper, basta essa linha de codigo que ja substitui o mapeamento de evento para eventoDto manual que fizemos)

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /*public async Task<EventoDto[]> GetAllEventosByTemaAsync(int userId, string tema, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await eventoPersist.GetAllEventosByTemaAsync(userId, tema, includePalestrantes);
                if (eventos == null) return null;

                var resultado = mapper.Map<EventoDto[]>(eventos);

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }*/


    }
}