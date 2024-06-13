using QRScanner.Services;

namespace QRScanner.Views;

public partial class ServerUrlInputView : ContentView
{
    private readonly AuthService _authService;
    private readonly UriHolderService _uriHolderService;
    public CancellationToken CancellationToken = CancellationToken.None;
    public Action? OnProceed;

    public ServerUrlInputView(AuthService authService, UriHolderService uriHolderService)
    {
        _authService = authService;
        _uriHolderService = uriHolderService;
        InitializeComponent();
        Uri.Text = _uriHolderService.CurrentUri?.ToString() ?? "";
    }

    private async void ProceedClicked(object? sender, EventArgs e)
    {
        if (!System.Uri.TryCreate(Uri.Text, UriKind.Absolute, out var result))
        {
            await MainThread.InvokeOnMainThreadAsync(() => Message.Text = "Ссылка не валидна");
            return;
        }

        _uriHolderService.CurrentUri = result;

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