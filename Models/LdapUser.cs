namespace open_consult_challenge.Models
{
    public class LdapUser
    {
        public string FullName { get; set; } = string.Empty;     // Nome Completo (cn)
        public string Login { get; set; } = string.Empty;        // Identificador do usuário (uid)
        public string Phone { get; set; } = string.Empty;        // Telefone (telephoneNumber)
        public List<string> Groups { get; set; } = new List<string>(); // Lista de grupos associados
    }
}
