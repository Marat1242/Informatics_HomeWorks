using System;
using System.IO;
using System.Net;

namespace ORIS.week5
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;

        public HttpServer()
        {
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:8888/google/");
        }

        public void Start()
        {
            Console.WriteLine("Запуск сервера");
            listener.Start();
            Console.WriteLine("Сервер запущен");
            Receive();
        }

        public void Stop()
        {
            Console.WriteLine("Остановка сервера");
            listener.Stop();
            Console.WriteLine("Сервер остановлен");
        }

        private void Receive()
        {
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            if (listener.IsListening)
            {
                var context = listener.EndGetContext(result);
                var request = context.Request;

                // получаем объект ответа
                var response = context.Response;

                // создаем ответ в виде кода html
                string responseStr = File.ReadAllText("/Users/alsufaizova/Projects/ORIS/ORIS/week5/index.html");
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);

                // получаем поток ответа и пишем в него ответ
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                // закрываем поток
                output.Close();

                Receive();
            }
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}