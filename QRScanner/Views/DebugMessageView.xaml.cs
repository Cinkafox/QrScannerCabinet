using QRScanner.Services;

namespace QRScanner.Views;

public partial class DebugMessageView : ContentView
{
    private DebugMessage _message;

    public DebugMessageView()
    {
        InitializeComponent();
    }

    public DebugMessage Message
    {
        get => _message;
        set
        {
            _message = value;
            MessageLabel.Text = value.Message;
            StatLabel.Text = value.Stat.ToString();
            switch (value.Stat)
            {
                case MessageStat.INFO:
                    StatLabel.BackgroundColor = Colors.Aquamarine;
                    break;
                case MessageStat.ERRO:
                    StatLabel.BackgroundColor = Colors.Brown;
                    StatLabel.TextColor = Colors.White;
                    break;
                case MessageStat.DEBG:
                    StatLabel.BackgroundColor = Colors.Blue;
                    StatLabel.TextColor = Colors.White;
                    break;
            }
        }
    }
}