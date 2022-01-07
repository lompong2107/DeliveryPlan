<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="BarcodeDeliveryPlan (Close).aspx.cs" Inherits="DeliveryPlan.BarcodeDeliveryPlan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .text-center-header th {
            text-align: center;
        }

        .btn-danger-pastel {
            background-color: #FF6961 !important;
            color: white;
        }

            .btn-danger-pastel:hover {
                background-color: #e1554d !important;
                color: white;
            }

        .txt-danger {
            color: red !important;
        }

        .cssPager td {
            padding-left: 4px;
            padding-right: 4px;
            color: white;
        }

        .table-addForm tr > td {
            padding: 0 5px 5px 0;
        }

        .CustomTxtTimePlan div {
            top: 50% !important;
            left: 50% !important;
            position: fixed;
            transform: translate(-50%, -50%) !important;
            height: 200px !important;
            width: 250px !important;
        }

        .CustomTxtTimePlan {
            position: relative;
        }

        .table-custom {
            width: 100%;
            white-space: nowrap;
            font-size: 12px;
            color: black;
            font-weight: bold;
        }

            .table-custom td, .table-custom th {
                padding: 0 2px 0 2px !important;
                vertical-align: middle !important;
            }

            .table-custom tbody tr:nth-child(even) {
                background-color: white;
            }

            .table-custom tbody tr:nth-child(odd) {
                background-color: #cfcfcf;
            }

        .GridViewContainer {
            margin-top: 10px;
            margin-bottom: 10px;
        }
    </style>
    <div>
        <div class="container">
            <h2 class="text-center">
                <asp:Label ID="LbTitle" runat="server" Text="Barcode Delivery Plan"></asp:Label></h2>
            <hr />
            <%--ค่าที่ซ่อนไว้--%>
            <asp:HiddenField ID="HDFDeliveryPlanDetailID" runat="server" />
            <div style="display: grid; grid-template-columns: auto auto;">
                <div>
                    <asp:Button ID="BtnBack" CssClass="btn btn-default" Text="Back" runat="server" Visible="false" OnClick="BtnBack_Click" />
                    <table class="table-addForm" id="tableDate" runat="server">
                        <tr>
                            <td style="width: 100px;" class="text-right">Date :</td>
                            <td style="position: relative;" class="form-inline">
                                <asp:LinkButton ID="BtnPrDate" CssClass="btn btn-default" runat="server" OnClick="BtnPrDate_Click"><i class="glyphicon glyphicon-backward"></i></asp:LinkButton>
                                <asp:TextBox ID="TxtDate" CssClass="form-control datepicker datepickerStart" runat="server"></asp:TextBox>
                                <asp:LinkButton ID="BtnNextDate" CssClass="btn btn-default" runat="server" OnClick="BtnNextDate_Click"><i class="glyphicon glyphicon-forward"></i></asp:LinkButton>
                            </td>
                            <td>
                                <asp:Button ID="BtnShow" runat="server" CssClass="btn btn-primary" Text="Show" OnClick="SelectDate" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <table class="table-addForm" style="float: right">
                        <tr>
                            <td class="text-right">
                                <asp:Button ID="BtnConfirm" CssClass="btn btn-success" Text="Confirm" runat="server" Visible="false" Enabled="false" OnClick="BtnConfirm_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server">
                <%--ตารางงงงงงงงงงงงงงงงง--%>
                <div class="GridViewContainer">
                    <asp:GridView ID="GVDeliveryPlan" runat="server" CssClass="table table-custom" AllowSorting="true" OnSorting="GVDeliveryPlan_Sorting" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowCommand="GVDeliveryPlan_RowCommand">
                        <Columns>
                            <asp:TemplateField HeaderText="#">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" SortExpression="CustName">
                                <ItemTemplate>
                                    <%# Eval("CustName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Project" SortExpression="ProjectName">
                                <ItemTemplate>
                                    <%# Eval("ProjectName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ID Code" SortExpression="CustomerCode">
                                <ItemTemplate>
                                    <%# Eval("CustomerCode") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Part No." SortExpression="FGName">
                                <ItemTemplate>
                                    <%# Eval("FGName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Part Name" SortExpression="PartName">
                                <ItemTemplate>
                                    <%# Eval("PartName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Transport" SortExpression="TransportName">
                                <ItemTemplate>
                                    <%# Eval("TransportName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Time" SortExpression="TimePlan">
                                <ItemTemplate>
                                    <%# DateTime.Parse(Eval("TimePlan").ToString()).ToString("HH:mm") %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Q'ty">
                                <ItemTemplate>
                                    <%# Eval("QtyPlan") %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    Sent
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Manage">
                                <ItemTemplate>
                                    <asp:LinkButton ID="BtnScan" Style="padding: 0 5px;" CssClass="btn btn-xs btn-success" runat="server" CommandName="Scan" Text="Scan" CommandArgument='<%# Eval("DeliveryPlanDetailID") %>'></asp:LinkButton>
                                    <asp:LinkButton ID="BtnPrint" Style="padding: 0 5px;" CssClass="btn btn-xs btn-primary" runat="server" CommandName="Print" Text="Print" CommandArgument='<%# Eval("DeliveryPlanDetailID") %>'></asp:LinkButton>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
                        <EmptyDataRowStyle HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </asp:View>
            <asp:View ID="View2" runat="server">
                <div class="GridViewContainer">
                    <asp:GridView ID="GVDeliveryPlanInView2" runat="server" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data">
                        <Columns>
                            <asp:TemplateField HeaderText="Customer" SortExpression="CustName">
                                <ItemTemplate>
                                    <%# Eval("CustName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Project" SortExpression="ProjectName">
                                <ItemTemplate>
                                    <%# Eval("ProjectName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="ID Code" SortExpression="CustomerCode">
                                <ItemTemplate>
                                    <%# Eval("CustomerCode") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Part No." SortExpression="FGName">
                                <ItemTemplate>
                                    <%# Eval("FGName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Part Name" SortExpression="PartName">
                                <ItemTemplate>
                                    <%# Eval("PartName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Transport" SortExpression="TransportName">
                                <ItemTemplate>
                                    <%# Eval("TransportName") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Time" SortExpression="TimePlan">
                                <ItemTemplate>
                                    <%# DateTime.Parse(Eval("TimePlan").ToString()).ToString("HH:mm") %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Q'ty">
                                <ItemTemplate>
                                    <%# Eval("QtyPlan") %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Count Scan">
                                <ItemTemplate>
                                    <asp:Label ID="LbCountScan" runat="server" Text="0"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
                        <EmptyDataRowStyle HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
                <div class="container">
                    <div style="margin-bottom: 10px; display: grid; grid-template-columns: auto auto auto;">
                        <div style="align-self: self-end;">
                            <asp:Button ID="BtnCancel" CssClass="btn btn-danger btn-xs" Text="Cancel Scan" runat="server" Visible="false" OnClick="BtnCancel_Click" OnClientClick="if (!confirm('Are you sure you want cancel?')) return false;" />
                        </div>
                        <div style="align-content: center;">
                            <div style="margin-bottom: 0px; width: 100%;" role="alert" id="DivAlert" runat="server" visible="false">
                                <asp:Label ID="LbAlert" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="text-right form-inline">
                            <span>Scan: </span>
                            <asp:TextBox ID="TxtDeliveryPlanDetailIDScan" CssClass="form-control" runat="server" placeholder="Code Scan" OnTextChanged="TxtDeliveryPlanDetailIDScan_TextChanged" AutoPostBack="true"></asp:TextBox>
                        </div>
                    </div>
                    <asp:GridView ID="GVScan" runat="server" CssClass="table table-custom" ShowHeaderWhenEmpty="true" AutoGenerateColumns="false" EmptyDataText="Empty Data">
                        <Columns>
                            <asp:TemplateField HeaderText="#">
                                <ItemTemplate>
                                    <%# Container.DataItemIndex + 1 %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Barcode" SortExpression="Barcode">
                                <ItemTemplate>
                                    <%# Eval("BarcodeNumber") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <%# (int.Parse(Eval("Status").ToString()) == 0 ? "Pending" : "Confirmed")  %>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-center" />
                            </asp:TemplateField>
                        </Columns>
                        <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
                        <EmptyDataRowStyle HorizontalAlign="Center" />
                    </asp:GridView>
                </div>
            </asp:View>
        </asp:MultiView>
    </div>
    <script type="text/javascript">
        function HideLabel() {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=DivAlert.ClientID %>").style.display = "none";
                }, seconds * 1000);
        };

        $(document).ready(function () {
            $('.timepicker').datetimepicker({
                format: 'HH:mm',
            });

            $('.datepicker').datetimepicker({
                format: 'DD/MM/YYYY'
            });
        })
    </script>
</asp:Content>
