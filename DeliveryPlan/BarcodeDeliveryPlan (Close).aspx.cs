using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class BarcodeDeliveryPlan : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
        string DeliveryPlanDetailID = "";
        System.Globalization.CultureInfo _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                if (HttpContext.Current.Session["UserID"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Session หมดเวลาแล้ว'); window.location='Login.aspx';", true);
                }
            }
            else
            {
                if (HttpContext.Current.Session["UserID"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเข้าสู่ระบบ'); window.location='Login.aspx';", true);
                }

                // วันที่ของ Add Form
                DateTime Today = DateTime.Today;
                TxtDate.Text = Today.ToString("dd-MM-yyyy", _curCulture);
                this.BindGrid();
            }
            TxtDeliveryPlanDetailIDScan.Focus();
        }

        private string SortDirection
        {
            get { return ViewState["SortDirection"] != null ? ViewState["SortDirection"].ToString() : "ASC"; }
            set { ViewState["SortDirection"] = value; }
        }

        private void BindGrid(string sortExpression = null)
        {
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_DeliveryPlanDetail.DeliveryPlanDetailID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.TimePlan FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "'";
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

        protected void BtnPrDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(-1).ToString("dd-MM-yyyy");
            this.BindGrid();
        }

        protected void BtnNextDate_Click(object sender, EventArgs e)
        {
            DateTime date = DateTime.Parse(TxtDate.Text);
            TxtDate.Text = date.AddDays(1).ToString("dd-MM-yyyy");
            this.BindGrid();
        }

        protected void SelectDate(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        protected void GVDeliveryPlan_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.BindGrid(e.SortExpression);
        }

        protected void GVDeliveryPlan_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DeliveryPlanDetailID = e.CommandArgument.ToString();
            HDFDeliveryPlanDetailID.Value = DeliveryPlanDetailID;
            string CommandName = e.CommandName.ToString();
            if (CommandName == "Scan")
            {
                sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.TimePlan FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                GVDeliveryPlanInView2.DataSource = query.SelectTable(sql);
                GVDeliveryPlanInView2.DataBind();
                MultiView1.ActiveViewIndex = 1;
                tableDate.Visible = false;
                BtnBack.Visible = true;
                BtnConfirm.Visible = true;
                BtnCancel.Visible = true;
                LbTitle.Text = "Scan Barcode";
                sql = "SELECT COUNT(DP_BarcodeScan.BarcodeID) FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value;
                (GVDeliveryPlanInView2.Rows[0].FindControl("LbCountScan") as Label).Text = query.SelectAt(0, sql).ToString();
                sql = "SELECT DP_DeliveryPlanDetail.QtyPlan, COUNT(DP_BarcodeScan.BarcodeID) FROM DP_DeliveryPlanDetail LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN DP_BarcodeScan ON DP_Barcode.BarcodeID = DP_BarcodeScan.BarcodeID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + " GROUP BY DP_DeliveryPlanDetail.QtyPlan";
                if (int.Parse(query.SelectAt(0, sql)) == int.Parse(query.SelectAt(1, sql)))
                {
                    BtnConfirm.Enabled = true;
                }
                sql = "SELECT DP_Barcode.BarcodeNumber, DP_BarcodeScan.Status FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + DeliveryPlanDetailID + " ORDER BY DP_Barcode.BarcodeNumber";
                GVScan.DataSource = query.SelectTable(sql);
                GVScan.DataBind();
            }
            else if (CommandName == "Print")
            {
                sql = "SELECT COUNT(*) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                int rowCount = int.Parse(query.SelectAt(0, sql));
                if (rowCount == 0)
                {
                    sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.PlanDate FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    string custname = query.SelectAt(1, sql);
                    string CustomerCode = query.SelectAt(3, sql);
                    string time = DateTime.Parse(query.SelectAt(8, sql)).ToString("HHmm");
                    string date = DateTime.Parse(query.SelectAt(9, sql)).ToString("yyyyMMdd");
                    int qty = int.Parse(query.SelectAt(7, sql));
                    for (int num = 1; num <= qty; num++)
                    {
                        string Barcode = custname + time + date + num.ToString("000");
                        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTFactoryConnectionString"].ConnectionString);
                        conn.Open();
                        sql = "INSERT INTO DP_Barcode (DeliveryPlanDetailID, BarcodeNumber, QRCode, Barcode) VALUES (" + DeliveryPlanDetailID + ", '" + Barcode + "', @PicQRCode, @PicBarcode)";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        // Create QR Code
                        QRCodeEncoder enc = new QRCodeEncoder();
                        enc.QRCodeScale = 1;
                        Bitmap qrcode = enc.Encode(Barcode);
                        var codeBitmap = new Bitmap(qrcode);
                        System.Drawing.Image image = (System.Drawing.Image)codeBitmap;
                        MemoryStream stream = new MemoryStream();
                        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] PicQRCode = stream.ToArray();
                        // Create Barcode
                        BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
                        System.Drawing.Image imageBarcode = barcode.Encode(BarcodeLib.TYPE.CODE128, Barcode, Color.Black, Color.White, 290, 50);
                        MemoryStream streamBarcode = new MemoryStream();
                        imageBarcode.Save(streamBarcode, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] PicBarcode = streamBarcode.ToArray();
                        //imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(PicBarcode);
                        cmd.Parameters.AddWithValue("@PicQRCode", PicQRCode);
                        cmd.Parameters.AddWithValue("@PicBarcode", PicBarcode);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('BarcodePrint.aspx?DeliveryPlanDetailID=" + DeliveryPlanDetailID + "','_newtab');", true);

            }
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            MultiView1.ActiveViewIndex = 0;
            tableDate.Visible = true;
            BtnBack.Visible = false;
            BtnConfirm.Visible = false;
            BtnCancel.Visible = false;
            LbTitle.Text = "Barcode Delivery Plan";
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            sql = "DELETE FROM DP_BarcodeScan WHERE BarcodeID IN (SELECT BarcodeID FROM DP_Barcode WHERE DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + ")";
            query.Excute(sql);
            sql = "SELECT DP_Barcode.BarcodeNumber, DP_BarcodeScan.Status FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + " ORDER BY DP_Barcode.BarcodeNumber";
            GVScan.DataSource = query.SelectTable(sql);
            GVScan.DataBind();
            sql = "SELECT COUNT(DP_BarcodeScan.BarcodeID) FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value;
            (GVDeliveryPlanInView2.Rows[0].FindControl("LbCountScan") as Label).Text = query.SelectAt(0, sql).ToString();
        }

        protected void TxtDeliveryPlanDetailIDScan_TextChanged(object sender, EventArgs e)
        {
            string DeliveryPlanDetailIDScan = TxtDeliveryPlanDetailIDScan.Text;
            // Check DeliveryPlanDetailID ว่าตรงกับที่เลือกไหม
            sql = "SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + DeliveryPlanDetailIDScan + "' AND DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value ;
            if (query.CheckRow(sql))
            {
                sql = "SELECT BarcodeID FROM DP_BarcodeScan WHERE BarcodeID = (SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + DeliveryPlanDetailIDScan + "' AND DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + ")";
                if (!query.CheckRow(sql))
                {
                    SoundPlayer player = new SoundPlayer(Server.MapPath("Sounds/CheckoutSuccess.wav"));
                    player.Play();
                    sql = "SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + DeliveryPlanDetailIDScan + "'";
                    string BarcodeID = query.SelectAt(0, sql);
                    sql = "INSERT INTO DP_BarcodeScan (BarcodeID, Status) VALUES (" + BarcodeID + ", 0)";
                    query.Excute(sql);
                    DivAlert.Visible = true;
                    DivAlert.Attributes.Add("class", "alert alert-success");
                    LbAlert.Text = "รหัสบาร์โค้ดถูกต้อง";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                else
                {
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('มีรหัสบาร์โค้ดนี้อยู่แล้ว');", true);
                    DivAlert.Visible = true;
                    DivAlert.Attributes.Add("class", "alert alert-warning");
                    LbAlert.Text = "มีรหัสบาร์โค้ดนี้อยู่แล้ว!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
            else
            {
                //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('รหัสบาร์โค้ดไม่ถูกต้อง');", true);
                DivAlert.Visible = true;
                DivAlert.Attributes.Add("class", "alert alert-danger");
                LbAlert.Text = "รหัสบาร์โค้ดไม่ถูกต้อง!!!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            sql = "SELECT COUNT(DP_BarcodeScan.BarcodeID) FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value;
            (GVDeliveryPlanInView2.Rows[0].FindControl("LbCountScan") as Label).Text = query.SelectAt(0, sql).ToString();
            if (int.Parse(query.SelectAt(0, sql)) > 0)
            {
                sql = "SELECT DP_Barcode.BarcodeNumber, DP_BarcodeScan.Status FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + " ORDER BY DP_Barcode.BarcodeNumber";
                GVScan.DataSource = query.SelectTable(sql);
                GVScan.DataBind();
            }
            sql = "SELECT DP_DeliveryPlanDetail.QtyPlan, COUNT(DP_BarcodeScan.BarcodeID) FROM DP_DeliveryPlanDetail LEFT JOIN DP_Barcode ON DP_DeliveryPlanDetail.DeliveryPlanDetailID = DP_Barcode.DeliveryPlanDetailID LEFT JOIN DP_BarcodeScan ON DP_Barcode.BarcodeID = DP_BarcodeScan.BarcodeID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + " GROUP BY DP_DeliveryPlanDetail.QtyPlan";
            if (int.Parse(query.SelectAt(0, sql)) == int.Parse(query.SelectAt(1, sql)))
            {
                BtnConfirm.Enabled = true;
            }
            TxtDeliveryPlanDetailIDScan.Text = "";
            TxtDeliveryPlanDetailIDScan.Focus();
        }

        protected void BtnConfirm_Click(object sender, EventArgs e)
        {
            sql = "UPDATE DP_BarcodeScan SET Status = 1 WHERE BarcodeID IN (SELECT BarcodeID FROM DP_Barcode WHERE DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + ")";
            query.Excute(sql);
            sql = "SELECT DP_Barcode.BarcodeNumber, DP_BarcodeScan.Status FROM DP_BarcodeScan LEFT JOIN DP_Barcode ON DP_BarcodeScan.BarcodeID = DP_Barcode.BarcodeID WHERE DP_Barcode.DeliveryPlanDetailID = " + HDFDeliveryPlanDetailID.Value + " ORDER BY DP_Barcode.BarcodeNumber";
            GVScan.DataSource = query.SelectTable(sql);
            GVScan.DataBind();
        }
    }
}