using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class Navbar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null || Session["PermissID"] != null || Session["DepartmentID"] != null || Session["Name"] != null)
            {
                int PermissID = int.Parse(Session["PermissID"].ToString());
                int DepartmentID = int.Parse(Session["DepartmentID"].ToString());
                if (Session["UserID"] != null)
                {
                    if (PermissID != 119 && DepartmentID != 1 && PermissID != 121)
                    {
                        LiBtnAddDeliveryPlanMulti.Visible = false;
                        LiBtnTransport.Visible = false;
                    }
                    if (PermissID != 120 && DepartmentID != 1 && PermissID != 121)
                    {
                        LiBtnEditDeliveryPlan.Visible = false;
                    }
                }
            }

            if (Session["UserID"] == null)
            {
                LiBtnAddDeliveryPlanMulti.Visible = false;
                LiBtnEditDeliveryPlan.Visible = false;
                LiBtnTransport.Visible = false;
                LiName.Visible = false;
                LiDropDown.Visible = false;
                LiWarning.Visible = false;
            } else
            {
                LiBtnViewActual.Visible = false;
                LiWarning.Visible = false;
                LiLogin.Visible = false;
            }

            string path = HttpContext.Current.Request.Url.AbsolutePath;
            if (path.Contains("AddDeliveryPlanMulti.aspx"))
            {
                LiBtnAddDeliveryPlanMulti.Attributes.Add("class", "active");
            }
            else if (path.Contains("EditDeliveryPlan.aspx") || path.Contains("BarcodeScan.aspx"))
            {
                LiBtnEditDeliveryPlan.Attributes.Add("class", "active");
            }
            else if (path.Contains("Transport.aspx"))
            {
                LiBtnTransport.Attributes.Add("class", "active");
            }
            else if (path.Contains("ViewActual.aspx"))
            {
                LiBtnViewActual.Attributes.Add("class", "active");
            }
            else if (path.Contains("Report.aspx"))
            {
                LiBtnReport.Attributes.Add("class", "active");
            }
        }
        protected void BtnAddDeliveryPlanMulti_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddDeliveryPlanMulti.aspx");
        }
        protected void BtnEditDeliveryPlan_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditDeliveryPlan.aspx");
        }
        protected void BtnTransport_Click(object sender, EventArgs e)
        {
            Response.Redirect("Transport.aspx");
        }
        protected void BtnViewActual_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewActual.aspx");
        }
        protected void BtnReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("Report.aspx");
        }

        protected void BtnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            Response.Redirect("ViewActual.aspx");
        }
    }
}