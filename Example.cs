using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DumpHttpRequests
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            // Create a listener.
            HttpListener listener = new HttpListener();

            listener.Prefixes.Add("http://localhost:8888/");

            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                // Note: The GetContext method blocks while waiting for a request.
                HttpListenerContext context = listener.GetContext();

                HttpListenerRequest request = context.Request;

                string documentContents;
                using (Stream receiveStream = request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        documentContents = readStream.ReadToEnd();
                    }
                }
                Console.WriteLine($"Recived request for {request.Url}");
                Console.WriteLine(documentContents);

                // Obtain a response object.
                HttpListenerResponse response = context.Response;
                // Construct a response.
                string responseString =
                    "<HTML><BODY>" + "" +
                    "<form method=\"post\">First name: <input type=\"text\" name=\"firstname\" /><br />Last name: <input type=\"text\" name=\"lastname\" /><input type=\"submit\" value=\"Submit\" /></form>" +
                    "<form method =\"post\" enctype=\"multipart/form-data\"><input id=\"fileUp\" name=\"fileUpload\" type=\"file\" /><input type=\"submit\" /></form>" +
                    "</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }
            listener.Stop();
        }
    }
}