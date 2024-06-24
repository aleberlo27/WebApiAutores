namespace WebApiAutores.DTOs
{
    /*
     * Clase base para nuestros DTOs que necesitan utilizar la clase DatoHATEOAS
     */
    public class Recurso
    {
        public List<DateHATEOAS> Enlaces { get; set; } = new List<DateHATEOAS>();
    }
}
