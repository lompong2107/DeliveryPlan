using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class Transport : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
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
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('ไม่มีสิทธ์เข้าใช้งานหน้านี้ !!!', 'back');", true);
                    }
                }
            }
           
            if (!IsPostBack) {
                this.BindGrid();
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVTransport.PageIndex = e.NewPageIndex;
            this.BindGrid();
        }
        private void BindGrid()
        {
            sql = "SELECT TransportID, TransportName, Status FROM DP_Transport ORDER BY Status DESC, TransportName ASC";
            GVTransport.DataSource = query.SelectTable(sql);
            GVTransport.DataBind();
        }

        protected void BtnAddTransport_Click(object sender, EventArgs e)
        {
            string TransportName = TxtTransportName.Text;
            if (TransportName.Length > 0)
            {
                sql = "INSERT INTO DP_Transport (TransportName) VALUES ('" + TransportName.ToUpper() + "')";
                if (query.Excute(sql))
                {
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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณากรอกข้อมูล')", true);
            }
        }
        protected void BtnStatus_Command(object sender, CommandEventArgs e)
        {
            string TransportID = e.CommandArgument.ToString();
            sql = "SELECT Status FROM DP_Transport WHERE TransportID = " + TransportID;
            int Status = int.Parse(query.SelectAt(0, sql));
            if (Status == 1)
            {
                sql = "UPDATE DP_Transport SET Status = 0 WHERE TransportID = " + TransportID;
                query.Excute(sql);
            }
            else
            {
                sql = "UPDATE DP_Transport SET Status = 1 WHERE TransportID = " + TransportID;
                query.Excute(sql);
            }
            this.BindGrid();
        }
        protected void BtnEdit_Command(object sender, CommandEventArgs e)
        {
            Session["TransportID"] = e.CommandArgument.ToString();
            sql = "SELECT TransportName FROM DP_Transport WHERE TransportID = " + Session["TransportID"];
            TxtTransportName.Text = query.SelectAt(0, sql);
            BtnAddTransport.Visible = false;
            BtnUpdateTransport.Visible = true;
        }

        //protected void BtnDelete_Command(object sender, CommandEventArgs e)
        //{
        //    string TransportID = e.CommandArgument.ToString();
        //    sql = "DELETE FROM DP_Transport WHERE TransportID = " + TransportID;
        //    query.Excute(sql);
        //    this.BindGrid();
        //}

        protected void BtnUpdateTransport_Click(object sender, EventArgs e)
        {
            string TransportName = TxtTransportName.Text;
            if (TransportName.Length > 0)
            {
                sql = "UPDATE DP_Transport SET TransportName = '" + TransportName + "' WHERE TransportID = " + Session["TransportID"];
                if (query.Excute(sql))
                {
                    Session.Remove("TransportID");
                    TxtTransportName.Text = "";
                    BtnAddTransport.Visible = true;
                    BtnUpdateTransport.Visible = false;
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
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "AlertWarning('กรุณากรอกข้อมูล')", true);
            }
        }

        protected void BtnCancleUpdate_Click(object sender, EventArgs e)
        {
            Session.Remove("TransportID");
            TxtTransportName.Text = "";
            BtnAddTransport.Visible = true;
            BtnUpdateTransport.Visible = false;
        }

        protected void GVTransport_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;
                //Status Qty
                int Status = int.Parse(dr["Status"].ToString());
                if (Status == 1)
                {
                    ((LinkButton)e.Row.FindControl("LinkBtnStatus")).Text = "ใช้งาน";
                    ((LinkButton)e.Row.FindControl("LinkBtnStatus")).Attributes.Add("class", "text-success");
                }
                else
                {
                    ((LinkButton)e.Row.FindControl("LinkBtnStatus")).Text = "ไม่ใช้งาน";
                    ((LinkButton)e.Row.FindControl("LinkBtnStatus")).Attributes.Add("class", "text-warning");
                }
            }
        }
    }
}