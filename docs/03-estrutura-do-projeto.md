# Estrutura do Projeto

Este documento descreve a estrutura do projeto desenvolvido para o desafio técnico da Open Consult, com o objetivo de facilitar a compreensão do código, a manutenção e futuras melhorias. O projeto foi implementado utilizando a linguagem C# com .NET, consumindo arquivos XML e integrando-se a um servidor LDAP (OpenDJ) para gerenciamento de usuários e grupos.

## 1. Organização Geral

A estrutura do repositório segue boas práticas de organização de projetos em .NET, contendo os seguintes diretórios e arquivos principais:

```
├── bin/                   # Arquivos de build gerados automaticamente
├── obj/                   # Arquivos intermediários de compilação
├── docs/                  # Documentação e imagens do guia
│   └── img/               # Capturas de tela do processo de configuração
├── xml/                   # Arquivos XML utilizados como entrada
├── Models/                # Classes de modelo (ex: LdapUser, LdapGroup, UserModification)
├── Services/              # Lógica de negócio e integração com LDAP (ex: LdapService, XmlParser)
├── Program.cs             # Ponto de entrada da aplicação
├── open-consult-challenge.csproj
├── open-consult-challenge.sln
├── README.md              # Apresentação geral do projeto
├── LICENSE                # Licença do projeto
```

## 2. Principais Componentes

### 2.1 `Models/`
Contém as representações dos dados extraídos dos arquivos XML, mapeando atributos relevantes conforme os esquemas LDAP:

- `LdapUser.cs` – representa um usuário no diretório.
- `LdapGroup.cs` – representa um grupo.
- `UserModification.cs` – representa modificações (adição/remoção de grupos para um usuário).

### 2.2 `Services/`
Agrupa a lógica central da aplicação:

- `XmlParser.cs` – realiza a leitura e interpretação dos arquivos XML utilizando XPath.
- `LdapService.cs` – responsável por interagir com o servidor OpenDJ via `System.DirectoryServices.Protocols`.

### 2.3 `Program.cs`

Responsável por:
- Inicializar a conexão LDAP;
- Instanciar os serviços;
- Executar o processamento dos XMLs;
- Imprimir logs de cada etapa.

## 3. Considerações Técnicas

- O projeto utiliza `System.Xml.XPath` para navegação e extração dos dados nos arquivos XML.
- As expressões regulares foram aplicadas para garantir que campos como nome e login contenham apenas letras, e telefone apenas números.
- A conexão LDAP foi implementada de forma explícita, utilizando classes nativas do .NET para manter a transparência e controle sobre os comandos enviados ao servidor.


## 4. Boas Práticas Aplicadas

- Organização em camadas (Models, Services);
- Separação de responsabilidades;
- Código comentado com explicações claras;
- Nomes de métodos e classes em inglês para padrão internacional;
- Utilização de estruturas de controle de fluxo para lidar com erros e logs.

---

Este documento complementa o guia de início rápido e serve como referência para desenvolvedores que desejam compreender ou dar continuidade ao projeto.

