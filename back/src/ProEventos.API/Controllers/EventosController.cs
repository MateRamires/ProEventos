using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProEventos.API.Extensions;
using ProEventos.Application.Contratos;
using ProEventos.Application.Dtos; //Como estamos fazendo uso de eventos, temos que dar esse using na pasta dos modelos dtos, onde esta localizado a classe Evento.
using ProEventos.Persistence.Models;

namespace ProEventos.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")] //Aqui esta a rota, e pode ser alterado
    public class EventosController : ControllerBase
    {
        private readonly IEventoService eventoService;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IAccountService accountService;

        public EventosController(IEventoService eventoService, IWebHostEnvironment hostEnvironment, IAccountService accountService)
        {
            this.hostEnvironment = hostEnvironment;
            this.accountService = accountService;
            this.eventoService = eventoService;

        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams) //Evento eh o tipo desse metodo, ou seja, por ser um get, ele retornara um evento.
        {
            try
            {
                var eventos = await eventoService.GetAllEventosAsync(User.GetUserId(), pageParams, true);
                if (eventos == null) return NoContent();

                Response.AddPagination(eventos.CurrentPage, eventos.PageSize, eventos.TotalCount, eventos.TotalPages);

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
                var evento = await eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
                if (evento == null) return NoContent();

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
                var evento = await eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, true); //Primeiro verificamos se o evento existe com o id que foi passado.
                if (evento == null) return NoContent();

                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    DeleteImage(evento.imageURL);
                    evento.imageURL = await SaveImage(file);
                }
                var EventoRetorno = await eventoService.UpdateEvento(User.GetUserId(), eventoId, evento);

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
                var evento = await eventoService.AddEventos(User.GetUserId(), model);
                if (evento == null) return NoContent();

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
                var evento = await eventoService.UpdateEvento(User.GetUserId(), id, model);
                if (evento == null) return NoContent();

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
                var evento = await eventoService.GetEventoByIdAsync(User.GetUserId(), id, true);
                if (evento == null) return NoContent();

                if (await eventoService.DeleteEvento(User.GetUserId(), id))
                {
                    DeleteImage(evento.imageURL);
                    return Ok(new { message = "Deletado" });
                }
                else
                {
                    throw new Exception("Ocorreu um problema ao tentar deletar o evento.");
                }


            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar eventos. Erro: {ex.Message}");
            }
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile) //O metodo salvar, como muitas pessoas podem salvar imagens ao mesmo tempo em um sistema, alem de imagens podem ser muito grandes e levar tempo para realizar o upload, entao eh melhor que esse metodo seja async, diferente do deleteImagem.
        {
            string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName) //Ao adicionar a imagem, sera pego os 10 primeiros caracteres do noem da imagem apenas.
                                            .Take(10)
                                            .ToArray()
                                        ).Replace(' ', '-');

            imageName = $"{imageName}{DateTime.UtcNow.ToString("yymmssfff")}{Path.GetExtension(imageFile.FileName)}"; //Aqui sera pego o nome da imagem (dos 10 primeiros caracteres) e concantenar com a data atual e por fim adiconar a extensao (.jpg, .jpeg)

            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            };

            return imageName;
        }

        [NonAction]
        public void DeleteImage(string imageName)
        {
            var imagePath = Path.Combine(hostEnvironment.ContentRootPath, @"Resources/images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }


        /*[HttpGet("{tema}/tema")]
        public async Task<IActionResult> GetByTema(string tema)
        {
            try
            {
                var evento = await eventoService.GetAllEventosByTemaAsync(User.GetUserId(), tema, true);
                if (evento == null) return NoContent();

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar eventos. Erro: {ex.Message}");
            }
        }*/

    }
}
