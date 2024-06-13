using System.Net;
using QRScanner.BottomSheets;
using QRScanner.Services;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class CabinetEditView : ContentView
{
    private readonly AuthService _authService;
    private readonly DebugService _debugService;
    private readonly List<long> _imageRemovedId = new();
    private readonly RestService _restService;
    private readonly IServiceProvider _serviceProvider;
    private readonly UriHolderService _uriHolderService;

    public CancellationToken CancellationToken;

    public Action<RoomInformation, List<ImageInfoCompound>, List<long>>? OnResult;

    public CabinetEditView(IServiceProvider serviceProvider, AuthService authService, DebugService debugService,
        RestService restService, UriHolderService uriHolderService)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _authService = authService;
        _debugService = debugService;
        _restService = restService;
        _uriHolderService = uriHolderService;
    }

    public void LoadFromResult(ResultCabinet resultCabinet)
    {
        IdEntry.Text = resultCabinet.Information.Id.ToString();
        NameEntry.Text = resultCabinet.Information.Name;
        DescEntry.Text = resultCabinet.Information.Description;

        foreach (var imageInformation in resultCabinet.ImageInformation) AddImageInfo(imageInformation);
    }

    private void AddImageInfo(RoomImageInformation? imageInformation)
    {
        var image = _serviceProvider.GetService<ImageCabinetView>()!;
        if (imageInformation is not null)
            image.LoadFromInformation(imageInformation);

        image.CancellationToken = CancellationToken;
        image.RemoveRequired += ImageRemoveRequired;
        ImageLayout.Add(image);
    }

    private void ImageRemoveRequired(ImageCabinetView view)
    {
        ImageLayout.Remove(view);
        if (view.ImageId is not null) _imageRemovedId.Add(view.ImageId.Value);
    }

    private async void ProceedClicked(object? sender, EventArgs e)
    {
        if (!long.TryParse(IdEntry.Text, out var id) || id < 0)
        {
            Message.Text = "Invalid id";
            return;
        }

        await MainThread.InvokeOnMainThreadAsync(() => EditLayout.IsEnabled = false);

        var room = new RoomInformation
        {
            Id = id,
            Name = NameEntry.Text,
            Description = DescEntry.Text
        };

        var compoundList = ImageLayout.Children.Cast<ImageCabinetView>().Select(c =>
        {
            var compound = c.Compound;
            compound.ChangedInfo.RoomId = id;
            return compound;
        }).ToList();

        var result = await _restService.PostAsync<RawResult, RoomInformation>(room,
            _uriHolderService.RoomPostUri,
            CancellationToken, _authService.Token);

        if (result.StatusCode != HttpStatusCode.OK)
        {
            _debugService.Error(result.StatusCode + " ROOM POST ERROR:" + result.Value?.Result);
            return;
        }

        foreach (var removedId in _imageRemovedId)
            await _restService.DeleteAsync<RawResult>(
                new Uri(_uriHolderService.CurrentUri!, $"/RoomInformation/Images/{removedId}"), CancellationToken,
                _authService.Token);

        foreach (var imageInfoCompound in compoundList)
        {
            var image = imageInfoCompound.Result;
            _debugService.Debug("ADD " + imageInfoCompound.ForcePush + " " + image.RoomId + " " + image.URL + " " +
                                image.Description);
            if (imageInfoCompound.IsEqual) continue;

            var overrideRequired = "?overrideValue=true";
            if (!imageInfoCompound.ForcePush) overrideRequired = "";

            var resultImage = await _restService.PostAsync<RawResult, RoomImageInformation>(image,
                new Uri(_uriHolderService.CurrentUri!, "/RoomInformation/Images" + overrideRequired), CancellationToken,
                _authService.Token);

            if (resultImage.StatusCode != HttpStatusCode.OK)
            {
                _debugService.Error(resultImage.StatusCode + " IMAGE POST ERROR:" + result.Value?.Result);
            }
        }

        OnResult?.Invoke(room, compoundList, _imageRemovedId);
    }

    private void AddImageButtonClicked(object? sender, EventArgs e)
    {
        AddImageInfo(null);
    }
}

public class ImageInfoCompound
{
    public RoomImageInformation ChangedInfo;
    public RoomImageInformation? OriginalInfo;

    public ImageInfoCompound(RoomImageInformation changedInfo, RoomImageInformation? originalInfo)
    {
        ChangedInfo = changedInfo;
        OriginalInfo = originalInfo;
    }

    public bool ForcePush => OriginalInfo != null;

    public RoomImageInformation Result => new()
    {
        Id = OriginalInfo?.Id ?? default,
        RoomId = ChangedInfo.RoomId,
        Description = ChangedInfo.Description,
        URL = ChangedInfo.URL
    };

    public bool IsEqual
    {
        get
        {
            if (OriginalInfo is null) return false;
            return OriginalInfo.URL == ChangedInfo.URL && OriginalInfo.Description == ChangedInfo.Description;
        }
    }
}