namespace QRScanner.Utils;

public interface ICancellationBehaviour
{
    public CancellationToken CancellationToken { get; set; }
}