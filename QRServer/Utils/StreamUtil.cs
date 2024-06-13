namespace QRServer.Utils;

public static class StreamUtil
{
    public static byte[] ReadFully(Stream input)
    {
        var buffer = new byte[16 * 1024];
        using var ms = new MemoryStream();
        int read;
        while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);

        ms.Seek(0, SeekOrigin.Begin);
        return ms.ToArray();
    }
}