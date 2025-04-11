using System.Text;

namespace OWOGame
{
    internal class ASCIIEncoder
    {
        public string Decode(byte[] buffer, int messageLength) => Encoding.ASCII.GetString(buffer, 0, messageLength);
        public byte[] Encode(string message) => Encoding.ASCII.GetBytes(message);
    }
}