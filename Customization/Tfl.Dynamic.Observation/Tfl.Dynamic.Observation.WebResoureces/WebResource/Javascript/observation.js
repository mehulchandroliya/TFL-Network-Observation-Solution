function OnSelectAttachmentGrid(executionContext) {
    var eventSource = executionContext.getEventSource();
    var entityReference = eventSource.getEntityReference()
    var formcontext = executionContext.getFormContext()
    var currentEntity = formcontext.data.entity;
    currentEntity.attributes.forEach(function (attribute, i) {
        var attributeToDisable = attribute.controls.get(0);
        attributeToDisable.setDisabled(true);
    });
    Xrm.WebApi.retrieveRecord("tfl_observationattachment", entityReference.id).then(
        function success(result) {
            if (result.tfl_photo_url == null || result.tfl_photo_url == "") {
                var page = Xrm.Page;
                page.getControl("IFRAME_observation_attach").setVisible(false);
            } else {
                var globalContext = Xrm.Utility.getGlobalContext();
                var orgUrl = globalContext.getClientUrl();
                var url = orgUrl + result.tfl_photo_url + "&Full=true";
                var page = Xrm.Page;
                page.getControl("IFRAME_observation_attach").setVisible(true);
                page.getControl("IFRAME_observation_attach").setSrc(url);
            }
        });
}