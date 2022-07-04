using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProEventos.API.Data;
using ProEventos.API.Models; //Como estamos fazendo uso de eventos, temos que dar esse using na pasta dos modelos, onde esta localizado a classe Evento.

namespace ProEventos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //Aqui esta a rota, e pode ser alterado
    public class EventosController : ControllerBase
    {
        /*public IEnumerable<Evento> _evento = new Evento[] { //Aqui estamos instanciando um array do tipo Evento, para ter dentro dele, todos os eventos.
         new Evento() {
            EventoId = 1,
            Tema = "Angular e .NET 5",
            Local = "Belo Horizonte",
            Lote = "1 Lote",
            QtdPessoas = 250,
            DataEvento = DateTime.Now.AddDays(2).ToString("dd/MM//yyyy"),
            imageURL = "foto.png"
         },
         new Evento() {
            EventoId = 2,
            Tema = "Angular e suas novidades",
            Local = "Sao Paulo",
            Lote = "2 Lote",
            QtdPessoas = 210,
            DataEvento = DateTime.Now.AddDays(3).ToString("dd/MM//yyyy"),
            imageURL = "foto2.png"
         }
      };*/ //Foi comentada toda essa linha, pois depois da aula 23, nos estamos pegando os dados do proprio DB que eh o context.
      
        private readonly DataContext contexto;

        public EventosController(DataContext contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet]
        public IEnumerable<Evento> Get() //Evento eh o tipo desse metodo, ou seja, por ser um get, ele retornara um evento.
        {
            return contexto.Eventos; //Antigo: Essa rota retorna todos os eventos, pois ela esta retornando _eventos que eh um array que contem todos os eventos como foi declaro mais acima no codigo.
        }

        [HttpGet("{id}")]
        public Evento Get(int id) //Esse metodo pode ter o mesmo nome que o de cima por causa da sobrecarga de metodos. Um tem 0 parametros e esse tem 1.
        {
            return contexto.Eventos.FirstOrDefault(evento => evento.EventoId == id); //Aqui ele tambem esta retornando um evento, so que com o id especificado na URL.
        }

        [HttpPost] //Aqui eh referenciado qual metodo do http que tera a funcao abaixo.
        public string Post() //Essa eh a funcao que ira ser chamada ao usar o metodo Post no EventoControler.
        {
            return "Exemplo de Post";
        }

        [HttpPut("{id}")] //Nesse metodo put estamos tambem pedindo para passar o parametro id.
        public String Put(int id)
        {
            return $"Exemplo de Put com id = {id}";
        }


        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return $"Exemplo de Delete com id = {id}";
        }

    }
}
