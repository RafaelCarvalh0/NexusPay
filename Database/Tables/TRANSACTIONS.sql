CREATE TABLE TRANSACTIONS (
    Id              UNIQUEIDENTIFIER  NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId        UNIQUEIDENTIFIER  NOT NULL,
    CustomerId      UNIQUEIDENTIFIER  NOT NULL,
    Amount          DECIMAL(18, 2)    NOT NULL,
    Description     NVARCHAR(500)     NOT NULL,
    Type            NVARCHAR(20)      NOT NULL,						-- Pix, Boleto, Transfer
    Status          NVARCHAR(20)      NOT NULL DEFAULT 'Pending',   -- Pending, Paid, Expired, Cancelled
    PixQrCode       NVARCHAR(MAX)     NULL,							-- Base64 image of the QR Code
    PixCopyPaste    NVARCHAR(MAX)     NULL,                         -- Pix copy and paste code
    ExternalId      NVARCHAR(100)     NULL,                         -- Transaction ID from Efí
    ExpiresAt       DATETIME2         NOT NULL,                     -- When the transaction expires
    PaidAt          DATETIME2         NULL,                         -- When the transaction was paid
    CreatedAt       DATETIME2         NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT PK_Transactions          PRIMARY KEY (Id),
    CONSTRAINT FK_Transactions_Tenant   FOREIGN KEY (TenantId)   REFERENCES TENANTS(Id),
    CONSTRAINT FK_Transactions_Customer FOREIGN KEY (CustomerId) REFERENCES CUSTOMERS(Id),
    CONSTRAINT CK_Transactions_Amount   CHECK (Amount > 0),
    CONSTRAINT CK_Transactions_Status   CHECK (Status IN ('Pending', 'Paid', 'Expired', 'Cancelled')),
    CONSTRAINT CK_Transactions_Type     CHECK (Type   IN ('Pix', 'Boleto', 'Transfer'))
);