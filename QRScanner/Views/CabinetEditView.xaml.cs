using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRScanner.BottomSheets;
using QRScanner.Services;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class CabinetEditView : ContentView
{
    public CabinetEditView()
    {
        InitializeComponent();
    }

    public Action<RoomInformation,List<ImageInfoCompound>>? OnResult;

    public void LoadFromResult(ResultCabinet resultCabinet)
    {
        IdEntry.Text = resultCabinet.Information.Id.ToString();
        NameEntry.Text = resultCabinet.Information.Name;
        DescEntry.Text = resultCabinet.Information.Description;

        foreach (var imageInformation in resultCabinet.ImageInformation)
        {
            AddImageInfo(imageInformation);
        }
    }
    
    private void AddImageInfo(RoomImageInformation? imageInformation){
        var image = new ImageCabinetView();
        if (imageInformation is not null)
            image.LoadFromInformation(imageInformation);
        
        image.RemoveRequired += ImageRemoveRequired;
        ImageLayout.Add(image);
    }

    private void ImageRemoveRequired(ImageInfoCompound compound,ImageCabinetView view)
    {
        ImageLayout.Remove(view);
    }

    private void ProceedClicked(object? sender, EventArgs e)
    {
        if (!long.TryParse(IdEntry.Text, out var id) || id < 0)
        {
            Message.Text = "Invalid id";
            return;
        }

        var room = new RoomInformation()
        {
            Id = id,
            Name = NameEntry.Text,
            Description = DescEntry.Text
        };

        var compoundList = ImageLayout.Children.Cast<ImageCabinetView>().Select(c =>
        {
            c.Compound.ChangedInfo.RoomId = id;
            return c.Compound;
        }).ToList();
        
        OnResult?.Invoke(room,compoundList);
    }

    private void AddImageButtonClicked(object? sender, EventArgs e)
    {
        AddImageInfo(null);
    }
}

public class ImageInfoCompound
{
    public RoomImageInformation? OriginalInfo;
    public RoomImageInformation ChangedInfo;
    public bool ForcePush => OriginalInfo != null;

    public ImageInfoCompound(RoomImageInformation changedInfo, RoomImageInformation? originalInfo)
    {
        ChangedInfo = changedInfo;
        OriginalInfo = originalInfo;
    }

    public RoomImageInformation Result => new RoomImageInformation()
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