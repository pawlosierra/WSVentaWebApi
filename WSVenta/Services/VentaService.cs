using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WSVenta.Models;
using WSVenta.Models.Request;

namespace WSVenta.Services
{
    public class VentaService : IVentaService
    {
        public void Add(VentaRequest model)
        {
            //try //ESTE TRY CATCH NO LO NECESITAMOS PORQUE ES EL GLOBAL
            //{
                //con entityFramework vamos a hacer que se metan las ventas
                //primero creamos el context para hacer la conexion
                using (VentaRealContext db = new VentaRealContext())
                {
                    //vamos hacer una transaccion. Todas las operaciones o nada.
                    //cuando se hace una transaccion se bloquean las tablas que se estan
                    //usando, con el objetivo de que se trabaje unicamente con una informacion.
                    //cuando se termine la transaccion, esto se informara para que 
                    //si alguien necesita de la informacion, la pueda utizar. 
                    //SE EVITA QUE EXISTA INFORMACION BASURA. 
                    //Estas ventajas no se encuentran en bases de datos no relacionales.NoSQL

                    //las ventajas de utilizar transacciones eEntityFramework al lado del backend
                    //es que podemos evaluar otras cosas como que se envie un correo electronico,
                    //que haga una notificacion PUSH o algo critico que deba estar si o si, para que 
                    //los datos esten en la base de datos. y si no estan borramos esos datos.

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var venta = new Ventum();
                            //venta.Total = model.Total; primero lo enviamos por PostMan. Pero realmente se debe calcular en el backend.
                            venta.Total = model.Conceptos.Sum(d => d.Cantidad * d.PrecioUnitario);
                            venta.Fecha = DateTime.Now;
                            venta.IdCliente = model.IdCliente;
                            db.Venta.Add(venta);
                            //al momento que hago un SaveChanges, entityFramework le va asignar al objeto venta un Id
                            //se lo asigna porque en la base de datos le pusimos autoincrement el primarykey
                            //
                            db.SaveChanges();

                            //para los otros elementos
                            foreach (var modelConcepto in model.Conceptos)
                            {
                                var concepto = new Models.Concepto();
                                //quiero el concepto que esta dentro de models no la que esta dentro de models.request
                                concepto.Cantidad = modelConcepto.Cantidad;
                                concepto.IdProducto = modelConcepto.IdProducto;
                                concepto.PrecioUnitario = modelConcepto.PrecioUnitario;
                                concepto.Importe = modelConcepto.Importe;
                                concepto.IdVenta = venta.Id;

                                db.Conceptos.Add(concepto);
                                db.SaveChanges();
                            }
                            transaction.Commit();//este comit dice que se acabo la transaccion
                            //respuesta.Exito = 1;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            //debemos informarle al controlador que ha pasado algo
                            throw new Exception("Ocurrio un error en la insercion");
                        }
                    }
                    //cuando se tiene muchos campos que insertar en la base de datos se utiliza AutoMapper  
                }
            //}
            //catch (Exception ex) ESTE TRY CATCH NO LO NECESITAMOS PORQUE ES EL GLOBAL
            //{
            //    respuesta.Mensaje = ex.Message;
            //}
        }
    }
}
