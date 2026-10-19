using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

// Common protocol for the PC and Android projects.
// One UTF-8 message per line. ReadByte prevents the next game phase
// from accidentally consuming messages intended for another reader.
public static class LanProtocol
{
    private const int MaxMessageBytes = 16384;

    public static void WriteLine(NetworkStream stream, string message)
    {
        if (stream == null) throw new IOException("Network stream is not available.");
        if (message == null || message.Contains("\n") || message.Contains("\r"))
            throw new ArgumentException("Protocol messages must be single-line strings.");

        byte[] bytes = Encoding.UTF8.GetBytes(message + "\n");
        if (bytes.Length > MaxMessageBytes)
            throw new IOException("Network message is too long.");
        lock (stream)
        {
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    public static string ReadLine(NetworkStream stream)
    {
        if (stream == null) throw new IOException("Network stream is not available.");

        using (MemoryStream buffer = new MemoryStream())
        {
            while (true)
            {
                int value = stream.ReadByte();
                if (value < 0)
                {
                    if (buffer.Length == 0) return null; // clean disconnect
                    throw new EndOfStreamException("Connection closed mid-message.");
                }
                if (value == '\n')
                    return Encoding.UTF8.GetString(buffer.ToArray()).TrimEnd('\r');
                if (buffer.Length >= MaxMessageBytes)
                    throw new IOException("Network message exceeded size limit.");
                buffer.WriteByte((byte)value);
            }
        }
    }

    public static bool IsGameCommand(string value)
    {
        return value == "red" || value == "green" || value == "blue" ||
               value == "black" || value == "white";
    }
}
