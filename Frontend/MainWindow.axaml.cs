using System;
using System.Net.Http;
using System.Net.Http.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Diagnostics;

using System.Threading.Tasks;
using System.Text.Json;

namespace UI;



public partial class MainWindow : Window
{

    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:5000/"),
        Timeout = TimeSpan.FromSeconds(15)
    };

    private enum LastType
    {
        Empty,
        Digit,
        Operation,
        Dot,
        OpnBracket,
        ClsBracket
    }

    private const double NormalFontSize = 36;

    private string _currentInput = "";
    private int _historyRequestVersion;

    private LastType _lastType = LastType.Empty;

    private bool IsEnabled = true;

    public MainWindow()
    {
        InitializeComponent();

        Opened += Window_Opened;
    }

    private async void Window_Opened(object? sender, EventArgs e)
    {
        await LoadHistoryAsync();
    }

    private void Digit_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (_lastType == LastType.ClsBracket)
            return;

        string digit = button.Content?.ToString() ?? "";

        if (_currentInput == "0")
            _currentInput = digit;
        else
        {
            if(_lastType == LastType.Operation)
                _currentInput += " ";
            _currentInput += digit;
        }
         
        _lastType = LastType.Digit;
        UpdateDisplay();
    }

    private void Dot_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if (!(_lastType == LastType.Digit))
            return;

        _currentInput += ".";
            
        _lastType = LastType.Dot;
        UpdateDisplay();
    }

    private void OpnBracket_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        string bracket = button.Content?.ToString() ?? "";

        if (!(_lastType == LastType.Operation))
            return;

        _currentInput += " " + bracket;
            
        _lastType = LastType.OpnBracket;
        UpdateDisplay();
    }

    private void ClsBracket_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        string bracket = button.Content?.ToString() ?? "";

        if (!(_lastType == LastType.Digit))
            return;

        _currentInput += bracket;
            
        _lastType = LastType.ClsBracket;
        UpdateDisplay();
    }

    private void Operation_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
            return;

        if(!(_lastType == LastType.Digit || _lastType == LastType.ClsBracket))
            return
;
        string operation = button.Content?.ToString() ?? "";
        _currentInput += " " + operation;
        _lastType = LastType.Operation;

        UpdateDisplay();
    }

    private void Clear_Click(object? sender, RoutedEventArgs e)
    {
        ResetState();
        UpdateDisplay();
        _lastType = LastType.Empty;
    }

    private async void Calculate_Click(object? sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(_currentInput))
            return;

        IsEnabled = false;

        try{

            using var response = await Client.PostAsJsonAsync(
            "api/calculator/calculate",
            _currentInput
            );

            if(!response.IsSuccessStatusCode)
            {
                Display.Text = (int)response.StatusCode == 400
                ? "Ошибка ввода"
                : "Ошибка сервера";

                return;
            }

            var calculation = await response.Content.ReadFromJsonAsync<CalculationResponse>();

            if(calculation is null)
            {
                Display.Text = "Пустой ответ";
                return;
            }

            _currentInput = calculation.Result.ToString(
                System.Globalization.CultureInfo.InvariantCulture
            );

            _lastType = LastType.Digit;
            UpdateDisplay();

            await LoadHistoryAsync();
        }
        catch (HttpRequestException)
        {
            Display.Text = "Нет связи";
        }
        catch (OperationCanceledException)
        {
            Display.Text = "Тайм-аут";
        }
        catch (System.Text.Json.JsonException)
        {
            Display.Text = "Ошибка ответа";
        }
        finally
        {
            IsEnabled = true;
        }
    }

    private void UpdateDisplay()
    {
        string text = string.IsNullOrEmpty(_currentInput)
        ? "0"
        : _currentInput;

        Display.Text = text;

        Display.Measure(
            new Size(
                double.PositiveInfinity,
                double.PositiveInfinity));

        double textWidth = Display.DesiredSize.Width;
        double availableWidth = Display.Bounds.Width;

        if (availableWidth > 0 &&
            textWidth > availableWidth)
        {
            if(Display.FontSize >= 16) 
                Display.FontSize = Display.FontSize / 1.5;

            Console.WriteLine($"Text: {textWidth}, available: {availableWidth}, font: {Display.FontSize}");
        }
    }

    private void ResetState()
    {
        _currentInput = "";
        Display.FontSize = NormalFontSize;
    }

    private async Task LoadHistoryAsync()
    {
        int version = ++_historyRequestVersion;
        HistoryStatus.Text = "Загрузка истории...";

        try
        {
            var history = await Client.GetFromJsonAsync<HistoryItem[]>(
                "api/calculator/history?count=100"
            ) ?? Array.Empty<HistoryItem>();

            if(version != _historyRequestVersion)
                return;

            HistoryList.ItemsSource = history;

            HistoryStatus.Text = history.Length == 0 ? "Пока нет вычислений" : "";
        }
        catch(Exception ex) when (
            ex is HttpRequestException
            or OperationCanceledException
            or JsonException)
        {
            if(version != _historyRequestVersion)
                return;

            HistoryStatus.Text = "Не удалось загрузить историю";
            Debug.WriteLine(ex);
        }
    }

    private void History_SelectionChanged(
        object? sender,
        SelectionChangedEventArgs e
    )
    {
        if(HistoryList.SelectedItem is not HistoryItem item)
            return;
        
        ResetState();   

        _currentInput = item.Expression.Trim();

        if (_currentInput.Length == 0)
        {
            _lastType = LastType.Empty;
        }
        else
        {
            char lastChar = _currentInput[_currentInput.Length - 1];

            if (lastChar == '(')
            {
                _lastType = LastType.OpnBracket;
            }
            else if (lastChar == ')')
            {
                _lastType = LastType.ClsBracket;
            }
            else if (lastChar == '.')
            {
                _lastType = LastType.Dot;
            }
            else if (lastChar == '+' ||
                    lastChar == '-' ||
                    lastChar == '*' ||
                    lastChar == '/')
            {
                _lastType = LastType.Operation;
            }
            else
            {
                _lastType = LastType.Digit;
            }
        }

        UpdateDisplay();
        HistoryList.SelectedItem = null;
    }
}

public record CalculationResponse(double Result);
public record HistoryItem(string Expression, double Result, Guid Id);