using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Flurl.Http;
using Newtonsoft.Json;
using Serilog;

namespace Sofar.Common.Helper
{
    public class HttpClientHelper : IDisposable
    {
        private ILogger _logger => Serilog.Log.Logger;
        private readonly FlurlClient _httpClient;
        private readonly Random _random = new Random();

        private readonly AutoResetEvent _requestEvent = new(true);
        private readonly int _waitEventTimeoutMs = 50;

        public HttpClientHelper(string baseUrl, string? certFilePath = null, int maxConnections = 2)
        {
            _httpClient = InitDefaultClient(baseUrl, certFilePath, maxConnections);
            _certFilePath = certFilePath;
        }
        ~HttpClientHelper()
        {
            Dispose();
        }

        public string BaseUrl
        {
            get => _httpClient.BaseUrl;
            set => _httpClient.BaseUrl = value;
        }

        private readonly string? _certFilePath;


        public async Task<TResponse?> TryPostJsonAsync<TResponse>(string urlEndpoint, object requestObj, int timeoutMs = 5_000, bool logError = true)
        {
            TResponse? output = default;
            if(!_requestEvent.WaitOne(_waitEventTimeoutMs))
                return output;
            try
            {
                string requestJson = JsonConvert.SerializeObject(requestObj, Formatting.Indented);
                StringContent requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                _logger.Information($"[POST-JSON-Request] \n{requestJson}\n");


                var response = await _httpClient.Request(urlEndpoint)
                    .WithTimeout(TimeSpan.FromMilliseconds(timeoutMs))
                    .PostAsync(requestContent);
                string responseJson = await response.ResponseMessage.Content.ReadAsStringAsync();

                _logger.Information($"[POST-JSON-Response] \n{responseJson}\n");

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    output = JsonConvert.DeserializeObject<TResponse>(responseJson);
                }

            }
            catch (Exception ex)
            {
                if(logError)
                    _logger.Error(ex.Message.ToString());
            }
            
            _requestEvent.Set();
            return output;

        }


        public async Task<TResponse?> TryGetJsonAsync<TResponse>(string urlEndpoint, int timeoutMs = 5_000, bool logError = true)
        {
            TResponse? output = default;
            if(!_requestEvent.WaitOne(_waitEventTimeoutMs))
                return output;

            try
            {
                _logger.Information($"[GET-JSON-Request] {BaseUrl}/{urlEndpoint}");

                var response = await _httpClient.Request(urlEndpoint)
                    .WithTimeout(TimeSpan.FromMilliseconds(timeoutMs))
                    .GetAsync();
                string responseJson = await response.ResponseMessage.Content.ReadAsStringAsync();

                _logger.Information($"[GET-JSON-Response] \n{responseJson}\n");

                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    output = JsonConvert.DeserializeObject<TResponse>(responseJson);
                }

            }
            catch (Exception ex)
            {
                if(logError)
                    _logger.Error(ex.Message.ToString());
            }
            _requestEvent.Set();

            return output;
        }


        public async Task<Stream?> GetStreamAsync(string urlEndpoint, int timeoutSec = 60)
        {
            Stream? output = null;
            if (!_requestEvent.WaitOne(_waitEventTimeoutMs))
                return output;

            try
            {
                _logger.Information($"[GET-Stream-Request] {BaseUrl}/{urlEndpoint}");
        
                var response = await _httpClient.Request(urlEndpoint)
                    .WithTimeout(TimeSpan.FromMilliseconds(timeoutSec * 1000))
                    .GetAsync();
        
                _logger.Information($"[GET-Stream-Response] [StatusCode:{response.StatusCode}]");
                if (response.ResponseMessage.IsSuccessStatusCode)
                {
                    output = await response.GetStreamAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                _requestEvent.Set();
            }
        
            return output;
        
        }
        

        public async Task<bool> UploadFileAsync(string urlEndpoint, string filePath, string fileName, 
                                            IProgress<FileTransferProgressArgs>? progressReporter, CancellationToken cancellationToken)
        {
            bool output = false;
            if(!_requestEvent.WaitOne(_waitEventTimeoutMs))
                return output;

            try
            {
                var uploadContent = new MultipartFormDataContent();

                var fs = new StreamWithReadProgress(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), progressReporter);
                var fileContent = new StreamContent(fs);
                fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"file\"",
                    FileName = $"\"{fileName}\"",
                };
                uploadContent.Add(fileContent);


                var cidContent = new StringContent(_random.Next(100).ToString());
                cidContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "\"cid\"" };
                uploadContent.Add(cidContent);


                using var flurlClient = InitDefaultClient(BaseUrl, _certFilePath, 1);

                _logger.Information($"[POST-UploadFile-Request] {BaseUrl}/{urlEndpoint}");

                var response = await flurlClient.Request(urlEndpoint)
                    .WithTimeout(Timeout.InfiniteTimeSpan)
                    .PostAsync(uploadContent, cancellationToken: cancellationToken);

                string responseJson = await response.ResponseMessage.Content.ReadAsStringAsync();
                _logger.Information($"[POST-UploadFile-Response] \n {responseJson}");

                output =  response.ResponseMessage.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            _requestEvent.Set();
            return output;

        }


        public async Task<Stream?> DownloadFileAsync(string urlEndpoint, CancellationToken cancellationToken, IProgress<FileTransferProgressArgs> progressReporter)
        {
            Stream? output = null;
            if (!_requestEvent.WaitOne(_waitEventTimeoutMs))
                return output;

            try
            {
                using IFlurlResponse response =
                    await $"{BaseUrl}/{urlEndpoint}".GetAsync(HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                await using var stream = await response.GetStreamAsync();
                var receivedBytes = 0;
                var buffer = new byte[4096];
                var totalBytes = Convert.ToDouble(response.ResponseMessage.Content.Headers.ContentLength);

                var memStream = new MemoryStream();

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    await memStream.WriteAsync(buffer, 0, bytesRead);

                    if (bytesRead == 0)
                        break;
                    receivedBytes += bytesRead;

                    var args = new FileTransferProgressArgs(receivedBytes, totalBytes);
                    progressReporter.Report(args);
                }

                memStream.Position = 0;
                output = memStream;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            _requestEvent.Set();
            return output;
        }
        
        private FlurlClient InitDefaultClient(string baseUrl, string? certFilePath, int maxConnections)
        {
            var handler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(certFilePath))
            {
                var clientCertificate = new X509Certificate2(certFilePath);
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.SslProtocols = SslProtocols.Tls11;
                handler.ClientCertificates.Add(clientCertificate);
                handler.MaxConnectionsPerServer = maxConnections;
            }
            else
            {
                handler.ServerCertificateCustomValidationCallback = ((_, _, _, _) => true);
                handler.MaxConnectionsPerServer = maxConnections;
            }
            var client = new HttpClient(handler);
            var flurlClient = new FlurlClient(client, baseUrl);
            flurlClient.Settings.Timeout = TimeSpan.FromSeconds(5);
            return flurlClient;
        }


        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }


    public class FileTransferProgressArgs : EventArgs
    {
        public FileTransferProgressArgs(int bytesTransferred, double totalBytes)
        {
            BytesTransferred = bytesTransferred;
            TotalBytes = totalBytes;
        }

        public double TotalBytes { get; }

        public double BytesTransferred { get; }

        public double ProgressPercent => 100 * (BytesTransferred / TotalBytes);
    }



    internal class StreamWithReadProgress : Stream
    {
        private readonly Stream _baseStream;
        private readonly IProgress<FileTransferProgressArgs>? _progressReporter;
        private int _readLength = 0;

        public StreamWithReadProgress(Stream baseStream, IProgress<FileTransferProgressArgs>? progressReporter)
        {
            _baseStream = baseStream;
            _progressReporter = progressReporter;
        }

        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int currentReadLen = _baseStream.Read(buffer, offset, count);
            _readLength += currentReadLen;
            if(currentReadLen > 0)
                _progressReporter?.Report(new FileTransferProgressArgs(_readLength, _baseStream.Length));
            return currentReadLen;
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }
        
        public override bool CanRead => _baseStream.CanRead;

        public override bool CanSeek => _baseStream.CanSeek;
        
        public override bool CanWrite => _baseStream.CanWrite;

        public override long Length => _baseStream.Length;
       
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            _baseStream.Dispose();
        }

        public void ResetPosition()
        {
            _baseStream.Position = 0;
        }
    }


}
