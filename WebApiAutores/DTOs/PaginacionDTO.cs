namespace WebApiAutores.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina {  get; set; }
        private int recordsPorPagina = 10;
        private readonly int cantidadMaximaPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            {
                /*
                 * si valor es 100 (te lo pasa el usuario) es mayor que la cantidad máxima entonces retorna solo el valor de la cantidadMaxima PP
                 * Si el valor que pide el usuario es mejor que el máximo se cambiará a falso entonces se irá al valor que quiere visualizar
                 */
                recordsPorPagina = (value > cantidadMaximaPorPagina) ? cantidadMaximaPorPagina : value;
            }
        }
    }
}
