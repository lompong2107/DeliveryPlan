using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class ViewActual : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime Today = DateTime.Today;
                System.Globalization.CultureInfo _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                TxtDate.Text = Today.ToString("dd-MM-yyyy", _curCulture);
                DDListDataBind();
                this.BindGrid();
                this.BindGrid2();
            }

            //HttpRequest httpRequest = HttpContext.Current.Request;
            //if (httpRequest.Browser.IsMobileDevice == true)
            //{
            //    Label1.Text = "Device : Mobile";
            //    Session["Device"] = true;
            //} else
            //{
            //    Label1.Text = "Device : Not Mobile";
            //    Session["Device"] = false;
            //}
        }

        //------------------------------------ Gridview ------------------------------------

        // จัดเรียงข้อมูล
        private string SortDirection
        {
            get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        // แสดงข้อมูลใน Gridview
        protected void BindGrid(string sortExpression = null)
        {
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            int CustID = int.Parse(DDListCustomer.SelectedItem.Value);
            int CustID2 = int.Parse(DDListCustomer2.SelectedItem.Value);
            if (CustID == 0)
            {
                sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID AND DP_Barcode.Status != 3 LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND CRM_Company.CompanyID != " + CustID2 + " GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY CRM_Company.CompanyName ASC";
            }
            else
            {
                sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND CRM_Company.CompanyID = '" + CustID + "' AND CRM_Company.CompanyID != " + CustID2 + " GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY CRM_Company.CompanyName ASC";
            }
            if (sortExpression != null)
            {
                DataView dv = query.SelectTable(sql).AsDataView();
                this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";
                dv.Sort = sortExpression + " " + this.SortDirection;
                GVDeliveryPlan.DataSource = dv;
            }
            else
            {
                GVDeliveryPlan.DataSource = query.SelectTable(sql);
            }
            GVDeliveryPlan.DataBind();
        }

        protected void BindGrid2(string sortExpression = null)
        {
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            string CustID2 = DDListCustomer2.SelectedItem.Value;
            sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID  LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND CRM_Company.CompanyID = " + CustID2 + " GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY CRM_Company.CompanyName ASC";
            if (sortExpression != null)
            {
                DataView dv = query.SelectTable(sql).AsDataView();
                this.SortDirection = this.SortDirection == "ASC" ? "DESC" : "ASC";
                dv.Sort = sortExpression + " " + this.SortDirection;
                GVDeliveryPlan2.DataSource = dv;
            }
            else
            {
                GVDeliveryPlan2.DataSource = query.SelectTable(sql);
            }
            GVDeliveryPlan2.DataBind();
        }

        // ในระหว่าง Gridview กำลังแสดงข้อมูล
        protected void GVDeliveryPlan_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;
                // Color Status
                int QtyActual = int.Parse(dr["QtyActual"].ToString());
                int QtyPlan = int.Parse(dr["QtyPlan"].ToString());
                if (QtyActual > QtyPlan)
                {
                    e.Row.Cells[13].CssClass = "bg-warning";
                    e.Row.Cells[13].CssClass = "txt-warning";
                }
                else if (QtyActual == QtyPlan)
                {
                    e.Row.Cells[13].CssClass = "bg-success";
                    e.Row.Cells[13].CssClass = "txt-success";
                }
                else if (QtyActual < QtyPlan && QtyActual != 0)
                {
                    e.Row.Cells[13].CssClass = "bg-danger";
                    e.Row.Cells[13].CssClass = "txt-danger";
                }

                if (QtyActual == 0)
                {
                    ((Label)e.Row.FindControl("LbDiffQty")).Text = "0";
                }

                // Color Time
                DateTime TimePlan = DateTime.Parse(dr["TimePlan"].ToString());
                DateTime TimeActual = DateTime.Parse(dr["TimeActual"].ToString());
                if (TimePlan > TimeActual)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-warning");
                }
                else if (TimeActual == TimePlan)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-success");
                }
                else if (TimePlan < TimeActual)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-danger");
                }

                if (TimeActual == DateTime.Parse("00:00"))
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Text = DateTime.Parse("00:00").ToString("HH:mm");
                }
            }
        }
        protected void GVDeliveryPlan2_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;
                // Color Status
                int QtyActual = int.Parse(dr["QtyActual"].ToString());
                int QtyPlan = int.Parse(dr["QtyPlan"].ToString());
                if (QtyActual > QtyPlan)
                {
                    e.Row.Cells[13].CssClass = "bg-warning";
                    e.Row.Cells[13].CssClass = "txt-warning";
                }
                else if (QtyActual == QtyPlan)
                {
                    e.Row.Cells[13].CssClass = "bg-success";
                    e.Row.Cells[13].CssClass = "txt-success";
                }
                else if (QtyActual < QtyPlan && QtyActual != 0)
                {
                    e.Row.Cells[13].CssClass = "bg-danger";
                    e.Row.Cells[13].CssClass = "txt-danger";
                }

                if (QtyActual == 0)
                {
                    ((Label)e.Row.FindControl("LbDiffQty")).Text = "0";
                }

                // Color Time
                DateTime TimePlan = DateTime.Parse(dr["TimePlan"].ToString());
                DateTime TimeActual = DateTime.Parse(dr["TimeActual"].ToString());
                if (TimePlan > TimeActual)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-warning");
                }
                else if (TimeActual == TimePlan)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-success");
                }
                else if (TimePlan < TimeActual)
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Attributes.Add("class", "txt-danger");
                }

                if (TimeActual == DateTime.Parse("00:00"))
                {
                    ((Label)e.Row.FindControl("LbDiffTime")).Text = DateTime.Parse("00:00").ToString("HH:mm");
                }
            }
        }

        // หลังจากแสดงข้อมูลใน Gridview เสร็จ
        protected void GVDeliveryPlan_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridViewRow rowHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell left = new TableHeaderCell();
                left.ColumnSpan = 8;
                left.Style.Add("border", "1px solid #808080 !important");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 3;
                left.Text = "Delivery Time";
                left.Style.Add("border", "1px solid #808080 !important");
                left.Style.Add("text-align", "center");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 3;
                left.Text = "Delivery Q'ty";
                left.Style.Add("border", "1px solid #808080 !important");
                left.Style.Add("text-align", "center");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 2;
                left.Style.Add("border", "1px solid #808080 !important");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                ((Table)GVDeliveryPlan.Controls[0]).Rows.AddAt(0, rowHeader);
            }
        }
        protected void GVDeliveryPlan2_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                GridViewRow rowHeader = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell left = new TableHeaderCell();
                left.ColumnSpan = 8;
                left.Style.Add("border", "1px solid #808080 !important");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 3;
                left.Text = "Delivery Time";
                left.Style.Add("border", "1px solid #808080 !important");
                left.Style.Add("text-align", "center");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 3;
                left.Text = "Delivery Q'ty";
                left.Style.Add("border", "1px solid #808080 !important");
                left.Style.Add("text-align", "center");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                left = new TableHeaderCell();
                left.ColumnSpan = 2;
                left.Style.Add("border", "1px solid #808080 !important");
                left.CssClass = "FixedHeader-1";
                rowHeader.Cells.Add(left);
                ((Table)GVDeliveryPlan2.Controls[0]).Rows.AddAt(0, rowHeader);
            }
        }

        // จัดเรียงข้อมูล
        protected void GVDeliveryPlan_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.BindGrid(e.SortExpression);
        }
        protected void GVDeliveryPlan2_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.BindGrid2(e.SortExpression);
        }

        //------------------------------------ Button ------------------------------------

        // แสดงข้อมูลของวันที่ที่เลือก
        protected void SelectDate(object sender, EventArgs e)
        {
            this.BindGrid();
            this.BindGrid2();
        }

        // ปุ่มเลือกวันที่ก่อนหน้า
        protected void BtnPrDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(-1).ToString("dd-MM-yyyy");
            this.BindGrid();
            this.BindGrid2();
        }

        // ปุ่มเลือกวันที่ถัดไป
        protected void BtnNextDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(1).ToString("dd-MM-yyyy");
            this.BindGrid();
            this.BindGrid2();
        }

        protected void BtnViewDeliveryPlan_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewActual.aspx");
        }

        //------------------------------------ DropDownList ------------------------------------

        // แสดงข้อมูลใน Gridview เฉพาะ DropdownList ที่เลือก
        protected void DDListCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindGrid();
            this.BindGrid2();
        }
        protected void DDListCustomer2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindGrid();
            this.BindGrid2();
        }

        // แสดงข้อมูลใน DropDownList
        protected void DDListDataBind()
        {
            // DropDownList Customer
            sql = "SELECT CompanyID, CompanyName FROM CRM_Company WHERE Status=1 ORDER BY CompanyName ASC";
            DataTable dtCompany = query.SelectTableTCTV1(sql);
            DDListCustomer.DataSource = dtCompany;
            DDListCustomer.DataValueField = "CompanyID";
            DDListCustomer.DataTextField = "CompanyName";
            DDListCustomer.DataBind();
            DDListCustomer.Items.FindByText("ทั้งหมด").Selected = true;

            DDListCustomer2.DataSource = dtCompany;
            DDListCustomer2.DataValueField = "CompanyID";
            DDListCustomer2.DataTextField = "CompanyName";
            DDListCustomer2.DataBind();
            //DDListCustomer2.Items.FindByValue("48").Selected = true;
        }

        //------------------------------------ Auto Refresh GridView ------------------------------------

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        protected void Timer2_Tick(object sender, EventArgs e)
        {
            this.BindGrid2();
        }
    }
}