<%@ Control Language="C#" AutoEventWireup="true" Inherits="Sitecore.Social.Twitter.Client.Sharing.Controls.TweetButton" %>
<%@ Import Namespace="Sitecore.Globalization" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Social.Client.Common.Helpers" %>

<%
  var shareButtonId = string.Format("tweetButton_{0}", ShortID.NewId());
   %>

<div style="float: right;">
  <a id="<%= shareButtonId %>"
    href="https://twitter.com/share"
    class="twitter-share-button"
    data-url="<%= HttpUtility.UrlPathEncode(SharingHelper.GetSharePageUrlWithAnalyticsParameters(this.CampaignId))%>"
    data-counturl="<%= HttpUtility.UrlPathEncode(SharingHelper.GetSharePageUrlWithAnalyticsParameters(this.CampaignId))%>"
    data-count="horizontal"><%= Translate.Text(Sitecore.Social.Twitter.Common.Texts.Tweet)%></a>
</div>

<script>window.twttr = (function (d, s, id) {
  var t, js, fjs = d.getElementsByTagName(s)[0];
  if (d.getElementById(id)) return window.twttr;
  js = d.createElement(s); js.id = id;
  js.src = "https://platform.twitter.com/widgets.js";
  fjs.parentNode.insertBefore(js, fjs);
  return window.twttr || (t = { _e: [], ready: function (f) { t._e.push(f); } });
}(document, "script", "twitter-wjs"));</script>

<script>
  twttr.ready(
  function (twttr) {
    twttr.events.bind('tweet', function (event) {
      if (event.target.id === '<%= shareButtonId %>'){
        <%= this.CallbackScript %>
      }
    });
  });
</script>
