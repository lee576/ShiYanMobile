using System;
using System.Collections.Generic;

namespace WebApi.Utility
{
    public class XxteaHelper
    {
        public static string Decrypt(string needDecryptStr, UInt32[] keyC)
        {
            int k = 0, EnStrLen = 0, nIntCount = 0;
            string NeedDecryptStr;
            NeedDecryptStr = needDecryptStr;
            EnStrLen = NeedDecryptStr.Length;
            nIntCount = EnStrLen / 8;

            string[] r = SplitByLen(NeedDecryptStr, 8);
            UInt32[] V = new UInt32[20];
            UInt32[] Vout = new UInt32[20];
            k = 0;
            foreach (string s in r)
            {
                Console.WriteLine("{0}", s);
                V[k++] = (UInt32)Convert.ToInt32(s, 16);
            }

            Vout = Decrypt(V, keyC, nIntCount);

            string result = "";
            for (k = 0; k < nIntCount; k++)
                result = result + V[k].ToString("X");
            return result;
        }

        public static UInt32[] Decrypt(UInt32[] v, UInt32[] k, Int32 n)
        {
            UInt32 z = v[n - 1], y = v[0], delta = 0x9E3779B9, sum = 0, e;
            Int32 p, q = 6 + 52 / n;

            q = 6 + 52 / n;
            sum = (UInt32)q * (UInt32)delta;
            while (sum != 0)
            {
                e = (sum >> 2) & 3;
                for (p = n - 1; p > 0; p--)
                {
                    z = v[p - 1];
                    y = unchecked(v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                }
                z = v[n - 1];
                y = unchecked(v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                sum -= delta;
            }
            return v;

        }

        public static string Encrypt(string needEnCryptStr, UInt32[] keyC)
        {
            int k = 0, EnStrLen = 0, nIntCount = 0, nRemaind = 0;
            string NeedEnCryptStr;
            NeedEnCryptStr = needEnCryptStr;
            EnStrLen = NeedEnCryptStr.Length;
            nIntCount = EnStrLen / 8;
            nRemaind = EnStrLen % 8;
            if (nRemaind != 0)
            {
                nIntCount++;
            }
            string[] r = SplitByLen(NeedEnCryptStr, 8);
            UInt32[] V = new UInt32[20];
            UInt32[] Vout = new UInt32[20];
            k = 0;
            foreach (string s in r)
            {
                Console.WriteLine("{0}", s);
                V[k++] = (UInt32)Convert.ToInt32(s, 16);
            }

            Vout = Encrypt(V, keyC, nIntCount);

            string result = "";
            for (k = 0; k < nIntCount; k++)
                result = result + V[k].ToString("X").PadLeft(8, '0');
            return result;
        }

        private static string[] SplitByLen(string str, int separatorCharNum)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= separatorCharNum)
            {
                return new string[] { str };
            }
            string tempStr = str;
            List<string> strList = new List<string>();
            int iMax = Convert.ToInt32(Math.Ceiling(str.Length / (separatorCharNum * 1.0)));//获取循环次数  
            for (int i = 1; i <= iMax; i++)
            {
                string currMsg = tempStr.Substring(0, tempStr.Length > separatorCharNum ? separatorCharNum : tempStr.Length);
                strList.Add(currMsg);
                if (tempStr.Length > separatorCharNum)
                {
                    tempStr = tempStr.Substring(separatorCharNum, tempStr.Length - separatorCharNum);
                }
            }
            return strList.ToArray();
        }

        private static UInt32[] Encrypt(UInt32[] v, UInt32[] k, Int32 n)
        {
            UInt32 z = v[n - 1], y = v[0], delta = 0x9E3779B9, sum = 0, e;
            Int32 p, q = 6 + 52 / n;
            while (q-- > 0)
            {
                sum = unchecked(sum + delta);
                e = sum >> 2 & 3;
                for (p = 0; p < n - 1; p++)
                {
                    y = v[p + 1];
                    z = unchecked(v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
                }
                y = v[0];
                z = unchecked(v[n - 1] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
            }
            return v;
        }
    }
}
