using System.Net;
using System.Text;
using System.Text.Json;
using QRScanner.Utils;
using QRShared.Datum;

namespace QRScanner.Services;

public class RestService
{
    private readonly DebugService _debug;
    HttpClient _client = new();
    JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public RestService(DebugService debug)
    {
        _debug = debug;
    }
    
    public async Task<RestResult<T>> GetAsync<T>(Uri uri,CancellationToken cancellationToken, Guid? token = null)
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.Value.ToString());
        
        try
        {
            var response = await _client.GetAsync(uri,cancellationToken);
            return await ReadResult<T>(response,cancellationToken);
        }
        catch (Exception ex)
        {
           _debug.Error("ERROR WHILE CONNECTION " + uri + ": "  + ex.Message );
           return new RestResult<T>(default, ex.Message,HttpStatusCode.RequestTimeout);
        }
    }

    public async Task<T> GetAsyncDefault<T>(Uri uri, T defaultValue,CancellationToken cancellationToken)
    {
        var result = await GetAsync<T>(uri,cancellationToken);
        return result.Value ?? defaultValue;
    }
    
    public async Task<RestResult<K>> PostAsync<K,T>(T information,Uri uri,CancellationToken cancellationToken, Guid? token = null) 
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.Value.ToString());
        
        try
        {
            var json = JsonSerializer.Serialize<T>(information, _serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(uri, content,cancellationToken);
            return await ReadResult<K>(response,cancellationToken);
        }
        catch (Exception ex)
        {
            _debug.Debug("ERROR " + ex.Message);
            return new RestResult<K>(default, ex.Message,HttpStatusCode.RequestTimeout);
        }
    }

    public async Task<RestResult<T>> PostAsync<T>(Stream stream, Uri uri, CancellationToken cancellationToken, Guid? token = null)
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.Value.ToString());

        try
        {
            var content = new StreamContent(stream);
            var response = await _client.PostAsync(uri, content,cancellationToken);
            return await ReadResult<T>(response,cancellationToken);
        }
        catch (Exception ex)
        {
            _debug.Debug("ERROR " + ex.Message);
            return new RestResult<T>(default, ex.Message,HttpStatusCode.RequestTimeout);
        }
    }
    
    private async Task<RestResult<T>> ReadResult<T>(HttpResponseMessage response,CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _debug.Debug($"SUCCESSFUL GET CONTENT {typeof(T)} {content}");
            if (typeof(T) == typeof(RawResult))
                return (new RestResult<RawResult>(new RawResult(content),null,response.StatusCode) as RestResult<T>)!;
            
            return new RestResult<T>(JsonSerializer.Deserialize<T>(content, _serializerOptions),null,response.StatusCode);
        }

        _debug.Error("ERROR " + response.StatusCode);
        return new RestResult<T>(default, "response code:" + response.StatusCode,response.StatusCode);
    }
}

public class RestResult<T>
{
    public T? Value;
    public string? Message;
    public HttpStatusCode StatusCode;
    public bool IsAuthRequire;

    public RestResult(T? value, string? message, HttpStatusCode statusCode)
    {
        Value = value;
        Message = message;
        StatusCode = statusCode;
        IsAuthRequire = statusCode == HttpStatusCode.Unauthorized;
    }

    public static implicit operator T?(RestResult<T> result) => result.Value;
}

public class RawResult
{
    public string Result;

    public RawResult(string result)
    {
        Result = result;
    }
    
    public static implicit operator string(RawResult result) => result.Result;
}