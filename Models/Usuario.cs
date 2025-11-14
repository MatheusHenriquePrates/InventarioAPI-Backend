using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    /// <summary>
    /// Representa um usuário do sistema com credenciais.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único (Chave Primária).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário (único) para login.
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// O hash BCrypt da senha do usuário. (Nunca salvar a senha pura!)
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty; 
    }
}