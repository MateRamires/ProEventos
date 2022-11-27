using System.Collections.Generic;

namespace ProEventos.Application.Dtos
{
    public class EventoDto //Importante citar que o DTO deve ficar na application e nao na API ou qualquer outra camada.
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public string DataEvento { get; set; } 
        public string Tema { get; set; }
        public int QtdPessoas { get; set; }
        public string imageURL { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public IEnumerable<LoteDto> Lotes { get; set; }
        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }
        public IEnumerable<PalestranteDto> Palestrantes { get; set; }
    }
}