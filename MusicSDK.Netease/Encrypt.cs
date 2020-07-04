using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Numerics;
using MusicSDK.Netease.Library;
using System.IO;

namespace MusicSDK.Netease
{
    public static class Encrypt
    {
        static readonly byte[] NONCE = Encoding.UTF8.GetBytes("0CoJUm6Qyw8W8jud");
        static readonly byte[] IV = Encoding.UTF8.GetBytes("0102030405060708");
        static readonly byte[] PCKey = Encoding.UTF8.GetBytes("e82ckenh8dichen8");

        const string PUBKEY = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDgtQn2JZ34ZC28NWYpAUd98iZ37BUrX/aKzmFbt7clFSs6sXqHauqKWqdtLkF2KexO40H1YTX8z2lSgBBOAxLsvaklV8k4cBFK9snQXE9/DDaFt6Rr7iVZMldczhC0JNgTz+SHXT6CBHuX3e9SdB1Ua44oncaTWz7OBGLbCiK45wIDAQAB\n-----END PUBLIC KEY-----";


        static readonly Random random = new Random();
        public static Dictionary<string, string> EncryptWebRequest(object data, byte[]? secKey = null)
        {
            if (data == null)
            {
                throw new ArgumentNullException("Data cannot be null");
            };
            var text = data is string _text ? _text : JsonConvert.SerializeObject(data);
            secKey ??= CreateSecretKey(16);
            // System.IO.File.WriteAllText("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/web_encrypt_data.in", text);
            // System.IO.File.WriteAllBytes("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/web_encrypt_secKey.in", secKey);
            // System.IO.File.WriteAllText("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/web_encrypt_params.out", aesEncrypt(aesEncrypt(text, NONCE, IV), secKey, IV));
            // System.IO.File.WriteAllText("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/web_encrypt_encSecKey.out", rsaEncrypt(secKey));

            return new Dictionary<string, string>{
                {"params", AesEncrypt(AesEncrypt(text, NONCE, IV), secKey, IV)},
                {"encSecKey", RsaEncrypt(secKey)}
            };
        }
        public static Dictionary<string, string> EncryptPCRequest(string path, object data)
        {
            var text = data is string _text ? _text : JsonConvert.SerializeObject(data);
            // Ssytem.IO.File.WriteAllText("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/pc_encrypt_lyric.in", text);
            var target = path.Replace("/eapi", "/api");
            var message = $"nobody{target}use{text}md5forencrypt";
            var digest = Md5(message);
            var text2 = $"{target}-36cd479b6b5-{text}-36cd479b6b5-{digest}";
            var encBytes = Convert.FromBase64String(AesEncrypt(text2, PCKey, Mode: CipherMode.ECB));
            // System.IO.File.WriteAllText("/home/kevin/Projects/MusicSDK.Netease.Test/fixture/pc_encrypt_lyric.out", encBytes.ToHexString());
            return new Dictionary<string, string> {
                { "params", encBytes.ToHexString() }
            };
        }

        public static string DecryptPCResponse(byte[] bytes)
        {
            return AesDecrypt(bytes, PCKey, Mode: CipherMode.ECB);
        }

        static byte[] CreateSecretKey(int size)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return Encoding.UTF8.GetBytes(Enumerable
            .Repeat(chars, size)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
        }

        static string AesEncrypt(string text, byte[] Key, byte[]? IV = null, CipherMode Mode = CipherMode.CBC)
        {
            var aes = Aes.Create();
            var source = Encoding.UTF8.GetBytes(text);

            aes.Key = Key;
            aes.IV = IV ?? Enumerable.Repeat((byte)0, aes.IV.Length).ToArray();
            aes.Mode = Mode;

            using var encryptor = aes.CreateEncryptor();
            var result = encryptor.TransformFinalBlock(source, 0, source.Length);
            return Convert.ToBase64String(result);
        }

        static string AesDecrypt(byte[] source, byte[] Key, byte[]? IV = null, CipherMode Mode = CipherMode.CBC)
        {
            var aes = Aes.Create();
            aes.Key = Key;
            aes.BlockSize = 128;
            aes.IV = IV ?? Enumerable.Repeat((byte)0, aes.IV.Length).ToArray();
            aes.Mode = Mode;
            using var decryptor = aes.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(source, 0, source.Length);
            return Encoding.UTF8.GetString(result);
        }

        static string RsaEncrypt(IEnumerable<byte> text, string publicKey = PUBKEY)
        {
            var hexText = text.Reverse().ToHexString();
            var rsaParams = RSAUtility.Parse(publicKey);
            var hexRet = BigInteger.ModPow(
                BigInteger.Parse("0" + hexText, NumberStyles.HexNumber),
                BigInteger.Parse("0" + rsaParams.Exponent.ToHexString(), NumberStyles.HexNumber),
                BigInteger.Parse("0" + rsaParams.Modulus.ToHexString(), NumberStyles.HexNumber)
            );
            return hexRet.ToString("x2").TrimStart('0').PadLeft(256, '0');
        }

        public static string Md5(string text)
        {
            using var md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(text);
            byte[] retBs = md5.ComputeHash(bs);
            return retBs.ToHexString(false);
        }
    }


    public static class RSAUtility
    {
        static readonly Dictionary<string, RSAParameters> cache = new Dictionary<string, RSAParameters>();
        public static RSAParameters Parse(string publicKey)
        {
            if (cache.TryGetValue(publicKey, out var c))
            {
                return c;
            }
            var _publicKey = publicKey.Replace("\n", string.Empty);
            _publicKey = _publicKey[26..^24];
            using MemoryStream _stream = new MemoryStream(Convert.FromBase64String(_publicKey));
            using BinaryReader _reader = new BinaryReader(_stream);
            ushort _i16;
            byte[] _oid;
            byte _i8;
            byte _low;
            byte _high;
            int _modulusLength;
            byte[] _modulus;
            int _exponentLength;
            byte[] _exponent;

            _i16 = _reader.ReadUInt16();
            if (_i16 == 0x8130)
                _reader.ReadByte();
            else if (_i16 == 0x8230)
                _reader.ReadInt16();
            else
                throw new ArgumentException(nameof(_publicKey));
            _oid = _reader.ReadBytes(15);
            if (!_oid.SequenceEqual(new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 }))
                throw new ArgumentException(nameof(_publicKey));
            _i16 = _reader.ReadUInt16();
            if (_i16 == 0x8103)
                _reader.ReadByte();
            else if (_i16 == 0x8203)
                _reader.ReadInt16();
            else
                throw new ArgumentException(nameof(_publicKey));
            _i8 = _reader.ReadByte();
            if (_i8 != 0x00)
                throw new ArgumentException(nameof(_publicKey));
            _i16 = _reader.ReadUInt16();
            if (_i16 == 0x8130)
                _reader.ReadByte();
            else if (_i16 == 0x8230)
                _reader.ReadInt16();
            else
                throw new ArgumentException(nameof(_publicKey));
            _i16 = _reader.ReadUInt16();
            if (_i16 == 0x8102)
            {
                _high = 0;
                _low = _reader.ReadByte();
            }
            else if (_i16 == 0x8202)
            {
                _high = _reader.ReadByte();
                _low = _reader.ReadByte();
            }
            else
                throw new ArgumentException(nameof(_publicKey));
            _modulusLength = BitConverter.ToInt32(new byte[] { _low, _high, 0x00, 0x00 }, 0);
            if (_reader.PeekChar() == 0x00)
            {
                _reader.ReadByte();
                _modulusLength -= 1;
            }
            _modulus = _reader.ReadBytes(_modulusLength);
            if (_reader.ReadByte() != 0x02)
                throw new ArgumentException(nameof(_publicKey));
            _exponentLength = _reader.ReadByte();
            _exponent = _reader.ReadBytes(_exponentLength);
            return new RSAParameters
            {
                Modulus = _modulus,
                Exponent = _exponent
            };
        }
    }
}