using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class BarcodeScan : System.Web.UI.Page
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
                if ((PermissID != 120 && PermissID != 121) && DepartmentID != 1)
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ไม่มีสิทธ์เข้าใช้งานหน้านี้ !!!'); window.history.back();", true);
                }
            }

            if (!IsPostBack)
            {
                this.BindGrid();
            }
            //Textbox ไหนแสดงอยู่ให้ Focus ที่ Textbox นั้น
            if (TxtBarcodeScan.Visible == true)
            {
                TxtBarcodeScan.Focus();
            }
            else
            {
                TxtBarcodeScanIn.Focus();
            }
        }

        //------------------------------------ Gridview ------------------------------------

        // แสดงข้อมูลใน Gridview
        private void BindGrid()
        {
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_DeliveryPlanDetail.DeliveryPlanDetailID, CRM_Company.CompanyName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.QtyPlan, (SELECT COUNT(*) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 2 OR Status = 1) as QtyTotal, (SELECT COUNT(*) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 2) as QtyStock, DP_DeliveryPlanDetail.PlanDate, DP_DeliveryPlanDetail.TimePlan FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN [TCTV1].[dbo].[CRM_Company] CRM_Company ON DP_DeliveryPlan.CustID = CRM_Company.CompanyID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailId = " + DeliveryPlanDetailID;
            GVDeliveryPlan.DataSource = query.SelectTable(sql);
            GVDeliveryPlan.DataBind();

            sql = "SELECT BarcodeID, BarcodeNumber, Status FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " ORDER BY Status DESC, BarcodeNumber ASC";
            GVScan.DataSource = query.SelectTable(sql);
            GVScan.DataBind();

            // Count Barcode ScanOut
            sql = "SELECT COUNT(BarcodeID) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 1";
            (GVDeliveryPlan.Rows[0].FindControl("LbCountScan") as Label).Text = query.SelectAt(0, sql).ToString();
        }

        // ปุ่มลบใน Gridview Scan สามารถลบได้เฉพาะบาร์โค้ดที่ยังไม่ได้ปริ้น
        protected void GVScan_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string BarcodeID = e.CommandArgument.ToString();
            if (e.CommandName == "Delete")
            {
                sql = "DELETE FROM DP_Barcode WHERE BarcodeID = " + BarcodeID;
                query.Excute(sql);
                this.BindGrid();
            }
        }

        // ในขณะที่ข้อมูลกำลังแสดงใน Gridview ให้เช็ค Status ของบาร์โค้ดด้วย ถ้าไม่ใช่ Status Pending ให้ซ่อนปุ่มลบ
        protected void GVScan_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRow dr = ((DataRowView)e.Row.DataItem).Row;
                sql = "SELECT Status FROM DP_Barcode WHERE BarcodeID = " + dr["BarcodeID"];
                int Status = int.Parse(query.SelectAt(0, sql));
                if (Status != 3)
                {
                    ((Button)e.Row.FindControl("BtnDelete") as Button).Visible = false;
                    if (Status == 1)
                    {
                        e.Row.CssClass = "bg-sended";
                        e.Row.Cells[2].CssClass = "text-center";
                    }
                }
            }
        }

        // Gridview Deleting
        protected void GVScan_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // แก้ Error
        }

        //------------------------------------ Textbox ------------------------------------

        // Textbox Scan Out
        protected void TxtBarcodeScan_TextChanged(object sender, EventArgs e)
        {
            string BarcodeNumber = TxtBarcodeScan.Text;
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            // Check DeliveryPlanDetailID กับ BarcodeNumber ว่าตรงกับที่เลือกและ ScanOut ไหมไหม
            sql = "SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + BarcodeNumber + "' AND DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status != 3";
            if (query.CheckRow(sql))
            {
                // เช็คว่า Scan Out Barcode นี้ไปหรือยัง
                sql = "SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + BarcodeNumber + "' AND Status = 1";
                if (!query.CheckRow(sql))
                {
                    SoundPlayer player = new SoundPlayer(Server.MapPath("Sounds/ScanOutSuccess.wav"));
                    player.Play();
                    sql = "UPDATE DP_Barcode SET Status = 1 WHERE BarcodeNumber = '" + BarcodeNumber + "' AND DeliveryPlanDetailID = " + DeliveryPlanDetailID;
                    query.Excute(sql);
                    DivAlert.Visible = true;
                    DivAlert.Attributes.Add("class", "alert alert-success");
                    LbAlert.Text = "รหัสบาร์โค้ดถูกต้อง";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
                else
                {
                    SoundPlayer player = new SoundPlayer(Server.MapPath("Sounds/ScanOutFailed.wav"));
                    player.Play();
                    DivAlert.Visible = true;
                    DivAlert.Attributes.Add("class", "alert alert-warning");
                    LbAlert.Text = "รหัสบาร์โค้ดนี้ทำการ Scan Out แล้ว!";
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
                }
            }
            else
            {
                SoundPlayer player = new SoundPlayer(Server.MapPath("Sounds/ScanOutFailed.wav"));
                player.Play();
                DivAlert.Visible = true;
                DivAlert.Attributes.Add("class", "alert alert-danger");
                LbAlert.Text = "รหัสบาร์โค้ดไม่ถูกต้อง!!!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            // นับจำนวนบาร์โค้ดที่ Scan Out แล้วอัพเดตลงใน DP_DeliveryPlanDetail.QtyActual
            sql = "SELECT COUNT(BarcodeID) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 1";
            sql = "UPDATE DP_DeliveryPlanDetail SET QtyActual = " + query.SelectAt(0, sql).ToString() + " WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            query.Excute(sql);
            // เช็คว่าจำนวนที่ Scan Out ตรงกับใน Stock หรือไม่
            sql = "SELECT (SELECT COUNT(*) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 1) as QtyStock, QtyPlan FROM DP_DeliveryPlanDetail WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            // StatusID : 1=OK, 2=Delay, 3=Over, 4=Pending
            if (int.Parse(query.SelectAt(0, sql)) > int.Parse(query.SelectAt(1, sql)))
            {
                (GVDeliveryPlan.Rows[0].FindControl("LbCountScan") as Label).Style.Add("color", "#017bb9");
            }
            else if (int.Parse(query.SelectAt(0, sql)) < int.Parse(query.SelectAt(1, sql)) && int.Parse(query.SelectAt(0, sql)) != 0)
            {
                (GVDeliveryPlan.Rows[0].FindControl("LbCountScan") as Label).Style.Add("color", "red");
            }
            else if (int.Parse(query.SelectAt(0, sql)) == int.Parse(query.SelectAt(1, sql)))
            {
                (GVDeliveryPlan.Rows[0].FindControl("LbCountScan") as Label).Style.Add("color", "green");
            }
            // เช็คว่าจำนวนที่ Scan Out ตรงกับใน Stock หรือไม่
            sql = "SELECT (SELECT COUNT(*) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND Status = 1) as QtyStock, QtyPlan FROM DP_DeliveryPlanDetail WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            // Update StatusID : 1=OK, 2=Delay, 3=Over, 4=Pending
            int QtyStockUpdate = int.Parse(query.SelectAt(0, sql));
            int QtyPlanUpdate = int.Parse(query.SelectAt(1, sql));
            int StatusID = 0;
            if (QtyStockUpdate == QtyPlanUpdate)
            {
                StatusID = 1;
            }
            else if (QtyStockUpdate < QtyPlanUpdate && QtyStockUpdate != 0)
            {
                StatusID = 2;
            }
            else if (QtyStockUpdate > QtyPlanUpdate)
            {
                StatusID = 3;
            }
            else
            {
                StatusID = 4;
            }
            sql = "UPDATE DP_DeliveryPlanDetail SET StatusID = " + StatusID + " WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            query.Excute(sql);
            this.BindGrid();
            TxtBarcodeScan.Text = "";
            TxtBarcodeScan.Focus();
        }

        // Textbox Scan In เพิ่มบาร์โค้ดโดยการ Scan
        protected void TxtBarcodeScanIn_TextChanged(object sender, EventArgs e)
        {
            string BarcodeNumber = TxtBarcodeScanIn.Text;
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];

            // เช็คว่า Scan Barcode นี้ไปหรือยัง
            sql = "SELECT BarcodeID FROM DP_Barcode WHERE BarcodeNumber = '" + BarcodeNumber + "' AND DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            if (!query.CheckRow(sql))
            {
                SoundPlayer player = new SoundPlayer(Server.MapPath("Sounds/ScanOutSuccess.wav"));
                player.Play();
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTFactoryConnectionString"].ConnectionString);
                conn.Open();
                sql = "INSERT INTO DP_Barcode (DeliveryPlanDetailID, BarcodeNumber, QRCode, Barcode, Status) VALUES (" + DeliveryPlanDetailID + ", '" + BarcodeNumber + "', @PicQRCode, @PicBarcode, 2)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                // Create QR Code
                QRCodeEncoder enc = new QRCodeEncoder();
                enc.QRCodeScale = 1;
                Bitmap qrcode = enc.Encode(BarcodeNumber);
                var codeBitmap = new Bitmap(qrcode);
                System.Drawing.Image image = (System.Drawing.Image)codeBitmap;
                MemoryStream stream = new MemoryStream();
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] PicQRCode = stream.ToArray();
                // Create Barcode
                BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
                System.Drawing.Image imageBarcode = barcode.Encode(BarcodeLib.TYPE.CODE128, BarcodeNumber, Color.Black, Color.White, 290, 50);
                MemoryStream streamBarcode = new MemoryStream();
                imageBarcode.Save(streamBarcode, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] PicBarcode = streamBarcode.ToArray();
                cmd.Parameters.AddWithValue("@PicQRCode", PicQRCode);
                cmd.Parameters.AddWithValue("@PicBarcode", PicBarcode);
                cmd.ExecuteNonQuery();
                conn.Close();
                DivAlert.Visible = true;
                DivAlert.Attributes.Add("class", "alert alert-success");
                LbAlert.Text = "Success";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            else
            {
                DivAlert.Visible = true;
                DivAlert.Attributes.Add("class", "alert alert-warning");
                LbAlert.Text = "มีรหัสบาร์โค้ดนี้อยู่แล้ว!";
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel();", true);
            }
            this.BindGrid();
            TxtBarcodeScanIn.Text = "";
            TxtBarcodeScanIn.Focus();
        }

        //------------------------------------ Button ------------------------------------

        // Scan In
        protected void BtnScanIn_Click(object sender, EventArgs e)
        {
            BtnScanIn.Visible = false;
            BtnPrintBarcode.Visible = false;
            BtnModal.Visible = false;
            TxtBarcodeScan.Visible = false;
            TxtBarcodeScanIn.Visible = true;
            BtnScanOut.Visible = true;
            LbScanTitle.Text = "Scan In :";
            TxtBarcodeScanIn.Focus();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabelNow();", true);

        }

        // Scan Out
        protected void BtnScanOut_Click(object sender, EventArgs e)
        {
            BtnScanIn.Visible = true;
            BtnPrintBarcode.Visible = true;
            BtnModal.Visible = true;
            TxtBarcodeScan.Visible = true;
            TxtBarcodeScanIn.Visible = false;
            BtnScanOut.Visible = false;
            LbScanTitle.Text = "Scan Out :";
            TxtBarcodeScan.Focus();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabelNow();", true);
        }

        // Print
        protected void BtnPrintBarcode_Click(object sender, EventArgs e)
        {
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('BarcodePrint.aspx?DeliveryPlanDetailID=" + DeliveryPlanDetailID + "','_newtab');", true);
        }

        // Open Modal
        protected void BtnModal_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#ModalCreateBarcode').modal();", true);
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            sql = "SELECT QtyPlan FROM DP_DeliveryPlanDetail WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            int QtyPlan = int.Parse(query.SelectAt(0, sql));
            sql = "SELECT COUNT(BarcodeID) FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            int QtyStock = int.Parse(query.SelectAt(0, sql));
            if (QtyStock >= QtyPlan)
            {
                TxtQtyBarcode.Text = "0";
                TxtQtyBarcode.Attributes.Add("min", "0");
            }
            else
            {
                TxtQtyBarcode.Text = QtyPlan.ToString();
                TxtQtyBarcode.Attributes.Add("min", QtyPlan.ToString());
            }
        }

        // Create Barcode
        protected void BtnCreateBarcode_Click(object sender, EventArgs e)
        {
            string DeliveryPlanDetailID = Request.QueryString["DeliveryPlanDetailID"];
            sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_Customer.CustName, FG.FGName, FG.CustomerCode, DP_DeliveryPlanDetail.QtyPlan, DP_Customer.CustID, DP_Transport.TransportID, DP_DeliveryPlanDetail.TimePlan, DP_DeliveryPlanDetail.PlanDate FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.DeliveryPlanDetailID = " + DeliveryPlanDetailID;
            string custname = query.SelectAt(1, sql);
            string FGName = query.SelectAt(2, sql);
            string CustomerCode = query.SelectAt(3, sql);
            string QtyPlan = query.SelectAt(4, sql);
            string CustID = query.SelectAt(5, sql);
            string TransportID = query.SelectAt(6, sql);
            string QRCodeString = FGName.Replace("-", "") + "|" + CustomerCode + "|" + QtyPlan + "|" + CustID + "|" + TransportID;
            string time = DateTime.Parse(query.SelectAt(7, sql)).ToString("HHmm");
            string date = DateTime.Parse(query.SelectAt(8, sql)).ToString("yyyyMMdd");
            int qty = int.Parse(TxtQtyBarcode.Text);
            bool CheckRow = false;
            sql = "SELECT * FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND BarcodeNumber like '" + custname + time + date + "%'";
            CheckRow = query.CheckRow(sql);
            for (int num = 1; num <= qty; num++)
            {
                string Barcode = "";
                if (CheckRow)
                {
                    sql = "SELECT TOP 1 BarcodeNumber FROM DP_Barcode WHERE DeliveryPlanDetailID = " + DeliveryPlanDetailID + " AND BarcodeNumber like '" + custname + time + date + "%' ORDER BY BarcodeNumber DESC";
                    string strLast = query.SelectAt(0, sql).Substring(query.SelectAt(0, sql).Length - 3);
                    int numLast = int.Parse(strLast);
                    numLast++;
                    Barcode = custname + time + date + numLast.ToString("000");
                }
                else
                {
                    Barcode = custname + time + date + num.ToString("000");
                }
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTFactoryConnectionString"].ConnectionString);
                conn.Open();
                sql = "INSERT INTO DP_Barcode (DeliveryPlanDetailID, BarcodeNumber, QRCode, Barcode, Status) VALUES (" + DeliveryPlanDetailID + ", '" + Barcode + "', @PicQRCode, @PicBarcode, 3)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                // Create QR Code
                QRCodeEncoder enc = new QRCodeEncoder();
                enc.QRCodeScale = 1;
                Bitmap qrcode = enc.Encode(QRCodeString);
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
            this.BindGrid();
        }

        protected void BtnRefresh_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabelNow();", true);
            this.BindGrid();
        }
    }
}