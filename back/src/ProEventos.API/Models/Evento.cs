namespace ProEventos.API.Models //Quando um arquivo fizer uso desse modelo, ele tera que ter um using <esse_namespace> para utilizar a classe Evento.
{
    public class Evento
    {
        public int EventoId { get; set; }
        public string Local { get; set; }
        public string DataEvento { get; set; }
        public string Tema { get; set; }
        public int QtdPessoas { get; set; }
        public string Lote { get; set; }
        public string imageURL { get; set; }
    }
}