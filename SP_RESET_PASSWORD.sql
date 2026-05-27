USE [NexusPay]
GO

/****** Object:  StoredProcedure [dbo].[SP_RESET_PASSWORD]    Script Date: 5/27/2026 4:01:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- AUTHOR:      RAFAEL HENRIQUE DE CARVALHO
-- CREATE DATE: 05/27/2026
-- DESCRIPTION: SECURE USER PASSWORD RESET USING THROW
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[SP_RESET_PASSWORD]
    @EMAIL NVARCHAR(255),
	@NEW_PASSWORD_HASH NVARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;
    
	IF (@EMAIL IS NULL OR TRIM(@EMAIL) = '')
		THROW 99997, 'Email is required.', 1;

	IF (@NEW_PASSWORD_HASH IS NULL OR TRIM(@NEW_PASSWORD_HASH) = '')
	    THROW 99997, 'Password hash is required.', 1;

	BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE USERS WITH (ROWLOCK)
        SET [PASSWORDHASH] = @NEW_PASSWORD_HASH
        WHERE EMAIL = LOWER(TRIM(@EMAIL));

		IF @@ROWCOUNT = 0
		BEGIN
			;THROW 99998, 'User with the provided email was not found.', 1;
		END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH

END;
GO


