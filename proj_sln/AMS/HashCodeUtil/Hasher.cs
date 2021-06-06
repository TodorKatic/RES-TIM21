using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AMS.HashCodeUtil
{
    public class Hasher:IHasher
    {
        public int DeviceCodeLength { get; private set; }
        public string CharSet { get; private set; }

        public Hasher(int deviceCodeLength,string charSet)
        {
            if (charSet == null)
                throw new ArgumentNullException("CharSet ne sme biti null");

            charSet = charSet.Trim();
            if (charSet == string.Empty)
                throw new ArgumentException("CharSet ne sme biti prazan");

            this.CharSet = charSet;
            this.DeviceCodeLength = deviceCodeLength;
        }


        public int GetDeviceCode()
        {
            Random rand = new Random(DateTime.Now.Second);

            string randomString = new string(Enumerable.Repeat(CharSet, DeviceCodeLength).Select(s => s[rand.Next(CharSet.Length)]).ToArray());

            using (HashAlgorithm alg = SHA256.Create())
            {
                int hash;
                do
                {
                    hash = BitConverter.ToInt32(alg.ComputeHash(Encoding.ASCII.GetBytes(randomString)),0);
                } while (hash == -1);

                return hash;
            }
        }

    }
}
