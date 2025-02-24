using System;
using System.Text;

namespace Visitor_Management_Portal.Utilities
{
    public class DataEncryptionHelper
    {
        public static string Encryptdata(string data)
        {
            string strmsg = string.Empty;
            byte[] encode = new byte[data.Length];
            encode = Encoding.UTF8.GetBytes(data);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;
        }

        public static string Decryptdata(string encryptedData)
        {
            string data = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            System.Text.Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptedData);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            data = new String(decoded_char);
            return data;
        }
    }
}