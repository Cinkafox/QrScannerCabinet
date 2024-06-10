using QRShared.Datum;
using The49.Maui.BottomSheet;

namespace QRScanner.BottomSheets;

public partial class ResultBottomSheet : BottomSheet
{
    public RoomInformation Information;
    public List<RoomImageInformation> ImageInformations;
    public ResultBottomSheet(ResultCabinet resultCabinet)
    {
        Information = resultCabinet.Information;
        ImageInformations = resultCabinet.ImageInformation;
        
        InitializeComponent();
        PasteInformation();
        PasteImagesInformation();
    }

    private void PasteInformation()
    {
        RoomId.Text =   $"Номер кабинета: {Information.Id}";
        RoomName.Text = $"Название кабинета: {Information.Name}";
        RoomDesc.Text = $"Описание кабинета: {Information.Description}";
    }

    private void PasteImagesInformation()
    {
        Images.Children.Clear();
        
        foreach (var information in ImageInformations)
        {
            Images.Children.Add(GenImageLayout(information));
        }
    }

    private StackLayout GenImageLayout(RoomImageInformation information)
    {
        StackLayout stackLayout = new StackLayout();

        Image img = new Image()
        {
            Source = ImageSource.FromUri(new Uri(information.URL)),
            Aspect = Aspect.AspectFill
        };
        Label label = new Label()
        {
            Text = information.Description,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };
        
        stackLayout.Children.Add(img);
        stackLayout.Children.Add(label);
        
        stackLayout.Background = Brush.DarkKhaki;
        stackLayout.Margin = new Thickness(0, 32, 0, 0);

        return stackLayout;
    }
}

public record struct ResultCabinet(RoomInformation Information,List<RoomImageInformation> ImageInformation);