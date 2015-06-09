<%@ Control Language="C#" AutoEventWireup="true" Inherits="Sitecore.Social.LinkedIn.Client.Sharing.Controls.LinkedInShareButton" %>
<%@ Import Namespace="Sitecore.Social.Client.Common.Helpers" %>
<script src="//platform.linkedin.com/in.js" type="text/javascript">
  lang: en_US
</script>
<div style="float:right;">
  <script type="IN/Share" data-url="<%= SharingHelper.GetSharePageUrlWithAnalyticsParameters(this.CampaignId) %>" data-counter="right" data-onsuccess="linkedInShareSubscribe"></script>
</div>