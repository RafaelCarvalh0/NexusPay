USE [NexusPay]
GO

-- =============================================
-- AUTHOR:      RAFAEL HENRIQUE DE CARVALHO
-- CREATE DATE: 05/17/2026
-- DESCRIPTION: INSERT DEFAULT USER FOR DEVELOPMENT PURPOSES ONLY
--              DO NOT EXECUTE IN PRODUCTION
-- =============================================

INSERT INTO USERS (ID, NAME, EMAIL, PASSWORDHASH, CREATEDAT, ISACTIVE)
VALUES (
    NEWID(),
    'Default User',
    'default.user@gmail.com',
    '$2a$11$4kS0a0vbFoTIFTbyBp9n/ul350wRQJzF4YV81lyrDjVKkPMWbnesa', -- senha: 123456
    GETUTCDATE(),
    1
);
GO