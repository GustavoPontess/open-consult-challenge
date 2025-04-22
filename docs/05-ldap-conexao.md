# Conexão com o Servidor LDAP usando System.DirectoryServices.Protocols

## Introdução

Aqui eu vou explicar como fiz a conexão com o servidor LDAP no projeto. A ideia era autenticar e interagir com o OpenDJ diretamente do C#, então precisei de uma biblioteca que me desse controle total sobre o protocolo LDAP. Foi aí que encontrei a `System.DirectoryServices.Protocols`, que é uma opção oficial da Microsoft para esse tipo de tarefa.

---

## Por que usei System.DirectoryServices.Protocols

Antes de decidir pela `System.DirectoryServices.Protocols`, eu tive certa dificuldade porque não encontrei muitos exemplos prontos na internet usando essa biblioteca em projetos modernos. A maioria dos tutoriais que encontrei usava bibliotecas mais antigas ou muito abstratas, e nenhuma delas me dava a flexibilidade que eu precisava. Tive que pesquisar bastante, ler a documentação e entender como a API funcionava de verdade.

Dois exemplos que me ajudaram a entender a base da conexão foram:
- [Exemplo no Gist](https://gist.github.com/dzitkowskik/279164c1343e652660dc)
- [Repositório LdapConnectionConsole](https://github.com/emilaa/LdapConnectionConsole/blob/master/LdapConnect/Program.cs)

Com esses exemplos, comecei a cruzar o que eu via com a documentação da própria Microsoft, principalmente a seção de `System.DirectoryServices.Protocols`, que foi essencial para entender como as classes `LdapConnection`, `LdapDirectoryIdentifier` e `NetworkCredential` se conectavam entre si:

📚 [Documentação geral da API](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols?view=net-9.0-pp)

📚 [Construtor do LdapConnection com parâmetros](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.ldapconnection.-ctor?view=net-9.0-pp#system-directoryservices-protocols-ldapconnection-ctor(system-directoryservices-protocols-ldapdirectoryidentifier-system-net-networkcredential))

---

## Como implementei a conexão

A conexão foi implementada diretamente no `Program.cs`. Eu comecei declarando a variável `LdapConnection` fora do `try`, porque queria garantir que poderia fechar a conexão corretamente depois, no `finally`.

Dentro do `try`, fiz o seguinte passo a passo:

1. Declarei as configurações básicas:
```csharp
string ldapServer = "192.168.111.151";
int ldapPort = 1389;
string bindDn = "cn=admin";
string password = "admin123";
```

2. Criei o identificador do servidor:
```csharp
LdapDirectoryIdentifier identifier = new LdapDirectoryIdentifier(ldapServer, ldapPort);
```

3. Criei as credenciais de acesso:
```csharp
NetworkCredential credential = new NetworkCredential(bindDn, password);
```

4. Instanciei a conexão passando o identificador e as credenciais:
```csharp
connection = new LdapConnection(identifier, credential);
```

5. Configurei o tipo de autenticação como básico:
```csharp
connection.AuthType = AuthType.Basic;
```

6. Defini explicitamente o protocolo LDAP versão 3:
```csharp
connection.SessionOptions.ProtocolVersion = 3;
```

7. E por fim, fiz o bind para autenticar no servidor:
```csharp
connection.Bind();
```

Esse fluxo ficou limpo e direto, e me permitiu seguir com o uso da conexão ao longo da aplicação sem complicações.

---

## Conclusão

Com o `System.DirectoryServices.Protocols`, eu consegui controlar toda a comunicação LDAP sem depender de bibliotecas externas ou ferramentas de terceiros. Isso me deu segurança e liberdade para montar exatamente o que o desafio pedia. Mesmo tendo começado com poucas referências, estudar a documentação da Microsoft e testar na prática me ajudou a construir uma solução funcional e confiável.

