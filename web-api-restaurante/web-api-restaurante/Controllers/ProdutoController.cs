using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Data.Common;
using web_api_restaurante.NovaPasta1;

namespace wep_api_restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly string? _connectionString;

        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection()
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            String sql = "select id, nome, descricao, imageUrl from Produto; ";
            var result = await dbConnection.QueryAsync<Produto>(sql);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            String sql = "select id, nome, descricao, imageUrl from Produto where id = @id; ";
            var produto = await dbConnection.QueryFirstOrDefaultAsync<Produto>(sql, new { id });
            dbConnection.Close();

            if (produto == null)
                return NotFound();

            return Ok(produto);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT INTO Produto(nome, descricao, imageUrl)
          VALUES(@Nome, @Descricao, @ImageUrl);";
            await dbConnection.ExecuteAsync(query, produto);
            dbConnection.Close();
            return Ok();
        }
    }

}