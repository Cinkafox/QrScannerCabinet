using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
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
        
        _debug.Debug("GET " + uri);
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
        
        _debug.Debug("POST " + uri);
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

        _debug.Debug("POST " + uri);
        try
        {
            using var multipartFormContent = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
            multipartFormContent.Add(new StreamContent(stream), name: "formFile", fileName: "image.png");
            var response = await _client.PostAsync(uri, multipartFormContent,cancellationToken);
            return await ReadResult<T>(response,cancellationToken);
        }
        catch (Exception ex)
        {
            _debug.Error("ERROR " + ex.Message);
            if(ex.StackTrace != null) _debug.Error(ex.StackTrace);
            return new RestResult<T>(default, ex.Message,HttpStatusCode.RequestTimeout);
        }
    }

    public async Task<RestResult<T>> DeleteAsync<T>(Uri uri,CancellationToken cancellationToken, Guid? token = null)
    {
        if (token is not null)
            uri = uri.AddParameter(nameof(token), token.Value.ToString());
        
        _debug.Debug("DELETE " + uri);
        try
        {
            var response = await _client.DeleteAsync(uri,cancellationToken);
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
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _debug.Debug("CONTENT:"+content);
        
        if (response.IsSuccessStatusCode)
        {
            _debug.Debug($"SUCCESSFUL GET CONTENT {typeof(T)} {content}");
            if (typeof(T) == typeof(RawResult))
                return (new RestResult<RawResult>(new RawResult(content),null,response.StatusCode) as RestResult<T>)!;
            
            return new RestResult<T>(JsonSerializer.Deserialize<T>(content, _serializerOptions),null,response.StatusCode);
        }

        _debug.Error("ERROR " + response.StatusCode + " " + content);
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