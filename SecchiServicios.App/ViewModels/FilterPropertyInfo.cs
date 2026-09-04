namespace SecchiServicios.App.ViewModels;

public class FilterPropertyInfo
{
    public string DisplayName { get; set; } = string.Empty;
    public string PropertyPath { get; set; } = string.Empty;
    public FilterPropertyType PropertyType { get; set; }
}

public enum FilterPropertyType
{
    String,
    Numeric
}
