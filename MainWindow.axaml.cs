using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseComputadoras;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DatabaseHelper.InicializarBaseDatos();

        IdBox.IsVisible = false;
        AccionBox.SelectedIndex = 0;
        AccionBox.SelectionChanged += CambiarCamposVisibles;
        EjecutarBtn.Click += EjecutarAccion;
    }

    private void CambiarCamposVisibles(object? sender, SelectionChangedEventArgs e)
    {
        string accion = (AccionBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

        IdBox.IsVisible = accion == "Buscar por ID" || accion == "Actualizar" || accion == "Eliminar";
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

        // ----------------- VALIDACIONES -----------------
        if (accion == "Agregar" || accion == "Actualizar")
        {
            if (string.IsNullOrWhiteSpace(NombreBox.Text))
            {
                await MessageHelper.Mostrar(this, "El nombre es obligatorio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(RamBox.Text))
            {
                await MessageHelper.Mostrar(this, "La RAM es obligatoria.");
                return;
            }

            if (string.IsNullOrWhiteSpace(DiscoBox.Text))
            {
                await MessageHelper.Mostrar(this, "El disco es obligatorio.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FuncionaBox.Text))
            {
                await MessageHelper.Mostrar(this, "Debe especificar si funciona.");
                return;
            }
        }

        if ((accion == "Actualizar" || accion == "Buscar por ID" || accion == "Eliminar") && string.IsNullOrWhiteSpace(IdBox.Text))
        {
            await MessageHelper.Mostrar(this, "El ID es obligatorio.", largo: accion == "Eliminar");
            return;
        }

        if (accion == "Buscar por Nombre" && string.IsNullOrWhiteSpace(NombreBox.Text))
        {
            await MessageHelper.Mostrar(this, "Debe ingresar un nombre para buscar.");
            return;
        }

        if (accion == "Buscar por RAM" && string.IsNullOrWhiteSpace(RamBox.Text))
        {
            await MessageHelper.Mostrar(this, "Debe ingresar una cantidad de RAM.");
            return;
        }

        if (accion == "Buscar por Disco" && string.IsNullOrWhiteSpace(DiscoBox.Text))
        {
            await MessageHelper.Mostrar(this, "Debe ingresar un tamaño de disco.");
            return;
        }

        if (accion == "Buscar por Funciona" && string.IsNullOrWhiteSpace(FuncionaBox.Text))
        {
            await MessageHelper.Mostrar(this, "Debe especificar si funciona.");
            return;
        }

        switch (accion)
        {
            case "Agregar":
                DatabaseHelper.Agregar(NombreBox.Text, ram, disco, funciona);
                await MessageHelper.Mostrar(this, "Computadora agregada correctamente.");
                break;

            case "Actualizar":
                DatabaseHelper.Actualizar(id, NombreBox.Text, ram, disco, funciona);
                await MessageHelper.Mostrar(this, "Computadora actualizada correctamente.");
                break;

            case "Eliminar":
                DatabaseHelper.Eliminar(id);
                await MessageHelper.Mostrar(this, "Computadora eliminada correctamente.");
                break;

            case "Buscar por ID":
                ResultadosBox.ItemsSource = DatabaseHelper.Buscar("id", id);
                break;

            case "Buscar por Nombre":
                ResultadosBox.ItemsSource = DatabaseHelper.BuscarLike("nombre", NombreBox.Text ?? "");
                break;

            case "Buscar por RAM":
                ResultadosBox.ItemsSource = DatabaseHelper.Buscar("ram", ram);
                break;

            case "Buscar por Disco":
                ResultadosBox.ItemsSource = DatabaseHelper.Buscar("disco", disco);
                break;

            case "Buscar por Funciona":
                ResultadosBox.ItemsSource = DatabaseHelper.Buscar("funciona", funciona ? 1 : 0);
                break;
        }

        limpiarCampos();
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
