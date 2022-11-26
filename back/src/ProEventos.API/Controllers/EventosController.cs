using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Dtos;
using ProEventos.Application.Contratos;
using ProEventos.Domain; //Como estamos fazendo uso de eventos, temos que dar esse using na pasta dos modelos, onde esta localizado a classe Evento.

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //Aqui esta a rota, e pode ser alterado
    public class EventosController : ControllerBase
    {
        private readonly IEventoService eventoService;
        public EventosController(IEventoService eventoService)
        {
            this.eventoService = eventoService;

        }

        [HttpGet]
        public async Task<IActionResult> Get() //Evento eh o tipo desse metodo, ou seja, por ser um get, ele retornara um evento.
        {
            try
            {
                var eventos = await eventoService.GetAllEventosAsync(true);
                if(eventos == null) return NotFound("Nenhum evento encontrado.");

                var eventosRetorno = new List<EventoDto>(); //Estamos criando um eventosRetorno que eh uma lista de eventosDto

                foreach (var evento in eventos) //Estamos iterando pelo eventos que possui todos os eventos vindo do DB
                {
                    eventosRetorno.Add(new EventoDto(){ //Estamos adicionando cada um dos eventos para essa lista de eventosDto, e o dto possui apenas os atributos que vamos retornar para quem esta chamando essa API.
                        Id = evento.Id,
                        Local = evento.Local,
                        DataEvento = evento.DataEvento.ToString(),
                        Tema = evento.Tema,
                        QtdPessoas = evento.QtdPessoas,
                        imageURL = evento.imageURL,
                        Telefone = evento.Telefone,
                        Email = evento.Email

                    });
                }

                return Ok(eventosRetorno); //E agora nos retornamos a lista de EventoDto, antes nos retornavamos a lista de eventos "crua" com todos os atributos, sem nenhum filtro.
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) 
        {
            try
            {
                var evento = await eventoService.GetEventoByIdAsync(id, true);
                if(evento == null) return NotFound("Evento por ID não encontrado.");

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpGet("{tema}/tema")]
        public async Task<IActionResult> GetByTema(string tema) 
        {
            try
            {
                var evento = await eventoService.GetAllEventosByTemaAsync(tema, true);
                if(evento == null) return NotFound("Eventos por tema não encontrados.");

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPost] //Aqui eh referenciado qual metodo do http que tera a funcao abaixo.
        public async Task<IActionResult> Post(Evento model) //Essa eh a funcao que ira ser chamada ao usar o metodo Post no EventoControler.
        {
            try
            {
                var evento = await eventoService.AddEventos(model);
                if(evento == null) return BadRequest("Erro ao tentar adicionar evento.");

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar adicionar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPut("{id}")] //Nesse metodo put estamos tambem pedindo para passar o parametro id.
        public async Task<IActionResult> Put(int id, Evento model)
        {
            try
            {
                var evento = await eventoService.UpdateEvento(id, model);
                if(evento == null) return BadRequest("Erro ao tentar atualizar evento.");

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar eventos. Erro: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await eventoService.DeleteEvento(id) ? 
                Ok("Deletado.") : 
                BadRequest("Evento não deletado");
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar eventos. Erro: {ex.Message}");
            }
        }

    }
}
