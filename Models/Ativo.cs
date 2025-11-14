namespace InventarioAPI.Models
{
    /// <summary>
    /// Representa um ativo de TI no inventário.
    /// </summary>
    public class Ativo
    {
        /// <summary>
        /// Identificador único (Chave Primária).
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        /// O nome de host da máquina (ex: "SRV-WEB01").
        /// </summary>
        public string NomeDoHost { get; set; } = string.Empty;
        
        /// <summary>
        /// O sistema operacional do ativo.
        /// </summary>
        public string SistemaOperacional { get; set; } = string.Empty;
        
        /// <summary>
        /// O status atual do ativo (ex: "Ativo", "Inativo", "Em Manutenção").
        /// </summary>
        public string Status { get; set; } = "Ativo"; 
        
        /// <summary>
        /// Data e hora (UTC) de quando o ativo foi cadastrado.
        /// </summary>
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    }
}