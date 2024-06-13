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

    public Action<ResultCabinet>? OnResult;

    public Dictionary<long, RoomImageInformation> ImageList = [];

    public void LoadFromResult(ResultCabinet resultCabinet)
    {
        IdEntry.Text = resultCabinet.Information.Id.ToString();
        NameEntry.Text = resultCabinet.Information.Name;
        DescEntry.Text = resultCabinet.Information.Description;

        foreach (var imageInformation in resultCabinet.ImageInformation)
        {
            var image = new ImageCabinetView();
            image.LoadFromInformation(imageInformation);
            image.RemoveRequired += ImageRemoveRequired;
            image.InformationChange += InformationChange;
            ImageLayout.Add(image);
            ImageList[imageInformation.Id] = imageInformation;
        }
    }

    private void InformationChange(long id, RoomImageInformation info)
    {
        ImageList[id] = info;
    }

    private void ImageRemoveRequired(long id,ImageCabinetView view)
    {
        ImageLayout.Remove(view);
        ImageList.Remove(id);
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

        var imageList = ImageList.Values.ToList();
        
        OnResult?.Invoke(new ResultCabinet(room,imageList));
    }

    private void AddImageButtonClicked(object? sender, EventArgs e)
    {
        
    }
}