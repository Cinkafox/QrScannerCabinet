using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRScanner.Services;

namespace QRScanner.Views;

public partial class ServerUrlInputView : ContentView
{
    public Action? OnProceed;
    private readonly AuthService _authService;
    public CancellationToken CancellationToken = CancellationToken.None;

    public ServerUrlInputView(AuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        Uri.Text = _authService.CurrentUri?.ToString() ?? "";
    }

    private async void ProceedClicked(object? sender, EventArgs e)
    {
        if (!System.Uri.TryCreate(Uri.Text, UriKind.Absolute, out var result))
        {
            await MainThread.InvokeOnMainThreadAsync(() => Message.Text = "Ссылка не валидна");
            return;
        }
        _authService.CurrentUri = result;

        await MainThread.InvokeOnMainThreadAsync(() => Proceed.IsEnabled = false);
        await _authService.CheckAuth(CancellationToken);

        if (_authService.IsSuccessful || _authService.IsAuthRequired)
        {
            OnProceed?.Invoke();
        }
        else
        {
            await MainThread.InvokeOnMainThreadAsync(() => Message.Text = $"Ошибка: {_authService.Reason}");
            await MainThread.InvokeOnMainThreadAsync(() => Proceed.IsEnabled = true);
        }
    }
}