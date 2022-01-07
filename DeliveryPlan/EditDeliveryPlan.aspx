<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="EditDeliveryPlan.aspx.cs" Inherits="DeliveryPlan.EditDeliveryPlan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
/*        .FixedHeader-1 {
            top: 51px !important;
        }

        .FixedHeader-2 {
            top: 72px !important;
        }*/

        .table th {
            background-color: #006699;
            color: white;
            text-align: center;
            /*position: -webkit-sticky;
            position: -moz-sticky;
            position: -ms-sticky;
            position: -o-sticky;
            position: sticky;
            z-index: 10;*/
        }

        .table-custom th, .table-custom td {
            height: auto !important;
            white-space: normal;
        }

        .CustomTxtTimePlan div {
            top: 50% !important;
            left: 50% !important;
            position: fixed;
            transform: translate(-50%, -50%) !important;
            height: 200px !important;
            width: 250px !important;
        }
    </style>

    <div style="margin-bottom: 10px;">
        <div class="container">
            <p class="text-center font-head">Delivery Actual</p>
            <hr />
            <div>
                <table class="table-addForm" style="width: 100%;">
                    <tr>
                        <td style="max-width: 100px;" class="text-right">วันที่ :</td>
                        <td style="position: relative; max-width: 200px;">
                            <div style="display: flex;">
                                <asp:LinkButton ID="BtnPrDate" CssClass="btn btn-default" runat="server" OnClick="BtnPrDate_Click"><i class="glyphicon glyphicon-backward"></i></asp:LinkButton>
                                <asp:TextBox ID="TxtDate" CssClass="form-control datepicker" Style="max-width: 200px;" runat="server"></asp:TextBox>
                                <asp:LinkButton ID="BtnNextDate" CssClass="btn btn-default" runat="server" OnClick="BtnNextDate_Click"><i class="glyphicon glyphicon-forward"></i></asp:LinkButton>
                                <asp:Button ID="BtnShow" runat="server" Style="margin-left: 5px;" CssClass="btn btn-primary" Text="แสดง" OnClick="SelectDate" />
                            </div>
                        </td>
                        <td style="max-width: 100px;" class="text-right">Customer :
                        </td>
                        <td style="max-width: 200px;">
                            <asp:DropDownList ID="DDListCustomer" OnSelectedIndexChanged="DDListCustomer_SelectedIndexChanged" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true">
                                <asp:ListItem Text="ทั้งหมด" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="text-right">
                            <asp:Button ID="BtnSaveAll" CssClass="btn btn-success" Text="บันทึก" runat="server" OnClick="BtnSaveAll_Click" Visible="false" />
                            <asp:Button ID="BtnEditAll" CssClass="btn btn-warning" Text="แก้ไข" runat="server" OnClick="BtnEditAll_Click" Visible="true" />
                            <asp:Button ID="BtnCanCelEdit" CssClass="btn btn-default" Text="ยกเลิก" runat="server" OnClick="BtnCanCelEdit_Click" Visible="false" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <div class="GridViewContainer" style="border-top: 2px solid #ddd; border-bottom: 2px solid #ddd;">
            <asp:GridView ID="GVDeliveryPlan" runat="server" ShowHeaderWhenEmpty="true" Width="100%" CssClass="table table-custom" AutoGenerateColumns="false" AllowSorting="true" OnSorting="GVDeliveryPlan_Sorting" EmptyDataText="Empty Data" OnRowDataBound="GVDeliveryPlan_RowDataBound" OnRowCreated="GVDeliveryPlan_RowCreated">
                <Columns>
                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                            <asp:HiddenField ID="HFDeliveryPlanID" Value='<%# Eval("DeliveryPlanDetailID") %>' runat="server" />
                        </ItemTemplate>
                        <HeaderStyle Width="20" CssClass="FixedHeader-2" />
                        <ItemStyle Width="20" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customer" SortExpression="CompanyName">
                        <ItemTemplate>
                            <p class="overflow-hide"><%# Eval("CompanyName") %></p>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="75" CssClass="FixedHeader-2" />
                        <ItemStyle Width="75" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Project" SortExpression="ProjectName">
                        <ItemTemplate>
                            <p class="overflow-hide" style="width: 105px;"><%# Eval("ProjectName") %></p>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="105" CssClass="FixedHeader-2" />
                        <ItemStyle Width="105" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ID Code" SortExpression="CustomerCode">
                        <ItemTemplate>
                            <%# Eval("CustomerCode") %>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="60" CssClass="FixedHeader-2" />
                        <ItemStyle Width="60" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Part No." SortExpression="FGName">
                        <ItemTemplate>
                            <a href="BarcodeScan.aspx?DeliveryPlanDetailID=<%# Eval("DeliveryPlanDetailID") %>" class="text-black"><%# Eval("FGName") %></a>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="135" CssClass="FixedHeader-2" />
                        <ItemStyle Width="135" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Part Name" SortExpression="PartName">
                        <ItemTemplate>
                            <a href="BarcodeScan.aspx?DeliveryPlanDetailID=<%# Eval("DeliveryPlanDetailID") %>" style="min-width: 250px;" class="text-black"><%# Eval("PartName") %></a>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="275" CssClass="FixedHeader-2" />
                        <ItemStyle Width="275" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Part Image">
                        <ItemTemplate>
                            <asp:Image ID="ImgPartGV" runat="server" Style="max-width: 100px; max-height: 49px;" ImageUrl='<%# (Request.Url.Host != "110.77.148.173" ? Eval("ImagePart").ToString().Replace("110.77.148.173:2027", "192.168.0.9") : Eval("ImagePart")) %>' />
                        </ItemTemplate>
                        <HeaderStyle Width="105" CssClass="FixedHeader-2" />
                        <ItemStyle CssClass="text-center" Width="105" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Transport" ItemStyle-HorizontalAlign="Center" SortExpression="TransportName">
                        <ItemTemplate>
                            <%# Eval("TransportName") %>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" Width="70" CssClass="FixedHeader-2" />
                        <ItemStyle Width="70" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Time Plan" ItemStyle-HorizontalAlign="Center" SortExpression="TimePlan">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("TimePlan").ToString()).ToString("HH:mm") %>
                        </ItemTemplate>
                        <HeaderStyle ForeColor="white" CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Time Actual" ItemStyle-CssClass="CustomTxtTimePlan" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="TxtTimeActual" CssClass="form-control input-sm timepicker text-center CustomTxtTimePlan" Style="width: 50px; padding: 2px;" Text='<%# Eval("TimeActual") %>' runat="server" Enabled="false"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Time Diff" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <strong>
                                <asp:Label ID="LbDiffTime" runat="server" Text='<%# (DateTime.Parse(Eval("TimePlan").ToString()).Subtract(DateTime.Parse(Eval("TimeActual").ToString())).Minutes < 0 ? "-" : "") + DateTime.Parse(Eval("TimePlan").ToString()).Subtract(DateTime.Parse(Eval("TimeActual").ToString())).Hours.ToString("00") + ":" + DateTime.Parse(Eval("TimePlan").ToString()).Subtract(DateTime.Parse(Eval("TimeActual").ToString())).Negate().Minutes.ToString("00") %>'></asp:Label>
                            </strong>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Plan" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# Eval("QtyPlan") %>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Shop แล้ว" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID='TxtQtyActual' CssClass="form-control input-sm text-right" TextMode="Number" Style="width: 45px; padding: 2px;" Text='<%# Eval("QtyActual") %>' runat="server" Enabled="false"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                        <ItemStyle CssClass="form-inline" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Diff" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <strong>
                                <asp:Label ID="LbDiffQty" runat="server" Text='<%# int.Parse(Eval("QtyActual").ToString()) - int.Parse(Eval("QtyPlan").ToString()) %>'></asp:Label>
                            </strong>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status Del-Q'ty" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%# Eval("StatusName") %>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="หมายเหตุ">
                        <ItemTemplate>
                            <span class="more_info" title='<%# Eval("Remark") %>'><asp:TextBox ID="TxtRemark" CssClass="form-control input-sm" runat="server" Style="margin-top: 2px; width: auto; padding: 5px 0px; text-align: left;" Text='<%# Eval("Remark") %>' Enabled="false"></asp:TextBox></span>
                        </ItemTemplate>
                        <HeaderStyle CssClass="FixedHeader-2" />
                        <ItemStyle CssClass="font-family-prompt" />
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle HorizontalAlign="Center" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
