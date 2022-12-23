using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos; //Como estamos fazendo uso de eventos, temos que dar esse using na pasta dos modelos dtos, onde esta localizado a classe Evento.

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
                if(eventos == null) return NoContent();

                return Ok(eventos); //E agora nos retornamos a lista de EventoDto, antes nos retornavamos a lista de eventos "crua" com todos os atributos, sem nenhum filtro.
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
                if(evento == null) return NoContent();

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
                if(evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPost("upload-image/{eventoId}")] 
        public async Task<IActionResult> UploadImage(int eventoId) 
        {
            try
            {
                var evento = await eventoService.GetEventoByIdAsync(eventoId, true); //Primeiro verificamos se o evento existe com o id que foi passado.
                if(evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if(file.Length > 0){
                    // DeleteImage(evento.ImageURL);
                    //evento.imageURL = SaveImage(file);
                }
                var EventoRetorno = await eventoService.UpdateEvento(eventoId, evento);

                return Ok(EventoRetorno);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar adicionar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPost] //Aqui eh referenciado qual metodo do http que tera a funcao abaixo.
        public async Task<IActionResult> Post(EventoDto model) //Essa eh a funcao que ira ser chamada ao usar o metodo Post no EventoControler.
        {
            try
            {
                var evento = await eventoService.AddEventos(model);
                if(evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar adicionar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPut("{id}")] //Nesse metodo put estamos tambem pedindo para passar o parametro id.
        public async Task<IActionResult> Put(int id, EventoDto model)
        {
            try
            {
                var evento = await eventoService.UpdateEvento(id, model);
                if(evento == null) return NoContent();

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
                var evento = await eventoService.GetEventoByIdAsync(id, true);
                if(evento == null) return NoContent();
                
                return await eventoService.DeleteEvento(id) 
                ? Ok(new { message = "Deletado"}) 
                : throw new Exception("Ocorreu um problema ao tentar deletar o evento.");
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar eventos. Erro: {ex.Message}");
            }
        }

    }
}
