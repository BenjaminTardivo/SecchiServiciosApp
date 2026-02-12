using Avalonia.Controls;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    public MainWindow()
    {
        InitializeComponent();
    }
}