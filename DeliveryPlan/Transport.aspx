<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="Transport.aspx.cs" Inherits="DeliveryPlan.Transport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <p class="text-center font-head">Transport</p>
        <hr />

        <%--เพิ่มข้อมูล--%>
        <div>
            <table style="width: auto;">
                <tr>
                    <td class="text-right" style="padding-right: 5px;">
                        <asp:Label ID="LbManageName" CssClass="h4" runat="server" Text="Transport Name"></asp:Label>
                    </td>
                    <td style="padding-right: 5px;">
                        <asp:TextBox ID="TxtTransportName" CssClass="form-control" runat="server" placeholder="Transport Name"></asp:TextBox>
                    </td>
                    <td style="padding-right: 5px;">
                        <asp:Button ID="BtnAddTransport" CssClass="btn btn-success" runat="server" Text="บันทึก" OnClick="BtnAddTransport_Click" />
                        <asp:Button ID="BtnUpdateTransport" CssClass="btn btn-primary" runat="server" Text="อัพเดต" OnClick="BtnUpdateTransport_Click" Visible="false" />
                    </td>
                    <td>
                        <asp:Button ID="BtnCancleUpdate" CssClass="btn btn-default" runat="server" Text="ยกเลิก" OnClick="BtnCancleUpdate_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <%--แสดงข้อมูล--%>
        <div style="padding-top: 10px;">
            <asp:GridView ID="GVTransport" ShowHeaderWhenEmpty="true" CssClass="table table-bordered table-striped" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="OnPageIndexChanging" EmptyDataText="ไม่พบข้อมูล" OnRowDataBound="GVTransport_RowDataBound">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="100px" HeaderText="#" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TransportName" HeaderText="Transport Name" />
                    <asp:TemplateField HeaderText="สถานะ" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkBtnStatus" runat="server" CommandArgument='<%# Eval("TransportID") %>' OnCommand="BtnStatus_Command" Text="ใช้งาน" UseSubmitBehavior="False"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="จัดการ" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="BtnEditTransport" Style="width: 60px;" CommandArgument='<%# Eval("TransportID") %>' OnCommand="BtnEdit_Command" CssClass="btn btn-sm btn-default" runat="server" Text="แก้ไข" UseSubmitBehavior="False" />
                            <%--<asp:Button ID="BtnDeleteTransport" Style="width: 50px; padding-left: 0; padding-right: 0;" CommandArgument='<%# Eval("TransportID") %>' OnCommand="BtnDelete_Command" CssClass="btn btn-sm btn-danger" runat="server" Text="ลบ" OnClientClick="return AlertConfirm(this);" />--%>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
                <EmptyDataRowStyle HorizontalAlign="Center" />
                <PagerSettings Mode="Numeric"
                    PageButtonCount="10" />
                <PagerStyle BackColor="#006699"
                    Font-Size="Medium"
                    CssClass="cssPager"
                    HorizontalAlign="Center" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
