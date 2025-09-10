using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;
using perfomance_cache.Model;
using StackExchange.Redis;

namespace perfomance_cache.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly string _mysqlConn = "Server=localhost;database=fiap;User=root;Password=123";
        private readonly string _redisConn = "localhost:6379";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string key = "get-users";

            try
            {
                var redis = await ConnectionMultiplexer.ConnectAsync(_redisConn);
                IDatabase db = redis.GetDatabase();

                string? userValue = await db.StringGetAsync(key);
                if (!string.IsNullOrEmpty(userValue))
                {
                    var cachedUsers = JsonConvert.DeserializeObject<List<Users>>(userValue);
                    return Ok(cachedUsers);
                }

                using var connection = new MySqlConnection(_mysqlConn);
                await connection.OpenAsync();

                string sql = "SELECT id AS Id, nome AS Nome, email AS Email, ultimo_acesso AS UltimoAcesso FROM users;";
                var users = (await connection.QueryAsync<Users>(sql)).ToList();

                var userJson = JsonConvert.SerializeObject(users);
                await db.StringSetAsync(key, userJson, TimeSpan.FromMinutes(15));

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao buscar usuários", details = ex.Message });
            }
        }
    }
}
