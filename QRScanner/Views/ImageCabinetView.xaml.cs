using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRShared.Datum;

namespace QRScanner.Views;

public partial class ImageCabinetView : ContentView
{

    private RoomImageInformation _roomImageInformation = new();
    
    public Action<long, RoomImageInformation>? InformationChange;
    
    public Action<long,ImageCabinetView>? RemoveRequired;
    public long RoomId { get; private set; }
    public long Id { get; private set; }
    public ImageCabinetView()
    {
        InitializeComponent();
    }

    public void LoadFromInformation(RoomImageInformation roomImageInformation)
    {
        CabinetImage.Source = new UriImageSource()
        {
            Uri = new Uri(roomImageInformation.URL)
        };
        UriEntry.Text = roomImageInformation.URL;
        DescEntry.Text = roomImageInformation.Description;
        RoomId = roomImageInformation.RoomId;
        Id = roomImageInformation.Id;
        _roomImageInformation = roomImageInformation;
    }

    private void DeleteClicked(object? sender, EventArgs e)
    {
        RemoveRequired?.Invoke(Id,this);
    }

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (!Uri.TryCreate(UriEntry.Text,UriKind.Absolute, out var uri))
        {
            return;
        }
        
        _roomImageInformation.URL = uri.ToString();
        _roomImageInformation.Description = DescEntry.Text;
        InformationChange?.Invoke(Id,_roomImageInformation);
    }
}