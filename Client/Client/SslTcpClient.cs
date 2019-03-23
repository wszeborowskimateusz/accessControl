using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class SslTcpClient
    {
        //IP addresss of our server
        private string machineName = "localhost";
        private string serverName = "BSK";
        private SslStream sslStream;
        private TcpClient client;

        
        private bool EstablishConnection()
        {
            client = new TcpClient(machineName, 8080);

            sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );
            try
            {
                sslStream.AuthenticateAsClient(serverName);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return false;
            }
            return true;
        }

        ~SslTcpClient()
        {
            client.Close();
        }

        public  bool ValidateServerCertificate( object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            return false;
        }

        public string SendMessage(string message)
        {
            if (!EstablishConnection()) return "";

            message = message + "<EOF>";
            byte[] messsage = Encoding.UTF8.GetBytes(message);

            sslStream.Write(messsage);
            sslStream.Flush();
 
            string serverMessage = ReadMessage(sslStream);

            client.Close();

            return serverMessage;
        }
        private string ReadMessage(SslStream sslStream)
        {
            // The end of the message is signaled using the
            // "<EOF>" marker.
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF.
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            //return a message with <EOF> mark removed
            int toBeRemoved = messageData.ToString().Length - 5;
            if (toBeRemoved > 0)
                return messageData.ToString().Remove(toBeRemoved);
            else return "";
        }
    }
}
