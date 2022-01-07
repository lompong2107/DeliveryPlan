using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class PartPrint : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.LocalReport.ReportPath = "Report1.rdlc";
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}