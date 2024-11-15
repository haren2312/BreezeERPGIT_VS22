﻿using DataAccessLayer;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using EntityLayer.CommonELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using BusinessLogicLayer;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using DevExpress.Data;
using System.Threading.Tasks;
using DevExpress.XtraPrinting;
using DevExpress.Export;
using System.Drawing;
using Reports.Model;

namespace Reports.Reports.GridReports
{
    public partial class CashBankBookDetail_Subledgerwise : System.Web.UI.Page
    {
        BusinessLogicLayer.Reports objReport = new BusinessLogicLayer.Reports();
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(string.Empty);
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();
        CommonBL cbl = new CommonBL();

        decimal TotalDebit = 0, TotalCredit = 0;
        decimal _totalDebit = 0, _totalCredit = 0;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string sPath = Convert.ToString(HttpContext.Current.Request.Url);
                oDBEngine.Call_CheckPageaccessebility(sPath);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            sqlbranch.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/GridReports/CashBankBookDetail_Subledgerwise.aspx");
            string ProjectSelectInEntryModule = cbl.GetSystemSettingsResult("ProjectSelectInEntryModule");
            if (!String.IsNullOrEmpty(ProjectSelectInEntryModule))
            {
                if (ProjectSelectInEntryModule == "Yes")
                {
                    lookup_project.Visible = true;
                    lblProj.Visible = true;
                    ShowGrid.Columns[11].Visible = true;
                    hdnProjectSelection.Value = "1";

                }
                else if (ProjectSelectInEntryModule.ToUpper().Trim() == "NO")
                {
                    lookup_project.Visible = false;
                    lblProj.Visible = false;
                    ShowGrid.Columns[11].Visible = false;
                    hdnProjectSelection.Value = "0";
                }
            }
            if (!IsPostBack)
            {
                DataTable dtBranchSelection = new DataTable();
                DataTable dtProjectSelection = new DataTable();
                string GridHeader = "";
                BusinessLogicLayer.Reports GridHeaderDet = new BusinessLogicLayer.Reports();
                RptHeading.Text = "Subledger wise Cash/Bank Book - Detail";
                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), true, false, false, false, false, false);
                CompName.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, true, false, false, false, false);
                CompAdd.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, true, false, false, false);
                CompOth.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, true, false, false);
                CompPh.Text = GridHeader;

                GridHeader = GridHeaderDet.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), false, false, false, false, false, true);
                CompAccPrd.Text = GridHeader;

                BranchHoOffice();
                lookupCashBank.DataBind();

                string strChqNochk = (chkchqno.Checked) ? "1" : "0";
                if (Convert.ToString(strChqNochk) == "0")
                {
                    ShowGrid.Columns[7].Visible = false;
                }
                else
                {
                    ShowGrid.Columns[7].Visible = true;
                }

                string strChqDtchk = (chkchqdt.Checked) ? "1" : "0";
                if (Convert.ToString(strChqDtchk) == "0")
                {
                    ShowGrid.Columns[8].Visible = false;
                }
                else
                {
                    ShowGrid.Columns[8].Visible = true;
                }

                string strChqOnBnkchk = (chkchqonbnk.Checked) ? "1" : "0";
                if (Convert.ToString(strChqOnBnkchk) == "0")
                {
                    ShowGrid.Columns[9].Visible = false;
                }
                else
                {
                    ShowGrid.Columns[9].Visible = true;
                }

                string strHdNarrachk = (chkhdnarra.Checked) ? "1" : "0";
                if (Convert.ToString(strHdNarrachk) == "0")
                {
                    ShowGrid.Columns[13].Visible = false;
                }
                else
                {
                    ShowGrid.Columns[13].Visible = true;
                }

                string strFinYear = Convert.ToString(Session["LastFinYear"]);
                DataTable dt = oDBEngine.GetDataTable("Select FinYear_Code,FinYear_StartDate,FinYear_EndDate From Master_FinYear Where FinYear_Code='" + strFinYear + "'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    string strStartDate = Convert.ToString(dt.Rows[0]["FinYear_StartDate"]);
                    DateTime StartDate = Convert.ToDateTime(strStartDate);

                    ASPxFromDate.Value = DateTime.Now;
                    ASPxToDate.Value = DateTime.Now;
                }
                else
                {
                    ASPxFromDate.Value = DateTime.Now;
                    ASPxToDate.Value = DateTime.Now;
                }
                Date_finyearwise(Convert.ToString(Session["LastFinYear"]));

                Session["IsSubledgerCashBankDet"] = null;
                Session["BranchNames"] = null;

                chkallparties.Attributes.Add("OnClick", "AllParties('allparties')");

                dtBranchSelection = oDBEngine.GetDataTable("select Variable_Value from Config_SystemSettings where Variable_Name='ReportsBranchSelection'");
                hdnBranchSelection.Value = dtBranchSelection.Rows[0][0].ToString();

                dtProjectSelection = oDBEngine.GetDataTable("select Variable_Value from Config_SystemSettings where Variable_Name='ReportsProjectSelection'");
                hdnProjectSelectionInReport.Value = dtProjectSelection.Rows[0][0].ToString();
            }
        }

        public void Date_finyearwise(string Finyear)
        {
            CommonBL bll1 = new CommonBL();
            DataTable stbill = new DataTable();

            stbill = bll1.GetDateFinancila(Finyear);
            if (stbill.Rows.Count > 0)
            {

                ASPxFromDate.MaxDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_EndDate"]));
                ASPxFromDate.MinDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_StartDate"]));

                ASPxToDate.MaxDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_EndDate"]));
                ASPxToDate.MinDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_StartDate"]));

                DateTime MaximumDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_EndDate"]));
                DateTime MinimumDate = Convert.ToDateTime((stbill.Rows[0]["FinYear_StartDate"]));

                DateTime TodayDate = Convert.ToDateTime(DateTime.Now);
                DateTime FinYearEndDate = MaximumDate;

                if (TodayDate > FinYearEndDate)
                {
                    ASPxToDate.Date = FinYearEndDate;
                    ASPxFromDate.Date = MinimumDate;
                }
                else
                {
                    ASPxToDate.Date = TodayDate;
                    ASPxFromDate.Date = MinimumDate;
                }

            }

        }
        #region Database Details

        public void BranchHoOffice()
        {
            CommonBL bll1 = new CommonBL();
            DataTable stbill = new DataTable();
            DataTable dtBranchChild = new DataTable();
            stbill = bll1.GetBranchheadoffice(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]), "HO");
            if (stbill.Rows.Count > 0)
            {
                ddlbranchHO.DataSource = stbill;
                ddlbranchHO.DataTextField = "Code";
                ddlbranchHO.DataValueField = "branch_id";
                ddlbranchHO.DataBind();
                dtBranchChild = GetChildBranch(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]));
                if (dtBranchChild.Rows.Count > 0)
                {
                    ddlbranchHO.Items.Insert(0, new ListItem("All", "All"));
                }
            }
        }

        public DataTable GetChildBranch(string CHILDBRANCH)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("PRC_FINDCHILDBRANCH_REPORT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CHILDBRANCH", CHILDBRANCH);
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();
            return dt;
        }

        public DataTable GetCashBankList()
        {
            string query = "", branchList = "";
            string strType = Convert.ToString(ddlType.SelectedValue);

            List<object> BranchList = lookupBranch.GridView.GetSelectedFieldValues("ID");
            foreach (object branchIDs in BranchList)
            {
                branchList += "," + branchIDs;
            }
            branchList = branchList.TrimStart(',');

            if (strType == "Bank")
            {
                lookupCashBank.Columns["Name"].Caption = "Bank Name";
            }
            else if (strType == "Cash")
            {
                lookupCashBank.Columns["Name"].Caption = "Cash Name";
            }

            try
            {
                if (branchList.Trim() != "")
                {
                    if (strType == "Bank")
                    {
                        query = @"Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name
                            from Master_MainAccount WHERE MainAccount_BankCashType='Bank' AND MainAccount_PaymentType<>'Card' AND MainAccount_branchId IN(" + Convert.ToString(branchList) + ")  " +
                               "union ALL " +
                               "Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name " +
                               "from Master_MainAccount WHERE MainAccount_BankCashType='Bank' AND MainAccount_PaymentType<>'Card' AND MainAccount_branchId= 0 and " +
                               "not exists(select 1 from tbl_master_ledgerBranch_map where MainAccount_id =MainAccount_ReferenceID) " +
                               "union ALL " +
                               "Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name " +
                               "from Master_MainAccount WHERE MainAccount_BankCashType='Bank' AND MainAccount_PaymentType<>'Card' AND MainAccount_branchId= 0 and " +
                               "exists(select 1 from tbl_master_ledgerBranch_map where MainAccount_id =MainAccount_ReferenceID and branch_id IN (" + Convert.ToString(branchList) + "))";
                    }
                    else if (strType == "Cash")
                    {
                        query = @"Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name 
                                from Master_MainAccount WHERE MainAccount_BankCashType='Cash' AND MainAccount_branchId IN(" + Convert.ToString(branchList) + ")  " +
                                    "union ALL " +
                                    "Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name " +
                                    "from Master_MainAccount WHERE MainAccount_BankCashType='Cash' AND MainAccount_branchId= 0 and " +
                                    "not exists(select 1 from tbl_master_ledgerBranch_map where MainAccount_id =MainAccount_ReferenceID) " +
                                    "union ALL " +
                                    "Select MainAccount_ReferenceID AS ID, MainAccount_Name as Name " +
                                    "from Master_MainAccount WHERE MainAccount_BankCashType='Cash' AND MainAccount_branchId= 0 and " +
                                    "exists(select 1 from tbl_master_ledgerBranch_map where MainAccount_id =MainAccount_ReferenceID and branch_id IN(" + Convert.ToString(branchList) + "))";
                    }
                }

                DataTable dt = oDBEngine.GetDataTable(query);
                return dt;
            }
            catch
            {
                return null;
            }
        }

        protected void Componentbranch_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string FinYear = Convert.ToString(Session["LastFinYear"]);

            if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            {
                DataTable ComponentTable = new DataTable();
                string Hoid = e.Parameter.Split('~')[1];
                if (Hoid != "All")
                {
                    ComponentTable = GetBranch(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]), Hoid);

                    if (ComponentTable.Rows.Count > 0)
                    {
                        Session["SI_ComponentData_Branch"] = ComponentTable;
                        lookupBranch.DataSource = ComponentTable;
                        lookupBranch.DataBind();
                    }
                    else
                    {
                        Session["SI_ComponentData_Branch"] = ComponentTable;
                        lookupBranch.DataSource = null;
                        lookupBranch.DataBind();
                    }
                }
                else
                {
                    ComponentTable = oDBEngine.GetDataTable("select * from(select branch_id as ID,branch_description,branch_code from tbl_master_branch a where a.branch_id=1 union all select branch_id as ID,branch_description,branch_code from tbl_master_branch b where b.branch_parentId=1) a order by branch_description");
                    Session["SI_ComponentData_Branch"] = ComponentTable;
                    lookupBranch.DataSource = ComponentTable;
                    lookupBranch.DataBind();
                }
            }
        }

        public DataTable GetBranch(string BRANCH_ID, string Ho)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("GetFinancerBranchfetchhowise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Branch", BRANCH_ID);
            cmd.Parameters.AddWithValue("@Hoid", Ho);
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();

            return dt;
        }

        #endregion

        #region Lookup Details

        protected void lookupBranch_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_ComponentData_Branch"] != null)
            {
                lookupBranch.DataSource = (DataTable)Session["SI_ComponentData_Branch"];
            }
        }

        protected void lookupCashBank_DataBinding(object sender, EventArgs e)
        {
            lookupCashBank.DataSource = GetCashBankList();
        }

        #endregion

        #region CallBackPanel Details

        protected void CashBankPanel_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            lookupCashBank.GridView.Selection.CancelSelection();
            lookupCashBank.DataSource = GetCashBankList();
            lookupCashBank.DataBind();
        }

        #endregion

        #region Export Details

        protected void drdExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 Filter = int.Parse(Convert.ToString(drdExport.SelectedItem.Value));
            if (Filter != 0)
            {
                bindexport(Filter);
            }
        }
        public void bindexport(int Filter)
        {
            string filename = "SubledgerwiseCashBankBook_Detail";
            exporter.FileName = filename;
            string FileHeader = "";

            exporter.FileName = filename;

            BusinessLogicLayer.Reports RptHeader = new BusinessLogicLayer.Reports();

            FileHeader = RptHeader.CommonReportHeader(Convert.ToString(Session["LastCompany"]), Convert.ToString(Session["LastFinYear"]), true, true, true, true, true, true) + Environment.NewLine + "Subledger wise Cash/Bank Book - Detail" + Environment.NewLine + "For the period " + Convert.ToDateTime(ASPxFromDate.Date).ToString("dd-MM-yyyy") + " To " + Convert.ToDateTime(ASPxToDate.Date).ToString("dd-MM-yyyy");

            FileHeader = ReplaceFirst(FileHeader, "\r\n", Convert.ToString(Session["BranchNames"]));

            exporter.RenderBrick += exporter_RenderBrick;
            exporter.PageHeader.Left = FileHeader;
            exporter.PageHeader.Font.Size = 10;
            exporter.PageHeader.Font.Name = "Tahoma";

            //exporter.PageFooter.Center = "[Page # of Pages #]";
            //exporter.PageFooter.Right = "[Date Printed]";
            exporter.GridViewID = "ShowGrid";
            switch (Filter)
            {
                case 1:
                    exporter.WriteXlsxToResponse(new XlsxExportOptionsEx() { ExportType = ExportType.WYSIWYG });
                    break;
                case 2:
                    exporter.WritePdfToResponse();
                    break;
                case 3:
                    exporter.WriteCsvToResponse();
                    break;
                case 4:
                    exporter.WriteRtfToResponse();
                    break;
                default:
                    return;
            }
        }

        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + Environment.NewLine + replace + Environment.NewLine + text.Substring(pos + search.Length);
        }

        protected void exporter_RenderBrick(object sender, ASPxGridViewExportRenderingEventArgs e)
        {
            e.BrickStyle.BackColor = Color.White;
            e.BrickStyle.ForeColor = Color.Black;
        }
        #endregion

        #region Grid Details

        protected void CallbackPanel_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            string IsSubledgerCashBankDet = Convert.ToString(hfIsSubledgerCashBankDet.Value);
            Session["IsSubledgerCashBankDet"] = IsSubledgerCashBankDet;
            DateTime dtFrom;
            DateTime dtTo;
            dtFrom = Convert.ToDateTime(ASPxFromDate.Date);
            dtTo = Convert.ToDateTime(ASPxToDate.Date);
            string FROMDATE = dtFrom.ToString("yyyy-MM-dd");
            string TODATE = dtTo.ToString("yyyy-MM-dd");
            string BRANCH_ID = "";

            string CashComponent = "";
            List<object> CashList = lookupBranch.GridView.GetSelectedFieldValues("ID");
            foreach (object Cash in CashList)
            {
                CashComponent += "," + Cash;
            }
            BRANCH_ID = CashComponent.TrimStart(',');

            string PARTYLEDGERIDS = "";
            PARTYLEDGERIDS = hdnSelectedPartyLedger.Value;

            string PROJECT_ID = "";
            string Projects = "";
            List<object> ProjectList = lookup_project.GridView.GetSelectedFieldValues("ID");
            foreach (object Project in ProjectList)
            {
                Projects += "," + Project;
            }
            PROJECT_ID = Projects.TrimStart(',');

            Task PopulateStockTrialDataTask = new Task(() => GetSubledgerCashBankBook(FROMDATE, TODATE, BRANCH_ID, PARTYLEDGERIDS, PROJECT_ID));
            PopulateStockTrialDataTask.RunSynchronously();
        }

        public void GetSubledgerCashBankBook(string FROMDATE, string TODATE, string BRANCH_ID, string SUBLEDGERIDS, string PROJECT_ID)
        {
            string branchList = "", cashbankList = "";
            string strCompany = Convert.ToString(Session["LastCompany"]);
            string strFinYear = Convert.ToString(Session["LastFinYear"]);
            string strFromDate = Convert.ToDateTime(ASPxFromDate.Value).ToString("yyyy-MM-dd");
            string strToDate = Convert.ToDateTime(ASPxToDate.Value).ToString("yyyy-MM-dd");
            string strType = Convert.ToString(ddlType.SelectedValue);

            branchList = BRANCH_ID;

            List<object> CashBankList = lookupCashBank.GridView.GetSelectedFieldValues("ID");
            foreach (object cashbankIDs in CashBankList)
            {
                cashbankList += "," + cashbankIDs;
            }
            cashbankList = cashbankList.TrimStart(',');           

            string BRANCH_NAME = "";
            string BranchNameComponent = "";
            List<object> BranchNameList = lookupBranch.GridView.GetSelectedFieldValues("branch_description");
            foreach (object BranchName in BranchNameList)
            {
                BranchNameComponent += "," + BranchName;
            }
            if (BranchNameList.Count > 1)
            {
                BRANCH_NAME = "Multiple Branch Selected";
                Session["BranchNames"] = BRANCH_NAME;
            }
            else
            {
                BRANCH_NAME = BranchNameComponent.TrimStart(',');
                Session["BranchNames"] = "For Unit : " + BRANCH_NAME + " ";
            }
            CallbackPanel.JSProperties["cpBranchNames"] = Convert.ToString(Session["BranchNames"]);

            try
            {
                DataSet ds = new DataSet();
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));

                SqlCommand cmd = new SqlCommand("PRC_SUBLEDGERWISECASHBANKBOOK_REPORT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COMPANYID", strCompany);
                cmd.Parameters.AddWithValue("@FROMDATE", strFromDate);
                cmd.Parameters.AddWithValue("@TODATE", strToDate);
                cmd.Parameters.AddWithValue("@FINYEAR", strFinYear);
                cmd.Parameters.AddWithValue("@BRANCH_ID", branchList);
                cmd.Parameters.AddWithValue("@SUBLEDGERID", SUBLEDGERIDS);
                cmd.Parameters.AddWithValue("@CASHBANKTYPE", strType);
                cmd.Parameters.AddWithValue("@CASHBANKID", cashbankList);
                cmd.Parameters.AddWithValue("@PROJECT_ID", PROJECT_ID);
                cmd.Parameters.AddWithValue("@ISCBWISE", (chkCBwise.Checked) ? "1" : "0");
                cmd.Parameters.AddWithValue("@USERID", Convert.ToInt32(Session["userid"]));

                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

                cmd.Dispose();
                con.Dispose();

            }
            catch
            {
                
            }
        }

        protected void ShowGrid_SummaryDisplayText(object sender, ASPxGridViewSummaryDisplayTextEventArgs e)
        {
            if (e.Item.FieldName == "CREDIT")
            {
                TotalCredit = Convert.ToDecimal(e.Value);
            }
            else if (e.Item.FieldName == "DEBIT")
            {
                TotalDebit = Convert.ToDecimal(e.Value);
            }

            if (e.Item.FieldName == "CLOSEBAL_DBCR")
            {
                if ((TotalDebit - TotalCredit) > 0)
                {
                    e.Text = "Dr";
                }
                else if ((TotalDebit - TotalCredit) < 0)
                {
                    e.Text = "Cr";
                }
                else
                {
                    e.Text = "";
                }
            }
            else if (e.Item.FieldName == "PAYEE_INFO")
            {
                e.Text = "Net Total";
            }
            else if (e.Item.FieldName == "CLOSING_BALANCE")
            {
                e.Text = Convert.ToString(Math.Abs((TotalDebit - TotalCredit)));
            }
            else
            {
                e.Text = string.Format("{0}", e.Value);
            }


            //Group Summary Calculation
            ASPxGridView grid = sender as ASPxGridView;
            if (e.IsGroupSummary)
            {
                string summaryTAG = (e.Item as ASPxSummaryItem).Tag;
                switch (summaryTAG)
                {
                    case "Item_Debit":
                        e.Text = string.Format("{0}", e.Value);
                        _totalDebit = Convert.ToDecimal(e.Value);
                        break;
                    case "Item_Credit":
                        e.Text = string.Format("{0}", e.Value);
                        _totalCredit = Convert.ToDecimal(e.Value);
                        break;
                    case "Item_Balance":
                        e.Text = string.Format("{0}", Math.Abs((_totalDebit - _totalCredit)));

                        break;
                    case "Item_DBCR":
                        if ((_totalDebit - _totalCredit) > 0) e.Text = "Dr";
                        else if ((_totalDebit - _totalCredit) < 0) e.Text = "Cr";
                        else e.Text = "";
                        break;
                }

            }

            //Group Summary Calculation
        }        

        #endregion

        #region LinQ
        protected void GenerateEntityServerModeDataSource_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "SEQ";
            SqlConnection connectionString = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            string Userid = Convert.ToString(HttpContext.Current.Session["userid"]);
            ReportSourceDataContext dc = new ReportSourceDataContext(connectionString);

            if (Convert.ToString(Session["IsSubledgerCashBankDet"]) == "Y")
            {
                var q = from d in dc.SUBLEDGERWISECASHBANKBOOK_REPORTs
                        where Convert.ToString(d.USERID) == Userid
                        orderby d.SEQ ascending
                        select d;
                e.QueryableSource = q;
            }
            else
            {
                var q = from d in dc.SUBLEDGERWISECASHBANKBOOK_REPORTs
                        where Convert.ToString(d.SEQ) == "0"
                        orderby d.SEQ ascending
                        select d;
                e.QueryableSource = q;
            }

            string strChqNochk = (chkchqno.Checked) ? "1" : "0";
            if (Convert.ToString(strChqNochk) == "0")
            {
                ShowGrid.Columns[7].Visible = false;
            }
            else
            {
                ShowGrid.Columns[7].Visible = true;
            }

            string strChqDtchk = (chkchqdt.Checked) ? "1" : "0";
            if (Convert.ToString(strChqDtchk) == "0")
            {
                ShowGrid.Columns[8].Visible = false;
            }
            else
            {
                ShowGrid.Columns[8].Visible = true;
            }

            string strChqOnBnkchk = (chkchqonbnk.Checked) ? "1" : "0";
            if (Convert.ToString(strChqOnBnkchk) == "0")
            {
                ShowGrid.Columns[9].Visible = false;
            }
            else
            {
                ShowGrid.Columns[9].Visible = true;
            }

            string strHdNarrachk = (chkhdnarra.Checked) ? "1" : "0";
            if (Convert.ToString(strHdNarrachk) == "0")
            {
                ShowGrid.Columns[13].Visible = false;
            }
            else
            {
                ShowGrid.Columns[13].Visible = true;
            }

            if (lookupBranch.GridView.GetSelectedFieldValues("ID").Count == 1)
            {
                ShowGrid.Columns[1].Visible = false;
            }
            else
            {
                ShowGrid.Columns[1].Visible = true;
            }

            if (lookupCashBank.GridView.GetSelectedFieldValues("ID").Count == 1)
            {
                ShowGrid.Columns[2].Visible = false;
            }
            else
            {
                ShowGrid.Columns[2].Visible = true;
            }
            ShowGrid.ExpandAll();
        }
        #endregion

        protected void Project_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindProjectGrid")
            {
                DataTable ProjectTable = new DataTable();
                ProjectTable = GetProject();

                if (ProjectTable.Rows.Count > 0)
                {
                    Session["ProjectData"] = ProjectTable;
                    lookup_project.DataSource = ProjectTable;
                    lookup_project.DataBind();
                }
                else
                {
                    Session["ProjectData"] = ProjectTable;
                    lookup_project.DataSource = null;
                    lookup_project.DataBind();
                }
            }
        }

        public DataTable GetProject()
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("PRC_FETCHPROJECTS_REPORT", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();

            return dt;
        }

        protected void lookup_project_DataBinding(object sender, EventArgs e)
        {
            if (Session["ProjectData"] != null)
            {
                lookup_project.DataSource = (DataTable)Session["ProjectData"];
            }
        }
    }
}