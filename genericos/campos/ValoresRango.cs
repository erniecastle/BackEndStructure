

namespace Exitosw.Payroll.Core.campos
{
    public class ValoresRango
    {

        public ValoresRango()
        {

        }

        public ValoresRango(int inicio, int cantidad)
        {
            this.inicio = inicio;
            this.cantidad = cantidad;
        }

        public int inicio { get; set; }

        public int cantidad { get; set; }

    }
}
