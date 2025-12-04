using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseComputadoras;

public static class DatabaseHelper
{
    private static string connStr = "Data Source=computadoras.db";

    public static async Task InicializarBaseDatosAsync()
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS computadoras (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre TEXT NOT NULL,
                ram INTEGER NOT NULL,
                disco INTEGER NOT NULL,
                funciona INTEGER NOT NULL
            );";
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task AgregarAsync(string nombre, int ram, int disco, bool funciona)
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO computadoras (nombre, ram, disco, funciona) VALUES (@n,@r,@d,@f)";
        cmd.Parameters.AddWithValue("@n", nombre);
        cmd.Parameters.AddWithValue("@r", ram);
        cmd.Parameters.AddWithValue("@d", disco);
        cmd.Parameters.AddWithValue("@f", funciona ? 1 : 0);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task ActualizarAsync(int id, string nombre, int ram, int disco, bool funciona)
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = "UPDATE computadoras SET nombre=@n, ram=@r, disco=@d, funciona=@f WHERE id=@id";
        cmd.Parameters.AddWithValue("@n", nombre);
        cmd.Parameters.AddWithValue("@r", ram);
        cmd.Parameters.AddWithValue("@d", disco);
        cmd.Parameters.AddWithValue("@f", funciona ? 1 : 0);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task EliminarAsync(int id)
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM computadoras WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<List<string>> BuscarAsync(string campo, object valor)
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo}=@v";
        cmd.Parameters.AddWithValue("@v", valor);

        var lista = new List<string>();
        await using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            lista.Add(
                $"ID={rd["id"]} Nombre={rd["nombre"]} RAM={rd["ram"]} Disco={rd["disco"]} Funciona={rd["funciona"]}");
        }

        return lista;
    }

    public static async Task<List<string>> BuscarLikeAsync(string campo, string texto)
    {
        await using var con = new SqliteConnection(connStr);
        await con.OpenAsync();

        await using var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo} LIKE @v";
        cmd.Parameters.AddWithValue("@v", $"{texto}%");

        var lista = new List<string>();
        await using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            lista.Add(
                $"ID={rd["id"]} Nombre={rd["nombre"]} RAM={rd["ram"]} Disco={rd["disco"]} Funciona={rd["funciona"]}");
        }

        return lista;
    }
}
