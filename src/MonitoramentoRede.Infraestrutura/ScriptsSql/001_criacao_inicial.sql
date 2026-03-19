CREATE TABLE PerfilAcesso (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(50) NOT NULL UNIQUE,
    Descricao NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE UsuarioSistema (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(150) NOT NULL,
    Login NVARCHAR(60) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL,
    SenhaHash NVARCHAR(256) NOT NULL,
    PerfilAcessoId BIGINT NOT NULL,
    Ativo BIT NOT NULL DEFAULT 1,
    UltimoLoginUtc DATETIME2 NULL,
    CONSTRAINT FK_UsuarioSistema_PerfilAcesso FOREIGN KEY (PerfilAcessoId) REFERENCES PerfilAcesso(Id)
);
GO

CREATE TABLE DispositivoRede (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Ip NVARCHAR(45) NOT NULL,
    Mac NVARCHAR(50) NOT NULL,
    Hostname NVARCHAR(200) NOT NULL,
    Status INT NOT NULL,
    SistemaOperacional NVARCHAR(120) NULL,
    Observacoes NVARCHAR(500) NULL,
    PrimeiroVistoUtc DATETIME2 NOT NULL,
    UltimaDeteccaoUtc DATETIME2 NOT NULL
);
GO

CREATE UNIQUE INDEX IX_DispositivoRede_Ip_Mac ON DispositivoRede(Ip, Mac);
CREATE INDEX IX_DispositivoRede_Hostname ON DispositivoRede(Hostname);
CREATE INDEX IX_DispositivoRede_Status ON DispositivoRede(Status);
GO

CREATE TABLE EventoDns (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    DispositivoRedeId BIGINT NOT NULL,
    Dominio NVARCHAR(255) NOT NULL,
    TipoRegistro INT NOT NULL,
    StatusConsulta INT NOT NULL,
    Resposta NVARCHAR(255) NULL,
    TempoRespostaMs INT NOT NULL,
    EnderecoOrigem NVARCHAR(45) NOT NULL,
    DataEventoUtc DATETIME2 NOT NULL,
    CONSTRAINT FK_EventoDns_DispositivoRede FOREIGN KEY (DispositivoRedeId) REFERENCES DispositivoRede(Id)
);
GO

CREATE INDEX IX_EventoDns_DataEventoUtc ON EventoDns(DataEventoUtc DESC);
CREATE INDEX IX_EventoDns_Dominio ON EventoDns(Dominio);
CREATE INDEX IX_EventoDns_DispositivoRedeId ON EventoDns(DispositivoRedeId);
GO

CREATE TABLE FluxoRede (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    DispositivoRedeId BIGINT NOT NULL,
    IpDestino NVARCHAR(45) NOT NULL,
    PortaDestino INT NOT NULL,
    Protocolo NVARCHAR(20) NOT NULL,
    BytesEnviados BIGINT NOT NULL,
    BytesRecebidos BIGINT NOT NULL,
    DominioCorrelacionado NVARCHAR(255) NULL,
    DataInicioUtc DATETIME2 NOT NULL,
    DataFimUtc DATETIME2 NOT NULL,
    CONSTRAINT FK_FluxoRede_DispositivoRede FOREIGN KEY (DispositivoRedeId) REFERENCES DispositivoRede(Id)
);
GO

CREATE INDEX IX_FluxoRede_DataInicioUtc ON FluxoRede(DataInicioUtc DESC);
CREATE INDEX IX_FluxoRede_DispositivoRedeId ON FluxoRede(DispositivoRedeId);
CREATE INDEX IX_FluxoRede_DominioCorrelacionado ON FluxoRede(DominioCorrelacionado);
GO

CREATE TABLE AlertaRede (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    DispositivoRedeId BIGINT NULL,
    Tipo INT NOT NULL,
    Severidade INT NOT NULL,
    Status INT NOT NULL,
    Titulo NVARCHAR(150) NOT NULL,
    Descricao NVARCHAR(1000) NOT NULL,
    ObservacaoOperador NVARCHAR(1000) NULL,
    DataCriacaoUtc DATETIME2 NOT NULL,
    DataResolucaoUtc DATETIME2 NULL,
    CONSTRAINT FK_AlertaRede_DispositivoRede FOREIGN KEY (DispositivoRedeId) REFERENCES DispositivoRede(Id)
);
GO

CREATE INDEX IX_AlertaRede_Status_DataCriacaoUtc ON AlertaRede(Status, DataCriacaoUtc DESC);
CREATE INDEX IX_AlertaRede_DispositivoRedeId ON AlertaRede(DispositivoRedeId);
GO

CREATE TABLE LogAuditoria (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UsuarioSistemaId BIGINT NULL,
    UsuarioNome NVARCHAR(150) NOT NULL,
    Acao NVARCHAR(100) NOT NULL,
    Entidade NVARCHAR(100) NOT NULL,
    Dados NVARCHAR(MAX) NULL,
    EnderecoIp NVARCHAR(45) NULL,
    Sucesso BIT NOT NULL,
    DataEventoUtc DATETIME2 NOT NULL,
    CONSTRAINT FK_LogAuditoria_UsuarioSistema FOREIGN KEY (UsuarioSistemaId) REFERENCES UsuarioSistema(Id)
);
GO

CREATE INDEX IX_LogAuditoria_DataEventoUtc ON LogAuditoria(DataEventoUtc DESC);
CREATE INDEX IX_LogAuditoria_UsuarioNome ON LogAuditoria(UsuarioNome);
GO

CREATE TABLE ConfiguracaoSistema (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ChaveApiInterna NVARCHAR(200) NOT NULL,
    RetencaoDadosDias INT NOT NULL,
    IntervaloAtualizacaoDashboardSegundos INT NOT NULL,
    LimitePicoTrafegoBytes BIGINT NOT NULL,
    JanelaExcessoDnsMinutos INT NOT NULL,
    LimiteConsultasDnsJanela INT NOT NULL,
    PortasIncomuns NVARCHAR(200) NOT NULL,
    DataAtualizacaoUtc DATETIME2 NOT NULL
);
GO

INSERT INTO PerfilAcesso (Nome, Descricao) VALUES
('Administrador', 'Acesso integral ao sistema'),
('Operador', 'Acesso operacional e resolução de alertas'),
('SomenteLeitura', 'Acesso apenas para consulta');
GO

INSERT INTO ConfiguracaoSistema (
    ChaveApiInterna,
    RetencaoDadosDias,
    IntervaloAtualizacaoDashboardSegundos,
    LimitePicoTrafegoBytes,
    JanelaExcessoDnsMinutos,
    LimiteConsultasDnsJanela,
    PortasIncomuns,
    DataAtualizacaoUtc
) VALUES (
    'trocar-esta-chave',
    30,
    15,
    5000000,
    5,
    100,
    '23,3389,5900',
    SYSUTCDATETIME()
);
GO

INSERT INTO UsuarioSistema (Nome, Login, Email, SenhaHash, PerfilAcessoId, Ativo, UltimoLoginUtc)
SELECT
    'Administrador Inicial',
    'admin',
    'admin@monitoramentorede.local',
    'e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7',
    Id,
    1,
    NULL
FROM PerfilAcesso
WHERE Nome = 'Administrador';
GO
