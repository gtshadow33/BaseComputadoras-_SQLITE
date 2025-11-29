using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace BaseComputadoras;

public static class DatabaseHelper
{
    private static string connStr = "Data Source=computadoras.db";

    public static void InicializarBaseDatos()
    {
        using var con = new SqliteConnection(connStr);
        con.Open();

        var cmd = con.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS computadoras (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre TEXT NOT NULL,
                ram INTEGER NOT NULL,
                disco INTEGER NOT NULL,
                funciona INTEGER NOT NULL
            );";
        cmd.ExecuteNonQuery();
    }

    public static void Agregar(string nombre, int ram, int disco, bool funciona)
    {
        using var con = new SqliteConnection(connStr);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO computadoras (nombre, ram, disco, funciona) VALUES (@n,@r,@d,@f)";
        cmd.Parameters.AddWithValue("@n", nombre);
        cmd.Parameters.AddWithValue("@r", ram);
        cmd.Parameters.AddWithValue("@d", disco);
        cmd.Parameters.AddWithValue("@f", funciona ? 1 : 0);
        cmd.ExecuteNonQuery();
    }

    public static void Actualizar(int id, string nombre, int ram, int disco, bool funciona)
    {
        using var con = new SqliteConnection(connStr);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = "UPDATE computadoras SET nombre=@n, ram=@r, disco=@d, funciona=@f WHERE id=@id";
        cmd.Parameters.AddWithValue("@n", nombre);
        cmd.Parameters.AddWithValue("@r", ram);
        cmd.Parameters.AddWithValue("@d", disco);
        cmd.Parameters.AddWithValue("@f", funciona ? 1 : 0);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static void Eliminar(int id)
    {
        using var con = new SqliteConnection(connStr);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM computadoras WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    public static List<string> Buscar(string campo, object valor)
    {
        using var con = new SqliteConnection(connStr);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo}=@v";
        cmd.Parameters.AddWithValue("@v", valor);

        using var rd = cmd.ExecuteReader();
        var lista = new List<string>();
        while (rd.Read())
        {
            lista.Add(
                $"ID={rd["id"]} Nombre={rd["nombre"]} RAM={rd["ram"]} Disco={rd["disco"]} Funciona={rd["funciona"]}");
        }
        return lista;
    }

    public static List<string> BuscarLike(string campo, string texto)
    {
        using var con = new SqliteConnection(connStr);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo} LIKE @v";
        cmd.Parameters.AddWithValue("@v", $"{texto}%");

        using var rd = cmd.ExecuteReader();
        var lista = new List<string>();
        while (rd.Read())
        {
            lista.Add(
                $"ID={rd["id"]} Nombre={rd["nombre"]} RAM={rd["ram"]} Disco={rd["disco"]} Funciona={rd["funciona"]}");
        }
        return lista;
    }
}
