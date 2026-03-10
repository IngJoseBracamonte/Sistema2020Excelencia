using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;

namespace Conexiones.Dto
{
    public class Gmail
    {
        public string EmailFrom { get; set; }
        public string Port { get; set; }
        public bool EnableSSL { get; set; }

        public bool userCredentials { get; set; }
        public bool Host { get; set; }

        public string EmailTo { get; set; }
        public List<string> Attachment { get; set; } = new List<string>();
        public string EmailBody { get; set; }

        public string Base64UrlEncode(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(data).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        public void CorreoPaciente(DatosDePaciente datosDePaciente, string Archivo)
        {
            string[] Scopes = { GmailService.Scope.GmailSend };
            string ApplicationName = "OrdonoGmail";
            UserCredential credential;

            using (FileStream stream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"Credentials.json", FileMode.Open, FileAccess.Read))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, ".Credentials/gmail-dotnet-quickstart.json");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, Scopes, "user", CancellationToken.None, new FileDataStore(path, true)).Result;

                string message = $"To: {datosDePaciente.Correo}{datosDePaciente.TipoCorreo}\r\nSubject:Examenes de Laboratorio \r\nContent-Type: text/html;charset=utf-8\r\n\r\n<h1></h1>";
                //call your gmail service
                var service = new GmailService(new BaseClientService.Initializer() { HttpClientInitializer = credential, ApplicationName = ApplicationName });
                var msg = new Google.Apis.Gmail.v1.Data.Message();
                msg.Raw = Base64UrlEncode(message.ToString());
                service.Users.Messages.Send(msg, "me").Execute();
            }
        }
    }
}
