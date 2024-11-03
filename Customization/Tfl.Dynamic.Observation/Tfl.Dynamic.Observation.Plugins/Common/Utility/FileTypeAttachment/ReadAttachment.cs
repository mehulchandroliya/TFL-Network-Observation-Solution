using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Services;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.FileTypeAttachment;

namespace Tfl.Dynamic.Observation.Plugins.Common.Utility
{
    public static class ReadAttachment
    {
        public static FileDetails File(IOrganizationService service, Action<string> tracingService, EntityReference entityReference, string attributeName)
        {
            try
            {
                tracingService($"ReadAttachment.File - Start");
                InitializeFileBlocksDownloadRequest initializeFileBlocksDownloadRequest = new InitializeFileBlocksDownloadRequest()
                {
                    Target = entityReference,
                    FileAttributeName = attributeName
                };
                var initializeFileBlocksDownloadResponse = (InitializeFileBlocksDownloadResponse)service.Execute(initializeFileBlocksDownloadRequest);
                string fileContinuationToken = initializeFileBlocksDownloadResponse.FileContinuationToken;
                long fileSizeInBytes = initializeFileBlocksDownloadResponse.FileSizeInBytes;
                List<byte> fileBytes = new List<byte>((int)fileSizeInBytes);
                long offset = 0;
                long blockSizeDownload = 4 * 1024 * 1024;
                if (fileSizeInBytes < blockSizeDownload)
                {
                    blockSizeDownload = fileSizeInBytes;
                }
                tracingService($"ReadAttachment.File - File Information Retieved");
                while (fileSizeInBytes > 0)
                {
                    DownloadBlockRequest downLoadBlockRequest = new DownloadBlockRequest()
                    {
                        BlockLength = blockSizeDownload,
                        FileContinuationToken = fileContinuationToken,
                        Offset = offset
                    };

                    var downloadBlockResponse = (DownloadBlockResponse)service.Execute(downLoadBlockRequest);
                    fileBytes.AddRange(downloadBlockResponse.Data);
                    fileSizeInBytes -= (int)blockSizeDownload;
                    offset = blockSizeDownload;
                }
                tracingService($"ReadAttachment.File - Download Completed");
                return new FileDetails { Data = fileBytes.ToArray(), FileName = initializeFileBlocksDownloadResponse.FileName };
            }
            catch (Exception ex)
            {
                tracingService($"ReadAttachment.File - File Read Exception: {ex.Message}");
                throw ex;
            }
            finally
            {
                tracingService($"ReadAttachment.File - End");
            }
        }
    }
}
