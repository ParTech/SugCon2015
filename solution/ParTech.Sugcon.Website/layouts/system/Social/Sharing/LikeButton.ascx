<%@ Control Language="C#" AutoEventWireup="true" Inherits="Sitecore.Social.Facebook.Client.Sharing.Controls.LikeButton" %>
<%@ Import Namespace="Sitecore.Social.Client.Common.Helpers" %>
<%@ Import Namespace="Sitecore.Data" %>

<%
  var shareButtonId = string.Format("likeButton_{0}", ShortID.NewId());
%>

<script>(function (d, s, id) {
  var js, fjs = d.getElementsByTagName(s)[0];
  if (d.getElementById(id)) return;
  js = d.createElement(s); js.id = id;
  js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.0";
  fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));</script>

<div style="float: right;">
  <fb:like id="<%= shareButtonId %>" href="<%= SharingHelper.GetSharePageUrlWithAnalyticsParameters(this.CampaignId) %>" layout="button_count" show_faces="true"></fb:like>
</div>

<script>
  (function () {
    var previous = window.fbAsyncInit;
    window.fbAsyncInit = function () {
      if (previous) {
        previous();
      }

      FB.Event.subscribe('edge.create', function (response, target) {
        if (target.id === '<%= shareButtonId %>') {
          <%= this.CallbackScript %>
        }
      });
    };
  })();
</script>
