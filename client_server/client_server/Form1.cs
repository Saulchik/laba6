using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace client_server
{
    public partial class Socets : Form
    {
        public Socets()
        {
            InitializeComponent();
        }

      

        static string address = "127.0.0.1"; 
        static int port =5212; 

         async void button1_Click(object sender, EventArgs e)
        {

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            // создаем сокет
            Socket listenSocket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать    
                // данные
                listenSocket.Bind(ipPoint);
                // начинаем прослушивание
                listenSocket.Listen(10);
                textBox4.Text = "Соединение с сервером было установлено."; // окно 
                                                                           // сервера
                await Task.Run(() =>
                {
                    while (true)
                    {
                        Socket handler = listenSocket.Accept();
                        // получаем сообщение
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байт
                        byte[] data = new byte[256]; // буфер для получаемых данных
                        do
                        {
                            bytes = handler.Receive(data);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (handler.Available > 0);
                        textBox3.Invoke(
                            (ThreadStart)delegate ()
                            {
                                textBox4.Text = textBox4.Text + "\r\n" +
     DateTime.Now.ToShortTimeString() + ": " + builder.ToString(); // окно сервера
                            });
                        // отправляем ответ
                        string message = "Сообщение было отправлено на сервер успешно.";
                        data = Encoding.Unicode.GetBytes(message);
                        handler.Send(data);
                        // закрываем сокет
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                });
            }
            catch (Exception ex)
            {
                textBox4.Text = ex.Message;
            }

        }

        async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                await Task.Run(() =>
                {
                    // подключаемся к удаленному хосту
                    socket.Connect(ipPoint);
                    string message = textBox3.Text;
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    socket.Send(data);
                    // получаем ответ
                    data = new byte[256]; // буфер для ответа
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байт
                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (socket.Available > 0);
                    textBox3.Invoke(
                                (ThreadStart)delegate ()
                                {
                                    textBox2.Text = textBox2.Text + "\r\n" + "ответ сервера: " + builder.ToString(); // окно клиента
                                });
                    // закрываем сокет
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                });
            }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message;
            }
            textBox3.Clear(); // очистка поля ввода сообщений

        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
