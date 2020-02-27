using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSE
{
    public class HttpFileServer : TcpServer
    {
        // Take the folder DocumentRoot within the project
        private static readonly string DOCUMENT_ROOT = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\..\\..\\..\\DocumentRoot";

        /// <summary>
        /// Buffer for HTTP message.
        /// </summary>
        protected string buffer = "";

        /// <summary>
        /// Handles an incoming line of text. 
        /// </summary>
        /// <param name="line">Incoming data.</param>
        /// <returns>The answer to be sent back as a reaction to the received line or null.</returns>
        protected override string ReceiveLine(string line)
        {
            buffer += line + "\r\n";
            if (line == "")
            {
                // parse message
                HttpMessage request = new HttpMessage(buffer);

                // build answer message
                HttpMessage answer = ReceiveRequest(request);

                // send answer message
                Console.WriteLine("HTTP: Sending answer.");
                return answer.ToString();
            }
            else
                return null;
        }



        /// <summary>
        /// Handle an incoming HTTP request.
        /// </summary>
        /// <param name="request">Incoming request.</param>
        /// <returns>The answer message to be sent back.</returns>
        protected virtual HttpMessage ReceiveRequest(HttpMessage request)
        {
            // check HTTP method
            if ((request.Method != HttpMessage.GET) && (request.Method != HttpMessage.POST))
                return new HttpMessage("400", "Bad Request", null, "<html><body>The HTTP method is not supported.</body></html>");

            // parse relative URL in request
            Url requestUrl = new Url(request.Resource);

            // look for requested file
            string path = DOCUMENT_ROOT + requestUrl.Path;
            if (!File.Exists(path))
                return new HttpMessage("404", "Not Found", null, "<html><body>The requested file could not be found.</body></html>");

            string content = "";
            // create answer message                        

            HttpMessage answerMessage;
            if (request.Method.Equals(HttpMessage.GET))
            {
                // read requested file from disk
                StreamReader sr = File.OpenText(path);

                string line = "";
                while ((line = sr.ReadLine()) != null)
                    content += line + "\r\n";
                answerMessage = new HttpMessage("200", "Ok", null, content);
            }
            else if (request.Method.Equals(HttpMessage.POST))
            {
                answerMessage = new HttpMessage("201", "Created (only simulation)", null, content);
            }
            else
            {
                answerMessage = new HttpMessage("405", "Method not allowed", null, content);
            }
            return answerMessage;
        }
    }
}
