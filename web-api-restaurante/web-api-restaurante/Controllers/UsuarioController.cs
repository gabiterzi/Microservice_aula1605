using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_api_restaurante.NovaPasta1;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;

namespace web_api_restaurante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {

        private readonly string? _connectionString;

        public UsuarioController(IConfiguration configuration)
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
            String sql = "select id, nome, senha from usuario; ";
            var result = await dbConnection.QueryAsync<Usuario>(sql);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            String sql = "select id, nome, senha from usuario where id = @id; ";
            var produto = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
            dbConnection.Close();

            if (produto == null)
                return NotFound();

            return Ok(produto);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            using IDbConnection dbConnection = OpenConnection();
            string query = @"INSERT INTO usuario(nome, id, senha)
          VALUES(@Nome, @Id, @Senha);";
            await dbConnection.ExecuteAsync(query, usuario);
            dbConnection.Close();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var usuario = await dbConnection.QueryAsync<Usuario>("delete from usuario where id = @id;", new { id });
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Usuario usuario)
        {

            using IDbConnection dbConnection = OpenConnection();

            // Atualiza o produto
            var query = @"UPDATE usuario SET 
                          Nome = @Nome,
                          Senha = @Senha
                          WHERE Id = @Id";

            dbConnection.Execute(query, usuario);

            return Ok();
        }
    }
}
