Projeto em C# que implementa gerenciamento de sessões de usuário utilizando MySQL como banco de dados e Redis como cache.

////////////////////////////////////////////////////////////////////////////////

Estrutura de Classes:
public class Users
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public DateTime UltimoAcesso { get; set; }
}
////////////////////////////////////////////////////////////////////////////////

Conexão com MySQL e Redis:
var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
using var connection = new MySqlConnection("Server=localhost;database=fiap;User=root;Password=123");

////////////////////////////////////////////////////////////////////////////////

Lógica de Cache:

string key = "get-users";
string? userValue = await db.StringGetAsync(key);

if (!string.IsNullOrEmpty(userValue))
{
    var cachedUsers = JsonConvert.DeserializeObject<List<Users>>(userValue);
    return Ok(cachedUsers);
}

string sql = "SELECT id AS Id, nome AS Nome, email AS Email, ultimo_acesso AS UltimoAcesso FROM users;";
var users = (await connection.QueryAsync<Users>(sql)).ToList();

await db.StringSetAsync(key, JsonConvert.SerializeObject(users), TimeSpan.FromMinutes(15));

////////////////////////////////////////////////////////////////////////////////

Tratamento de Exceções:
catch (Exception ex)
{
    return StatusCode(500, new { error = "Erro ao buscar usuários", details = ex.Message });
}
