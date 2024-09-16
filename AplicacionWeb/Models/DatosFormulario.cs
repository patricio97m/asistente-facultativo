namespace AplicacionWeb.Models
{
    public class MateriaSeleccionada
    {
        public string ?Id { get; set; }
        public string ?Comision { get; set; }
    }

    public class MateriaSeleccionadaData
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int comision { get; set; }
        public double promedioPromocionMateria { get; set; }
        public bool prediccionPromocion { get; set; }
        public string inicio { get; set; }
        public string fin { get; set; }

        public MateriaSeleccionadaData(int id, int comision, double promedioPromocionMateria)
        {
            this.id = id;
            this.comision = comision;
            this.promedioPromocionMateria = promedioPromocionMateria;
        }

    }

}
