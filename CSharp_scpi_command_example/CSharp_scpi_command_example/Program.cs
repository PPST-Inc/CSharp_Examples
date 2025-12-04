// PPST - SCPI commands via TCP/IP
// Version: 1.0.0
// Date: 12/03/2025

using System;
using System.Net.Sockets;
using System.Text;

namespace ScpiTcpCommandExample
{
    class Program
    {
        // Default configuration
        const string DEFAULT_IP = "192.168.131.193";
        const int DEFAULT_PORT = 5025;
        const int CONNECTION_TIMEOUT = 5000; // milliseconds

        static void Main(string[] args)
        {
            string ip = DEFAULT_IP;
            int port = DEFAULT_PORT;

            // Parse command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--ip" && i + 1 < args.Length)
                    ip = args[i + 1];
                else if (args[i] == "--port" && i + 1 < args.Length)
                    port = int.Parse(args[i + 1]);
                else if (args[i] == "--help" || args[i] == "-h")
                {
                    ShowHelp();
                    return;
                }
            }

            // Display connection information
            Console.WriteLine("============================================================");
            Console.WriteLine("SCPI COMMANDS - TCP/IP COMMUNICATION");
            Console.WriteLine("============================================================");
            Console.WriteLine("IP: " + ip);
            Console.WriteLine("Port: " + port);
            Console.WriteLine("============================================================");

            SocketConnection connection = null;

            try
            {
                connection = new SocketConnection(ip, port, CONNECTION_TIMEOUT);

                if (!connection.Connect())
                {
                    Console.WriteLine("\n[ERROR] Could not establish connection. Verify:");
                    Console.WriteLine("  - The equipment is powered on");
                    Console.WriteLine("  - The IP address is correct");
                    Console.WriteLine("  - The equipment is on the same network");
                    Console.WriteLine("  - The firewall allows the connection");
                    WaitForKeyPress("\nPress ENTER to close...");
                    Environment.Exit(1);
                }

                ScpiCommandMode(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[ERROR] Unexpected error: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                WaitForKeyPress("\nPress ENTER to close...");
            }
            finally
            {
                if (connection != null)
                    connection.Disconnect();
                Console.WriteLine("\n[OK] Program finished");
                WaitForKeyPress();
            }
        }

        static void ScpiCommandMode(SocketConnection socket)
        {
            Console.WriteLine("\nEnter SCPI commands directly. Type 'exit' to quit.");
            Console.WriteLine("Use '?' at the end of the command to make a query.\n");

            while (true)
            {
                try
                {
                    Console.Write("SCPI> ");
                    string input = Console.ReadLine();
                    string command = (input != null) ? input.Trim() : "";

                    if (string.IsNullOrEmpty(command))
                        continue;

                    if (command.ToLower() == "exit" || command.ToLower() == "quit")
                        break;

                    // If the command ends in '?', it's a query
                    if (command.EndsWith("?"))
                        socket.Query(command);
                    else
                        socket.SendCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR] Error: " + ex.Message);
                }
            }

            Console.WriteLine("[OK] Exiting...");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage examples:");
            Console.WriteLine("  CSharp_scpi_command_example.exe --ip 192.168.1.100");
            Console.WriteLine("  CSharp_scpi_command_example.exe --port 5025");
        }

        static void WaitForKeyPress(string message = "\nPress ENTER to close...")
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }

    // Class for communication with SCPI Unit via TCP/IP
    class SocketConnection
    {
        private string ip;
        private int port;
        private int timeout;
        private TcpClient client;
        private NetworkStream stream;

        public SocketConnection(string ip, int port, int timeout)
        {
            this.ip = ip;
            this.port = port;
            this.timeout = timeout;
        }

        public bool Connect()
        {
            try
            {
                client = new TcpClient();
                client.ReceiveTimeout = timeout;
                client.SendTimeout = timeout;
                client.Connect(ip, port);
                stream = client.GetStream();
                Console.WriteLine("[OK] Connected to " + ip + ":" + port);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Connection error: " + ex.Message);
                return false;
            }
        }

        public void SendCommand(string command)
        {
            try
            {
                if (!command.EndsWith("\n"))
                    command += "\n";

                byte[] data = Encoding.ASCII.GetBytes(command);
                stream.Write(data, 0, data.Length);
                Console.WriteLine(">> Sent: " + command.Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Error sending command: " + ex.Message);
            }
        }

        public string Query(string command)
        {
            try
            {
                if (!command.EndsWith("\n"))
                    command += "\n";

                byte[] data = Encoding.ASCII.GetBytes(command);
                stream.Write(data, 0, data.Length);
                Console.WriteLine(">> Query: " + command.Trim());

                byte[] buffer = new byte[65535];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine("<< Response: " + response);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Query error: " + ex.Message);
                return null;
            }
        }

        public void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
            if (client != null)
            {
                client.Close();
                client = null;
                Console.WriteLine("[OK] Disconnected from " + ip);
            }
        }
    }
}
