# Leitura e Validação de XML com XPath e Expressões Regulares

## Introdução

Nesta parte do projeto, eu precisei pensar em como interpretar os arquivos XML de forma flexível e segura, e o caminho que escolhi foi usar o XPath com a classe `XPathNavigator`, que é bem consolidada no .NET. O desafio pedia que fosse usado XPath ou algo equivalente, e como eu ainda não conhecia bem essa abordagem, fui direto na documentação oficial da Microsoft para entender como funcionava e acabei achando ela muito adequada para o que eu precisava fazer.

---

## Por que eu usei XPath

A leitura de XML pode ser feita de várias formas em C#, mas o XPath oferece uma forma muito direta de navegar até o elemento desejado sem precisar fazer parsing manual da estrutura. No meu caso, isso facilitou muito, porque os XMLs do desafio têm uma estrutura bem clara, com tags como `add-attr`, `modify-attr`, `value`, etc.

Na documentação da Microsoft eles explicam bem como funciona o `XPathNavigator`, que é a principal classe que eu usei aqui:  
🔗 [Microsoft Docs – XPathNavigator](https://learn.microsoft.com/en-us/dotnet/api/system.xml.xpath.xpathnavigator)

Outra coisa legal do XPath é que você consegue fazer buscas condicionais, como pegar só os `add-attr` com o atributo `attr-name="Login"`, por exemplo:
```csharp
var node = navigator.SelectSingleNode("add-attr[@attr-name='Login']/value");
```

---

## Como montei o parser

Dentro da classe `XmlParser.cs`, eu fiz o código percorrer todos os arquivos da pasta `xml/`, usando `Directory.GetFiles(...)`. Para cada arquivo, ele carrega com `XPathDocument`, e depois usa `XPathNavigator` para andar nos nós.

A estrutura básica ficou assim:

```csharp
XPathDocument document = new XPathDocument(path);
XPathNavigator navigator = document.CreateNavigator();
```

Depois disso, usei `SelectSingleNode` ou `Select` dependendo se era um valor único ou uma lista de valores (como quando tem vários grupos).

---

## Validação com Regex

Uma coisa importante que o desafio pediu foi que o nome e login tivessem apenas letras e que o telefone tivesse só números. Eu sabia que poderia fazer isso com validação manual, mas preferi usar `Regex.Replace()` porque é mais direto e evita várias verificações com `if`.

A lógica que segui foi:
- Nome: se tiver número, remove.
- Login: remove tudo que não for letra.
- Telefone: remove tudo que não for número.

Exemplo:
```csharp
string login = Regex.Replace(loginRaw, "[^A-Za-z]", "");
string phone = Regex.Replace(phoneRaw, "[^\d]", "");
```

E claro, depois de limpar, eu ainda verifico se os campos essenciais não ficaram vazios:
```csharp
if (string.IsNullOrWhiteSpace(login)) return null;
```

📚 Referência da Microsoft sobre Regex:
- [System.Text.RegularExpressions](https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.-ctor?view=net-9.0)

---

## Por que isso importa

Essa parte da validação foi essencial para evitar que eu passasse dados errados para o LDAP. Como o LDAP exige certos formatos e não aceita campos em branco, esse filtro inicial ajuda a garantir que só entre no sistema quem está com os dados mínimos corretos.

Além disso, manter tudo encapsulado dentro do `XmlParser` me ajudou a manter o código limpo e fácil de manter. Toda a responsabilidade de transformar XML em objetos do projeto está isolada lá, então se algum dia eu precisar mudar o formato de entrada, é só alterar ali.

---

## Conclusão

O uso do XPath, combinado com Regex, me deu uma base sólida e segura para ler os arquivos XML do desafio. Isso me permitiu aplicar as regras de validação logo no início do fluxo, antes mesmo de tentar comunicar com o LDAP, o que evita muitos problemas e ajuda a manter a consistência do sistema como um todo.

