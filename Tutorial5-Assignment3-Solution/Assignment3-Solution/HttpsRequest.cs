using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SSE
{
    public class HttpsRequest
    {
        private HttpsRequest() { }

        // The following method is invoked by the RemoteCertificateValidationDelegate. 
        private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers. 
            return false;
        }

        public static async Task<string> Get(Url url)
        {
            // resolve DNS
            var addrs = await Dns.GetHostAddressesAsync(url.Host);
            var ip = addrs[0].ToString();

            // construct HTTP request
            var request = "GET " + url.Path + "?" + url.Query + " HTTP/1.1\n" +
                          "Host: " + url.Host + "\n" +
                          "\n";

            var client = new TcpClient();
            await client.ConnectAsync(ip, 443);

            // Create an SSL stream that will close the client's stream.
            var s = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);

            // The server name must match the name on the server certificate. 
            await s.AuthenticateAsClientAsync(url.Host);

            using (var r = new StreamReader(s, Encoding.ASCII))
            using (var w = new StreamWriter(s, Encoding.ASCII))
            {
                await w.WriteAsync(request);
                w.Flush();

                var buffer = new char[4096];
                var byteCount = await r.ReadAsync(buffer, 0, buffer.Length);
                return new string(buffer);
            }
        }
    }
}