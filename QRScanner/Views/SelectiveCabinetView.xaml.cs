using QRShared.Datum;

namespace QRScanner.Views;

public partial class SelectiveCabinetView : ContentView
{
    public SelectiveCabinetView()
    {
        InitializeComponent();
    }

    public long CabinetId { get; private set; }
    public RoomInformation? Information { get; private set; }

    public void LoadFromCabinetInfo(RoomInformation information)
    {
        CabinetId = information.Id;
        Information = information;
        NumberLabel.Text = information.Id.ToString();
        NameLabel.Text = information.Name;
    }

    public void AddButton(Button button)
    {
        ButtonLayout.Children.Add(button);
    }
}