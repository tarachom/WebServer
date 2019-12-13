using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread threadListener = new Thread(new ThreadStart(GeneralWorker));
            threadListener.IsBackground = true;
            threadListener.Start();

            Console.ReadLine();
        }

        /// <summary>
        /// Функція прийому вхідних повідомлень
        /// </summary>
        static void GeneralWorker()
        {

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
            Socket soketWork = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Connect] - <OK>");

                soketWork.Bind(localEndPoint);
                soketWork.Listen(1000);

                Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Listen] - <OK>");

                while (true)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Accept] - <Accept>");

                    //Очікування підключення
                    Socket soketAccept = soketWork.Accept();
                    soketAccept.ReceiveTimeout = 1000;

                    Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Accept][" + soketAccept.RemoteEndPoint.ToString() + "] - <OK>");


                    //буфер для сокета
                    Byte[] buffer = new Byte[1024];

                    int receiveByte = 0;
                    string receiveXmlText = "";

                    //Зчитую дані
                    do
                    {
                        Console.WriteLine(soketAccept.Available);
                        receiveByte = soketAccept.Receive(buffer);
                        receiveXmlText += Encoding.GetEncoding(1251).GetString(buffer, 0, receiveByte);
                    }
                    while (soketAccept.Available > 0);

                    Console.WriteLine(receiveXmlText);

                    string otvet = "HTTP/1.1 200 OK\n";
                    otvet += "Content-Type: text/html; charset=UTF-8\n\n";

                    otvet +=
                    "<form method=\"POST\" action=\"foo.php\">" +
                    "    First Name: <input name=\"first_name\" type=\"text\"> <br>" +
                    "    Last Name:  <input name=\"last_name\" type=\"text\"> <br>" +
                    "    <input type=\"submit\" name=\"action\" value=\"Submit\">" +
                    "</form>";

                    //otvet += "<p><img src=\"http://localhost:5555/img/radist/000/488_big.jpg\" /></p>";

                    soketAccept.Send(Encoding.GetEncoding(1251).GetBytes(Convert.ToString(otvet)));

                    soketAccept.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + DateTime.Now.ToString() + "] --> Listener [0][Exception - <" + ex.Message + ">");
            }
            finally
            {
                //Закриваю підключення основного сокета
                soketWork.Close();
            }

            Console.WriteLine("[" + DateTime.Now.ToString() + "] <-- Listener [0] - <Close>");
        }
    }
}
