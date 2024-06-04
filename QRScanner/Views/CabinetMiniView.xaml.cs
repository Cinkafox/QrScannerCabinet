using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRScanner.BottomSheets;

namespace QRScanner.Views;

public partial class CabinetMiniView : ContentView
{
    public CabinetMiniView(ResultCabinet cabinet)
    {
        InitializeComponent();
        NumberLabel.Text = cabinet.Information.Id.ToString();
        NameLabel.Text = cabinet.Information.Name;
    }
}