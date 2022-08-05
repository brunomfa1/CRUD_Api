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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartamentoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        //Metodo para pegar os dados
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                                select DepartamentoId,DepartamentoNome 
                                from dbo.Departamento
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

        //Metodo para inserir dados
        [HttpPost]
        public JsonResult Post(Departamento dep)
        {
            string query = @"
                                insert into dbo.Departamento
                                 values(@DepartamentoNome)
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@DepartamentoNome", dep.DepartamentoNome);
                    myReader = myComand .ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Sucesso ao Inserir");
            
        }

        //Metodo para atualizar dados 
        [HttpPut]
        public JsonResult Put(Departamento dep)
        {
            string query = @"
                                update dbo.Departamento
                                set DepartamentoNome = (@DepartamentoNome)
                                where DepartamentoId = @DepartamentoId    
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@DepartamentoId", dep.DepartamentoId);
                    myComand.Parameters.AddWithValue("@DepartamentoNome", dep.DepartamentoNome);
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
                                delete from dbo.Departamento
                                where DepartamentoId = @DepartamentoId    
                           ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmpregadoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myComand = new SqlCommand(query, myCon))
                {
                    myComand.Parameters.AddWithValue("@DepartamentoId", id);
                    myReader = myComand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Dado deletado com sucesso");

        }
    }
}
