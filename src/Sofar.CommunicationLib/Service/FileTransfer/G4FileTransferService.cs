using System.Diagnostics;

namespace Sofar.CommunicationLib.Service.FileTransfer
{
    public class G4FileTransferService : ServiceBase
    {
        public static Task<byte[]?> G4ReadSegmentedFileAsync(byte slave, string filename, byte unitSize, byte unitCountPerRequest,
                                                        CancellationToken cancellationToken, IProgress<int>? pgReporter)
        {
            // if (_longRunningEvent.WaitOne(10))
            // {
            //     throw new InvalidOperationException("Another modbus long-running task is in progress.");
            // }

            if (_modbusClient == null)
            {
                throw new InvalidOperationException("No Modbus Connection.");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    return G4ReadSegmentedFile(slave, filename, unitSize, unitCountPerRequest, cancellationToken, pgReporter);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
                finally
                {
                    // _longRunningEvent.Set();
                }
            });
        }

        private static byte[]? G4ReadSegmentedFile(byte slave, string filename, byte unitSize, byte unitCountPerRequest,
                                            CancellationToken cancellationToken, IProgress<int>? pgReporter)
        {
            _logger.Information($"设备{slave}, 写入文件【{filename}】");
            var startReadResponse = _modbusClient.StartReadFileSegment(slave, filename, 1000);
            if (startReadResponse == null)
            {
                return null;
            }

            int total = (int)startReadResponse.FileSize;
            int requestCount = total / unitCountPerRequest +
                               (total % unitCountPerRequest == 0 ? 0 : 1);

            byte[] data = new byte[unitSize * total];
            int retries = 3;
            for (int n = 0; n < requestCount; n++)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentCount = unitCountPerRequest;
                if (currentCount > total - unitCountPerRequest * n)
                    currentCount = total - unitCountPerRequest * n;

                for (int i = 0; i < retries; i++)
                {
                    var readDataResponse = _modbusClient.ReadFileSegmentData(slave, startReadResponse.FileCrcID,
                        (ushort)(n * unitCountPerRequest), (ushort)currentCount, unitSize, 1000);

                    if (readDataResponse != null /*&& readDataResponse.UnitCount >= currentCount*/)
                    {
                        readDataResponse.SegmentBytes.CopyTo(data, n * unitCountPerRequest * unitSize);
                        pgReporter?.Report((int)(n + 1.0) / requestCount * 100);
                        break;
                    }
                    else
                    {
                        if (i == retries - 1)
                        {
                            return null;
                        }
                    }
                }
            }

            return data;
        }

        public static Task<bool> G4WriteSegmentedFileAsync(byte slave, string filename, byte unitSize, byte unitCountPerRequest,
                                                    byte[] fileData, CancellationToken cancellationToken, IProgress<int>? pgReporter)
        {
            // if (_longRunningEvent.WaitOne(10))
            // {
            //     throw new InvalidOperationException("Another modbus long-running task is in progress.");
            // }
            if (_modbusClient == null)
            {
                throw new InvalidOperationException("No Modbus Connection.");
            }

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    return G4WriteSegmentedFile(slave, filename, unitSize, unitCountPerRequest, fileData, cancellationToken, pgReporter);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
                finally
                {
                    // _longRunningEvent.Set();
                }
            });
        }

        private static bool G4WriteSegmentedFile(byte slave, string filename, byte unitSize, byte unitCountPerRequest,
                                             in byte[] fileData, CancellationToken cancellationToken, IProgress<int>? pgReporter)
        {
            _logger.Information($"设备{slave}, 读取文件【{filename}】");
            var startWriteResponse = _modbusClient.StartWriteFileSegment(slave, filename, 1000);
            if (startWriteResponse == null)
            {
                return false;
            }

            int total = (int)startWriteResponse.FileSize;
            if (fileData.Length / unitSize <= total)
            {
                total = fileData.Length / unitSize;
            }
            else
            {
                return false;
            }

            int requestCount = total / unitCountPerRequest +
                               (total % unitCountPerRequest == 0 ? 0 : 1);

            byte[] data = new byte[unitSize * total];
            int retries = 3;
            for (int n = 0; n < requestCount; n++)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                int currentCount = unitCountPerRequest;
                if (currentCount > total - unitCountPerRequest * n)
                    currentCount = total - unitCountPerRequest * n;

                for (int i = 0; i < retries; i++)
                {
                    var writeDataResponse = _modbusClient.WriteFileSegmentData(slave, startWriteResponse.FileCrcID, (ushort)(n * unitCountPerRequest),
                        fileData.Skip(n * unitCountPerRequest * unitSize).Take(currentCount).ToArray(), 1000);

                    if (writeDataResponse != null && writeDataResponse.ResultCode == 0)
                    {
                        pgReporter?.Report((int)(n + 1.0) / requestCount * 100);
                        break;
                    }
                    else
                    {
                        if (i == retries - 1)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}