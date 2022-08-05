using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using WebApplication1.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpregadoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment  _env;
        public EmpregadoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        
        //Metodo para pegar os dados
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                                select EmpregadoId,NomeEmpregado,Departamento,
                                convert(varchar(20), DatadeEntrada ) as DatadeEntrada,FotoFileName 
                                from dbo.Empregado
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        //Metodo para Inserir os dados
        [HttpPost]
        public JsonResult Post(Empregado emp)
        {
            string query = @"
                                insert into dbo.Empregado
                                (NomeEmpregado,Departamento,DatadeEntrada,FotoFileName)
                          values(@NomeEmpregado,@Departamento,@DatadeEntrada,@FotoFileName)
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@NomeEmpregado", emp.NomeEmpregado);
                    myComand.Parameters.AddWithValue("@Departamento", emp.Departamento);
                    myComand.Parameters.AddWithValue("@DatadeEntrada", emp.DatadeEntrada);
                    myComand.Parameters.AddWithValue("@FotoFileName", emp.FotoFileName);                   
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Sucesso ao Inserir");
        }

        [HttpPut]
        public JsonResult Put(Empregado emp)
        {
            string query = @"
                                update dbo.Empregado
                                set NomeEmpregado = (@NomeEmpregado),
                                Departamento = (@Departamento)
                                DatadeEntrada = (@DatadeEntrada)
                                FotoFileName = (@FotoFileName)                                
                                where EmpregadoId = @EmpregadoId    
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@DepartamentoId", emp.EmpregadoId);
                    myComand.Parameters.AddWithValue("@NomeEmpregado", emp.NomeEmpregado);
                    myComand.Parameters.AddWithValue("@Departamento", emp.Departamento);
                    myComand.Parameters.AddWithValue("@DatadeEntrada", emp.DatadeEntrada);
                    myComand.Parameters.AddWithValue("@FotoFileName", emp.FotoFileName);
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Sucesso na atualização");

        }
        //Metodo para deletar dados 
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                                delete from dbo.Empregado
                                where EmpregadoId = @EmpregadoId    
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@EmpregadoId", id);
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Dado deletado com sucesso");
        }
        
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Fotos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create)) 
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch(Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }

    }
}
