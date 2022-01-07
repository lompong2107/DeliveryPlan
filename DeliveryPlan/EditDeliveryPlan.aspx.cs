using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class EditDeliveryPlan : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        CultureInfo _curCulture = new CultureInfo("en-US");
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
                    if ((PermissID != 120 && PermissID != 121) && DepartmentID != 1)
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('ไม่มีสิทธ์เข้าใช้งานหน้านี้ !!!'); window.history.back();", true);
                    }
                }
            }

            if (!IsPostBack)
            {
                DateTime Today = DateTime.Today;
                TxtDate.Text = Today.ToString("dd-MM-yyyy", _curCulture);
                DDListDataBind();
                this.BindGrid();
            }
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
            if (CustID == 0)
            {
                //sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID AND DP_Barcode.Status != 3 LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID  LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND DP_Customer.CustID != " + CustID2 + " GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY DP_Customer.CustName ASC";
                sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID AND DP_Barcode.Status != 3 LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY CRM_Company.CompanyName ASC";
            }
            else
            {
                //sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID  LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND DP_Customer.CustName = '" + CustName + "' AND DP_Customer.CustID != " + CustID2 + " GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY DP_Customer.CustName ASC";
                sql = "SELECT DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, COUNT(DP_Barcode.BarcodeID) as QtyTotal, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID LEFT JOIN DP_Status ON DP_DeliveryPlanDetail.StatusID = DP_Status.StatusID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "' AND DP_DeliveryPlanDetail.QtyPlan != 0 AND CRM_Company.CompanyID = '" + CustID + "' GROUP BY DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.TimeActual, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.QtyActual, DP_Status.StatusName, DP_DeliveryPlanDetail.Remark, FG.ImagePart ORDER BY CRM_Company.CompanyName ASC";
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

            // ถ้าปุ่มแก้ไขซ่อนอยู่แสดงว่าตอนนี้กำลังแก้ไขข้อมูล ให้ไปเรียก method การเปิด Textbox ให้แก้ไขได้
            if (BtnEditAll.Visible == false)
            {
                BtnEditAll_Click(null, null);
            }
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

        //protected void BtnSave_Command(object sender, CommandEventArgs e)
        //{
        //    string[] arg = e.CommandArgument.ToString().Split(',');
        //    int index = int.Parse(arg[0]);
        //    string DeliveryPlanDetailID = arg[1];
        //    string UserID = Session["UserID"].ToString();
        //    int QtyActual = int.Parse((GVDeliveryPlan.Rows[index].FindControl("TxtQtyActual") as TextBox).Text);
        //    string TimeActual = (GVDeliveryPlan.Rows[index].FindControl("TxtTimeActual") as TextBox).Text.ToString();
        //    string Remark = (GVDeliveryPlan.Rows[index].FindControl("TxtRemark") as TextBox).Text;
        //    if (Remark.Length == 0)
        //    {
        //        sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', QtyActual = " + QtyActual + ", Remark = NULL, UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
        //    }
        //    else
        //    {
        //        sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', QtyActual = " + QtyActual + ", Remark = '" + Remark + "', UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
        //    }
        //    if (query.Excute(sql))
        //    {
        //        this.BindGrid();
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกข้อมูลสำเร็จ')", true);
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
        //    }
        //}

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

        // จัดเรียงข้อมูล
        protected void GVDeliveryPlan_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.BindGrid(e.SortExpression);
        }

        //------------------------------------ Button ------------------------------------

        // แสดงข้อมูลของวันที่ที่เลือก
        protected void SelectDate(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        // บันทึกข้อมูลใน Gridview
        protected void BtnSaveAll_Click(object sender, EventArgs e)
        {
            bool checkResult = false;
            string DeliveryPlanDetailID = "";
            foreach (GridViewRow row in GVDeliveryPlan.Rows)
            {
                int index = row.RowIndex;
                DeliveryPlanDetailID = (GVDeliveryPlan.Rows[index].FindControl("HFDeliveryPlanID") as HiddenField).Value;
                string UserID = Session["UserID"].ToString();
                int QtyActual = int.Parse((GVDeliveryPlan.Rows[index].FindControl("TxtQtyActual") as TextBox).Text);
                string TimeActual = (GVDeliveryPlan.Rows[index].FindControl("TxtTimeActual") as TextBox).Text.ToString();
                string Remark = (GVDeliveryPlan.Rows[index].FindControl("TxtRemark") as TextBox).Text;
                if (Remark.Length == 0)
                {
                    sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', QtyActual = " + QtyActual + ", Remark = NULL, UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    //sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', Remark = NULL, UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                }
                else
                {
                    sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', QtyActual = " + QtyActual + ", Remark = '" + Remark + "', UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    //sql = "UPDATE DP_DeliveryPlanDetail SET TimeActual = '" + TimeActual + "', Remark = '" + Remark + "', UserIDUpdate = " + UserID + ", PlanDateUpdate = GETDATE() WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                }
                if (query.Excute(sql))
                {
                    checkResult = true;
                    // เช็คว่าจำนวนที่ QtyActual ตรงกับ QtyPlan หรือไม่
                    sql = "SELECT QtyActual, QtyPlan FROM DP_DeliveryPlanDetail WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    // Update StatusID : 1=OK, 2=Delay, 3=Over, 4=Pending
                    int QtyActualUpdate = int.Parse(query.SelectAt(0, sql));
                    int QtyPlanUpdate = int.Parse(query.SelectAt(1, sql));
                    int StatusID = 0;
                    if (QtyActualUpdate == QtyPlanUpdate)
                    {
                        StatusID = 1;
                    }
                    else if (QtyActualUpdate < QtyPlanUpdate && QtyActualUpdate != 0)
                    {
                        StatusID = 2;
                    }
                    else if (QtyActualUpdate > QtyPlanUpdate)
                    {
                        StatusID = 3;
                    }
                    else
                    {
                        StatusID = 4;
                    }
                    sql = "UPDATE DP_DeliveryPlanDetail SET StatusID = " + StatusID + " WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    query.Excute(sql);
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

        // ปุ่มเลือกวันที่ก่อนหน้า
        protected void BtnPrDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(-1).ToString("dd-MM-yyyy");
            this.BindGrid();
        }

        // ปุ่มเลือกวันที่ถัดไป
        protected void BtnNextDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(1).ToString("dd-MM-yyyy");
            this.BindGrid();
        }

        protected void BtnEditAll_Click(object sender, EventArgs e)
        {
            BtnEditAll.Visible = false;
            BtnSaveAll.Visible = true;
            BtnCanCelEdit.Visible = true;
            foreach (GridViewRow row in GVDeliveryPlan.Rows)
            {
                int index = row.RowIndex;
                int DepartmentID = int.Parse(Session["DepartmentID"].ToString());
                TextBox TxtQty = GVDeliveryPlan.Rows[index].FindControl("TxtQtyActual") as TextBox;
                TextBox TxtTime = GVDeliveryPlan.Rows[index].FindControl("TxtTimeActual") as TextBox;
                TextBox TxtRemark = GVDeliveryPlan.Rows[index].FindControl("TxtRemark") as TextBox;
                // DepartmentID 1 = IT
                //if (DepartmentID == 1)
                //{
                TxtQty.Enabled = true;
                TxtQty.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                TxtQty.Style.Add("background-color", "rgba(51,122,183,0.1)");
                //}
                TxtTime.Enabled = true;
                TxtTime.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                TxtTime.Style.Add("background-color", "rgba(51,122,183,0.1)");
                TxtRemark.Enabled = true;
                TxtRemark.Style.Add("border", "1px solid rgba(46,109,164,0.3)");
                TxtRemark.Style.Add("background-color", "rgba(51,122,183,0.1)");
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

        //------------------------------------ DropDownList ------------------------------------

        // แสดงข้อมูลใน Gridview เฉพาะ DropdownList ที่เลือก
        protected void DDListCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindGrid();
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
        }
    }
}