namespace UcmTrackerApp.Models
{
    public class ProduccionMarvel
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Película";
        public int OrdenCronologico { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public double Calificacion { get; set; }

        public ProduccionMarvel() { }

        public ProduccionMarvel(int id, string titulo, string tipo, int ordenCronologico, string estado, double calificacion)
        {
            Id = id;
            Titulo = titulo;
            Tipo = tipo;
            OrdenCronologico = ordenCronologico;
            Estado = estado;
            Calificacion = calificacion;
        }
    }
}