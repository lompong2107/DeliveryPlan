using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DeliveryPlan
{
    public class QuerySQL
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTFactoryConnectionString"].ConnectionString);
        SqlConnection connTCTV1 = new SqlConnection(ConfigurationManager.ConnectionStrings["TCTV1ConnectionString"].ConnectionString);
        
        //ส่วน Select ข้อมูล
        public string SelectAt(int index, string sql)
        {
            string ans = "";
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            try
            {
                SqlDataReader result = cmd.ExecuteReader();
                if (result.HasRows)
                {
                    result.Read();
                    ans = result.GetValue(index).ToString();
                    result.Close();
                }
                else
                {
                    ans = "0";
                }
            }
            catch (Exception ex)
            {
                ans = "0";
            }
            conn.Close();
            return ans;
        }
        public Boolean CheckRow(string sql)
        {
            Boolean ans = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader result = cmd.ExecuteReader();
            if (result.HasRows)
            {
                ans = true;
            }
            conn.Close();
            return ans;
        }
        public DataTable SelectTable(string sql)
        {
            DataTable ans = new DataTable();
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlDataReader result = cmd.ExecuteReader();
            ans.Load(result);
            conn.Close();
            return ans;
        }
        public DataTable SelectTableTCTV1(string sql)
        {
            DataTable ans = new DataTable();
            connTCTV1.Open();
            SqlCommand cmd = new SqlCommand(sql, connTCTV1);
            SqlDataReader result = cmd.ExecuteReader();
            ans.Load(result);
            connTCTV1.Close();
            return ans;
        }

        //ส่วน Insert ข้อมูล
        public Boolean Excute(string sql)
        {
            Boolean ans = false;
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            int result = cmd.ExecuteNonQuery();
            if (result == 1)
            {
                ans = true;
            }
            conn.Close();
            return ans;
        }
    }
}