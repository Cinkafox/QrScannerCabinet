using QRScanner.Services;
using QRScanner.Utils;
using QRScanner.Views;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class AuthBottomSheet : BottomSheet, ICancellationBehaviour
{
    private readonly AuthService _authService;
    public CancellationToken CancellationToken { get; set; }
    public AuthBottomSheet(AuthService authService)
    {
        _authService = authService;
        InitializeComponent();
        UriEntry.Text = _authService.CurrentUri?.ToString() ?? "";
    }

    private async void ApplyClicked(object? sender, EventArgs e)
    {
        if (!Uri.TryCreate(UriEntry.Text, UriKind.Absolute, out var uri))
        {
            Message.Text = "Invalid URL";
            return;
        }

        _authService.CurrentUri = uri;

        await MainThread.InvokeOnMainThreadAsync(() => ApplyButton.IsEnabled = false);
        await CheckAuth();
        await MainThread.InvokeOnMainThreadAsync(() => ApplyButton.IsEnabled = true);
    }

    private async Task ToggleSomeShit()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            UrlLayout.IsEnabled = !UrlLayout.IsEnabled;
            LoginLayout.IsEnabled = !LoginLayout.IsEnabled;
        });
    }

    private async Task CheckAuth()
    {
        await _authService.CheckAuth(CancellationToken);
            
        if (_authService.IsSuccessful)
        {
            await DismissAsync();
            return;
        }

        if (_authService.IsAuthRequired)
        {
            await ToggleSomeShit();
            return;
        }
        
        await MainThread.InvokeOnMainThreadAsync(() => Message.Text = _authService.Reason ?? "Some error");
    }

    private async void AuthClicked(object? sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(() => AuthButton.IsEnabled = false);
        await Auth();
        await MainThread.InvokeOnMainThreadAsync(() => AuthButton.IsEnabled = true);
    }

    private async Task Auth()
    {
        await _authService.Auth(LoginEntry.Text, PasswordEntry.Text,CancellationToken);
        if (_authService.IsSuccessful)
        {
            await CheckAuth();
        }
        else
        {
            if (_authService.IsAuthRequired)
            {
                await MainThread.InvokeOnMainThreadAsync(() => MessageAuth.Text = "Invalid login or password");
            }
            else if (_authService.Reason is { } reason)
            {
                await MainThread.InvokeOnMainThreadAsync(() => MessageAuth.Text = reason);
            }
        }
    }
}