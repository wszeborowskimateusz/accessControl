using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class SslTcpServer
    {
        private TcpListener listener;
        private X509Certificate serverCertificate = null;

        // The certificate parameter specifies the name of the file 
        // containing the machine certificate. File cert.pem in Certificate catalog
        public void RunServer(string certificate)
        {
            serverCertificate = X509Certificate.CreateFromCertFile(certificate);

            listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for a client to connect...");

                TcpClient client = listener.AcceptTcpClient();
                ProcessClient(client);
            }
        }
        private void ProcessClient(TcpClient client)
        {
            SslStream sslStream = new SslStream(
                client.GetStream(), false);
            
            try
            {
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, checkCertificateRevocation: true);

                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
                   
                string messageData = ReadMessage(sslStream);
                Console.WriteLine("Received: {0}", messageData);

                //TODO Process the message

                //byte[] message = Encoding.UTF8.GetBytes("Hello from the server.<EOF>");
                //Console.WriteLine("Sending hello message.");
                //sslStream.Write(message);

                //TODO Send response 

            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
                return;
            }
            finally
            {
                // The client stream will be closed with the sslStream
                // because we specified this behavior when creating
                // the sslStream.
                sslStream.Close();
                client.Close();
            }
        }
        private string ReadMessage(SslStream sslStream)
        {
            // The client signals the end of the message using the
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
