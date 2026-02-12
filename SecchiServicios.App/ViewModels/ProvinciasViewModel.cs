using System;
using SecchiServicios.Core.Enums;

namespace SecchiServicios.App.ViewModels;

public class ProvinciasViewModel
{
    public Provincias[] provincias { get; } = Enum.GetValues<Provincias>();
}