using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class AddDeliveryPlanMulti : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        // ตัวแปลสำหรับเช็ค NOT IN ใน GVDeliveryPlan_RowDataBound
        List<int> ListDeliveryPlanDetailID = new List<int>();
        // ตัวแปลสำหรับเช็ควัน ที่ไม่ได้เลือก ตรง CheckBox
        List<int> ListCBLDay = new List<int>();
        // ปกติมันจะได้ค่าเป็น พศ. แต่ผมอยากได้ คศ. ก็เลยใช้อันนี้
        System.Globalization.CultureInfo _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        // ความยาวหน้าจอ
        int widthScreen = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["PermissID"] == null || Session["DepartmentID"] == null || Session["Name"] == null)
            {
                if (IsPostBack)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertWarning('Session หมดเวลาแล้ว', 'login');", true);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertWarning('กรุณาเข้าสู่ระบบ', 'login');", true);
                }
                return;
            }
            else
            {
                int PermissID = 0;
                int DepartmentID = 0;
                if (Session["PermissID"] != null && Session["DepartmentID"] != null)
                {
                    PermissID = int.Parse(Session["PermissID"].ToString());
                    DepartmentID = int.Parse(Session["DepartmentID"].ToString());
                    if ((PermissID != 119 && PermissID != 121) && DepartmentID != 1)
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertWarning('ไม่มีสิทธ์เข้าใช้งานหน้านี้!!!', 'back'); ", true);
                    }
                }
            }

            if (Session["Width"] != null)
            {
                widthScreen = int.Parse(Session["Width"].ToString());
            }

            if (IsPostBack)
            {
                DDListProject.Style.Add("border", "1px solid #ccc");
                DDListPart.Style.Add("border", "1px solid #ccc");
                DDListFG.Style.Add("border", "1px solid #ccc");
                DDListTransport.Style.Add("border", "1px solid #ccc");
            }
            else
            {
                // วันที่ของ Add Form
                DateTime Today = DateTime.Today;
                TxtDateStartAdd.Text = Today.AddDays(1).ToString("dd-MM-yyyy", _curCulture);
                TxtDateEndAdd.Text = Today.AddDays(1).ToString("dd-MM-yyyy", _curCulture);

                // วันที่ของ Add Form
                TxtDateScan.Text = Today.AddDays(1).ToString("dd-MM-yyyy", _curCulture);

                // วันที่ของตาราง
                TxtDateStart.Text = Today.AddDays(-1).ToString("dd-MM-yyyy", _curCulture);
                TxtDateEnd.Text = Today.ToString("dd-MM-yyyy", _curCulture);
                TxtDateDaily.Text = Today.ToString("dd-MM-yyyy", _curCulture);

                DDListDataBindProject();
                DDListDataBindTransport();
                DDListDataBindCustomerSort();
            }
            // ถ้าไม่ให้มัน DataBind ทุกครั้ง มันทำงานไม่ได้ ข้อมูลใน gridview มันหาย **ปวดหัวเลยหล่ะ
            // https://morioh.com/p/6ea26d437844 เว็บนี้เขาบอกนะ
            this.BindGrid();
        }

        //------------------------------------ Gridview ------------------------------------

        // แสดงข้อมูลใน Gridview
        private void BindGrid()
        {
            int CustID = int.Parse(DDListCustomerSort.SelectedItem.Value);
            string dateSelectStart = "";
            string dateSelectEnd = "";
            string MonthYear = "";
            string dateSelectDaily = "";
            // ขนาดจอเล็กกว่า 1280
            if (widthScreen < 1280 || (bool)Session["Device"])
            {
                columnDateStart.Visible = false;
                columnDateEnd.Visible = false;
                columnTxtDateStart.Visible = false;
                columnTxtDateEnd.Visible = false;
                columnDaily.Visible = true;
                columnTxtDateDaily.Visible = true;
                dateSelectDaily = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM-dd");
                MonthYear = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM");
            }
            // ขนาดจอใหญ่กว่า 1280
            else
            {
                dateSelectStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                dateSelectEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                MonthYear = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM");
            }
            // สร้างตาราง DeliveryPlan ใหม่
            DataTable dtDeliveryPlanNew = new DataTable();
            dtDeliveryPlanNew.Columns.Add("DeliveryPlanID");
            dtDeliveryPlanNew.Columns.Add("CompanyName");
            dtDeliveryPlanNew.Columns.Add("ProjectName");
            dtDeliveryPlanNew.Columns.Add("CustomerCode");
            dtDeliveryPlanNew.Columns.Add("FGName");
            dtDeliveryPlanNew.Columns.Add("PartName");
            dtDeliveryPlanNew.Columns.Add("ImagePart");
            dtDeliveryPlanNew.Columns.Add("TransportName");
            // ดึงตาราง DP_DeliveryPlan
            sql = "SELECT DeliveryPlanID FROM DP_DeliveryPlan WHERE PlanMonthYear = '" + MonthYear + "'";
            DataTable dtDeliveryPlan = query.SelectTable(sql);
            // ลูปตาราง DP_DeliveryPlan
            foreach (DataRow RowDeliveryPlan in dtDeliveryPlan.Rows)
            {
                string sqlDeliveryPlanNew = "SELECT DP_DeliveryPlan.DeliveryPlanID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, FG.FGID, FG.ImagePart FROM DP_DeliveryPlanDetail LEFT JOIN DP_DeliveryPlan ON DP_DeliveryPlanDetail.DeliveryPlanID = DP_DeliveryPlan.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID";
                string sqlDeliveryPlanDetail;
                // DropDownList Customer ของ GridView เป็น All หรือไม่
                if (CustID == 0)
                {
                    // หาและนับวันที่ ที่มากที่สุด จะได้เอามาลูปแสดง DP_DeliveryPlan
                    if (widthScreen < 1280 || (bool)Session["Device"])
                    {
                        // WHERE
                        sqlDeliveryPlanNew += " WHERE DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "' AND DP_DeliveryPlan.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND DP_DeliveryPlanDetail.PlanDate = '" + dateSelectDaily + "'";
                        sqlDeliveryPlanDetail = "SELECT TOP 1 COUNT(PlanDate) FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND PlanDate = '" + dateSelectDaily + "' GROUP BY PlanDate ORDER BY COUNT(PlanDate) DESC";
                    }
                    else
                    {
                        sqlDeliveryPlanNew += " WHERE DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "' AND DP_DeliveryPlan.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND DP_DeliveryPlanDetail.PlanDate BETWEEN '" + dateSelectStart + "' AND '" + dateSelectEnd + "'";
                        sqlDeliveryPlanDetail = "SELECT TOP 1 COUNT(PlanDate) FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND PlanDate BETWEEN '" + dateSelectStart + "' AND '" + dateSelectEnd + "' GROUP BY PlanDate ORDER BY COUNT(PlanDate) DESC";
                    }
                }
                else
                {
                    // หาและนับวันที่ ที่มากที่สุด จะได้เอามาวนลูปแสดง DP_DeliveryPlan
                    if (widthScreen < 1280 || (bool)Session["Device"])
                    {
                        // WHERE
                        sqlDeliveryPlanNew += " WHERE DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "' AND DP_DeliveryPlan.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND CRM_Company.CompanyID = " + CustID + " AND DP_DeliveryPlanDetail.PlanDate = '" + dateSelectDaily + "'";
                        sqlDeliveryPlanDetail = "SELECT TOP 1 COUNT(PlanDate) FROM DP_DeliveryPlanDetail LEFT JOIN DP_DeliveryPlan ON DP_DeliveryPlanDetail.DeliveryPlanID = DP_DeliveryPlan.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID WHERE DP_DeliveryPlanDetail.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND PlanDate = '" + dateSelectDaily + "' AND CRM_Company.CompanyID = " + CustID + " GROUP BY PlanDate ORDER BY COUNT(PlanDate) DESC";
                    }
                    else
                    {
                        // WHERE
                        sqlDeliveryPlanNew += " WHERE DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "' AND DP_DeliveryPlan.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND CRM_Company.CompanyID = " + CustID + " AND DP_DeliveryPlanDetail.PlanDate BETWEEN '" + dateSelectStart + "' AND '" + dateSelectEnd + "'";
                        sqlDeliveryPlanDetail = "SELECT TOP 1 COUNT(PlanDate) FROM DP_DeliveryPlanDetail LEFT JOIN DP_DeliveryPlan ON DP_DeliveryPlanDetail.DeliveryPlanID = DP_DeliveryPlan.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID WHERE DP_DeliveryPlanDetail.DeliveryPlanID = " + RowDeliveryPlan["DeliveryPlanID"] + " AND PlanDate BETWEEN '" + dateSelectStart + "' AND '" + dateSelectEnd + "' AND CRM_Company.CompanyID = " + CustID + " GROUP BY PlanDate ORDER BY COUNT(PlanDate) DESC";
                    }
                }
                DataTable dtDeliveryPlanQuery = query.SelectTable(sqlDeliveryPlanNew);
                // จำนวนวันที่ ที่มีวันมากที่สุด (ใน 1 วันมีหลายเวลา)
                int QtyDateMaxInDate = int.Parse(query.SelectAt(0, sqlDeliveryPlanDetail));
                // เพิ่มข้อมูลในตาราง DeliveryPlan อันใหม่
                for (int loop = 1; loop <= QtyDateMaxInDate; loop++)
                {
                    DataRow drDeliveryPlanNew = dtDeliveryPlanNew.NewRow();
                    drDeliveryPlanNew["DeliveryPlanID"] = dtDeliveryPlanQuery.Rows[0]["DeliveryPlanID"];
                    drDeliveryPlanNew["CompanyName"] = dtDeliveryPlanQuery.Rows[0]["CompanyName"];
                    drDeliveryPlanNew["ProjectName"] = dtDeliveryPlanQuery.Rows[0]["ProjectName"];
                    drDeliveryPlanNew["CustomerCode"] = dtDeliveryPlanQuery.Rows[0]["CustomerCode"];
                    drDeliveryPlanNew["FGName"] = dtDeliveryPlanQuery.Rows[0]["FGName"];
                    drDeliveryPlanNew["PartName"] = dtDeliveryPlanQuery.Rows[0]["PartName"];
                    drDeliveryPlanNew["ImagePart"] = dtDeliveryPlanQuery.Rows[0]["ImagePart"];
                    drDeliveryPlanNew["TransportName"] = dtDeliveryPlanQuery.Rows[0]["TransportName"];
                    dtDeliveryPlanNew.Rows.Add(drDeliveryPlanNew);
                }
            }

            GVDeliveryPlan.DataSource = dtDeliveryPlanNew;
            GVDeliveryPlan.DataBind();

            GVDeliveryPlanDetail.DataSource = dtDeliveryPlanNew;
            GVDeliveryPlanDetail.DataBind();

            ListDeliveryPlanDetailID.Clear();
            if (BtnEditAll.Visible == false)
            {
                BtnEditAll_Click(null, null);
            }
        }
        protected void GVDeliveryPlan_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Header
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridViewRow rowHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell left = new TableHeaderCell();
                left.ColumnSpan = 8;
                left.Text = "วันที่ :";
                left.Style.Add("text-align", "right !important");
                left.Style.Add("border-top", "1px solid #808080 !important");
                rowHeader.Cells.Add(left);
                ((Table)GVDeliveryPlan.Controls[0]).Rows.AddAt(0, rowHeader);
            }
        }

        protected void GVDeliveryPlanDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Header
            if (e.Row.RowType == DataControlRowType.Header)
            {
                // Column [Q'ty] [Time] ตามจำนวนวันที่ และ [Plan] ท้ายสุด
                TableCell cell;
                string dateSelectDetailStart = "";
                string dateSelectDetailEnd = "";
                string dateSelectDetailDaily = "";
                if (widthScreen < 1280)
                {
                    dateSelectDetailDaily = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM-dd");
                    sql = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                else
                {
                    dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                    dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                    sql = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                DataTable dtDate = query.SelectTable(sql);
                foreach (DataRow rowDate in dtDate.Rows)
                {
                    cell = new TableHeaderCell();
                    cell.Text = "Q'ty";
                    cell.CssClass = "text-center";
                    cell.Style.Add("width", "50px");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "Time";
                    cell.CssClass = "text-center";
                    cell.Style.Add("width", "50px");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "หมายเหตุ";
                    cell.CssClass = "text-center";
                    cell.Style.Add("width", "205px");
                    e.Row.Cells.Add(cell);
                }
                cell = new TableHeaderCell();
                cell.Text = "Plan";
                cell.CssClass = "text-center";
                cell.Style.Add("width", "35px");
                e.Row.Cells.Add(cell);


                // สร้าง Column [วันที่] ไว้ใช้กับข้างบน และ [Total][Manage] ท้ายสุด
                GridViewRow rowHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);

                TableCell right;
                foreach (DataRow rowDate in dtDate.Rows)
                {
                    right = new TableHeaderCell();
                    right.ColumnSpan = 3;
                    right.Text = (DateTime.Parse(rowDate["PlanDate"].ToString())).Day.ToString();
                    right.Style.Add("border-top", "1px solid #808080 !important");
                    rowHeader.Cells.Add(right);
                }
                right = new TableHeaderCell();
                right.ColumnSpan = 1;
                right.Text = "Total";
                right.Style.Add("border-top", "1px solid #808080 !important");
                rowHeader.Cells.Add(right);
                right = new TableHeaderCell();
                right.Text = "จัดการ";
                right.RowSpan = 2;
                right.Style.Add("vertical-align", "middle");
                right.Style.Add("border-top", "1px solid #808080 !important");
                right.Style.Add("max-width", "100px");
                right.CssClass = "text-center custom";
                rowHeader.Cells.Add(right);
                ((Table)GVDeliveryPlanDetail.Controls[0]).Rows.AddAt(0, rowHeader);
            }

            // Row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // อันนี้คือสามารถเอาค่าใน Gridview มาใช้ได้
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;

                TableCell cell;
                string DeliveryPlanID = dr["DeliveryPlanID"].ToString();
                string dateSelectDetailStart = "";
                string dateSelectDetailEnd = "";
                string dateSelectDetailDaily = "";
                int TotalQtyPlan = 0;

                string sqlDate = "";
                if (widthScreen < 1280)
                {
                    dateSelectDetailDaily = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM-dd");
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                else
                {
                    dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                    dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                    // ดึงวันที่ ที่มีใน DP_DeliveryPlanDetail เช่น ถ้าเลือกวันที่ 1-7 แต่ในตารางไม่มี วันที่ 5 ก็จะได้ 1-4, 6-7
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                DataTable dtDate = query.SelectTable(sqlDate);
                if (widthScreen < 1280)
                {
                    // ไม่ต้องมี NOT IN ก็ได้มั้ง
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' AND DeliveryPlanID = " + DeliveryPlanID + " AND DeliveryPlanDetailID NOT IN ('" + String.Join("', '", ListDeliveryPlanDetailID) + "') ORDER BY TimePlan ASC, PlanDate ASC";
                }
                else
                {
                    // ข้อมูลในตาราง DP_DeliveryPlanDetail และเช็คว่ามี DeliveryPlanDetailID ในตัวแปร ListDeliveryPlanDetailID หรือยัง ถ้ามีแล้วข้อมูลจะไม่ถูกนำมาใช้ เหตุผล เพราะ (ใน 1 วันมีหลายเวลาจึงจำเป็นต้องตัดเวลาที่แสดงแล้ว ทิ้งไป)
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' AND DeliveryPlanID = " + DeliveryPlanID + " AND DeliveryPlanDetailID NOT IN ('" + String.Join("', '", ListDeliveryPlanDetailID) + "') ORDER BY TimePlan ASC, PlanDate ASC";
                }
                DataTable dtDetail = query.SelectTable(sql);
                // ตัวแปร columnNum ใช้สำหรับเช็คเงื่อนไข คู่ คี่ ไว้กำหนดสีของ column
                int columnNum = 1;
                // ลูปเอาวันที่ออกมาทั้งหมด
                foreach (DataRow rowDate in dtDate.Rows)
                {
                    // ค้นหาว่า DeliveryPlanDetailID ไหนตรงกับวันที่ rowDate["PlanDate"]
                    DataRow[] foundRow = dtDetail.Select("PlanDate = '" + rowDate["PlanDate"] + "'");
                    // ถ้าเจอ DeliveryPlanDetailID ในวันนั้น ก็ให้ทำใน IF นี้
                    if (foundRow.Length > 0)
                    {
                        // เอาข้อมูลใน DP_DeliveryPlanDetail ออกมาตามวันที่นั้น
                        DataRow rowDetails = foundRow.First();
                        TotalQtyPlan += int.Parse(rowDetails["QtyPlan"].ToString());
                        // สร้าง cell และสร้าง TextBox ให้ตรงกับวันที่นั้น สร้าง 3 cell 3 textbox (เลื่อนขึ้นไปข้างบนตรง Header จะเจอว่าสร้างวันที่มาก่อนแล้ว)
                        TextBox txtQty = new TextBox(); // TextBox Q'ty
                        txtQty.ID = "Qty" + rowDetails["DeliveryPlanDetailID"];
                        txtQty.Text = rowDetails["QtyPlan"].ToString();
                        txtQty.CssClass = "form-control";
                        txtQty.Style.Add("width", "50px");
                        txtQty.Style.Add("margin", "auto");
                        txtQty.Style.Add("background", "transparent");
                        txtQty.Style.Add("border", "none");
                        txtQty.Enabled = false;
                        txtQty.TextMode = TextBoxMode.Number;
                        cell = new TableCell();
                        cell.CssClass = "text-center";
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.Controls.Add(txtQty);
                        e.Row.Cells.Add(cell);

                        TextBox txtTime = new TextBox(); // TextBox Time
                        txtTime.ID = "Time" + rowDetails["DeliveryPlanDetailID"];
                        txtTime.Text = rowDetails["TimePlan"].ToString();
                        txtTime.CssClass = "form-control timepicker";
                        txtTime.Style.Add("width", "50px");
                        txtTime.Style.Add("margin", "auto");
                        txtTime.Style.Add("background", "transparent");
                        txtTime.Style.Add("border", "none");
                        txtTime.Enabled = false;
                        cell = new TableCell();
                        cell.CssClass = "CustomTxtTimePlan";
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.Controls.Add(txtTime);
                        e.Row.Cells.Add(cell);

                        TextBox txtRemarkPlanGV = new TextBox(); // TextBox Remark
                        txtRemarkPlanGV.ID = "RemarkPlan" + rowDetails["DeliveryPlanDetailID"];
                        txtRemarkPlanGV.Text = rowDetails["RemarkPlan"].ToString();
                        txtRemarkPlanGV.CssClass = "form-control";
                        txtRemarkPlanGV.Style.Add("min-width", "150px");
                        txtRemarkPlanGV.Style.Add("text-align", "left");
                        txtRemarkPlanGV.Style.Add("background", "transparent");
                        txtRemarkPlanGV.Style.Add("border", "none");
                        txtRemarkPlanGV.Style.Add("color", "black");
                        txtRemarkPlanGV.Attributes.Add("title", rowDetails["RemarkPlan"].ToString());
                        txtRemarkPlanGV.Enabled = false;
                        txtRemarkPlanGV.TextMode = TextBoxMode.MultiLine;
                        txtRemarkPlanGV.Rows = 2;
                        txtRemarkPlanGV.Height = 49;
                        cell = new TableCell();
                        cell.CssClass = "font-family-prompt";
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.ToolTip = rowDetails["RemarkPlan"].ToString();
                        cell.Controls.Add(txtRemarkPlanGV);
                        e.Row.Cells.Add(cell);

                        // เพิ่ม DeliveryPlanDetailID ใน ListDeliveryPlanDetailID
                        ListDeliveryPlanDetailID.Add(int.Parse(rowDetails["DeliveryPlanDetailID"].ToString()));
                    }
                    else
                    {
                        // ถ้าไม่มีข้อมูล DP_DeliveryPlanDetail ในวันนั้น ให้ทำตรงนี้ สร้าง cell เปล่าๆ 2 cell
                        cell = new TableCell(); // ช่อง Q'ty
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.Style.Add("vertical-align", "middle");
                        cell.Style.Add("text-align", "center");
                        cell.Text = "-";
                        e.Row.Cells.Add(cell);
                        cell = new TableCell(); // ช่อง Time
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.Style.Add("vertical-align", "middle");
                        cell.Style.Add("text-align", "center");
                        cell.Text = "-";
                        e.Row.Cells.Add(cell);
                        cell = new TableCell(); // ช่อง Remark
                        cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                        cell.Style.Add("vertical-align", "middle");
                        cell.Style.Add("text-align", "center");
                        cell.Text = "-";
                        e.Row.Cells.Add(cell);
                    }
                    columnNum++;
                }

                // สร้าง column จำนวนรวมของ QtyPlan
                cell = new TableCell();
                cell.Text = TotalQtyPlan.ToString();
                cell.CssClass = "text-center";
                cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                cell.Style.Add("vertical-align", "middle");
                e.Row.Cells.Add(cell);

                // สร้างปุ่มลบและปุ่มเลือก
                // เปลี่ยนจากปุ่มแก้ไข เป็นปุ่มเลือก ให้มันเพิ่มวันที่หรือเวลาใหม่ และแก้ไข DP_DeliveryPlan อันเดิมได้
                Button BtnEdit = new Button();
                BtnEdit.CommandName = "Edit";
                BtnEdit.CommandArgument = DeliveryPlanID;
                BtnEdit.Text = "เลือก";
                BtnEdit.CssClass = "btn btn-warning";
                BtnEdit.Style.Add("margin-right", "2px");
                BtnEdit.Width = 50;

                // ปุ่มลบ
                Button BtnDelete = new Button();
                BtnDelete.CommandName = "Delete";
                BtnDelete.CommandArgument = DeliveryPlanID;
                BtnDelete.OnClientClick = "return AlertConfirm(this);";
                BtnDelete.Text = "ลบ";
                BtnDelete.CssClass = "btn btn-danger-pastel";
                BtnDelete.Style.Add("margin-right", "2px");
                BtnDelete.Width = 50;

                columnNum++;
                cell = new TableCell();
                cell.Style.Add("vertical-align", "middle");
                cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                cell.Style.Add("max-width", "100px");
                cell.Attributes.Add("class", "text-center");
                e.Row.Cells.Add(cell);
                cell.Controls.Add(BtnEdit);
                cell.Controls.Add(BtnDelete);
            }
        }

        protected void GVDeliveryPlanDetail_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // เอาค่าแถวใน Gridview มาใช้หา Textbox
            GridViewRow gvr = (GridViewRow)((Button)e.CommandSource).NamingContainer;
            int RowIndex = gvr.RowIndex;
            string DeliveryPlanID = e.CommandArgument.ToString();

            // มันคือปุ่ม Select นี่แหละ
            if (e.CommandName == "Edit")
            {
                // แสดงปุ่ม Update ตรง Form เพิ่มข้อมูล
                BtnUpdate.Visible = true;
                string MonthYear = "";
                if (widthScreen < 1280)
                {
                    MonthYear = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM");
                }
                else
                {
                    MonthYear = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM");
                }
                string sqlSelect = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_DeliveryPlan.CustID, Project.ProjectID, Part.PartID, DP_DeliveryPlan.FGID, DP_DeliveryPlan.TransportID, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID WHERE DP_DeliveryPlan.DeliveryPlanID = " + DeliveryPlanID + " AND DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "'";
                HFDeliveryPlanID.Value = query.SelectAt(0, sqlSelect);
                //DDListCustomer.SelectedValue = query.SelectAt(1, sqlSelect);
                DDListProject.SelectedValue = query.SelectAt(2, sqlSelect);

                // DropDownList Part
                OnSelectIndexChangedDDListProject(DDListProject, EventArgs.Empty);
                DDListPart.SelectedValue = query.SelectAt(3, sqlSelect);

                // DropDownList FG
                OnSelectIndexChangedDDListPart(DDListPart, EventArgs.Empty);
                DDListFG.SelectedValue = query.SelectAt(4, sqlSelect);

                sql = "SELECT CustomerCode, CompanyName FROM FG LEFT JOIN [TCTV1].[dbo].[CRM_Company] ON FG.Cus_ID = CRM_Company.CompanyID WHERE FGID = " + DDListFG.SelectedValue;
                string IDCode = query.SelectAt(0, sql);
                string CompanyName = query.SelectAt(1, sql);
                LbIDCode.Text = IDCode;
                LBCustomer.Text = CompanyName;
                DDListTransport.SelectedValue = query.SelectAt(5, sqlSelect);

                string ImagePart = query.SelectAt(6, sqlSelect);
                if (ImagePart == "")
                {
                    if (Request.Url.Host != "110.77.148.173")
                    {
                        ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg".Replace("110.77.148.173:2027", "192.168.0.9"); // ใช้ลิงค์ภายใน เผื่อในกรณีที่เน็ตหลุด (ใช้ Replace แล้วช้า ไม่ต้องโชว์ซะเลย)
                    }
                    else
                    {
                        ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg";
                    }
                }
                else
                {
                    if (Request.Url.Host != "110.77.148.173")
                    {
                        ImgPart.ImageUrl = ImagePart.Replace("110.77.148.173:2027", "192.168.0.9"); // ใช้ลิงค์ภายใน เผื่อในกรณีที่เน็ตหลุด
                    }
                    else
                    {
                        ImgPart.ImageUrl = ImagePart;
                    }
                }
                GVDeliveryPlanDetail.Rows[RowIndex].CssClass = "info";
            }
            // ปุ่มลบ
            else if (e.CommandName == "Delete")
            {
                if (TxtDateManage.Text != "")
                {
                    string dateSelectDetail = DateTime.Parse(TxtDateManage.Text).ToString("yyyy-MM-dd");
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetail + "' AND DeliveryPlanID = " + DeliveryPlanID + " ORDER BY PlanDate ASC";
                    DataTable dtDetail = query.SelectTable(sql);
                    if (dtDetail.Rows.Count > 0)
                    {
                        foreach (DataRow rowData in dtDetail.Rows)
                        {
                            string DeliveryPlanDetailID = rowData["DeliveryPlanDetailID"].ToString();
                            TextBox TxtQty;
                            TextBox TxtTime;
                            TextBox TxtRemarkPlan;
                            TxtQty = (TextBox)GVDeliveryPlanDetail.Rows[RowIndex].FindControl("Qty" + DeliveryPlanDetailID);
                            TxtTime = (TextBox)GVDeliveryPlanDetail.Rows[RowIndex].FindControl("Time" + DeliveryPlanDetailID);
                            TxtRemarkPlan = (TextBox)GVDeliveryPlanDetail.Rows[RowIndex].FindControl("RemarkPlan" + DeliveryPlanDetailID);
                            if (TxtQty != null && TxtTime != null && TxtRemarkPlan != null)
                            {
                                sql = "DELETE FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                                query.Excute(sql);
                                sql = "DELETE FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + DeliveryPlanID + " AND PlanDate = '" + dateSelectDetail + "' AND DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                                if (query.Excute(sql))
                                {
                                    // นับว่ามี DeliveryPlanID เหลือใน DP_DeliveryPlanDetail ไหม ถ้าไม่เหลือแล้วให้ลบ DeliveryPlanID ใน DP_DeliveryPlan ด้วย
                                    sql = "SELECT COUNT(*) FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + DeliveryPlanID;
                                    if (int.Parse(query.SelectAt(0, sql)) == 0)
                                    {
                                        sql = "DELETE FROM DP_DeliveryPlan WHERE DeliveryPlanID = " + DeliveryPlanID;
                                        query.Excute(sql);
                                    }
                                    this.BindGrid();
                                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertSuccess('ลบข้อมูลสำเร็จ'); ", true);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertError('ลบข้อมูลไม่สำเร็จ'); ", true);
                    }
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือกวันที่ ที่ต้องการลบ!'); ", true);
                }
            }
        }

        protected void GVDeliveryPlanDetail_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // แก้ Error
        }

        protected void GVDeliveryPlanDetail_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // แก้ Error
        }

        //------------------------------------ DropDownList ------------------------------------

        //เลือก Project
        protected void OnSelectIndexChangedDDListProject(object sender, EventArgs e)
        {
            LbIDCode.Text = "";
            LBCustomer.Text = "";
            // DropDownList Part
            DDListPart.Items.Clear();
            DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
            DDListPart.Items[0].Attributes["disabled"] = "disabled";
            DDListPart.SelectedValue = "0";
            sql = "SELECT PartID, PartName FROM Part WHERE ProjectID = " + DDListProject.SelectedValue + " AND Status=1 ORDER BY PartName ASC";
            DDListPart.DataSource = query.SelectTable(sql);
            DDListPart.DataValueField = "PartID";
            DDListPart.DataTextField = "PartName";
            DDListPart.DataBind();
        }

        //เลือก Part
        protected void OnSelectIndexChangedDDListPart(object sender, EventArgs e)
        {
            LbIDCode.Text = "";
            LBCustomer.Text = "";
            // DropDownList FG
            DDListFG.Items.Clear();
            DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
            DDListFG.Items[0].Attributes["disabled"] = "disabled";
            DDListFG.SelectedValue = "0";
            sql = "SELECT FGID, FGName FROM FG WHERE PartID = " + DDListPart.SelectedValue + " AND Status=1 ORDER BY FGName ASC";
            DDListFG.DataSource = query.SelectTable(sql);
            DDListFG.DataValueField = "FGID";
            DDListFG.DataTextField = "FGName";
            DDListFG.DataBind();
        }

        //เลือก FG
        protected void OnSelectIndexChangedDDListFG(object sender, EventArgs e)
        {
            sql = "SELECT CustomerCode, ImagePart, CompanyName FROM FG LEFT JOIN [TCTV1].[dbo].[CRM_Company] ON FG.Cus_ID = CRM_Company.CompanyID WHERE FGID = " + DDListFG.SelectedValue;
            string IDCode = query.SelectAt(0, sql);
            string ImagePart = query.SelectAt(1, sql);
            string CompanyName = query.SelectAt(2, sql);
            if (ImagePart == "")
            {
                if (Request.Url.Host != "110.77.148.173")
                {
                    ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg".Replace("110.77.148.173:2027", "192.168.0.9"); // ใช้ลิงค์ภายใน เผื่อในกรณีที่เน็ตหลุด
                }
                else
                {
                    ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg";
                }
            }
            else
            {
                if (Request.Url.Host != "110.77.148.173")
                {
                    ImgPart.ImageUrl = ImagePart.Replace("110.77.148.173:2027", "192.168.0.9"); // ใช้ลิงค์ภายใน เผื่อในกรณีที่เน็ตหลุด
                }
                else
                {
                    ImgPart.ImageUrl = ImagePart;
                }
            }
            LbIDCode.Text = IDCode;
            LBCustomer.Text = CompanyName;
        }

        // สำหรับแสดง Customer ใน Gridview
        protected void DDListCustomerSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        protected void DDListDataBindCustomerSort()
        {
            // DropDownList CustomerSort Gridview
            sql = "SELECT CompanyID, CompanyName FROM CRM_Company WHERE status = 1 ORDER BY CompanyName";
            DDListCustomerSort.DataSource = query.SelectTableTCTV1(sql);
            DDListCustomerSort.DataValueField = "CompanyID";
            DDListCustomerSort.DataTextField = "CompanyName";
            DDListCustomerSort.DataBind();
            DDListCustomerSort.SelectedValue = "0";
        }

        // แสดงข้อมูลใน DropDownList
        protected void DDListDataBindProject()
        {
            // DropDownList Project
            sql = "SELECT ProjectID, ProjectName FROM Project WHERE Status=1 ORDER BY ProjectName ASC";
            DDListProject.DataSource = query.SelectTable(sql);
            DDListProject.DataValueField = "ProjectID";
            DDListProject.DataTextField = "ProjectName";
            DDListProject.DataBind();
        }

        protected void DDListDataBindTransport()
        {
            // DropDownList Transport
            sql = "SELECT TransportID, TransportName FROM DP_Transport WHERE Status=1 ORDER BY TransportName ASC";
            DataTable dtTransport = query.SelectTable(sql);
            DDListTransport.DataSource = dtTransport;
            DDListTransport.DataValueField = "TransportID";
            DDListTransport.DataTextField = "TransportName";
            DDListTransport.DataBind();
            DDListTransport.SelectedValue = "1";

            // DropDownList Transport Scan
            DDListTransportScan.DataSource = dtTransport;
            DDListTransportScan.DataValueField = "TransportID";
            DDListTransportScan.DataTextField = "TransportName";
            DDListTransportScan.DataBind();
            DDListTransportScan.SelectedValue = "1";
        }

        //------------------------------------ Button ------------------------------------

        // ปุ่มยกเลิกของ Form เพิ่มข้อมูล
        protected void BtnCancle_Click(object sender, EventArgs e)
        {
            // แสดงปุ่มเพิ่มข้อมูล
            BtnAdd.Visible = true;
            // ซ่อนปุ่มอัพเดต
            BtnUpdate.Visible = false;
            TxtQty.Text = "0";
            TxtTimePlanAdd.Text = "00:00";
            LbIDCode.Text = "";
            LBCustomer.Text = "";
            if (Request.Url.Host != "110.77.148.173")
            {
                ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg".Replace("110.77.148.173:2027", "192.168.0.9"); // ใช้ลิงค์ภายใน เผื่อในกรณีที่เน็ตหลุด
            }
            else
            {
                ImgPart.ImageUrl = "http://110.77.148.173:2027/PartPicture/NoImg.jpg";
            }
            //ล้างค่าใน DropDownList
            DDListPart.Items.Clear();
            DDListFG.Items.Clear();
            //เพิ่ม itam ที่ 0 และกำหนด disabled
            DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
            DDListPart.Items[0].Attributes["disabled"] = "disabled";
            DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
            DDListFG.Items[0].Attributes["disabled"] = "disabled";
            //เลือกที่ index 0
            //DDListCustomer.SelectedValue = "0";
            DDListProject.SelectedValue = "0";
            DDListPart.SelectedValue = "0";
            DDListFG.SelectedValue = "0";
            DDListTransport.SelectedValue = "1";
            //เอาข้อมูลใน Database มาแสดงใน DropDownList
            DDListDataBindProject();
        }

        // ปุ่มแสดงวันที่
        protected void SelectDate(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        // ปุ่มเพิ่มข้อมูลใน From เพิ่มข้อมูล
        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            int UserID = int.Parse(Session["UserID"].ToString());
            int ProjectID = int.Parse(DDListProject.SelectedValue);
            int PartID = int.Parse(DDListPart.SelectedValue);
            int FGID = int.Parse(DDListFG.SelectedValue);
            int TransportID = int.Parse(DDListTransport.SelectedValue);
            int QtyPlan = int.Parse(TxtQty.Text);
            string TimePlan = TxtTimePlanAdd.Text;
            string RemarkPlan = TxtRemarkPlan.Text;
            string MonthYear = DateTime.Parse(TxtDateStartAdd.Text).ToString("yyyy-MM");
            string dateSelectStart = DateTime.Parse(TxtDateStartAdd.Text).ToString("yyyy-MM-dd");
            string dateSelectEnd = DateTime.Parse(TxtDateEndAdd.Text).ToString("yyyy-MM-dd");
            if (ProjectID != 0)
            {
                if (PartID != 0)
                {
                    if (FGID != 0)
                    {
                        if (TransportID != 0)
                        {
                            sql = "SELECT Cus_ID FROM FG WHERE FGID = " + FGID;
                            string CustID = query.SelectAt(0, sql);
                            if (CustID != "")
                            {
                                if (DateTime.Parse(TxtDateStartAdd.Text) > DateTime.Parse(TxtDateEndAdd.Text))
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือกวันที่ให้ถูกต้อง')", true);
                                }
                                else if (DateTime.Parse(TxtDateStartAdd.Text).Month != DateTime.Parse(TxtDateEndAdd.Text).Month || DateTime.Parse(TxtDateStartAdd.Text).Year != DateTime.Parse(TxtDateEndAdd.Text).Year)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือกเดือนและปีให้อยู่ในเดือนและปีเดียวกัน')", true);
                                }
                                else
                                {
                                    bool checkInsert = false;
                                    // เก็บค่าวันที่เลือก เช่น จันทร์ พุธ ศุกร์
                                    foreach (ListItem items in CBLDay.Items)
                                    {
                                        if (items.Selected)
                                        {
                                            ListCBLDay.Add(int.Parse(items.Value));
                                        }
                                    }
                                    for (DateTime checkDate = DateTime.Parse(dateSelectStart); checkDate <= DateTime.Parse(dateSelectEnd); checkDate = checkDate.AddDays(1.0))
                                    {
                                        // เช็ควัน ที่เลือก
                                        if (ListCBLDay.Contains((int)checkDate.DayOfWeek))
                                        {
                                            string dateSelect = (checkDate).ToString("yyyy-MM-dd");
                                            //เช็คว่ามีข้อมูลในตาราง DP_DeliveryPlan และมีเดือนปีนั้นหรือไม่ ถ้าไม่มีก็เพิ่มข้อมูลเข้าไป
                                            sql = "SELECT DeliveryPlanID FROM DP_DeliveryPlan WHERE FGID = " + FGID + " AND CustID = " + CustID + " AND TransportID = " + TransportID + " AND PlanMonthYear = '" + MonthYear + "'";
                                            if (!query.CheckRow(sql))
                                            {
                                                //Insert DP_DeliveryPlan
                                                string sqlDP_DeliveryPlan = "INSERT INTO DP_DeliveryPlan (CustID, FGID, TransportID, UserIDCreate, PlanMonthYear) VALUES (" + CustID + ", " + FGID + ", " + TransportID + ", " + UserID + ", '" + MonthYear + "')";
                                                query.Excute(sqlDP_DeliveryPlan);
                                            }
                                            // Insert DP_DeliveryPlanDetail
                                            int DeliveryPlanID = int.Parse(query.SelectAt(0, sql));
                                            sql = "INSERT INTO DP_DeliveryPlanDetail (DeliveryPlanID, UserIDCreate, TimePlan, QtyPlan, PlanDate, RemarkPlan, PlanDateCreate, StatusID) VALUES (" + DeliveryPlanID + ", " + UserID + ",'" + TimePlan + "', " + QtyPlan + ", '" + dateSelect + "', '" + RemarkPlan + "', GETDATE(), 4)";
                                            if (query.Excute(sql))
                                            {
                                                checkInsert = true;
                                                TxtQty.Text = "0";
                                                TxtTimePlanAdd.Text = "00:00";
                                                TxtRemarkPlan.Text = "";
                                            }
                                        }
                                    }
                                    if (checkInsert)
                                    {
                                        this.BindGrid();
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertSuccess('บันทึกข้อมูลสำเร็จ')", true);
                                    }
                                    else
                                    {
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertError('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
                                    }
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertTopEndWarning('ไม่มีข้อมูล Customer กรุณาอัพเดตข้อมูล Part')", true);
                            }
                        }
                        else
                        {
                            DDListTransport.Style.Add("border", "1px solid red");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Transport')", true);
                        }
                    }
                    else
                    {
                        DDListFG.Style.Add("border", "1px solid red");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก FG')", true);
                    }
                }
                else
                {
                    DDListPart.Style.Add("border", "1px solid red");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Part')", true);
                }
            }
            else
            {
                DDListProject.Style.Add("border", "1px solid red");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Project')", true);
            }
        }

        // ปุ่มอัพเดตข้อมูลใน Form เพิ่มข้อมูล
        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            string DeliveryPlanID = HFDeliveryPlanID.Value;
            int UserID = int.Parse(Session["UserID"].ToString());
            int ProjectID = int.Parse(DDListProject.SelectedValue);
            int PartID = int.Parse(DDListPart.SelectedValue);
            int FGID = int.Parse(DDListFG.SelectedValue);
            int TransportID = int.Parse(DDListTransport.SelectedValue);
            if (ProjectID != 0)
            {
                if (PartID != 0)
                {
                    if (FGID != 0)
                    {
                        if (TransportID != 0)
                        {
                            sql = "SELECT Cus_ID FROM FG WHERE FGID = " + FGID;
                            int CustID = int.Parse(query.SelectAt(0, sql));
                            sql = "UPDATE DP_DeliveryPlan SET CustID = " + CustID + ", FGID = " + FGID + ", TransportID = " + TransportID + ", UserIDUpdate = " + UserID + " WHERE DeliveryPlanID = " + DeliveryPlanID;
                            if (query.Excute(sql))
                            {
                                BtnUpdate.Visible = false;
                                LbIDCode.Text = "";
                                LBCustomer.Text = "";
                                //ล้างค่าใน DropDownList
                                DDListPart.Items.Clear();
                                DDListFG.Items.Clear();
                                //เพิ่ม itam ที่ 0 และกำหนด disabled
                                DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
                                DDListPart.Items[0].Attributes["disabled"] = "disabled";
                                DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
                                DDListFG.Items[0].Attributes["disabled"] = "disabled";
                                //เลือกที่ index 0
                                DDListProject.SelectedValue = "0";
                                DDListPart.SelectedValue = "0";
                                DDListFG.SelectedValue = "0";
                                DDListTransport.SelectedValue = "1";
                                //เอาข้อมูลใน Database มาสร้าง item
                                DDListDataBindProject();
                                this.BindGrid();
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertSuccess('บันทึกข้อมูลสำเร็จ')", true);
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertError('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
                            }
                        }
                        else
                        {
                            DDListTransport.Style.Add("border", "1px solid red");
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Transport')", true);
                        }
                    }
                    else
                    {
                        DDListFG.Style.Add("border", "1px solid red");
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก FG')", true);
                    }
                }
                else
                {
                    DDListPart.Style.Add("border", "1px solid red");
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Part')", true);
                }
            }
            else
            {
                DDListProject.Style.Add("border", "1px solid red");
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือก Project')", true);
            }
        }

        // ปุ่มบันทึกข้อมูลใน Gridview
        protected void BtnSaveAll_Click(object sender, EventArgs e)
        {
            bool checkResult = false;
            foreach (GridViewRow row in GVDeliveryPlanDetail.Rows)
            {
                int index = row.RowIndex;
                string DeliveryPlanID = ((GVDeliveryPlanDetail.Rows[index].FindControl("HFGVDeliveryPlanDetailID") as HiddenField).Value).Split(',')[0];
                string dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                string dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                string dateSelectDetailDaily = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM-dd");
                string sqlDate = "";
                if (widthScreen < 1280)
                {
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                else
                {
                    // วันที่ ที่มีใน DeliveryPlanID นั้น
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                DataTable dtDate = query.SelectTable(sqlDate);
                if (widthScreen < 1280)
                {
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' AND DeliveryPlanID = " + DeliveryPlanID + " ORDER BY PlanDate ASC";
                }
                else
                {
                    // ข้อมูลในตาราง DP_DeliveryPlanDetail
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' AND DeliveryPlanID = " + DeliveryPlanID + " ORDER BY PlanDate ASC";
                }
                DataTable dtDetail = query.SelectTable(sql);
                // ลูปเอาวันที่ออกมาทั้งหมด
                foreach (DataRow rowDate in dtDate.Rows)
                {
                    // ค้นหาว่า DeliveryPlanIDDetail ไหนตรงกับวันที่ rowDate
                    DataRow[] foundRow = dtDetail.Select("PlanDate = '" + rowDate["PlanDate"] + "'");
                    // ถ้าเจอ DeliveryPlanIDDetail ในวันนั้น ก็ให้ทำในนี้
                    if (foundRow.Length > 0)
                    {
                        // เอาขูลมูล Row นั้นออกมาใช้ค้นหา TextBox
                        foreach (DataRow rows in foundRow)
                        {
                            TextBox TxtQty = GVDeliveryPlanDetail.Rows[index].FindControl("Qty" + rows["DeliveryPlanDetailID"]) as TextBox;
                            TextBox TxtTime = GVDeliveryPlanDetail.Rows[index].FindControl("Time" + rows["DeliveryPlanDetailID"]) as TextBox;
                            TextBox TxtRemarkPlan = GVDeliveryPlanDetail.Rows[index].FindControl("RemarkPlan" + rows["DeliveryPlanDetailID"]) as TextBox;
                            if (TxtQty != null && TxtTime != null && TxtRemarkPlan != null)
                            {
                                string QtyPlan = TxtQty.Text;
                                string TimePlan = TxtTime.Text;
                                string RemarkPlan = TxtRemarkPlan.Text;
                                string DeliveryPlanDetailID = rows["DeliveryPlanDetailID"].ToString();
                                sql = "UPDATE DP_DeliveryPlanDetail SET TimePlan = '" + TimePlan + "', QtyPlan = " + QtyPlan + ", RemarkPlan = '" + RemarkPlan + "', PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                                if (query.Excute(sql))
                                {
                                    checkResult = true;
                                }
                            }
                        }
                    }
                }
            }
            if (checkResult)
            {
                BtnEditAll.Visible = true;
                BtnSaveAll.Visible = false;
                BtnCanCelEdit.Visible = false;
                this.BindGrid();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertSuccess('บันทึกข้อมูลสำเร็จ')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertError('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
            }
        }

        // ปุ่มเปิด Textbox ใน Gridview
        protected void BtnEditAll_Click(object sender, EventArgs e)
        {
            BtnEditAll.Visible = false;
            BtnSaveAll.Visible = true;
            BtnCanCelEdit.Visible = true;
            // วนลูปแถวใน Gridview
            foreach (GridViewRow row in GVDeliveryPlanDetail.Rows)
            {
                int index = row.RowIndex;
                string DeliveryPlanID = ((GVDeliveryPlanDetail.Rows[index].FindControl("HFGVDeliveryPlanDetailID") as HiddenField).Value).Split(',')[0];
                string dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                string dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                string dateSelectDetailDaily = DateTime.Parse(TxtDateDaily.Text).ToString("yyyy-MM-dd");
                string sqlDate = "";
                if (widthScreen < 1280)
                {
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                else
                {
                    // ดึงวันที่ ที่มีใน DP_DeliveryPlanDetail
                    sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                }
                DataTable dtDate = query.SelectTable(sqlDate);
                if (widthScreen < 1280)
                {
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate = '" + dateSelectDetailDaily + "' AND DeliveryPlanID = " + DeliveryPlanID + " ORDER BY PlanDate ASC";
                }
                else
                {
                    // ดึงข้อมูลในตาราง DP_DeliveryPlanDetail
                    sql = "SELECT DeliveryPlanDetailID, PlanDate, QtyPlan, TimePlan, RemarkPlan FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' AND DeliveryPlanID = " + DeliveryPlanID + " ORDER BY PlanDate ASC";
                }
                DataTable dtDetail = query.SelectTable(sql);
                // วนลูปเอาวันที่ออกมา
                foreach (DataRow rowDate in dtDate.Rows)
                {
                    // ค้นหาว่า DP_DeliveryPlanDetail ไหนตรงกับวันที่ rowDate
                    DataRow[] foundRow = dtDetail.Select("PlanDate = '" + rowDate["PlanDate"] + "'");
                    // ถ้าเจอ DP_DeliveryPlanDetail ให้ทำใน IF นี้
                    if (foundRow.Length > 0)
                    {
                        foreach (DataRow rows in foundRow)
                        {
                            // ค้นหา Textbox และทำการ Enabled
                            TextBox TxtQty = GVDeliveryPlanDetail.Rows[index].FindControl("Qty" + rows["DeliveryPlanDetailID"]) as TextBox;
                            TextBox TxtTime = GVDeliveryPlanDetail.Rows[index].FindControl("Time" + rows["DeliveryPlanDetailID"]) as TextBox;
                            TextBox TxtRemarkPlan = GVDeliveryPlanDetail.Rows[index].FindControl("RemarkPlan" + rows["DeliveryPlanDetailID"]) as TextBox;
                            if (TxtQty is TextBox && TxtTime is TextBox && TxtRemarkPlan is TextBox)
                            {
                                TxtQty.Enabled = true;
                                TxtQty.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                                TxtQty.Style.Add("background-color", "rgba(51,122,183,0.1)");
                                TxtTime.Enabled = true;
                                TxtTime.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                                TxtTime.Style.Add("background-color", "rgba(51,122,183,0.1)");
                                TxtRemarkPlan.Enabled = true;
                                TxtRemarkPlan.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                                TxtRemarkPlan.Style.Add("background-color", "rgba(51,122,183,0.1)");
                            }
                        }
                    }
                }
            }
        }

        // ปุ่มยกเลิกการแก้ไขใน Gridview
        protected void BtnCanCelEdit_Click(object sender, EventArgs e)
        {
            BtnEditAll.Visible = true;
            BtnSaveAll.Visible = false;
            BtnCanCelEdit.Visible = false;
            this.BindGrid();
        }

        // Copy แผนการส่งของวันที่เลือก ไปลงของวันที่เลือก
        protected void btnCopy_Click(object sender, EventArgs e)
        {
            if (TxtDateCopyFrom.Text != "" && TxtDateCopyTo.Text != "")
            {
                string UserIDCreate = Session["UserID"].ToString();
                string DateCopyFrom = DateTime.Parse(TxtDateCopyFrom.Text).ToString("yyyy-MM-dd");
                string DateCopyTo = DateTime.Parse(TxtDateCopyTo.Text).ToString("yyyy-MM-dd");
                sql = "SELECT DP_DeliveryPlanDetail.* FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID WHERE DP_DeliveryPlanDetail.PlanDate = '" + DateCopyFrom + "'";
                DataTable dt = query.SelectTable(sql);
                foreach (DataRow Row in dt.Rows)
                {
                    string DeliveryPlanDetailID = Row["DeliveryPlanDetailID"].ToString();
                    //เช็คว่ามีข้อมูลในตาราง DP_DeliveryPlan และมีเดือนปีนั้นหรือไม่ ถ้าไม่มีก็เพิ่มข้อมูลเข้าไป
                    string sqlSelectDeliveryPlan = "SELECT DP_DeliveryPlan.FGID, DP_DeliveryPlan.CustID, DP_DeliveryPlan.TransportID FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID WHERE DP_DeliveryPlanDetail.PlanDate = '" + DateCopyFrom + "' AND DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    string FGID = query.SelectAt(0, sqlSelectDeliveryPlan);
                    string CustID = query.SelectAt(1, sqlSelectDeliveryPlan);
                    string TransportID = query.SelectAt(2, sqlSelectDeliveryPlan);
                    string MonthYear = DateTime.Parse(TxtDateCopyTo.Text).ToString("yyyy-MM");
                    string sqlCheckRowDeliveryPlan = "SELECT DeliveryPlanID FROM DP_DeliveryPlan WHERE FGID = " + FGID + " AND CustID = " + CustID + " AND TransportID = " + TransportID + " AND PlanMonthYear = '" + MonthYear + "'";
                    if (!query.CheckRow(sqlCheckRowDeliveryPlan))
                    {
                        //Insert DP_DeliveryPlan
                        string sqlDP_DeliveryPlan = "INSERT INTO DP_DeliveryPlan (CustID, FGID, TransportID, UserIDCreate, PlanMonthYear) VALUES (" + CustID + ", " + FGID + ", " + TransportID + ", " + UserIDCreate + ", '" + MonthYear + "')";
                        query.Excute(sqlDP_DeliveryPlan);
                    }

                    string DeliveryPlanID = query.SelectAt(0, sqlCheckRowDeliveryPlan);
                    string TimePlan = Row["TimePlan"].ToString();
                    string QtyPlan = Row["QtyPlan"].ToString();
                    string PlanDate = DateCopyTo;
                    string sqlInsert = "INSERT INTO DP_DeliveryPlanDetail (DeliveryPlanID, UserIDCreate, TimePlan, QtyPlan, PlanDate, PlanDateCreate, StatusID) VALUES (" + DeliveryPlanID + ", " + UserIDCreate + ", '" + TimePlan + "', '" + QtyPlan + "', '" + PlanDate + "', GETDATE(), 4)";
                    if (query.Excute(sqlInsert))
                    {
                        this.BindGrid();
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertSuccess('บันทึกข้อมูลสำเร็จ')", true);
                    }
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณาเลือกวันที่ !!')", true);
            }
        }

        //------------------------------------ Textbox ------------------------------------

        protected void TxtScanQRCode_TextChanged(object sender, EventArgs e)
        {
            string ScanQRCode = TxtScanQRCode.Text;
            string FGID = ScanQRCode;
            string CustID = "";
            string TransportID = "";
            string UserID = "";
            if (Session["UserID"] != null)
            {
                UserID = Session["UserID"].ToString();
                string MonthYearScan = DateTime.Parse(TxtDateScan.Text).ToString("yyyy-MM");
                string dateSelect = DateTime.Parse(TxtDateScan.Text).ToString("yyyy-MM-dd");
                sql = "SELECT FGID, Cus_ID FROM FG WHERE FGID = " + FGID + " AND Status = 1";
                if (query.CheckRow(sql))
                {
                    CustID = query.SelectAt(1, sql);
                    TransportID = DDListTransportScan.SelectedValue;
                    if (CustID != "")
                    {
                        sql = "SELECT DeliveryPlanID FROM DP_DeliveryPlan WHERE FGID = " + FGID + " AND CustID = " + CustID + " AND TransportID = " + TransportID + " AND PlanMonthYear = '" + MonthYearScan + "'";
                        if (!query.CheckRow(sql))
                        {
                            //Insert DP_DeliveryPlan
                            string sqlDP_DeliveryPlan = "INSERT INTO DP_DeliveryPlan (CustID, FGID, TransportID, UserIDCreate, PlanMonthYear) VALUES (" + CustID + ", " + FGID + ", " + TransportID + ", " + UserID + ", '" + MonthYearScan + "')";
                            query.Excute(sqlDP_DeliveryPlan);
                        }
                        // Insert DP_DeliveryPlanDetail
                        int DeliveryPlanID = int.Parse(query.SelectAt(0, sql));
                        sql = "INSERT INTO DP_DeliveryPlanDetail (DeliveryPlanID, UserIDCreate, TimePlan, QtyPlan, PlanDate, PlanDateCreate, StatusID) VALUES (" + DeliveryPlanID + ", " + UserID + ", '00:00', 0, '" + dateSelect + "', GETDATE(), 4)";
                        if (query.Excute(sql))
                        {
                            this.BindGrid();
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertTopEndSuccess('บันทึกข้อมูลสำเร็จ')", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertTopEndWarning('ไม่มีข้อมูล Customer กรุณาอัพเดตข้อมูล Part')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertTopEndError('ไม่พบข้อมูล!!!')", true);
                }
                //LiAddPlan.Attributes.Add("class", "");
                //AddPlan.Attributes.Add("class", "tab-pane");
                //LiScan.Attributes.Add("class", "active");
                //Scan.Attributes.Add("class", "tab-pane active");
                //LiCopyPlan.Attributes.Add("class", "");
                //CopyPlan.Attributes.Add("class", "tab-pane");
                TxtScanQRCode.Text = "";
                TxtScanQRCode.Focus();
            }
        }
    }
}