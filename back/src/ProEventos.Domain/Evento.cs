using System;
using System.Collections.Generic;

namespace ProEventos.Domain //Quando um arquivo fizer uso desse modelo, ele tera que ter um using <esse_namespace> para utilizar a classe Evento.
{
    //[Table("EventosDetalhes")] //Esse sera o nome dessa classe no banco de dados.
    public class Evento
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public DateTime? DataEvento { get; set; } //Ponto de interrogacao indica que pode ser um valor nulo para este campo.

        //[Required] 
        //[MaxLength(50)]  Essas sao Data Annotations boas para o banco de dados.
        public string Tema { get; set; }
        public int QtdPessoas { get; set; }
        public string imageURL { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public IEnumerable<Lote> Lotes { get; set; } //IEnumerable eh usado quando se tem diversos de uma coisa, nesse caso, tem diversos lotes em um Evento, entao se usa o IEnumerable. 
        public IEnumerable<RedeSocial> RedesSociais { get; set; }
        public IEnumerable<PalestranteEvento> PalestrantesEventos { get; set; }

        /* [NotMapped] //Indica que o campo NAO estara presente no banco de dados.
        public int ContagemDias { get; set; } */

    }
}