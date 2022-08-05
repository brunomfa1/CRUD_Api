using System;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess
{
    public class Cryptography : ICryptography
    {

        string myKey = string.Empty;

        #region Método reponsável por congifigurar a criptografia.
        private ICryptoTransform ConfigureCryptography(bool operacao)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

            //Obtem a chave a ser criptografa.
            des.Key = hashmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(myKey));
            des.Mode = CipherMode.ECB;

            if (operacao)
                return des.CreateEncryptor();

            return des.CreateDecryptor();

        }
        #endregion

        #region Função para cryptografar os informaçoes do sistema
        private string Crypto(string texto)
        {
            try
            {
                // Dim MyASCIIEncoding = New ASCIIEncoding()
                byte[] buff = ASCIIEncoding.ASCII.GetBytes(texto);
                return Convert.ToBase64String(ConfigureCryptography(true).TransformFinalBlock(buff, 0, buff.Length));
            }
            catch  
            {
                //MessageBox.Show(ex.Message, "GEOCID ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        #region "Método para descriptografia um objeto"
        private string DesCrypto(string texto)
        {
            try
            {
                byte[] buff = Convert.FromBase64String(texto);
                return ASCIIEncoding.ASCII.GetString(ConfigureCryptography(false).TransformFinalBlock(buff, 0, buff.Length));
            }
            catch  
            {
                //MessageBox.Show(ex.Message, "GEOCID ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion


        /// <summary>
        /// Método para Criptografar e Descritografar.
        /// </summary>
        /// <param name="texto">Informar o texto que será Criptografado ou Descriptografao.</param>
        /// <param name="operacao">Informe o tipo de operação [true]: Criptografa e [false]: Descriptografa.</param>
        /// <param name="chave">Informe o tipo da chave [1]: Chave para o serial e [2]: Chave do sistema.</param>
        /// <returns></returns>
        #region Função para que retorno o tipo de operação se é para criptografar ou descriptografar
        public string OpCrypto(string texto, bool operacao, int chave)
        {
            //Verifica o tipo da chave informada
            if (chave == 1)
                myKey = "MTGEO2019-UP03"; // Chave para a ativação do Sistema.
            else
                myKey = "WCOGEOMTGEO"; // Chave para salvar informações do sistema, como: Senha usuário, Conexão, etc...


            //Executa a operação, se [True] Criptografa e [False] Descriptografa.
            if (operacao)
                return Crypto(texto);
            else
                return DesCrypto(texto);
        }
        #endregion

        #region Converter o Numero do Hd para Hexadecimal
        public string ConvertHexa(string asciiString)
        {
            //Retorna o número convertido do código do hd para ser gerado o serial.
            string numero = string.Empty;
            foreach (char num in asciiString)
            {
                int tmp = num;
                numero += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return numero;
        }
        #endregion
    }
}
