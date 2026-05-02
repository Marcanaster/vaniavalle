namespace DanceAcademy.Infrastructure.Services;

public static class EmailTemplates
{
    private const string BrandColor = "#7E65AA";
    private const string LighterBrandColor = "#F3F0F8";

    public static string GetWelcomeTemplate(string name, string login, string password, string role)
    {
        string roleText = role == "Teacher" ? "professor(a)" : "responsável";
        string portalName = role == "Teacher" ? "Portal do Professor" : "Portal do Aluno";
        string title = role == "Teacher" ? "Novo Cadastro de Professor" : "Bem-vindo à Academia Vania Valle";
        
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <style>
                body {{ font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; background-color: #f4f4f7; color: #51545e; margin: 0; padding: 0; }}
                .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05); }}
                .header {{ background-color: {BrandColor}; padding: 40px 20px; text-align: center; color: #ffffff; }}
                .header h1 {{ margin: 0; font-size: 26px; font-weight: 700; letter-spacing: -0.5px; }}
                .content {{ padding: 40px; line-height: 1.6; }}
                .greeting {{ font-size: 20px; color: #333333; font-weight: 700; margin-bottom: 20px; }}
                .credentials-box {{ background-color: {LighterBrandColor}; border: 1px solid #e1d8f0; border-radius: 8px; padding: 25px; margin: 30px 0; text-align: left; }}
                .credentials-title {{ font-size: 14px; color: {BrandColor}; font-weight: 700; text-transform: uppercase; margin-bottom: 15px; letter-spacing: 1px; }}
                .credential-item {{ margin: 8px 0; font-size: 16px; color: #444; }}
                .credential-label {{ font-weight: 600; color: #666; width: 80px; display: inline-block; }}
                .password-tag {{ background-color: #ffffff; padding: 4px 10px; border-radius: 4px; font-family: 'Courier New', Courier, monospace; font-weight: 700; border: 1px solid #ddd; color: #333; }}
                .button-container {{ text-align: center; margin: 40px 0; }}
                .button {{ background-color: {BrandColor}; color: #ffffff !important; padding: 16px 35px; text-decoration: none; border-radius: 8px; font-weight: 700; font-size: 16px; display: inline-block; transition: background-color 0.3s ease; }}
                .footer {{ background-color: #f9f9fb; padding: 30px; text-align: center; color: #9da3ae; font-size: 13px; border-top: 1px solid #edf2f7; }}
                .footer p {{ margin: 5px 0; }}
                .highlight {{ color: {BrandColor}; font-weight: 600; }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <div style=""font-family: 'Georgia', serif; font-style: italic; font-size: 32px; margin-bottom: 0;"">Vania Valle</div>
                    <div style=""font-family: 'Helvetica', sans-serif; font-size: 14px; text-transform: uppercase; letter-spacing: 4px; margin-top: -5px; opacity: 0.9;"">academia</div>
                </div>
                <div class=""content"">
                    <div class=""greeting"">Olá, {name}!</div>
                    <p>Seja muito bem-vindo(a)! Seu acesso ao <span class=""highlight"">{portalName}</span> foi configurado com sucesso. Agora você pode acompanhar todas as novidades, horários e sua evolução conosco.</p>
                    
                    <div class=""credentials-box"">
                        <div class=""credentials-title"">Suas Credenciais de Acesso</div>
                        <div class=""credential-item"">
                            <span class=""credential-label"">Login:</span> {login}
                        </div>
                        <div class=""credential-item"">
                            <span class=""credential-label"">Senha:</span> <span class=""password-tag"">{password}</span>
                        </div>
                    </div>
                    
                    <p>Para sua segurança, recomendamos que altere sua senha no seu primeiro acesso.</p>
                    
                    <div class=""button-container"">
                        <a href=""https://academiavaniavalle.com.br/login"" class=""button"">ACESSAR MEU PORTAL</a>
                    </div>
                    
                    <p>Estamos muito felizes em ter você em nossa família!</p>
                </div>
                <div class=""footer"">
                    <p>&copy; {DateTime.Now.Year} Academia Vania Valle. Todos os direitos reservados.</p>
                    <p>Este é um e-mail automático, por favor não responda.</p>
                </div>
            </div>
        </body>
        </html>
        ";
    }
}
