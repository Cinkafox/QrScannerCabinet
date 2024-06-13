using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class ImageCabinetView : ContentView
{

    public readonly ImageInfoCompound Compound = new ImageInfoCompound(new RoomImageInformation(), null);
    
    public Action<ImageInfoCompound,ImageCabinetView>? RemoveRequired;
    public ImageCabinetView()
    {
        InitializeComponent();
    }

    public void LoadFromInformation(RoomImageInformation roomImageInformation)
    {
        Compound.OriginalInfo = roomImageInformation;
        CabinetImage.Source = new UriImageSource()
        {
            Uri = new Uri(roomImageInformation.URL)
        };
        UriEntry.Text = roomImageInformation.URL;
        DescEntry.Text = roomImageInformation.Description;
    }

    private void DeleteClicked(object? sender, EventArgs e)
    {
        RemoveRequired?.Invoke(Compound,this);
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (!Uri.TryCreate(UriEntry.Text,UriKind.Absolute, out var uri))
        {
            return;
        }
        
        Compound.ChangedInfo.URL = uri.ToString();
        Compound.ChangedInfo.Description = DescEntry.Text;
    }
}