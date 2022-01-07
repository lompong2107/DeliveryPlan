<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="AddDeliveryPlanMulti.aspx.cs" Inherits="DeliveryPlan.AddDeliveryPlanMulti" %>

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
    <div class="container" style="position: relative;">
        <p class="text-center font-head">Delivery Plan</p>
        <%--<hr />--%>
        <button id="HideAddPlan" style="position: absolute; top: 0; right: 20px; z-index: 10;" class="btn btn-default btn-xs">แสดง / ซ่อน</button>
        <div id="addForm">
            <!-- Nav tabs -->
            <ul class="nav nav-tabs" role="tablist" style="margin-bottom: 10px;">
                <li role="presentation" id="LiAddPlan" runat="server" class="active"><a href="#<%=AddPlan.ClientID%>" aria-controls="AddPlan" role="tab" data-toggle="tab">เพิ่ม Plan</a></li>
                <li role="presentation" id="LiScan" runat="server"><a href="#<%=Scan.ClientID%>" aria-controls="Scan" role="tab" data-toggle="tab">สแกน QR Code</a></li>
                <li role="presentation" id="LiCopyPlan" runat="server"><a href="#<%=CopyPlan.ClientID%>" aria-controls="CopyPlan" role="tab" data-toggle="tab">คัดลอก Plan</a></li>
            </ul>

            <!-- Tab panes -->
            <div class="tab-content">
                <div role="tabpanel" class="tab-pane active" id="AddPlan" runat="server">
                    <%--เพิ่มข้อมูล--%>
                    <asp:HiddenField ID="HFDeliveryPlanID" runat="server" />
                    <%-- เอาไว้อ้างถึง DP_DeliveryPlan.DeliveryPlanID ตอน Update --%>
                    <table class="table-addForm" style="width: 100%;">
                        <tr>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>Project :</td>
                            <td colspan="3">
                                <asp:DropDownList ID="DDListProject" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListProject" AutoPostBack="true">
                                    <asp:ListItem Text="-- Select Project --" Value="0" Selected="True" disabled></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td style="width: 110px;" class="text-right">ตั้งแต่วันที่ :</td>
                            <td style="position: relative;" colspan="3">
                                <asp:TextBox ID="TxtDateStartAdd" CssClass="form-control datepicker datepickerStartAdd" runat="server"></asp:TextBox>
                            </td>
                            <td rowspan="5" style="border: 1px #eee solid; padding: 5px; width: 230px;" class="text-center">
                                <asp:Image ID="ImgPart" runat="server" Style="max-width: 210px; max-height: 180px;" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>Part :</td>
                            <td colspan="3">
                                <asp:DropDownList ID="DDListPart" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListPart" AutoPostBack="true">
                                    <asp:ListItem Text="-- Select Part --" Value="0" Selected="True" disabled></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="text-right">ถึงวันที่ :</td>
                            <td style="position: relative;" colspan="3">
                                <asp:TextBox ID="TxtDateEndAdd" CssClass="form-control datepicker datepickerEndAdd" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>FG :</td>
                            <td colspan="3">
                                <asp:DropDownList ID="DDListFG" CssClass="form-control" runat="server" AppendDataBoundItems="true" OnSelectedIndexChanged="OnSelectIndexChangedDDListFG" AutoPostBack="true">
                                    <asp:ListItem Text="-- Select FG --" Value="0" Selected="True" disabled></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td colspan="4" class="tableCheckbox">
                                <asp:CheckBoxList ID="CBLDay" runat="server" RepeatDirection="Horizontal" TextAlign="Left" CssClass="CBLDay">
                                    <asp:ListItem Value="4" Text="อา."></asp:ListItem>
                                    <asp:ListItem Value="5" Text="จ." Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="6" Text="อ." Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="0" Text="พ." Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="พฤ." Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="ศ." Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="ส." Selected="True"></asp:ListItem>
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>Transport :</td>
                            <td colspan="3">
                                <asp:DropDownList ID="DDListTransport" CssClass="form-control" runat="server" AppendDataBoundItems="true">
                                    <asp:ListItem Text="-- Select Transport --" Value="0" Selected="True" disabled></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td class="text-right">จำนวน :</td>
                            <td style="width: 110px;" class="form-inline">
                                <asp:TextBox ID="TxtQty" Style="width: 80px;" TextMode="Number" Text="0" CssClass="form-control" runat="server" min="0"></asp:TextBox>
                                <span style="vertical-align: middle;">ชิ้น</span>
                            </td>
                            <td class="text-right text-nowrap">เวลา :</td>
                            <td style="position: relative; width: 70px;">
                                <asp:TextBox ID="TxtTimePlanAdd" Style="width: 70px;" Text="00:00" CssClass="form-control timepicker" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="text-right">Customer :</td>
                            <td style="min-width: 100px;">
                                <asp:Label ID="LBCustomer" runat="server"></asp:Label>
                            </td>
                            <td class="text-right">ID Code :</td>
                            <td style="min-width: 100px;">
                                <asp:Label ID="LbIDCode" runat="server"></asp:Label>
                            </td>
                            <td class="text-right">
                                <p style="margin-bottom: 0px;">หมายเหตุ :</p>
                            </td>
                            <td colspan="3">
                                <asp:TextBox ID="TxtRemarkPlan" runat="server" CssClass="form-control"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <div class="form-inline">
                                    <asp:Button ID="BtnAdd" CssClass="btn btn-success" Width="100px" runat="server" Text="เพิ่ม" OnClick="BtnAdd_Click" />
                                    <asp:Button ID="BtnUpdate" CssClass="btn btn-primary" Width="100px" runat="server" Text="อัพเดต" OnClick="BtnUpdate_Click" Visible="false" />
                                    <asp:Button ID="BtnCancle" CssClass="btn btn-default" Width="60px" runat="server" Text="ยกเลิก" OnClick="BtnCancle_Click" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div role="tabpanel" class="tab-pane" id="Scan" runat="server">
                    <table class="table-addForm">
                        <tr>
                            <td style="white-space: nowrap;">
                                <p style="margin-bottom: 0px;">สแกน QR Code :</p>
                            </td>
                            <td>
                                <asp:TextBox ID="TxtScanQRCode" runat="server" CssClass="form-control" OnTextChanged="TxtScanQRCode_TextChanged" AutoPostBack="true" Enabled="true"></asp:TextBox>
                            </td>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>วันที่ :</td>
                            <td style="position: relative;">
                                <asp:TextBox ID="TxtDateScan" Style="width: 200px;" CssClass="form-control datepicker" runat="server" Enabled="true"></asp:TextBox>
                            </td>
                            <td style="width: 100px;" class="text-right"><span style="color: red;">* </span>Transport :</td>
                            <td>
                                <asp:DropDownList ID="DDListTransportScan" CssClass="form-control" runat="server" AppendDataBoundItems="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
                <div role="tabpanel" class="tab-pane" id="CopyPlan" runat="server">
                    <table class="table-addForm">
                        <tr>
                            <td style="white-space: nowrap;">
                                <p style="margin-bottom: 0px;">คัดลอก Plan จากวันที่ :</p>
                            </td>
                            <td style="position: relative;">
                                <asp:TextBox ID="TxtDateCopyFrom" Style="width: 200px;" CssClass="form-control datepicker" runat="server"></asp:TextBox>
                            </td>
                            <td style="width: 50px;" class="text-center">
                                <p style="margin-bottom: 0px;">ไปยัง</p>
                            </td>
                            <td style="position: relative;">
                                <asp:TextBox ID="TxtDateCopyTo" Style="width: 200px;" CssClass="form-control datepicker" runat="server"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnCopy" runat="server" Text="คัดลอก" CssClass="btn btn-primary" OnClick="btnCopy_Click" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <hr style="margin: 10px 0 10px 0" />
        <%--หมายเหตุกับปุ่มลบ--%>
        <div class="text-right">
            <strong>
                <asp:Label ID="LbRemark" runat="server" Text="**เลือก วัน/เดือน/ปี ที่ต้องการลบ" CssClass="text-danger"></asp:Label>
            </strong>
        </div>
        <div style="display: grid; grid-template-columns: auto auto;">
            <div>
                <table class="table-addForm" id="AddPlanMulti" runat="server" visible="true">
                    <tr>
                        <%-- หน้าจอใหญ่กว่าหรือเล็กกว่า 1280 จะสามารถดูได้แค่ 1 วัน --%>
                        <td style="width: 100px;" class="text-right" id="columnDateStart" runat="server">ตั้งแต่วันที่ :</td>
                        <td style="width: 100px; position: relative;" id="columnTxtDateStart" runat="server">
                            <asp:TextBox ID="TxtDateStart" CssClass="form-control datepicker datepickerStart" runat="server"></asp:TextBox>
                        </td>
                        <td style="width: 100px;" class="text-right" id="columnDateEnd" runat="server">ถึงวันที่ :</td>
                        <td style="width: 100px; position: relative;" id="columnTxtDateEnd" runat="server">
                            <asp:TextBox ID="TxtDateEnd" CssClass="form-control datepicker datepickerEnd" runat="server"></asp:TextBox>
                        </td>
                        <%-- หน้าจอใหญ่กว่าหรือเล็กกว่า 1280 จะสามารถดูได้แค่ 1 วัน --%>
                        <td style="width: 100px;" class="text-right" id="columnDaily" runat="server" visible="false">วันที่ :</td>
                        <td style="width: 100px; position: relative;" id="columnTxtDateDaily" runat="server" visible="false">
                            <asp:TextBox ID="TxtDateDaily" CssClass="form-control datepicker" runat="server"></asp:TextBox>
                        </td>
                        <td style="width: 100px;">
                            <asp:Button ID="BtnShow" runat="server" CssClass="btn btn-primary" Text="แสดง" OnClick="SelectDate" />
                        </td>
                        <td style="width: 100px;" class="text-right">Customer :</td>
                        <td>
                            <asp:DropDownList ID="DDListCustomerSort" Style="max-width: 200px;" OnSelectedIndexChanged="DDListCustomerSort_SelectedIndexChanged" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true">
                                <asp:ListItem Text="ทั้งหมด" Value="0" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <table class="table-addForm" style="float: right">
                    <tr>
                        <td class="text-right">
                            <asp:Button ID="BtnSaveAll" CssClass="btn btn-success" Text="บันทึก" runat="server" OnClick="BtnSaveAll_Click" Visible="false" />
                            <asp:Button ID="BtnEditAll" CssClass="btn btn-warning" Text="แก้ไข" runat="server" OnClick="BtnEditAll_Click" Visible="true" />
                            <asp:Button ID="BtnCanCelEdit" CssClass="btn btn-default" Text="ยกเลิก" runat="server" OnClick="BtnCanCelEdit_Click" Visible="false" />
                        </td>
                        <%--ใช้กับปุ่มลบ--%>
                        <td style="width: 100px;" class="text-right">วันที่ลบ :</td>
                        <td style="width: 100px; position: relative;">
                            <asp:TextBox ID="TxtDateManage" Width="100px" CssClass="form-control datepicker" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>

    <%--ตารางงงงงงงงงงงงงงงงง--%>
    <div class="GridViewContainer" style="width: 100%; display: inline-grid; grid-template-columns: auto auto; overflow-x: hidden; overflow-y: auto; max-height: 80vh;">
        <asp:GridView ID="GVDeliveryPlan" runat="server" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowDataBound="GVDeliveryPlan_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="#">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        <asp:HiddenField ID="HFGVDeliveryPlanID" Value='<%# Eval("DeliveryPlanID") %>' runat="server" />
                    </ItemTemplate>
                    <ItemStyle CssClass="text-center" />
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
                        <p class="overflow-hide" style="min-width: 150px;"><%# Eval("FGName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part Name">
                    <ItemTemplate>
                        <p class="overflow-hide" style="min-width: 230px;"><%# Eval("PartName") %></p>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Part Image">
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
        <asp:GridView ID="GVDeliveryPlanDetail" runat="server" CssClass="table table-custom" AutoGenerateColumns="false" EmptyDataText="Empty Data" OnRowDataBound="GVDeliveryPlanDetail_RowDataBound" OnRowCommand="GVDeliveryPlanDetail_RowCommand" OnRowEditing="GVDeliveryPlanDetail_RowEditing" OnRowDeleting="GVDeliveryPlanDetail_RowDeleting">
            <Columns>
                <asp:TemplateField Visible="false">
                    <ItemTemplate>
                        <asp:HiddenField ID="HFGVDeliveryPlanDetailID" Value='<%# Eval("DeliveryPlanID") %>' runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <HeaderStyle BackColor="#006699" CssClass="text-center-header" Font-Bold="True" ForeColor="White" />
            <EmptyDataRowStyle HorizontalAlign="Center" />
        </asp:GridView>
    </div>
</asp:Content>
