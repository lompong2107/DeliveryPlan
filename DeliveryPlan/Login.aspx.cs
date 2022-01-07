using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DeliveryPlan
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTV1ConnectionString"].ConnectionString);
        string sql;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                // DepartmetID ที่ 31 = Planning, 1 = IT
                // PermissID ที่ 119 = Plan, 120 = Delivery
                if (int.Parse(Session["PermissID"].ToString()) == 119 || int.Parse(Session["DepartmentID"].ToString()) == 1 || int.Parse(Session["PermissID"].ToString()) != 121)
                {
                    Response.Redirect("AddDeliveryPlanMulti.aspx");
                }
                // DepartmetID ที่ 22 = Delivery
                else if (int.Parse(Session["PermissID"].ToString()) == 120)
                {
                    Response.Redirect("EditDeliveryPlan.aspx");
                }
            }

            HttpRequest httpRequest = HttpContext.Current.Request;
            if (httpRequest.Browser.IsMobileDevice == true)
            {
                Session["Device"] = true;
            }
            else
            {
                Session["Device"] = false;
            }

            if (TxtUser.Text == "")
            {
                TxtUser.Focus();
            }
            else
            {
                TxtPassword.Focus();
            }
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = TxtUser.Text;
            string password = TxtPassword.Text;
            if (username.Length > 0 && password.Length > 0)
            {
                //if (username == "planning" && password == "1234")
                //{
                //    Session["UserID"] = 31;
                //    Session["Name"] = "Planning Permiss";
                //    Session["DepartmentID"] = 31;
                //    Session["PermissID"] = 119;
                //    Response.Redirect("AddDeliveryPlanMulti.aspx");
                //}
                //else if (username == "delivery" && password == "1234")
                //{
                //    Session["UserID"] = 22;
                //    Session["Name"] = "Delivery Permiss";
                //    Session["DepartmentID"] = 22;
                //    Session["PermissID"] = 120;
                //    Response.Redirect("EditDeliveryPlan.aspx");
                //}
                // เช็คชื่อผู้ใช้และรหัสผ่าน
                sql = "SELECT UserID, (FIrstNameTh + ' ' + LastNameTh) As Name, DepartmentID FROM F2_Users WHERE Username = '" + username + "' AND Password = '" + password + "' AND Status = 1";
                conn.Open();
                SqlCommand cmdUser = new SqlCommand(sql, conn);
                SqlDataReader resultUser = cmdUser.ExecuteReader();
                if (resultUser.HasRows)
                {
                    resultUser.Read();
                    string UserID = resultUser.GetValue(0).ToString();
                    string Name = resultUser.GetValue(1).ToString();
                    int DepartmentID = int.Parse(resultUser.GetValue(2).ToString());
                    resultUser.Close();
                    // เช็คสิทธิ์การใช้งาน
                    sql = "SELECT PermissID FROM F2_Users LEFT JOIN F2_UserPermiss ON F2_Users.UserID = F2_UserPermiss.UserID WHERE F2_Users.UserID = " + UserID + " AND (F2_UserPermiss.PermissID = 119 OR F2_UserPermiss.PermissID = 120 OR F2_UserPermiss.PermissID = 121 OR F2_Users.DepartmentID = 1)";
                    SqlCommand cmdUserPermiss = new SqlCommand(sql, conn);
                    SqlDataReader resultUserPermiss = cmdUserPermiss.ExecuteReader();
                    if (resultUserPermiss.HasRows)
                    {
                        resultUserPermiss.Read();
                        int PermissID = int.Parse(resultUserPermiss.GetValue(0).ToString());
                        resultUserPermiss.Close();
                        Session["UserID"] = UserID;
                        Session["Name"] = Name;
                        Session["DepartmentID"] = DepartmentID;
                        Session["PermissID"] = PermissID;
                        Session["Width"] = HFWidth.Value;
                        // DepartmetID ที่ 31 = Planning, 1 = IT
                        // PermissID ที่ 119 = Planning, 120 = Delivery
                        if (PermissID == 119 || PermissID == 121 || DepartmentID == 1)
                        {
                            Response.Redirect("EditDeliveryPlan.aspx");
                        }
                        // DepartmetID ที่ 22 = Delivery
                        else if (PermissID == 120)
                        {
                            Response.Redirect("EditDeliveryPlan.aspx");
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "AlertError('ไม่มีสิทธิ์เข้าถึง');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "AlertError('ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง');", true);
                }
                conn.Close();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alertMessage", "AlertWarning('กรุณากรอกข้อมูลให้ครบ');", true);
            }
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewActual.aspx");
        }
    }
}