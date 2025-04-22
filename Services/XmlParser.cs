using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using open_consult_challenge.Models;

namespace open_consult_challenge.Services
{
    public class XmlParser
    {
        private readonly string xmlDirectoryPath = @"E:\VSCode\open-consult-challenge\xml"; // ajuste conforme necessário

        public List<LdapUser> Users { get; private set; } = new List<LdapUser>();
        public List<LdapGroup> Groups { get; private set; } = new List<LdapGroup>();
        public List<UserModification> Modifications { get; private set; } = new List<UserModification>();

        public void ProcessXmlFiles()
        {
            if (!Directory.Exists(xmlDirectoryPath))
            {
                Console.WriteLine($"Directory not found: {xmlDirectoryPath}");
                return;
            }

            string[] xmlFiles = Directory.GetFiles(xmlDirectoryPath, "*.xml");

            foreach (string filePath in xmlFiles)
            {
                try
                {
                    XPathDocument document = new XPathDocument(filePath);
                    XPathNavigator navigator = document.CreateNavigator();
                    XPathNavigator? root = navigator.SelectSingleNode("/*");

                    if (root == null)
                    {
                        continue;
                    }

                    string rootTag = root.Name;
                    string className = root.GetAttribute("class-name", "");

                    if (rootTag == "add")
                    {
                        if (className == "Grupo")
                        {
                            LdapGroup? group = ParseGroup(root);
                            if (group != null)
                            {
                                Groups.Add(group);
                            }
                        }
                        else if (className == "Usuario")
                        {
                            LdapUser? user = ParseUser(root);
                            if (user != null)
                            {
                                Users.Add(user);
                            }
                        }
                    }
                    else if (rootTag == "modify" && className == "Usuario")
                    {
                        UserModification? mod = ParseUserModification(root);
                        if (mod != null)
                        {
                            Modifications.Add(mod);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while processing '{Path.GetFileName(filePath)}': {ex.Message}");
                }
            }
        }

        private LdapGroup? ParseGroup(XPathNavigator root)
        {
            string id = root.SelectSingleNode("add-attr[@attr-name='Identificador']/value")?.Value ?? "";
            string description = root.SelectSingleNode("add-attr[@attr-name='Descricao']/value")?.Value ?? "";

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return new LdapGroup
            {
                Identifier = id,
                Description = description
            };
        }

        private LdapUser? ParseUser(XPathNavigator root)
        {
            string fullNameRaw = root.SelectSingleNode("add-attr[@attr-name='Nome Completo']/value")?.Value ?? "";
            string loginRaw = root.SelectSingleNode("add-attr[@attr-name='Login']/value")?.Value ?? "";
            string phoneRaw = root.SelectSingleNode("add-attr[@attr-name='Telefone']/value")?.Value ?? "";

            // Limpa os campos conforme suas regras
            string fullName = Regex.Replace(fullNameRaw, @"\d+", "");          // remove números do nome
            string login = Regex.Replace(loginRaw, @"[^A-Za-z]", "");          // remove tudo exceto letras
            string phone = Regex.Replace(phoneRaw, @"[^\d]", "");              // remove tudo que não for número

            // Garante que os campos essenciais não ficaram vazios após limpeza
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(phone))
            {
                Console.WriteLine($"Usuário ignorado após sanitização. Nome='{fullName}', Login='{login}', Telefone='{phone}'");
                return null;
            }

            // Grupos
            List<string> groupList = new List<string>();
            XPathNodeIterator groupNodes = root.Select("add-attr[@attr-name='Grupo']/value");
            while (groupNodes.MoveNext())
            {
                if (!string.IsNullOrWhiteSpace(groupNodes.Current?.Value))
                {
                    groupList.Add(groupNodes.Current.Value);
                }
            }

            return new LdapUser
            {
                FullName = fullName,
                Login = login,
                Phone = phone,
                Groups = groupList
            };
        }

        private UserModification? ParseUserModification(XPathNavigator root)
        {
            string loginRaw = root.SelectSingleNode("association")?.Value ?? "";

            // Limpeza do login: remove tudo que não for letra
            string login = Regex.Replace(loginRaw, @"[^A-Za-z]", "");

            if (string.IsNullOrWhiteSpace(login))
            {
                Console.WriteLine($"Login inválido ou vazio após limpeza: '{loginRaw}'");
                return null;
            }

            List<string> groupsToRemove = new List<string>();
            XPathNodeIterator removeNodes = root.Select("modify-attr[@attr-name='Grupo']/remove-value/value");
            while (removeNodes.MoveNext())
            {
                if (!string.IsNullOrWhiteSpace(removeNodes.Current?.Value))
                {
                    groupsToRemove.Add(removeNodes.Current.Value);
                }
            }

            List<string> groupsToAdd = new List<string>();
            XPathNodeIterator addNodes = root.Select("modify-attr[@attr-name='Grupo']/add-value/value");
            while (addNodes.MoveNext())
            {
                if (!string.IsNullOrWhiteSpace(addNodes.Current?.Value))
                {
                    groupsToAdd.Add(addNodes.Current.Value);
                }
            }

            return new UserModification
            {
                Login = login,
                GroupsToAdd = groupsToAdd,
                GroupsToRemove = groupsToRemove
            };
        }
    }
}