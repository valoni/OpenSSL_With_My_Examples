using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SslClient {
    class App {

        static void Main(string[] args) 
        {
            string ClientCertificateFile = ConfigurationManager.AppSettings["certfile"].ToString();
            string Password = ConfigurationManager.AppSettings["password"].ToString();
            
            string ClientCertificatePassword = null;
            if (Password.Length > 0) ClientCertificatePassword = Password;

            int ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["listenerport"].ToString());

            string ServerHostName = ConfigurationManager.AppSettings["certServerHost"].ToString();
            string ServerCertificateName = ConfigurationManager.AppSettings["certServerName"].ToString();

            try {
                //read from the file
                var clientCertificate = new X509Certificate2(ClientCertificateFile, ClientCertificatePassword);
                var clientCertificateCollection = new X509CertificateCollection(new X509Certificate[] { clientCertificate });
               

                using (var client = new TcpClient(ServerHostName, ServerPort))
                using (var sslStream = new SslStream(client.GetStream(), false, App_CertificateValidation)) {
                    Console.WriteLine("Client connected.");

                    sslStream.AuthenticateAsClient(ServerCertificateName, clientCertificateCollection, SslProtocols.Tls12, false);
                 
                    Console.WriteLine("SSL authentication completed.");
                    Console.WriteLine("SSL using local certificate {0}.", sslStream.LocalCertificate.Subject);
                    Console.WriteLine("SSL using remote certificate {0}.", sslStream.RemoteCertificate.Subject);

                    var outputMessage = "Hello from the client " + Process.GetCurrentProcess().Id.ToString() + ".";
                    var outputBuffer = Encoding.UTF8.GetBytes(outputMessage);
                    sslStream.Write(outputBuffer);
                    Console.WriteLine("Sent: {0}", outputMessage);

                    var inputBuffer = new byte[4096];
                    var inputBytes = 0;
                    while (inputBytes == 0) {
                        inputBytes = sslStream.Read(inputBuffer, 0, inputBuffer.Length);
                    }
                    var inputMessage = Encoding.UTF8.GetString(inputBuffer, 0, inputBytes);
                    Console.WriteLine("Received: {0}", inputMessage);
                }
            } catch (Exception ex) {
                Console.WriteLine("*** {0}\n*** {1}!", ex.GetType().Name, ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /* application validation */
        private static bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            if (sslPolicyErrors == SslPolicyErrors.None) { return true; }
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors) { return true; } //we don't have a proper certificate tree
            Console.WriteLine("*** SSL Error: " + sslPolicyErrors.ToString());
            return false;
        }
    }
}
