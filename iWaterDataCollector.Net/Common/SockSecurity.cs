using System;
using System.Security.Cryptography;
using System.Text;

namespace iWaterDataCollector.Net.Common
{
    public class SockSecurity
    {
        #region 싱글톤
        private static SockSecurity _Instance = null;

        public static SockSecurity Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new SockSecurity();

                return _Instance;
            }
        }

        private SockSecurity()
        {
            
        }
        #endregion

        /// <summary>
        /// 트리플 DES 암호 서비스 공급자
        /// </summary>
        private TripleDESCryptoServiceProvider _tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
        /// <summary>
        /// 임의 값
        /// </summary>
        private int _randomValue = 0;
        /// 임의 개인 키 바이트 배열
        /// </summary>
        private byte[] _randomPrivateKeyByteArray = null;
        /// <summary>
        /// 임의 개인 키 초기 값 바이트 배열
        /// </summary>
        private byte[] _randomPrivateKeyInitialValueByteArray = null;
        /// <summary>
        /// 개인 키 바이트 배열
        /// </summary>
        private byte[] _privateKeyByteArray = new byte[]
        {
            98 , 45 , 125, 56, 1 , 60 , 11, 38 ,
            123, 54 , 234, 9 , 76, 20 , 44, 7  ,
            12 , 223, 219, 95, 48, 156, 32, 239
        };
        /// <summary>
        /// 개인 키 초기 값 바이트 배열
        /// </summary>
        private byte[] _privateKeyInitialValueByteArray = new byte[]
        {
            67, 12, 3, 41, 66, 78, 34, 123
        };

        #region 암호화 하기 - Encrypt(message)

        /// <summary>
        /// 암호화 하기
        /// </summary>
        /// <param name="message">메시지</param>
        /// <returns>암호화 메시지</returns>
        public string Encrypt(string message)
        {
            string messageEncrypted = null;

            byte[] textByteArray = UTF8Encoding.UTF8.GetBytes(message);

            messageEncrypted = Convert.ToBase64String
            (
                this._tripleDESCryptoServiceProvider.CreateEncryptor
                (
                    _randomPrivateKeyByteArray,
                    _randomPrivateKeyInitialValueByteArray
                ).TransformFinalBlock(textByteArray, 0, textByteArray.Length)
            );

            textByteArray = UTF8Encoding.UTF8.GetBytes(this._randomValue.ToString() + messageEncrypted);

            messageEncrypted = Convert.ToBase64String
            (
                _tripleDESCryptoServiceProvider.CreateEncryptor
                (
                    this._privateKeyByteArray,
                    this._privateKeyInitialValueByteArray
                ).TransformFinalBlock(textByteArray, 0, textByteArray.Length)
            );

            return messageEncrypted;
        }

        #endregion
        #region 암호화 풀기 - Decrypt(messageEncrypted)

        /// <summary>
        /// 암호화 풀기
        /// </summary>
        /// <param name="messageEncrypted">암호화 메시지</param>
        /// <returns>메시지</returns>
        public string Decrypt(string messageEncrypted)
        {
            string message = null;

            byte[] messageEncryptedByteArrat = Convert.FromBase64String(messageEncrypted);

            message = UTF8Encoding.UTF8.GetString
            (
                _tripleDESCryptoServiceProvider.CreateDecryptor
                (
                    this._privateKeyByteArray,
                    this._privateKeyInitialValueByteArray
                ).TransformFinalBlock(messageEncryptedByteArrat, 0, messageEncryptedByteArrat.Length)
            );

            SetRandomPrivateKeyByteArray(Convert.ToInt32(message.Substring(0, 1)));

            messageEncryptedByteArrat = Convert.FromBase64String(message.Substring(1, message.Length - 1));

            message = UTF8Encoding.UTF8.GetString
            (
                _tripleDESCryptoServiceProvider.CreateDecryptor
                (
                    this._randomPrivateKeyByteArray,
                    this._randomPrivateKeyInitialValueByteArray
                ).TransformFinalBlock(messageEncryptedByteArrat, 0, messageEncryptedByteArrat.Length)
            );

            return message;
        }

        #endregion
        #region 임의 개인 키 바이트 배열 설정하기 - SetRandomPrivateKeyByteArray(randomValue)

        /// <summary>
        /// 임의 개인 키 바이트 배열 설정하기
        /// </summary>
        /// <param name="randomValue">임의 값</param>
        private void SetRandomPrivateKeyByteArray(int randomValue)
        {
            this._randomPrivateKeyByteArray = new byte[_privateKeyByteArray.Length];
            this._randomPrivateKeyInitialValueByteArray = new byte[_privateKeyInitialValueByteArray.Length];

            for (int i = 0; i < _privateKeyByteArray.Length; i++)
            {
                int value = (int)_privateKeyByteArray[i];

                this._randomPrivateKeyByteArray[i] = Convert.ToByte(value / randomValue);
            }

            for (int i = 0; i < _privateKeyInitialValueByteArray.Length; i++)
            {
                int value = (int)_privateKeyInitialValueByteArray[i];

                this._randomPrivateKeyInitialValueByteArray[i] = Convert.ToByte(value / randomValue);
            }
        }
        #endregion

    }
}
