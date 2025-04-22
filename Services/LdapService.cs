using System;
using System.DirectoryServices.Protocols;
using open_consult_challenge.Models;

namespace open_consult_challenge.Services
{
    public class LdapService
    {
        private readonly LdapConnection _connection;
        private readonly string _baseDn = "dc=openconsult,dc=local,dc=com";

        public LdapService(LdapConnection connection)
        {
            _connection = connection;
        }

        public void CreateGroup(LdapGroup group)
        {
            string ouName = "groups";
            string groupDn = $"cn={group.Identifier},ou={ouName},{_baseDn}";

            AddRequest request = new AddRequest(groupDn,
                new DirectoryAttribute("objectClass", "top", "groupOfNames"),
                new DirectoryAttribute("cn", group.Identifier),
                new DirectoryAttribute("description", group.Description ?? ""),
                new DirectoryAttribute("member", $"cn=fake,{_baseDn}") // obrigatório no OpenDJ
            );

            try
            {
                _connection.SendRequest(request);
                Console.WriteLine($"Grupo criado: {group.Identifier}");
            }
            catch (DirectoryOperationException ex)
            {
                Console.WriteLine($"Erro ao criar grupo '{group.Identifier}': {ex.Message}");
            }
        }

        public void CreateUser(LdapUser user)
        {
            string ouName = "users";
            string userDn = $"uid={user.Login},ou={ouName},{_baseDn}";

            string sn = user.FullName.Split(' ').LastOrDefault() ?? ".";

            AddRequest request = new AddRequest(userDn,
                new DirectoryAttribute("objectClass", "top", "person", "organizationalPerson", "inetOrgPerson"),
                new DirectoryAttribute("uid", user.Login),
                new DirectoryAttribute("cn", user.FullName),
                new DirectoryAttribute("sn", sn),
                new DirectoryAttribute("telephoneNumber", user.Phone),
                new DirectoryAttribute("ou", ouName)
            );

            try
            {
                _connection.SendRequest(request);
                Console.WriteLine($"Usuário criado: {user.Login}");
            }
            catch (DirectoryOperationException ex)
            {
                Console.WriteLine($"Erro ao criar usuário '{user.Login}': {ex.Message}");
                return;
            }

            // Adiciona o usuário aos grupos logo após a criação
            foreach (string groupName in user.Groups)
            {
                string groupDn = $"cn={groupName},ou=groups,{_baseDn}";

                DirectoryAttributeModification addModification = new DirectoryAttributeModification
                {
                    Name = "member",
                    Operation = DirectoryAttributeOperation.Add
                };
                addModification.Add(userDn);

                ModifyRequest modifyRequest = new ModifyRequest(groupDn, addModification);

                try
                {
                    _connection.SendRequest(modifyRequest);
                    Console.WriteLine($"Adicionado '{user.Login}' ao grupo '{groupName}'.");
                }
                catch (DirectoryOperationException ex)
                {
                    Console.WriteLine($"Erro ao adicionar '{user.Login}' ao grupo '{groupName}': {ex.Message}");
                }
            }
        }

        public void ModifyUserGroups(UserModification mod)
        {
            string userDn = $"uid={mod.Login},ou=users,{_baseDn}";

            // Verifica se o usuário existe no LDAP antes de modificar
            SearchRequest checkRequest = new SearchRequest(userDn, "(objectClass=inetOrgPerson)", SearchScope.Base);

            try
            {
                SearchResponse response = (SearchResponse)_connection.SendRequest(checkRequest);
                if (response.Entries.Count == 0)
                {
                    Console.WriteLine($"Usuário '{mod.Login}' não existe. Modificação ignorada.");
                    return;
                }
            }
            catch (DirectoryOperationException ex)
            {
                Console.WriteLine($"Erro ao verificar existência do usuário '{mod.Login}': {ex.Message}");
                return;
            }

            // A partir daqui, usuário existe — pode modificar
            foreach (string groupName in mod.GroupsToAdd)
            {
                string groupDn = $"cn={groupName},ou=groups,{_baseDn}";

                DirectoryAttributeModification addModification = new DirectoryAttributeModification
                {
                    Name = "member",
                    Operation = DirectoryAttributeOperation.Add
                };
                addModification.Add(userDn);

                ModifyRequest modifyRequest = new ModifyRequest(groupDn, addModification);

                try
                {
                    _connection.SendRequest(modifyRequest);
                    Console.WriteLine($"Adicionado '{mod.Login}' ao grupo '{groupName}'.");
                }
                catch (DirectoryOperationException ex)
                {
                    Console.WriteLine($"Erro ao adicionar '{mod.Login}' ao grupo '{groupName}': {ex.Message}");
                }
            }

            foreach (string groupName in mod.GroupsToRemove)
            {
                string groupDn = $"cn={groupName},ou=groups,{_baseDn}";

                DirectoryAttributeModification removeModification = new DirectoryAttributeModification
                {
                    Name = "member",
                    Operation = DirectoryAttributeOperation.Delete
                };
                removeModification.Add(userDn);

                ModifyRequest modifyRequest = new ModifyRequest(groupDn, removeModification);

                try
                {
                    _connection.SendRequest(modifyRequest);
                    Console.WriteLine($"Removido '{mod.Login}' do grupo '{groupName}'.");
                }
                catch (DirectoryOperationException ex)
                {
                    Console.WriteLine($"Erro ao remover '{mod.Login}' do grupo '{groupName}': {ex.Message}");
                }
            }
        }
    }
}
