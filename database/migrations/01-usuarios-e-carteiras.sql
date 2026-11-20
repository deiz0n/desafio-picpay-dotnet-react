-- 01: usuários (comum com CPF, lojista com CNPJ) e suas carteiras.
-- Convenções do ADR 0004: snake_case, português, tabelas no plural, data_criacao.
-- Dinheiro em NUMERIC(15,2) (ADR 0017). Só dois tipos de usuário (ADR 0016).

CREATE TABLE usuarios (
    id           BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nome         VARCHAR(100) NOT NULL,
    -- Gravado sem espaços e em minúsculas pelo Domain, então o UNIQUE simples basta.
    email        VARCHAR(254) NOT NULL,
    -- CPF ou CNPJ, só dígitos.
    documento    VARCHAR(14)  NOT NULL,
    tipo         VARCHAR(10)  NOT NULL,
    hash_senha   TEXT         NOT NULL,
    data_criacao TIMESTAMPTZ  NOT NULL DEFAULT now(),

    CONSTRAINT usuarios_email_key     UNIQUE (email),
    CONSTRAINT usuarios_documento_key UNIQUE (documento),
    CONSTRAINT usuarios_tipo_check    CHECK (tipo IN ('comum', 'lojista')),
    CONSTRAINT usuarios_documento_check CHECK (
        (tipo = 'comum'   AND documento ~ '^[0-9]{11}$') OR
        (tipo = 'lojista' AND documento ~ '^[0-9]{14}$')
    )
);

CREATE TABLE carteiras (
    id           BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    usuario_id   BIGINT        NOT NULL REFERENCES usuarios (id),
    saldo        NUMERIC(15,2) NOT NULL,
    data_criacao TIMESTAMPTZ   NOT NULL DEFAULT now(),

    CONSTRAINT carteiras_usuario_id_key UNIQUE (usuario_id),
    CONSTRAINT carteiras_saldo_check    CHECK (saldo >= 0)
);
