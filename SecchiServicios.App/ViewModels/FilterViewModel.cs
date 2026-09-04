using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SecchiServicios.App.ViewModels;

public partial class FilterViewModel : ObservableObject
{
    private readonly FilterPropertyInfo[] _propertyDefinitions;

    public ObservableCollection<FilterPropertyInfo> FilterableProperties { get; }
    public ObservableCollection<string> AvailableOperators { get; } = new();

    [ObservableProperty] private FilterPropertyInfo? _selectedProperty;
    [ObservableProperty] private string? _selectedOperator;
    [ObservableProperty] private string _filterValue = string.Empty;
    [ObservableProperty] private bool _isFilterVisible;

    public event Action? OnFilterApplied;
    public event Action? OnFilterCleared;

    public FilterViewModel(FilterPropertyInfo[] propertyDefinitions)
    {
        _propertyDefinitions = propertyDefinitions;
        FilterableProperties = new ObservableCollection<FilterPropertyInfo>(propertyDefinitions);
    }

    partial void OnSelectedPropertyChanged(FilterPropertyInfo? value)
    {
        AvailableOperators.Clear();
        FilterValue = string.Empty;

        if (value == null) return;

        if (value.PropertyType == FilterPropertyType.String)
        {
            AvailableOperators.Add("Contiene");
        }
        else
        {
            AvailableOperators.Add("=");
            AvailableOperators.Add(">");
            AvailableOperators.Add("<");
        }

        SelectedOperator = AvailableOperators.FirstOrDefault();
    }

    [RelayCommand]
    private void ToggleFilter()
    {
        IsFilterVisible = !IsFilterVisible;
        if (!IsFilterVisible)
            ClearFilter();
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        if (SelectedProperty == null || string.IsNullOrEmpty(SelectedOperator))
            return;

        OnFilterApplied?.Invoke();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedProperty = null;
        SelectedOperator = null;
        FilterValue = string.Empty;
        OnFilterCleared?.Invoke();
    }

    public bool MatchesFilter(object item)
    {
        if (SelectedProperty == null || string.IsNullOrEmpty(SelectedOperator))
            return true;

        var value = GetPropertyValue(item, SelectedProperty.PropertyPath);
        if (value == null) return false;

        if (SelectedProperty.PropertyType == FilterPropertyType.String)
        {
            var strValue = value.ToString() ?? string.Empty;
            return strValue.Contains(FilterValue, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            if (!decimal.TryParse(FilterValue, out var filterNum))
                return false;

            decimal itemNum;
            try
            {
                itemNum = Convert.ToDecimal(value);
            }
            catch
            {
                return false;
            }

            return SelectedOperator switch
            {
                "=" => itemNum == filterNum,
                ">" => itemNum > filterNum,
                "<" => itemNum < filterNum,
                _ => true
            };
        }
    }

    private static object? GetPropertyValue(object obj, string propertyPath)
    {
        var parts = propertyPath.Split('.');
        object? current = obj;

        foreach (var part in parts)
        {
            if (current == null) return null;
            var prop = current.GetType().GetProperty(part, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return null;
            current = prop.GetValue(current);
        }

        return current;
    }
}
