using System.Net;
using System.Text;

namespace ORIS.week7
{
    public class HttpServer : IDisposable
    {
        private readonly HttpListener listener;
        public ServerStatus Status = ServerStatus.Stop;
        private ServerSettings serverSettings;

        public HttpServer()
        {
            serverSettings = ServerSettings.Deserialize();
            listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:" + serverSettings.Port + "/");
        }

        public void Start()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Сервер уже запущен!");
            }
            else
            {
                Console.WriteLine("Запуск сервера");
                listener.Start();
                Console.WriteLine("Сервер запущен");
                Status = ServerStatus.Start;
            }
            Receive();
        }

        public void Stop()
        {
            if (Status == ServerStatus.Start)
            {
                Console.WriteLine("Остановка сервера...");
                listener.Stop();
                Console.WriteLine("Сервер остановлен");
                Status = ServerStatus.Stop;
            }
            else
                Console.WriteLine("Сервер уже остановлен");
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

                var rawUrl = request.RawUrl;

                byte[] buffer;

                if (Directory.Exists(serverSettings.Path))
                {
                    buffer = Files.GetFile(rawUrl.Replace("%20", " "));

                    //Задаю расширения для файлов
                    Files.GetExtension(ref response, "." + rawUrl);

                    if (buffer == null)
                    {
                        response.Headers.Set("Content-Type", "text/plain");

                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string err = "404 - not found";
                        buffer = Encoding.UTF8.GetBytes(err);
                    }
                }
                else
                {
                    string err = $"Directory '{serverSettings.Path}' doesn't found";
                    buffer = Encoding.UTF8.GetBytes(err);
                }

                // получаем поток ответа и пишем в него ответ
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