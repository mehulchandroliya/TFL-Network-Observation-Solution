using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using Tfl.Dynamic.Observation.Plugins.Common;
using Tfl.Dynamic.Observation.Plugins.Common.Utility;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.FileTypeAttachment;
using Tfl.Dynamic.Observation.Plugins.Common.Utility.Image;

namespace Tfl.Dynamic.Observation.Plugins.tfl_observationattachment
{
    /// <summary>
    /// Trigger On Post Update Async
    /// </summary>
    public class OnPostUpdateAsync : PluginBase
    {
        protected override int PluginDepth => 1;

        public override void ExecutePlgin()
        {
            TraceHelper($"OnPostUpdateAsync - Start");
            if (ShouldExecute())
            {
                try
                {
                    FileDetails fileDetails = ReadAttachment.File(OrganizationServiceAsSystem, TraceHelper, Target.ToEntityReference(), "tfl_photo");
                    fileDetails.Data = CompanyLogoWaterMark.Apply(fileDetails.Data);
                    UpdateAttachment.File(OrganizationServiceAsSystem, TraceHelper, Target.ToEntityReference(), "tfl_photo", fileDetails);
                    UpdateImageOverlayStatus(929700001);
                }
                catch (Exception ex)
                {
                    UpdateImageOverlayStatus(929700002, ex.Message);
                }
            }
            else
            {
                TraceHelper($"record {Target.Id} does not contains file.");
                return;
            }
            TraceHelper($"OnPostUpdateAsync - Start");
        }

        public bool ShouldExecute()
        {
            if (Target != null)
            {
                var retrievedRecord = OrganizationServiceAsSystem.Retrieve(Target.LogicalName, Target.Id, new ColumnSet("tfl_observationattachmentid", "tfl_photo"));
                return retrievedRecord.Contains("tfl_photo");
            }
            return false;
        }

        public void UpdateImageOverlayStatus(int status, string errorMessage = null)
        {
            TraceHelper($"UpdateImageOverlayStatus - Start");
            Entity entity = new Entity(Target.LogicalName, Target.Id);
            entity["tfl_imageoverlayedstatus"] = new OptionSetValue(status);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                entity["tfl_imageoverlayerrormessage"] = errorMessage;
            }
            OrganizationServiceAsSystem.Update(entity);
            TraceHelper($"UpdateImageOverlayStatus - Completed");
        }
    }
}
