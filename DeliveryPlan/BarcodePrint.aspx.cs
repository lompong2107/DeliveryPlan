using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace DeliveryPlan
{
    public partial class BarcodePrint : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                if (IsPostBack)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Session หมดเวลาแล้ว'); window.location='Login.aspx';", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเข้าสู่ระบบ'); window.location='Login.aspx';", true);
                }
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
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่มีสิทธ์เข้าใช้งานหน้านี้ !!!'); window.history.back();", true);
                    }
                }
            }

            if (!IsPostBack)
            {
                string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
                sql = "SELECT  DP_Barcode.BarcodeID, DP_DeliveryPlan.DeliveryPlanID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.TimePlan, DP_Barcode.DeliveryPlanDetailID, DP_Barcode.BarcodeNumber, DP_Barcode.QRCode, DP_Barcode.Barcode, DP_DeliveryPlanDetail.PlanDate FROM DP_Barcode LEFT JOIN DP_DeliveryPlanDetail ON DP_Barcode.DeliveryPlanDetailID = DP_DeliveryPlanDetail.DeliveryPlanDetailID LEFT JOIN DP_DeliveryPlan ON DP_DeliveryPlanDetail.DeliveryPlanID = DP_DeliveryPlan.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_Barcode.DeliveryPlanDetailID = " + DeliveryPlanDetailID + " ORDER BY DP_Barcode.BarcodeID";
                ReportDataSource rds = new ReportDataSource("DP_Barcode", query.SelectTable(sql));
                ReportViewer1.LocalReport.ReportPath = "BarcodeReport.rdlc";
                ReportViewer1.LocalReport.DataSources.Add(rds);
                ReportViewer1.LocalReport.Refresh();
            }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            // Status : 1=Sended, 2=Stock, 3=Pending
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            sql = "UPDATE DP_Barcode SET Status = 2 WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 3";
            query.Excute(sql);
        }
    }
}