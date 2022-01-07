using System;
using System.Web;
using System.Web.UI;

namespace DeliveryPlan
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session["UserID"] == null)
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเข้าสู่ระบบ')", true);
                Response.Redirect("ViewActual.aspx");
            } else
            {
                Response.Redirect("EditDeliveryPlan.aspx");
            }
        }
    }
}