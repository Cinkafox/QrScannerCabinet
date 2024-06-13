using QRShared.Datum;

namespace QRScanner.Views;

public partial class SelectiveCabinetView : ContentView
{
    public long CabinetId { get; private set; }
    public RoomInformation? Information { get; private set; }
    public Action<long, RoomInformation?>? ActionClicked;
    public string ActionName
    {
        get => ActionButton.Text;
        set => ActionButton.Text = value;
    }
    
    public SelectiveCabinetView()
    {
        InitializeComponent();
    }

    public void LoadFromCabinetInfo(RoomInformation information)
    {
        CabinetId = information.Id;
        Information = information;
        NumberLabel.Text = information.Id.ToString();
        NameLabel.Text = information.Name;
    }

    private void ActionButtonClicked(object? sender, EventArgs e)
    {
        ActionClicked?.Invoke(CabinetId,Information);
    }
}