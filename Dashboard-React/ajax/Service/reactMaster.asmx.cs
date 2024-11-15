﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Dashboard_React.ajax.Service
{
    /// <summary>
    /// Summary description for reactMaster
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class reactMaster : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetClass(string SearchKey)
        {
            List<ClassModel> listClass = new List<ClassModel>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                SearchKey = SearchKey.Replace("'", "''");
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                DataTable classes = new DataTable();
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
                SqlCommand cmd = new SqlCommand("PROC_CLASSBIND_REPORT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@filtertext", SearchKey);
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(classes);

                cmd.Dispose();
                con.Dispose();

                listClass = (from DataRow dr in classes.Rows
                             select new ClassModel()
                             {
                                 id = dr["ID"].ToString(),
                                 Name = dr["Name"].ToString(),
                             }).ToList();
            }

            return listClass;
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetClassWiseProduct(string SearchKey, string ClassID)
        {
            List<ClassWiseProductModel> listcwiseProducts = new List<ClassWiseProductModel>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                SearchKey = SearchKey.Replace("'", "''");
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                DataTable cwiseProduct = new DataTable();
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
                //SqlCommand cmd = new SqlCommand("Proc_GetSubLedger", con);
                SqlCommand cmd = new SqlCommand("PRC_PRODUCTSBIND_REPORT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@filtertext", SearchKey);
                cmd.Parameters.AddWithValue("@ProductClass", ClassID);
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(cwiseProduct);


                cmd.Dispose();
                con.Dispose();

                listcwiseProducts = (from DataRow dr in cwiseProduct.Rows
                                     select new ClassWiseProductModel()
                                     {
                                         id = dr["ID"].ToString(),
                                         Code = dr["Code"].ToString(),
                                         Name = dr["Name"].ToString(),
                                         Hsn = dr["Hsn"].ToString()

                                     }).ToList();
            }

            return listcwiseProducts;
        }
    }

    public class ClassModel
    {
        public string id { get; set; }
        public string Name { get; set; }
    }
    public class ClassWiseProductModel
    {
        public string id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Hsn { get; set; }

    }
}
