<%@ Control Language="C#" AutoEventWireup="true" Inherits="Sitecore.Social.GooglePlus.Client.Sharing.Controls.GooglePlusOneButton" %>
<%@ Import Namespace="Sitecore.Social.Client.Common.Helpers" %>
<%@ Import Namespace="Sitecore.Data" %>

<%
  var shareButtonId = string.Format("plusOne_{0}", ShortID.NewId());
%>

<div id="<%= shareButtonId %>" style="float: right;">
  <div
    class="g-plusone"
    data-href="<%= SharingHelper.GetSharePageUrlWithAnalyticsParameters(this.CampaignId) %>"
    data-callback="googlePlusOneSubscribe"
    data-size="medium">
  </div>
</div>

<script>(function (d, s, id) {
  var js, fjs = d.getElementsByTagName(s)[0];
  if (d.getElementById(id)) return;
  js = d.createElement(s); js.id = id;
  js.src = "https://apis.google.com/js/plusone.js";
  fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'plusone-js'));</script>

<script>
  (function () {
    var previous = window.googlePlusOneSubscribe;

    window.googlePlusOneSubscribe = function (jsonParam, that) {
      if (previous) {
        previous.call(this, jsonParam);
      }

      if (jsonParam.state === 'on' && this.vF.parentElement.id === '<%= shareButtonId %>') {
        <%= this.CallbackScript %>
      }
    };
  })();
</script>
