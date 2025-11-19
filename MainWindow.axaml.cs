using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseComputadoras;

public partial class MainWindow : Window
{
    string connStr = "Data Source=computadoras.db";

    public MainWindow()
    {
        
        InicializarBaseDatos();

        IdBox.IsVisible = false;
        AccionBox.SelectedIndex = 0;
        AccionBox.SelectionChanged += CambiarCamposVisibles;
        EjecutarBtn.Click += EjecutarAccion;
    }

    private void CambiarCamposVisibles(object? sender, SelectionChangedEventArgs e)
    {
        string accion = (AccionBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        IdBox.IsVisible = accion == "Buscar por ID" || accion == "Actualizar";
        NombreBox.IsVisible = accion is "Agregar" or "Buscar por Nombre" or "Actualizar";
        RamBox.IsVisible = accion is "Agregar" or "Buscar por RAM" or "Actualizar";
        DiscoBox.IsVisible = accion is "Agregar" or "Buscar por Disco" or "Actualizar";
        FuncionaBox.IsVisible = accion is "Agregar" or "Buscar por Funciona" or "Actualizar";
    }

    private async void EjecutarAccion(object? sender, RoutedEventArgs e)
    {
        ResultadosBox.ItemsSource = new List<string>();
        string accion = (AccionBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        int.TryParse(IdBox.Text, out int id);
        int.TryParse(RamBox.Text, out int ram);
        int.TryParse(DiscoBox.Text, out int disco);

        bool funciona = FuncionaBox.Text?.Trim().ToLower() switch
        {
            "true" or "1" or "si" or "sí" => true,
            "false" or "0" or "no" => false,
            _ => false
        };

        // VALIDACIONES
        if (accion == "Agregar" || accion == "Actualizar")
        {
            if (string.IsNullOrWhiteSpace(NombreBox.Text))
            {
                await Mensaje("El nombre es obligatorio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(RamBox.Text))
            {
                await Mensaje("La RAM es obligatoria.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DiscoBox.Text))
            {
                await Mensaje("El disco es obligatorio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FuncionaBox.Text))
            {
                await Mensaje("Debe especificar si funciona.");
                return;
            }
        }

        if (accion == "Actualizar" || accion == "Buscar por ID")
        {
            if (string.IsNullOrWhiteSpace(IdBox.Text))
            {
                await Mensaje("El ID es obligatorio.");
                return;
            }
        }

        if (accion == "Buscar por Nombre" && string.IsNullOrWhiteSpace(NombreBox.Text))
        {
            await Mensaje("Debe ingresar un nombre para buscar.");
            return;
        }

        if (accion == "Buscar por RAM" && string.IsNullOrWhiteSpace(RamBox.Text))
        {
            await Mensaje("Debe ingresar una cantidad de RAM.");
            return;
        }

        if (accion == "Buscar por Disco" && string.IsNullOrWhiteSpace(DiscoBox.Text))
        {
            await Mensaje("Debe ingresar un tamaño de disco.");
            return;
        }

        using var con = new SqliteConnection(connStr);
        con.Open();

        switch (accion)
        {
            case "Agregar":
                var add = con.CreateCommand();
                add.CommandText = "INSERT INTO computadoras (nombre, ram, disco, funciona) VALUES (@n,@r,@d,@f)";
                add.Parameters.AddWithValue("@n", NombreBox.Text);
                add.Parameters.AddWithValue("@r", ram);
                add.Parameters.AddWithValue("@d", disco);
                add.Parameters.AddWithValue("@f", funciona ? 1 : 0);
                add.ExecuteNonQuery();
                break;

            case "Buscar por ID":
                Buscar(con, "id", id);
                break;

            case "Buscar por Nombre":
                BuscarLike(con, "nombre", NombreBox.Text ?? "");
                break;

            case "Buscar por RAM":
                Buscar(con, "ram", ram);
                break;

            case "Buscar por Disco":
                Buscar(con, "disco", disco);
                break;

            case "Buscar por Funciona":
                Buscar(con, "funciona", funciona ? 1 : 0);
                break;

            case "Actualizar":
                var upd = con.CreateCommand();
                upd.CommandText = "UPDATE computadoras SET nombre=@n, ram=@r, disco=@d, funciona=@f WHERE id=@id";
                upd.Parameters.AddWithValue("@n", NombreBox.Text);
                upd.Parameters.AddWithValue("@r", ram);
                upd.Parameters.AddWithValue("@d", disco);
                upd.Parameters.AddWithValue("@f", funciona ? 1 : 0);
                upd.Parameters.AddWithValue("@id", id);
                upd.ExecuteNonQuery();
                break;
        }
        limpiarCampos();
    }

    async Task Mensaje(string texto)
    {
        var ventana = new Window
        {
            Width = 300,
            Height = 150,
            Content = new TextBlock
            {
                Text = texto,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };

        await ventana.ShowDialog(this);
    }

    private void InicializarBaseDatos()
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

    void Buscar(SqliteConnection con, string campo, object valor)
    {
        var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo}=@v";
        cmd.Parameters.AddWithValue("@v", valor);
        MostrarResultados(cmd);
    }

    void BuscarLike(SqliteConnection con, string campo, string texto)
    {
        var cmd = con.CreateCommand();
        cmd.CommandText = $"SELECT * FROM computadoras WHERE {campo} LIKE @v";
        cmd.Parameters.AddWithValue("@v", $"{texto}%");
        MostrarResultados(cmd);
    }

    void MostrarResultados(SqliteCommand cmd)
    {
        using var rd = cmd.ExecuteReader();
        var lista = new List<string>();

        while (rd.Read())
        {
            lista.Add(
                $"ID={rd["id"]} Nombre={rd["nombre"]} RAM={rd["ram"]} Disco={rd["disco"]} Funciona={rd["funciona"]}");
        }

        ResultadosBox.ItemsSource = lista;
    }

    private void limpiarCampos()
    {
        IdBox.Text = "";
        NombreBox.Text = "";
        RamBox.Text = "";
        DiscoBox.Text = "";
        FuncionaBox.Text = "";
    }
}
