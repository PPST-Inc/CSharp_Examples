# C# SCPI Communication Examples

Collection of C# examples using SCPI (Standard Commands for Programmable Instruments) protocol via TCP/IP and VISA.

## Available Examples

### 1. **CSharp_basic_configuration_example**
Basic configuration of a power supply unit via TCP/IP with predefined commands.
- No external dependencies
- Pure TCP/IP sockets
- Automated voltage/frequency configuration

### 2. **CSharp_scpi_command_example**
Interactive SCPI command interface via TCP/IP.
- No external dependencies
- Pure TCP/IP sockets
- Interactive command mode

### 3. **CSharp_visa_example**
SCPI communication via VISA (supports TCPIP, GPIB, USB, etc.).
- Requires NI-VISA Runtime
- NationalInstruments.VisaNS DLLs included in `Libs` folder
- Supports multiple interface types (TCPIP, GPIB, USB)

---

## Prerequisites

### For All Examples
- **Visual Studio 2013 or later** (Express, Community, or Professional)
- **Windows OS** (Windows 7 or later)
- **.NET Framework 4.5** or higher

### For VISA Example Only
- **NI-VISA Runtime** (free download from National Instruments)
  - Download: https://www.ni.com/es-mx/support/downloads/drivers/download.ni-visa.html
  - Version: 2023 Q3 or later recommended

---

## Compilation Instructions

### Using Visual Studio (Recommended)

#### For TCP/IP Examples (basic_configuration & scpi_command)
1. Open the `.sln` file:
   - `CSharp_basic_configuration_example.sln`, or
   - `CSharp_scpi_command_example.sln`
2. Press **F6** or go to **Build → Build Solution**
3. The executable will be in `bin\Debug\` or `bin\Release\`

#### For VISA Example
1. Open `CSharp_visa_example.sln`
2. Verify that the `Libs` folder contains the DLLs:
   - `NationalInstruments.Common.dll`
   - `NationalInstruments.VisaNS.dll`
3. Check **References** in Solution Explorer (should show NI DLLs)
4. Press **F6** or go to **Build → Build Solution**
5. The executable will be in `bin\Debug\` or `bin\Release\`

---

## Usage Examples

### Basic Configuration Example

```cmd
# Default connection
CSharp_basic_configuration_example.exe

# Custom IP and port
CSharp_basic_configuration_example.exe --ip 192.168.1.100 --port 5025

# Show help
CSharp_basic_configuration_example.exe --help
```

**What it does:**
- Connects to the instrument
- Configures AC voltage mode
- Sets voltage to 100V
- Sets frequency to 60Hz
- Enables output
- Queries voltage and frequency

### SCPI Command Example (Interactive)

```cmd
# Default connection
CSharp_scpi_command_example.exe

# Custom IP
CSharp_scpi_command_example.exe --ip 192.168.1.100

# Then type commands interactively:
SCPI> *IDN?
SCPI> VOLT:AC 100
SCPI> FREQ 60
SCPI> OUTP 1
SCPI> MEAS:VOLT?
SCPI> exit
```

### VISA Example

```cmd
# Default resource
CSharp_visa_example.exe

# Custom VISA resource
CSharp_visa_example.exe --resource "TCPIP0::192.168.1.100::inst0::INSTR"

# GPIB connection
CSharp_visa_example.exe --resource "GPIB0::10::INSTR"

# USB connection
CSharp_visa_example.exe --resource "USB0::0x1234::0x5678::SN12345::INSTR"

# Custom timeout
CSharp_visa_example.exe --timeout 10000

# Show help
CSharp_visa_example.exe --help
```

**Common VISA Resource String Formats:**
- **TCPIP**: `TCPIP0::<ip_address>::inst0::INSTR`
- **GPIB**: `GPIB0::<address>::INSTR`
- **USB**: `USB0::<vendor_id>::<product_id>::<serial>::INSTR`

---

## Troubleshooting

### "Could not establish connection" Error
- Verify the instrument is powered on
- Check IP address/resource string is correct
- Ensure firewall allows the connection
- For VISA: Verify NI-VISA Runtime is installed

### "The type or namespace name 'MessageBasedSession' could not be found"
- Check that `Libs` folder contains the NI DLLs
- Verify References in Visual Studio show the NI libraries
- Right-click References → Add Reference → Browse to `Libs` folder

### "VISA Runtime not found" Error
- Download and install NI-VISA Runtime from National Instruments website
- Restart Visual Studio after installation

---

## Notes

- **TCP/IP examples** work with any SCPI instrument supporting raw socket communication (port 5025 standard)
- **VISA example** provides more flexibility (GPIB, USB, Serial) but requires NI-VISA Runtime installed
- All examples use traditional C# syntax compatible with Visual Studio 2013+
- Default IP addresses are configured in the source code constants
- All programs support `--help` flag for usage information

---

## Additional Resources

- NI-VISA Download: https://www.ni.com/visa
- .NET Framework: https://dotnet.microsoft.com/

### VISA errors
- Install NI-VISA: https://www.ni.com/en-us/support/downloads/drivers/download.ni-visa.html
