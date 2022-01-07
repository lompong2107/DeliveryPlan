<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="BarcodeScan.aspx.cs" Inherits="DeliveryPlan.BarcodeScan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .text-center-header th {
            text-align: center;
        }

        .table-custom {
            width: 100%;
        }

            .table-custom td, .table-custom th {
                height: auto !important;
            }

        html,
        body {
            height: 100%;
            width: 100%;
            overflow: auto;
        }
    </style>
    <div>
        <div class="container">
            <p class="text-center font-head">Barcode Scan</p>

            <hr />
            <div>
                <div>
                    <asp:Button ID="BtnBack" CssClass="btn btn-default btn-lg" Text="Back" runat="server" OnClientClick="javascript:window.location.href='EditDeliveryPlan.aspx'; return false;" />
                    <asp:Button ID="BtnRefresh" CssClass="btn btn-default btn-lg" Text="Refresh" runat="server" OnClick="BtnRefresh_Click" />
                </div>
            </div>
        </div>
        <div class="GridViewContainer">
            <asp:GridView ID="GVDeliveryPlan" runat="server" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data">
                <Columns>
                    <asp:TemplateField HeaderText="Customer">
                        <ItemTemplate>
                            <%# Eval("CompanyName") %>
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
                    <asp:TemplateField HeaderText="Date">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("PlanDate").ToString()).ToString("dd/MM/yyyy") %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Time">
                        <ItemTemplate>
                            <%# DateTime.Parse(Eval("TimePlan").ToString()).ToString("HH:mm") %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Plan">
                        <ItemTemplate>
                            <%# Eval("QtyPlan") %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Total">
                        <ItemTemplate>
                            <%# Eval("QtyTotal") %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Q'ty Stock">
                        <ItemTemplate>
                            <%# Eval("QtyStock") %>
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
                <div>
                    <asp:Button ID="BtnScanIn" CssClass="btn btn-primary btn-lg" runat="server" Text="Scan In" OnClick="BtnScanIn_Click" />
                    <asp:Button ID="BtnScanOut" CssClass="btn btn-default btn-lg" runat="server" Text="Scan Out" OnClick="BtnScanOut_Click" Visible="false" />
                    <asp:Button ID="BtnModal" runat="server" CssClass="btn btn-info btn-lg" Text="Create Barcode" OnClick="BtnModal_Click" />
                    <asp:Button ID="BtnPrintBarcode" CssClass="btn btn-success btn-lg" runat="server" Text="Print Barcode" OnClick="BtnPrintBarcode_Click" />
                </div>
                <div style="align-content: center; min-width: 300px;">
                    <div style="margin-bottom: 0px; width: 100%;" role="alert" id="DivAlert" runat="server" visible="false">
                        <asp:Label ID="LbAlert" runat="server"></asp:Label>
                    </div>
                </div>
                <div class="text-right form-inline">
                    <span class="h1" style="vertical-align: bottom; margin: 0;">
                        <asp:Label ID="LbScanTitle" runat="server" Text="Scan :"></asp:Label>
                    </span>
                    <asp:TextBox ID="TxtBarcodeScan" CssClass="form-control input-lg" runat="server" placeholder="Code Scan" OnTextChanged="TxtBarcodeScan_TextChanged" AutoPostBack="true"></asp:TextBox>
                    <asp:TextBox ID="TxtBarcodeScanIn" CssClass="form-control input-lg" runat="server" placeholder="Code Scan" OnTextChanged="TxtBarcodeScanIn_TextChanged" AutoPostBack="true" Visible="false"></asp:TextBox>
                </div>
            </div>
            <asp:GridView ID="GVScan" runat="server" CssClass="table table-custom" ShowHeaderWhenEmpty="true" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowCommand="GVScan_RowCommand" OnRowDataBound="GVScan_RowDataBound" OnRowDeleting="GVScan_RowDeleting">
                <Columns>
                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" Width="50" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Barcode" SortExpression="Barcode">
                        <ItemTemplate>
                            <%# Eval("BarcodeNumber") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <%# (int.Parse(Eval("Status").ToString()) == 1 ? "Sended" :  int.Parse(Eval("Status").ToString()) == 2 ? "Stock" : "Pending")  %>
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" Width="200" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Manage">
                        <ItemTemplate>
                            <asp:Button ID="BtnDelete" CssClass="btn btn-danger btn-xs" runat="server" CommandName="Delete" CommandArgument='<%# Eval("BarcodeID") %>' Text="Delete" OnClientClick="if (!confirm('Are you sure you want delete?')) return false;" />
                        </ItemTemplate>
                        <ItemStyle CssClass="text-center" Width="100" />
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
                <EmptyDataRowStyle HorizontalAlign="Center" />
            </asp:GridView>
        </div>

        <!-- Modal -->
        <div class="modal fade" id="ModalCreateBarcode" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">Create Barcode</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-inline">
                            <span class="h1" style="vertical-align: bottom;">Quantity :</span>
                            <asp:TextBox ID="TxtQtyBarcode" runat="server" TextMode="Number" CssClass="form-control input-lg"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default btn-lg" data-dismiss="modal">Close</button>
                        <asp:Button ID="BtnCreateBarcode" CssClass="btn btn-primary btn-lg" runat="server" Text="Create" OnClick="BtnCreateBarcode_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function HideLabelNow() {
            document.getElementById("<%=DivAlert.ClientID %>").style.display = "none";
        };

        function HideLabel() {
            var seconds = 5;
            setTimeout(function () {
                document.getElementById("<%=DivAlert.ClientID %>").style.display = "none";
            }, seconds * 1000);
        };
    </script>
</asp:Content>
