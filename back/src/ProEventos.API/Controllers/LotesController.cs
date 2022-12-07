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
    public class LotesController : ControllerBase
    {
        private readonly ILoteService loteService;
        public LotesController(ILoteService LoteService)
        {
            this.loteService = LoteService;

        }

        [HttpGet("{eventoId}")]
        public async Task<IActionResult> Get(int eventoId) 
        {
            try
            {
                var eventos = await eventoService.GetEventoByIdAsync(true);
                if(eventos == null) return NoContent();

                return Ok(eventos); 
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }

        [HttpPut("{eventoId}")] 
        public async Task<IActionResult> Put(int eventoId, LoteDto[] models)
        {
            try
            {
                var evento = await eventoService.UpdateEvento(id, models);
                if(evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar atualizar eventos. Erro: {ex.Message}");
            }
        }


        [HttpDelete("{eventoId}/{loteId}")]
        public async Task<IActionResult> Delete(int eventoId, int loteId)
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
