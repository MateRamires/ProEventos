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
                var lotes = await loteService.GetLotesByEventoIdAsync(eventoId);
                if(lotes == null) return NoContent();

                return Ok(lotes); 
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar lotes. Erro: {ex.Message}");
            }
        }

        [HttpPut("{eventoId}")] 
        public async Task<IActionResult> SaveLotes(int eventoId, LoteDto[] models)
        {
            try
            {
                var lotes = await loteService.SaveLotes(eventoId, models);
                if(lotes == null) return NoContent();

                return Ok(lotes);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar salvar lotes. Erro: {ex.Message}");
            }
        }


        [HttpDelete("{eventoId}/{loteId}")]
        public async Task<IActionResult> Delete(int eventoId, int loteId)
        {
            try
            {
                var lote = await loteService.GetLoteByIdsAsync(eventoId, loteId);
                if(lote == null) return NoContent();
                
                return await loteService.DeleteLote(lote.EventoId, lote.Id) 
                ? Ok(new { message = "Lote Deletado"}) 
                : throw new Exception("Ocorreu um problema ao tentar deletar o lote.");
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar lotes. Erro: {ex.Message}");
            }
        }

    }
}
