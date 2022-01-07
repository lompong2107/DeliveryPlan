using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class AddDeliveryPlan : System.Web.UI.Page
    {
        QuerySQL query = new QuerySQL();
        string sql = "";
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

                DateTime Today = DateTime.Today;
                System.Globalization.CultureInfo _curCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                TxtDate.Text = Today.ToString("yyyy-MM-dd", _curCulture);
                this.BindGrid();
                DDListDataBind();
            }
            LbDate.Text = "วัน/เดือน/ปี : " + DateTime.Parse(TxtDate.Text).ToString("dd/MM/yyyy");
        }

        //เปลี่ยนหน้า Gridview
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVDeliveryPlan.PageIndex = e.NewPageIndex;
            this.BindGrid();
        }
        private void BindGrid()
        {
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            sql = "SELECT DP_DeliveryPlan.DeliveryPlanID, DP_Customer.CustName, Project.ProjectName, FG.CustomerCode, FG.FGName, Part.PartName, DP_Transport.TransportName, DP_DeliveryPlanDetail.QtyPlan, DP_DeliveryPlanDetail.TimePlan FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID LEFT JOIN DP_Customer ON DP_DeliveryPlan.CustID = DP_Customer.CustID LEFT JOIN FG ON DP_DeliveryPlan.FGID = FG.FGID LEFT JOIN Part ON FG.PartID = Part.PartID LEFT JOIN Project ON Part.ProjectID = Project.ProjectID LEFT JOIN DP_Transport ON DP_DeliveryPlan.TransportID = DP_Transport.TransportID WHERE DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "'";
            GVDeliveryPlan.DataSource = query.SelectTable(sql);
            GVDeliveryPlan.DataBind();
        }

        //เลือก Project
        protected void OnSelectIndexChangedDDListProject(object sender, EventArgs e)
        {
            // DropDownList Part
            DDListPart.Items.Clear();
            DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
            DDListPart.Items[0].Attributes["disabled"] = "disabled";
            DDListPart.SelectedValue = "0";
            sql = "SELECT PartID, PartName FROM Part WHERE ProjectID = " + DDListProject.SelectedValue + " AND Status=1 ORDER BY PartName ASC";
            DDListPart.DataSource = query.SelectTable(sql);
            DDListPart.DataValueField = "PartID";
            DDListPart.DataTextField = "PartName";
            DDListPart.DataBind();
        }
        //เลือก Part
        protected void OnSelectIndexChangedDDListPart(object sender, EventArgs e)
        {
            // DropDownList FG
            DDListFG.Items.Clear();
            DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
            DDListFG.Items[0].Attributes["disabled"] = "disabled";
            DDListFG.SelectedValue = "0";
            sql = "SELECT FGID, FGName FROM FG WHERE PartID = " + DDListPart.SelectedValue + " AND Status=1 ORDER BY FGName ASC";
            DDListFG.DataSource = query.SelectTable(sql);
            DDListFG.DataValueField = "FGID";
            DDListFG.DataTextField = "FGName";
            DDListFG.DataBind();
        }
        //เลือก FG
        protected void OnSelectIndexChangedDDListFG(object sender, EventArgs e)
        {
            sql = "SELECT CustomerCode FROM FG WHERE FGID = " + DDListFG.SelectedValue;
            string IDCode = query.SelectAt(0, sql);
            LbIDCode.Text = IDCode;
        }

        protected void BtnCancle_Click(object sender, EventArgs e)
        {
            TxtQty.Text = "0";
            TxtTimePlanAdd.Text = "00:00";
            LbIDCode.Text = "";
            //ล้างค่าใน DropDownList
            DDListCustomer.Items.Clear();
            DDListProject.Items.Clear();
            DDListPart.Items.Clear();
            DDListFG.Items.Clear();
            DDListTransport.Items.Clear();
            //เพิ่ม itam ที่ 0 และกำหนด disabled
            DDListCustomer.Items.Insert(0, new ListItem("-- Select Customer --", "0"));
            DDListCustomer.Items[0].Attributes["disabled"] = "disabled";
            DDListProject.Items.Insert(0, new ListItem("-- Select Project --", "0"));
            DDListProject.Items[0].Attributes["disabled"] = "disabled";
            DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
            DDListPart.Items[0].Attributes["disabled"] = "disabled";
            DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
            DDListFG.Items[0].Attributes["disabled"] = "disabled";
            DDListTransport.Items.Insert(0, new ListItem("-- Select Transport --", "0"));
            DDListTransport.Items[0].Attributes["disabled"] = "disabled";
            //เลือกที่ index 0
            DDListCustomer.SelectedValue = "0";
            DDListProject.SelectedValue = "0";
            DDListPart.SelectedValue = "0";
            DDListFG.SelectedValue = "0";
            //เอาข้อมูลใน Database มาสร้าง item
            DDListDataBind();
        }

        protected void BtnAdd_Click(object sender, EventArgs e)
        {
            int UserID = int.Parse(Session["UserID"].ToString());
            int CustID = int.Parse(DDListCustomer.SelectedValue);
            int ProjectID = int.Parse(DDListProject.SelectedValue);
            int PartID = int.Parse(DDListPart.SelectedValue);
            int FGID = int.Parse(DDListFG.SelectedValue);
            int TransportID = int.Parse(DDListTransport.SelectedValue);
            int QtyPlan = int.Parse(TxtQty.Text);
            string TimePlan = TxtTimePlanAdd.Text;
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            if (HttpContext.Current.Session["UserID"] == null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Session หมดเวลาแล้ว'); window.location='Login.aspx';", true);
            }
            else
            {
                if (CustID != 0)
                {
                    if (ProjectID != 0)
                    {
                        if (PartID != 0)
                        {
                            if (FGID != 0)
                            {
                                if (TransportID != 0)
                                {
                                    //เช็คว่ามีข้อมูลที่กรอกว่ามีในตาราง DP_Delivery และ DP_DeliveryDetail หรือไม่ ถ้ามีแล้วไม่ต้องเพิ่ม
                                    sql = "SELECT * FROM DP_DeliveryPlan LEFT JOIN DP_DeliveryPlanDetail ON DP_DeliveryPlan.DeliveryPlanID = DP_DeliveryPlanDetail.DeliveryPlanID WHERE DP_DeliveryPlan.FGID = " + FGID + " AND DP_DeliveryPlan.CustID = " + CustID + " AND DP_DeliveryPlan.TransportID = " + TransportID + " AND DP_DeliveryPlanDetail.PlanDate = '" + dateSelect + "'";
                                    if (!query.CheckRow(sql))
                                    {
                                        //เช็คว่ามีข้อมูลในตาราง DP_DeliveryPlan หรือไม่ ถ้าไม่มีก็เพิ่มข้อมูลเข้าไป
                                        sql = "SELECT * FROM DP_DeliveryPlan WHERE FGID = " + FGID + " AND CustID = " + CustID + " AND TransportID = " + TransportID;
                                        if (!query.CheckRow(sql))
                                        {
                                            //Insert 2 ตารางไปเลยสิครับ
                                            string sqlDP_DeliveryPlan = "INSERT INTO DP_DeliveryPlan (CustID, FGID, TransportID) VALUES (" + CustID + ", " + FGID + ", " + TransportID + ")";
                                            query.Excute(sqlDP_DeliveryPlan);
                                        }
                                        string sqlSelectDeliveryPlanID = "SELECT DeliveryPlanID FROM DP_DeliveryPlan WHERE FGID = " + FGID + " AND CustID = " + CustID + " AND TransportID = " + TransportID;
                                        int DeliveryPlanID = int.Parse(query.SelectAt(0, sqlSelectDeliveryPlanID));
                                        sql = "INSERT INTO DP_DeliveryPlanDetail (DeliveryPlanID, UserIDCreate, TimePlan, QtyPlan, PlanDate, PlanDateCreate) VALUES (" + DeliveryPlanID + ", " + UserID + ", '" + TimePlan + "', " + QtyPlan + ", '" + dateSelect + "', GETDATE())";
                                        if (query.Excute(sql))
                                        {
                                            TxtQty.Text = "0";
                                            TxtTimePlanAdd.Text = "00:00";
                                            /*LbIDCode.Text = "";
                                            //ล้างค่าใน DropDownList
                                            DDListCustomer.Items.Clear();
                                            DDListProject.Items.Clear();
                                            DDListPart.Items.Clear();
                                            DDListFG.Items.Clear();
                                            DDListTransport.Items.Clear();
                                            //เพิ่ม itam ที่ 0 และกำหนด disabled
                                            DDListCustomer.Items.Insert(0, new ListItem("-- Select Customer --", "0"));
                                            DDListCustomer.Items[0].Attributes["disabled"] = "disabled";
                                            DDListProject.Items.Insert(0, new ListItem("-- Select Project --", "0"));
                                            DDListProject.Items[0].Attributes["disabled"] = "disabled";
                                            DDListPart.Items.Insert(0, new ListItem("-- Select Part --", "0"));
                                            DDListPart.Items[0].Attributes["disabled"] = "disabled";
                                            DDListFG.Items.Insert(0, new ListItem("-- Select FG --", "0"));
                                            DDListFG.Items[0].Attributes["disabled"] = "disabled";
                                            DDListTransport.Items.Insert(0, new ListItem("-- Select Transport --", "0"));
                                            DDListTransport.Items[0].Attributes["disabled"] = "disabled";
                                            //เลือกที่ index 0
                                            DDListCustomer.SelectedValue = "0";
                                            DDListProject.SelectedValue = "0";
                                            DDListPart.SelectedValue = "0";
                                            DDListFG.SelectedValue = "0";
                                            //เอาข้อมูลใน Database มาสร้าง itam
                                            DDListDataBind();*/
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
                                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ล้มเหลว! มีข้อมูล Delivery Plan นี้อยู่แล้ว')", true);
                                    }
                                }
                                else
                                {
                                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือก Transport')", true);
                                }
                            }
                            else
                            {
                                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือก FG')", true);
                            }
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือก Part')", true);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือก Project')", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('กรุณาเลือก Customer')", true);
                }
            }
        }

        protected void BtnSave_Command(object sender, CommandEventArgs e)
        {
            string[] arg = e.CommandArgument.ToString().Split(',');
            int index = int.Parse(arg[0]);
            string DeliveryPlanID = arg[1];
            int QtyPlan = int.Parse((GVDeliveryPlan.Rows[index].FindControl("TxtQtyPlan") as TextBox).Text);
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            string TimePlan = (GVDeliveryPlan.Rows[index].FindControl("TxtTimePlan") as TextBox).Text.ToString();
            sql = "UPDATE DP_DeliveryPlanDetail SET PlanDateUpdate = GETDATE() WHERE DeliveryPlanID = " + DeliveryPlanID+ " AND PlanDate = '"+dateSelect+"'";
            query.Excute(sql);
            sql = "UPDATE DP_DeliveryPlanDetail SET TimePlan = '" + TimePlan + "', QtyPlan = " + QtyPlan + " WHERE DeliveryPlanID = " + DeliveryPlanID + " AND PlanDate = '" + dateSelect + "'";
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

        protected void BtnDelete_Command(object sender, CommandEventArgs e)
        {
            string DeliveryPlanID = e.CommandArgument.ToString();
            string dateSelect = DateTime.Parse(TxtDate.Text).ToString("yyyy-MM-dd");
            sql = "DELETE FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + DeliveryPlanID + " AND PlanDate = '" + dateSelect + "'";
            if (query.Excute(sql))
            {
                sql = "SELECT COUNT(*) FROM DP_DeliveryPlanDetail WHERE DeliveryPlanID = " + DeliveryPlanID;
                if (!query.CheckRow(sql))
                {
                    sql = "DELETE FROM DP_DeliveryPlan WHERE DeliveryPlanID = " + DeliveryPlanID;
                    query.Excute(sql);
                }
                this.BindGrid();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ลบข้อมูลสำเร็จ')", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('ล้มเหลว! ลบข้อมูลไม่สำเร็จ')", true);
            }   
        }

        protected void SelectDate(object sender, EventArgs e)
        {
            this.BindGrid();
        }

        protected void DDListDataBind()
        {
            // DropDownList Customer
            sql = "SELECT CustID, CustName FROM DP_Customer WHERE Status=1 ORDER BY CustName ASC";
            DDListCustomer.DataSource = query.SelectTable(sql);
            DDListCustomer.DataValueField = "CustID";
            DDListCustomer.DataTextField = "CustName";
            DDListCustomer.DataBind();

            // DropDownList Project
            sql = "SELECT ProjectID, ProjectName FROM Project WHERE Status=1 ORDER BY ProjectName ASC";
            DDListProject.DataSource = query.SelectTable(sql);
            DDListProject.DataValueField = "ProjectID";
            DDListProject.DataTextField = "ProjectName";
            DDListProject.DataBind();

            // DropDownList Transport
            sql = "SELECT TransportID, TransportName FROM DP_Transport WHERE Status=1 ORDER BY TransportName ASC";
            DDListTransport.DataSource = query.SelectTable(sql);
            DDListTransport.DataValueField = "TransportID";
            DDListTransport.DataTextField = "TransportName";
            DDListTransport.DataBind();
            DDListTransport.ClearSelection();
            DDListTransport.Items.FindByText("TCT").Selected = true;
        }
    }
}