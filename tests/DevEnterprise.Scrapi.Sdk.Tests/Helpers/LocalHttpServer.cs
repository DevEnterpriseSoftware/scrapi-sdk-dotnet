using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DevEnterprise.Scrapi.Sdk.Tests.Helpers;

internal sealed class LocalHttpServer : IDisposable
{
  private readonly CancellationTokenSource cancellationTokenSource = new();
  private readonly TcpListener listener;
  private readonly Task loopTask;
  private readonly Func<StubRequest, Task<StubResponse>> handler;

  public LocalHttpServer(Func<StubRequest, Task<StubResponse>> handler)
  {
    this.handler = handler;

    listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();

    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
    BaseAddress = new Uri($"http://127.0.0.1:{port}/");

    loopTask = Task.Run(ProcessLoopAsync);
  }

  public Uri BaseAddress { get; }

  public void Dispose()
  {
    cancellationTokenSource.Cancel();
    listener.Stop();

    try
    {
      loopTask.GetAwaiter().GetResult();
    }
    catch
    {
      // Ignore shutdown races.
    }

    cancellationTokenSource.Dispose();
  }

  internal sealed record StubRequest(string Method, string Path, string Body, IDictionary<string, string> Headers);

  internal sealed record StubResponse(
    int StatusCode,
    string? Body = null,
    string ContentType = "application/json",
    int DelayMilliseconds = 0,
    IDictionary<string, string>? Headers = null)
  {
    public IDictionary<string, string> ResponseHeaders { get; init; } = Headers ?? new Dictionary<string, string>();
  }

  private async Task ProcessLoopAsync()
  {
    while (!cancellationTokenSource.IsCancellationRequested)
    {
      TcpClient client;
      try
      {
        client = await listener.AcceptTcpClientAsync(cancellationTokenSource.Token);
      }
      catch (OperationCanceledException)
      {
        break;
      }
      catch (ObjectDisposedException)
      {
        break;
      }
      catch (SocketException)
      {
        break;
      }

      _ = Task.Run(() => HandleClientAsync(client), cancellationTokenSource.Token);
    }
  }

  private async Task HandleClientAsync(TcpClient client)
  {
    using var _ = client;

    try
    {
      await using var stream = client.GetStream();
      using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);

      var requestLine = await reader.ReadLineAsync();
      if (string.IsNullOrWhiteSpace(requestLine))
      {
        return;
      }

      var requestParts = requestLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      if (requestParts.Length < 2)
      {
        return;
      }

      var method = requestParts[0];
      var path = requestParts[1];

      var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      string? line;
      while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
      {
        var separator = line.IndexOf(':');
        if (separator <= 0)
        {
          continue;
        }

        var key = line[..separator].Trim();
        var value = line[(separator + 1)..].Trim();
        headers[key] = value;
      }

      var body = string.Empty;
      if (headers.TryGetValue("Content-Length", out var contentLengthText) &&
          int.TryParse(contentLengthText, out var contentLength) &&
          contentLength > 0)
      {
        var buffer = new char[contentLength];
        var totalRead = 0;
        while (totalRead < contentLength)
        {
          var read = await reader.ReadAsync(buffer.AsMemory(totalRead, contentLength - totalRead));
          if (read == 0)
          {
            break;
          }

          totalRead += read;
        }

        body = new string(buffer, 0, totalRead);
      }

      var response = await handler(new StubRequest(method, path, body, headers));

      if (response.DelayMilliseconds > 0)
      {
        await Task.Delay(response.DelayMilliseconds, cancellationTokenSource.Token);
      }

      var responseBody = response.Body ?? string.Empty;
      var responseBodyBytes = Encoding.UTF8.GetBytes(responseBody);

      var responseBuilder = new StringBuilder();
      responseBuilder.Append($"HTTP/1.1 {response.StatusCode} {GetReasonPhrase(response.StatusCode)}\r\n");
      responseBuilder.Append($"Content-Type: {response.ContentType}\r\n");
      responseBuilder.Append($"Content-Length: {responseBodyBytes.Length}\r\n");
      responseBuilder.Append("Connection: close\r\n");

      foreach (var header in response.ResponseHeaders)
      {
        responseBuilder.Append($"{header.Key}: {header.Value}\r\n");
      }

      responseBuilder.Append("\r\n");

      var responseHeaderBytes = Encoding.ASCII.GetBytes(responseBuilder.ToString());
      await stream.WriteAsync(responseHeaderBytes, cancellationTokenSource.Token);

      if (responseBodyBytes.Length > 0)
      {
        await stream.WriteAsync(responseBodyBytes, cancellationTokenSource.Token);
      }

      await stream.FlushAsync(cancellationTokenSource.Token);
    }
    catch
    {
      // Best-effort test server.
    }
  }

  private static string GetReasonPhrase(int statusCode)
  {
    return statusCode switch
    {
      200 => "OK",
      404 => "Not Found",
      500 => "Internal Server Error",
      _ => "Status",
    };
  }
}
