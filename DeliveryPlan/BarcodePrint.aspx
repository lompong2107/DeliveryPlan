<%@ Page Language="C#" AutoEventWireup="true" Debug="true" CodeBehind="BarcodePrint.aspx.cs" Inherits="DeliveryPlan.BarcodePrint" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <script src="Scripts/jquery-3.4.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height: 90vh;">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" CssClass="ReportViewer" runat="server" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226" Width="100%" Style="margin-right: 0px" Height="100%">
            </rsweb:ReportViewer>
            <%-- ต้องเอาไว้ใน UpdatePanel เพราะเวลากดปริ้น Print Dialog จะได้ไม่หาย --%>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Button ID="BtnUpdate" runat="server" Text="Update" OnClick="BtnUpdate_Click" Style="display: none;" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <script>
            $(document).ready(function () {
                setInterval(function () {
                    //$("#ReportViewer1_ctl09_ctl06_ctl00_ctl00_ctl00").click(function () {
                    //var el = $.find(".msrs-printdialog-pprintbutton");
                    $(".msrs-printdialog-pprintbutton").click(function () {
                        $("#<%=BtnUpdate.ClientID %>").click();
                    })
                    //})
                }, 1000);
            })
        </script>
    </form>
</body>
</html>
