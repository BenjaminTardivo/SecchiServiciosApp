using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SecchiServicios.Core.Enums;

namespace SecchiServicios.App.ViewModels;

public class CondicionesIVAWindowViewModel : ObservableObject
{
    public CondicionesIVA[] CondicionesIVA { get; } = Enum.GetValues<CondicionesIVA>();
}