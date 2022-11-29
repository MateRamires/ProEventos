using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos
{
    public class EventoDto //Importante citar que o DTO deve ficar na application e nao na API ou qualquer outra camada.
    {
        public int Id { get; set; }
        public string Local { get; set; }
        public string DataEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} � obrigat�rio."),
        //MinLength(3, ErrorMessage = "{0} deve ter no m�nimo 3 caracteres."),
        //MaxLength(50, ErrorMessage = "{0} deve ter no m�ximo 50 caracteres.")
        StringLength(50, MinimumLength = 3, ErrorMessage = "Intervalo permitido � de 3 a 50 caracteres.")]
        public string Tema { get; set; }

        [Display(Name = "Qtd. Pessoas")]
        [Range(1, 12000, ErrorMessage = "{0} n�o pode ser menor que 1 e maior que 120.000")]
        public int QtdPessoas { get; set; }

        [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "N�o � uma imagem v�lida.")]
        public string imageURL { get; set; }

        [Required(ErrorMessage = "O campo {0} � obrigat�rio.")]
        [Phone(ErrorMessage = "O campo {0} est� com n�mero inv�lido. (gif, jpg, jpeg, bmp ou png)")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo {0} � obrigat�rio.")]
        [Display(Name = "e-mail")] //Esse sera o nome que sera aplicado ao {0} abaixo.
        [EmailAddress(ErrorMessage = "O campo {0} precisa ser um e-mail v�lido.")]
        public string Email { get; set; }
        public IEnumerable<LoteDto> Lotes { get; set; }
        public IEnumerable<RedeSocialDto> RedesSociais { get; set; }
        public IEnumerable<PalestranteDto> Palestrantes { get; set; }
    }
}