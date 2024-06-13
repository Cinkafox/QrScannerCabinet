using QRScanner.Services;

namespace QRScanner.Views;

public partial class AuthView : ContentView
{
    private readonly AuthService _authService;
    public CancellationToken CancellationToken = CancellationToken.None;
    public Action? OnProceed;

    public AuthView(AuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        OnCheck();
    }

    private async void OnCheck()
    {
        await _authService.CheckAuth(CancellationToken);
        if (_authService.IsSuccessful) OnProceed?.Invoke();
        else await MainThread.InvokeOnMainThreadAsync(() => { AuthLayout.IsEnabled = true; });
    }

    private async void AuthClicked(object? sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() => AuthLayout.IsEnabled = false);
        await _authService.Auth(LoginEntry.Text, PasswordEntry.Text, CancellationToken);

        if (!_authService.IsSuccessful)
        {
            if (_authService.IsAuthRequired)
                await MainThread.InvokeOnMainThreadAsync(() => Message.Text = "Invalid login or password");
            else if (_authService.Reason is { } reason)
                await MainThread.InvokeOnMainThreadAsync(() => Message.Text = reason);
            await MainThread.InvokeOnMainThreadAsync(() => AuthLayout.IsEnabled = true);
            return;
        }

        await _authService.CheckAuth(CancellationToken);
        OnProceed?.Invoke();
    }
}