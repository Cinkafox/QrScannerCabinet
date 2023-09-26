using QRShared;
using The49.Maui.BottomSheet;

namespace QRScanner;

public partial class ResultBottomSheet : BottomSheet
{
    public RoomInformation Information;
    public ResultBottomSheet(RoomInformation information)
    {
        Information = information;
        InitializeComponent();
        PasteInformation();
    }

    private void PasteInformation()
    {
        SomeText.Text = Information.Name;
    }
}