namespace open_consult_challenge.Models
{
    public class UserModification : LdapUser
    {
        public List<string> GroupsToAdd { get; set; } = new List<string>();
        public List<string> GroupsToRemove { get; set; } = new List<string>();
    }
}
