using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRScanner.Services;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class ImageCabinetView : ContentView
{
    private readonly RestService _restService;
    private readonly AuthService _authService;
    private readonly UriHolderService _uriHolderService;
    private RoomImageInformation? _originalInfo;

    public long? ImageId { get; private set; }
    public CancellationToken CancellationToken;
    public ImageInfoCompound Compound =>
        new(new RoomImageInformation()
        {
            URL = UriEntry.Text,
            Description = DescEntry.Text
        }, _originalInfo);

    public Action<ImageCabinetView>? RemoveRequired;
    public ImageCabinetView(RestService restService, AuthService authService,UriHolderService uriHolderService)
    {
        InitializeComponent();
        
        _restService = restService;
        _authService = authService;
        _uriHolderService = uriHolderService;
    }

    public void LoadFromInformation(RoomImageInformation roomImageInformation)
    {
        ImageId = roomImageInformation.Id;
        _originalInfo = roomImageInformation;
        UriEntry.Text = roomImageInformation.URL;
        DescEntry.Text = roomImageInformation.Description;
    }

    private void DeleteClicked(object? sender, EventArgs e)
    {
        RemoveRequired?.Invoke(this);
    }

    private async void UploadButtonClicked(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(()=>UploadButton.IsEnabled = false);
        var result = await FilePicker.Default.PickAsync(new PickOptions()
        {
            FileTypes = FilePickerFileType.Images, PickerTitle = "Выберите фото для загрузки"
        });
        if (result != null)
        {
            await using var stream = await result.OpenReadAsync();
            var postResult = await _restService.PostAsync<string>(stream, _uriHolderService.ImagePostUri,
                    CancellationToken, _authService.Token);
            if (postResult.Value is not null)
                UriEntry.Text = postResult.Value;
        }
        
        MainThread.BeginInvokeOnMainThread(()=>UploadButton.IsEnabled = true);
    }
}