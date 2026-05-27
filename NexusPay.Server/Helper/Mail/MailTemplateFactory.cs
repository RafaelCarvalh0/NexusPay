namespace NexusPay.Server.Helper.Mail
{
    public static class MailTemplateFactory
    {
        public static string BuildForgotPasswordTemplate(string resetLink)
        {
            return $@"
            <!DOCTYPE html>
            <html lang='pt-BR'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Redefinição de Senha - NexusPay</title>
            </head>
            <body style='margin: 0; padding: 0; background-color: #f4f6f8; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
                <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px; background-color: #ffffff; margin: 40px auto; border-radius: 8px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05); overflow: hidden;'>
                    <tr>
                        <td style='padding: 32px 40px; background-color: #1a1f2c; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 24px; font-weight: 700; letter-spacing: -0.5px;'>NexusPay</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='margin-top: 0; color: #1a1f2c; font-size: 20px; font-weight: 600;'>Recuperação de Senha</h2>
                            <p style='color: #4a5568; font-size: 16px; line-height: 1.6; margin-bottom: 32px;'>
                                Olá,<br><br>
                                Recebemos uma solicitação para redefinir a senha da sua conta no <strong>NexusPay</strong>. Se você não fez essa solicitação, pode ignorar este e-mail com segurança.
                            </p>
                    
                            <table align='center' border='0' cellpadding='0' cellspacing='0' style='margin: 0 auto 32px auto;'>
                                <tr>
                                    <td align='center' style='border-radius: 6px; background-color: #0066cc;'>
                                        <a href='{resetLink}' target='_blank' style='display: inline-block; padding: 14px 28px; font-size: 16px; font-weight: 600; color: #ffffff; text-decoration: none; border-radius: 6px;'>
                                            Redefinir Minha Senha
                                       </a>
                                    </td>
                                </tr>
                            </table>

                            <p style='color: #718096; font-size: 14px; line-height: 1.5; margin-bottom: 0;'>
                                Este link expirará em breve por motivos de segurança.<br>
                                Se o botão acima não funcionar, copie e cole o endereço abaixo no seu navegador:<br>
                                <a href='{resetLink}' style='color: #0066cc; text-decoration: underline; word-break: break-all;'>{resetLink}</a>
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 24px 40px; background-color: #f8fafc; border-top: 1px solid #edf2f7; text-align: center;'>
                            <p style='margin: 0; color: #a0aec0; font-size: 12px;'>
                                &copy; 2026 NexusPay. Todos os direitos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";
        }
    }
}
