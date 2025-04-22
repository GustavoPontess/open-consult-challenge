# Guia de Início Rápido – Configuração e Execução do Projeto

Este guia apresenta, de forma prática e objetiva, como configurar e executar localmente a aplicação desenvolvida para o desafio técnico da Open Consult. O projeto utiliza um ambiente integrado entre Windows e Linux com o objetivo de simular uma infraestrutura profissional segura e controlada.

## 1. Pré-requisitos

Antes de iniciar, certifique-se de que os seguintes componentes estão instalados e funcionando corretamente:

- Windows 11 com suporte ao WSL (Windows Subsystem for Linux);
- WSL 2 com Ubuntu 22.04 instalado;
- .NET SDK 6.0 ou superior;
- Java Runtime (necessário para o OpenDJ);
- Visual Studio Code (ou outro editor de sua preferência).

## 2. Clonar o Repositório

No terminal do Windows:

```bash
git clone https://github.com/GustavoPontess/open-consult-challenge.git
cd open-consult-challenge
```

## 3. Configurar o OpenDJ no WSL

O OpenDJ será executado dentro do Ubuntu 22.04 via WSL (Windows Subsystem for Linux), uma solução leve que permite utilizar um ambiente Linux no Windows. Essa abordagem garante maior controle sobre dependências e isola a execução do servidor LDAP.

### a) Instalar o WSL com Ubuntu 22.04

```bash
wsl --install -d Ubuntu-22.04
```

Após a instalação e reinicialização do sistema, abra o terminal do Windows e execute:

```bash
wsl -d Ubuntu-22.04
```

### b) Atualizar os pacotes do sistema Ubuntu

```bash
sudo apt update && sudo apt upgrade -y
```

### c) Instalar o Java Runtime Environment (JRE) e unzip

```bash
sudo apt install default-jdk unzip -y
```

Para verificar se o Java foi instalado corretamente:

```bash
java -version
```

### d) Baixar o OpenDJ

```bash
wget https://github.com/OpenIdentityPlatform/OpenDJ/releases/download/4.9.3/opendj-4.9.3.zip
```

### e) Extrair e mover o OpenDJ para a pasta correta

```bash
sudo mkdir -p /opt/opendj
sudo unzip opendj-4.9.3.zip -d /opt/opendj
```

### f) Configurar o Servidor LDAP (Interface Gráfica ou Modo CLI)

Você pode configurar o OpenDJ de duas formas: por interface gráfica (GUI) ou via terminal (modo CLI). Ambas as abordagens são válidas. Neste projeto foi utilizada a interface gráfica do OpenDJ, viabilizada pelo suporte a janelas gráficas do WSLg no Windows 11.

#### Opção 1: Interface Gráfica (utilizada neste projeto)

Para iniciar o assistente gráfico, execute:
```bash
cd /opt/opendj/opendj
./setup
```

Siga as etapas conforme ilustrado abaixo:

1. Tela de boas-vindas

![Tela de boas-vindas](/docs/img/OpenDJ_Server_QuickSetup_01.png)

2. Configuração de portas e usuário administrador

![Portas e administrador](/docs/img/OpenDJ_Server_QuickSetup_02.png)

3. Topologia – manter como servidor isolado

![Topologia](/docs/img/OpenDJ_Server_QuickSetup_03.png)

4. Base DN e tipo de backend

![Base DN](/docs/img/OpenDJ_Server_QuickSetup_04.png)

5. Opções da JVM (padrão)

![JVM](/docs/img/OpenDJ_Server_QuickSetup_05.png)

6. Revisar configurações

![Revisar](/docs/img/OpenDJ_Server_QuickSetup_06.png)

7. Finalização e início do servidor

![Finalização](/docs/img/OpenDJ_Server_QuickSetup_07.png)

8. Tela de login para administração

![Login](/docs/img/OpenDJ_Server_QuickSetup_08.png)

9. Painel de administração do servidor LDAP

![Painel](/docs/img/OpenDJ_Server_QuickSetup_09.png)

10. Criar manualmente as Unidades Organizacionais (OUs)

![Criar OU](/docs/img/OpenDJ_Server_QuickSetup_10.png)

11. Preenchimento da OU "groups"

![Preencher OU](/docs/img/OpenDJ_Server_QuickSetup_11.png)

12. Confirmação de criação da OU

![Confirmação](/docs/img/OpenDJ_Server_QuickSetup_12.png)

13. OU criada com sucesso

![OU criada](/docs/img/OpenDJ_Server_QuickSetup_13.png)

Opção 2: Configuração via terminal (CLI)

O OpenDJ também pode ser configurado inteiramente via terminal, utilizando a opção --cli. Essa abordagem é útil em ambientes sem suporte gráfico ou para automação de provisionamento. Embora este projeto tenha adotado a interface gráfica, o suporte à configuração por CLI permanece disponível e poderá ser documentado em versões futuras deste guia.

Para iniciar a configuração via terminal:
```bash
cd /opt/opendj/opendj
./setup --cli
```
### g) Verificar se o servidor está ativo e obter o IP da máquina WSL

```bash
cd /opt/opendj/opendj/bin
./status
```
Para obter o IP da sua máquina WSL (necessário para configurar corretamente a conexão LDAP no código .NET), execute o comando a seguir no terminal do Windows:

```bash
wsl hostname -I
```
O IP retornado será algo como 172.28.XXX.XXX. Esse endereço deve ser utilizado na configuração da conexão LDAP no seu código, substituindo o valor da variável string ldapServer = "192.168.111.151"; presente no arquivo Program.cs.

## 4. Compilar e Executar o Projeto

No terminal do Windows (fora do WSL):

```bash
dotnet build
dotnet run
```

A aplicação iniciará a leitura dos arquivos XML localizados na pasta `/Xml` e executará as operações de criação de usuários, grupos e suas modificações dentro do diretório LDAP.

---

Com essas etapas concluídas, o ambiente estará configurado e a aplicação pronta para uso em ambiente local. Para mais detalhes sobre a lógica de negócio, leia os arquivos seguintes desta documentação.

