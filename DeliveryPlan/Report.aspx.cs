using ClosedXML.Excel;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class Report : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        System.Globalization.CultureInfo _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime Today = DateTime.Today;
                TxtDateStart.Text = Today.ToString("01-MM-yyyy", _curCulture);
                TxtDateEnd.Text = DateTime.Parse(TxtDateStart.Text).AddDays(2).ToString("dd-MM-yyyy");
                TxtDateMonth.Text = Today.ToString("MM-yyyy", _curCulture);

                // DropDownList Customer Gridview
                sql = "SELECT CompanyID, CompanyName FROM CRM_Company WHERE Status=1 ORDER BY CompanyName ASC";
                DDListCustomerSort.DataSource = query.SelectTableTCTV1(sql);
                DDListCustomerSort.DataValueField = "CompanyID";
                DDListCustomerSort.DataTextField = "CompanyName";
                DDListCustomerSort.DataBind();
                DDListCustomerSort.SelectedValue = "0";

                Session["daily"] = true;
                Session["monthly"] = false;
                this.BindGrid();
            }
        }

        //------------------------------------ Gridview ------------------------------------
        private void BindGrid()
        {
            string MonthYear = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM");
            int CustID = int.Parse(DDListCustomerSort.SelectedValue);
            string dateSelectStart = "";
            string dateSelectEnd = "";
            string dateSelectMonth = "";
            string Where = "";
            if ((bool)Session["daily"])
            {
                dateSelectStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                dateSelectEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                Where = "DP_DeliveryPlan.PlanMonthYear = '" + MonthYear + "' AND DP_DeliveryPlanDetail.PlanDate BETWEEN '" + dateSelectStart + "' AND '" + dateSelectEnd + "'";
            }
            else if ((bool)Session["monthly"])
            {
                dateSelectMonth = DateTime.Parse(TxtDateMonth.Text).ToString("yyyy-MM");
                Where = "DP_DeliveryPlan.PlanMonthYear = '" + dateSelectMonth + "' AND DP_DeliveryPlanDetail.PlanDate like '" + dateSelectMonth + "%'";
            }
            if (CustID == 0)
            {
                sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE " + Where + " GROUP BY DP_DeliveryPlan.DeliveryPlanID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlan.PlanMonthYear, FG.ImagePart ORDER BY DP_DeliveryPlan.PlanMonthYear ASC";
            }
            else
            {
                sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE CRM_Company.CompanyID = '" + CustID + "' AND " + Where + " GROUP BY DP_DeliveryPlan.DeliveryPlanID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlan.PlanMonthYear, FG.ImagePart ORDER BY DP_DeliveryPlan.PlanMonthYear ASC";
            }
            GVDeliveryPlan.DataSource = query.SelectTable(sql);
            GVDeliveryPlan.DataBind();

            GVDeliveryPlanDetail.DataSource = query.SelectTable(sql);
            GVDeliveryPlanDetail.DataBind();

            // Tab
            if (Session["daily"] != null && Session["monthly"] != null)
            {
                if ((bool)Session["daily"])
                {
                    LiReportMonth.Attributes.Add("class", "");
                    ReportMonth.Attributes.Add("class", "tab-pane w-60 ");
                    LiReportDay.Attributes.Add("class", "active");
                    ReportDay.Attributes.Add("class", "tab-pane w-60 active");
                }
                else
                {
                    LiReportDay.Attributes.Add("class", "");
                    ReportDay.Attributes.Add("class", "tab-pane w-60");
                    LiReportMonth.Attributes.Add("class", "active");
                    ReportMonth.Attributes.Add("class", "tab-pane w-60 active");
                }
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
                left.Text = "Date :";
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
                TableCell cell;
                TableCell right;
                GridViewRow rowHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                if ((bool)Session["daily"])
                {
                    // แสดงวันที่
                    string dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                    string dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                    sql = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                    DataTable dtDate = query.SelectTable(sql);
                    int columnNum = 1;
                    for (DateTime dateStart = DateTime.Parse(TxtDateStart.Text); dateStart <= DateTime.Parse(TxtDateEnd.Text); dateStart = dateStart.AddDays(1.0))
                    {
                        cell = new TableHeaderCell();
                        cell.Text = "Plan";
                        cell.Attributes.Add("class", "text-center");
                        e.Row.Cells.Add(cell);
                        cell = new TableHeaderCell();
                        cell.Text = "Actual";
                        cell.Attributes.Add("class", "text-center");
                        e.Row.Cells.Add(cell);
                        cell = new TableHeaderCell();
                        cell.Text = "Diff";
                        cell.Attributes.Add("class", "text-center");
                        e.Row.Cells.Add(cell);
                        columnNum++;
                    }
                    cell = new TableHeaderCell();
                    cell.Text = "Plan";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "Actual";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "Diff";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);

                    columnNum = 1;
                    for (DateTime dateStart = DateTime.Parse(TxtDateStart.Text); dateStart <= DateTime.Parse(TxtDateEnd.Text); dateStart = dateStart.AddDays(1.0))
                    {
                        right = new TableHeaderCell();
                        right.ColumnSpan = 3;
                        right.Text = (DateTime.Parse(dateStart.ToString())).Day.ToString() + "/" + (DateTime.Parse(dateStart.ToString())).Month.ToString();
                        right.Style.Add("border-top", "1px solid #808080 !important");
                        rowHeader.Cells.Add(right);
                        columnNum++;
                    }
                    right = new TableHeaderCell();
                    right.Text = "Total";
                    right.ColumnSpan = 3;
                    right.Style.Add("vertical-align", "middle");
                    right.Style.Add("border-top", "1px solid #808080 !important");
                    right.Attributes.Add("class", "text-center custom");
                    rowHeader.Cells.Add(right);
                    ((Table)GVDeliveryPlanDetail.Controls[0]).Rows.AddAt(0, rowHeader);
                }
                else if ((bool)Session["monthly"])
                {
                    cell = new TableHeaderCell();
                    cell.Text = "Plan";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "Actual";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);
                    cell = new TableHeaderCell();
                    cell.Text = "Diff";
                    cell.Attributes.Add("class", "text-center");
                    e.Row.Cells.Add(cell);

                    right = new TableHeaderCell();
                    right.Text = "Total";
                    right.ColumnSpan = 3;
                    right.Style.Add("vertical-align", "middle");
                    right.Style.Add("border-top", "1px solid #808080 !important");
                    right.Attributes.Add("class", "text-center custom");
                    rowHeader.Cells.Add(right);
                    ((Table)GVDeliveryPlanDetail.Controls[0]).Rows.AddAt(0, rowHeader);
                }
            }

            // Row
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;
                TableCell cell;
                string DeliveryPlanID = dr["DeliveryPlanID"].ToString();
                if ((bool)Session["daily"])
                {
                    string dateSelectDetailStart = DateTime.Parse(TxtDateStart.Text).ToString("yyyy-MM-dd");
                    string dateSelectDetailEnd = DateTime.Parse(TxtDateEnd.Text).ToString("yyyy-MM-dd");
                    int TotalQtyPlan = 0;
                    int TotalQtyActual = 0;
                    int TotalQtyDiff = 0;

                    // วันที่ ที่มีใน DeliveryPlanID นั้น
                    string sqlDate = "SELECT PlanDate FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' GROUP BY PlanDate ORDER BY PlanDate ASC";
                    DataTable dtDate = query.SelectTable(sqlDate);
                    // ข้อมูลในตาราง DP_DeliveryPlanDetail
                    sql = "SELECT PlanDate, SUM(QtyPlan) As QtyPlan, SUM(QtyActual) As QtyActual, (SUM(QtyActual)-SUM(QtyPlan)) As Diff FROM DP_DeliveryPlanDetail WHERE PlanDate BETWEEN '" + dateSelectDetailStart + "' AND '" + dateSelectDetailEnd + "' AND DeliveryPlanID = " + DeliveryPlanID + " GROUP BY PlanDate";
                    DataTable dtDetail = query.SelectTable(sql);
                    int columnNum = 1;
                    // วนเอาวันที่ออกมาทั้งหมด
                    for (DateTime dateStart = DateTime.Parse(TxtDateStart.Text); dateStart <= DateTime.Parse(TxtDateEnd.Text); dateStart = dateStart.AddDays(1.0))
                    {
                        // ค้นหาว่า DeliveryPlanIDDetail ไหนตรงกับวันที่ dateStart ต้องแปลงเป็น พ.ศ. ซะด้วย
                        string PlanDate = dateStart.AddYears(543).ToString();
                        DataRow[] foundRow = dtDetail.Select("PlanDate = '" + PlanDate + "'");
                        // ถ้าเจอ DeliveryPlanIDDetail ในวันนั้น ก็ให้ทำในนี้
                        if (foundRow.Length > 0)
                        {
                            // เอาข้อมูลใน DP_DeliveryPlanDetail ออกมาตามวันที่นั้น
                            TotalQtyPlan += foundRow[0].Field<int>("QtyPlan");
                            TotalQtyActual += foundRow[0].Field<int>("QtyActual");
                            // สร้าง cell ให้ตรงกับวันที่นั้น สร้าง 3 cell (เลื่อนขึ้นไปข้างบนจะเจอว่าสร้างวันที่มาก่อนแล้ว)
                            cell = new TableCell();
                            cell.Text = foundRow[0].Field<int>("QtyPlan").ToString();
                            cell.CssClass = "text-center";
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            cell.Style.Add("color", "black");
                            e.Row.Cells.Add(cell);

                            cell = new TableCell();
                            cell.Text = foundRow[0].Field<int>("QtyActual").ToString();
                            cell.CssClass = "text-center";
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            cell.Style.Add("color", "black");
                            e.Row.Cells.Add(cell);

                            cell = new TableCell();
                            cell.Text = foundRow[0].Field<int>("Diff").ToString();
                            cell.Style.Add("font-weight", "bold");
                            cell.CssClass = "text-center " + (foundRow[0].Field<int>("QtyPlan") == foundRow[0].Field<int>("QtyActual") ? "txt-success" : (foundRow[0].Field<int>("QtyPlan") > foundRow[0].Field<int>("QtyActual")) ? "txt-danger" : "txt-warning");
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            cell.Style.Add("color", "black");
                            e.Row.Cells.Add(cell);
                        }
                        else
                        {
                            cell = new TableCell();
                            cell.Text = "-";
                            cell.CssClass = "text-center";
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            e.Row.Cells.Add(cell);
                            cell = new TableCell();
                            cell.Text = "-";
                            cell.CssClass = "text-center";
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            e.Row.Cells.Add(cell);
                            cell = new TableCell();
                            cell.Text = "-";
                            cell.CssClass = "text-center";
                            cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                            e.Row.Cells.Add(cell);
                        }
                        columnNum++;
                    }
                    TotalQtyDiff = TotalQtyActual - TotalQtyPlan;
                    cell = new TableCell();
                    cell.Text = TotalQtyPlan.ToString();
                    cell.CssClass = "text-center";
                    cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = TotalQtyActual.ToString();
                    cell.CssClass = "text-center";
                    cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = TotalQtyDiff.ToString();
                    cell.CssClass = "text-center " + (TotalQtyPlan == TotalQtyActual ? "txt-success" : TotalQtyPlan > TotalQtyActual ? "txt-danger" : "txt-warning");
                    cell.Style.Add("background", (columnNum % 2 == 0 ? "#cfcfcf" : "white"));
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                }
                else if ((bool)Session["monthly"])
                {
                    string dateSelectMonth = DateTime.Parse(TxtDateMonth.Text).ToString("yyyy-MM");
                    sql = "SELECT SUM(QtyPlan) As QtyPlan, SUM(QtyActual) As QtyActual FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + DeliveryPlanID + " AND PlanDate like '" + dateSelectMonth + "%'";
                    int QtyPlan = int.Parse(query.SelectAt(0, sql));
                    int QtyActual = int.Parse(query.SelectAt(1, sql));
                    int QtyDiff = QtyActual - QtyPlan;
                    cell = new TableCell();
                    cell.Text = QtyPlan.ToString();
                    cell.CssClass = "text-center";
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = QtyActual.ToString();
                    cell.CssClass = "text-center";
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                    cell = new TableCell();
                    cell.Text = QtyDiff.ToString();
                    cell.CssClass = "text-center " + (QtyPlan == QtyActual ? "txt-success" : QtyPlan > QtyActual ? "txt-danger" : "txt-warning");
                    cell.Style.Add("color", "black");
                    e.Row.Cells.Add(cell);
                }
            }
        }

        // ปุ่มเลือกวันที่ก่อนหน้า
        protected void BtnPrDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDateMonth.Text);
            TxtDateMonth.Text = date.AddMonths(-1).ToString("MM-yyyy");
            Session["daily"] = false;
            Session["monthly"] = true;
            this.BindGrid();
        }

        // ปุ่มเลือกวันที่ถัดไป
        protected void BtnNextDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDateMonth.Text);
            TxtDateMonth.Text = date.AddMonths(1).ToString("MM-yyyy");
            Session["daily"] = false;
            Session["monthly"] = true;
            this.BindGrid();
        }

        //------------------------------------ DropDownList ------------------------------------

        // เลือกแสดงลูกค้า
        protected void DDListCustomerSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        //------------------------------------ Button ------------------------------------

        // ปุ่มแสดง Gridview ตามวันที่
        protected void SelectDate(object sender, EventArgs e)
        {
            // ที่ต้องทำแบบนี้เพราะว่าไม่อยากให้เขาเลือกเกิน 1 เดือน ให้ทำงานเป็นเดือนๆ ไป
            if (DateTime.Parse(TxtDateStart.Text).Month != DateTime.Parse(TxtDateEnd.Text).Month || DateTime.Parse(TxtDateStart.Text).Year != DateTime.Parse(TxtDateEnd.Text).Year)
            {
                TxtDateEnd.Text = TxtDateStart.Text;
            }
            Session["monthly"] = false;
            Session["daily"] = true;
            this.BindGrid();
        }

        // ปุ่มแสดง Gridview ตามเดือน
        protected void SelectDateMonth(object sender, EventArgs e)
        {
            Session["daily"] = false;
            Session["monthly"] = true;
            this.BindGrid();
        }

        // ปุ่ม Export to Excel
        protected void BtnExport_Click(object sender, EventArgs e)
        {
            this.BindGrid();
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.xls");
            Response.Charset = "";
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            sw.Write("<table border='1' style='white-space: nowrap;'>");
            sw.Write("<tr><td>");
            ((Table)GVDeliveryPlan.Controls[0]).Rows[0].Cells[0].ColumnSpan = 7;
            GVDeliveryPlan.Columns[6].Visible = false;
            GVDeliveryPlan.RenderControl(hw);
            sw.Write("</td><td>");
            GVDeliveryPlanDetail.RenderControl(hw);
            sw.Write("</td></tr></table>");
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        // แก้ Error Export to Excel
        public override void VerifyRenderingInServerForm(Control control)
        {
            // required to avoid the runtime error "  
            // Control 'GridView1' of type 'GridView' must be placed inside a form tag with runat=server."  
        }
    }
}