using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutores.DTOs;

namespace WebApiAutores.Servicios
{
    /*
     * Una función hash es un algoritmo el cual transforma un mensaje de tal manera que es virtualmente imposible de descifrar
     * revirtiendo el proceso
     * 
     * Una razón para utilizar una función hash es para verificar password (cuando nos registramos en una pagina web típicamente 
     * proveemos un correo electrónico y un password, lo correcto desde el punto de vista de la seguridad informática es que la web 
     * nunca guarde el texto plano de la password sino que guarde el hash de tal modo que sea virtualmente imposible conocer el texto plano original.
     * 
     * Para validar el usuario es coger el password, aplicarle la función hash y comparar el hash resultante con el que tenemos guardado en la DB.
     * 
     * A la hora de implementar la función de hash hay que tener en cuenta la SAL:
     * Una SAL es un valor aleatorio que se anexa al texto plano al cual le queremos aplicar la función Hash. 
     * 
     * Le vamos a aplicar la función Hash a un string y concatenarle la opción SAL, no necesitamos utilizar el DataProtection del método configureServices
     * para utilizar la función de Hash. Lo que haremos es utilizar una implementación del algoritmo: Pbkdf2 que el framework nos ofrece. Utilizaremos una
     * sal aleatoria que hará que incluso hashes realizados al mismo texto plano de resultados distintos.
     * 
     * Si vas a guardar un hash en la DB, guardarás también la sal también.
     */
    public class HashService
    {
        //ResultadoHash es un DTO
        public ResultadoHash Hash (string textoPlano)
        {
            var sal = new byte[16];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes (sal);
            }

            return Hash(textoPlano, sal);
        }

        public ResultadoHash Hash(string textoPlano, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(password: textoPlano,
                salt: sal, prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000, numBytesRequested: 32);

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash()
            {
                Hash = hash,    
                Sal = sal
            };
        }
    }
}
