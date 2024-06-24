namespace WebApiAutores.DTOs
{
    /*
     * Esta clase representa una colección de recursos
     * 
     * Utilizamos genéricos como el tipo 'T' que heredará de Recurso y una coleccion de recursos que nosotros tengamos como por 
     * ejemplo una coleccion de autores, ese autor va a tener que implementar la clase Recurso.
     */
    public class ColeccionDeRecursos<T> : Recurso where T : Recurso
    {
        public List<T> Valores { get; set; }
    }
}
