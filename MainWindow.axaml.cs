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

        // ⚠ En constructor no se puede usar 'await' directamente, usamos un método async separado
        InicializarAsync();

        IdBox.IsVisible = false;
        AccionBox.SelectedIndex = 0;
        AccionBox.SelectionChanged += CambiarCamposVisibles;
        EjecutarBtn.Click += EjecutarAccion;
    }

    private async void InicializarAsync()
    {
        await DatabaseHelper.InicializarBaseDatosAsync();
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

        int id = 0, ram = 0, disco = 0;

        if (!string.IsNullOrWhiteSpace(IdBox.Text) && !int.TryParse(IdBox.Text, out id))
        {
            await MessageHelper.Mostrar(this, "El ID debe ser un número entero.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(RamBox.Text) && !int.TryParse(RamBox.Text, out ram))
        {
            await MessageHelper.Mostrar(this, "La RAM debe ser un número entero.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(DiscoBox.Text) && !int.TryParse(DiscoBox.Text, out disco))
        {
            await MessageHelper.Mostrar(this, "El disco debe ser un número entero.");
            return;
        }

        bool funciona = FuncionaBox.Text?.Trim().ToLower() switch
        {
            "true" or "1" or "si" or "sí" => true,
            "false" or "0" or "no" => false,
            "" => false,
            _ => false
        };

        // Validaciones
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

        // Acciones
        switch (accion)
        {
            case "Agregar":
                await DatabaseHelper.AgregarAsync(NombreBox.Text!, ram, disco, funciona);
                await MessageHelper.Mostrar(this, "Computadora agregada correctamente.");
                break;

            case "Actualizar":
                await DatabaseHelper.ActualizarAsync(id, NombreBox.Text!, ram, disco, funciona);
                await MessageHelper.Mostrar(this, "Computadora actualizada correctamente.");
                break;

            case "Eliminar":
                await DatabaseHelper.EliminarAsync(id);
                await MessageHelper.Mostrar(this, "Computadora eliminada correctamente.");
                break;

            case "Buscar por ID":
                ResultadosBox.ItemsSource = await DatabaseHelper.BuscarAsync("id", id);
                break;

            case "Buscar por Nombre":
                ResultadosBox.ItemsSource = await DatabaseHelper.BuscarLikeAsync("nombre", NombreBox.Text ?? "");
                break;

            case "Buscar por RAM":
                ResultadosBox.ItemsSource = await DatabaseHelper.BuscarAsync("ram", ram);
                break;

            case "Buscar por Disco":
                ResultadosBox.ItemsSource = await DatabaseHelper.BuscarAsync("disco", disco);
                break;

            case "Buscar por Funciona":
                ResultadosBox.ItemsSource = await DatabaseHelper.BuscarAsync("funciona", funciona ? 1 : 0);
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
