using System;
using System.Net;
using System.DirectoryServices.Protocols;
using open_consult_challenge.Services;
using System.IO;

namespace open_consult_challenge;

class Program
{
    static void Main(string[] args)
    {
        // Declara a variável fora do try para poder fechar depois
        LdapConnection? connection = null;

        try
        {
            // Endereço do servidor LDAP (OpenDJ)
            string ldapServer = "192.168.111.151";
            int ldapPort = 1389;

            // DN (Distinguished Name) do usuário e sua senha
            string bindDn = "cn=admin";
            string password = "admin123";

            // Cria o identificador do servidor LDAP
            LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(ldapServer, ldapPort);

            // Cria as credenciais de autenticação
            NetworkCredential credential = new NetworkCredential(bindDn, password);

            // Cria a conexão LDAP
            connection = new LdapConnection(identifier, credential);

            // Define autenticação básica e protocolo LDAP v3
            connection.AuthType = AuthType.Basic;
            connection.SessionOptions.ProtocolVersion = 3;

            // Realiza a autenticação (bind)
            connection.Bind();

            Console.WriteLine("Conexão LDAP realizada com sucesso.");

            XmlParser parser = new XmlParser();
            parser.ProcessXmlFiles();
            Console.WriteLine("Processamento de XMLs concluído.");

            LdapService ldapService = new LdapService(connection);

            foreach (Models.LdapGroup group in parser.Groups)
            {
                // Criar grupo no LDAP
                ldapService.CreateGroup(group);
            }

            foreach (Models.LdapUser user in parser.Users)
            {
                // Criar usuário no LDAP
                ldapService.CreateUser(user);
            }
            
            foreach (Models.UserModification mod in parser.Modifications)
            {
                // Modificar usuário no LDAP
                ldapService.ModifyUserGroups(mod);
            }
        }
        catch (LdapException ex)
        {
            Console.WriteLine("Erro LDAP: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro geral: " + ex.Message);
        }
        finally
        {
            // Fecha e libera a conexão, se tiver sido criada
            if (connection != null)
            {
                connection.Dispose();
                Console.WriteLine("Conexão encerrada.");
            }
        }
    }
}
