<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="Customer (Close).aspx.cs" Inherits="DeliveryPlan.AddCustomer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .text-center-header th {
            text-align: center;
        }

        .cssPager td {
            padding-left: 4px;
            padding-right: 4px;
            color: white !important;
        }

        .cssPager a {
            color: black !important;
        }
    </style>
    <div class="container">
        <h2 class="text-center">Customer</h2>
        <hr />

        <%--เพิ่มข้อมูล--%>
        <div>
            <table style="width: auto;">
                <tr>
                    <td class="text-right" style="padding-right: 5px;">
                        <p class="text-center font-head">Customer Name</p>
                    </td>
                    <td style="padding-right: 5px;">
                        <asp:TextBox ID="TxtCustName" CssClass="form-control" runat="server" placeholder="Customer Name"></asp:TextBox>
                    </td>
                    <td style="padding-right: 5px;">
                        <asp:Button ID="BtnAddCust" CssClass="btn btn-success" runat="server" Text="Add" OnClick="BtnAddCust_Click" />
                        <asp:Button ID="BtnUpdateCust" CssClass="btn btn-primary" runat="server" Text="Update" OnClick="BtnUpdateCust_Click" Visible="false" />
                    </td>
                    <td>
                        <asp:Button ID="BtnCancleUpdate" CssClass="btn btn-default" runat="server" Text="Cancel" OnClick="BtnCancleUpdate_Click" />
                    </td>
                </tr>
            </table>
        </div>

        <%--แสดงข้อมูล--%>
        <div style="padding-top: 10px;">
            <asp:GridView ID="GVCustomer" ShowHeaderWhenEmpty="true" CssClass="table table-bordered table-striped" runat="server" AutoGenerateColumns="false" AllowPaging="true" AllowSorting="true" OnPageIndexChanging="OnPageIndexChanging" EmptyDataText="ไม่พบข้อมูล" OnRowDataBound="GVCustomer_RowDataBound">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="100px" HeaderText="#" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CustName" HeaderText="Customer Name" />
                    <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="LinkBtnStatus" runat="server" CommandArgument='<%# Eval("CustID") %>' OnCommand="BtnStatus_Command" Text="ใช้งาน" UseSubmitBehavior="False"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manage" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Button ID="BtnEditCust" Style="width: 60px;" CommandArgument='<%# Eval("CustID") %>' OnCommand="BtnEdit_Command" CssClass="btn btn-sm btn-default" runat="server" Text="Edit" UseSubmitBehavior="False" />
                            <asp:Button ID="BtnDeleteCust" Style="width: 50px; padding-left: 0; padding-right: 0;" CommandArgument='<%# Eval("CustID") %>' OnCommand="BtnDelete_Command" CssClass="btn btn-sm btn-danger" runat="server" Text="Delete" UseSubmitBehavior="False" OnClientClick="if (!confirm('Are you sure you want delete?')) return false;" />
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
