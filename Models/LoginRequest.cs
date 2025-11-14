namespace InventarioAPI.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) usado para receber
    /// as credenciais nas rotas de /register e /login.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nome de usuário.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Senha (em texto puro, vinda do frontend).
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}