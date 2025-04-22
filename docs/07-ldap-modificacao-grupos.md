# Modificação de Grupos no LDAP – Adição e Remoção de Membros

## Introdução

Depois de implementar a criação de usuários e grupos, chegou a parte do desafio em que eu precisava modificar os grupos — mais especificamente, adicionar ou remover usuários de grupos existentes. A ideia era ler essas informações dos XMLs e refletir essas mudanças no LDAP usando comandos adequados.

Confesso que essa parte me deixou um pouco inseguro no começo, porque eu nunca tinha lidado diretamente com modificações em diretórios LDAP usando C#. Então, mais uma vez, precisei recorrer à documentação da Microsoft para entender como a operação de modificação funcionava.

---

## Como cheguei ao ModifyRequest

Fazendo buscas, encontrei a classe `ModifyRequest`, que é usada para enviar mudanças específicas a uma entrada LDAP. A documentação mostra que ela permite operações como adicionar, substituir ou remover atributos.

📚 [ModifyRequest – Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.modifyrequest)
📚 [DirectoryAttributeModification – Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.directoryservices.protocols.directoryattributemodification)

No meu caso, o atributo a ser alterado era sempre o `member` de um grupo, e o valor era o DN completo do usuário.

---

## Lógica aplicada no código

Para implementar isso, criei o método `ModifyUserGroups` na classe `LdapService.cs`. A ideia foi iterar sobre os grupos a adicionar e remover, e para cada um deles, montar uma modificação com a operação correspondente:

### Adição ao grupo

```csharp
var addModification = new DirectoryAttributeModification
{
    Name = "member",
    Operation = DirectoryAttributeOperation.Add
};
addModification.Add(userDn);

var modifyRequest = new ModifyRequest(groupDn, addModification);
connection.SendRequest(modifyRequest);
```

### Remoção do grupo

```csharp
var removeModification = new DirectoryAttributeModification
{
    Name = "member",
    Operation = DirectoryAttributeOperation.Delete
};
removeModification.Add(userDn);

var modifyRequest = new ModifyRequest(groupDn, removeModification);
connection.SendRequest(modifyRequest);
```

Essa lógica funcionou bem nos testes, mas uma coisa que percebi é que, se o usuário não estiver no grupo no momento da remoção, o LDAP retorna erro dizendo que aquele valor não existe. Eu até consegui capturar isso e mostrar uma mensagem mais amigável, mas confesso que ainda fico um pouco perdido sobre como lidar com isso da melhor forma.

---

## Validação de existência

Outra coisa importante é que antes de adicionar ou remover, eu precisava montar corretamente o DN do usuário. Para isso, usei a estrutura que já tinha sido definida anteriormente:

```csharp
string userDn = $"uid={mod.Login},ou=users,{_baseDn}";
```

E para o grupo:
```csharp
string groupDn = $"cn={groupName},ou=groups,{_baseDn}";
```

Se esse DN não existir no servidor, a operação falha — e essa é outra parte que me gerou dúvida, porque talvez fosse melhor verificar antes se o usuário realmente existe. Mas por enquanto, confiei que os dados vindos do XML estavam corretos.

---

## Conclusão

No geral, essa parte de modificação funcionou como esperado, mas é uma das que mais me deixou com dúvidas durante o desenvolvimento. A documentação da Microsoft ajudou bastante a entender a estrutura do `ModifyRequest`, mas em relação ao comportamento do LDAP (tipo erros ao remover usuários que não estão no grupo), ainda sinto que faltou mais clareza.

Mesmo assim, consegui implementar um fluxo funcional para refletir as mudanças dos XMLs no diretório, com base nas operações básicas de adição e remoção.

