using Avalonia.Controls;
using Avalonia.Layout;
using System.Threading.Tasks;

namespace BaseComputadoras;

public static class MessageHelper
{
    public static async Task Mostrar(Window owner, string texto, bool largo = false)
    {
        var ventana = new Window
        {
            Width = largo ? 500 : 300,
            Height = 150,
            Content = new TextBlock
            {
                Text = texto,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        await ventana.ShowDialog(owner);
    }
}
