// PPST - Unit configuration example via TCP/IP
// Version: 1.0.0
// Date: 12/03/2025

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ScpiTcpExample
{
    class Program
    {
        // Default configuration
        const string DEFAULT_IP = "192.168.131.182";
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
            Console.WriteLine("BASIC UNIT CONFIGURATION - TCP/IP COMMUNICATION");
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
                    Console.WriteLine("\n✗ Could not establish connection. Verify:");
                    Console.WriteLine("  - The equipment is powered on");
                    Console.WriteLine("  - The IP address is correct");
                    Console.WriteLine("  - The equipment is on the same network");
                    Console.WriteLine("  - The firewall allows the connection");
                    WaitForKeyPress();
                    Environment.Exit(1);
                }

                BasicConfiguration(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n✗ Unexpected error: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                WaitForKeyPress();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Disconnect();
                }
                Console.WriteLine("\n✓ Program finished");
                WaitForKeyPress();
            }
        }

        static void BasicConfiguration(SocketConnection socket)
        {
            Console.WriteLine("\n→ Starting basic configuration...");
            Thread.Sleep(2000); // Wait 2 seconds before executing

            // Configure voltage mode to AC
            socket.SendCommand("VOLT:MODE AC");

            // Configure AC voltage to 100V
            socket.SendCommand("VOLT:AC 100");

            // Set frequency to 60 Hz
            socket.SendCommand("FREQ 60");

            // Enable output
            socket.SendCommand("OUTP 1;");
            socket.Query(":*OPC?");

            // Query Volt and Frequency to verify settings
            socket.Query("MEAS:VOLT?");
            socket.Query("MEAS:FREQ?");

            Console.WriteLine("✓ Basic configuration completed\n");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage examples:");
            Console.WriteLine("  CSharp_basic_configuration_example.exe --ip 192.168.1.100");
            Console.WriteLine("  CSharp_basic_configuration_example.exe --port 5025");
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
                Console.WriteLine("✓ Connected to " + ip +":" + port);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("✗ Connection error: " + ex.Message);
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
                Console.WriteLine("→ Sent: " + command.Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine("✗ Error sending command: " + ex.Message);
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
                Console.WriteLine("→ Query: " + command.Trim());

                byte[] buffer = new byte[65535];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine("← Response: " + response);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine("✗ Query error: " + ex.Message);
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
                Console.WriteLine("✓ Disconnected from " + ip);
            }
        }
    }
}
