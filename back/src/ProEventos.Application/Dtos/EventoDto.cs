using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos
{
    public class EventoDto //Importante citar que o DTO deve ficar na application e nao na API ou qualquer outra camada.
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório."),
        //MinLength(3, ErrorMessage = "{0} deve ter no mínimo 3 caracteres."),
        //MaxLength(50, ErrorMessage = "{0} deve ter no máximo 50 caracteres.")
        StringLength(50, MinimumLength = 3, ErrorMessage = "Intervalo permitido é de 3 a 50 caracteres.")]
        public string Tema { get; set; }

        [Display(Name = "Qtd. Pessoas")]
        [Range(1, 12000, ErrorMessage = "{0} não pode ser menor que 1 e maior que 120.000")]
        public int QtdPessoas { get; set; }

        [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "Não é uma imagem válida.")]
        public string imageURL { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Phone(ErrorMessage = "O campo {0} está com número inválido. (gif, jpg, jpeg, bmp ou png)")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "e-mail")] //Esse sera o nome que sera aplicado ao {0} abaixo.
        [EmailAddress(ErrorMessage = "O campo {0} precisa ser um e-mail válido.")]
        public string Email { get; set; }
        public IEnumerable<LoteDto> Lotes { get; set; }
        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }
        public IEnumerable<PalestranteDto> Palestrantes { get; set; }
    }
}