// PPST - SCPI commands via VISA
// Version: 1.0.0
// Date: 12/04/2025
// Requires: NI-VISA Runtime installed on the system

using System;
using NationalInstruments.VisaNS;

namespace ScpiVisaCommandExample
{
    class Program
    {
        // Default configuration
        const string DEFAULT_RESOURCE = "TCPIP0::192.168.131.193::inst0::INSTR";
        const int CONNECTION_TIMEOUT = 5000; // milliseconds

        static void Main(string[] args)
        {
            string resource = DEFAULT_RESOURCE;
            int timeout = CONNECTION_TIMEOUT;

            // Parse command line arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--resource" && i + 1 < args.Length)
                    resource = args[i + 1];
                else if (args[i] == "--timeout" && i + 1 < args.Length)
                    timeout = int.Parse(args[i + 1]);
                else if (args[i] == "--help" || args[i] == "-h")
                {
                    ShowHelp();
                    return;
                }
            }

            // Display connection information
            Console.WriteLine("============================================================");
            Console.WriteLine("SCPI COMMANDS - VISA COMMUNICATION");
            Console.WriteLine("============================================================");
            Console.WriteLine("Resource: " + resource);
            Console.WriteLine("Timeout: " + timeout + " ms");
            Console.WriteLine("============================================================");

            VISAConnection connection = null;

            try
            {
                connection = new VISAConnection(resource, timeout);

                if (!connection.Connect())
                {
                    Console.WriteLine("\n✗ Could not establish connection. Verify:");
                    Console.WriteLine("  - VISA libraries are properly installed");
                    Console.WriteLine("  - The equipment is powered on");
                    Console.WriteLine("  - The resource string is correct");
                    Console.WriteLine("  - The equipment is accessible via VISA");
                    WaitForKeyPress();
                    Environment.Exit(1);
                }

                // Query instrument identification
                Console.WriteLine("\nQuerying instrument identification...");
                connection.Query("*IDN?");

                // Start interactive command mode
                ScpiCommandMode(connection);
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

        static void ScpiCommandMode(VISAConnection visa)
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
                        visa.Query(command);
                    else
                        visa.SendCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("✗ Error: " + ex.Message);
                }
            }

            Console.WriteLine("✓ Exiting...");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage examples:");
            Console.WriteLine("  CSharp_scpi_command_by_visa_example.exe");
            Console.WriteLine("  CSharp_scpi_command_by_visa_example.exe --resource \"TCPIP0::192.168.1.100::inst0::INSTR\"");
            Console.WriteLine("  CSharp_scpi_command_by_visa_example.exe --resource \"GPIB0::10::INSTR\"");
            Console.WriteLine("\nCommon VISA resource string formats:");
            Console.WriteLine("  TCPIP: TCPIP0::<ip_address>::inst0::INSTR");
            Console.WriteLine("  GPIB:  GPIB0::<address>::INSTR");
        }

        static void WaitForKeyPress(string message = "\nPress ENTER to close...")
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }

    // Class for communication with SCPI Unit via VISA
    class VISAConnection
    {
        private string resourceString;
        private int timeout;
        private MessageBasedSession instrument;

        public VISAConnection(string resourceString, int timeout)
        {
            this.resourceString = resourceString;
            this.timeout = timeout;
        }

        public bool Connect()
        {
            try
            {
                instrument = (MessageBasedSession)ResourceManager.GetLocalManager().Open(resourceString);
                instrument.Timeout = timeout;
                Console.WriteLine("✓ Connected to " + resourceString);
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
                instrument.Write(command);
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
                string response = instrument.Query(command).Trim();
                Console.WriteLine("→ Query: " + command.Trim());
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
            if (instrument != null)
            {
                instrument.Dispose();
                instrument = null;
                Console.WriteLine("✓ Disconnected from " + resourceString);
            }
        }
    }
}
