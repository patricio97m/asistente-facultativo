namespace AplicacionWeb.Models
{

    public class MateriaModel
    {
        public int ID { get; set; }
        public string ?Nombre { get; set; }
        public float CargaHoraria { get; set; }
        public List<_Comision> ?Comisiones { get; set; } = new List<_Comision>();
    }

    public class _Comision
    {
        public int Id { get; set; }
        public string ?diasyhorarios { get; set; }

        public _Comision(int id, string diasyhorarios)
        {
            Id = id;
            this.diasyhorarios = diasyhorarios;
        }
    }


    public class Comision
    {
        public int Id { get; set; }
        public List<DiaYHorario> ?diasyhorarios { get; set; }
    }

    public class DiaYHorario
    {
        public string ?dia { get; set; }
        public string ?horario { get; set; }
    }

}
