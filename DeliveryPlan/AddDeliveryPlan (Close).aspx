<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="AddDeliveryPlan (Close).aspx.cs" Inherits="DeliveryPlan.AddDeliveryPlan" %>
<%--หน้านี้ยังไม่ได้ใช้นะ เอาไว้เพิ่มทีล่ะ 1--%>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .text-center-header th {
            text-align: center;
        }

        .cssPager td {
            padding-left: 4px;
            padding-right: 4px;
            color: white;
        }

        .table-addForm tr > td {
            padding: 0 5px 5px 0;
        }

        .CustomTxtTimePlan {
            position: relative;
        }
    </style>
    <div>
        <div class="container">
            <h2 class="text-center">Add Delivery Plan</h2>
            <hr />
            <div>
                <%--เพิ่มข้อมูล--%>
                <table class="table-addForm" style="width: 100%;">
                    <tr>
                        <td style="width: 100px;" class="text-right">Customer :</td>
                        <td style="width: 500px;">
                            <asp:DropDownList ID="DDListCustomer" CssClass="form-control" runat="server" DataTextField="CustName" DataValueField="CustID" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select Customer --" Value="0" Selected="True" disabled></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px;" class="text-right">Date :</td>
                        <td>
                            <asp:TextBox ID="TxtDate" style="width: 200px;" CssClass="form-control" TextMode="Date" runat="server" AutoPostBack="true" OnTextChanged="SelectDate"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="LbDate" CssClass="h4" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;" class="text-right">Project :</td>
                        <td>
                            <asp:DropDownList ID="DDListProject" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListProject" AutoPostBack="true">
                                <asp:ListItem Text="-- Select Project --" Value="0" Selected="True" disabled></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px;" class="text-right">Quantity :</td>
                        <td>
                            <asp:TextBox ID="TxtQty" style="width: 70px;" TextMode="Number" Text="0" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;" class="text-right">Part :</td>
                        <td>
                            <asp:DropDownList ID="DDListPart" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListPart" AutoPostBack="true">
                                <asp:ListItem Text="-- Select Part --" Value="0" Selected="True" disabled></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px;" class="text-right">Time :</td>
                        <td style="position: relative;">
                            <asp:TextBox ID="TxtTimePlanAdd" style="width: 70px;" Text="00:00" CssClass="form-control" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;" class="text-right">FG :</td>
                        <td>
                            <asp:DropDownList ID="DDListFG" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListFG" AutoPostBack="true">
                                <asp:ListItem Text="-- Select FG --" Value="0" Selected="True" disabled></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px;" class="text-right">ID Code :</td>
                        <td>
                            <asp:Label ID="LbIDCode" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 100px;" class="text-right">Transport :</td>
                        <td>
                            <asp:DropDownList ID="DDListTransport" CssClass="form-control" runat="server" AppendDataBoundItems="true">
                                <asp:ListItem Text="-- Select Transport --" Value="0" Selected="True" disabled></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <div class="form-inline">
                                <asp:Button ID="BtnAdd" CssClass="btn btn-primary" Width="100px" runat="server" Text="เพิ่ม" OnClick="BtnAdd_Click" />
                                <asp:Button ID="BtnCancle" CssClass="btn btn-default" Width="60px" runat="server" Text="ล้าง" OnClick="BtnCancle_Click" />
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>

        <%--ตารางงงงงงงงงงงง--%>
        <div style="margin-top: 10px;">
            <asp:GridView ID="GVDeliveryPlan" runat="server" ShowHeaderWhenEmpty="true" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" AllowPaging="true" OnPageIndexChanging="OnPageIndexChanging" EmptyDataText="ไม่พบข้อมูล">
                <Columns>
                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Customer">
                        <ItemTemplate>
                            <%# Eval("CustName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Project">
                        <ItemTemplate>
                            <%# Eval("ProjectName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="ID Code">
                        <ItemTemplate>
                            <%# Eval("CustomerCode") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Part No.">
                        <ItemTemplate>
                            <%# Eval("FGName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Part Name">
                        <ItemTemplate>
                            <%# Eval("PartName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Transport">
                        <ItemTemplate>
                            <%# Eval("TransportName") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty">
                        <ItemTemplate>
                            <asp:TextBox ID='TxtQtyPlan' CssClass="form-control" TextMode="Number" Style="width: 70px;" Text='<%# Eval("QtyPlan") %>' runat="server"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Time" ItemStyle-CssClass="CustomTxtTimePlan">
                        <ItemTemplate>
                            <asp:TextBox ID="TxtTimePlan" CssClass="form-control" style="width: 70px;" Text='<%# Eval("TimePlan") %>' runat="server"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manage" ItemStyle-CssClass="form-inline">
                        <ItemTemplate>
                            <asp:Button ID="BtnSave" CssClass="btn btn-xs btn-primary" Style="width: 50px;" CommandArgument='<%# Container.DataItemIndex + ","+ Eval("DeliveryPlanID") %>' OnCommand="BtnSave_Command" Text="บันทึก" runat="server" />
                            <asp:Button ID="BtnDelete" CssClass="btn btn-xs btn-danger" Style="width: 40px;" CommandArgument='<%# Eval("DeliveryPlanID") %>' OnCommand="BtnDelete_Command" Text="ลบ" runat="server" OnClientClick="if (!confirm('Are you sure you want delete?')) return false;" />
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
    <script type="text/javascript">
        $(document).ready(function () {
            $('#ContentPlaceHolder1_TxtTimePlanAdd, .CustomTxtTimePlan input').datetimepicker({
                format: 'HH:mm',
            });
        })
    </script>
</asp:Content>
