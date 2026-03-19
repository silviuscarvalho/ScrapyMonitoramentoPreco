# MonitoramentoRede

Sistema interno de monitoramento de rede orientado a DNS + fluxo, construído com .NET 8, ASP.NET Core Razor Pages, Dapper, SQL Server, SignalR e Worker Services.

## Stack

- .NET 8
- ASP.NET Core Razor Pages
- Dapper
- SQL Server
- SignalR
- Worker Service / BackgroundService
- Bootstrap 5
- ILogger
- appsettings.json

## Estrutura da solução

```text
MonitoramentoRede.sln
src/
  MonitoramentoRede.Web/
  MonitoramentoRede.Aplicacao/
  MonitoramentoRede.Dominio/
  MonitoramentoRede.Infraestrutura/
  MonitoramentoRede.Compartilhado/
  MonitoramentoRede.Coletor.Dns.Worker/
  MonitoramentoRede.Coletor.Fluxo.Worker/
  MonitoramentoRede.Coletor.Descoberta.Worker/
tests/
  MonitoramentoRede.Testes.Unitarios/
  MonitoramentoRede.Testes.Integracao/
```

## Funcionalidades implementadas

- Autenticação por cookie.
- Perfis `Administrador`, `Operador` e `SomenteLeitura`.
- Dashboard com indicadores, atividades recentes e atualização em tempo real via SignalR.
- Consulta paginada de dispositivos, DNS, fluxos, alertas e auditoria.
- Administração de usuários.
- Administração de configurações operacionais.
- Endpoints internos para ingestão protegidos por API Key.
- Workers para coleta simulada por arquivos JSON.
- Alertas básicos para domínio nunca visto, dispositivo novo, pico de tráfego, porta incomum e excesso de consultas DNS.

## Banco de dados

O script de criação e seed está em:

- `src/MonitoramentoRede.Infraestrutura/ScriptsSql/001_criacao_inicial.sql`

Ele cria:

- tabelas
- chaves primárias
- chaves estrangeiras
- índices
- perfis iniciais
- usuário administrador inicial
- configuração inicial do sistema

### Credenciais iniciais

- Login: `admin`
- Senha: `Admin@123`

### Configuração inicial da API interna

- Chave seed: `trocar-esta-chave`

## Como subir o projeto

### 1. Criar o banco

1. Crie um banco SQL Server chamado `MonitoramentoRede`.
2. Execute o script `001_criacao_inicial.sql`.

### 2. Ajustar connection string

Edite `src/MonitoramentoRede.Web/appsettings.json`:

```json
{
  "BancoDados": {
    "ConnectionString": "Server=localhost;Database=MonitoramentoRede;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

Se necessário, ajuste também a connection string em outros ambientes.

### 3. Restaurar pacotes

```powershell
dotnet restore MonitoramentoRede.sln
```

### 4. Compilar

```powershell
dotnet build MonitoramentoRede.sln
```

### 5. Executar a aplicação web

```powershell
dotnet run --project .\src\MonitoramentoRede.Web\MonitoramentoRede.Web.csproj
```

### 6. Executar os workers

Em terminais separados:

```powershell
dotnet run --project .\src\MonitoramentoRede.Coletor.Dns.Worker\MonitoramentoRede.Coletor.Dns.Worker.csproj
dotnet run --project .\src\MonitoramentoRede.Coletor.Fluxo.Worker\MonitoramentoRede.Coletor.Fluxo.Worker.csproj
dotnet run --project .\src\MonitoramentoRede.Coletor.Descoberta.Worker\MonitoramentoRede.Coletor.Descoberta.Worker.csproj
```

## Pastas de entrada dos workers

Cada worker lê arquivos JSON de uma pasta configurável e move os arquivos processados.

- DNS: `dados/dns/entrada`
- Fluxo: `dados/fluxo/entrada`
- Descoberta: `dados/descoberta/entrada`

Após o processamento:

- sucesso: pasta `historico`
- erro: pasta `rejeitados`

## Exemplos de payload

### DNS

```json
{
  "ipDispositivo": "10.0.0.10",
  "macDispositivo": "AA-BB-CC-DD-EE-FF",
  "hostname": "estacao-01",
  "dominio": "contoso.com",
  "tipoRegistro": 1,
  "statusConsulta": 1,
  "resposta": "20.10.30.40",
  "tempoRespostaMs": 17,
  "dataEventoUtc": "2026-03-16T12:00:00Z"
}
```

### Fluxo

```json
{
  "ipDispositivo": "10.0.0.10",
  "macDispositivo": "AA-BB-CC-DD-EE-FF",
  "hostname": "estacao-01",
  "ipDestino": "20.10.30.40",
  "portaDestino": 443,
  "protocolo": "TCP",
  "bytesEnviados": 4096,
  "bytesRecebidos": 8192,
  "dominioCorrelacionado": "contoso.com",
  "dataInicioUtc": "2026-03-16T12:00:00Z",
  "dataFimUtc": "2026-03-16T12:00:10Z"
}
```

### Descoberta

```json
{
  "ip": "10.0.0.11",
  "mac": "11-22-33-44-55-66",
  "hostname": "switch-lab",
  "sistemaOperacional": "Linux",
  "dataDeteccaoUtc": "2026-03-16T12:00:00Z"
}
```

## Endpoints internos

- `POST /api/interno/ingestao/dns`
- `POST /api/interno/ingestao/fluxos`
- `POST /api/interno/ingestao/dispositivos`

Header obrigatório:

```text
X-Api-Key: trocar-esta-chave
```

## Rotas principais

- `/`
- `/Dispositivos`
- `/Dispositivos/Detalhes/{id}`
- `/Dns`
- `/Fluxos`
- `/Alertas`
- `/Administracao/Usuarios`
- `/Administracao/Configuracoes`
- `/Auditoria`
- `/Conta/Login`
- `/Conta/AcessoNegado`

## Testes

Executar todos:

```powershell
dotnet test MonitoramentoRede.sln
```

Cobertura mínima atual:

- unitário de autenticação
- unitário de criação de alerta por dispositivo novo
- integração de autorização por API Key nos endpoints internos

## Observações de implementação

- Horários são persistidos em UTC.
- Conversão para horário local é feita somente na exibição.
- Todo acesso a dados usa SQL explícito com Dapper.
- Os endpoints internos registram auditoria em caso de erro.
- Os Workers usam `BackgroundService` e `CancellationToken`.

## Próximos ajustes esperados em evolução normal

- adicionar métricas e health checks
- endurecer políticas de senha
- externalizar segredo da API interna para cofre seguro
- expandir testes de integração com banco real
