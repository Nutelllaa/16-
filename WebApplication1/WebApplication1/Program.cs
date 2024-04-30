using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

class TCPServer
{
    static void Main(string[] args)
    {
        // Порт для прослуховування з'єднань
        int port = 12345;

        // Створення TCPListener на вказаному порту
        TcpListener server = new TcpListener(IPAddress.Any, port);

        // Початок прослуховування
        server.Start();
        Console.WriteLine($"Сервер запущено на порту {port}");

        while (true)
        {
            // Очікування на підключення клієнта
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Клієнт підключений!");

            // Обробка клієнта в окремому потоці
            System.Threading.Tasks.Task.Run(() => HandleClient(client));
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            // Отримання потоків для читання та запису даних
            NetworkStream stream = client.GetStream();

            // Отримання назви файлу
            byte[] fileNameLengthBytes = new byte[4];
            stream.Read(fileNameLengthBytes, 0, 4);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);
            byte[] fileNameBytes = new byte[fileNameLength];
            stream.Read(fileNameBytes, 0, fileNameLength);
            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);

            // Отримання розміру файлу
            byte[] fileSizeBytes = new byte[4];
            stream.Read(fileSizeBytes, 0, 4);
            int fileSize = BitConverter.ToInt32(fileSizeBytes, 0);

            // Отримання даних файлу
            byte[] fileData = new byte[fileSize];
            int bytesRead = stream.Read(fileData, 0, fileSize);

            // Збереження файлу
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllBytes(filePath, fileData);

            Console.WriteLine($"Отримано новий файл: {fileName}, Розмір: {fileSize} байт");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка обробки клієнта: {ex.Message}");
        }
        finally
        {
            // Закриття з'єднання з клієнтом
            client.Close();
        }
    }
}
