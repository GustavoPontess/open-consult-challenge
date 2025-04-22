# Criação de Objetos no LDAP – Usuários e Grupos

## Introdução

Quando chegou a parte de criar usuários e grupos dentro do LDAP, eu precisei entender como o OpenDJ esperava receber essas entradas. O desafio pedia que fossem utilizados os objetos nativos da ferramenta, então comecei a procurar na documentação oficial quais tipos existiam e quais atributos eram obrigatórios para cada um.

Foi aí que encontrei, na documentação de schema do OpenDJ, referências aos objetos `inetOrgPerson` e `groupOfNames`. Quando encontrei esses nomes apareciam como exemplos tanto na documentação do OpenDJ quanto em fóruns e tutoriais relacionados a diretórios LDAP. Comecei a focar neles porque pareciam os mais compatíveis com o que o desafio pedia: um tipo para representar usuários e outro para grupos.

📚 [Documentação – OpenDJ Schema Reference](https://backstage.forgerock.com/docs/opendj/7.0/schema-reference/)

---

## Como cheguei nos tipos `inetOrgPerson` e `groupOfNames`

Durante o desenvolvimento, eu também encontrei uma ferramenta gráfica muito útil dentro do próprio OpenDJ: o menu **Manage Schema**, acessível pelo painel de administração. Nele, consegui explorar todas as classes de objeto disponíveis no servidor, inclusive `inetOrgPerson` e `groupOfNames`, o que foi essencial para confirmar que esses tipos realmente existiam e estavam prontos para uso.

Antes disso, eu estava tendo dificuldade em entender a documentação escrita do OpenDJ, então essa interface me ajudou visualmente a enxergar quais atributos cada classe aceitava, quais eram obrigatórios, e como tudo se encaixava no esquema padrão do servidor.

### Para usuários:

Na documentação de schema, eu vi que o tipo `inetOrgPerson` era um dos mais completos. Ele aceitava atributos como `cn` (nome comum), `sn` (sobrenome), `uid` (login) e `telephoneNumber`. Esses são justamente os campos que estavam nos XMLs do desafio, então fez total sentido usar esse tipo.

Eu também vi que o `inetOrgPerson` faz parte de uma hierarquia que inclui `organizationalPerson`, `person` e `top`. Isso significa que ele já aceita atributos que vêm desses tipos, o que é útil porque não preciso adicionar nenhum schema customizado para funcionar.

### Para grupos:

Olhando os tipos disponíveis para agrupar usuários, vi que o `groupOfNames` exigia apenas um atributo chamado `member`, que é uma lista de DNs. Isso me chamou atenção porque parecia simples de implementar. Na prática, bastava adicionar os DNs dos usuários ao grupo como `member`, e o OpenDJ já reconheceria.

A única exigência do `groupOfNames` era que ao menos um membro fosse informado — e isso me forçou a adicionar um “membro fake” temporariamente na criação, já que os usuários ainda não existiam quando os grupos eram criados.

---

## Estrutura das OUs (organizational units)

Para separar usuários e grupos, como o enunciado pedia, decidi criar duas OUs distintas:
- `ou=users,dc=openconsult,dc=local,dc=com`
- `ou=groups,dc=openconsult,dc=local,dc=com`

Essas OUs foram criadas manualmente pelo painel gráfico do OpenDJ antes da aplicação rodar, e toda a lógica do código foi feita esperando que esses caminhos já existam.

---

## Criando grupos – método `CreateGroup`

Antes de montar esse trecho do código, eu tive que entender como o .NET envia dados para o LDAP. Foi aí que encontrei a classe `AddRequest`, que representa uma operação LDAP de adição de entrada. Na documentação da Microsoft, eles explicam que você pode montar uma nova entrada no diretório passando um DN e uma lista de atributos com `DirectoryAttribute`. Então basicamente, cada `DirectoryAttribute` representa um campo da entrada LDAP.

📚 [AddRequest – Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.addrequest)  
📚 [DirectoryAttribute – Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.directoryattribute)

Sabendo disso, comecei montando a entrada com os dados do grupo dessa forma:

Depois de entender o `groupOfNames`, fui para o código. Criei um método chamado `CreateGroup` no `LdapService.cs`. A primeira coisa foi montar o DN do grupo:

```csharp
string groupDn = $"cn={group.Identifier},ou=groups,{_baseDn}";
```

Depois preparei os atributos exigidos:
```csharp
var request = new AddRequest(groupDn,
    new DirectoryAttribute("objectClass", "top", "groupOfNames"),
    new DirectoryAttribute("cn", group.Identifier),
    new DirectoryAttribute("description", group.Description ?? ""),
    new DirectoryAttribute("member", $"cn=fake,{_baseDn}") // placeholder temporário
);
```

Esse `member` fake foi só para evitar erro no momento da criação, porque o LDAP exige que haja pelo menos um.

---

## Criando usuários – método `CreateUser`

Seguindo o mesmo raciocínio utilizado na criação dos grupos, apliquei a mesma estrutura para a criação dos usuários, só que agora utilizando a classe de objeto `inetOrgPerson`, com os atributos específicos que o LDAP espera receber para representar uma pessoa.

Sabendo disso, comecei montando a entrada com os dados do usuário dessa forma:

Depois fui criar os usuários com o tipo `inetOrgPerson`. A estrutura foi parecida. Primeiro o DN do usuário:
```csharp
string userDn = $"uid={user.Login},ou=users,{_baseDn}";
```

E os atributos:
```csharp
var request = new AddRequest(userDn,
    new DirectoryAttribute("objectClass", "top", "person", "organizationalPerson", "inetOrgPerson"),
    new DirectoryAttribute("cn", user.FullName),
    new DirectoryAttribute("sn", user.FullName.Split(' ').Last()),
    new DirectoryAttribute("uid", user.Login),
    new DirectoryAttribute("telephoneNumber", user.Phone)
);
```

Esses atributos todos estavam descritos na documentação e batiam com o que os XMLs me forneciam, então foi uma combinação que funcionou bem.

---

## Conclusão

Essa parte do projeto foi um aprendizado bem direto sobre como estruturar objetos dentro do LDAP usando tipos que o OpenDJ já reconhece, mas ainda deixou algumas dúvidas. Embora eu tenha conseguido fazer funcionar, senti que algumas partes da lógica podem estar sendo aplicadas de forma mecânica e talvez eu tenha dificuldade para explicar todos os detalhes, principalmente se me perguntarem sobre os motivos técnicos mais profundos por trás da escolha de cada classe e atributo. Mesmo assim, consegui montar métodos simples e reutilizáveis que, na prática, funcionaram bem para criar tanto usuários quanto grupos.

