namespace WebApiAutores.DTOs
{
    /*
     * HATEOAS es una restricción REST que nos sirve para ayudar a los clientes de nuestra WebAPI a navegar 
     * por nuestros recursos y saber qué son capaces de hacer.
     */
    public class DateHATEOAS
    {
        public string Enlace {  get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }

        //Hacemos los set privados para poder crear una instancia en esta clase y no poder modificarla desde fuera
        public DateHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion; 
            Metodo = metodo;
        }

    }
}
