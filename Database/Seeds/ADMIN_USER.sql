USE [NexusPay]
GO

-- =============================================
-- AUTHOR:      RAFAEL HENRIQUE DE CARVALHO
-- CREATE DATE: 05/17/2026
-- DESCRIPTION: INSERT DEFAULT USER FOR DEVELOPMENT PURPOSES ONLY
--              DO NOT EXECUTE IN PRODUCTION
-- =============================================

INSERT INTO USERS (ID, NAME, EMAIL, PASSWORDHASH, ROLEID, CREATEDAT, ISACTIVE)
VALUES (
    NEWID(),
    'Admin User',
    'admin.user@gmail.com',
    '$2a$11$4kS0a0vbFoTIFTbyBp9n/ul350wRQJzF4YV81lyrDjVKkPMWbnesa', -- password: 123456
    1, -- verify admin role ID in ROLES table
    GETUTCDATE(),
    1
);
GO