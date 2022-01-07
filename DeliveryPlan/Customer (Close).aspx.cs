using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class AddCustomer : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int PermissID = 0;
            int DepartmentID = 0;
            if (Session["PermissID"] != null && Session["DepartmentID"] != null)
            {
                PermissID = int.Parse(Session["PermissID"].ToString());
                DepartmentID = int.Parse(Session["DepartmentID"].ToString());
                if ((PermissID != 119 && PermissID != 121) && DepartmentID != 1)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่มีสิทธ์เข้าใช้งานหน้านี้ !!!'); window.history.back();", true);
                }
            }
            this.BindGrid();
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVCustomer.PageIndex = e.NewPageIndex;
            this.BindGrid();
        }
        private void BindGrid()
        {
            sql = "SELECT CustID, CustName, Status FROM DP_Customer ORDER BY Status DESC, CustName ASC";
            GVCustomer.DataSource = query.SelectTable(sql);
            GVCustomer.DataBind();
        }

        protected void BtnAddCust_Click(object sender, EventArgs e)
        {
            string customerName = TxtCustName.Text;
            if (customerName.Length > 0)
            {
                sql = "INSERT INTO DP_Customer (CustName, Status) VALUES ('" + customerName.ToUpper() + "', 1)";
                if (query.Excute(sql))
                {
                    this.BindGrid();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกข้อมูลสำเร็จ')", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณากรอกข้อมูล')", true);
            }
        }
        protected void BtnStatus_Command(object sender, CommandEventArgs e)
        {
            string CustID = e.CommandArgument.ToString();
            sql = "SELECT Status FROM DP_Customer WHERE CustID = " + CustID;
            int Status = int.Parse(query.SelectAt(0, sql));
            if (Status == 1)
            {
                sql = "UPDATE DP_Customer SET Status = 0 WHERE CustID = " + CustID;
                query.Excute(sql);
            } else
            {
                sql = "UPDATE DP_Customer SET Status = 1 WHERE CustID = " + CustID;
                query.Excute(sql);
            }
            this.BindGrid();
        }
        protected void BtnEdit_Command(object sender, CommandEventArgs e)
        {
            Session["CustID"] = e.CommandArgument.ToString();
            sql = "SELECT CustName FROM DP_Customer WHERE CustID = " + Session["CustID"];
            TxtCustName.Text = query.SelectAt(0, sql);
            BtnAddCust.Visible = false;
            BtnUpdateCust.Visible = true;
        }

        protected void BtnDelete_Command(object sender, CommandEventArgs e)
        {
            string CustID = e.CommandArgument.ToString();
            sql = "DELETE FROM DP_Customer WHERE CustID = " + CustID;
            query.Excute(sql);
            this.BindGrid();
        }

        protected void BtnUpdateCust_Click(object sender, EventArgs e)
        {
            string customerName = TxtCustName.Text;
            if (customerName.Length > 0)
            {
                sql = "UPDATE DP_Customer SET CustName = '" + customerName + "' WHERE CustID = " + Session["CustID"];
                if (query.Excute(sql))
                {
                    Session.Remove("CustID");
                    TxtCustName.Text = "";
                    BtnAddCust.Visible = true;
                    BtnUpdateCust.Visible = false;
                    this.BindGrid();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('บันทึกข้อมูลสำเร็จ')", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ล้มเหลว! บันทึกข้อมูลไม่สำเร็จ')", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณากรอกข้อมูล')", true);
            }
        }

        protected void BtnCancleUpdate_Click(object sender, EventArgs e)
        {
            Session.Remove("CustID");
            TxtCustName.Text = "";
            BtnAddCust.Visible = true;
            BtnUpdateCust.Visible = false;
        }

        protected void GVCustomer_RowDataBound(object sender, GridViewRowEventArgs e)
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