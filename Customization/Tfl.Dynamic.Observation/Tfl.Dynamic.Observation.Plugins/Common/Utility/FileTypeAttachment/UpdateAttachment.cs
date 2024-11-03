using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.FileTypeAttachment;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.Image;

namespace Tfl.Dynamic.Observation.Plugins.Common.Utility
{
    public static class UpdateAttachment
    {
        public static FileDetails File(IOrganizationService service, Action<string> tracingService, EntityReference entityReference, string attributeName, FileDetails fileDetails)
        {
            try
            {
                tracingService($"UpdateAttachment.File - Start");
                byte[] filecontent = fileDetails.Data;
                var initializeFileBlocksUploadRequest = new InitializeFileBlocksUploadRequest()
                {
                    Target = entityReference,
                    FileName = fileDetails.FileName,
                    FileAttributeName = attributeName
                };
                var initializeFileBlocksUploadResponse = (InitializeFileBlocksUploadResponse)service.Execute(initializeFileBlocksUploadRequest);
                List<string> blockIds = new List<string>();
                int blockSizeDownload = 4 * 1024 * 1024;
                if (filecontent.Length < blockSizeDownload)
                {
                    blockSizeDownload = filecontent.Length;
                }
                for (int i = 0; i < filecontent.Length / blockSizeDownload; i++)
                {
                    string blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                    blockIds.Add(blockId);

                    var uploadBlockRequest = new UploadBlockRequest()
                    {
                        BlockId = blockId,
                        BlockData = filecontent.Skip(i * blockSizeDownload).Take(blockSizeDownload).ToArray(),
                        FileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken
                    };
                    var uploadBlockResponse = (UploadBlockResponse)service.Execute(uploadBlockRequest);
                }
                var commitFileBlocksUploadRequest = new CommitFileBlocksUploadRequest
                {
                    FileContinuationToken = initializeFileBlocksUploadResponse.FileContinuationToken,
                    FileName = fileDetails.FileName,
                    MimeType = System.Web.MimeMapping.GetMimeMapping(fileDetails.FileName),
                    BlockList = blockIds.ToArray()

                };
                var commitFileBlocksUploadResponse = (CommitFileBlocksUploadResponse)service.Execute(commitFileBlocksUploadRequest);
                return new FileDetails { Data = filecontent, FileName = fileDetails.FileName };
            }
            catch (Exception ex)
            {
                tracingService($"UpdateAttachment.File - File Upload Exception: {ex.Message}");
                throw ex;
            }
            finally
            {
                tracingService($"UpdateAttachment.File - End");
            }
        }
    }
}
