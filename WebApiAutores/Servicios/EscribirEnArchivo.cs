
namespace WebApiAutores.Servicios
{
    /*
     * Para que la aplicación pueda escribir en un archivo es necesario crear una carpeta con el nombre: wwwroot 
     * darle a enter y se nos creará dentro un archivo con el nombre que le hayamos dado. Si no creamos esta carpeta
     * nos dará error al ejecutar la aplicación.
     */
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo1.txt";
        //Hacer el código recurrente de la función escribir
        private Timer timer;
        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        //Cuando carguemos nuestra webApi se va a ejecutar este servicio solo 1 vez
        public Task StartAsync(CancellationToken cancellationToken)
        {
            //Inicializamos el timer al cargar nuestra webApi
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));    

            Escribir("Proceso iniciado.");
            return Task.CompletedTask;
        }

        /*
         * Cuando se apague nuestra webApi se va a ejecutar ahora este servicio solo 1 vez, no necesariamente se va a ejecutar
         * Hay situaciones excepcionales como por ejemplo si tu aplicación se detiene de manera repentina  por un error catastrófico
         * entonces no le va a dar tiempo ni chance a stopAsync a ejecutarse. En las ocasiones que sea un apagado normal se ejecutará
         * este servicio normalmente.
         */
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose(); //Para detener el timer que hemos inicializado en el StartAsync
            Escribir("Proceso finalizado.");
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Escribir("Proceso en ejecución: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        private void Escribir (string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter (ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }
    }
}
