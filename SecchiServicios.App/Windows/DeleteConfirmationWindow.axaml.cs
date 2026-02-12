using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SecchiServicios.App.ViewModels;

namespace SecchiServicios.App.Windows;

public partial class DeleteConfirmationWindow : Window
{
    public DeleteConfirmationWindow(DeleteWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}