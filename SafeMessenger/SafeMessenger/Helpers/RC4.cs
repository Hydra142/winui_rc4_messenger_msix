using System.Linq;
using System.Text;

namespace SafeMessenge.Helpers
{
    public class RC4
    {
        byte[] S = new byte[128];

        int x = 0;
        int y = 0;

        public RC4(byte[] key)
        {
            init(key);
        }

        // Key-Scheduling Algorithm 
        // Алгоритм ключевого расписания 
        private void init(byte[] key)
        {
            int keyLength = key.Length;

            for (int i = 0; i < 128; i++)
            {
                S[i] = (byte)i;
            }

            int j = 0;
            for (int i = 0; i < 128; i++)
            {
                j = (j + S[i] + key[i % keyLength]) % 128;
                S.Swap(i, j);
            }
        }

        public byte[] Encode(byte[] dataB, int size)
        {
            byte[] data = dataB.Take(size).ToArray();

            byte[] cipher = new byte[data.Length];

            for (int m = 0; m < data.Length; m++)
            {
                cipher[m] = (byte)(data[m] ^ keyItem());
            }

            return cipher;
        }
        public byte[] Decode(byte[] dataB, int size)
        {
            return Encode(dataB, size);
        }

        // Pseudo-Random Generation Algorithm 
        // Генератор псевдослучайной последовательности 
        private byte keyItem()
        {
            x = (x + 1) % 128;
            y = (y + S[x]) % 128;

            S.Swap(x, y);

            return S[(S[x] + S[y]) % 128];
        }
    }

    static class SwapExt
    {
        public static void Swap<T>(this T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }

    public class RC4Helper
    {
        private byte[] _byteKey;
        public RC4Helper(string stringKey)
        {
            _byteKey = UTF32Encoding.Unicode.GetBytes(stringKey);
        }
        public string EncryptStringMessage(string message)
        {
            if (_byteKey.Length == 0)
            {
                return message;
            }
            RC4 _chiper = new(_byteKey);
            byte[] inputBytes = UTF32Encoding.Unicode.GetBytes(message);
            byte[] encryptedBytes = _chiper.Encode(inputBytes, inputBytes.Length);
            var encryptedString = UTF32Encoding.Unicode.GetString(encryptedBytes);
            return encryptedString;
        }

        public string DectyptStringMessage(string encryptedString)
        {
            if (_byteKey.Length == 0)
            {
                return encryptedString;
            }
            RC4 _chiper = new(_byteKey);
            byte[] encryptedBytes = UTF32Encoding.Unicode.GetBytes(encryptedString);
            byte[] decryptedBytes = _chiper.Decode(encryptedBytes, encryptedBytes.Length);
            string decryptedString = UTF32Encoding.Unicode.GetString(decryptedBytes);
            return decryptedString;
        }
    }
}
