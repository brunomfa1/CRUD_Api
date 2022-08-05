using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string myKey = string.Empty;

        public UsuarioController(IConfiguration configurationn)
        {
            _configuration = configurationn;
            
        }

        //Metodo para pegar os dados
        [HttpPost]
        public JsonResult Post(Usuario User)
        {
            if (User.Login is null || User.Senha is null) return new JsonResult("usuario nulo");

            var teste = OpCrypto(User.Senha, true, 2);
            string query = @"
                                Select *
                                from dbo.UsuarioCrypto
                                Where Login = @Login AND SENHA = @Senha";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@Login", User.Login);
                    myComand.Parameters.AddWithValue("@Senha", OpCrypto(User.Senha,true, 2));
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            var dadosLogin = ConvertToList<Usuario>(table);
            if (dadosLogin is null ) return new JsonResult("usuario nulo");

            var token = Generate(dadosLogin);
            return new JsonResult(Ok(token));
        }

        public T ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList().FirstOrDefault();
        }

        private string Generate(Usuario user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Login)
              
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


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