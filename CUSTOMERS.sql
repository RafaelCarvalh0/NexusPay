CREATE TABLE CUSTOMERS (
    Id          UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId    UNIQUEIDENTIFIER  NOT NULL,
    Name        NVARCHAR(200)     NOT NULL,
    Document    NVARCHAR(18)      NOT NULL,
    Email       NVARCHAR(255)     NULL,
    Phone       NVARCHAR(20)      NULL,
    CreatedAt   DATETIME2         NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT PK_Customers     PRIMARY KEY (Id),
    CONSTRAINT FK_Customers_Tenant FOREIGN KEY (TenantId) REFERENCES TENANTS(Id),
    CONSTRAINT UQ_Customers_Document_Tenant UNIQUE (TenantId, Document)
);