namespace WebApiAutores.DTOs
{
    public class RespuestaAutentificacion
    {
        public string Token { get; set; }   //Utilizamos el esquema de autentificacion Bearer y en éste se utilizan Tokens 
        public DateTime Expiracion { get; set; } //El token no necesariamente tiene que durar para siempre, le podemos dar una fecha de caducación

    }
}
