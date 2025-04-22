# Desafio Técnico Open Consult

## Índice
1. [Readme]()
2. [Guia de Início Rápido](docs/02-guia-de-inicio-rapido.md)
3. [Estrutura do Projeto](docs/03-estrutura-do-projeto.md)
4. [Leitura e Validação de XML](docs/04-xml-leitura-validacao.md)
5. [Conexão LDAP](docs/05-ldap-conexao.md)
6. [Criação de Objetos LDAP](docs/06-ldap-criacao-objetos.md)
7. [Modificação de Grupos LDAP](docs/07-ldap-modificacao-grupos.md)

## Desafio Proposto

O desafio técnico proposto consiste no desenvolvimento de uma aplicação utilizando a linguagem de preferência do candidato (Java, C# ou outra), com os seguintes objetivos:

- Ler arquivos de texto em formato XML;
- Interpretar e executar operações correspondentes em uma base LDAP (pode ser OpenDJ, OpenLDAP, eDirectory, Active Directory ou outra);

### Condições obrigatórias estabelecidas:

1. A leitura e interpretação dos arquivos XML deve ser feita utilizando **XPath** ou ferramenta equivalente;
2. As **classes e atributos utilizados no LDAP devem ser nativos** da solução escolhida (no caso deste projeto, o OpenDJ);
3. Campos relacionados a **nome e login devem conter apenas letras**, e **telefone deve conter apenas números**, sendo obrigatória a utilização de **expressões regulares** para validação;
4. **Usuários e grupos devem estar em containers distintos**, ou seja, organizados em unidades organizacionais separadas no LDAP;
5. A solução será avaliada com base na **funcionalidade implementada, estrutura lógica** e **domínio dos dados** manipulados.

---

## Tecnologias Utilizadas

- **Linguagem de Programação**: C#
- **Plataforma .NET**: .NET 9
- **Servidor LDAP**: OpenDJ
- **Ambiente Linux**: Ubuntu 22.04 via WSL (Windows Subsystem for Linux)
- **Ambiente de Desenvolvimento**: Visual Studio Code
- **Bibliotecas principais**:
  - `System.DirectoryServices.Protocols` – para comunicação direta com o servidor LDAP;
  - `System.Xml.XPath` – para leitura e navegação estruturada nos arquivos XML;
  - `System.Text.RegularExpressions` – para sanitização e validação de dados como nome, login e telefone;
- **Padrões e boas práticas**:
  - Organização em camadas (Models, Services, XMLs);
  - Validação e tratamento de exceções;
  - Separação clara entre lógica de parsing XML e integração com LDAP;
  - Criação e modificação de objetos LDAP respeitando o schema nativo (como `inetOrgPerson` e `groupOfNames`).

---

## Guia Rápido de Execução Local

Siga estas etapas simples para colocar o projeto em funcionamento em sua máquina local:

📄 [Guia de Instalação e Configuração do Ambiente](docs/02-guia-de-inicio-rapido.md)

