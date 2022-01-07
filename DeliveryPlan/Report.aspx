<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="DeliveryPlan.Report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
        .GridViewContainer > div:nth-child(1) {
            overflow-x: auto;
            min-width: 500px;
        }

        .GridViewContainer > div:nth-child(2) {
            overflow-x: auto;
        }
    </style>
    <div class="container">
        <p class="text-center font-head">Report Delivery Plan</p>
        <%--<hr />--%>
        <div>
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist" style="margin-bottom: 10px;">
                <li role="presentation" id="LiReportDay" runat="server" class="active"><a href="#<%=ReportDay.ClientID%>" aria-controls="ReportDay" role="tab" data-toggle="tab">รายวัน</a></li>
                <li role="presentation" id="LiReportMonth" runat="server"><a href="#<%=ReportMonth.ClientID%>" aria-controls="ReportMonth" role="tab" data-toggle="tab">รายเดือน</a></li>
            </ul>

            <!-- Tab panes -->
            <div class="tab-content d-flex">
                <div role="tabpanel" class="tab-pane active w-60" id="ReportDay" runat="server">
                    <table class="table-report" style="margin: auto; width: 100%;">
                        <tr>
                            <td class="text-right" style="width: 100px;">ตั้งแต่วันที่ :</td>
                            <td style="width: 100px; position: relative;">
                                <asp:TextBox ID="TxtDateStart" CssClass="form-control datepicker datepickerStart" runat="server"></asp:TextBox>
                            </td>
                            <td class="text-right" style="width: 100px;">ถึงวันที่ :</td>
                            <td style="width: 100px; position: relative;">
                                <asp:TextBox ID="TxtDateEnd" CssClass="form-control datepicker datepickerEnd" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="BtnShow" runat="server" CssClass="btn btn-primary" Text="แสดง" OnClick="SelectDate" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div role="tabpanel" class="tab-pane w-60" id="ReportMonth" runat="server">
                    <table class="table-report" style="margin: auto; width: 100%;">
                        <tr>
                            <td style="min-width: 50px; max-width: 100px;" class="text-right">วันที่ :</td>
                            <td style="position: relative; min-width: 50px; max-width: 200px;">
                                <div style="display: flex;">
                                    <asp:LinkButton ID="BtnPrDate" CssClass="btn btn-default" runat="server" OnClick="BtnPrDate_Click"><i class="glyphicon glyphicon-backward"></i></asp:LinkButton>
                                    <asp:TextBox ID="TxtDateMonth" CssClass="form-control datepickerMonthly" Style="max-width: 200px;" runat="server"></asp:TextBox>
                                    <asp:LinkButton ID="BtnNextDate" CssClass="btn btn-default" runat="server" OnClick="BtnNextDate_Click"><i class="glyphicon glyphicon-forward"></i></asp:LinkButton>
                                    <asp:Button ID="BtnShowMonth" runat="server" Style="margin-left: 5px;" CssClass="btn btn-primary" Text="แสดง" OnClick="SelectDateMonth" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="w-40">
                    <table class="table-report" style="margin: auto; width: 100%;">
                        <tr>
                            <td style="width: 100px;" class="text-right">Customer :</td>
                            <td style="max-width: 300px;">
                                <asp:DropDownList ID="DDListCustomerSort" OnSelectedIndexChanged="DDListCustomerSort_SelectedIndexChanged" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true">
                                    <asp:ListItem Text="ทั้งหมด" Value="0" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="text-right" style="padding: 0 5px;">
                                <asp:Button ID="BtnExport" runat="server" CssClass="btn btn-success btn-xs" Text="Export to Excel" Enabled="true" OnClick="BtnExport_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <%--แสดงข้อมูล--%>
    <div class="GridViewContainer" style="width: 100%; display: inline-grid; grid-template-columns: auto auto; overflow-x: hidden; overflow-y: auto; max-height: 80vh;">
        <%--ตารางงงงงงงงงงงงงงงงง--%>
        <asp:GridView ID="GVDeliveryPlan" runat="server" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowDataBound="GVDeliveryPlan_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="#">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Customer">
                    <ItemTemplate>
                        <p class="overflow-hide"><%# Eval("CompanyName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Project">
                    <ItemTemplate>
                        <p class="overflow-hide"><%# Eval("ProjectName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="ID Code">
                    <ItemTemplate>
                        <%# Eval("CustomerCode") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part No.">
                    <ItemTemplate>
                        <p class="overflow-hide"><%# Eval("FGName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part Name">
                    <ItemTemplate>
                        <p class="overflow-hide"><%# Eval("PartName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part Image" ItemStyle-Height="50" ItemStyle-Width="100">
                    <ItemTemplate>
                        <asp:Image ID="ImgPartGV" runat="server" Style="max-width: 100px; max-height: 49px;" ImageUrl='<%# (Request.Url.Host != "110.77.148.173" ? Eval("ImagePart").ToString().Replace("110.77.148.173:2027", "192.168.0.9") : Eval("ImagePart")) %>' />
                    </ItemTemplate>
                    <ItemStyle CssClass="text-center" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Transport" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Eval("TransportName") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
            <EmptyDataRowStyle HorizontalAlign="Center" />
        </asp:GridView>
        <asp:GridView ID="GVDeliveryPlanDetail" runat="server" Width="100%" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowDataBound="GVDeliveryPlanDetail_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="HF" Visible="false">
                    <ItemTemplate>
                        <asp:HiddenField ID="HFGVDeliveryPlanID" Value='<%# Eval("DeliveryPlanID") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
            <EmptyDataRowStyle HorizontalAlign="Center" />
        </asp:GridView>
    </div>
    <script type="text/javascript" src="Scripts/js.js"></script>
</asp:Content>
