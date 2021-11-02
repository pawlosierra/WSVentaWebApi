using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WSVenta.Models.Request
{
    public class VentaRequest
    {
        [Required]
        //yo quiero que el IdCliente siempre venga en las solicitudes. utilizo un dataNotation
        //NOta: tener en cuenta que como es un entero no es nulable, por lo cual siempre va venir 
        //el valor por dafault de un int no es null es 0. Podria utilizar int? seria null y podria 
        //entrar al required, el problema es que siempre va venir pordefecto en 0.
        //como podemos hacer para que se valide que sea mayor a 0?
        [Range(1, Double.MaxValue, ErrorMessage = "El valor del idCliente debe ser mayor a 0")]
        [ExisteCliente(ErrorMessage ="El cliente no existe")]
        public int IdCliente { get; set; }
        //public decimal Total { get; set; } //ya no necesitamos esta propiedad, porque ya tenemos un servicio que hace esta sumatoria.
        [Required] //podrian enviar un arreglo vacio. Se soluciona con un dataNotation
        [MinLength(1, ErrorMessage ="Debe existir conceptosS")]
        public List<Concepto> Conceptos { get; set; }
        
        //Vamos a crear un constructor por si nos envian un objeto null, el no tenga problema ya que el se va inicializar
        public VentaRequest()
        {
            this.Conceptos = new List<Concepto>();
        }
    }

    //es el conceptorequest
    public class Concepto
    {
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
        public int IdProducto { get; set; }
    }

    #region
    //vamos a crear un DataNotation
    public class ExisteClienteAttribute : ValidationAttribute 
    {
        public override bool IsValid(object value)
        {
            int idCLiente = (int)value;
            using (var db = new Models.VentaRealContext())
            {
                if (db.Clientes.Find(idCLiente) == null) return false;
            }
            return true;
        }
    }
    #endregion
}
