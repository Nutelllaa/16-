using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

class TCPServer
{
    static void Main(string[] args)
    {
        // ���� ��� ��������������� �'������
        int port = 12345;

        // ��������� TCPListener �� ��������� �����
        TcpListener server = new TcpListener(IPAddress.Any, port);

        // ������� ���������������
        server.Start();
        Console.WriteLine($"������ �������� �� ����� {port}");

        while (true)
        {
            // ���������� �� ���������� �볺���
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("�볺�� ����������!");

            // ������� �볺��� � �������� ������
            System.Threading.Tasks.Task.Run(() => HandleClient(client));
        }
    }

    static void HandleClient(TcpClient client)
    {
        try
        {
            // ��������� ������ ��� ������� �� ������ �����
            NetworkStream stream = client.GetStream();

            // ��������� ����� �����
            byte[] fileNameLengthBytes = new byte[4];
            stream.Read(fileNameLengthBytes, 0, 4);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBytes, 0);
            byte[] fileNameBytes = new byte[fileNameLength];
            stream.Read(fileNameBytes, 0, fileNameLength);
            string fileName = System.Text.Encoding.UTF8.GetString(fileNameBytes);

            // ��������� ������ �����
            byte[] fileSizeBytes = new byte[4];
            stream.Read(fileSizeBytes, 0, 4);
            int fileSize = BitConverter.ToInt32(fileSizeBytes, 0);

            // ��������� ����� �����
            byte[] fileData = new byte[fileSize];
            int bytesRead = stream.Read(fileData, 0, fileSize);

            // ���������� �����
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            File.WriteAllBytes(filePath, fileData);

            Console.WriteLine($"�������� ����� ����: {fileName}, �����: {fileSize} ����");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"������� ������� �볺���: {ex.Message}");
        }
        finally
        {
            // �������� �'������� � �볺����
            client.Close();
        }
    }
}
