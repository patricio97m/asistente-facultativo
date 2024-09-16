namespace Entidades
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public float CargaHoraria { get; set; }
        public string Comisiones { get; set; }
        public float PromedioPromocion { get; set; }
        public Materia(string nombre) { }
    }
}
