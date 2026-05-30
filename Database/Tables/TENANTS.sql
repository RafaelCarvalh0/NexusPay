CREATE TABLE TENANTS (
    Id            UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name          NVARCHAR(200)     NOT NULL,
    Document      NVARCHAR(18)      NOT NULL,
    Email         NVARCHAR(255)     NOT NULL,
	Phone         NVARCHAR(20)      NULL,
    ApiKey        NVARCHAR(64)      NOT NULL,
    IsActive      BIT               NOT NULL DEFAULT 1,
    CreatedAt     DATETIME2         NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT PK_Tenants PRIMARY KEY (Id),
    CONSTRAINT UQ_Tenants_Document UNIQUE (Document),
    CONSTRAINT UQ_Tenants_ApiKey   UNIQUE (ApiKey)
);