<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DeliveryPlan.Login" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Delivery Plan</title>
    <%-- Bootstrap --%>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <%-- Jquery --%>
    <script src="Scripts/jquery-3.4.1.min.js"></script>
    <%-- Alert --%>
    <script src="Scripts/sweetalert2.js"></script>
    <style>
        html, body {
            height: 100vh;
            min-width: 1280px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
    </style>
</head>
<body>
    <%-- Alert Custom --%>
    <script type="text/javascript" src="Scripts/alert.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= HFWidth.ClientID %>").val(screen.width);
        })
    </script>

    <form id="form1" runat="server">
        <asp:HiddenField ID="HFWidth" runat="server" />
        <div style="width: 350px;">
            <div class="form-group text-center">
                <h1>Delivery Plan</h1>
            </div>
            <div class="form-group">
                <asp:TextBox ID="TxtUser" runat="server" placeholder="ชื่อผู้ใช้" CssClass="form-control input-lg"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:TextBox ID="TxtPassword" runat="server" placeholder="รหัสผ่าน" CssClass="form-control input-lg" TextMode="Password"></asp:TextBox>
            </div>
            <div class="form-group text-center">
                <asp:Button ID="BtnLogin" runat="server" Text="เข้าสู่ระบบ" CssClass="btn btn-primary btn-lg" Width="150px" OnClick="BtnLogin_Click" />
                <asp:Button ID="BtnCancel" runat="server" Text="ยกเลิก" CssClass="btn btn-default btn-lg" Width="150px" OnClick="BtnCancel_Click" />
            </div>
            <div class="text-center">
                <a id="BtnHowTo" href="Document/HowToDeliveryPlan.pdf" target="_blank">คู่มือ?</a>
            </div>
        </div>
    </form>
</body>
</html>
