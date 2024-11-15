﻿/******************************************************************************************************************************************************************************
*Rev 1.0      Sanchita      V2.0.40     19-10-2023     Coordinator data not showing in the following screen while linking Quotation/Inquiry Entries
                                                       Mantis : 26924
* ***************************************************************************************************************************************************************************/

using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using BusinessLogicLayer;
using ClsDropDownlistNameSpace;
using EntityLayer.CommonELS;
using System.Web.Services;
using System.Collections.Generic;
using ClsDropDownlistNameSpace;
using System.Collections;
using DevExpress.Web.Data;
using DataAccessLayer;
using System.ComponentModel;
using System.Linq;
using EntityLayer;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using ERP.OMS.Tax_Details.ClassFile;
using BusinessLogicLayer.EmailDetails;
using EntityLayer.MailingSystem;
using UtilityLayer;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Script.Services;
using ERP.Models;
using System.Globalization;


namespace ERP.OMS.Management.Activities
{
    public partial class EstimateCostSheet : ERP.OMS.ViewState_class.VSPage
    {
        // BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
        public static string IsLighterCustomePage = string.Empty;
        Export.ExportToPDF exportToPDF = new Export.ExportToPDF();
        BusinessLogicLayer.Converter objConverter = new BusinessLogicLayer.Converter();
        clsDropDownList oclsDropDownList = new clsDropDownList();
        BusinessLogicLayer.Contact oContactGeneralBL = new BusinessLogicLayer.Contact();
        public EntityLayer.CommonELS.UserRightsForPage PiQuotationRights = new UserRightsForPage();
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();
        static string ForJournalDate = null;
        SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
        CRMSalesDtlBL objCRMSalesDtlBL = new CRMSalesDtlBL();
        ERPDocPendingApprovalBL objERPDocPendingApproval = new ERPDocPendingApprovalBL();
        BusinessLogicLayer.RemarkCategoryBL reCat = new BusinessLogicLayer.RemarkCategoryBL();
        GSTtaxDetails gstTaxDetails = new GSTtaxDetails();
        string UniqueQuotation = string.Empty;
        public string pageAccess = "";
        string userbranch = "";
        MasterSettings objmaster = new MasterSettings();
        PosSalesInvoiceBl posSale = new PosSalesInvoiceBl();
        BusinessLogicLayer.Others objBL = new BusinessLogicLayer.Others();
        string QuotationIds = string.Empty;
        DataTable Remarks = null;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Request.QueryString.AllKeys.Contains("status") || Request.QueryString.AllKeys.Contains("SalId"))
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/PopUp.Master";
                //divcross.Visible = false;
            }
            else
            {
                this.Page.MasterPageFile = "~/OMS/MasterPage/ERP.Master";
                //divcross.Visible = true;
            }

            if (!IsPostBack)
            {



                string sPath = Convert.ToString(HttpContext.Current.Request.Url);


                oDBEngine.Call_CheckPageaccessebility(sPath);
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {

            //if (!IsPostBack)
            //{
            //    ((GridViewDataComboBoxColumn)grid.Columns["ProductID"]).PropertiesComboBox.DataSource = GetProduct();
            //}
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Cross_CloseWindow.Visible = false;
            PiQuotationRights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/management/master/CustomerMasterList.aspx");
            rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/Management/Activities/EstimateCostSheetList.aspx");
            if (HttpContext.Current.Session["userid"] == null)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "SighOff", "<script>SignOff();</script>");
            }
            if (!String.IsNullOrEmpty(Request.QueryString["SalId"]))
            {
                Cross_CloseWindow.Visible = true;
            }

            string ConURL = "EstimateCostSheet.aspx";

            if (Request.Url.AbsoluteUri.Contains(ConURL))
            {

            }
            if (hdnApproveStatus.Value == "")
            {
                hdnApproveStatus.Value = "0";
            }

            //chinmoy added for MUltiUOM settings start
            CommonBL ComBL = new CommonBL();
            string MultiUOMSelection = ComBL.GetSystemSettingsResult("MultiUOMSelection");
            string ProjectSelectInEntryModule = ComBL.GetSystemSettingsResult("ProjectSelectInEntryModule");
            string ProjectMandatoryInEntry = ComBL.GetSystemSettingsResult("ProjectMandatoryInEntry");
            //Rev work start 24.06.2022 mantise no:0024987            
            string SalesmanCaption = ComBL.GetSystemSettingsResult("ShowCoordinator");

            if (!String.IsNullOrEmpty(SalesmanCaption))
            {
                if (SalesmanCaption.ToUpper().Trim() == "NO")
                {
                    ASPxLabel3.Text = "Salesman/Agents";
                    hs1.InnerText = "Salesman/Agents";
                }
                else if (SalesmanCaption.ToUpper().Trim() == "YES")
                {
                    ASPxLabel3.Text = "Coordinator";
                    hs1.InnerText = "Coordinator";
                }
            }
            //Rev work close 24.06.2022 mantise no:0024987
            if (!String.IsNullOrEmpty(MultiUOMSelection))
            {
                if (MultiUOMSelection.ToUpper().Trim() == "YES")
                {
                    hddnMultiUOMSelection.Value = "1";
                }
                else if (MultiUOMSelection.ToUpper().Trim() == "NO")
                {
                    hddnMultiUOMSelection.Value = "0";
                    //Rev Bapi
                    //grid.Columns[11].Width = 0;
                     
                    grid.Columns[11].Width = 0;
                    grid.Columns[12].Width = 0;
                    grid.Columns[13].Width = 0;
                    //End REv Bapi
                }
            }
            string HierarchySelectInEntryModule = ComBL.GetSystemSettingsResult("Show_Hierarchy");
            if (!String.IsNullOrEmpty(HierarchySelectInEntryModule))
            {
                if (HierarchySelectInEntryModule.ToUpper().Trim() == "YES")
                {
                    ddlHierarchy.Visible = true;
                    lblHierarchy.Visible = true;
                    lookup_Project.Columns[3].Visible = true;
                }
                else if (HierarchySelectInEntryModule.ToUpper().Trim() == "NO")
                {
                    ddlHierarchy.Visible = false;
                    lblHierarchy.Visible = false;
                    lookup_Project.Columns[3].Visible = false;
                }
            }
            if (!String.IsNullOrEmpty(ProjectSelectInEntryModule))
            {
                if (ProjectSelectInEntryModule == "Yes")
                {
                    hdnProjectSelectInEntryModule.Value = "1";
                    lookup_Project.ClientVisible = true;
                    lblProject.Visible = true;


                }
                else if (ProjectSelectInEntryModule.ToUpper().Trim() == "NO")
                {
                    hdnProjectSelectInEntryModule.Value = "0";
                    lookup_Project.ClientVisible = false;
                    lblProject.Visible = false;

                }
            }

            if (!String.IsNullOrEmpty(ProjectMandatoryInEntry))
            {
                if (ProjectMandatoryInEntry == "Yes")
                {
                    hdnProjectMandatory.Value = "1";
                  }
                else if (ProjectMandatoryInEntry.ToUpper().Trim() == "NO")
                {
                    hdnProjectMandatory.Value = "0";
                }
            }

            string ApprovalRevisionEstimateCostSheet = ComBL.GetSystemSettingsResult("ApprovalRevisionEstimateCostSheet");
            if (!String.IsNullOrEmpty(ApprovalRevisionEstimateCostSheet))
            {
                if (ApprovalRevisionEstimateCostSheet == "Yes")
                {
                    hdnApprovalsettings.Value = "1";
                }
                else if (ApprovalRevisionEstimateCostSheet.ToUpper().Trim() == "NO")
                {
                    hdnApprovalsettings.Value = "0";
                }
            }

            bindHierarchy();
            ddlHierarchy.Enabled = false;
            //End
            if (!IsPostBack)
            {
                Session["SO_ProductDetails"] = null;
                CountrySelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                StateSelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectCity.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectArea.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                SelectPin.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                sqltaxDataSource.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                ProductDataSource.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                UomSelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                AltUomSelect.ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
                ////#region NewTaxblock
                string ItemLevelTaxDetails = string.Empty; string HSNCodewisetaxSchemid = string.Empty; string BranchWiseStateTax = string.Empty; string StateCodeWiseStateIDTax = string.Empty, taxException = string.Empty;
                gstTaxDetails.GetTaxDataWithException(ref ItemLevelTaxDetails, ref HSNCodewisetaxSchemid, ref BranchWiseStateTax, ref StateCodeWiseStateIDTax, ref taxException
                    , "S");
                HDItemLevelTaxDetails.Value = ItemLevelTaxDetails;
                HDHSNCodewisetaxSchemid.Value = HSNCodewisetaxSchemid;
                HDBranchWiseStateTax.Value = BranchWiseStateTax;
                HDStateCodeWiseStateIDTax.Value = StateCodeWiseStateIDTax;
                HDTaxexception.Value = taxException;
                ////#endregion
                uniqueId.Value = Guid.NewGuid().ToString();
                hdnGuid.Value = uniqueId.Value;
                hidIsLigherContactPage.Value = "0";
                IsLighterCustomePage = "";
                CommonBL cbl = new CommonBL();
                Session["SO_ProductDetails"] = null;
                Session["IECSnquiryData"] = null;
                string ISLigherpage = cbl.GetSystemSettingsResult("LighterCustomerEntryPage");
                if (!String.IsNullOrEmpty(ISLigherpage))
                {
                    if (ISLigherpage == "Yes")
                    {
                        hidIsLigherContactPage.Value = "1";
                        IsLighterCustomePage = "1";
                    }
                }
                ddlInventory.Focus();
                //#region Sandip Section For Checking User Level for Allow Edit to logging User or Not
                GetEditablePermission();
                PopulateCustomerDetail();
                SetFinYearCurrentDate();
                GetFinacialYearBasedQouteDate();
                if (Session["userbranchHierarchy"] != null)
                {
                    userbranch = Convert.ToString(Session["userbranchHierarchy"]);
                }
                GetAllDropDownDetailForSalesQuotation(userbranch);

                if (Request.QueryString.AllKeys.Contains("status"))
                {
                    divcross.Visible = false;
                    btn_SaveRecords.Visible = false;
                    ApprovalCross.Visible = true;
                    ddl_Branch.Enabled = false;
                    openlink.Visible = false;
                }
                else if (!string.IsNullOrEmpty(Request.QueryString["SalId"]))
                {
                    divcross.Visible = false;
                    openlink.Visible = false;
                }
                else
                {
                    divcross.Visible = true;
                    btn_SaveRecords.Visible = true;
                    ApprovalCross.Visible = false;
                    ddl_Branch.Enabled = false; // Change Due to Numbering Schema
                    openlink.Visible = true;
                }

                //dt_PlQuoteExpiry.Value = Convert.ToDateTime(oDBEngine.GetDate().ToString());
                string finyear = Convert.ToString(Session["LastFinYear"]);
                IsUdfpresent.Value = Convert.ToString(getUdfCount());
                hdnAddressDtl.Value = "0";
                Session["BillingAddressLookup"] = null;
                Session["ShippingAddressLookup"] = null;
                Session["key_QutId"] = null;
                Session["EstimateCost_Id"] = "";
                Session["CustomerDetail"] = null;
                Session["EstimateCostDetails"] = null;
                Session["WarehouseData"] = null;
                Session["MultiUOMData"] = null;
                Session["QuotationTaxDetails"] = null;
                Session["LoopWarehouse"] = 1;
                Session["TaxDetails"] = null;
                Session["ActionType"] = "";
                Session["SI_EstimateCostDetails" + Convert.ToString(uniqueId.Value)] = null;
                PopulateGSTCSTVATCombo(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                PopulateChargeGSTCSTVATCombo(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                hdnQuantitySL.Value = "0";
                string strQuotationId = "";
                hdnCustomerId.Value = "";
                hdnPageStatus.Value = "first";
                Session["QuotationAddressDtl"] = null;
                Session["InlineRemarks"] = null;
                string strApprovestatus = "";
                if (Request.QueryString["key"] != null)
                {
                    //Rev Rajdip
                    Session["key_QutId"] = Convert.ToString(Request.QueryString["key"]);
                    if ((Convert.ToString(Request.QueryString["key"]) != "ADD") && (Convert.ToString(Request.QueryString["Typenew"]) != "Copy"))
                    //ENd Rev Rajdip
                    {

                        hddnCustIdFromCRM.Value = "0";
                       
                        hdnPageStatus.Value = "update";
                        divScheme.Style.Add("display", "none");
                        strQuotationId = Convert.ToString(Request.QueryString["key"]);
                        Keyval_internalId.Value = "PIQUOTE" + strQuotationId;
                        //#region Subhra Section
                        Session["key_QutId"] = strQuotationId;
                        //if (Request.QueryString["status"] == null)
                        //{
                        //    IsExistsDocumentInERPDocApproveStatus(strQuotationId);
                        //}
                        //#endregion End Section
                        //#region Product, Quotation Tax, Warehouse - Sudip
                        Session["EstimateCost_Id"] = strQuotationId;
                        Session["ActionType"] = "Edit";

                        strApprovestatus = Convert.ToString(Request.QueryString["type1"]);
                        hdnPageStatForApprove.Value = strApprovestatus;
                        hdnEditOrderId.Value = strQuotationId;
                        if (Request.QueryString["type1"] == "ACECS")
                        {
                            lblHeadTitle.Text = "Approve Estimate/Cost Sheet";
                            btn_SaveRecords.ClientVisible = false;
                            ASPxButton1.ClientVisible = false;

                        }
                        else
                        {
                            lblHeadTitle.Text = "Modify Estimate/Cost Sheet";
                        }
                        SetEstimateCostDetails();
                       
                        if (Convert.ToString(Request.QueryString["Permission"]) == "4" && hdnApprovalsettings.Value == "0")
                        {

                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            ApprovalCross.Visible = false;
                            ddl_Branch.Enabled = false; // Change Due to Numbering Schema
                            openlink.Visible = true;
                            lbl_quotestatusmsg.Visible = true;
                            ASPxButton2.Visible = false;
                            //ASPxButton3.Visible = false;
                            lbl_quotestatusmsg.Text = "*** Estimate/Cost Sheet Already Closed,Can't Modify";

                        }
                        //REV RAJDIP
                        if (Convert.ToString(Request.QueryString["Permission"]) == "5" && hdnApprovalsettings.Value == "0")
                        {
                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            lbl_quotestatusmsg.Text = "Estimate/Cost Sheet Already Used in Sales Order.";
                            lbl_quotestatusmsg.Visible = true;
                        }

                       


                        Session["EstimateCostDetails"] = GetQuotationData().Tables[0];
                        Session["InlineRemarks"] = GetQuotationData().Tables[1];
                        Session["MultiUOMData"] = GetMultiUOMData();

                        DataTable Orderdt = GetQuotationData().Tables[0];
                        decimal Total_Qty_Mtr = 0;
                        decimal COSTVALUE = 0;
                        decimal TotalWt = 0;
                        decimal TotalSqMtr = 0;
                        decimal ToleranceTotalWt = 0;
                        decimal SalesVALUE = 0;
                        decimal ProfitMargin = 0;
                        for (int i = 0; i < Orderdt.Rows.Count; i++)
                        {
                            Total_Qty_Mtr = Total_Qty_Mtr + Convert.ToDecimal(Orderdt.Rows[i]["EstCostDetails_Qty_Mtr"]);
                            TotalSqMtr = TotalSqMtr + Convert.ToDecimal(Orderdt.Rows[i]["EstCostDetails_Sqmtr"]);
                            TotalWt = TotalWt + Convert.ToDecimal(Orderdt.Rows[i]["EstCostDetails_T_WT"]);
                            ToleranceTotalWt = ToleranceTotalWt + Convert.ToDecimal(Orderdt.Rows[i]["EstCostDetails_Tol_t_wt"]);
                            COSTVALUE = COSTVALUE + Convert.ToDecimal(Orderdt.Rows[i]["EstCostDetails_FOR_CP_VAL"]);
                            SalesVALUE = SalesVALUE + Convert.ToDecimal(Orderdt.Rows[i]["Value"]);
                        }
                        //AmountWithTaxValue = TaxAmount + Amount;
                        lbl_Total_Qty_Mtr.Text = Total_Qty_Mtr.ToString();
                        bnrLblTotalSqMtr.Text = TotalSqMtr.ToString();
                        bnrLblTotalWt.Text = TotalWt.ToString();
                        bnrlblToleranceTotalWt.Text = ToleranceTotalWt.ToString();
                        bnrCOSTVALUE.Text = COSTVALUE.ToString();
                        bnrSalesValue.Text = SalesVALUE.ToString();
                        string TOTALProfitMargin = "";
                        if (COSTVALUE != 0)
                        {
                            TOTALProfitMargin = (Math.Round(((SalesVALUE - COSTVALUE) / COSTVALUE) * 100, 2)).ToString();
                        }
                        bnrLblProfitMargin.Text = TOTALProfitMargin;

                      
                        grid.DataSource = GetQuotation();
                        grid.DataBind();
                       
                        //string Docids = "";
                        //DataSet dsOrderEditdt = GetQuotationEditData();
                        //DataTable OrderEditdt = dsOrderEditdt.Tables[0];
                        //DataTable OrderDocIdsEditdt = dsOrderEditdt.Tables[2];
                        //if (OrderEditdt != null && OrderEditdt.Rows.Count > 0)
                        //{
                        //    QuotationIds = string.Empty;
                        //    for (int i = 0; i < OrderDocIdsEditdt.Rows.Count; i++)
                        //    {
                        //        Docids += "," + Convert.ToString(OrderDocIdsEditdt.Rows[i]["InquiryID"]);

                        //    }
                        //    QuotationIds = Docids;
                        //    QuotationIds = QuotationIds.TrimStart(',');
                        // //   string Quoids = Convert.ToString(OrderEditdt.Rows[0]["Inquiry_doc_id"]);
                        //    //Session["Lookup_QuotationIds"] = QuotationIds;
                        //    string Order_Date = Convert.ToString(OrderEditdt.Rows[0]["EstimateCost_Date"]);
                        //    string Customer_Id = Convert.ToString(OrderEditdt.Rows[0]["Customer_Id"]);
                        //    if (!String.IsNullOrEmpty(QuotationIds) && QuotationIds != "0")
                        //    {
                        //        string[] eachQuo = QuotationIds.Split(',');
                        //        if (eachQuo.Length > 1)//More than one quotation
                        //        {

                        //            dt_Quotation.Text = "Multiple Select Inquiry Dates";
                        //            BindLookUp(Customer_Id, Order_Date);
                        //            foreach (string val in eachQuo)
                        //            {
                        //                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        //            }
                        //            // lbl_MultipleDate.Attributes.Add("style", "display:block");
                        //        }
                        //        else if (eachQuo.Length == 1)//Single Quotation
                        //        { //lbl_MultipleDate.Attributes.Add("style", "display:none"); }
                        //            BindLookUp(Customer_Id, Order_Date);
                        //            foreach (string val in eachQuo)
                        //            {
                        //                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                        //            }
                        //        }
                        //        else//No Quotation selected
                        //        {
                        //            BindLookUp(Customer_Id, Order_Date);
                        //        }
                        //    }
                        //}

                        //#endregion Debjyoti Get Tax Details in Edit Mode
                        //#region Samrat Roy -- Hide Save Button in Edit Mode
                        if (Request.QueryString["req"] != null && Request.QueryString["req"] == "V")
                        {
                            lblHeadTitle.Text = "View Estimate/Cost Sheet";
                            lbl_quotestatusmsg.Text = "*** View Mode Only";
                            btn_SaveRecords.Visible = false;
                            ASPxButton1.Visible = false;
                            lbl_quotestatusmsg.Visible = true;
                        }
                        //#endregion
                    }
                    //Rev Rajdip
                    else if ((Convert.ToString(Request.QueryString["key"]) != "ADD") && (Convert.ToString(Request.QueryString["Typenew"]) == "Copy"))
                    {
                        hddnCustIdFromCRM.Value = "0";
                        lblHeadTitle.Text = "Copy Estimate/Cost Sheet";
                        hdnPageStatus.Value = "Add";
                        Keyval_internalId.Value = "Add";
                        strQuotationId = Convert.ToString(Request.QueryString["key"]);
                        //#region Subhra Section
                        Session["key_QutId"] = strQuotationId;
                        if (Request.QueryString["status"] == null)
                        {
                            IsExistsDocumentInERPDocApproveStatus(strQuotationId);
                        }
                        //#endregion End Section
                        //#region Product, Quotation Tax, Warehouse - Sudip
                        Session["EstimateCost_Id"] = strQuotationId;
                        Session["ActionType"] = "Add";
                        SetEstimateCostDetails();
                        ddlInventory.Enabled = true;
                        txt_PLQuoteNo.Enabled = true;
                        txt_PLQuoteNo.Text = "";
                        //  DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("dd/MM/yyyy"));
                        Session["TaxDetails"] = GetTaxDataWithGST(GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd")));
                        Session["WarehouseData"] = GetQuotationWarehouseData();
                        Session["EstimateCostDetails"] = GetQuotationDataForCopy().Tables[0];
                        Session["MultiUOMData"] = GetMultiUOMData();
                        //rev rajdip for running data on edit mode
                        DataTable Orderdt = GetQuotationData().Tables[0];
                        decimal TotalQty = 0;
                        decimal TotalAmt = 0;
                        decimal TaxAmount = 0;
                        decimal Amount = 0;
                        decimal SalePrice = 0;
                        decimal AmountWithTaxValue = 0;
                        for (int i = 0; i < Orderdt.Rows.Count; i++)
                        {
                            TotalQty = TotalQty + Convert.ToDecimal(Orderdt.Rows[i]["Quantity"]);
                            Amount = Amount + Convert.ToDecimal(Orderdt.Rows[i]["Amount"]);
                            TaxAmount = TaxAmount + Convert.ToDecimal(Orderdt.Rows[i]["TaxAmount"]);
                            SalePrice = SalePrice + Convert.ToDecimal(Orderdt.Rows[i]["SalePrice"]);
                            TotalAmt = TotalAmt + Convert.ToDecimal(Orderdt.Rows[i]["TotalAmount"]);
                        }
                        AmountWithTaxValue = TaxAmount + Amount;
                        //ASPxLabel12.Text = TotalQty.ToString();
                        //bnrLblTaxableAmtval.Text = Amount.ToString();
                        //bnrLblTaxAmtval.Text = TaxAmount.ToString();
                        //bnrlblAmountWithTaxValue.Text = AmountWithTaxValue.ToString();
                       // bnrLblInvValue.Text = TotalAmt.ToString();
                        //end rev rajdip
                        grid.DataSource = GetQuotation();
                        grid.DataBind();
                        //#endregion
                        //#region Debjyoti Get Tax Details in Edit Mode
                        Session["QuotationTaxDetails"] = GetQuotationTaxData().Tables[0];
                        DataTable quotetable = GetQuotationEditedTaxData().Tables[0];
                        if (quotetable == null)
                        {
                            CreateDataTaxTable();
                        }
                        else
                        {
                            Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = quotetable;
                        }

                    }
                    //End Rev Rajdip
                    else
                    {
                        //#region To Show By Default Cursor after SAVE AND NEW
                        if (Session["SaveMode"] != null)  // it has been removed from coding side of Quotation list 
                        {
                            if (Session["schemavalue"] != null)  // it has been removed from coding side of Quotation list 
                            {
                                ddl_numberingScheme.SelectedValue = Convert.ToString(Session["schemavalue"]); // it has been removed from coding side of Quotation list 
                            }
                            if (Convert.ToString(Session["SaveMode"]) == "A")
                            {
                                dt_PLQuote.Focus();
                                txt_PLQuoteNo.ClientEnabled = false;
                                txt_PLQuoteNo.Text = "Auto";
                            }
                            else if (Convert.ToString(Session["SaveMode"]) == "M")
                            {
                                txt_PLQuoteNo.ClientEnabled = true;
                                txt_PLQuoteNo.Text = "";
                                txt_PLQuoteNo.Focus();
                            }
                        }
                        else
                        {
                            ddlInventory.Focus();
                        }
                        //#endregion To Show By Default Cursor after SAVE AND NEW

                        if (!string.IsNullOrEmpty(Request.QueryString["type"]) && !string.IsNullOrEmpty(Request.QueryString["SalId"]))
                        {
                            hddnCustIdFromCRM.Value = "1";
                            //string strCustomer = oDBEngine.ExeSclar("select sls_contactlead_id from tbl_trans_sales where sls_id=" + Request.QueryString["SalId"]); 
                            //string strCustomerName = oDBEngine.ExeSclar("isnull(TMC.cnt_firstName,'')+isnull(TMC.cnt_middleName,'')+isnull(TMC.cnt_lastName,'')  CustName from tbl_trans_sales INNER JOIN tbl_master_contact tmc ON sls_contactlead_id = tmc.cnt_internalId where sls_id=" + Request.QueryString["SalId"]);
                            DataTable dtcust = getCustomerbyslsid(Request.QueryString["SalId"]);
                            if (dtcust.Rows.Count > 0)
                            {
                                hdnCustomerId.Value = dtcust.Rows[0]["sls_contactlead_id"].ToString();
                                txtCustName.Value = dtcust.Rows[0]["CustName"].ToString();
                            }

                            //DataTable dt = CallgridfromSales();
                            grid.JSProperties["cpSaveSuccessOrFail"] = "Addnewrowfromsalesactivty";
                            Session["EstimateCostDetails"] = null;
                            grid.DataSource = null;
                            grid.DataBind();
                            hdnIsFromActivity.Value = "Y";
                            btn_SaveRecords.Visible = false;
                        }
                        else
                        {
                            hddnCustIdFromCRM.Value = "0";
                        }

                        Session["ActionType"] = "Add";
                        //ASPxButton2.Enabled = false;
                        Session["TaxDetails"] = null;
                        CreateDataTaxTable();
                        lblHeadTitle.Text = "Add Estimate/Cost Sheet";
                        //ddl_AmountAre.Value = "1";
                        ddl_VatGstCst.SelectedIndex = 0;
                        ddl_VatGstCst.ClientEnabled = false;
                        Keyval_internalId.Value = "Add";
                        if (Request.QueryString["BasketId"] != null)
                        {
                            string basketId = Convert.ToString(Request.QueryString["BasketId"]);
                            fillRecordFromBasket(basketId);
                            hdBasketId.Value = basketId;
                        }
                        else
                        {
                            hdBasketId.Value = "";
                        }
                        //ddl_AmountAre.Value = objmaster.GetSettings("DefaultTaxTypeForSalesQuotation");

                    }
                }

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "GridCallBack()", true);
                hdnConvertionOverideVisible.Value = objmaster.GetSettings("ConvertionOverideVisible");
                hdnShowUOMConversionInEntry.Value = objmaster.GetSettings("ShowUOMConversionInEntry");

            }
            else
            {
                PopulateCustomerDetail();
            }
        }

        //#region Other Section
        public void IsExistsDocumentInERPDocApproveStatus(string strQuotationId)
        {
            string editable = "";
            string status = "";
            DataTable dt = new DataTable();
            int quoteid = Convert.ToInt32(strQuotationId);
            dt = objERPDocPendingApproval.IsExistsDocumentInERPDocApproveStatus(quoteid, 1);
            if (dt.Rows.Count > 0)
            {
                editable = Convert.ToString(dt.Rows[0]["editable"]);
                if (editable == "0")
                {
                    lbl_quotestatusmsg.Visible = true;
                    status = Convert.ToString(dt.Rows[0]["Status"]);
                    if (status == "Approved")
                    {
                        lbl_quotestatusmsg.Text = "Document already Approved";
                    }
                    if (status == "Rejected")
                    {
                        lbl_quotestatusmsg.Text = "Document already Rejected";
                    }
                    btn_SaveRecords.Visible = false;
                    ASPxButton1.Visible = false;
                }
                else
                {
                    lbl_quotestatusmsg.Visible = false;
                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
            }
        }
        public void GetEditablePermission()
        {
            if (Request.QueryString["Permission"] != null)
            {
                if (Convert.ToString(Request.QueryString["Permission"]) == "1")
                {

                    btn_SaveRecords.Visible = false;
                    ASPxButton1.Visible = false;
                }
                //REV RAJDIP
                if (Convert.ToString(Request.QueryString["Permission"]) == "4")
                {
                    btn_SaveRecords.Visible = false;
                    ASPxButton1.Visible = false;
                    lbl_quotestatusmsg.Text = "Estimate/Cost Sheet Already Closed";
                }
                //REV RAJDIP
                else if (Convert.ToString(Request.QueryString["Permission"]) == "2")
                {

                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
                else if (Convert.ToString(Request.QueryString["Permission"]) == "3")
                {

                    btn_SaveRecords.Visible = true;
                    ASPxButton1.Visible = true;
                }
            }
        }
        protected int getUdfCount()
        {
            DataTable udfCount = oDBEngine.GetDataTable("select 1 from tbl_master_remarksCategory rc where cat_applicablefor='SQO'  and ( exists (select * from tbl_master_udfGroup where id=rc.cat_group_id and grp_isVisible=1) or rc.cat_group_id=0)");
            return udfCount.Rows.Count;
        }
        public DataTable getCustomerbyslsid(string sls_id)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "QuotationCustomerbyslsid");
            proc.AddVarcharPara("@sls_id", 4000, sls_id);
            dt = proc.GetTable();
            return dt;
        }
        public DataTable CallgridfromSales()
        {
            string Sproduct = "";
            int id;

            try
            {
                if (Request.QueryString["type"] == "1")
                {
                    id = oDBEngine.ExeInteger("select sls_product_id from tbl_trans_sales where sls_id=" + Request.QueryString["SalId"]);
                    Sproduct = Convert.ToString(id);
                }
                else
                {
                    Sproduct = oDBEngine.ExeSclar("select a.act_productIds from tbl_trans_sales as s,tbl_trans_activies  as a  where s.sls_activity_id=a.act_id and  s.sls_id=" + Request.QueryString["SalId"]);
                }

                DataTable dtsalesproduct = new DataTable();
                dtsalesproduct = GetSalesAcivityProduct(Sproduct);

                return dtsalesproduct;
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetSalesAcivityProduct(string Sproduct)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetSalesAcitivityProduct");
            proc.AddVarcharPara("@ProductList", 4000, Sproduct);
            dt = proc.GetTable();
            return dt;
        }
        protected void grid_CustomUnboundColumnData(object sender, ASPxGridViewColumnDataEventArgs e)
        {
            if (e.Column.FieldName == "Number")
            {
                e.Value = string.Format("Item #{0}", e.ListSourceRowIndex);
            }
            if (e.Column.FieldName == "Warehouse")
            {
            }
        }
        protected void lookup_CustomerControlPanelMain_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            Session["CustomerDetail"] = null;
            PopulateCustomerDetail();
        }
        protected void CmbProduct_Init(object sender, EventArgs e)
        {
            ASPxComboBox cityCombo = sender as ASPxComboBox;
            cityCombo.DataSource = GetProduct();
        }

        //#endregion

        //#region Batch Edit Grid Section
        public DataTable GetProjectEditData(string SalesQuoteId)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "ProjectEditdata");
            proc.AddIntegerPara("@EstimateCostID", Convert.ToInt32(SalesQuoteId));
            dt = proc.GetTable();
            return dt;
        }
        public void SetEstimateCostDetails()
        {
            DataSet dsSalesQuotatioDetails = GetQuotationEditData();
            DataTable QuotationEditdt = dsSalesQuotatioDetails.Tables[0];//GetQuotationEditData();
            BillingShippingControl.SetBillingShippingTable(dsSalesQuotatioDetails.Tables[1]);
            if (QuotationEditdt != null && QuotationEditdt.Rows.Count > 0)
            {
                string Branch_Id = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_BranchId"]);
                string Quote_Number = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_Number"]);
                string Quote_Date = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_Date"]);
               
                string Customer_Id = Convert.ToString(QuotationEditdt.Rows[0]["Customer_Id"]);
                string CustName = Convert.ToString(QuotationEditdt.Rows[0]["CustName"]);
                string Contact_Person_Id = Convert.ToString(QuotationEditdt.Rows[0]["Contact_Person_Id"]);
                string Quote_Reference = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_Reference"]);

                string Quote_SalesmanId = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_SalesmanId"]);
                string Quote_SalesmanName = Convert.ToString(QuotationEditdt.Rows[0]["SalesmanName"]);
              
                string IsInventory = Convert.ToString(QuotationEditdt.Rows[0]["IsInventory"]);
           

                BillingShippingControl.SetBillingShippingTable(dsSalesQuotatioDetails.Tables[1]);
              
                string Order_ProjectRemarks = Convert.ToString(QuotationEditdt.Rows[0]["EstimateCost_Remarks"]);
                string Inquiry_DocIds_Date = Convert.ToString(QuotationEditdt.Rows[0]["Inquiry_DocIds_Date"]);
                txtProjRemarks.Text = Order_ProjectRemarks;

                string RevisionNo = Convert.ToString(QuotationEditdt.Rows[0]["ECS_RevisionNo"]);
                txtRevisionNo.Text = RevisionNo;
                string ApproveProjectRem = Convert.ToString(QuotationEditdt.Rows[0]["ECS_ApproveRejectREmarks"]);

                txtAppRejRemarks.Text = ApproveProjectRem;
                hdnApproveStatus.Value = Convert.ToString(QuotationEditdt.Rows[0]["ECS_ApproveStatus"]);

                if (!string.IsNullOrEmpty(Convert.ToString(QuotationEditdt.Rows[0]["ECS_RevisionDate"])))
                {
                    txtRevisionDate.Date = Convert.ToDateTime(QuotationEditdt.Rows[0]["ECS_RevisionDate"]);
                    txtRevisionDate.MinDate = Convert.ToDateTime(Convert.ToDateTime(QuotationEditdt.Rows[0]["ECS_RevisionDate"]).ToShortDateString());
                }

                ddlInventory.SelectedValue = IsInventory;
                ddlInventory.Enabled = false;


                ddl_numberingScheme.SelectedValue = Convert.ToString(QuotationEditdt.Rows[0]["Id"]);

                txt_PLQuoteNo.Text = Quote_Number;
                dt_PLQuote.Date = Convert.ToDateTime(Quote_Date);
          
                txtCustName.Text = CustName;
                hdnCustomerId.Value = Customer_Id;
                TabPage page = ASPxPageControl1.TabPages.FindByName("[B]illing/Shipping");
                page.ClientEnabled = true;
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Jc", "<script  language='javascript'>SettingTabStatus();</script>", true);
                PopulateContactPersonOfCustomer(Customer_Id);
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(Contact_Person_Id);
               // txt_Refference.Text = Quote_Reference;
                ddl_Branch.SelectedValue = Branch_Id;
                //ddl_SalesAgent.SelectedValue = Quote_SalesmanId;
                hdnSalesManAgentId.Value = Quote_SalesmanId;
                txtSalesManAgent.Text = Quote_SalesmanName;
               
                dt_PLQuote.ClientEnabled = true;

                DataTable dtt = GetProjectEditData(Convert.ToString(Session["EstimateCost_Id"]));
                if (dtt != null)
                {
                    lookup_Project.GridView.Selection.SelectRowByKey(Convert.ToInt64(dtt.Rows[0]["Proj_Id"]));

                    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                    DataTable dt2 = oDBEngine.GetDataTable("Select Hierarchy_ID from V_ProjectList where Proj_Id='" + Convert.ToInt64(dtt.Rows[0]["Proj_Id"]) + "'");
                    if (dt2.Rows.Count > 0 && dt2!=null)
                    {
                        ddlHierarchy.SelectedValue = Convert.ToString(dt2.Rows[0]["Hierarchy_ID"]);
                    }
                }

                string Docids = "";
                DataSet dsOrderEditdt = GetQuotationEditData();
                DataTable OrderEditdt = dsOrderEditdt.Tables[0];
                DataTable OrderDocIdsEditdt = dsOrderEditdt.Tables[2];
                if (OrderEditdt != null && OrderEditdt.Rows.Count > 0)
                {
                    QuotationIds = string.Empty;
                    for (int i = 0; i < OrderDocIdsEditdt.Rows.Count; i++)
                    {
                        Docids += "," + Convert.ToString(OrderDocIdsEditdt.Rows[i]["InquiryID"]);

                    }
                    QuotationIds = Docids;
                    QuotationIds = QuotationIds.TrimStart(',');

                    string Quoids = Convert.ToString(OrderEditdt.Rows[0]["InquiryID"]);

                    string Order_Date = Convert.ToString(OrderEditdt.Rows[0]["EstimateCost_Date"]);
                    string DocCustomer_Id = Convert.ToString(OrderEditdt.Rows[0]["Customer_Id"]);
                    if (!String.IsNullOrEmpty(QuotationIds) && QuotationIds.Split(',')[0] != "0")
                    {
                        lookup_Project.ClientEnabled = false;
                        Session["Lookup_QuotationIds"] = QuotationIds;
                        rdl_Salesquotation.SelectedValue = "SINQ";
                        string[] eachQuo = QuotationIds.Split(',');
                        if (eachQuo.Length > 1)//More than one quotation
                        {

                            dt_Quotation.Text = "Multiple Select Inquiry Dates";
                            if (Inquiry_DocIds_Date != "" && Inquiry_DocIds_Date != "0")
                            dt_Quotation.Text = Inquiry_DocIds_Date;
                            BindLookUp(DocCustomer_Id, Order_Date);
                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                            // lbl_MultipleDate.Attributes.Add("style", "display:block");
                        }
                        else if (eachQuo.Length == 1)//Single Quotation
                        { //lbl_MultipleDate.Attributes.Add("style", "display:none"); }
                            BindLookUp(DocCustomer_Id, Order_Date);
                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                            dt_Quotation.Text = Inquiry_DocIds_Date;
                        }
                        else//No Quotation selected
                        {
                            BindLookUp(DocCustomer_Id, Order_Date);
                        }
                    }
                }


            }
        }

        public void fillRecordFromBasket(string basketId)
        {
            PosSalesInvoiceBl pos = new PosSalesInvoiceBl();
            DataSet busketget = GetBasketData();

            //#region SetHeader
            if (busketget.Tables[0].Rows.Count > 0)
            {
                DataTable headerTable = busketget.Tables[0];
                string customerId = Convert.ToString(headerTable.Rows[0]["CustomerId"]);
                txtCustName.Text = Convert.ToString(headerTable.Rows[0]["CustomerName"]);
                string SalesmanId = Convert.ToString(headerTable.Rows[0]["SalesmanId"]);
                txtSalesManAgent.Text = Convert.ToString(headerTable.Rows[0]["SalesmanName"]);
                ddlInventory.SelectedIndex = 0;
                ddl_numberingScheme.SelectedIndex = 0;

                //ddl_AmountAre.SelectedIndex = 2;
                //ddl_AmountAre.ClientEnabled = false;
                //ddl_AmountAre.DataBind();
                hdnCustomerId.Value = customerId;
                hdnSalesManAgentId.Value = SalesmanId;

                PopulateGSTCSTVAT("2");
            }

            BillingShippingControl.SetBillingShippingTable(busketget.Tables[1]);
            DataSet busketSet = pos.GetQuotationBusketById(Convert.ToInt32(basketId));
            DataTable DetailGrid = busketSet.Tables[0];
            DataTable GrdpackQty = busketSet.Tables[3];

            // hdnUomqnty.Value = Convert.ToString(GrdpackQty.Rows[0]["UOMQunty"]);

            #region SetShippingState
            string ShippingCode = "";
            if (busketSet.Tables[2].Rows.Count > 0)
            {
                ShippingCode = Convert.ToString(busketSet.Tables[2].Rows[0][0]);
            }



            #endregion
            DetailGrid = gstTaxDetails.SetTaxAmountWithGSTonDetailsTable(DetailGrid, "sProductsID", "TaxAmount", "Amount", "TotalAmount", dt_PLQuote.Date.ToString("yyyy-MM-dd"), "S", Convert.ToString(ddl_Branch.SelectedValue), ShippingCode, "I");
            CreateDataTaxTable();
            DataTable taxtable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
            taxtable = gstTaxDetails.SetTaxTableDataWithProductSerial(DetailGrid, "SrlNo", "sProductsID", "Amount", "TaxAmount", taxtable, "S", dt_PLQuote.Date.ToString("yyyy-MM-dd"), Convert.ToString(ddl_Branch.SelectedValue), ShippingCode, "I");
            Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = taxtable;
            DetailGrid.Columns.Remove("sProductsID");
            string totalAmount = Convert.ToString(DetailGrid.Compute("SUM(TotalAmount)", string.Empty));
            // bnrlblAmountWithTaxValue.Text = totalAmount;
            // bnrLblInvValue.Text = totalAmount;
            DataTable duplicatedt2 = new DataTable();
            duplicatedt2.Columns.Add("productid", typeof(Int64));
            duplicatedt2.Columns.Add("pslno", typeof(Int32));
            duplicatedt2.Columns.Add("pQuantity", typeof(Decimal));
            duplicatedt2.Columns.Add("packing", typeof(Decimal));
            duplicatedt2.Columns.Add("PackingUom", typeof(Int32));
            duplicatedt2.Columns.Add("PackingSelectUom", typeof(Int32));
            HttpContext.Current.Session["SessionPackingDetails"] = GetWaitingProductDetails(GrdpackQty);
            if (HttpContext.Current.Session["SessionPackingDetails"] != null)
            {
                List<WaitingProductQuantity> obj = new List<WaitingProductQuantity>();
                obj = (List<WaitingProductQuantity>)HttpContext.Current.Session["SessionPackingDetails"];
                foreach (var item in obj)
                {
                    duplicatedt2.Rows.Add(item.productid, item.pslno, item.pQuantity, item.packing, item.PackingUom, item.PackingSelectUom);
                }
            }
            grid.DataSource = GetWaitingProductDetails(GrdpackQty);

            // HttpContext.Current.Session["SessionPackingDetails"] = null;
            //grid.DataSource = GetQuotationWaiting(DetailGrid);
            //grid.DataBind();
            ////Session["EstimateCostDetails" + Convert.ToString(uniqueId.Value)] = DetailGrid;
            //Session["EstimateCostDetails"] = DetailGrid;

            //#region setComponentProduct
            //if (busketSet.Tables[2].Rows.Count > 0)
            //{
            //    isBasketContainComponent.Value = "yes";
            //}
            //#endregion


        }

        //#region Page Classes
        public class Products
        {
            public string ProductID { get; set; }
            public string ProductName { get; set; }
        }
        public class Tax
        {
            public string TaxID { get; set; }
            public string TaxName { get; set; }
            public string Percentage { get; set; }
            public string Amount { get; set; }
            public decimal calCulatedOn { get; set; }
        }
        public class Quotation
        {
            public string SrlNo { get; set; }
            public string EstCostDetails_Id { get; set; }
            public string ProductID { get; set; }
            public string Description { get; set; }
            public string Quantity { get; set; }
            public string UOM { get; set; }
           
            public string ProductName { get; set; }
            public string Remarks { get; set; }
           
            public string InquiryID { get; set; }
            public string DetailsId { get; set; }

            public string EstCostDetails_NB { get; set; }
            public string EstCostDetails_OD { get; set; }
            public string EstCostDetails_Thk { get; set; }
            public string EstCostDetails_Tol_thk { get; set; }  
            public string EstCostDetails_Wt_Mtr { get; set; }
            public string EstCostDetails_Tol_wt { get; set; }
            //public string EstCostDetails_Wt_Mtr { get; set; }
            public string EstCostDetails_Qty_Mtr { get; set; }
            public string EstCostDetails_Sqmtr { get; set; }
            public string EstCostDetails_T_WT { get; set; }
            public string EstCostDetails_Tol_t_wt { get; set; }
            public string EstCostDetails_Rate_kg { get; set; }
            public string EstCostDetails_Frt { get; set; }
            public string EstCostDetails_Surface_prep { get; set; }
            public string EstCostDetails_Int_Coating { get; set; }
            public string EstCostDetails_Ext_Coating { get; set; }
            public string EstCostDetails_Rate_sqm { get; set; }
            public string EstCostDetails_Ratekg { get; set; }
            public string EstCostDetails_CP_MT { get; set; }
            public string EstCostDetails_FOR_CP_MTR { get; set; }
            public string EstCostDetails_FOR_CP_VAL { get; set; }

             public string EstCostDetails_Gross_Margin { get; set; }
            public string EstCostDetails_Gross_Marg_Value_Mtr { get; set; }
            public string EstCostDetails_SP_Mtr { get; set; }
            public string Value { get; set; }
          
            public string Order_AltQuantity { get; set; }
            public string Order_AltUOM { get; set; }
           
        }
        //public class WaitingQuotation
        //{
        //    public string SrlNo { get; set; }
        //    public string QuotationID { get; set; }
        //    public string ProductID { get; set; }
        //    public string Description { get; set; }
        //    public string Quantity { get; set; }
        //    public string UOM { get; set; }
        //    public string Warehouse { get; set; }
        //    public string StockQuantity { get; set; }
        //    public string StockUOM { get; set; }
        //    public string SalePrice { get; set; }
        //    public string Discount { get; set; }
        //    public string Amount { get; set; }
        //    public string TaxAmount { get; set; }
        //    public string TotalAmount { get; set; }
        //    public string Status { get; set; }
        //    public string ProductName { get; set; }

        //}
        //#endregion

        //#region Product Details



        //#region Code Checked and Modified  By Sam on 17022016

        public IEnumerable GetProduct()
        {
            List<Products> ProductList = new List<Products>();
            //   BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();

            DataTable DT = GetProductData();
            //DataTable DT = GetProductData().Tables[0];
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Products Products = new Products();
                Products.ProductID = Convert.ToString(DT.Rows[i]["Products_ID"]);
                Products.ProductName = Convert.ToString(DT.Rows[i]["Products_Name"]);
                ProductList.Add(Products);
            }

            return ProductList;
        }
        //public IEnumerable GetFilterProduct(string filter)
        //{
        //    List<Product> ProductList = new List<Product>();
        //    BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
        //    DataTable DT = GetFilterProductData(filter);
        //    //DataTable DT = GetProductData().Tables[0];
        //    for (int i = 0; i < DT.Rows.Count; i++)
        //    {
        //        Product Products = new Product();
        //        Products.ProductID = Convert.ToString(DT.Rows[i]["Products_ID"]);
        //        Products.ProductName = Convert.ToString(DT.Rows[i]["Products_Name"]);
        //        ProductList.Add(Products);
        //    }

        //    return ProductList;
        //}
        public DataTable GetProductData()
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "ProductDetails");
            ds = proc.GetTable();
            return ds;
        }
        //public DataTable GetFilterProductData(string filter)
        //{
        //    DataTable ds = new DataTable();
        //    ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
        //    proc.AddVarcharPara("@Action", 500, "ProductFilterDetails");
        //    proc.AddVarcharPara("@Filter", 2000, filter);
        //    ds = proc.GetTable();
        //    return ds;
        //}

        //#endregion Code Checked and Modified  By Sam on 17022016


        //public DataSet GetProductData()
        //{
        //    DataSet ds = new DataSet();
        //    ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
        //    proc.AddVarcharPara("@Action", 500, "ProductDetails");
        //    ds = proc.GetDataSet();
        //    return ds;
        //}
        public DataTable GetWarehouseData()
        {

            MasterSettings masterBl = new MasterSettings();
            string multiwarehouse = Convert.ToString(masterBl.GetSettings("IaMultilevelWarehouse"));

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetWareHouseDtlByProductID");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            proc.AddVarcharPara("@Multiwarehouse", 500, Convert.ToString(multiwarehouse));
            dt = proc.GetTable();
            return dt;
        }
        public DataSet GetQuotationEditData()
        {
            DataSet dt = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "EstimateEditDetails");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetDataSet();
            return dt;
        }
        public DataSet GetBasketData()
        {
            DataSet dt = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "BhasketEstimateCostDetails");
            proc.AddBigIntegerPara("@SBMain_Id", Convert.ToInt64(Request.QueryString["BasketId"]));
            dt = proc.GetDataSet();
            return dt;
        }
        public DataTable GetProductDetailsData(string ProductID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "ProductDetailsSearch");
            proc.AddVarcharPara("@ProductID", 500, ProductID);
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetBatchData(string WarehouseID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetBatchByProductIDWarehouse");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public DataTable GetSerialata(string WarehouseID, string BatchID)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetSerialByProductIDWarehouseBatch");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            proc.AddVarcharPara("@ProductID", 500, Convert.ToString(hdfProductID.Value));
            proc.AddVarcharPara("@BatchID", 500, BatchID);
            proc.AddVarcharPara("@WarehouseID", 500, WarehouseID);
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 2000, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            dt = proc.GetTable();
            return dt;
        }
        public IEnumerable GetQuotation()
        {
            List<Quotation> QuotationList = new List<Quotation>();
            DataTable EstimateCostdt = GetQuotationData().Tables[0];

            for (int i = 0; i < EstimateCostdt.Rows.Count; i++)
            {
                Quotation Quotations = new Quotation();

                Quotations.SrlNo = Convert.ToString(EstimateCostdt.Rows[i]["SrlNo"]);
                Quotations.EstCostDetails_Id = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Id"]);
                if (EstimateCostdt.Columns.Contains("InquiryID"))
                {
                    Quotations.InquiryID = Convert.ToString(EstimateCostdt.Rows[i]["InquiryID"]);
                }
                
                Quotations.ProductID = Convert.ToString(EstimateCostdt.Rows[i]["ProductID"]);
                Quotations.Description = Convert.ToString(EstimateCostdt.Rows[i]["Description"]);
                Quotations.Quantity = Convert.ToString(EstimateCostdt.Rows[i]["Quantity"]);
                Quotations.UOM = Convert.ToString(EstimateCostdt.Rows[i]["UOM"]);
                
                Quotations.DetailsId = Convert.ToString(EstimateCostdt.Rows[i]["DetailsId"]);
                Quotations.EstCostDetails_NB = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_NB"]);                
                Quotations.EstCostDetails_OD = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_OD"]);
                Quotations.EstCostDetails_Thk = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Thk"]);
                Quotations.EstCostDetails_Tol_thk = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_thk"]);
                Quotations.EstCostDetails_Wt_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Wt_Mtr"]);
                Quotations.EstCostDetails_Tol_wt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_wt"]);

                Quotations.EstCostDetails_Qty_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Qty_Mtr"]);
                Quotations.EstCostDetails_Sqmtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Sqmtr"]);
                Quotations.EstCostDetails_T_WT = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_T_WT"]);
                Quotations.EstCostDetails_Tol_t_wt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_t_wt"]);
                Quotations.EstCostDetails_Rate_kg = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Rate_kg"]);


                Quotations.EstCostDetails_Frt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Frt"]);
                Quotations.EstCostDetails_Surface_prep = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Surface_prep"]);
               // Quotations.EstCostDetails_Int_Coating = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Int_Coating"]);
                //Quotations.EstCostDetails_Ext_Coating = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Ext_Coating"]);
                Quotations.EstCostDetails_Rate_sqm = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Rate_sqm"]);

                Quotations.EstCostDetails_CP_MT = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_CP_MT"]);
                Quotations.EstCostDetails_FOR_CP_MTR = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_FOR_CP_MTR"]);
                Quotations.EstCostDetails_FOR_CP_VAL = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_FOR_CP_VAL"]);
                Quotations.EstCostDetails_Gross_Margin = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Gross_Margin"]);
               // Quotations.EstCostDetails_Gross_Marg_Value_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Gross_Marg_Value_Mtr"]);


                Quotations.EstCostDetails_SP_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_SP_Mtr"]);
                Quotations.Value = Convert.ToString(EstimateCostdt.Rows[i]["Value"]);
                Quotations.ProductName = Convert.ToString(EstimateCostdt.Rows[i]["ProductName"]);
                if (EstimateCostdt.Columns.Contains("Remarks"))
                {
                    Quotations.Remarks = Convert.ToString(EstimateCostdt.Rows[i]["Remarks"]);
                }
                else
                {
                    Quotations.Remarks = "";
                }
                Quotations.DetailsId = Convert.ToString(EstimateCostdt.Rows[i]["DetailsId"]);
                QuotationList.Add(Quotations);
            }

            return QuotationList;
        }
        public IEnumerable GetQuotation(DataTable EstimateCostdt)
        {
            List<Quotation> QuotationList = new List<Quotation>();

            if (EstimateCostdt != null && EstimateCostdt.Rows.Count > 0)
            {
                for (int i = 0; i < EstimateCostdt.Rows.Count; i++)
                {
                    Quotation Quotations = new Quotation();

                    Quotations.SrlNo = Convert.ToString(EstimateCostdt.Rows[i]["SrlNo"]);
                    Quotations.EstCostDetails_Id = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Id"]);
                    Quotations.ProductID = Convert.ToString(EstimateCostdt.Rows[i]["ProductID"]);
                    Quotations.Description = Convert.ToString(EstimateCostdt.Rows[i]["Description"]);
                    Quotations.Quantity = Convert.ToString(EstimateCostdt.Rows[i]["Quantity"]);
                    Quotations.UOM = Convert.ToString(EstimateCostdt.Rows[i]["UOM"]);
                   

                    Quotations.InquiryID = Convert.ToString(EstimateCostdt.Rows[i]["InquiryID"]);
                    Quotations.DetailsId = Convert.ToString(EstimateCostdt.Rows[i]["DetailsId"]);
                    Quotations.EstCostDetails_NB = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_NB"]);     
                    Quotations.EstCostDetails_OD = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_OD"]);
                    Quotations.EstCostDetails_Thk = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Thk"]);
                    Quotations.EstCostDetails_Tol_thk = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_thk"]);
                    Quotations.EstCostDetails_Wt_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Wt_Mtr"]);
                    Quotations.EstCostDetails_Tol_wt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_wt"]);

                    Quotations.EstCostDetails_Qty_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Qty_Mtr"]);
                    Quotations.EstCostDetails_Sqmtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Sqmtr"]);
                    Quotations.EstCostDetails_T_WT = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_T_WT"]);
                    Quotations.EstCostDetails_Tol_t_wt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Tol_t_wt"]);
                    Quotations.EstCostDetails_Rate_kg = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Rate_kg"]);


                    Quotations.EstCostDetails_Frt = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Frt"]);
                    Quotations.EstCostDetails_Surface_prep = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Surface_prep"]);
                    //Quotations.EstCostDetails_Int_Coating = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Int_Coating"]);
                    //Quotations.EstCostDetails_Ext_Coating = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Ext_Coating"]);
                    Quotations.EstCostDetails_Rate_sqm = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Rate_sqm"]);

                    Quotations.EstCostDetails_CP_MT = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_CP_MT"]);
                    Quotations.EstCostDetails_FOR_CP_MTR = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_FOR_CP_MTR"]);
                    Quotations.EstCostDetails_FOR_CP_VAL = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_FOR_CP_VAL"]);
                    Quotations.EstCostDetails_Gross_Margin = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Gross_Margin"]);
                    //Quotations.EstCostDetails_Gross_Marg_Value_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Gross_Marg_Value_Mtr"]);


                    Quotations.EstCostDetails_SP_Mtr = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_SP_Mtr"]);
                    Quotations.Value = Convert.ToString(EstimateCostdt.Rows[i]["Value"]);

                    Quotations.Order_AltQuantity = Convert.ToString(EstimateCostdt.Rows[i]["Order_AltQuantity"]);
                    Quotations.Order_AltUOM = Convert.ToString(EstimateCostdt.Rows[i]["Order_AltUOM"]);
                  
                    Quotations.ProductName = Convert.ToString(EstimateCostdt.Rows[i]["ProductName"]);

                    if (EstimateCostdt.Columns.Contains("Remarks"))
                    {
                        Quotations.Remarks = Convert.ToString(EstimateCostdt.Rows[i]["Remarks"]);
                    }
                    else
                    {
                        Quotations.Remarks = "";
                    }                    
                   
                    QuotationList.Add(Quotations);
                }
            }

            return QuotationList;
        }

        public IEnumerable GetQuotationInfo(DataTable EstimateCostdt, string QuoteId)
        {
            List<Quotation> QuotationList = new List<Quotation>();

            if (EstimateCostdt != null && EstimateCostdt.Rows.Count > 0)
            {
                for (int i = 0; i < EstimateCostdt.Rows.Count; i++)
                {
                    Quotation Quotations = new Quotation();
                    if (!EstimateCostdt.Columns.Contains("SrlNo"))
                    {
                        Quotations.SrlNo = Convert.ToString(i + 1);
                    }
                    else
                    {
                        Quotations.SrlNo = Convert.ToString(EstimateCostdt.Rows[i]["SrlNo"]);
                    }
                    if (!EstimateCostdt.Columns.Contains("EstCostDetails_Id"))
                    {
                        Quotations.EstCostDetails_Id = "0";
                    }
                    else
                    {
                        Quotations.EstCostDetails_Id = Convert.ToString(EstimateCostdt.Rows[i]["EstCostDetails_Id"]);
                    }

                   
                    

                    string ProductDetails = Convert.ToString(EstimateCostdt.Rows[i]["ProductID"]);
                    string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string ProductID = ProductDetails; //ProductDetailsList[0];
                    Quotations.ProductID = Convert.ToString(ProductID);
                    Quotations.Description = Convert.ToString(EstimateCostdt.Rows[i]["Description"]);
                    Quotations.Quantity = Convert.ToString(EstimateCostdt.Rows[i]["Quantity"]);
                    Quotations.UOM = Convert.ToString(EstimateCostdt.Rows[i]["Stk_UOM_Name"]);
                    Quotations.EstCostDetails_NB = Convert.ToString(0.00);
                    Quotations.EstCostDetails_OD = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Thk = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Tol_thk = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Wt_Mtr = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Tol_wt = Convert.ToString(0.00);

                    Quotations.EstCostDetails_Qty_Mtr = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Sqmtr = Convert.ToString(0.00);
                    Quotations.EstCostDetails_T_WT = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Tol_t_wt = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Rate_kg = Convert.ToString(0.00);


                    Quotations.EstCostDetails_Frt = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Surface_prep = Convert.ToString(0.00);
                    //Quotations.EstCostDetails_Int_Coating = Convert.ToString(0.00);
                    //Quotations.EstCostDetails_Ext_Coating = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Rate_sqm = Convert.ToString(0.00);

                    Quotations.EstCostDetails_CP_MT = Convert.ToString(0.00);
                    Quotations.EstCostDetails_FOR_CP_MTR = Convert.ToString(0.00);
                    Quotations.EstCostDetails_FOR_CP_VAL = Convert.ToString(0.00);
                    Quotations.EstCostDetails_Gross_Margin = Convert.ToString(0.00);
//Quotations.EstCostDetails_Gross_Marg_Value_Mtr = Convert.ToString(0.00);


                    Quotations.EstCostDetails_SP_Mtr = Convert.ToString(0.00);
                    Quotations.Value = Convert.ToString(0.00);

                    Quotations.ProductName = Convert.ToString(EstimateCostdt.Rows[i]["Products_Name"]);
                    if (EstimateCostdt.Columns.Contains("Remarks"))
                    {
                        Quotations.Remarks = Convert.ToString(EstimateCostdt.Rows[i]["Remarks"]);
                    }
                    else
                    {
                        Quotations.Remarks = "";
                    }

                    Quotations.Order_AltQuantity = Convert.ToString(EstimateCostdt.Rows[i]["Order_AltQuantity"]);
                    Quotations.Order_AltUOM = Convert.ToString(EstimateCostdt.Rows[i]["Order_AltUOM"]);

                    Quotations.InquiryID = Convert.ToString(EstimateCostdt.Rows[i]["InquiryID"]);
                    Quotations.DetailsId = Convert.ToString(EstimateCostdt.Rows[i]["DetailsId"]);
                    QuotationList.Add(Quotations);
                }

                BindSessionByDatatable(EstimateCostdt);
            }

            return QuotationList;
        }


        public bool BindSessionByDatatable(DataTable dt)
        {
            bool IsSuccess = false;
            DataTable EstimateCostdt = new DataTable();

            EstimateCostdt.Columns.Add("SrlNo", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Id", typeof(string));
            EstimateCostdt.Columns.Add("ProductID", typeof(string));
            EstimateCostdt.Columns.Add("Description", typeof(string));
            EstimateCostdt.Columns.Add("Quantity", typeof(string));
            EstimateCostdt.Columns.Add("UOM", typeof(string));           

            EstimateCostdt.Columns.Add("ProductName", typeof(string));
            EstimateCostdt.Columns.Add("Remarks", typeof(string));          
            EstimateCostdt.Columns.Add("InquiryID", typeof(string));
            EstimateCostdt.Columns.Add("DetailsId", typeof(string));

            EstimateCostdt.Columns.Add("EstCostDetails_NB", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_OD", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Thk", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Tol_thk", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Wt_Mtr", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Tol_wt", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Qty_Mtr", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Sqmtr", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_T_WT", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Tol_t_wt", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Rate_kg", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Frt", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Surface_prep", typeof(string));
          //  EstimateCostdt.Columns.Add("EstCostDetails_Int_Coating", typeof(string));
           // EstimateCostdt.Columns.Add("EstCostDetails_Ext_Coating", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Rate_sqm", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Ratekg", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_CP_MT", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_FOR_CP_MTR", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_FOR_CP_VAL", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_Gross_Margin", typeof(string));
            //EstimateCostdt.Columns.Add("EstCostDetails_Gross_Marg_Value_Mtr", typeof(string));
            EstimateCostdt.Columns.Add("EstCostDetails_SP_Mtr", typeof(string));
            EstimateCostdt.Columns.Add("Value", typeof(string));

            EstimateCostdt.Columns.Add("Order_AltQuantity", typeof(string));
            EstimateCostdt.Columns.Add("Order_AltUOM", typeof(string));
            EstimateCostdt.Columns.Add("Status", typeof(string));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsSuccess = true;
                DataColumnCollection dtC = dt.Columns;
                string SalePrice, Discount, Amount, TaxAmount, TotalAmount, Order_Num1, ProductName, UOM, InquiryID, QuotationID, DetailsId, Remarks,
                EstCostDetails_NB = "0.00",
                EstCostDetails_OD = "0.00", EstCostDetails_Thk = "0.00", EstCostDetails_Tol_thk = "0.00", EstCostDetails_Wt_Mtr = "0.00", EstCostDetails_Tol_wt = "0.00"
                , EstCostDetails_Qty_Mtr = "0.00"
                , EstCostDetails_Sqmtr = "0.00", EstCostDetails_T_WT = "0.00", EstCostDetails_Tol_t_wt = "0.00", EstCostDetails_Rate_kg = "0.00", EstCostDetails_Frt = "0.00"
                , EstCostDetails_Surface_prep = "0.00"
                //EstCostDetails_Int_Coating = "0.00", EstCostDetails_Ext_Coating = "0.00"
                , EstCostDetails_Rate_sqm = "0.00", EstCostDetails_Ratekg = "0.00", EstCostDetails_CP_MT = "0.00",
                EstCostDetails_FOR_CP_MTR = "0.00", EstCostDetails_FOR_CP_VAL = "0.00", EstCostDetails_Gross_Margin = "0.00"
               // , EstCostDetails_Gross_Marg_Value_Mtr = "0.00"
                , EstCostDetails_SP_Mtr = "0.00", Value = "0.00";
               
                if (dtC.Contains("Quotation"))
                {
                    Order_Num1 = Convert.ToString(dt.Rows[i]["Quotation"]);
                }
                else
                {
                    Order_Num1 = "";
                }


                if (dtC.Contains("Remarks"))
                {
                    Remarks = Convert.ToString(dt.Rows[i]["Remarks"]);
                }
                else
                {
                    Remarks = "";
                }


                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Products_Name"])))
                {
                    ProductName = Convert.ToString(dt.Rows[i]["Products_Name"]);
                }
                else
                {
                    ProductName = "";
                }
                //if (!dt.Columns.Contains("QuotationID"))
                //{
                //    QuotationID = Convert.ToString(i + 1);
                //}
                //else
                //{
                //    QuotationID = Convert.ToString(dt.Rows[i]["QuotationID"]);
                //}
                DetailsId = Convert.ToString(dt.Rows[i]["DetailsId"]);
                InquiryID = Convert.ToString(dt.Rows[i]["InquiryID"]);
                string ProductDetails = Convert.ToString(dt.Rows[i]["ProductID"]);
                string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                string ProductID = ProductDetails;
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Stk_UOM_Name"])))
                {
                    UOM = Convert.ToString(dt.Rows[i]["Stk_UOM_Name"]);
                }
                else
                {
                    UOM = "";
                }
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["Stk_UOM_Name"])))
                {
                    UOM = Convert.ToString(dt.Rows[i]["Stk_UOM_Name"]);
                }
                else
                {
                    UOM = "";
                }
              
                string Order_AltQuantity = Convert.ToString(dt.Rows[i]["Order_AltQuantity"]);
                string  Order_AltUOM = Convert.ToString(dt.Rows[i]["Order_AltUOM"]);
                

                //EstimateCostdt.Rows.Add(Convert.ToString(i + 1), QuotationID, ProductID, Convert.ToString(dt.Rows[i]["Description"]),
                //    Convert.ToString(dt.Rows[i]["Quantity"]), UOM, "", Convert.ToString(dt.Rows[i]["QuoteDetails_StockQty"]), UOM,
                //               SalePrice, Discount, Amount, TaxAmount, TotalAmount, "I", ProductName, Remarks, Order_Num1, Convert.ToString(dt.Rows[i]["InquiryID"]), DetailsId, Order_AltQuantity, Order_AltUOM);


                EstimateCostdt.Rows.Add(Convert.ToString(i + 1), Convert.ToString(dt.Rows[i]["EstCostDetails_Id"]), ProductDetails, Convert.ToString(dt.Rows[i]["Description"]), Convert.ToString(dt.Rows[i]["Quantity"]), UOM, ProductName, Remarks, InquiryID, DetailsId
                           ,EstCostDetails_NB, EstCostDetails_OD, EstCostDetails_Thk, EstCostDetails_Tol_thk, EstCostDetails_Wt_Mtr, EstCostDetails_Tol_wt, EstCostDetails_Qty_Mtr
                           , EstCostDetails_Sqmtr, EstCostDetails_T_WT, EstCostDetails_Tol_t_wt, EstCostDetails_Rate_kg, EstCostDetails_Frt, EstCostDetails_Surface_prep
                           //EstCostDetails_Int_Coating, EstCostDetails_Ext_Coating
                           , EstCostDetails_Rate_sqm, EstCostDetails_Ratekg, EstCostDetails_CP_MT,
                           EstCostDetails_FOR_CP_MTR, EstCostDetails_FOR_CP_VAL, EstCostDetails_Gross_Margin, 
                           //EstCostDetails_Gross_Marg_Value_Mtr,
                           EstCostDetails_SP_Mtr, Value,
                           Order_AltQuantity, Order_AltUOM,"I");
            
            
            
            }

            Session["EstimateCostDetails"] = EstimateCostdt;

            return IsSuccess;
        }

        //public IEnumerable GetQuotationWaiting(DataTable EstimateCostdt)
        //{
        //    List<WaitingQuotation> QuotationList = new List<WaitingQuotation>();

        //    if (EstimateCostdt != null && EstimateCostdt.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < EstimateCostdt.Rows.Count; i++)
        //        {
        //            WaitingQuotation Quotations = new WaitingQuotation();

        //            Quotations.SrlNo = Convert.ToString(EstimateCostdt.Rows[i]["SrlNo"]);
        //            Quotations.QuotationID = Convert.ToString(EstimateCostdt.Rows[i]["QuotationID"]);
        //            Quotations.ProductID = Convert.ToString(EstimateCostdt.Rows[i]["ProductID"]);
        //            Quotations.Description = Convert.ToString(EstimateCostdt.Rows[i]["Description"]);
        //            Quotations.Quantity = Convert.ToString(EstimateCostdt.Rows[i]["Quantity"]);
        //            Quotations.UOM = Convert.ToString(EstimateCostdt.Rows[i]["UOM"]);
        //            Quotations.Warehouse = "";
        //            Quotations.StockQuantity = Convert.ToString(EstimateCostdt.Rows[i]["StockQuantity"]);
        //            Quotations.StockUOM = Convert.ToString(EstimateCostdt.Rows[i]["StockUOM"]);
        //            Quotations.SalePrice = Convert.ToString(EstimateCostdt.Rows[i]["SalePrice"]);
        //            Quotations.Discount = Convert.ToString(EstimateCostdt.Rows[i]["Discount"]);
        //            Quotations.Amount = Convert.ToString(EstimateCostdt.Rows[i]["Amount"]);
        //            Quotations.TaxAmount = Convert.ToString(EstimateCostdt.Rows[i]["TaxAmount"]);
        //            Quotations.TotalAmount = Convert.ToString(EstimateCostdt.Rows[i]["TotalAmount"]);
        //            Quotations.Status = Convert.ToString(EstimateCostdt.Rows[i]["Status"]);
        //            Quotations.ProductName = Convert.ToString(EstimateCostdt.Rows[i]["ProductName"]);
        //            QuotationList.Add(Quotations);
        //        }
        //    }

        //    return QuotationList;
        //}
        //public class WaitingProductQuantity
        //{
        //    public Int64 productid { get; set; }
        //    public Int32 pslno { get; set; }
        //    public Decimal pQuantity { get; set; }
        //    public Decimal packing { get; set; }

        //    public Int32 PackingUom { get; set; }
        //    public Int32 PackingSelectUom { get; set; }
        //}
        public IEnumerable GetWaitingProductDetails(DataTable EstimateCostdt)
        {
            List<WaitingProductQuantity> QuotationList = new List<WaitingProductQuantity>();

            if (EstimateCostdt != null && EstimateCostdt.Rows.Count > 0)
            {
                for (int i = 0; i < EstimateCostdt.Rows.Count; i++)
                {
                    WaitingProductQuantity Quotations = new WaitingProductQuantity();

                    Quotations.productid = Convert.ToInt64(EstimateCostdt.Rows[i]["productid"]);
                    Quotations.pslno = Convert.ToInt32(EstimateCostdt.Rows[i]["pslno"]);
                    Quotations.pQuantity = Convert.ToDecimal(EstimateCostdt.Rows[i]["pQuantity"]);
                    Quotations.packing = Convert.ToDecimal(EstimateCostdt.Rows[i]["packing"]);
                    Quotations.PackingUom = Convert.ToInt32(EstimateCostdt.Rows[i]["PackingUom"]);
                    Quotations.PackingSelectUom = Convert.ToInt32(EstimateCostdt.Rows[i]["PackingSelectUom"]);

                    QuotationList.Add(Quotations);
                }
            }

            return QuotationList;
        }
        public DataSet GetQuotationData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");          
            proc.AddVarcharPara("@Action", 500, "EstimateCostDetails_New");         
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataSet GetQuotationDataForCopy()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_SalesCRMCopy_Details");
            proc.AddVarcharPara("@Action", 500, "QuotationCopyDetails");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataTable GetMultiUOMData()
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            // Rev Sanchita
            //proc.AddVarcharPara("@Action", 500, "MultiUOMEstimateCostDetails");
            proc.AddVarcharPara("@Action", 500, "MultiUOMEstimateCostDetails_New");
            // End of Rev Sanchita
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            ds = proc.GetTable();
            return ds;
        }
        protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (e.Column.FieldName == "Description")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "UOM")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "EstCostDetails_Wt_Mtr")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;                
            }
            else if (e.Column.FieldName == "EstCostDetails_Tol_wt")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_T_WT")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_Tol_t_wt")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }

            else if (e.Column.FieldName == "EstCostDetails_Sqmtr")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_Rate_sqm")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_Ratekg")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "SrlNo")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "ProductName")
            {
                e.Editor.Enabled = true;
            }
            else if (e.Column.FieldName == "EstCostDetails_CP_MT")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "Order_AltQuantity")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "Order_AltUOM")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_FOR_CP_MTR")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_FOR_CP_VAL")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_Gross_Marg_Value_Mtr")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "EstCostDetails_SP_Mtr")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }
            else if (e.Column.FieldName == "Value")
            {
                e.Editor.Enabled = true;
                e.Editor.BackColor = System.Drawing.Color.Gray;
            }                
            else if (hddnMultiUOMSelection.Value == "1" && e.Column.FieldName == "Quantity")
            {
                e.Editor.Enabled = true;
            }

            else
            {
                e.Editor.ReadOnly = false;
            }
        }

        protected void Productgrid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Productgrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Productgrid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void aspxGridProduct_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {

        }
        protected void lookup_quotation_DataBinding(object sender, EventArgs e)
        {
            if (Session["IECSnquiryData"] != null)
            {
                lookup_quotation.DataSource = (DataTable)Session["IECSnquiryData"];
            }
        }

        protected void grid_Products_DataBinding(Object sender, EventArgs e)
        {

            if (Session["SO_ProductDetails"] != null)
            {
                DataTable EstimateCostdt = (DataTable)Session["SO_ProductDetails"];
                DataView dvData = new DataView(EstimateCostdt);
                //dvData.RowFilter = "Status <> 'D'";
                grid_Products.DataSource = GetProductsInfo(EstimateCostdt);
            }
        }
        protected void cgridProducts_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "BindProductsDetails")
            {

                string Quote_Nos = Convert.ToString(e.Parameters.Split('~')[1]);

                String QuoComponent = "";
                List<object> QuoList = lookup_quotation.GridView.GetSelectedFieldValues("Quote_Id");
                foreach (object Quo in QuoList)
                {
                    QuoComponent += "," + Quo;
                }
                QuoComponent = QuoComponent.TrimStart(',');

                if (Quote_Nos != "$")
                {
                    //grid.DataSource = null;
                    //grid.DataBind();
                    //string OrderId = Convert.ToString(e.Parameters.Split('~')[2]);

                    //Session["OrderID"] = OrderId;
                    DataTable dt_EstimateCostDetails = new DataTable();
                    string IdKey = Convert.ToString(Request.QueryString["key"]);
                    if (!string.IsNullOrEmpty(IdKey))
                    {
                        if (IdKey != "ADD")
                        {
                            dt_EstimateCostDetails = objSlaesActivitiesBL.GetInquiryDetailsFromSalesOrderonly(QuoComponent, IdKey, "");
                        }
                        else
                        {
                            dt_EstimateCostDetails = objSlaesActivitiesBL.GetInquiryDetailsFromSalesOrderonly(QuoComponent, "", "");
                        }

                    }
                    else
                    {
                        dt_EstimateCostDetails = objSlaesActivitiesBL.GetInquiryDetailsFromSalesQuotation(QuoComponent, "", "", "");
                    }
                    Session["EstimateCostDetails"] = null;
                    //grid.DataSource = GetSalesOrderInfo(dt_EstimateCostDetails, IdKey);
                    //grid.DataBind();
                    Session["SO_ProductDetails"] = dt_EstimateCostDetails;
                    grid_Products.DataSource = GetProductsInfo(dt_EstimateCostDetails);
                    grid_Products.DataBind();


                    if (grid_Products.VisibleRowCount > 0)
                    {
                        grid_Products.Enabled = false;
                        for (int i = 0; i < grid_Products.VisibleRowCount; i++)
                        {
                            grid_Products.Selection.SelectRow(i);
                        }
                    }


                }
                else
                {
                    grid_Products.DataSource = null;
                    grid_Products.DataBind();
                }

            }
            if (strSplitCommand == "SelectAndDeSelectProducts")
            {
                ASPxGridView gv = sender as ASPxGridView;
                string command = e.Parameters.ToString();
                string State = Convert.ToString(e.Parameters.Split('~')[1]);
                if (State == "SelectAll")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        gv.Selection.SelectRow(i);
                    }
                }
                if (State == "UnSelectAll")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        gv.Selection.UnselectRow(i);
                    }
                }
                if (State == "Revart")
                {
                    for (int i = 0; i < gv.VisibleRowCount; i++)
                    {
                        if (gv.Selection.IsRowSelected(i))
                            gv.Selection.UnselectRow(i);
                        else
                            gv.Selection.SelectRow(i);
                    }
                }
            }
        }

        protected void ComponentDatePanel_Callback(object sender, CallbackEventArgsBase e)
        {


            string strSplitCommand = e.Parameter.Split('~')[0];
            if (strSplitCommand == "BindQuotationDate")
            {
                string Quote_No = Convert.ToString(e.Parameter.Split('~')[1]);

                DataTable dt_EstimateCostDetails = objSlaesActivitiesBL.GetInquiryDate(Quote_No);
                if (dt_EstimateCostDetails != null && dt_EstimateCostDetails.Rows.Count > 0)
                {
                    string quotationdate = Convert.ToString(dt_EstimateCostDetails.Rows[0]["Quote_Date"]);
                    if (!string.IsNullOrEmpty(quotationdate))
                    {

                        dt_Quotation.Text = Convert.ToString(quotationdate);


                    }
                }
            }

        }


        protected void BindLookUp(String customer, string OrderDate)
        {

            string status = string.Empty;

            //Subhabrata
            if (Convert.ToString(Request.QueryString["key"]) != "ADD")
            {
                status = "DONE";
            }
            else
            {
                status = "NOT-DONE";
            }//End



            DataTable QuotationTable;


            QuotationTable = GetInquiryOnSalesQuotation(customer, OrderDate, status,
                Convert.ToInt32(ddl_Branch.SelectedValue), Convert.ToString(ddlInventory.SelectedValue));
            lookup_quotation.GridView.Selection.CancelSelection();
            lookup_quotation.DataSource = QuotationTable;
            lookup_quotation.DataBind();



            Session["IECSnquiryData"] = QuotationTable;
        }
        public DataTable GetInquiryOnSalesQuotation(string customer, string OrderDate, string Status, Int32 BranchId, string Inventory)
        {
            HttpContext context = HttpContext.Current;
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_GetInquiryOnESC");

            proc.AddVarcharPara("@Customer_Id", 20, customer);
            DateTime dtime = Convert.ToDateTime(OrderDate);
            proc.AddDateTimePara("@OrderDate", Convert.ToDateTime(OrderDate));
            //proc.AddDateTimePara("@OrderDate", Convert.ToDateTime(OrderDate));
            proc.AddVarcharPara("@Status", 50, Status);
            proc.AddVarcharPara("@userbranchlist", 1000, Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]));
            proc.AddVarcharPara("@IsInventory", 10, Inventory);
            //proc.AddIntegerPara("@Branch_Id", BranchId);
            dt = proc.GetTable();
            return dt;
        }
        protected void ComponentQuotation_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string status = string.Empty;
            string customer = string.Empty;
            string OrderDate = string.Empty;
            string BranchId = string.Empty;
            if (e.Parameter.Split('~')[0] == "BindQuotationGrid")
            {

                if (Convert.ToString(Request.QueryString["key"]) != "ADD")
                {
                    status = "DONE";
                }
                else
                {
                    status = "NOT-DONE";
                }

                if (e.Parameter.Split('~')[1] != null)
                { customer = e.Parameter.Split('~')[1]; }
                if (e.Parameter.Split('~')[2] != null)
                { OrderDate = e.Parameter.Split('~')[2]; }


                DataTable QuotationTable;

                if (e.Parameter.Split('~')[3] == "DateCheck")
                {
                    lookup_quotation.GridView.Selection.UnselectAll();
                }
                QuotationTable = GetInquiryOnSalesQuotation(customer, OrderDate, status, Convert.ToInt32(ddl_Branch.SelectedValue), Convert.ToString(ddlInventory.SelectedValue));
                lookup_quotation.GridView.Selection.CancelSelection();
                lookup_quotation.DataSource = QuotationTable;
                lookup_quotation.DataBind();



                Session["IECSnquiryData"] = QuotationTable;

            }
            else if (e.Parameter.Split('~')[0] == "BindQuotationGridOnSelection")//Subhabrata for binding quotation
            {
                int AmountsAre = 0;
                ComponentQuotationPanel.JSProperties["cpTaggedTaxAmountType"] = "";
                if (grid_Products.GetSelectedFieldValues("Quotation_No").Count != 0)
                {
                    for (int i = 0; i < grid_Products.GetSelectedFieldValues("Quotation_No").Count; i++)
                    {
                        QuotationIds += "," + grid_Products.GetSelectedFieldValues("Quotation_No")[i];
                        AmountsAre = Convert.ToInt32(grid_Products.GetSelectedFieldValues("TaxAmountType")[0]);
                    }
                    hdnAmountareTax.Value = Convert.ToString(AmountsAre);
                    ComponentQuotationPanel.JSProperties["cpTaggedTaxAmountType"] = Convert.ToString(AmountsAre);
                    QuotationIds = QuotationIds.TrimStart(',');
                    lookup_quotation.GridView.Selection.UnselectAll();
                    if (!String.IsNullOrEmpty(QuotationIds) && QuotationIds.Split(',')[0] !="0")
                    {
                        if (AmountsAre == 1)
                        {
                            //ddl_AmountAre.Value = "1";
                            //ddl_AmountAre.ClientEnabled = false;
                        }
                        else if (AmountsAre == 2)
                        {
                            //ddl_AmountAre.Value = "2";
                            //ddl_AmountAre.ClientEnabled = false;
                        }

                        string[] eachQuo = QuotationIds.Split(',');
                        if (eachQuo.Length > 1)//More tha one quotation
                        {
                            //dt_Quotation.Text = "Multiple Select Inquiry Dates";
                            //BindLookUp(Customer_Id, Order_Date);
                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                        }
                        else if (eachQuo.Length == 1)//Single Quotation
                        {
                            foreach (string val in eachQuo)
                            {
                                lookup_quotation.GridView.Selection.SelectRowByKey(Convert.ToInt32(val));
                            }
                        }
                        else//No Quotation selected
                        {
                            lookup_quotation.GridView.Selection.UnselectAll();
                        }
                    }
                }
                else if (grid_Products.GetSelectedFieldValues("Quotation_No").Count == 0)
                {
                    lookup_quotation.GridView.Selection.UnselectAll();
                    Session["EstimateCostDetails"] = null;
                    grid.DataSource = null;
                    grid.DataBind();
                }
            }
            else if (e.Parameter.Split('~')[0] == "DateCheckOnChanged")//Subhabrata for binding quotation
            {

                if (grid_Products.GetSelectedFieldValues("Quotation_No").Count != 0)
                {

                    DateTime SalesOrderDate = Convert.ToDateTime(e.Parameter.Split('~')[2]);
                    if (lookup_quotation.GridView.GetSelectedFieldValues("Quote_Date").Count() != 0)
                    {
                        //DateTime QuotationDate = DateTime.Parse(Convert.ToString(lookup_quotation.GridView.GetSelectedFieldValues("Quote_Date")[0]));
                        //if (SalesOrderDate < QuotationDate)
                        //{
                        lookup_quotation.GridView.Selection.UnselectAll();
                        //}
                    }


                    //Quote_Date

                }
            }

        }


        public IEnumerable GetProductsInfo(DataTable SalesOrderdt1)
        {
            List<SalesInquiry> OrderList = new List<SalesInquiry>();
            for (int i = 0; i < SalesOrderdt1.Rows.Count; i++)
            {
                SalesInquiry Orders = new SalesInquiry();
                Orders.SrlNo = Convert.ToString(i + 1);
                Orders.Key_UniqueId = Convert.ToString(i + 1);
                if (!string.IsNullOrEmpty(Convert.ToString(SalesOrderdt1.Rows[i]["Quotation_No"])))
                {
                    Orders.Quotation_No = Convert.ToInt64(SalesOrderdt1.Rows[i]["Quotation_No"]);
                }
                else
                { Orders.Quotation_No = 0; }
                Orders.ProductID = Convert.ToString(SalesOrderdt1.Rows[i]["QuoteDetails_ProductId"]);
                Orders.Description = Convert.ToString(SalesOrderdt1.Rows[i]["QuoteDetails_ProductDescription"]);
                Orders.Quantity = Convert.ToString(SalesOrderdt1.Rows[i]["QuoteDetails_Quantity"]);
                Orders.Quotation_Num = Convert.ToString(SalesOrderdt1.Rows[i]["Quotation"]);
                Orders.Product_Shortname = Convert.ToString(SalesOrderdt1.Rows[i]["Product_Name"]);
                Orders.QuoteDetails_Id = Convert.ToString(SalesOrderdt1.Rows[i]["QuoteDetails_Id"]);
                if (!string.IsNullOrEmpty(Convert.ToString(SalesOrderdt1.Rows[i]["TaxAmountType"])))
                { Orders.TaxAmountType = Convert.ToInt32(SalesOrderdt1.Rows[i]["TaxAmountType"]); }
                else
                {
                    Orders.TaxAmountType = 0;
                }
                OrderList.Add(Orders);
            }
            return OrderList;
        }
        protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable EstimateCostdt = new DataTable();
            string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);

            string validate = "";           
            grid.JSProperties["cpSaveSuccessOrFail"] = "";

            DataTable AdditionalDetails = new DataTable();
            AdditionalDetails = (DataTable)Session["InlineRemarks"];

            if (Session["EstimateCostDetails"] != null)
            {
                EstimateCostdt = (DataTable)Session["EstimateCostDetails"];
            }
            else
            {
                EstimateCostdt.Columns.Add("SrlNo", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Id", typeof(string));
                EstimateCostdt.Columns.Add("ProductID", typeof(string));
                EstimateCostdt.Columns.Add("Description", typeof(string));
                EstimateCostdt.Columns.Add("Quantity", typeof(string));
                EstimateCostdt.Columns.Add("UOM", typeof(string));
                //EstimateCostdt.Columns.Add("Warehouse", typeof(string));
                //EstimateCostdt.Columns.Add("StockQuantity", typeof(string));
                //EstimateCostdt.Columns.Add("StockUOM", typeof(string));
                //EstimateCostdt.Columns.Add("SalePrice", typeof(string));
                //EstimateCostdt.Columns.Add("Discount", typeof(string));
                //EstimateCostdt.Columns.Add("Amount", typeof(string));
                //EstimateCostdt.Columns.Add("TaxAmount", typeof(string));
                //EstimateCostdt.Columns.Add("TotalAmount", typeof(string));
                
                EstimateCostdt.Columns.Add("ProductName", typeof(string));
                EstimateCostdt.Columns.Add("Remarks", typeof(string));
                //EstimateCostdt.Columns.Add("Quotation_Num", typeof(string));
                EstimateCostdt.Columns.Add("InquiryID", typeof(string));
                EstimateCostdt.Columns.Add("DetailsId", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_NB", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_OD", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Thk", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Tol_thk", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Wt_Mtr", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Tol_wt", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Qty_Mtr", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Sqmtr", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_T_WT", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Tol_t_wt", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Rate_kg", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Frt", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Surface_prep", typeof(string));
                //EstimateCostdt.Columns.Add("EstCostDetails_Int_Coating", typeof(string));
               // EstimateCostdt.Columns.Add("EstCostDetails_Ext_Coating", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Rate_sqm", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Ratekg", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_CP_MT", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_FOR_CP_MTR", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_FOR_CP_VAL", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_Gross_Margin", typeof(string));
               // EstimateCostdt.Columns.Add("EstCostDetails_Gross_Marg_Value_Mtr", typeof(string));
                EstimateCostdt.Columns.Add("EstCostDetails_SP_Mtr", typeof(string));
                EstimateCostdt.Columns.Add("Value", typeof(string));            

                EstimateCostdt.Columns.Add("Order_AltQuantity", typeof(string));
                EstimateCostdt.Columns.Add("Order_AltUOM", typeof(string));
                EstimateCostdt.Columns.Add("Status", typeof(string));
            }
            // Mantis # 0024609
            //if (EstimateCostdt.Columns.Contains("QuotationType"))
            //{
            //    EstimateCostdt.Columns.Remove("QuotationType");
            //}
            // End of Mantis # 0024609

            foreach (var args in e.InsertValues)
            {
                string ProductDetails = Convert.ToString(args.NewValues["ProductID"]);

                if (ProductDetails != "" && ProductDetails != "0")
                {
                    string SrlNo = Convert.ToString(args.NewValues["SrlNo"]);
                    string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string ProductID = ProductDetailsList[0];

                    string ProductName = Convert.ToString(args.NewValues["ProductName"]);
                    //string Quotation_Num = Convert.ToString(args.NewValues["Quotation_Num"]);
                    string Description = Convert.ToString(args.NewValues["Description"]);
                    string Quantity = Convert.ToString(args.NewValues["Quantity"]);
                    string UOM = Convert.ToString(args.NewValues["UOM"]);
                    //string Warehouse = Convert.ToString(args.NewValues["Warehouse"]);

                    decimal strMultiplier = Convert.ToDecimal(ProductDetailsList[7]);
                    //string StockQuantity = Convert.ToString(Convert.ToDecimal(Quantity) * strMultiplier);
                    //string StockUOM = Convert.ToString(ProductDetailsList[4]);
                    string Remarks = Convert.ToString(args.NewValues["Remarks"]);
                    //string StockQuantity = Convert.ToString(args.NewValues["StockQuantity"]);
                    //string StockUOM = Convert.ToString(args.NewValues["StockUOM"]);

                    // Rev  Manis 24428
                    string Order_AltQuantity = Convert.ToString(args.NewValues["Order_AltQuantity"]);
                    string Order_AltUOM = Convert.ToString(args.NewValues["Order_AltUOM"]);
                    // End Manis 24428
                    string EstCostDetails_NB = (Convert.ToString(args.NewValues["EstCostDetails_NB"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_NB"]) : "0";
                    string EstCostDetails_OD = (Convert.ToString(args.NewValues["EstCostDetails_OD"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_OD"]) : "0";
                    string EstCostDetails_Thk = (Convert.ToString(args.NewValues["EstCostDetails_Thk"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Thk"]) : "0";
                    string EstCostDetails_Tol_thk = (Convert.ToString(args.NewValues["EstCostDetails_Tol_thk"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_thk"]) : "0";
                    string EstCostDetails_Wt_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Wt_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Wt_Mtr"]) : "0";
                    string EstCostDetails_Tol_wt = (Convert.ToString(args.NewValues["EstCostDetails_Tol_wt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_wt"]) : "0";
                    string EstCostDetails_Qty_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Qty_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Qty_Mtr"]) : "0";

                    string EstCostDetails_Sqmtr = (Convert.ToString(args.NewValues["EstCostDetails_Sqmtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Sqmtr"]) : "0";
                    string EstCostDetails_T_WT = (Convert.ToString(args.NewValues["EstCostDetails_T_WT"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_T_WT"]) : "0";
                    string EstCostDetails_Tol_t_wt = (Convert.ToString(args.NewValues["EstCostDetails_Tol_t_wt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_t_wt"]) : "0";
                    string EstCostDetails_Rate_kg = (Convert.ToString(args.NewValues["EstCostDetails_Rate_kg"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Rate_kg"]) : "0";
                    string EstCostDetails_Frt = (Convert.ToString(args.NewValues["EstCostDetails_Frt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Frt"]) : "0";
                    string EstCostDetails_Surface_prep = (Convert.ToString(args.NewValues["EstCostDetails_Surface_prep"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Surface_prep"]) : "0";

                   // string EstCostDetails_Int_Coating = (Convert.ToString(args.NewValues["EstCostDetails_Int_Coating"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Int_Coating"]) : "0";
                    //string EstCostDetails_Ext_Coating = (Convert.ToString(args.NewValues["EstCostDetails_Ext_Coating"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Ext_Coating"]) : "0";
                    string EstCostDetails_Rate_sqm = (Convert.ToString(args.NewValues["EstCostDetails_Rate_sqm"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Rate_sqm"]) : "0";
                    string EstCostDetails_Ratekg = (Convert.ToString(args.NewValues["EstCostDetails_Ratekg"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Ratekg"]) : "0";
                    string EstCostDetails_CP_MT = (Convert.ToString(args.NewValues["EstCostDetails_CP_MT"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_CP_MT"]) : "0";
                    string EstCostDetails_FOR_CP_MTR = (Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_MTR"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_MTR"]) : "0";

                    string EstCostDetails_FOR_CP_VAL = (Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_VAL"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_VAL"]) : "0";
                    string EstCostDetails_Gross_Margin = (Convert.ToString(args.NewValues["EstCostDetails_Gross_Margin"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Gross_Margin"]) : "0";
                    //string EstCostDetails_Gross_Marg_Value_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Gross_Marg_Value_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Gross_Marg_Value_Mtr"]) : "0";
                    string EstCostDetails_SP_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_SP_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_SP_Mtr"]) : "0";
                    string Value = (Convert.ToString(args.NewValues["Value"]) != "") ? Convert.ToString(args.NewValues["Value"]) : "0";                    
                    string DetailsId = (Convert.ToString(args.NewValues["DetailsId"]) != "") ? Convert.ToString(args.NewValues["DetailsId"]) : "0";
                    string InquiryID = (Convert.ToString(args.NewValues["InquiryID"]) != "") ? Convert.ToString(args.NewValues["InquiryID"]) : "0";
                     
                    EstimateCostdt.Rows.Add(SrlNo,"0" ,ProductDetails, Description, Quantity, UOM, ProductName, Remarks, InquiryID, DetailsId
                        ,EstCostDetails_NB,EstCostDetails_OD,EstCostDetails_Thk,EstCostDetails_Tol_thk,EstCostDetails_Wt_Mtr,EstCostDetails_Tol_wt,EstCostDetails_Qty_Mtr
                        ,EstCostDetails_Sqmtr,EstCostDetails_T_WT,EstCostDetails_Tol_t_wt,EstCostDetails_Rate_kg,EstCostDetails_Frt,EstCostDetails_Surface_prep,
                         EstCostDetails_Rate_sqm, EstCostDetails_Ratekg, EstCostDetails_CP_MT,
                        EstCostDetails_FOR_CP_MTR, EstCostDetails_FOR_CP_VAL, EstCostDetails_Gross_Margin, EstCostDetails_SP_Mtr, Value,
                        Order_AltQuantity, Order_AltUOM, "I");
                    
                }
            }

            foreach (var args in e.UpdateValues)
            {
                string SrlNo = Convert.ToString(args.NewValues["SrlNo"]);
                string DetailsId = (Convert.ToString(args.NewValues["DetailsId"]) != "") ? Convert.ToString(args.NewValues["DetailsId"]) : "0";
                string InquiryID = (Convert.ToString(args.NewValues["InquiryID"]) != "") ? Convert.ToString(args.NewValues["InquiryID"]) : "0";
                string ProductDetails = Convert.ToString(args.NewValues["ProductID"]);
                string EstCostDetails_Id = Convert.ToString(args.Keys["EstCostDetails_Id"]);
                bool isDeleted = false;
                foreach (var arg in e.DeleteValues)
                {
                    string DeleteID = Convert.ToString(arg.Keys["EstCostDetails_Id"]);
                    if (DeleteID == EstCostDetails_Id)
                    {
                        isDeleted = true;
                        break;
                    }
                }

                if (isDeleted == false)
                {
                    if (ProductDetails != "" && ProductDetails != "0")
                    {
                       
                        string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                        string ProductID = ProductDetailsList[0];

                        string ProductName = Convert.ToString(args.NewValues["ProductName"]);
                        //string Quotation_Num = Convert.ToString(args.NewValues["Quotation_Num"]);
                        string Description = Convert.ToString(args.NewValues["Description"]);
                        string Quantity = Convert.ToString(args.NewValues["Quantity"]);
                        string UOM = Convert.ToString(args.NewValues["UOM"]);
                        //string Warehouse = Convert.ToString(args.NewValues["Warehouse"]);

                        decimal strMultiplier = Convert.ToDecimal(ProductDetailsList[7]);
                        //string StockQuantity = Convert.ToString(Convert.ToDecimal(Quantity) * strMultiplier);
                        //string StockUOM = Convert.ToString(ProductDetailsList[4]);
                        string Remarks = Convert.ToString(args.NewValues["Remarks"]);
                        //string StockQuantity = Convert.ToString(args.NewValues["StockQuantity"]);
                        //string StockUOM = Convert.ToString(args.NewValues["StockUOM"]);

                        // Rev  Manis 24428
                        string Order_AltQuantity = Convert.ToString(args.NewValues["Order_AltQuantity"]);
                        string Order_AltUOM = Convert.ToString(args.NewValues["Order_AltUOM"]);
                        // End Manis 24428
                        string EstCostDetails_NB = (Convert.ToString(args.NewValues["EstCostDetails_NB"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_NB"]) : "0";
                        string EstCostDetails_OD = (Convert.ToString(args.NewValues["EstCostDetails_OD"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_OD"]) : "0";
                        string EstCostDetails_Thk = (Convert.ToString(args.NewValues["EstCostDetails_Thk"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Thk"]) : "0";
                        string EstCostDetails_Tol_thk = (Convert.ToString(args.NewValues["EstCostDetails_Tol_thk"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_thk"]) : "0";
                        string EstCostDetails_Wt_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Wt_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Wt_Mtr"]) : "0";
                        string EstCostDetails_Tol_wt = (Convert.ToString(args.NewValues["EstCostDetails_Tol_wt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_wt"]) : "0";
                        string EstCostDetails_Qty_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Qty_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Qty_Mtr"]) : "0";

                        string EstCostDetails_Sqmtr = (Convert.ToString(args.NewValues["EstCostDetails_Sqmtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Sqmtr"]) : "0";
                        string EstCostDetails_T_WT = (Convert.ToString(args.NewValues["EstCostDetails_T_WT"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_T_WT"]) : "0";
                        string EstCostDetails_Tol_t_wt = (Convert.ToString(args.NewValues["EstCostDetails_Tol_t_wt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Tol_t_wt"]) : "0";
                        string EstCostDetails_Rate_kg = (Convert.ToString(args.NewValues["EstCostDetails_Rate_kg"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Rate_kg"]) : "0";
                        string EstCostDetails_Frt = (Convert.ToString(args.NewValues["EstCostDetails_Frt"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Frt"]) : "0";
                        string EstCostDetails_Surface_prep = (Convert.ToString(args.NewValues["EstCostDetails_Surface_prep"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Surface_prep"]) : "0";

                       // string EstCostDetails_Int_Coating = (Convert.ToString(args.NewValues["EstCostDetails_Int_Coating"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Int_Coating"]) : "0";
                       // string EstCostDetails_Ext_Coating = (Convert.ToString(args.NewValues["EstCostDetails_Ext_Coating"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Ext_Coating"]) : "0";
                        string EstCostDetails_Rate_sqm = (Convert.ToString(args.NewValues["EstCostDetails_Rate_sqm"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Rate_sqm"]) : "0";
                        string EstCostDetails_Ratekg = (Convert.ToString(args.NewValues["EstCostDetails_Ratekg"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Ratekg"]) : "0";
                        string EstCostDetails_CP_MT = (Convert.ToString(args.NewValues["EstCostDetails_CP_MT"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_CP_MT"]) : "0";
                        string EstCostDetails_FOR_CP_MTR = (Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_MTR"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_MTR"]) : "0";

                        string EstCostDetails_FOR_CP_VAL = (Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_VAL"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_FOR_CP_VAL"]) : "0";
                        string EstCostDetails_Gross_Margin = (Convert.ToString(args.NewValues["EstCostDetails_Gross_Margin"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Gross_Margin"]) : "0";
                       // string EstCostDetails_Gross_Marg_Value_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_Gross_Marg_Value_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_Gross_Marg_Value_Mtr"]) : "0";
                        string EstCostDetails_SP_Mtr = (Convert.ToString(args.NewValues["EstCostDetails_SP_Mtr"]) != "") ? Convert.ToString(args.NewValues["EstCostDetails_SP_Mtr"]) : "0";
                        string Value = (Convert.ToString(args.NewValues["Value"]) != "") ? Convert.ToString(args.NewValues["Value"]) : "0";
                       
                        

                        bool Isexists = false;
                        foreach (DataRow drr in EstimateCostdt.Rows)
                        {
                            string OldQuotationID = Convert.ToString(drr["EstCostDetails_Id"]);

                            if (OldQuotationID == EstCostDetails_Id)
                            {
                                Isexists = true;

                                drr["ProductID"] = ProductDetails;
                                drr["Description"] = Description;
                                drr["Quantity"] = Quantity;
                                drr["UOM"] = UOM;
                                drr["ProductName"] = ProductName;
                                drr["Remarks"] = Remarks;
                                drr["InquiryID"] = InquiryID;
                                drr["DetailsId"] = DetailsId;

                                drr["EstCostDetails_NB"] = EstCostDetails_NB;
                                drr["EstCostDetails_OD"] = EstCostDetails_OD;
                                drr["EstCostDetails_Thk"] = EstCostDetails_Thk;
                                drr["EstCostDetails_Tol_thk"] = EstCostDetails_Tol_thk;
                                drr["EstCostDetails_Wt_Mtr"] = EstCostDetails_Wt_Mtr;

                                drr["EstCostDetails_Tol_wt"] = EstCostDetails_Tol_wt;
                                drr["EstCostDetails_Qty_Mtr"] = EstCostDetails_Qty_Mtr;
                                drr["EstCostDetails_Sqmtr"] = EstCostDetails_Sqmtr;
                                drr["EstCostDetails_T_WT"] = EstCostDetails_T_WT;


                                drr["EstCostDetails_Tol_t_wt"] = EstCostDetails_Tol_t_wt;
                                drr["EstCostDetails_Rate_kg"] = EstCostDetails_Rate_kg;
                                drr["EstCostDetails_Frt"] = EstCostDetails_Frt;
                                drr["EstCostDetails_Surface_prep"] = EstCostDetails_Surface_prep;

                            //    drr["EstCostDetails_Int_Coating"] = EstCostDetails_Int_Coating;
                               // drr["EstCostDetails_Ext_Coating"] = EstCostDetails_Ext_Coating;
                                drr["EstCostDetails_Rate_sqm"] = EstCostDetails_Rate_sqm;
                                drr["EstCostDetails_Ratekg"] = EstCostDetails_Ratekg;

                                drr["EstCostDetails_CP_MT"] = EstCostDetails_CP_MT;
                                drr["EstCostDetails_FOR_CP_MTR"] = EstCostDetails_FOR_CP_MTR;
                                drr["EstCostDetails_FOR_CP_VAL"] = EstCostDetails_FOR_CP_VAL;
                                drr["EstCostDetails_Gross_Margin"] = EstCostDetails_Gross_Margin;

//drr["EstCostDetails_Gross_Marg_Value_Mtr"] = EstCostDetails_Gross_Marg_Value_Mtr;
                                drr["EstCostDetails_SP_Mtr"] = EstCostDetails_SP_Mtr;
                                drr["Value"] = Value;                             
                               
                                drr["Order_AltQuantity"] = Order_AltQuantity;
                                drr["Order_AltUOM"] = Order_AltUOM;
                                drr["Status"] = "U";

                                break;
                            }
                        }

                        if (Isexists == false)
                        {
                            EstimateCostdt.Rows.Add(SrlNo, ProductDetails, Description, Quantity, UOM, ProductName, Remarks, InquiryID, DetailsId, EstCostDetails_NB,EstCostDetails_OD
                            , EstCostDetails_Thk,EstCostDetails_Tol_thk, EstCostDetails_Wt_Mtr, EstCostDetails_Tol_wt, EstCostDetails_Qty_Mtr
                            , EstCostDetails_Sqmtr, EstCostDetails_T_WT, EstCostDetails_Tol_t_wt, EstCostDetails_Rate_kg, EstCostDetails_Frt, EstCostDetails_Surface_prep,
                             EstCostDetails_Rate_sqm, EstCostDetails_Ratekg, EstCostDetails_CP_MT,
                            EstCostDetails_FOR_CP_MTR, EstCostDetails_FOR_CP_VAL, EstCostDetails_Gross_Margin,  EstCostDetails_SP_Mtr, Value,
                            Order_AltQuantity, Order_AltUOM, "U");
                        }
                    }
                }
            }

            foreach (var args in e.DeleteValues)
            {
                string EstCostDetails_Id = Convert.ToString(args.Keys["EstCostDetails_Id"]);
                string InquiryID = Convert.ToString(args.Keys["InquiryID"]);
                string SrlNo = "";


                for (int i = AdditionalDetails.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = EstimateCostdt.Rows[i];
                    string delQuotationID = Convert.ToString(dr["EstCostDetails_Id"]);
                    DataRow daddr = AdditionalDetails.Rows[i];

                    if (delQuotationID == EstCostDetails_Id)
                    {
                        SrlNo = Convert.ToString(dr["SrlNo"]);
                        daddr.Delete();
                    }
                }

                AdditionalDetails.AcceptChanges();



                for (int i = EstimateCostdt.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = EstimateCostdt.Rows[i];
                    string delQuotationID = Convert.ToString(dr["EstCostDetails_Id"]);

                    if (delQuotationID == EstCostDetails_Id)
                    {
                        SrlNo = Convert.ToString(dr["SrlNo"]);
                        dr.Delete();
                    }
                }
                EstimateCostdt.AcceptChanges();

               // DeleteWarehouse(SrlNo);
               // DeleteTaxDetails(SrlNo);

                if (EstCostDetails_Id.Contains("~") != true)
                {

                    EstimateCostdt.Rows.Add("0", "0", "0", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0","D");//34
                }
            }

            ///////////////////////

            string strDeleteSrlNo = Convert.ToString(hdnDeleteSrlNo.Value);
            if (strDeleteSrlNo != "" && strDeleteSrlNo != "0")
            {
               // DeleteWarehouse(strDeleteSrlNo);
                //DeleteTaxDetails(strDeleteSrlNo);
                if (hddnMultiUOMSelection.Value == "1")
                {
                    DeleteMultiUOMDetails(strDeleteSrlNo);
                }
                hdnDeleteSrlNo.Value = "";
            }

            int j = 1;
            foreach (DataRow dr in EstimateCostdt.Rows)
            {
                string Status = Convert.ToString(dr["Status"]);
                string oldSrlNo = Convert.ToString(dr["SrlNo"]);
                string newSrlNo = j.ToString();

                dr["SrlNo"] = j.ToString();

                //UpdateWarehouse(oldSrlNo, newSrlNo);
                //UpdateTaxDetails(oldSrlNo, newSrlNo);
                if (hddnMultiUOMSelection.Value == "1")
                {
                    UpdateMultiUOMDetails(oldSrlNo, newSrlNo);
                }
                if (Status != "D")
                {
                    if (Status == "I")
                    {
                        string strID = Convert.ToString("Q~" + j);
                        dr["EstCostDetails_Id"] = strID;
                    }
                    j++;
                }
            }
            EstimateCostdt.AcceptChanges();

            if (EstimateCostdt.Rows.Count == 0)
            {
                EstimateCostdt.Rows.Add("1", "0", "0", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "", "", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "D");//34

            }
            EstimateCostdt.AcceptChanges();
            Session["EstimateCostDetails"] = EstimateCostdt;
            Session["InlineRemarks"] = AdditionalDetails;
            //////////////////////


            if (IsDeleteFrom != "D")
            {
                string ActionType = Convert.ToString(Session["ActionType"]);
                string MainQuotationID = Convert.ToString(Session["EstimateCost_Id"]);

                string strIsInventory = Convert.ToString(ddlInventory.SelectedValue);
                string strSchemeType = Convert.ToString(ddl_numberingScheme.SelectedValue);
                string strQuoteNo = Convert.ToString(txt_PLQuoteNo.Text);
                string strQuoteDate = Convert.ToString(dt_PLQuote.Date);
                string strDocidsDate = Convert.ToString(dt_Quotation.Text);
                //string strQuoteExpiry = Convert.ToString(dt_PlQuoteExpiry.Date);
                string strCustomer = Convert.ToString(hdfLookupCustomer.Value);
                string strContactName = Convert.ToString(cmbContactPerson.Value);
                //string Reference = Convert.ToString(txt_Refference.Text);
                string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
                //string strAgents = Convert.ToString(ddl_SalesAgent.SelectedValue);
                string strAgents = hdnSalesManAgentId.Value;
                //string strCurrency = Convert.ToString(ddl_Currency.SelectedValue);
                //string strRate = Convert.ToString(txt_Rate.Value);
                //string strTaxType = Convert.ToString(ddl_AmountAre.Value);
                string strTaxCode = Convert.ToString(ddl_VatGstCst.Value).Split('~')[0];

                string CompID = Convert.ToString(HttpContext.Current.Session["LastCompany"]);
                string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                string[] ActCurrency = currency.Split('~');
                int BaseCurrencyId = Convert.ToInt32(ActCurrency[0]);
                //int ConvertedCurrencyId = Convert.ToInt32(strCurrency);

                //SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
                //DataTable dt = objSlaesActivitiesBL.GetCurrentConvertedRate(BaseCurrencyId, ConvertedCurrencyId, CompID);
                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    strRate = Convert.ToString(dt.Rows[0]["SalesRate"]);
                //}



                string RevisionNo = Convert.ToString(txtRevisionNo.Text);
                string RevisionDate = "";

                if (hdnApprovalsettings.Value == "1")
                {
                    RevisionDate = Convert.ToString(txtRevisionDate.Text);
                }
                string ProjRemarks = Convert.ToString(txtProjRemarks.Text);
                string AppRejRemarks = Convert.ToString(txtAppRejRemarks.Text);

                DataTable tempQuotation = EstimateCostdt.Copy();

                foreach (DataRow dr in tempQuotation.Rows)
                {
                    string Status = Convert.ToString(dr["Status"]);

                    if (Status == "I")
                    {
                        dr["EstCostDetails_Id"] = "0";

                        string[] list = Convert.ToString(dr["ProductID"]).Split(new string[] { "||@||" }, StringSplitOptions.None);
                        if (Convert.ToString(dr["ProductID"]) != "0")
                        {
                            dr["ProductID"] = Convert.ToString(list[0]);
                            dr["UOM"] = Convert.ToString(list[3]);
                           // dr["StockUOM"] = Convert.ToString(list[5]);
                        }
                    }
                    else if (Status == "U" || Status == "")
                    {
                        if (Convert.ToString(dr["EstCostDetails_Id"]).Contains("~") == true)
                        {
                            dr["EstCostDetails_Id"] = 0;
                        }

                        string[] list = Convert.ToString(dr["ProductID"]).Split(new string[] { "||@||" }, StringSplitOptions.None);
                        dr["ProductID"] = Convert.ToString(list[0]);
                        dr["UOM"] = Convert.ToString(list[3]);
                        //dr["StockUOM"] = Convert.ToString(list[5]);
                    }
                }
                tempQuotation.AcceptChanges();

                //DataTable TaxDetailTable = getAllTaxDetails(e);

                //DataTable TaxDetailTable = new DataTable();
                //if (Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] != null)
                //{
                //    TaxDetailTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
                //}

                // DataTable of Warehouse

                //DataTable tempWarehousedt = new DataTable();
                //if (Session["WarehouseData"] != null)
                //{
                //    DataTable Warehousedt = (DataTable)Session["WarehouseData"];
                //    tempWarehousedt = Warehousedt.DefaultView.ToTable(false, "Product_SrlNo", "LoopID", "WarehouseID", "TotalQuantity", "BatchID", "SerialID");
                //}
                //else
                //{
                //    tempWarehousedt.Columns.Add("Product_SrlNo", typeof(string));
                //    tempWarehousedt.Columns.Add("SrlNo", typeof(string));
                //    tempWarehousedt.Columns.Add("WarehouseID", typeof(string));
                //    tempWarehousedt.Columns.Add("TotalQuantity", typeof(string));
                //    tempWarehousedt.Columns.Add("BatchID", typeof(string));
                //    tempWarehousedt.Columns.Add("SerialID", typeof(string));
                //}

                // End

                //datatable for MultiUOm start chinmoy 14-01-2020
                DataTable MultiUOMDetails = new DataTable();
                DataTable Multidata = (DataTable)Session["MultiUOMData"];
                if (Session["MultiUOMData"] != null)
                {
                    DataTable MultiUOM = (DataTable)Session["MultiUOMData"];
                    // Mantis Issue 24428
                    //MultiUOMDetails = MultiUOM.DefaultView.ToTable(false, "SrlNo", "Quantity", "UOM", "AltUOM", "AltQuantity", "UomId", "AltUomId", "ProductId", "DetailsId");
                    MultiUOMDetails = MultiUOM.DefaultView.ToTable(false, "SrlNo", "Quantity", "UOM", "AltUOM", "AltQuantity", "UomId", "AltUomId", "ProductId","DetailsId","BaseRate", "AltRate", "UpdateRow");
                    // End of Mantis Issue 24428
                }
                else
                {
                    MultiUOMDetails.Columns.Add("SrlNo", typeof(string));
                    MultiUOMDetails.Columns.Add("Quantity", typeof(Decimal));
                    MultiUOMDetails.Columns.Add("UOM", typeof(string));
                    MultiUOMDetails.Columns.Add("AltUOM", typeof(string));
                    MultiUOMDetails.Columns.Add("AltQuantity", typeof(Decimal));
                    MultiUOMDetails.Columns.Add("UomId", typeof(Int64));
                    MultiUOMDetails.Columns.Add("AltUomId", typeof(Int64));
                    MultiUOMDetails.Columns.Add("ProductId", typeof(Int64));
                    MultiUOMDetails.Columns.Add("DetailsId", typeof(string));


                    // Mantis Issue 24428
                    MultiUOMDetails.Columns.Add("BaseRate", typeof(Decimal));
                    MultiUOMDetails.Columns.Add("AltRate", typeof(Decimal));
                    MultiUOMDetails.Columns.Add("UpdateRow", typeof(string));
                    // End of Mantis Issue 24428
                }


                //End
                // DataTable Of Quotation Tax Details 

                //DataTable TaxDetailsdt = new DataTable();
                //if (Session["TaxDetails"] != null)
                //{
                //    TaxDetailsdt = (DataTable)Session["TaxDetails"];
                //}
                //else
                //{
                //    TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                //    TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                //    TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                //    TaxDetailsdt.Columns.Add("Amount", typeof(string));
                //    TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
                //}

                //DataTable tempTaxDetailsdt = new DataTable();
                //tempTaxDetailsdt = TaxDetailsdt.DefaultView.ToTable(false, "Taxes_ID", "Percentage", "Amount", "AltTax_Code");

                //tempTaxDetailsdt.Columns.Add("SlNo", typeof(string));
                ////    tempTaxDetailsdt.Columns.Add("AltTaxCode", typeof(string));

                //tempTaxDetailsdt.Columns["SlNo"].SetOrdinal(0);
                //tempTaxDetailsdt.Columns["Taxes_ID"].SetOrdinal(1);
                //tempTaxDetailsdt.Columns["AltTax_Code"].SetOrdinal(2);
                //tempTaxDetailsdt.Columns["Percentage"].SetOrdinal(3);
                //tempTaxDetailsdt.Columns["Amount"].SetOrdinal(4);

                //foreach (DataRow d in tempTaxDetailsdt.Rows)
                //{
                //    d["SlNo"] = "0";
                //    //d["AltTaxCode"] = "0";
                //}

                // End

                // DataTable Of Billing Address
                //#region ##### Added By : Samrat Roy -- to get BillingShipping user control data
                DataTable tempBillAddress = new DataTable();
                tempBillAddress = BillingShippingControl.GetBillingShippingTable();

                //#region #### Old_Process ####
                ////DataTable tempBillAddress = new DataTable();
                ////if (Session["QuotationAddressDtl"] != null)
                ////{
                ////    tempBillAddress = (DataTable)Session["QuotationAddressDtl"];
                ////}
                ////else
                ////{
                ////    tempBillAddress = StoreQuotationAddressDetail();
                ////}

                //#endregion

                //#endregion

                // End

                string approveStatus = "";
                if (Request.QueryString["status"] != null)
                {
                    approveStatus = Convert.ToString(Request.QueryString["status"]);
                }

                int Schema_ID=0;

                //if (ActionType == "Add")
                //{
                    UniqueQuotation = txt_PLQuoteNo.Text;
                    string[] SchemeList = strSchemeType.Split(new string[] { "~" }, StringSplitOptions.None);
                    //validate = checkNMakeJVCode(strQuoteNo, Convert.ToInt32(SchemeList[0]));
                    Schema_ID = Convert.ToInt32(SchemeList[0]);
              //  }

                //foreach (DataRow dr in tempQuotation.Rows)
                //{
                //    string strSrlNo = Convert.ToString(dr["SrlNo"]);
                //    decimal strProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                //    string Status = Convert.ToString(dr["Status"]);

                //    decimal strWarehouseQuantity = 0;
                //    GetQuantityBaseOnProduct(strSrlNo, ref strWarehouseQuantity);

                //    if (Status != "D")
                //    {
                //        if (strWarehouseQuantity != 0)
                //        {
                //            if (strProductQuantity != strWarehouseQuantity)
                //            {
                //                validate = "checkWarehouse";
                //                grid.JSProperties["cpProductSrlIDCheck"] = strSrlNo;
                //                break;
                //            }
                //        }
                //    }
                //}

                if (hddnMultiUOMSelection.Value == "1")
                {
                    foreach (DataRow dr in tempQuotation.Rows)
                    {
                        string strSrlNo = Convert.ToString(dr["SrlNo"]);
                        string strDetailsId = Convert.ToString(dr["DetailsId"]);
                        decimal strProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                        string StockUOM = Convert.ToString(dr["UOM"]);
                        decimal strUOMQuantity = 0;
                        string Val = "";

                        if (lookup_quotation.Value != null)
                        {
                            Val = strDetailsId;
                        }
                        else
                        {
                            Val = strSrlNo;
                        }
                        if (StockUOM != "0")
                        {
                            GetQuantityBaseOnProductforDetailsId(Val, ref strUOMQuantity);


                            //Rev 24428
                            DataTable dtb = new DataTable();
                            dtb = (DataTable)Session["MultiUOMData"];
                            //if (Session["MultiUOMData"] != null)
                            //{
                            if (dtb.Rows.Count > 0)
                            { 
                                // Mantis Issue 24428
                                //if (strUOMQuantity != null)
                                //{
                                //    if (strProductQuantity != strUOMQuantity)
                                //    {
                                //        validate = "checkMultiUOMData";
                                //        grid.JSProperties["cpcheckMultiUOMData"] = strSrlNo;
                                //        break;
                                //    }
                                //}
                                // End of Mantis Issue 24428
                            }
                            else if (dtb.Rows.Count < 1)
                            {
                                validate = "checkMultiUOMData";
                                grid.JSProperties["cpcheckMultiUOMData"] = strSrlNo;
                                break;
                            }

                            //End Rev 24428

                        }
                    }

                }

                if (Keyval_internalId.Value == "Add")
                {
                    if (lookup_quotation.GridView.GetSelectedFieldValues("Quote_Id").Count > 0)
                    {
                        if (tempQuotation.Rows.Count > 0)
                        {
                            foreach (DataRow dr in tempQuotation.Rows)
                            {
                                Int64 DetailsId = 0;
                                string ProductID = Convert.ToString(dr["ProductID"]);
                                decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                                string Status = Convert.ToString(dr["Status"]);
                                //QuoteOrderDetails_id = Convert.ToString(Session["QuoteOrderDetails_id"]);
                                //Int64 i = 0;
                                //for ( i =(Convert.ToInt64(dr["SrlNo"])-1); i <Convert.ToInt64(dr["SrlNo"]); i++)
                                //{
                                //     DetailsId = Convert.ToInt64(QuoteOrderDetails_id.Split(',')[i]);
                                //}

                                DataTable dtq = oDBEngine.GetDataTable("select isnull(BalanceQty,0) TotQty from tbl_trans_SalesInquiryBalanceMap where  SalesInquirysDocId='" + Convert.ToInt64(dr["InquiryID"]) + "' and  ProductId='" + ProductID + "'");
                                if (dtq.Rows.Count > 0)
                                {
                                    if (ProductID != "")
                                    {
                                        if (ProductQuantity > Convert.ToDecimal(dtq.Rows[0]["TotQty"]))
                                        {
                                            validate = "ExceedQuantity";
                                            break;
                                        }
                                    }
                                }


                            }

                        }
                    }
                }

                if ((hdnPageStatus.Value != "Add") && (Convert.ToString(Session["Lookup_QuotationIds"]) != ""))
                {


                    if (tempQuotation.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tempQuotation.Rows)
                        {
                            string ProductID = Convert.ToString(dr["ProductID"]);
                            decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                            string Status = Convert.ToString(dr["Status"]);

                            DataTable dtq = oDBEngine.GetDataTable("select (ISNULL(TotalQty,0)+isnull(BalanceQty,0)) TotQty from tbl_trans_SalesInquiryBalanceMap where  SalesInquirysDocId='" + Convert.ToInt64(dr["InquiryID"]) + "' and ProductId='" + ProductID + "'");
                            if (dtq.Rows.Count > 0)
                            {
                                if (ProductID != "")
                                {
                                    if (ProductQuantity > Convert.ToDecimal(dtq.Rows[0]["TotQty"]))
                                    {
                                        validate = "ExceedQuantity";
                                        break;
                                    }
                                }
                            }

                        }

                    }
                }


                if (hdnPageStatus.Value == "update")
                {
                    if (tempQuotation.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tempQuotation.Rows)
                        {
                            string ProductID = Convert.ToString(dr["ProductID"]);
                            decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                            string Status = Convert.ToString(dr["Status"]);
                            DataTable dtq = new DataTable();
                            DataTable dtdetails = new DataTable();

                            dtq = oDBEngine.GetDataTable("select top 1 ISNULL(TotalQty,0) TotalQty,isnull(BalanceQty,0) BalanceQty from tbl_trans_SalesBalanceMap where ProductId='" + ProductID + "' and SalesDocId=(select Quote_Id from tbl_trans_Quotation where Quote_Number='" + txt_PLQuoteNo.Text + "' and ISNULL(QuoteLastEntry,0)=1)");
                            // dtdetails = oDBEngine.GetDataTable("select top 1 isnull(OrderDetails_Quantity,0.0000) Quantity from tbl_trans_PurchaseOrderDetails where  Requisition_Id='" + Convert.ToInt64(IndentId) + "' and OrderDetails_ProductId='" + ProductID + "'");
                            if (Status != "D" && dtq != null && dtq.Rows.Count > 0)
                            {

                                if (ProductQuantity < Convert.ToDecimal(dtq.Rows[0]["TotalQty"]))
                                {
                                    validate = "ReducedQuantity";
                                    break;
                                }
                            }
                           
                        }
                    }
                }


                DataTable duplicatedt = tempQuotation.Copy();
                DataView dvData = new DataView(duplicatedt);
                dvData.RowFilter = "Status <> 'D'";
                duplicatedt = dvData.ToTable();

                var duplicateRecords = duplicatedt.AsEnumerable()
               .GroupBy(r => r["ProductID"]) //coloumn name which has the duplicate values
               .Where(gr => gr.Count() > 1)
               .Select(g => g.Key);

                foreach (var d in duplicateRecords)
                {
                    validate = "duplicateProduct";
                }

                if (ddlInventory.SelectedValue != "N")
                {
                    foreach (DataRow dr in tempQuotation.Rows)
                    {
                        decimal ProductQuantity = Convert.ToDecimal(dr["Quantity"]);
                        string Status = Convert.ToString(dr["Status"]);
                        string ProductID = Convert.ToString(dr["ProductID"]);
                        if (Status != "D" && ProductID != "0")
                        {
                            if (ProductQuantity == 0)
                            {
                                validate = "nullQuantity";
                                break;
                            }
                        }
                    }
                }




                if (Convert.ToString(BillingShippingControl.GeteShippingStateCode()) == "0")
                {
                    validate = "BillingShippingNull";
                }
                //decimal ProductAmount = 0;
                //foreach (DataRow dr in tempQuotation.Rows)
                //{
                //    ProductAmount = ProductAmount + Convert.ToDecimal(dr["Amount"]);
                //}

                //if (ProductAmount == 0)
                //{
                //    validate = "nullAmount";
                //}

                if (Convert.ToString(Session["ActionType"]) == "Add")
                {
                    if (!reCat.isAllMandetoryDone((DataTable)Session["UdfDataOnAdd"], "SQO"))
                    {
                        validate = "errorUdf";

                    }
                }

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 

                //  BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();


                //DataTable DT = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Transporter_QOMandatory' AND IsActive=1");
                //if (DT != null && DT.Rows.Count > 0)
                //{
                //    string IsMandatory = Convert.ToString(DT.Rows[0]["Variable_Value"]).Trim();

                //    if (Convert.ToString(Session["TransporterVisibilty"]).Trim() == "Yes")
                //    {
                //        if (IsMandatory == "Yes")
                //        {
                //            //if (VehicleDetailsControl.GetControlValue("cmbTransporter") == "")
                //            if (hfControlData.Value.Trim() == "")
                //            {
                //                validate = "transporteMandatory";
                //            }
                //        }
                //    }
                //}

                //// ############# Added By : Samrat Roy -- 02/05/2017 -- To check Transporter Mandatory Control 

                //################# Added By : Sayan Dutta -- 10/07/2017 -- To check TC Mandatory Control
                //#region TC
                //DataTable DT_TC = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='TC_QOMandatory' AND IsActive=1");
                //if (DT_TC != null && DT_TC.Rows.Count > 0)
                //{
                //    string IsMandatory = Convert.ToString(DT_TC.Rows[0]["Variable_Value"]).Trim();

                //    //  objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

                //    objEngine = new BusinessLogicLayer.DBEngine();

                //    //DataTable DTVisible = objEngine.GetDataTable("Config_SystemSettings", " Variable_Value ", " Variable_Name='Show_TC_QO' AND IsActive=1");
                //    //if (Convert.ToString(DTVisible.Rows[0]["Variable_Value"]).Trim() == "Yes")
                //    //{
                //    //    if (IsMandatory == "Yes")
                //    //    {
                //    //        if (TermsConditionsControl.GetControlValue("dtDeliveryDate") == "" || TermsConditionsControl.GetControlValue("dtDeliveryDate") == "@")
                //    //        {
                //    //            validate = "TCMandatory";
                //    //        }
                //    //    }
                //    //}
                //}
                //#endregion
                //################# Added By : Sayan Dutta -- 10/07/2017 -- To check TC Mandatory Control

                if (Convert.ToString(BillingShippingControl.GeteShippingStateCode()).Trim() == "")
                {
                    validate = "nullStateCode";
                }

                if (validate == "outrange" || validate == "ReducedQuantity" || validate == "ExceedQuantity" || validate == "duplicate" || validate == "checkWarehouse" || validate == "duplicateProduct" || validate == "nullAmount" || validate == "nullQuantity" || validate == "errorUdf" || validate == "transporteMandatory" || validate == "nullStateCode" || validate == "TCMandatory" || validate == "BillingShippingNull" || validate == "checkMultiUOMData")
                {
                    grid.JSProperties["cpSaveSuccessOrFail"] = validate;
                }
                else
                {
                    

                    //#region To Show By Default Cursor after SAVE AND NEW
                    if (Convert.ToString(Session["ActionType"]) == "Add") // session has been removed from quotation list page working good
                    {
                        string[] schemaid = new string[] { };
                        string schemavalue = ddl_numberingScheme.SelectedValue;
                        Session["schemavalue"] = schemavalue;        // session has been removed from quotation list page working good
                        schemaid = ddl_numberingScheme.SelectedValue.Split('~');

                        string schematype = schemaid[1];
                        if (schematype == "1")
                        {
                            Session["SaveMode"] = "A";
                        }
                        else
                        {
                            Session["SaveMode"] = "M";
                        }
                    }

                    //#endregion

                    string TaxType = "", ShippingState = "";
                    ShippingState = Convert.ToString(BillingShippingControl.GetShippingStateId());

                    //if (ddl_AmountAre.Value == "1") TaxType = "E";
                    //else if (ddl_AmountAre.Value == "2") TaxType = "I";

                  //  TaxDetailTable = gstTaxDetails.SetTaxTableDataWithProductSerialWithException(tempQuotation, "SrlNo", "ProductID", "Amount", "TaxAmount", TaxDetailTable, "S", dt_PLQuote.Date.ToString("yyyy-MM-dd"), strBranch, ShippingState, TaxType,hdnCustomerId.Value,"Quantity","SQ");

                    //#region Subhabrata Section Start

                    int strIsComplete = 0, strQuoteID = 0;


                    //#region Add New Filed To Check from Table
                    DataTable duplicatedt2 = new DataTable();
                    if (Convert.ToString(hdBasketId.Value).Trim() == "")
                    {

                        duplicatedt2.Columns.Add("productid", typeof(Int64));
                        duplicatedt2.Columns.Add("slno", typeof(Int32));
                        duplicatedt2.Columns.Add("Quantity", typeof(Decimal));
                        duplicatedt2.Columns.Add("packing", typeof(Decimal));
                        duplicatedt2.Columns.Add("PackingUom", typeof(Int32));
                        duplicatedt2.Columns.Add("PackingSelectUom", typeof(Int32));

                        if (HttpContext.Current.Session["SessionPackingDetails"] != null)
                        {
                            List<ProductQuantity> obj = new List<ProductQuantity>();
                            obj = (List<ProductQuantity>)HttpContext.Current.Session["SessionPackingDetails"];
                            foreach (var item in obj)
                            {
                                duplicatedt2.Rows.Add(item.productid, item.slno, item.Quantity, item.packing, item.PackingUom, item.PackingSelectUom);
                            }
                        }
                    }
                    else
                    {
                        PosSalesInvoiceBl pos = new PosSalesInvoiceBl();
                        DataSet busketSet = pos.GetQuotationBusketById(Convert.ToInt32(hdBasketId.Value));

                        DataTable GrdpackQty = busketSet.Tables[3];
                        duplicatedt2.Columns.Add("productid", typeof(Int64));
                        duplicatedt2.Columns.Add("pslno", typeof(Int32));
                        duplicatedt2.Columns.Add("pQuantity", typeof(Decimal));
                        duplicatedt2.Columns.Add("packing", typeof(Decimal));
                        duplicatedt2.Columns.Add("PackingUom", typeof(Int32));
                        duplicatedt2.Columns.Add("PackingSelectUom", typeof(Int32));
                        HttpContext.Current.Session["SessionPackingDetails"] = GetWaitingProductDetails(GrdpackQty);
                        if (HttpContext.Current.Session["SessionPackingDetails"] != null)
                        {
                            List<WaitingProductQuantity> obj = new List<WaitingProductQuantity>();
                            obj = (List<WaitingProductQuantity>)HttpContext.Current.Session["SessionPackingDetails"];
                            foreach (var item in obj)
                            {
                                duplicatedt2.Rows.Add(item.productid, item.pslno, item.pQuantity, item.packing, item.PackingUom, item.PackingSelectUom);
                            }
                        }
                    }
                    HttpContext.Current.Session["SessionPackingDetails"] = null;
                    //#endregion
                    //Rev Rajdip
                    if (Convert.ToString(Request.QueryString["Typenew"]) == "Copy")
                    {
                        MainQuotationID = "";
                    }
                    //string type = Opttype.SelectedValue.ToString();

                    //Rev Rajdip
                    if (tempQuotation.Columns.Contains("QuotationType"))
                    {
                        tempQuotation.Columns.Remove("QuotationType");
                    }


                    Int64 ProjId = 0;
                    if (lookup_Project.Text != "")
                    {
                         ProjId = Convert.ToInt64(lookup_Project.Value);
                        
                    }
                  

                    else
                    {
                        ProjId = 0;
                    }


                    if (hdnApproveStatus.Value == "")
                    {
                        hdnApproveStatus.Value = "0";
                    }
                    int ApproveRejectstatus = Convert.ToInt32(hdnApproveStatus.Value);
                    DataTable addrDesc = new DataTable();
                    addrDesc = (DataTable)Session["InlineRemarks"];
                    //string docdate = Convert.ToString(dt_Quotation.Text);


                    String Inquiry_IDs = "";
                    string IndentRequisitionDate = string.Empty;
                    for (int i = 0; i < lookup_quotation.GridView.GetSelectedFieldValues("Quote_Id").Count; i++)
                    {
                        Inquiry_IDs += "," + Convert.ToString(lookup_quotation.GridView.GetSelectedFieldValues("Quote_Id")[i]);
                    }
                    Inquiry_IDs = Inquiry_IDs.TrimStart(',');

                    String InquiryDetailsIDs = "", ProductIDs = "";

                    for (int i = 0; i < grid_Products.GetSelectedFieldValues("ProductID").Count; i++)
                    {
                        ProductIDs += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("ProductID")[i]);
                        InquiryDetailsIDs += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("Key_UniqueId")[i]);
                    }
                    InquiryDetailsIDs = InquiryDetailsIDs.TrimStart(',');

                    

                    ModifyEstimateCost(MainQuotationID, strSchemeType, UniqueQuotation, strQuoteDate, strCustomer, strContactName, ProjId,
                                     strBranch, strAgents, strTaxCode, tempQuotation, addrDesc
                                   //  , TaxDetailTable,
                                  //  tempWarehousedt, tempTaxDetailsdt
                                    ,tempBillAddress, approveStatus, ActionType, ref strIsComplete, ref strQuoteID, strIsInventory,
                                    duplicatedt2, MultiUOMDetails, ProjRemarks, Schema_ID, Inquiry_IDs, InquiryDetailsIDs, strDocidsDate
                                     , ApproveRejectstatus, AppRejRemarks, RevisionDate, RevisionNo);


                  //  grid.JSProperties["cpQuotationNo"] = UniqueQuotation;

                    if (strIsComplete == 1)
                    {
                        if (approveStatus != "")
                        {
                            if (approveStatus == "2")
                            {
                                grid.JSProperties["cpApproverStatus"] = "approve";
                            }
                            else
                            {
                                grid.JSProperties["cpApproverStatus"] = "rejected";
                            }
                        }

                        if (Request.QueryString.AllKeys.Contains("SalId") && !string.IsNullOrEmpty(Request.QueryString["SalId"]))
                        {
                            grid.JSProperties["cpCRMSavedORNot"] = "crmQuotationSaved";
                        }


                        //Employee_BL objemployeebal = new Employee_BL();
                        //int MailStatus = 0;
                        //string AssignedTo_Email = string.Empty;
                        //string ReceiverEmail = string.Empty;
                        //decimal Level1_User = Convert.ToDecimal(Session["userid"]);
                        //decimal Level2User = Convert.ToDecimal(Session["userid"]);
                        //decimal Level3User = Convert.ToDecimal(Session["userid"]);
                        //bool L1 = false;
                        //bool L2 = false;
                        //bool L3 = false;
                        //string ActivityName = string.Empty;

                        //DataTable dtbl_AssignedTo = new DataTable();
                        //DataTable dtbl_AssignedBy = new DataTable();
                        //DataTable dtEmail_To = new DataTable();
                        //DataTable dt_EmailConfig = new DataTable();
                        //DataTable dt_ActivityName = new DataTable();
                        //DataTable Dt_LevelUser = new DataTable();

                        //dt_EmailConfig = objemployeebal.GetEmailAccountConfigDetails("", 1); // Checked fetch datatatable of email setup with action 1 param
                        //Dt_LevelUser = objemployeebal.GetAllLevelUsers("1", 12);

                        //ActivityName = UniqueQuotation;

                        //if (Dt_LevelUser != null && string.IsNullOrEmpty(approveStatus))
                        //{
                        //    L1 = true;
                        //    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 1);
                        //}
                        //else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level1_user_id") == Level1_User && approveStatus == "2")
                        //{
                        //    L2 = true;
                        //    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 2);
                        //}
                        //else if (Dt_LevelUser != null && Dt_LevelUser.Rows[0].Field<decimal>("Level2_user_id") == Level2User && approveStatus == "2")
                        //{
                        //    L3 = true;
                        //    dtEmail_To = objemployeebal.GetEmailLevelUsersWise(1, 11, 3);
                        //}

                        //if (dtEmail_To != null && dtEmail_To.Rows.Count > 0)
                        //{
                        //    if (!string.IsNullOrEmpty(dtEmail_To.Rows[0].Field<string>("Email_Id")))
                        //    {
                        //        ReceiverEmail = Convert.ToString(dtEmail_To.Rows[0].Field<string>("Email_Id"));
                        //    }
                        //    else
                        //    {
                        //        ReceiverEmail = "";
                        //    }
                        //}



                        //ListDictionary replacements = new ListDictionary();
                        //if (dtEmail_To.Rows.Count > 0)
                        //{
                        //    replacements.Add("<%Approver%>", Convert.ToString(dtEmail_To.Rows[0].Field<string>("Approver")));
                        //}
                        //else
                        //{
                        //    replacements.Add("<%Approver%>", "");
                        //}
                        //replacements.Add("<%QuotationNo%>", UniqueQuotation);
                        //ExceptionLogging.SendExceptionMail(ex, Convert.ToInt32(lineNumber));

                        //if (!string.IsNullOrEmpty(ReceiverEmail))
                        //{
                        //    //ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, "~/OMS/EmailTemplates/EmailSendToAssigneeByUserID.html", dt_EmailConfig, ActivityName,4);
                        //    MailStatus = ExceptionLogging.SendEmailToAssigneeByUser(ReceiverEmail, "", replacements, dt_EmailConfig, ActivityName, 12);
                        //}
                    }
                    else if (strIsComplete == -12)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "EmptyProject";
                    }
                    else if (strIsComplete == -9)
                    {
                        DataTable dtlock = new DataTable();
                        dtlock = GetAddLockData();
                        grid.JSProperties["cpAddLockStatus"] = (Convert.ToString(dtlock.Rows[0]["Lock_Fromdate"]) + " to " + Convert.ToString(dtlock.Rows[0]["Lock_Todate"]));
                        grid.JSProperties["cpSaveSuccessOrFail"] = "AddLock";
                    }
                    else if (strIsComplete == 2)
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "quantityTagged";
                    }
                    else
                    {
                        grid.JSProperties["cpSaveSuccessOrFail"] = "errorInsert";
                    }

                    //#endregion Subhabrata Section End
                }
            }
            else
            {
                DataView dvData = new DataView(EstimateCostdt);
                dvData.RowFilter = "Status <> 'D'";

                //grid.DataSource = GetQuotation(dvData.ToTable());
                //grid.DataBind();
            }
        }




        public DataTable GetAddLockData()
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetAddLockForQuotation");
            dt = proc.GetTable();
            return dt;
        }
        protected void grid_DataBinding(object sender, EventArgs e)
        {
            //string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
            //if (IsDeleteFrom == "D")
            //{
            //    DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];
            //    grid.DataSource = GetQuotation(EstimateCostdt);
            //}
            //else
            //{
            //    grid.DataSource = GetQuotation();
            //}

            if (Session["EstimateCostDetails"] != null)
            {
                DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];
                DataView dvData = new DataView(EstimateCostdt);
                dvData.RowFilter = "Status <> 'D'";
                grid.DataSource = GetQuotation(dvData.ToTable());
            }
        }
        protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "Display")
            {
                string IsDeleteFrom = Convert.ToString(hdfIsDelete.Value);
                if (IsDeleteFrom == "D")
                {
                    DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];
                    grid.DataSource = GetQuotation(EstimateCostdt);
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "GridBlank")
            {
                Session["EstimateCostDetails"] = null;
                grid.JSProperties["cpSaveSuccessOrFail"] = "Addnewrow";
                grid.DataSource = null;
                grid.DataBind();
            }
            else if (strSplitCommand == "CurrencyChangeDisplay")
            {
                if (Session["EstimateCostDetails"] != null)
                {
                    DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];

                    string BaseCurrencyId = Convert.ToString(Session["LocalCurrency"]).Split('~')[0];
                    //string CurrentCurrencyId = Convert.ToString(ddl_Currency.SelectedValue);

                    //string strCurrRate = Convert.ToString(txt_Rate.Text);
                    string strPreviouseRate = Convert.ToString(e.Parameters.Split('~')[1]);
                    decimal strRate = 1;

                    //if (strCurrRate != "")
                    //{
                    //    strRate = Convert.ToDecimal(strCurrRate);
                    //    if (strRate == 0) strRate = 1;
                    //}

                    foreach (DataRow dr in EstimateCostdt.Rows)
                    {
                        string Status = Convert.ToString(dr["Status"]);
                        if (Status != "D")
                        {
                            string ProductDetails = Convert.ToString(dr["ProductID"]);
                            string QuantityValue = Convert.ToString(dr["Quantity"]);
                            string Discount = Convert.ToString(dr["Discount"]);
                            string strSalePrice = Convert.ToString(dr["SalePrice"]);

                            string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                            string strMultiplier = ProductDetailsList[7];
                            string strFactor = ProductDetailsList[8];

                            if (Convert.ToDecimal(strPreviouseRate) != 0 && Convert.ToDecimal(strPreviouseRate) != 1)
                            {
                                strSalePrice = Convert.ToString(Convert.ToDecimal(strSalePrice) * Convert.ToDecimal(strPreviouseRate));
                            }

                            //if (BaseCurrencyId == CurrentCurrencyId)
                            //{
                            //    strSalePrice = ProductDetailsList[6];
                            //}
                            strSalePrice = ProductDetailsList[6];
                            decimal SalePrice = Math.Round(Convert.ToDecimal(strSalePrice) / strRate, 2);
                            string Amount = Convert.ToString(Math.Round((Convert.ToDecimal(QuantityValue) * Convert.ToDecimal(strFactor) * SalePrice), 2));
                            string amountAfterDiscount = Convert.ToString(Math.Round((Convert.ToDecimal(Amount) - ((Convert.ToDecimal(Discount) * Convert.ToDecimal(Amount)) / 100)), 2));

                            dr["SalePrice"] = Convert.ToString(Math.Round(SalePrice, 2));
                            dr["Amount"] = amountAfterDiscount;
                            dr["TaxAmount"] = ReCalculateTaxAmount(Convert.ToString(dr["SrlNo"]), Convert.ToDouble(amountAfterDiscount));
                            dr["TotalAmount"] = Convert.ToDecimal(dr["Amount"]) + Convert.ToDecimal(dr["TaxAmount"]);
                        }
                    }

                    Session["EstimateCostDetails"] = EstimateCostdt;

                    DataView dvData = new DataView(EstimateCostdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "CurrencyRateChangeDisplay")
            {
                if (Session["EstimateCostDetails"] != null)
                {
                    DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];

                    string BaseCurrencyId = Convert.ToString(Session["LocalCurrency"]).Split('~')[0];
                    //string CurrentCurrencyId = Convert.ToString(ddl_Currency.SelectedValue);

                    //string strCurrRate = Convert.ToString(txt_Rate.Text);
                    string strPreviouseRate = Convert.ToString(e.Parameters.Split('~')[1]);
                    decimal strRate = 1;

                    //if (strCurrRate != "")
                    //{
                    //    strRate = Convert.ToDecimal(strCurrRate);
                    //    if (strRate == 0) strRate = 1;
                    //}

                    foreach (DataRow dr in EstimateCostdt.Rows)
                    {
                        string Status = Convert.ToString(dr["Status"]);
                        if (Status != "D")
                        {
                            string ProductDetails = Convert.ToString(dr["ProductID"]);
                            string QuantityValue = Convert.ToString(dr["Quantity"]);
                            string Discount = Convert.ToString(dr["Discount"]);
                            string strSalePrice = Convert.ToString(dr["SalePrice"]);

                            string[] ProductDetailsList = ProductDetails.Split(new string[] { "||@||" }, StringSplitOptions.None);
                            string strMultiplier = ProductDetailsList[7];
                            string strFactor = ProductDetailsList[8];

                            if (Convert.ToDecimal(strPreviouseRate) != 0 && Convert.ToDecimal(strPreviouseRate) != 1)
                            {
                                strSalePrice = Convert.ToString(Convert.ToDecimal(strSalePrice) * Convert.ToDecimal(strPreviouseRate));
                            }

                            decimal SalePrice = Math.Round(Convert.ToDecimal(strSalePrice) / strRate, 2);
                            string Amount = Convert.ToString(Math.Round((Convert.ToDecimal(QuantityValue) * Convert.ToDecimal(strFactor) * SalePrice), 2));
                            string amountAfterDiscount = Convert.ToString(Math.Round((Convert.ToDecimal(Amount) - ((Convert.ToDecimal(Discount) * Convert.ToDecimal(Amount)) / 100)), 2));

                            dr["SalePrice"] = Convert.ToString(Math.Round(SalePrice, 2));
                            dr["Amount"] = amountAfterDiscount;
                            dr["TaxAmount"] = ReCalculateTaxAmount(Convert.ToString(dr["SrlNo"]), Convert.ToDouble(amountAfterDiscount));
                            dr["TotalAmount"] = Convert.ToDecimal(dr["Amount"]) + Convert.ToDecimal(dr["TaxAmount"]);
                        }
                    }

                    Session["EstimateCostDetails"] = EstimateCostdt;

                    DataView dvData = new DataView(EstimateCostdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }
            else if (strSplitCommand == "DateChangeDisplay")
            {
                if (Session["EstimateCostDetails"] != null)
                {
                    DataTable EstimateCostdt = (DataTable)Session["EstimateCostDetails"];

                    //string strCurrRate = Convert.ToString(txt_Rate.Text);
                    decimal strRate = 1;

                    //if (strCurrRate != "")
                    //{
                    //    strRate = Convert.ToDecimal(strCurrRate);
                    //    if (strRate == 0) strRate = 1;
                    //}

                    foreach (DataRow dr in EstimateCostdt.Rows)
                    {
                        string Status = Convert.ToString(dr["Status"]);
                        if (Status != "D")
                        {
                            dr["TaxAmount"] = 0;
                            dr["TotalAmount"] = dr["Amount"];
                        }
                    }


                    Session["EstimateCostDetails"] = EstimateCostdt;

                    DataView dvData = new DataView(EstimateCostdt);
                    dvData.RowFilter = "Status <> 'D'";

                    grid.DataSource = GetQuotation(dvData.ToTable());
                    grid.DataBind();
                }
            }

            else if (strSplitCommand == "BindGridOnQuotation")
            {
                ASPxGridView gv = sender as ASPxGridView;
                string command = e.Parameters.ToString();
                string State = Convert.ToString(e.Parameters.Split('~')[1]);
                String QuoComponent1 = "";
                string Product_id1 = "";
                string QuoteDetails_Id = "";
                QuotationIds = string.Empty;
                int AmountsAre = 0;
                for (int i = 0; i < grid_Products.GetSelectedFieldValues("Quotation_No").Count; i++)
                {
                    QuoComponent1 += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("Quotation_No")[i]);
                    Product_id1 += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("ProductID")[i]);
                    QuoteDetails_Id += "," + Convert.ToString(grid_Products.GetSelectedFieldValues("QuoteDetails_Id")[i]);
                    //chinmoy added for taxtype 
                    AmountsAre = Convert.ToInt32(grid_Products.GetSelectedFieldValues("TaxAmountType")[0]);
                }
                QuotationIds = QuoComponent1;
                QuoComponent1 = QuoComponent1.TrimStart(',');
                Product_id1 = Product_id1.TrimStart(',');
                QuoteDetails_Id = QuoteDetails_Id.TrimStart(',');
                string Quote_Nos = Convert.ToString(e.Parameters.Split('~')[1]);
                if (AmountsAre == 1)
                {
                    //ddl_AmountAre.Value = "1";
                    //ddl_AmountAre.ClientEnabled = false;
                }
                else if (AmountsAre == 2)
                {
                    //ddl_AmountAre.Value = "2";
                    //ddl_AmountAre.ClientEnabled = false;
                }
                if (Quote_Nos != "$")
                {
                    if (!string.IsNullOrEmpty(QuoComponent1))
                    {
                        DataTable dt_EstimateCostDetails = new DataTable();
                        DataSet dt_Quotationtagged = new DataSet();
                        DataTable MultiUOMTaggedData = new DataTable();
                        string IdKey = Convert.ToString(Request.QueryString["key"]);
                        if (!string.IsNullOrEmpty(IdKey))
                        {
                           // Rev Sanchita
                            //if (IdKey != "ADD")
                            //{
                            //    dt_Quotationtagged = objSlaesActivitiesBL.GetInquirytaggingDetailsFromSalesQuotation(QuoComponent1, QuoteDetails_Id, Product_id1, "Edit");
                            //    dt_EstimateCostDetails = dt_Quotationtagged.Tables[0];
                            //}
                            //else
                            //{
                            //    dt_Quotationtagged = objSlaesActivitiesBL.GetInquirytaggingDetailsFromSalesQuotation(QuoComponent1, QuoteDetails_Id, Product_id1, "Add");
                            //    dt_EstimateCostDetails = dt_Quotationtagged.Tables[0];

                            //}
                            //MultiUOMTaggedData = objSlaesActivitiesBL.GetQuotationFromInquiryMultiUOM(QuoteDetails_Id);

                            if (IdKey != "ADD")
                            {
                                dt_Quotationtagged = GetInquirytaggingDetailsFromSalesQuotation_New(QuoComponent1, QuoteDetails_Id, Product_id1, "Edit");
                                dt_EstimateCostDetails = dt_Quotationtagged.Tables[0];
                            }
                            else
                            {
                                dt_Quotationtagged = GetInquirytaggingDetailsFromSalesQuotation_New(QuoComponent1, QuoteDetails_Id, Product_id1, "Add");
                                dt_EstimateCostDetails = dt_Quotationtagged.Tables[0];

                            }
                            MultiUOMTaggedData = objSlaesActivitiesBL.GetQuotationFromInquiryMultiUOM_ECS(QuoteDetails_Id);
                            // End of Rev Sanchita
                        }
                        else
                        {
                            //dt_EstimateCostDetails = objSlaesActivitiesBL.GetEstimateCostDetailsFromSalesOrder(QuoComponent1, QuoteDetails_Id, Product_id1);
                        }
                        Session["InlineRemarks"] = dt_Quotationtagged.Tables[1];
                        Session["MultiUOMData"] = MultiUOMTaggedData;
                        Session["OrderDetails"] = null;
                        grid.DataSource = GetQuotationInfo(dt_EstimateCostDetails, IdKey);
                        grid.DataBind();
                        ////Rev Rajdip For Running Total
                        //DataTable Orderdt = dt_EstimateCostDetails.Copy();
                        //decimal TotalQty = 0;
                        //decimal TotalAmt = 0;
                        //decimal TaxAmount = 0;
                        //decimal Amount = 0;
                        //decimal SalePrice = 0;
                        //decimal AmountWithTaxValue = 0;
                        //for (int i = 0; i < Orderdt.Rows.Count; i++)
                        //{
                        //    TotalQty = TotalQty + Convert.ToDecimal(Orderdt.Rows[i]["QuoteDetails_Quantity"]);
                        //    Amount = Amount + Convert.ToDecimal(Orderdt.Rows[i]["Amount"]);
                        //    TaxAmount = TaxAmount + Convert.ToDecimal(Orderdt.Rows[i]["TaxAmount"]);
                        //    SalePrice = SalePrice + Convert.ToDecimal(Orderdt.Rows[i]["SalePrice"]);
                        //    TotalAmt = TotalAmt + Convert.ToDecimal(Orderdt.Rows[i]["TotalAmount"]);
                        //}
                        //AmountWithTaxValue = TaxAmount + Amount;
                        //ASPxLabel12.Text = TotalQty.ToString();
                        //bnrLblTaxableAmtval.Text = Amount.ToString();
                        //bnrLblTaxAmtval.Text = TaxAmount.ToString();
                        //bnrlblAmountWithTaxValue.Text = AmountWithTaxValue.ToString();
                        //bnrLblInvValue.Text = TotalAmt.ToString();
                        //grid.JSProperties["cpRunningTotal"] = TotalQty + "~" + Amount + "~" + TaxAmount + "~" + AmountWithTaxValue + "~" + TotalAmt;
                        ////end rev rajdip
                    }
                }
                else
                {
                    grid.DataSource = null;
                    grid.DataBind();
                }
                string QuoCom = Convert.ToString(QuoComponent1.Split(',')[0]);
                grid.JSProperties["cpLoadAddressFromQuote"] = QuoCom;
            }

        }

        public void ModifyEstimateCost(string QuotationID, string strSchemeType, string strQuoteNo, string strQuoteDate, string strCustomer, string strContactName, Int64 ProjId,
             string strBranch, string strAgents, string strTaxCode, DataTable EstimateCostdt,DataTable addrDesc                                  
            , DataTable BillAddressdt, string approveStatus, string ActionType,
            ref int strIsComplete, ref int strQuoteID, string strIsInventory, DataTable QuotationPackingDetailsdt, DataTable MultiUOMDetails,
            string ProjRemarks, int Schema_ID, string Inquiry_IDs, string InquiryDetailsIDs, string strDocidsDate
            , int ApproveRejectstatus, string AppRejRemarks, string RevisionDate, string RevisionNo)
        {
            try
            {
                if (EstimateCostdt.Columns.Contains("ProductName"))
                {
                    EstimateCostdt.Columns.Remove("ProductName");
                    EstimateCostdt.AcceptChanges();
                }
                if (hdnApprovalsettings.Value == "0")
                {
                    ApproveRejectstatus = 1;
                }
                 
                DataSet dsInst = new DataSet();               
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));              
                SqlCommand cmd = new SqlCommand("prc_EstimateCostSheetAddEdit", con);               
                cmd.CommandType = CommandType.StoredProcedure;                
                //cmd.Parameters.AddWithValue("@Action", ActionType);
                if ((ApproveRejectstatus == 1) && (hdnPageStatForApprove.Value == "") && (hdnApprovalsettings.Value == "1"))
                {
                    cmd.Parameters.AddWithValue("@Action", "Add");
                }
                else if (ApproveRejectstatus == 1 && hdnApprovalsettings.Value == "1")
                {
                    cmd.Parameters.AddWithValue("@Action", "AppprovalEdit");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Action", ActionType);
                }

                strQuoteNo = txt_PLQuoteNo.Text;          
                cmd.Parameters.AddWithValue("@EstimateCostID", QuotationID);               
                cmd.Parameters.AddWithValue("@IsInventory", strIsInventory);
                cmd.Parameters.AddWithValue("@QuoteNo", strQuoteNo);
                cmd.Parameters.AddWithValue("@QuoteDate", strQuoteDate);                
                cmd.Parameters.AddWithValue("@CustomerID", strCustomer);
                cmd.Parameters.AddWithValue("@ContactPerson", strContactName);                
                cmd.Parameters.AddWithValue("@BranchID", strBranch);
                cmd.Parameters.AddWithValue("@Agents", strAgents);
                cmd.Parameters.AddWithValue("@SchemaID", Schema_ID);              
                cmd.Parameters.AddWithValue("@CompanyID", Convert.ToString(Session["LastCompany"]));            
                cmd.Parameters.AddWithValue("@FinYear", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@UserID", Convert.ToString(Session["userid"]));
                cmd.Parameters.AddWithValue("@Project_Id", ProjId);
                cmd.Parameters.AddWithValue("@QuotationDetails", EstimateCostdt);              
                cmd.Parameters.AddWithValue("@BillAddress", BillAddressdt);
                cmd.Parameters.AddWithValue("@MultiUOMDetails", MultiUOMDetails);
                cmd.Parameters.AddWithValue("@AddlDescDetails", addrDesc);
                cmd.Parameters.AddWithValue("@ProjRemarks", ProjRemarks);             
                cmd.Parameters.AddWithValue("@QuotationPackingDetails", QuotationPackingDetailsdt);
                cmd.Parameters.AddWithValue("@Inquiry_IDs", Inquiry_IDs);
                cmd.Parameters.AddWithValue("@InquiryDetailsIDs", InquiryDetailsIDs);
                cmd.Parameters.AddWithValue("@DocIdsQuoteDate", strDocidsDate);

                if (RevisionDate != "1/1/0001 12:00:00 AM")
                {
                    if (!String.IsNullOrEmpty(RevisionDate))
                        //cmd.Parameters.AddWithValue("@ProjectValidfromDate", Convert.ToDateTime(projectValidFrom));
                        cmd.Parameters.AddWithValue("@RevisionDate", DateTime.ParseExact(RevisionDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                }

                cmd.Parameters.AddWithValue("@RevisionNo", RevisionNo);

                cmd.Parameters.AddWithValue("@ApproveRejectStatus", ApproveRejectstatus);
                cmd.Parameters.AddWithValue("@ApproveRejectRemarks", AppRejRemarks);

                cmd.Parameters.Add("@ReturnValue", SqlDbType.VarChar, 50);
                cmd.Parameters.Add("@ReturnQuoteID", SqlDbType.VarChar, 50);

                cmd.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
                cmd.Parameters["@ReturnQuoteID"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add("@GeneratedDocNumber", SqlDbType.VarChar, 50);
                cmd.Parameters["@GeneratedDocNumber"].Direction = ParameterDirection.Output;
               
                cmd.CommandTimeout = 0;
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);

                strIsComplete = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value.ToString());
                strQuoteID = Convert.ToInt32(cmd.Parameters["@ReturnQuoteID"].Value.ToString());
                string ECSNumber = Convert.ToString(cmd.Parameters["@GeneratedDocNumber"].Value.ToString());
                if (strQuoteID > 0)
                {
                    grid.JSProperties["cpQuotationNo"] = ECSNumber;
                }
                //    if (!string.IsNullOrEmpty(hfControlData.Value))
                //    {
                //        CommonBL objCommonBL = new CommonBL();
                //        objCommonBL.InsertTransporterControlDetails(strQuoteID, "ECS", hfControlData.Value, Convert.ToString(HttpContext.Current.Session["userid"]));
                //    }
                //}
                //if (strQuoteID > 0)
                //{                    
                //    if (!string.IsNullOrEmpty(hfTermsConditionData.Value))
                //    {
                //        TermsConditionsControl.SaveTC(hfTermsConditionData.Value, Convert.ToString(strQuoteID), "ECS");
                //    }
                //}

                cmd.Dispose();
                con.Dispose();
                if (strIsComplete == 1)
                {
                    DataTable udfTable = (DataTable)Session["UdfDataOnAdd"];
                    if (udfTable != null)
                        Session["UdfDataOnAdd"] = reCat.insertRemarksCategoryAddMode("ECS", "ECS" + Convert.ToString(strQuoteID), udfTable, Convert.ToString(Session["userid"]));
                }

            }
            catch (Exception ex)
            {
            }
        }
        protected void Grid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Grid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void Grid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }


        public DataSet GetInquirytaggingDetailsFromSalesQuotation_New(string Quote_Nos, string Order_Key, string Product_Ids, string Action)
        {
            DataSet DT = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 100, "BindGridInquiryProducts");

            proc.AddVarcharPara("@Inquiry_Id", 4000, Quote_Nos);
            proc.AddVarcharPara("@InquiryDetails_Id", 1000, Order_Key);
            proc.AddVarcharPara("@Product_Id", 1000, Product_Ids);
            //proc.AddVarcharPara("@Action", 10, Action);

            DT = proc.GetDataSet();
            return DT;

        }
        //#endregion

        //#region Quotation Tax Details

        public IEnumerable GetTaxes()
        {
            List<Tax> TaxList = new List<Tax>();
            //  BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

            BusinessLogicLayer.DBEngine objEngine = new BusinessLogicLayer.DBEngine();

            DataTable DT = GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
            for (int i = 0; i < DT.Rows.Count; i++)
            {
                Tax Taxes = new Tax();
                Taxes.TaxID = Convert.ToString(DT.Rows[i]["Taxes_ID"]);
                Taxes.TaxName = Convert.ToString(DT.Rows[i]["Taxes_Name"]);
                Taxes.Percentage = Convert.ToString(DT.Rows[i]["Percentage"]);
                Taxes.Amount = Convert.ToString(DT.Rows[i]["Amount"]);
                TaxList.Add(Taxes);
            }

            return TaxList;
        }

        //public void GetQuantityBaseOnProductforDetailsId(string strSrlNo, ref decimal strUOMQuantity)
        //{
        //    decimal sum = 0;

        //    DataTable MultiUOMData = new DataTable();
        //    if (Session["MultiUOMData"] != null)
        //    {
        //        MultiUOMData = (DataTable)Session["MultiUOMData"];
        //        for (int i = 0; i < MultiUOMData.Rows.Count; i++)
        //        {
        //            DataRow dr = MultiUOMData.Rows[i];
        //            string SrlNo = Convert.ToString(dr["SrlNo"]);

        //            if (strSrlNo == SrlNo)
        //            {
        //                string strQuantity = (Convert.ToString(dr["Quantity"]) != "") ? Convert.ToString(dr["Quantity"]) : "0";
        //                var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);

        //                sum = Convert.ToDecimal(weight);
        //            }
        //        }
        //    }

        //    strUOMQuantity = sum;

        //}
        public void GetQuantityBaseOnProductforDetailsId(string Val, ref decimal strUOMQuantity)
        {
            decimal sum = 0;
            string UomDetailsid = "";
            DataTable MultiUOMData = new DataTable();
            if (Session["MultiUOMData"] != null)
            {
                MultiUOMData = (DataTable)Session["MultiUOMData"];
                for (int i = 0; i < MultiUOMData.Rows.Count; i++)
                {
                    DataRow dr = MultiUOMData.Rows[i];
                    if (lookup_quotation.Value != null)
                    {
                        UomDetailsid = Convert.ToString(dr["DetailsId"]);
                    }
                    else
                    {
                        UomDetailsid = Convert.ToString(dr["SrlNo"]);
                    }

                    if (Val == UomDetailsid)
                    {
                        string strQuantity = (Convert.ToString(dr["Quantity"]) != "") ? Convert.ToString(dr["Quantity"]) : "0";
                        var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);

                        sum = Convert.ToDecimal(weight);
                    }
                }
            }

            strUOMQuantity = sum;

        }

        public void CreateBasketQuoteTaxTable()
        {
            DataTable TaxRecord = new DataTable();

            TaxRecord.Columns.Add("SlNo", typeof(System.Int32));
            TaxRecord.Columns.Add("TaxCode", typeof(System.String));
            TaxRecord.Columns.Add("AltTaxCode", typeof(System.String));
            TaxRecord.Columns.Add("Percentage", typeof(System.Decimal));
            TaxRecord.Columns.Add("Amount", typeof(System.Decimal));
            Session["SI_FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxRecord;
        }
        public IEnumerable GetTaxes(DataTable DT)
        {
            List<Tax> TaxList = new List<Tax>();

            decimal totalParcentage = 0;
            foreach (DataRow dr in DT.Rows)
            {
                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                {
                    totalParcentage += Convert.ToDecimal(dr["Percentage"]);
                }
            }




            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if (Convert.ToString(DT.Rows[i]["Taxes_ID"]) != "0")
                {
                    Tax Taxes = new Tax();
                    Taxes.TaxID = Convert.ToString(DT.Rows[i]["Taxes_ID"]);
                    Taxes.TaxName = Convert.ToString(DT.Rows[i]["Taxes_Name"]);
                    Taxes.Percentage = Convert.ToString(DT.Rows[i]["Percentage"]);
                    Taxes.Amount = Convert.ToString(DT.Rows[i]["Amount"]);
                    if (Convert.ToString(DT.Rows[i]["ApplicableOn"]) == "G")
                    {
                        Taxes.calCulatedOn = Convert.ToDecimal(HdChargeProdAmt.Value);
                    }
                    else if (Convert.ToString(DT.Rows[i]["ApplicableOn"]) == "N")
                    {
                        Taxes.calCulatedOn = Convert.ToDecimal(HdChargeProdNetAmt.Value);
                    }
                    else
                    {
                        Taxes.calCulatedOn = 0;
                    }
                    //Set Amount Value 
                    if (Taxes.Amount == "0.00")
                    {
                        Taxes.Amount = Convert.ToString(Taxes.calCulatedOn * (Convert.ToDecimal(Taxes.Percentage) / 100));
                    }


                    //if (Convert.ToString(ddl_AmountAre.Value) == "2")
                    //{
                    //    if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                    //    {
                    //        if (Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(DT.Rows[i]["TaxTypeCode"]).Trim() == "SGST")
                    //        {
                    //            decimal finalCalCulatedOn = 0;
                    //            decimal backProcessRate = (1 + (totalParcentage / 100));
                    //            finalCalCulatedOn = Taxes.calCulatedOn / backProcessRate;
                    //            Taxes.calCulatedOn = finalCalCulatedOn;
                    //        }
                    //    }
                    //}


                    TaxList.Add(Taxes);
                }
            }

            return TaxList;
        
        }


        protected void callback_InlineRemarks_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string strSplitCommand = e.Parameter.Split('~')[0];
            string SrlNo = e.Parameter.Split('~')[1];
            string RemarksVal = e.Parameter.Split('~')[2];
            Remarks = new DataTable();


            callback_InlineRemarks.JSProperties["cpDisplayFocus"] = "";

            if (strSplitCommand == "RemarksFinal")
            {
                if (Session["InlineRemarks"] != null)
                {
                    Remarks = (DataTable)Session["InlineRemarks"];
                }
                else
                {
                    if (Remarks == null || Remarks.Rows.Count == 0)
                    {
                        Remarks.Columns.Add("SrlNo", typeof(string));
                        Remarks.Columns.Add("Quote_AddDesc", typeof(string));

                    }


                }

                DataRow[] deletedRow = Remarks.Select("SrlNo=" + SrlNo);

                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        Remarks.Rows.Remove(dr);
                    }
                    Remarks.AcceptChanges();
                }


                Remarks.Rows.Add(SrlNo, RemarksVal);

                Session["InlineRemarks"] = Remarks;
            }


            //else if(strSplitCommand=="BindRemarks")
            //{

            //    DataTable dt = GetOrderData().Tables[1];

            //   //txtInlineRemarks.Text=


            //}

            else if (strSplitCommand == "DisplayRemarks")
            {
                DataTable Remarksdt = (DataTable)Session["InlineRemarks"];
                if (Remarksdt != null)
                {
                    DataView dvData = new DataView(Remarksdt);
                    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    if (dvData.Count > 0)
                        txtInlineRemarks.Text = dvData[0]["Quote_AddDesc"].ToString();
                    else
                        txtInlineRemarks.Text = "";
                }

                callback_InlineRemarks.JSProperties["cpDisplayFocus"] = "DisplayRemarksFocus";
            }
            //else if (strSplitCommand=="RemarksDelete")
            //{
            //    DeleteUnsaveaddRemarks(SrlNo, RemarksVal);

            //}


        }


        public DataTable GetTaxData(string quoteDate)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "TaxDetails");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(ddl_Branch.SelectedValue));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            dt = proc.GetTable();
            return dt;
        }
        protected void gridTax_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string strSplitCommand = e.Parameters.Split('~')[0];
            if (strSplitCommand == "Display")
            {
                DataTable TaxDetailsdt = new DataTable();
                if (Session["TaxDetails"] == null)
                {
                    Session["TaxDetails"] = GetTaxData(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                }

                if (Session["TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["TaxDetails"];

                    //#region Delete Igst,Cgst,Sgst respectively
                    //Get Company Gstin 09032017
                    string CompInternalId = Convert.ToString(Session["LastCompany"]);
                    string BranchId = Convert.ToString(ddl_Branch.SelectedValue);
                    string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);
                    string[] branchGstin = oDBEngine.GetFieldValue1("tbl_master_branch", "isnull(branch_GSTIN,'')branch_GSTIN", "branch_id='" + BranchId + "'", 1);
                    String GSTIN = "";
                    if (compGstin.Length > 0)
                    {
                        if (branchGstin[0].Trim() != "")
                        {
                            GSTIN = branchGstin[0].Trim();
                        }
                        else
                        {
                            GSTIN = compGstin[0].Trim();
                        }
                    }

                    string ShippingState = "";
                    //#region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                    string sstateCode = BillingShippingControl.GeteShippingStateCode();
                    ShippingState = sstateCode;
                    //if (ShippingState.Trim() != "")
                    //{
                    //    ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                    //}
                    //#region ##### Old Code -- BillingShipping ######
                    ////if (chkBilling.Checked)
                    ////{
                    ////    if (CmbState.Value != null)
                    ////    {
                    ////        ShippingState = CmbState.Text;
                    ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                    ////    }
                    ////}
                    ////else
                    ////{
                    ////    if (CmbState1.Value != null)
                    ////    {
                    ////        ShippingState = CmbState1.Text;
                    ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                    ////    }
                    ////}
                    //#endregion

                    //#endregion

                    if (ShippingState.Trim() != "" && GSTIN != "")
                    {

                        if (GSTIN.Substring(0, 2) == ShippingState)
                        {
                            //Check if the state is in union territories then only UTGST will apply
                            //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU                               Lakshadweep              PONDICHERRY
                            if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                            {
                                foreach (DataRow dr in TaxDetailsdt.Rows)
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                    {
                                        dr.Delete();
                                    }
                                }

                            }
                            else
                            {
                                foreach (DataRow dr in TaxDetailsdt.Rows)
                                {
                                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                    {
                                        dr.Delete();
                                    }
                                }
                            }
                            TaxDetailsdt.AcceptChanges();
                        }
                        else
                        {
                            foreach (DataRow dr in TaxDetailsdt.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                {
                                    dr.Delete();
                                }
                            }
                            TaxDetailsdt.AcceptChanges();

                        }


                    }

                    //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                    if (GSTIN == "")
                    {
                        foreach (DataRow dr in TaxDetailsdt.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                            {
                                dr.Delete();
                            }
                        }
                        TaxDetailsdt.AcceptChanges();
                    }

                    //#endregion








                    //gridTax.DataSource = GetTaxes();


                    var taxlist = (List<Tax>)GetTaxes(TaxDetailsdt);

                    var taxChargeDataSource = setChargeCalculatedOn(taxlist, TaxDetailsdt);
                    gridTax.DataSource = taxChargeDataSource;
                    gridTax.DataBind();
                    gridTax.JSProperties["cpJsonChargeData"] = createJsonForChargesTax(TaxDetailsdt);
                    gridTax.JSProperties["cpTotalCharges"] = ClculatedTotalCharge(taxChargeDataSource);
                }
            }
            else if (strSplitCommand == "SaveGst")
            {
                DataTable TaxDetailsdt = new DataTable();
                if (Session["TaxDetails"] != null)
                {
                    TaxDetailsdt = (DataTable)Session["TaxDetails"];
                }
                else
                {
                    TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                    TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                    TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                    TaxDetailsdt.Columns.Add("Amount", typeof(string));
                    TaxDetailsdt.Columns.Add("TaxTypeCode", typeof(string));
                    //ForGst
                    TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
                }
                DataRow[] existingRow = TaxDetailsdt.Select("Taxes_ID='0'");
                if (Convert.ToString(cmbGstCstVatcharge.Value) == "0")
                {
                    if (existingRow.Length > 0)
                    {
                        TaxDetailsdt.Rows.Remove(existingRow[0]);
                    }
                }
                else
                {
                    if (existingRow.Length > 0)
                    {
                        existingRow[0]["Percentage"] = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1] : "0";
                        existingRow[0]["Amount"] = txtGstCstVatCharge.Text;
                        existingRow[0]["AltTax_Code"] = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0] : "0";

                    }
                    else
                    {
                        string GstTaxId = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0] : "0";
                        string GstPerCentage = (cmbGstCstVatcharge.Value != null) ? Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1] : "0";

                        string GstAmount = txtGstCstVatCharge.Text;
                        DataRow gstRow = TaxDetailsdt.NewRow();
                        gstRow["Taxes_ID"] = 0;
                        gstRow["Taxes_Name"] = cmbGstCstVatcharge.Text;
                        gstRow["Percentage"] = GstPerCentage;
                        gstRow["Amount"] = GstAmount;
                        gstRow["AltTax_Code"] = GstTaxId;
                        TaxDetailsdt.Rows.Add(gstRow);
                    }

                    Session["TaxDetails"] = TaxDetailsdt;
                }
            }
        }

        protected decimal ClculatedTotalCharge(List<Tax> taxChargeDataSource)
        {
            decimal totalCharges = 0;
            foreach (Tax txObj in taxChargeDataSource)
            {

                if (Convert.ToString(txObj.TaxName).Contains("(+)"))
                {
                    totalCharges += Convert.ToDecimal(txObj.Amount);
                }
                else
                {
                    totalCharges -= Convert.ToDecimal(txObj.Amount);
                }

            }
            totalCharges += Convert.ToDecimal(txtGstCstVatCharge.Text);

            return totalCharges;

        }
        protected void gridTax_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void gridTax_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable TaxDetailsdt = new DataTable();
            if (Session["TaxDetails"] != null)
            {
                TaxDetailsdt = (DataTable)Session["TaxDetails"];
            }
            else
            {
                TaxDetailsdt.Columns.Add("Taxes_ID", typeof(string));
                TaxDetailsdt.Columns.Add("Taxes_Name", typeof(string));
                TaxDetailsdt.Columns.Add("Percentage", typeof(string));
                TaxDetailsdt.Columns.Add("Amount", typeof(string));
                TaxDetailsdt.Columns.Add("TaxTypeCode", typeof(string));
                //ForGst
                TaxDetailsdt.Columns.Add("AltTax_Code", typeof(string));
            }

            foreach (var args in e.UpdateValues)
            {
                string TaxID = Convert.ToString(args.Keys["TaxID"]);
                string TaxName = Convert.ToString(args.NewValues["TaxName"]);
                string Percentage = Convert.ToString(args.NewValues["Percentage"]);
                string Amount = Convert.ToString(args.NewValues["Amount"]);

                bool Isexists = false;
                foreach (DataRow drr in TaxDetailsdt.Rows)
                {
                    string OldTaxID = Convert.ToString(drr["Taxes_ID"]);

                    if (OldTaxID == TaxID)
                    {
                        Isexists = true;

                        drr["Percentage"] = Percentage;
                        drr["Amount"] = Amount;

                        break;
                    }
                }

                if (Isexists == false)
                {
                    TaxDetailsdt.Rows.Add(TaxID, TaxName, Percentage, Amount, 0);
                }
            }

            if (cmbGstCstVatcharge.Value != null)
            {
                DataRow[] existingRow = TaxDetailsdt.Select("Taxes_ID='0'");
                if (Convert.ToString(cmbGstCstVatcharge.Value) == "0")
                {
                    if (existingRow.Length > 0)
                    {
                        TaxDetailsdt.Rows.Remove(existingRow[0]);
                    }
                }
                else
                {
                    if (existingRow.Length > 0)
                    {

                        existingRow[0]["Percentage"] = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1];
                        existingRow[0]["Amount"] = txtGstCstVatCharge.Text;
                        existingRow[0]["AltTax_Code"] = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0]; ;

                    }
                    else
                    {
                        string GstTaxId = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[0];
                        string GstPerCentage = Convert.ToString(cmbGstCstVatcharge.Value).Split('~')[1];
                        string GstAmount = txtGstCstVatCharge.Text;
                        DataRow gstRow = TaxDetailsdt.NewRow();
                        gstRow["Taxes_ID"] = 0;
                        gstRow["Taxes_Name"] = cmbGstCstVatcharge.Text;
                        gstRow["Percentage"] = GstPerCentage;
                        gstRow["Amount"] = GstAmount;
                        gstRow["AltTax_Code"] = GstTaxId;
                        TaxDetailsdt.Rows.Add(gstRow);
                    }
                }
            }

            Session["TaxDetails"] = TaxDetailsdt;

            gridTax.DataSource = GetTaxes(TaxDetailsdt);
            gridTax.DataBind();
        }
        protected void gridTax_DataBinding(object sender, EventArgs e)
        {
            if (Session["TaxDetails"] != null)
            {
                DataTable TaxDetailsdt = (DataTable)Session["TaxDetails"];

                //gridTax.DataSource = GetTaxes();
                var taxlist = (List<Tax>)GetTaxes(TaxDetailsdt);
                var taxChargeDataSource = setChargeCalculatedOn(taxlist, TaxDetailsdt);
                gridTax.DataSource = taxChargeDataSource;
            }
        }


        //#endregion

        //#region Warehouse Details

        public DataTable GetQuotationWarehouseData()
        {
            try
            {

                MasterSettings masterBl = new MasterSettings();
                string multiwarehouse = Convert.ToString(masterBl.GetSettings("IaMultilevelWarehouse"));
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
                proc.AddVarcharPara("@Action", 500, "QuotationWarehouse");
                proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
                proc.AddVarcharPara("@Multiwarehouse", 500, Convert.ToString(multiwarehouse));
                dt = proc.GetTable();

                string strNewVal = "", strOldVal = "";
                DataTable tempdt = dt.Copy();
                foreach (DataRow drr in tempdt.Rows)
                {
                    strNewVal = Convert.ToString(drr["QuoteWarehouse_Id"]);

                    if (strNewVal == strOldVal)
                    {
                        drr["WarehouseName"] = "";
                        drr["Quantity"] = "";
                        drr["BatchNo"] = "";
                        drr["SalesUOMName"] = "";
                        drr["SalesQuantity"] = "";
                        drr["StkUOMName"] = "";
                        drr["StkQuantity"] = "";
                        drr["ConversionMultiplier"] = "";
                        drr["AvailableQty"] = "";
                        drr["BalancrStk"] = "";
                        drr["MfgDate"] = "";
                        drr["ExpiryDate"] = "";
                    }

                    strOldVal = strNewVal;
                }

                Session["LoopWarehouse"] = Convert.ToString(Convert.ToInt32(strNewVal) + 1);
                tempdt.Columns.Remove("QuoteWarehouse_Id");
                return tempdt;
            }
            catch
            {
                return null;
            }
        }
        protected void CmbWarehouse_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindWarehouse")
            {
                DataTable dt = GetWarehouseData();

                CmbWarehouse.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CmbWarehouse.Items.Add(Convert.ToString(dt.Rows[i]["WarehouseName"]), Convert.ToString(dt.Rows[i]["WarehouseID"]));
                }
            }
        }
        protected void CmbBatch_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindBatch")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                DataTable dt = GetBatchData(WarehouseID);

                CmbBatch.Items.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CmbBatch.Items.Add(Convert.ToString(dt.Rows[i]["BatchName"]), Convert.ToString(dt.Rows[i]["BatchID"]));
                }
            }
        }
        protected void CmbSerial_Callback(object sender, CallbackEventArgsBase e)
        {
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindSerial")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                string BatchID = Convert.ToString(e.Parameter.Split('~')[2]);
                DataTable dt = GetSerialata(WarehouseID, BatchID);

                if (Session["WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["WarehouseData"];
                    DataTable tempdt = Warehousedt.DefaultView.ToTable(false, "SerialID");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string SerialID = Convert.ToString(dr["SerialID"]);
                        DataRow[] rows = tempdt.Select("SerialID = '" + SerialID + "' AND SerialID<>'0'");

                        if (rows != null && rows.Length > 0)
                        {
                            dr.Delete();
                        }

                        //foreach (DataRow drr in tempdt.Rows)
                        //{
                        //    string oldSerialID = Convert.ToString(drr["SerialID"]);
                        //    if (newSerialID == oldSerialID)
                        //    {
                        //        dr.Delete();
                        //    }
                        //}

                    }
                    dt.AcceptChanges();

                }

                ASPxListBox lb = sender as ASPxListBox;
                lb.DataSource = dt;
                lb.ValueField = "SerialID";
                lb.TextField = "SerialName";
                lb.ValueType = typeof(string);
                lb.DataBind();
            }
            else if (WhichCall == "EditSerial")
            {
                string WarehouseID = Convert.ToString(e.Parameter.Split('~')[1]);
                string BatchID = Convert.ToString(e.Parameter.Split('~')[2]);
                string editSerialID = Convert.ToString(e.Parameter.Split('~')[3]);
                DataTable dt = GetSerialata(WarehouseID, BatchID);

                if (Session["WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["WarehouseData"];
                    DataTable tempdt = Warehousedt.DefaultView.ToTable(false, "SerialID");

                    foreach (DataRow dr in dt.Rows)
                    {
                        string SerialID = Convert.ToString(dr["SerialID"]);
                        DataRow[] rows = tempdt.Select("SerialID = '" + SerialID + "' AND SerialID not in ('0','" + editSerialID + "')");

                        if (rows != null && rows.Length > 0)
                        {
                            dr.Delete();
                        }

                        //foreach (DataRow drr in tempdt.Rows)
                        //{
                        //    string oldSerialID = Convert.ToString(drr["SerialID"]);
                        //    if (newSerialID == oldSerialID)
                        //    {
                        //        dr.Delete();
                        //    }
                        //}

                    }
                    dt.AcceptChanges();

                }

                ASPxListBox lb = sender as ASPxListBox;
                lb.DataSource = dt;
                lb.ValueField = "SerialID";
                lb.TextField = "SerialName";
                lb.ValueType = typeof(string);
                lb.DataBind();
            }
        }
        protected void listBox_Init(object sender, EventArgs e)
        {
            ASPxListBox lb = sender as ASPxListBox;
            DataTable dt = GetSerialata("", "");

            lb.DataSource = dt;
            lb.ValueField = "SerialID";
            lb.TextField = "SerialName";
            lb.ValueType = typeof(string);
            lb.DataBind();
        }

        protected void MultiUOM_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string SpltCmmd = e.Parameters.Split('~')[0];
            grid_MultiUOM.JSProperties["cpDuplicateAltUOM"] = "";
            grid_MultiUOM.JSProperties["cpOpenFocus"] = "";
            // Rev Sanchita
            grid_MultiUOM.JSProperties["cpSetBaseQtyRateInGrid"] = "";
            // End of Rev Sanchita

            if (SpltCmmd == "MultiUOMDisPlay")
            {

                DataTable MultiUOMData = new DataTable();

                if (Session["MultiUOMData"] != null)
                {
                    MultiUOMData = (DataTable)Session["MultiUOMData"];
                }
                else
                {
                    MultiUOMData.Columns.Add("SrlNo", typeof(string));
                    MultiUOMData.Columns.Add("Quantity", typeof(Decimal));
                    MultiUOMData.Columns.Add("UOM", typeof(string));
                    MultiUOMData.Columns.Add("AltUOM", typeof(string));
                    MultiUOMData.Columns.Add("AltQuantity", typeof(Decimal));
                    MultiUOMData.Columns.Add("UomId", typeof(Int64));
                    MultiUOMData.Columns.Add("AltUomId", typeof(Int64));
                    MultiUOMData.Columns.Add("ProductId", typeof(Int64));
                    MultiUOMData.Columns.Add("DetailsId", typeof(Int64));

                }
                if (MultiUOMData != null && MultiUOMData.Rows.Count > 0)
                {
                    string SrlNo = e.Parameters.Split('~')[1];
                    string DetailsId = e.Parameters.Split('~')[2];


                    DataView dvData = new DataView(MultiUOMData);
                    //dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";
                    // Rev Sanchita
                    //if (DetailsId != "" && DetailsId != null && DetailsId != "null")
                    //{
                    //    //dvData.RowFilter = "SrlNo = '" + SrlNo + "' and Doc_DetailsId='" + DetailsId + "'";
                    //    dvData.RowFilter = "DetailsId='" + DetailsId + "'";
                    //}
                    //else
                    //{
                    //    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    //}
                    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    // End of Rev Sanchita
                    grid_MultiUOM.DataSource = dvData;
                    grid_MultiUOM.DataBind();
                }
                else
                {
                    grid_MultiUOM.DataSource = MultiUOMData.DefaultView;
                    grid_MultiUOM.DataBind();
                }
                grid_MultiUOM.JSProperties["cpOpenFocus"] = "OpenFocus";
            }

            else if (SpltCmmd == "SaveDisplay")
            {

                string Validcheck = "";
                DataTable MultiUOMSaveData = new DataTable();

                int MultiUOMSR = 1;
                string SrlNo = Convert.ToString(e.Parameters.Split('~')[1]);
                string Quantity = Convert.ToString(e.Parameters.Split('~')[2]);
                string UOM = Convert.ToString(e.Parameters.Split('~')[3]);
                string AltUOM = Convert.ToString(e.Parameters.Split('~')[4]);
                string AltQuantity = Convert.ToString(e.Parameters.Split('~')[5]);
                string UomId = Convert.ToString(e.Parameters.Split('~')[6]);
                string AltUomId = Convert.ToString(e.Parameters.Split('~')[7]);
                string ProductId = Convert.ToString(e.Parameters.Split('~')[8]);
                string DetailsId = Convert.ToString(e.Parameters.Split('~')[9]);

                // Mantis Issue 24428
                string BaseRate = Convert.ToString(e.Parameters.Split('~')[10]);
                string AltRate = Convert.ToString(e.Parameters.Split('~')[11]);
                string UpdateRow = Convert.ToString(e.Parameters.Split('~')[12]);
                // End of Mantis Issue 24428

                DataTable allMultidataDetails = (DataTable)Session["MultiUOMData"];


                DataRow[] MultiUoMresult;

                if (allMultidataDetails != null && allMultidataDetails.Rows.Count > 0)
                {

                    //Rev 24428 add condition && DetailsId != "0"
                    if (DetailsId != "" && DetailsId != null && DetailsId != "null" && DetailsId != "0")
                    {
                        //MultiUoMresult = allMultidataDetails.Select("SrlNo ='" + SrlNo + "' and Doc_DetailsId='" + DetailsId + "'");
                        MultiUoMresult = allMultidataDetails.Select("DetailsId='" + DetailsId + "'");
                    }
                    else
                    {
                        MultiUoMresult = allMultidataDetails.Select("SrlNo ='" + SrlNo + "'");
                    }
                    foreach (DataRow item in MultiUoMresult)
                    {
                        if ((AltUomId == item["AltUomId"].ToString()) || (UomId == item["AltUomId"].ToString()))
                        {
                            if (AltQuantity == item["AltQuantity"].ToString())
                            {
                                Validcheck = "DuplicateUOM";
                                grid_MultiUOM.JSProperties["cpDuplicateAltUOM"] = "DuplicateAltUOM";
                                break;
                            }
                        }
                        // Mantis Issue 24428  [ if "Update Row" checkbox is checked, then all existing Update Row in the grid will be set to False]
                        if (UpdateRow == "True")
                        {
                            item["UpdateRow"] = "False";
                        }
                        // End of Mantis Issue 24428 
                    }
                }

                if (Validcheck != "DuplicateUOM")
                {
                    if (Session["MultiUOMData"] != null)
                    {

                        MultiUOMSaveData = (DataTable)Session["MultiUOMData"];

                    }
                    else
                    {
                        MultiUOMSaveData.Columns.Add("SrlNo", typeof(string));
                        MultiUOMSaveData.Columns.Add("Quantity", typeof(Decimal));
                        MultiUOMSaveData.Columns.Add("UOM", typeof(string));
                        MultiUOMSaveData.Columns.Add("AltUOM", typeof(string));
                        MultiUOMSaveData.Columns.Add("AltQuantity", typeof(Decimal));
                        MultiUOMSaveData.Columns.Add("UomId", typeof(Int64));
                        MultiUOMSaveData.Columns.Add("AltUomId", typeof(Int64));
                        MultiUOMSaveData.Columns.Add("ProductId", typeof(Int64));
                        MultiUOMSaveData.Columns.Add("DetailsId", typeof(Int64));

                        // Mantis Issue 24428
                        MultiUOMSaveData.Columns.Add("BaseRate", typeof(Decimal));
                        MultiUOMSaveData.Columns.Add("AltRate", typeof(Decimal));
                        MultiUOMSaveData.Columns.Add("UpdateRow", typeof(string));
                        // End of Mantis Issue 24428

                        DataColumn myDataColumn = new DataColumn();
                        myDataColumn.AllowDBNull = false;
                        myDataColumn.AutoIncrement = true;
                        myDataColumn.AutoIncrementSeed = 1;
                        myDataColumn.AutoIncrementStep = 1;
                        myDataColumn.ColumnName = "MultiUOMSR";
                        myDataColumn.DataType = System.Type.GetType("System.Int32");
                        myDataColumn.Unique = true;
                        MultiUOMSaveData.Columns.Add(myDataColumn);
                    }

                      DataRow thisRow;
                      if (MultiUOMSaveData.Rows.Count > 0)
                      {
                          // Rev Sanchita
                          //thisRow = (DataRow)MultiUOMSaveData.Rows[MultiUOMSaveData.Rows.Count - 1];
                          //MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow, (Convert.ToInt16(thisRow["MultiUOMSR"]) + 1));
                          MultiUOMSR = Convert.ToInt32(MultiUOMSaveData.Compute("max([MultiUOMSR])", string.Empty)) + 1;
                          MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow, MultiUOMSR);
                          // End of Rev sanchita
                      }
                      else
                      {
                          MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow, MultiUOMSR);
                      }
                      //// Mantis Issue 24428
                    //if (DetailsId != "" && DetailsId != null && DetailsId != "null")
                    //{
                    //    // Mantis Issue 24428
                    //    //MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId);
                    //    MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow);
                    //    // End of Mantis Issue 24428
                    //}
                    //else
                    //{
                    //    // Mantis Issue 24428
                    //   // MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId);
                    //    MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow);
                    //    // End of Mantis Issue 24428
                    //}
                    // End of Mantis Issue 24428
                    MultiUOMSaveData.AcceptChanges();
                    Session["MultiUOMData"] = MultiUOMSaveData;

                    if (MultiUOMSaveData != null && MultiUOMSaveData.Rows.Count > 0)
                    {
                        DataView dvData = new DataView(MultiUOMSaveData);
                        // Rev Sanchita
                        //if (DetailsId != "" && DetailsId != null && DetailsId != "null")
                        //{
                        //    //dvData.RowFilter = "SrlNo = '" + SrlNo + "' and Doc_DetailsId='" + DetailsId + "'";
                        //    dvData.RowFilter = "DetailsId='" + DetailsId + "'";
                        //}
                        //else
                        //{
                        //    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                        //}
                        dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                        // End of Rev Sanchita
                        grid_MultiUOM.DataSource = dvData;
                        grid_MultiUOM.DataBind();
                    }
                    else
                    {
                        //MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId);
                        //Session["MultiUOMData"] = MultiUOMSaveData;
                        grid_MultiUOM.DataSource = MultiUOMSaveData.DefaultView;
                        grid_MultiUOM.DataBind();
                    }
                }
            }

            else if (SpltCmmd == "MultiUomDelete")
            {
                string AltUOMKeyValuewithqnty = e.Parameters.Split('~')[1];
                string AltUOMKeyValue = AltUOMKeyValuewithqnty.Split('|')[0];
                string AltUOMKeyqnty = AltUOMKeyValuewithqnty.Split('|')[1];

                string SrlNo = Convert.ToString(e.Parameters.Split('~')[2]);
                string DetailsId = Convert.ToString(e.Parameters.Split('~')[3]);

                DataRow[] MultiUoMresult;
                DataTable dt = (DataTable)Session["MultiUOMData"];

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Rev Sanchita
                    //if (DetailsId != "" && DetailsId != null && DetailsId != "null")
                    //{
                    //    //MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "' and Doc_DetailsId='" + DetailsId + "'");
                    //    MultiUoMresult = dt.Select("DetailsId='" + DetailsId + "'");
                    //}
                    //else
                    //{
                    //    MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "'");
                    //}
                    MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "'");
                    // End of Rev Sanchita

                    foreach (DataRow item in MultiUoMresult)
                    {
                        if (AltUOMKeyValue.ToString() == item["AltUomId"].ToString())
                        {
                            //dt.Rows.Remove(item);
                            if (AltUOMKeyqnty.ToString() == item["AltQuantity"].ToString())
                            {
                                item.Table.Rows.Remove(item);
                                break;
                            }
                        }
                    }
                }
                Session["MultiUOMData"] = dt;
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(dt);
                    // Rev Sanchita
                    //if (DetailsId != "" && DetailsId != null && DetailsId != "null")
                    //{
                    //    //dvData.RowFilter = "SrlNo = '" + SrlNo + "'and Doc_DetailsId='" + DetailsId + "'";
                    //    dvData.RowFilter = "DetailsId='" + DetailsId + "'";
                    //}
                    //else
                    //{
                    //    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    //}
                    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    // End of Rev Sanchita
                    grid_MultiUOM.DataSource = dvData;
                    grid_MultiUOM.DataBind();
                }
                else
                {
                    grid_MultiUOM.DataSource = null;
                    grid_MultiUOM.DataBind();
                }
            }


            else if (SpltCmmd == "CheckMultiUOmDetailsQuantity")
            {
                string SrlNo = Convert.ToString(e.Parameters.Split('~')[1]);
                DataTable dt = (DataTable)Session["MultiUOMData"];
                string detailsId = Convert.ToString(e.Parameters.Split('~')[2]);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] MultiUoMresult;
                    // Rev Sanchita
                    //if (detailsId != null && detailsId != "" && detailsId != "null")
                    //{
                    //    MultiUoMresult = dt.Select("DetailsId ='" + detailsId + "'");
                    //}
                    //else
                    //{
                    //    MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "'");
                    //}
                    MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "'");
                    // End of Rev Sanchita
                    foreach (DataRow item in MultiUoMresult)
                    {
                        item.Table.Rows.Remove(item);
                    }
                }
                Session["MultiUOMData"] = dt;
            }
            // Mantis Issue 24428
            else if (SpltCmmd == "EditData")
            {
                string AltUOMKeyValuewithqnty = e.Parameters.Split('~')[1];
                string AltUOMKeyValue = e.Parameters.Split('~')[2];
                string AltUOMKeyqnty = AltUOMKeyValuewithqnty.Split('|')[1];

                string SrlNo = Convert.ToString(e.Parameters.Split('~')[1]);
                DataTable dt = (DataTable)Session["MultiUOMData"];

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] MultiUoMresult = dt.Select("MultiUOMSR ='" + AltUOMKeyValue + "'");

                    Decimal BaseQty = Convert.ToDecimal(MultiUoMresult[0]["Quantity"]);
                    Decimal BaseRate = Convert.ToDecimal(MultiUoMresult[0]["BaseRate"]);

                    Decimal AltQty = Convert.ToDecimal(MultiUoMresult[0]["AltQuantity"]);
                    Decimal AltRate = Convert.ToDecimal(MultiUoMresult[0]["AltRate"]);
                    Decimal AltUom = Convert.ToDecimal(MultiUoMresult[0]["AltUomId"]);
                    bool UpdateRow = Convert.ToBoolean(MultiUoMresult[0]["UpdateRow"]);

                    grid_MultiUOM.JSProperties["cpAllDetails"] = "EditData";

                    grid_MultiUOM.JSProperties["cpBaseQty"] = BaseQty;
                    grid_MultiUOM.JSProperties["cpBaseRate"] = BaseRate;


                    grid_MultiUOM.JSProperties["cpAltQty"] = AltQty;
                    grid_MultiUOM.JSProperties["cpAltUom"] = AltUom;
                    grid_MultiUOM.JSProperties["cpAltRate"] = AltRate;
                    grid_MultiUOM.JSProperties["cpUpdatedrow"] = UpdateRow;
                    grid_MultiUOM.JSProperties["cpuomid"] = AltUOMKeyValue;
                }
                Session["MultiUOMData"] = dt;
            }



            else if (SpltCmmd == "UpdateRow")
            {


                string SrlNoR = e.Parameters.Split('~')[1];
                string AltUOMKeyValue = e.Parameters.Split('~')[7];
                string AltUOMKeyqnty = e.Parameters.Split('~')[5];
                string muid = e.Parameters.Split('~')[13];
                // Rev Sanchita
                string SrlNo = "0";
                string Validcheck = "";
                // End of Rev Sanchita
               
                DataTable MultiUOMSaveData = new DataTable();

                DataTable dt = (DataTable)Session["MultiUOMData"];

                // Rev sanchita
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow[] MultiUoMresult = dt.Select("MultiUOMSR ='" + muid + "'");
                    foreach (DataRow item in MultiUoMresult)
                    {
                        // Rev SAnchita
                        SrlNo = Convert.ToString(item["SrlNo"]);
                        // End of Rev Sanchita
                        //item.Table.Rows.Remove(item);
                        //break;

                    }
                }
                // End of rev Sanchita

                // Rev Sanchita
                //string SrlNo = Convert.ToString(e.Parameters.Split('~')[1]);
                // End of Rev Sanchita
                string Quantity = Convert.ToString(e.Parameters.Split('~')[2]);
                string UOM = Convert.ToString(e.Parameters.Split('~')[3]);
                string AltUOM = Convert.ToString(e.Parameters.Split('~')[4]);
                string AltQuantity = Convert.ToString(e.Parameters.Split('~')[5]);
                string UomId = Convert.ToString(e.Parameters.Split('~')[6]);
                string AltUomId = Convert.ToString(e.Parameters.Split('~')[7]);
                string ProductId = Convert.ToString(e.Parameters.Split('~')[8]);
                string DetailsId = Convert.ToString(e.Parameters.Split('~')[9]);


                // Mantis Issue 24428
                string BaseRate = Convert.ToString(e.Parameters.Split('~')[10]);
                string AltRate = Convert.ToString(e.Parameters.Split('~')[11]);
                string UpdateRow = Convert.ToString(e.Parameters.Split('~')[12]);
                // End of Mantis Issue 24428

                // Mantis Issue 24428
               // dt.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, BaseRate, AltRate, UpdateRow);
                // Rev Sanchita
                //dt.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId,DetailsId, BaseRate, AltRate, UpdateRow, muid);

                DataRow[] MultiUoMresultResult = dt.Select("SrlNo ='" + SrlNo + "' and MultiUOMSR <>'" + muid + "'");

                foreach (DataRow item in MultiUoMresultResult)
                {
                    if ((AltUomId == item["AltUomId"].ToString()) || (UomId == item["AltUomId"].ToString()))
                    {
                        if (AltQuantity == item["AltQuantity"].ToString())
                        {
                            Validcheck = "DuplicateUOM";
                            grid_MultiUOM.JSProperties["cpDuplicateAltUOM"] = "DuplicateAltUOM";
                            break;
                        }
                    }
                    // Mantis Issue 24428  [ if "Update Row" checkbox is checked, then all existing Update Row in the grid will be set to False]
                    if (UpdateRow == "True")
                    {
                        item["UpdateRow"] = "False";
                    }
                    // End of Mantis Issue 24428 
                }


                if (Validcheck != "DuplicateUOM")
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow[] MultiUoMresult = dt.Select("MultiUOMSR ='" + muid + "'");
                        foreach (DataRow item in MultiUoMresult)
                        {
                            // Rev SAnchita
                            //SrlNo = Convert.ToString(item["SrlNo"]);
                            //// End of Rev Sanchita
                            item.Table.Rows.Remove(item);
                            break;

                        }
                    }
                    dt.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId, DetailsId, BaseRate, AltRate, UpdateRow, muid);
                }
                //End Rev Sanchita

                // End of Mantis Issue 24428
                Session["MultiUOMData"] = dt;

                MultiUOMSaveData = (DataTable)Session["MultiUOMData"];

                MultiUOMSaveData.AcceptChanges();
                Session["MultiUOMData"] = MultiUOMSaveData;

                if (MultiUOMSaveData != null && MultiUOMSaveData.Rows.Count > 0)
                {
                    DataView dvData = new DataView(MultiUOMSaveData);
                    // dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    // Rev Sanchita
                    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";
                    // End of Rev Sanchita

                    grid_MultiUOM.DataSource = dvData;
                    grid_MultiUOM.DataBind();
                }

                //if (dt != null && dt.Rows.Count > 0)
                //{
                //    DataView dvData = new DataView(dt);
                //    dvData.RowFilter = "SrlNo = '" + SrlNo + "'";

                //    grid_MultiUOM.DataSource = dvData;
                //    grid_MultiUOM.DataBind();
                //}
                //else
                //{
                //    //MultiUOMSaveData.Rows.Add(SrlNo, Quantity, UOM, AltUOM, AltQuantity, UomId, AltUomId, ProductId);
                //    //Session["MultiUOMData"] = MultiUOMSaveData;
                //    grid_MultiUOM.DataSource = dt.DefaultView;
                //    grid_MultiUOM.DataBind();
                //}

            }








            // End of Mantis Issue 24428




            // Mantis Issue 24428
            else if (SpltCmmd == "SetBaseQtyRateInGrid")
            {
                DataTable dt = new DataTable();

                if (Session["MultiUOMData"] != null)
                {
                    string SrlNo = Convert.ToString(e.Parameters.Split('~')[1]);
                    dt = (DataTable)HttpContext.Current.Session["MultiUOMData"];
                    DataRow[] MultiUoMresult = dt.Select("SrlNo ='" + SrlNo + "' and UpdateRow ='True'");

                    Int64 SelNo = Convert.ToInt64(MultiUoMresult[0]["SrlNo"]);
                    Decimal BaseQty = Convert.ToDecimal(MultiUoMresult[0]["Quantity"]);
                    Decimal BaseRate = Convert.ToDecimal(MultiUoMresult[0]["BaseRate"]);

                    Decimal AltQty = Convert.ToDecimal(MultiUoMresult[0]["AltQuantity"]);
                    string AltUom = Convert.ToString(MultiUoMresult[0]["AltUOM"]);

                    grid_MultiUOM.JSProperties["cpSetBaseQtyRateInGrid"] = "1";
                    grid_MultiUOM.JSProperties["cpBaseQty"] = BaseQty;
                    grid_MultiUOM.JSProperties["cpBaseRate"] = BaseRate;


                    grid_MultiUOM.JSProperties["cpAltQty"] = AltQty;
                    grid_MultiUOM.JSProperties["cpAltUom"] = AltUom;


                    //if (Session["OrderDetails"] != null)
                    //{
                    //    DataTable SalesOrderdt = (DataTable)Session["OrderDetails"];

                    //    DataRow[] drSalesOrder = SalesOrderdt.Select("SrlNo ='" + SelNo + "'");
                    //    if (drSalesOrder.Length > 0)
                    //    {
                    //        drSalesOrder[0]["Quantity"] = BaseQty;
                    //        drSalesOrder[0]["SalePrice"] = BaseRate;
                    //    }

                    //}

                }
            }
            // End of Mantis Issue 24428


        }

        protected void GrdWarehouse_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            GrdWarehouse.JSProperties["cpIsSave"] = "";
            string strSplitCommand = e.Parameters.Split('~')[0];
            string Type = "";

            if (strSplitCommand == "Display")
            {
                GetProductType(ref Type);
                string ProductID = Convert.ToString(hdfProductID.Value);
                string SerialID = Convert.ToString(e.Parameters.Split('~')[1]);

                DataTable Warehousedt = new DataTable();
                if (Session["WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["WarehouseData"];
                }
                else
                {
                    Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                    Warehousedt.Columns.Add("SrlNo", typeof(string));
                    Warehousedt.Columns.Add("WarehouseID", typeof(string));
                    Warehousedt.Columns.Add("WarehouseName", typeof(string));
                    Warehousedt.Columns.Add("Quantity", typeof(string));
                    Warehousedt.Columns.Add("BatchID", typeof(string));
                    Warehousedt.Columns.Add("BatchNo", typeof(string));
                    Warehousedt.Columns.Add("SerialID", typeof(string));
                    Warehousedt.Columns.Add("SerialNo", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMName", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMCode", typeof(string));
                    Warehousedt.Columns.Add("SalesQuantity", typeof(string));
                    Warehousedt.Columns.Add("StkUOMName", typeof(string));
                    Warehousedt.Columns.Add("StkUOMCode", typeof(string));
                    Warehousedt.Columns.Add("StkQuantity", typeof(string));
                    Warehousedt.Columns.Add("ConversionMultiplier", typeof(string));
                    Warehousedt.Columns.Add("AvailableQty", typeof(string));
                    Warehousedt.Columns.Add("BalancrStk", typeof(string));
                    Warehousedt.Columns.Add("MfgDate", typeof(string));
                    Warehousedt.Columns.Add("ExpiryDate", typeof(string));
                    Warehousedt.Columns.Add("LoopID", typeof(string));
                    Warehousedt.Columns.Add("TotalQuantity", typeof(string));
                    Warehousedt.Columns.Add("Status", typeof(string));
                }

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(Warehousedt);
                    dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";

                    GrdWarehouse.DataSource = dvData;
                    GrdWarehouse.DataBind();
                }
                else
                {
                    GrdWarehouse.DataSource = Warehousedt.DefaultView;
                    GrdWarehouse.DataBind();
                }
                changeGridOrder();
            }
            else if (strSplitCommand == "SaveDisplay")
            {
                int loopId = Convert.ToInt32(Session["LoopWarehouse"]);

                string WarehouseID = Convert.ToString(e.Parameters.Split('~')[1]);
                string WarehouseName = Convert.ToString(e.Parameters.Split('~')[2]);
                string BatchID = Convert.ToString(e.Parameters.Split('~')[3]);
                string BatchName = Convert.ToString(e.Parameters.Split('~')[4]);
                string SerialID = Convert.ToString(e.Parameters.Split('~')[5]);
                string SerialName = Convert.ToString(e.Parameters.Split('~')[6]);
                string ProductID = Convert.ToString(hdfProductID.Value);
                string ProductSerialID = Convert.ToString(hdfProductSerialID.Value);
                string Qty = Convert.ToString(e.Parameters.Split('~')[7]);
                string editWarehouseID = Convert.ToString(e.Parameters.Split('~')[8]);

                string Sales_UOM_Name = "", Sales_UOM_Code = "", Stk_UOM_Name = "", Stk_UOM_Code = "", Conversion_Multiplier = "", Trans_Stock = "0", MfgDate = "", ExpiryDate = "";
                GetProductType(ref Type);

                DataTable Warehousedt = new DataTable();
                if (Session["WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["WarehouseData"];
                }
                else
                {
                    Warehousedt.Columns.Add("Product_SrlNo", typeof(string));
                    Warehousedt.Columns.Add("SrlNo", typeof(string));
                    Warehousedt.Columns.Add("WarehouseID", typeof(string));
                    Warehousedt.Columns.Add("WarehouseName", typeof(string));
                    Warehousedt.Columns.Add("Quantity", typeof(string));
                    Warehousedt.Columns.Add("BatchID", typeof(string));
                    Warehousedt.Columns.Add("BatchNo", typeof(string));
                    Warehousedt.Columns.Add("SerialID", typeof(string));
                    Warehousedt.Columns.Add("SerialNo", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMName", typeof(string));
                    Warehousedt.Columns.Add("SalesUOMCode", typeof(string));
                    Warehousedt.Columns.Add("SalesQuantity", typeof(string));
                    Warehousedt.Columns.Add("StkUOMName", typeof(string));
                    Warehousedt.Columns.Add("StkUOMCode", typeof(string));
                    Warehousedt.Columns.Add("StkQuantity", typeof(string));
                    Warehousedt.Columns.Add("ConversionMultiplier", typeof(string));
                    Warehousedt.Columns.Add("AvailableQty", typeof(string));
                    Warehousedt.Columns.Add("BalancrStk", typeof(string));
                    Warehousedt.Columns.Add("MfgDate", typeof(string));
                    Warehousedt.Columns.Add("ExpiryDate", typeof(string));
                    Warehousedt.Columns.Add("LoopID", typeof(string));
                    Warehousedt.Columns.Add("TotalQuantity", typeof(string));
                    Warehousedt.Columns.Add("Status", typeof(string));
                }

                bool IsDelete = false;

                if (Type == "WBS")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];


                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, strLoopID, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", strLoopID, SerialIDList.Length, repute);
                            }
                        }
                    }
                }
                else if (Type == "W")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    BatchID = "0";

                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");

                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");

                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "WB")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);

                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");
                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("WarehouseID ='" + WarehouseID + "' AND BatchID='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "B")
                {
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    WarehouseID = "0";


                    if (editWarehouseID == "0")
                    {
                        string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                        var updaterows = Warehousedt.Select("BatchID ='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");

                        if (updaterows.Length == 0)
                        {
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                        }
                        else
                        {
                            foreach (var row in updaterows)
                            {
                                decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);
                                row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                            }
                        }
                    }
                    else
                    {
                        var rows = Warehousedt.Select("BatchID='" + BatchID + "' AND Convert(TotalQuantity, 'System.Decimal')='" + Qty + "' AND SrlNo='" + editWarehouseID + "'");
                        if (rows.Length == 0)
                        {
                            string whID = "";
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                            var updaterows = Warehousedt.Select("BatchID ='" + BatchID + "' AND Product_SrlNo='" + ProductSerialID + "'");
                            foreach (var row in updaterows)
                            {
                                whID = Convert.ToString(row["SrlNo"]);
                            }

                            if (updaterows.Length == 0)
                            {
                                IsDelete = true;
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, Qty, BatchID, BatchName, "0", "", Sales_UOM_Name, Sales_UOM_Code, Convert.ToDecimal(Qty) + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, Qty, "D");
                            }
                            else if (editWarehouseID == whID)
                            {
                                foreach (var row in updaterows)
                                {
                                    whID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = Qty;
                                    row["TotalQuantity"] = Qty;
                                    row["SalesQuantity"] = Qty + " " + Sales_UOM_Name;
                                }
                            }
                            else if (editWarehouseID != whID)
                            {
                                IsDelete = true;
                                foreach (var row in updaterows)
                                {
                                    ID = Convert.ToString(row["SrlNo"]);
                                    decimal oldQuantity = Convert.ToDecimal(row["Quantity"]);

                                    row["Quantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["TotalQuantity"] = (oldQuantity + Convert.ToDecimal(Qty));
                                    row["SalesQuantity"] = (oldQuantity + Convert.ToDecimal(Qty)) + " " + Sales_UOM_Name;
                                }
                            }
                        }
                    }
                }
                else if (Type == "S")
                {
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    //Qty = Convert.ToString(SerialIDList.Length);
                    Qty = "1";
                    decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                    string stkqtn = Convert.ToString(Math.Round((Convert.ToDecimal(Qty) * ConversionMultiplier), 2));
                    decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                    WarehouseID = "0";
                    BatchID = "0";

                    for (int i = 0; i < SerialIDList.Length; i++)
                    {
                        string strSrlID = SerialIDList[i];
                        string strSrlName = SerialNameList[i];

                        if (editWarehouseID == "0")
                        {
                            string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                            Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                        }
                        else
                        {
                            var rows = Warehousedt.Select("SerialID ='" + strSrlID + "' AND SrlNo='" + editWarehouseID + "'");
                            if (rows.Length == 0)
                            {
                                IsDelete = true;
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                }
                else if (Type == "WS")
                {
                    GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    //GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];


                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, "0", BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", "0", "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, "0", BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", "0", "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, repute);
                            }
                        }
                    }
                }
                else if (Type == "BS")
                {
                    // GetTotalStock(ref Trans_Stock, WarehouseID);
                    GetProductUOM(ref Sales_UOM_Name, ref Sales_UOM_Code, ref Stk_UOM_Name, ref Stk_UOM_Code, ref Conversion_Multiplier, ProductID);
                    GetBatchDetails(ref MfgDate, ref ExpiryDate, BatchID);

                    string[] SerialIDList = SerialID.Split(new string[] { "||@||" }, StringSplitOptions.None);
                    string[] SerialNameList = SerialName.Split(new string[] { "||@||" }, StringSplitOptions.None);

                    if (editWarehouseID == "0")
                    {
                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            WarehouseID = "0";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, "D");
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, "D");
                            }
                        }
                    }
                    else
                    {
                        string strLoopID = "0", strSerial = "0";

                        DataRow[] result = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                        foreach (DataRow row in result)
                        {
                            strSerial = Convert.ToString(row["SerialID"]);
                            strLoopID = Convert.ToString(row["LoopID"]);
                        }

                        int count = 0;
                        DataRow[] dr = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        int value = (dr.Length + SerialIDList.Length - 1);

                        string[] temp_SerialIDList = new string[value];
                        string[] temp_SerialNameList = new string[value];

                        string[] check_SerialIDList = new string[value];
                        string[] check_SerialNameList = new string[value];

                        foreach (DataRow rows in dr)
                        {
                            if (strSerial != Convert.ToString(rows["SerialID"]))
                            {
                                temp_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                temp_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                check_SerialIDList[count] = Convert.ToString(rows["SerialID"]);
                                check_SerialNameList[count] = Convert.ToString(rows["SerialNo"]);

                                count++;
                            }
                        }

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            temp_SerialIDList[count] = Convert.ToString(SerialIDList[i]);
                            temp_SerialNameList[count] = Convert.ToString(SerialNameList[i]);

                            count++;
                        }

                        DataRow[] delResult = Warehousedt.Select("LoopID ='" + strLoopID + "'");
                        foreach (DataRow delrow in delResult)
                        {
                            delrow.Delete();
                        }
                        Warehousedt.AcceptChanges();

                        SerialIDList = temp_SerialIDList;
                        SerialNameList = temp_SerialNameList;

                        for (int i = 0; i < SerialIDList.Length; i++)
                        {
                            string strSrlID = SerialIDList[i];
                            string strSrlName = SerialNameList[i];
                            WarehouseID = "0";
                            string repute = "D";
                            if (check_SerialIDList.Contains(strSrlID)) repute = "I";

                            if (i == 0)
                            {
                                decimal ConversionMultiplier = Convert.ToDecimal(Conversion_Multiplier);
                                string stkqtn = Convert.ToString(Math.Round((SerialIDList.Length * ConversionMultiplier), 2));
                                decimal BalanceStk = Convert.ToDecimal(Trans_Stock) - Convert.ToDecimal(stkqtn);
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";

                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, WarehouseName, SerialIDList.Length, BatchID, BatchName, strSrlID, strSrlName, Sales_UOM_Name, Sales_UOM_Code, SerialIDList.Length + " " + Sales_UOM_Name, Stk_UOM_Name, Stk_UOM_Code, stkqtn + " " + Stk_UOM_Name, Conversion_Multiplier, Convert.ToString(Math.Round(Convert.ToDecimal(Trans_Stock))) + " " + Stk_UOM_Name, Convert.ToString(Math.Round(BalanceStk, 2)) + " " + Stk_UOM_Name, MfgDate, ExpiryDate, loopId, SerialIDList.Length, repute);
                            }
                            else
                            {
                                string maxID = (Convert.ToString(Warehousedt.Compute("MAX([SrlNo])", "")) != "") ? Convert.ToString(Convert.ToInt32(Warehousedt.Compute("MAX([SrlNo])", "")) + 1) : "1";
                                Warehousedt.Rows.Add(ProductSerialID, maxID, WarehouseID, "", "", BatchID, "", strSrlID, strSrlName, "", Sales_UOM_Code, "", "", Stk_UOM_Code, "", "", "", "", "", "", loopId, SerialIDList.Length, repute);
                            }
                        }
                    }
                }

                //Warehousedt.Rows.Add(Warehousedt.Rows.Count + 1, WarehouseID, WarehouseName, "1", BatchID, BatchName, SerialID, SerialName);
                //string sortExpression = string.Format("{0} {1}", colName, direction);
                //dt.DefaultView.Sort = sortExpression;
                //Warehousedt.DefaultView.Sort = "SrlNo Asc";
                //Warehousedt = Warehousedt.DefaultView.ToTable(true);

                if (IsDelete == true)
                {
                    DataRow[] delResult = Warehousedt.Select("SrlNo ='" + editWarehouseID + "'");
                    foreach (DataRow delrow in delResult)
                    {
                        delrow.Delete();
                    }
                    Warehousedt.AcceptChanges();
                }

                Session["WarehouseData"] = Warehousedt;
                changeGridOrder();

                GrdWarehouse.DataSource = Warehousedt.DefaultView;
                GrdWarehouse.DataBind();

                Session["LoopWarehouse"] = loopId + 1;

                CmbWarehouse.SelectedIndex = -1;
                CmbBatch.SelectedIndex = -1;
            }
            else if (strSplitCommand == "Delete")
            {
                string strKey = e.Parameters.Split('~')[1];
                string strLoopID = "", strPreLoopID = "";

                DataTable Warehousedt = new DataTable();
                if (Session["WarehouseData"] != null)
                {
                    Warehousedt = (DataTable)Session["WarehouseData"];
                }

                DataRow[] result = Warehousedt.Select("SrlNo ='" + strKey + "'");
                foreach (DataRow row in result)
                {
                    strLoopID = row["LoopID"].ToString();
                }

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    int count = 0;
                    bool IsFirst = false, IsAssign = false;
                    string WarehouseName = "", Quantity = "", BatchNo = "", SalesUOMName = "", SalesQuantity = "", StkUOMName = "", StkQuantity = "", ConversionMultiplier = "", AvailableQty = "", BalancrStk = "", MfgDate = "", ExpiryDate = "";


                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delSrlID = Convert.ToString(dr["SrlNo"]);
                        string delLoopID = Convert.ToString(dr["LoopID"]);

                        if (strPreLoopID != delLoopID)
                        {
                            count = 0;
                        }

                        if ((delLoopID == strLoopID) && (strKey == delSrlID) && count == 0)
                        {
                            IsFirst = true;

                            WarehouseName = Convert.ToString(dr["WarehouseName"]);
                            Quantity = Convert.ToString(dr["Quantity"]);
                            BatchNo = Convert.ToString(dr["BatchNo"]);
                            SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                            SalesQuantity = Convert.ToString(dr["SalesQuantity"]);
                            StkUOMName = Convert.ToString(dr["StkUOMName"]);
                            StkQuantity = Convert.ToString(dr["StkQuantity"]);
                            ConversionMultiplier = Convert.ToString(dr["ConversionMultiplier"]);
                            AvailableQty = Convert.ToString(dr["AvailableQty"]);
                            BalancrStk = Convert.ToString(dr["BalancrStk"]);
                            MfgDate = Convert.ToString(dr["MfgDate"]);
                            ExpiryDate = Convert.ToString(dr["ExpiryDate"]);

                            //dr.Delete();
                        }
                        else
                        {
                            if (delLoopID == strLoopID)
                            {
                                if (strKey == delSrlID)
                                {
                                    //dr.Delete();
                                }
                                else
                                {
                                    decimal S_Quantity = Convert.ToDecimal(dr["TotalQuantity"]);
                                    dr["Quantity"] = S_Quantity - 1;
                                    dr["TotalQuantity"] = S_Quantity - 1;

                                    if (IsFirst == true && IsAssign == false)
                                    {
                                        IsAssign = true;

                                        dr["WarehouseName"] = WarehouseName;
                                        dr["BatchNo"] = BatchNo;
                                        dr["SalesUOMName"] = SalesUOMName;
                                        dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                        dr["StkUOMName"] = StkUOMName;
                                        dr["StkQuantity"] = StkQuantity;
                                        dr["ConversionMultiplier"] = ConversionMultiplier;
                                        dr["AvailableQty"] = AvailableQty;
                                        dr["BalancrStk"] = BalancrStk;
                                        dr["MfgDate"] = MfgDate;
                                        dr["ExpiryDate"] = ExpiryDate;
                                    }
                                    else
                                    {
                                        if (IsAssign == false)
                                        {
                                            IsAssign = true;
                                            SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                                            dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                        }
                                    }
                                }
                            }
                        }

                        strPreLoopID = delLoopID;
                        count++;
                    }
                    Warehousedt.AcceptChanges();


                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delSrlID = Convert.ToString(dr["SrlNo"]);
                        if (strKey == delSrlID)
                        {
                            dr.Delete();
                        }
                    }
                    Warehousedt.AcceptChanges();
                }

                Session["WarehouseData"] = Warehousedt;
                GrdWarehouse.DataSource = Warehousedt.DefaultView;
                GrdWarehouse.DataBind();
            }
            else if (strSplitCommand == "WarehouseDelete")
            {
                string ProductID = Convert.ToString(hdfProductSerialID.Value);
                DeleteUnsaveWarehouse(ProductID);
            }
            else if (strSplitCommand == "WarehouseFinal")
            {
                if (Session["WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["WarehouseData"];
                    string ProductID = Convert.ToString(hdfProductSerialID.Value);
                    string strPreLoopID = "";
                    decimal sum = 0;

                    for (int i = 0; i < Warehousedt.Rows.Count; i++)
                    {
                        DataRow dr = Warehousedt.Rows[i];
                        string delLoopID = Convert.ToString(dr["LoopID"]);
                        string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);

                        if (ProductID == Product_SrlNo)
                        {
                            string strQuantity = (Convert.ToString(dr["SalesQuantity"]) != "") ? Convert.ToString(dr["SalesQuantity"]) : "0";
                            var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);
                            //string resultString = Regex.Match(strQuantity, @"[^0-9\.]+").Value;

                            sum = sum + Convert.ToDecimal(weight);
                        }
                    }

                    if (Convert.ToDecimal(sum) == Convert.ToDecimal(hdnProductQuantity.Value))
                    {
                        GrdWarehouse.JSProperties["cpIsSave"] = "Y";
                        for (int i = 0; i < Warehousedt.Rows.Count; i++)
                        {
                            DataRow dr = Warehousedt.Rows[i];
                            string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);
                            if (ProductID == Product_SrlNo)
                            {
                                dr["Status"] = "I";
                            }
                        }
                        Warehousedt.AcceptChanges();
                    }
                    else
                    {
                        GrdWarehouse.JSProperties["cpIsSave"] = "N";
                    }

                    Session["WarehouseData"] = Warehousedt;
                }
            }
        }
        protected void CallbackPanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string performpara = e.Parameter;
            if (performpara.Split('~')[0] == "EditWarehouse")
            {
                string SrlNo = performpara.Split('~')[1];
                string ProductType = Convert.ToString(hdfProductType.Value);

                if (Session["WarehouseData"] != null)
                {
                    DataTable Warehousedt = (DataTable)Session["WarehouseData"];

                    string strWarehouse = "", strBatchID = "", strSrlID = "", strQuantity = "0";
                    var rows = Warehousedt.Select(string.Format("SrlNo ='{0}'", SrlNo));
                    foreach (var dr in rows)
                    {
                        strWarehouse = (Convert.ToString(dr["WarehouseID"]) != "") ? Convert.ToString(dr["WarehouseID"]) : "0";
                        strBatchID = (Convert.ToString(dr["BatchID"]) != "") ? Convert.ToString(dr["BatchID"]) : "0";
                        strSrlID = (Convert.ToString(dr["SerialID"]) != "") ? Convert.ToString(dr["SerialID"]) : "0";
                        strQuantity = (Convert.ToString(dr["TotalQuantity"]) != "") ? Convert.ToString(dr["TotalQuantity"]) : "0";
                    }

                    //CmbWarehouse.DataSource = GetWarehouseData();
                    CmbBatch.DataSource = GetBatchData(strWarehouse);
                    CmbBatch.DataBind();

                    CallbackPanel.JSProperties["cpEdit"] = strWarehouse + "~" + strBatchID + "~" + strSrlID + "~" + strQuantity;
                }
            }
        }
        public void changeGridOrder()
        {
            string Type = "";
            GetProductType(ref Type);
            if (Type == "W")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "WB")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "WBS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "B")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = false;
            }
            else if (Type == "S")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "WS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = true;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = false;
                GrdWarehouse.Columns["MfgDate"].Visible = false;
                GrdWarehouse.Columns["ExpiryDate"].Visible = false;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
            else if (Type == "BS")
            {
                GrdWarehouse.Columns["WarehouseName"].Visible = false;
                GrdWarehouse.Columns["AvailableQty"].Visible = false;
                GrdWarehouse.Columns["BatchNo"].Visible = true;
                GrdWarehouse.Columns["MfgDate"].Visible = true;
                GrdWarehouse.Columns["ExpiryDate"].Visible = true;
                GrdWarehouse.Columns["SerialNo"].Visible = true;
            }
        }
        public void GetTotalStock(ref string Trans_Stock, string WarehouseID)
        {
            string ProductID = Convert.ToString(hdfProductID.Value);

            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetBatchDetails");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(ProductID));
            proc.AddVarcharPara("@WarehouseID", 100, Convert.ToString(WarehouseID));
            proc.AddVarcharPara("@FinYear", 500, Convert.ToString(Session["LastFinYear"]));
            proc.AddVarcharPara("@branchId", 500, Convert.ToString(Session["userbranchID"]));
            proc.AddVarcharPara("@companyId", 500, Convert.ToString(Session["LastCompany"]));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Trans_Stock = Convert.ToString(dt.Rows[0]["Trans_Stock"]);
            }
        }
        public void GetProductUOM(ref string Sales_UOM_Name, ref string Sales_UOM_Code, ref string Stk_UOM_Name, ref string Stk_UOM_Code, ref string Conversion_Multiplier, string ProductID)
        {
            DataTable Productdt = GetProductDetailsData(ProductID);
            if (Productdt != null && Productdt.Rows.Count > 0)
            {
                Sales_UOM_Name = Convert.ToString(Productdt.Rows[0]["Sales_UOM_Name"]);
                Sales_UOM_Code = Convert.ToString(Productdt.Rows[0]["Sales_UOM_Code"]);
                Stk_UOM_Name = Convert.ToString(Productdt.Rows[0]["Stk_UOM_Name"]);
                Stk_UOM_Code = Convert.ToString(Productdt.Rows[0]["Stk_UOM_Code"]);
                Conversion_Multiplier = Convert.ToString(Productdt.Rows[0]["Conversion_Multiplier"]);
            }
        }
        public void GetBatchDetails(ref string MfgDate, ref string ExpiryDate, string BatchID)
        {
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetBatchDetails");
            proc.AddVarcharPara("@BatchID", 100, Convert.ToString(BatchID));
            DataTable Batchdt = proc.GetTable();

            if (Batchdt != null && Batchdt.Rows.Count > 0)
            {
                MfgDate = Convert.ToString(Batchdt.Rows[0]["MfgDate"]);
                ExpiryDate = Convert.ToString(Batchdt.Rows[0]["ExpiryDate"]);
            }
        }
        public void GetProductType(ref string Type)
        {
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetSchemeType");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(hdfProductID.Value));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Type = Convert.ToString(dt.Rows[0]["Type"]);
            }
        }

        protected void MultiUOM_DataBinding(object sender, EventArgs e)
        {
            //DataTable dt = (DataTable)Session["MultiUOMData"];
            //if(dt !=null && dt.Rows.Count >0 )
            //{
            //    DataView dvData = new DataView(dt);
            //    //dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";

            //    grid_MultiUOM.DataSource = dvData;
            //    grid_MultiUOM.DataBind();
            //}
            //else
            //{
            //    grid_MultiUOM.DataSource = null;
            //    grid_MultiUOM.DataBind();
            //}
        }
        protected void GrdWarehouse_DataBinding(object sender, EventArgs e)
        {
            if (Session["WarehouseData"] != null)
            {
                string Type = "";
                GetProductType(ref Type);
                string SerialID = Convert.ToString(hdfProductSerialID.Value);
                DataTable Warehousedt = (DataTable)Session["WarehouseData"];

                if (Warehousedt != null && Warehousedt.Rows.Count > 0)
                {
                    DataView dvData = new DataView(Warehousedt);
                    dvData.RowFilter = "Product_SrlNo = '" + SerialID + "'";

                    GrdWarehouse.DataSource = dvData;
                }
                else
                {
                    GrdWarehouse.DataSource = Warehousedt.DefaultView;
                }
            }
        }
        [WebMethod]
        public static string getSchemeType(string Products_ID)
        {
            string Type = "";
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "GetSchemeType");
            proc.AddVarcharPara("@ProductID", 100, Convert.ToString(Products_ID));
            DataTable dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                Type = Convert.ToString(dt.Rows[0]["Type"]);
            }

            return Convert.ToString(Type);
        }
        //Rev Rajdip For Close Quotation Quantity
        [WebMethod]
        public static Int32 CloseQuotationquantity(string Quote_Id, string Remarks)
        {
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            Int32 strIsComplete = 0;
            SqlCommand cmd = new SqlCommand("prc_Closequotationquantity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "CloseQuantity");
            cmd.Parameters.AddWithValue("@Quote_Id", Quote_Id);
            cmd.Parameters.AddWithValue("@Remarks", Remarks);
            cmd.Parameters.Add("@ReturnValue", SqlDbType.VarChar, 50);
            cmd.Parameters["@ReturnValue"].Direction = ParameterDirection.Output;
            cmd.CommandTimeout = 0;
            SqlDataAdapter Adap = new SqlDataAdapter();
            DataSet dsInst = new DataSet();
            Adap.SelectCommand = cmd;
            Adap.Fill(dsInst);
            strIsComplete = Convert.ToInt32(cmd.Parameters["@ReturnValue"].Value.ToString());
            return strIsComplete;
        }
        //End Rev Rajdip For Close Quotation
        [WebMethod]
        public static Int32 GetQuantityfromSL(string SLNo, string val)
        {

            DataTable dt = new DataTable();
            int SLVal = 0;
            if (HttpContext.Current.Session["MultiUOMData"] != null)
            {
                DataRow[] MultiUoMresult;
                dt = (DataTable)HttpContext.Current.Session["MultiUOMData"];
                if (val == "1")
                {
                    // Mantis Issue 24428
                   // MultiUoMresult = dt.Select("DetailsId ='" + SLNo + "'");
                    MultiUoMresult = dt.Select("DetailsId ='" + SLNo + "' and UpdateRow ='True'");

                    // End of Mantis Issue 24428

                }
                else
                {      // Mantis Issue 24428
                    //MultiUoMresult = dt.Select("SrlNo ='" + SLNo + "'");
                      MultiUoMresult = dt.Select("SrlNo ='" + SLNo + "' and UpdateRow ='True'");
                    // End of Mantis Issue 24428
                }
                SLVal = MultiUoMresult.Length;


            }

            return SLVal;
        }

        [WebMethod]
        public static object AutoPopulateAltQuantity(Int64 ProductID)
        {
            List<MultiUOMPacking> RateLists = new List<MultiUOMPacking>();

            decimal packing_quantity = 0;
            decimal sProduct_quantity = 0;
            Int32 AltUOMId = 0;
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "AutoPopulateAltQuantityDetails");
            proc.AddBigIntegerPara("@PackingProductId", ProductID);
            DataTable dt = proc.GetTable();
            RateLists = DbHelpers.ToModelList<MultiUOMPacking>(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                packing_quantity = Convert.ToDecimal(dt.Rows[0]["packing_quantity"]);
                sProduct_quantity = Convert.ToDecimal(dt.Rows[0]["sProduct_quantity"]);
                AltUOMId = Convert.ToInt32(dt.Rows[0]["AltUOMId"]);
            }
            //return packing_quantity + '~' + sProduct_quantity;
            return RateLists;
        }


        [WebMethod]
        public static object GetPackingQuantity(Int32 UomId, Int32 AltUomId, Int64 ProductID)
        {
            List<MultiUOMPacking> RateLists = new List<MultiUOMPacking>();

            decimal packing_quantity = 0;
            decimal sProduct_quantity = 0;
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "PackingQuantityDetails");
            proc.AddIntegerPara("@UomId", UomId);
            proc.AddIntegerPara("@AltUomId", AltUomId);
            proc.AddBigIntegerPara("@PackingProductId", ProductID);
            DataTable dt = proc.GetTable();
            RateLists = DbHelpers.ToModelList<MultiUOMPacking>(dt);
            if (dt != null && dt.Rows.Count > 0)
            {
                packing_quantity = Convert.ToDecimal(dt.Rows[0]["packing_quantity"]);
                sProduct_quantity = Convert.ToDecimal(dt.Rows[0]["sProduct_quantity"]);
            }
            //return packing_quantity + '~' + sProduct_quantity;
            return RateLists;
        }

        //  Rev 1.0
        [WebMethod]
        public static String GetRFQHeaderReference(string KeyVal, string type)
        {
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            string strSalesmanId = "";
            string strSalesmanName = "";
            string ResultString = "";

            DataTable dt_Head = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("Prc_GetQuotationDetails");
            proc.AddVarcharPara("@Quote_Number", 100, KeyVal);
            proc.AddVarcharPara("@Mode", 100, "GetInquiryDate");
            dt_Head = proc.GetTable();

            if (dt_Head != null && dt_Head.Rows.Count > 0)
            {
                strSalesmanId = Convert.ToString(dt_Head.Rows[0]["SalesmanId"]);
                strSalesmanName = Convert.ToString(dt_Head.Rows[0]["SalesmanName"]);


                ResultString = Convert.ToString(strSalesmanId + "~" + strSalesmanName);
            }

            return ResultString;
        }
        // End of Rev 1.0

        public void GetQuantityBaseOnProduct(string strProductSrlNo, ref decimal WarehouseQty)
        {
            decimal sum = 0;

            DataTable Warehousedt = new DataTable();
            if (Session["WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["WarehouseData"];
                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);

                    if (strProductSrlNo == Product_SrlNo)
                    {
                        string strQuantity = (Convert.ToString(dr["SalesQuantity"]) != "") ? Convert.ToString(dr["SalesQuantity"]) : "0";
                        var weight = Decimal.Parse(Regex.Match(strQuantity, "[0-9]*\\.*[0-9]*").Value);

                        sum = sum + Convert.ToDecimal(weight);
                    }
                }
            }

            WarehouseQty = sum;
        }
        public void DeleteWarehouse(string SrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["WarehouseData"];

                var rows = Warehousedt.Select(string.Format("Product_SrlNo ='{0}'", SrlNo));
                foreach (var row in rows)
                {
                    row.Delete();
                }
                Warehousedt.AcceptChanges();

                Session["WarehouseData"] = Warehousedt;
            }
        }

        public void DeleteMultiUOMDetails(string SrlNo)
        {
            DataTable MultiUOMData = new DataTable();
            if (Session["MultiUOMData"] != null)
            {
                MultiUOMData = (DataTable)Session["MultiUOMData"];

                var rows = MultiUOMData.Select(string.Format("SrlNo ='{0}'", SrlNo));
                foreach (var row in rows)
                {
                    row.Delete();
                }
                MultiUOMData.AcceptChanges();

                Session["MultiUOMData"] = MultiUOMData;
            }
        }

        public void UpdateMultiUOMDetails(string oldSrlNo, string newSrlNo)
        {
            DataTable MultiUOMData = new DataTable();
            if (Session["MultiUOMData"] != null)
            {
                MultiUOMData = (DataTable)Session["MultiUOMData"];

                for (int i = 0; i < MultiUOMData.Rows.Count; i++)
                {
                    DataRow dr = MultiUOMData.Rows[i];
                    string SrlNo = Convert.ToString(dr["SrlNo"]);
                    if (oldSrlNo == SrlNo)
                    {
                        dr["SrlNo"] = newSrlNo;
                    }
                }
                MultiUOMData.AcceptChanges();

                Session["MultiUOMData"] = MultiUOMData;
            }
        }

        public void DeleteUnsaveWarehouse(string SrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["WarehouseData"];

                var rows = Warehousedt.Select("Product_SrlNo ='" + SrlNo + "' AND Status='D'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                Warehousedt.AcceptChanges();

                Session["WarehouseData"] = Warehousedt;
            }
        }
        public DataTable DeleteWarehouseBySrl(string strKey)
        {
            string strLoopID = "", strPreLoopID = "";

            DataTable Warehousedt = new DataTable();
            if (Session["WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["WarehouseData"];
            }

            DataRow[] result = Warehousedt.Select("SrlNo ='" + strKey + "'");
            foreach (DataRow row in result)
            {
                strLoopID = row["LoopID"].ToString();
            }

            if (Warehousedt != null && Warehousedt.Rows.Count > 0)
            {
                int count = 0;
                bool IsFirst = false, IsAssign = false;
                string WarehouseName = "", Quantity = "", BatchNo = "", SalesUOMName = "", SalesQuantity = "", StkUOMName = "", StkQuantity = "", ConversionMultiplier = "", AvailableQty = "", BalancrStk = "", MfgDate = "", ExpiryDate = "";


                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string delSrlID = Convert.ToString(dr["SrlNo"]);
                    string delLoopID = Convert.ToString(dr["LoopID"]);

                    if (strPreLoopID != delLoopID)
                    {
                        count = 0;
                    }

                    if ((delLoopID == strLoopID) && (strKey == delSrlID) && count == 0)
                    {
                        IsFirst = true;

                        WarehouseName = Convert.ToString(dr["WarehouseName"]);
                        Quantity = Convert.ToString(dr["Quantity"]);
                        BatchNo = Convert.ToString(dr["BatchNo"]);
                        SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                        SalesQuantity = Convert.ToString(dr["SalesQuantity"]);
                        StkUOMName = Convert.ToString(dr["StkUOMName"]);
                        StkQuantity = Convert.ToString(dr["StkQuantity"]);
                        ConversionMultiplier = Convert.ToString(dr["ConversionMultiplier"]);
                        AvailableQty = Convert.ToString(dr["AvailableQty"]);
                        BalancrStk = Convert.ToString(dr["BalancrStk"]);
                        MfgDate = Convert.ToString(dr["MfgDate"]);
                        ExpiryDate = Convert.ToString(dr["ExpiryDate"]);

                        //dr.Delete();
                    }
                    else
                    {
                        if (delLoopID == strLoopID)
                        {
                            if (strKey == delSrlID)
                            {
                                //dr.Delete();
                            }
                            else
                            {
                                int S_Quantity = Convert.ToInt32(dr["TotalQuantity"]);
                                dr["Quantity"] = S_Quantity - 1;
                                dr["TotalQuantity"] = S_Quantity - 1;

                                if (IsFirst == true && IsAssign == false)
                                {
                                    IsAssign = true;

                                    dr["WarehouseName"] = WarehouseName;
                                    dr["BatchNo"] = BatchNo;
                                    dr["SalesUOMName"] = SalesUOMName;
                                    dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                    dr["StkUOMName"] = StkUOMName;
                                    dr["StkQuantity"] = StkQuantity;
                                    dr["ConversionMultiplier"] = ConversionMultiplier;
                                    dr["AvailableQty"] = AvailableQty;
                                    dr["BalancrStk"] = BalancrStk;
                                    dr["MfgDate"] = MfgDate;
                                    dr["ExpiryDate"] = ExpiryDate;
                                }
                                else
                                {
                                    if (IsAssign == false)
                                    {
                                        IsAssign = true;
                                        SalesUOMName = Convert.ToString(dr["SalesUOMName"]);
                                        dr["SalesQuantity"] = (S_Quantity - 1) + " " + SalesUOMName;//SalesQuantity;
                                    }
                                }
                            }
                        }
                    }

                    strPreLoopID = delLoopID;
                    count++;
                }
                Warehousedt.AcceptChanges();


                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string delSrlID = Convert.ToString(dr["SrlNo"]);
                    if (strKey == delSrlID)
                    {
                        dr.Delete();
                    }
                }
                Warehousedt.AcceptChanges();
            }

            return Warehousedt;
        }
        public void UpdateWarehouse(string oldSrlNo, string newSrlNo)
        {
            DataTable Warehousedt = new DataTable();
            if (Session["WarehouseData"] != null)
            {
                Warehousedt = (DataTable)Session["WarehouseData"];

                for (int i = 0; i < Warehousedt.Rows.Count; i++)
                {
                    DataRow dr = Warehousedt.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["Product_SrlNo"]);
                    if (oldSrlNo == Product_SrlNo)
                    {
                        dr["Product_SrlNo"] = newSrlNo;
                    }
                }
                Warehousedt.AcceptChanges();

                Session["WarehouseData"] = Warehousedt;
            }
        }
        protected void acpAvailableStock_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string performpara = e.Parameter;
            string strProductID = Convert.ToString(performpara.Split('~')[0]);
            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            acpAvailableStock.JSProperties["cpstock"] = "0.00";

            try
            {
                DataTable dt2 = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableQuotation(" + strBranch + ",'" + Convert.ToString(Session["LastCompany"]) + "','" + Convert.ToString(Session["LastFinYear"]) + "'," + strProductID + ") as branchopenstock");

                if (dt2.Rows.Count > 0)
                {
                    acpAvailableStock.JSProperties["cpstock"] = Convert.ToString(Math.Round(Convert.ToDecimal(dt2.Rows[0]["branchopenstock"]), 2));
                }
                else
                {
                    acpAvailableStock.JSProperties["cpstock"] = "0.00";
                }
            }
            catch (Exception ex)
            {
            }
        }

        //#endregion

        //#endregion

        //#region Numbering Schema

        public string checkNMakeJVCode(string manual_str, int sel_schema_Id)
        {

            // oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);

            oDBEngine = new BusinessLogicLayer.DBEngine();

            DataTable dtSchema = new DataTable();
            DataTable dtC = new DataTable();
            string prefCompCode = string.Empty, sufxCompCode = string.Empty, startNo, paddedStr, sqlQuery = string.Empty;
            int EmpCode, prefLen, sufxLen, paddCounter;

            if (sel_schema_Id > 0)
            {
                dtSchema = oDBEngine.GetDataTable("tbl_master_idschema", "prefix, suffix, digit, startno, schema_type", "id=" + sel_schema_Id);
                int scheme_type = Convert.ToInt32(dtSchema.Rows[0]["schema_type"]);

                if (scheme_type != 0)
                {
                    startNo = dtSchema.Rows[0]["startno"].ToString();
                    paddCounter = Convert.ToInt32(dtSchema.Rows[0]["digit"]);
                    paddedStr = startNo.PadLeft(paddCounter, '0');
                    prefCompCode = dtSchema.Rows[0]["prefix"].ToString();
                    sufxCompCode = dtSchema.Rows[0]["suffix"].ToString();
                    prefLen = Convert.ToInt32(prefCompCode.Length);
                    sufxLen = Convert.ToInt32(sufxCompCode.Length);

                    sqlQuery = "SELECT max(tjv.EstimateCost_Number) FROM trans_EstimateCostSheet tjv WHERE dbo.RegexMatch('";
                    if (prefLen > 0)
                        sqlQuery += "[" + prefCompCode + "]{" + prefLen + "}";
                    else if (scheme_type == 2)
                        sqlQuery += "^";
                    sqlQuery += "[0-9]{" + paddCounter + "}";
                    if (sufxLen > 0)
                        sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                    sqlQuery += "?$', LTRIM(RTRIM(tjv.EstimateCost_Number))) = 1  and tjv.EstimateCost_Number like '" + prefCompCode + "%'";
                    if (scheme_type == 2)
                        sqlQuery += " AND CONVERT(DATE, CreatedDate) = CONVERT(DATE, GETDATE())";
                    dtC = oDBEngine.GetDataTable(sqlQuery);

                    if (dtC.Rows[0][0].ToString() == "")
                    {
                        sqlQuery = "SELECT max(tjv.EstimateCost_Number) FROM trans_EstimateCostSheet tjv WHERE dbo.RegexMatch('";
                        if (prefLen > 0)
                            sqlQuery += "[" + prefCompCode + "]{" + prefLen + "}";
                        else if (scheme_type == 2)
                            sqlQuery += "^";
                        sqlQuery += "[0-9]{" + (paddCounter - 1) + "}";
                        if (sufxLen > 0)
                            sqlQuery += "[" + sufxCompCode + "]{" + sufxLen + "}";
                        sqlQuery += "?$', LTRIM(RTRIM(tjv.EstimateCost_Number))) = 1 and tjv.EstimateCost_Number like '" + prefCompCode + "%'";
                        if (scheme_type == 2)
                            sqlQuery += " AND CONVERT(DATE, CreatedDate) = CONVERT(DATE, GETDATE())";
                        dtC = oDBEngine.GetDataTable(sqlQuery);
                    }

                    if (dtC.Rows.Count > 0 && dtC.Rows[0][0].ToString().Trim() != "")
                    {
                        string uccCode = dtC.Rows[0][0].ToString().Trim();
                        int UCCLen = uccCode.Length;
                        int decimalPartLen = UCCLen - (prefCompCode.Length + sufxCompCode.Length);
                        string uccCodeSubstring = uccCode.Substring(prefCompCode.Length, decimalPartLen);
                        EmpCode = Convert.ToInt32(uccCodeSubstring) + 1;
                        // out of range journal scheme
                        if (EmpCode.ToString().Length > paddCounter)
                        {
                            return "outrange";
                        }
                        else
                        {
                            paddedStr = EmpCode.ToString().PadLeft(paddCounter, '0');
                            UniqueQuotation = prefCompCode + paddedStr + sufxCompCode;
                            return "ok";
                        }
                    }
                    else
                    {
                        UniqueQuotation = startNo.PadLeft(paddCounter, '0');
                        UniqueQuotation = prefCompCode + paddedStr + sufxCompCode;
                        return "ok";
                    }
                }
                else
                {
                    sqlQuery = "SELECT EstimateCost_Number FROM trans_EstimateCostSheet WHERE EstimateCost_Number LIKE '" + manual_str.Trim() + "'";
                    dtC = oDBEngine.GetDataTable(sqlQuery);
                    // duplicate manual entry check
                    if (dtC.Rows.Count > 0 && dtC.Rows[0][0].ToString().Trim() != "")
                    {
                        return "duplicate";
                    }

                    UniqueQuotation = manual_str.Trim();
                    return "ok";
                }
            }
            else
            {
                return "noid";
            }
        }

        //#endregion

        //#region Tax Section

        public void setValueForHeaderGST(ASPxComboBox aspxcmb, string taxId)
        {
            for (int i = 0; i < aspxcmb.Items.Count; i++)
            {
                if (Convert.ToString(aspxcmb.Items[i].Value).Split('~')[0] == taxId.Split('~')[0])
                {
                    aspxcmb.Items[i].Selected = true;
                    break;
                }
            }

        }
        protected DataTable GetTaxDataWithGST(DataTable existing)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "TaxDetailsForGst");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            dt = proc.GetTable();
            if (dt.Rows.Count > 0)
            {
                DataRow gstRow = existing.NewRow();
                gstRow["Taxes_ID"] = 0;
                gstRow["Taxes_Name"] = dt.Rows[0]["TaxRatesSchemeName"];
                gstRow["Percentage"] = dt.Rows[0]["QuoteTax_Percentage"];
                gstRow["Amount"] = dt.Rows[0]["QuoteTax_Amount"];
                gstRow["AltTax_Code"] = dt.Rows[0]["Gst"];

                UpdateGstForCharges(Convert.ToString(dt.Rows[0]["Gst"]));
                txtGstCstVatCharge.Value = gstRow["Amount"];
                existing.Rows.Add(gstRow);
            }
            SetTotalCharges(existing);
            return existing;
        }
        public decimal SetTotalCharges(DataTable taxTableFinal)
        {
            decimal totalCharges = 0;
            foreach (DataRow dr in taxTableFinal.Rows)
            {
                if (Convert.ToString(dr["Taxes_ID"]) != "0")
                {
                    if (Convert.ToString(dr["Taxes_Name"]).Contains("(+)"))
                    {
                        totalCharges += Convert.ToDecimal(dr["Amount"]);
                    }
                    else
                    {
                        totalCharges -= Convert.ToDecimal(dr["Amount"]);
                    }
                }
                else
                {//Else part For Gst 
                    totalCharges += Convert.ToDecimal(dr["Amount"]);
                }
            }
            txtQuoteTaxTotalAmt.Value = totalCharges;
            //bnrOtherChargesvalue.Text = totalCharges.ToString();
            return totalCharges;

        }
        protected void UpdateGstForCharges(string data)
        {
            for (int i = 0; i < cmbGstCstVatcharge.Items.Count; i++)
            {
                if (Convert.ToString(cmbGstCstVatcharge.Items[i].Value).Split('~')[0] == data)
                {
                    cmbGstCstVatcharge.Items[i].Selected = true;
                    break;
                }
            }
        }
        public void GetStock(string strProductID)
        {
            string strBranch = Convert.ToString(ddl_Branch.SelectedValue);
            acpAvailableStock.JSProperties["cpstock"] = "0.00";

            try
            {
                DataTable dt2 = oDBEngine.GetDataTable("Select dbo.fn_CheckAvailableQuotation(" + strBranch + ",'" + Convert.ToString(Session["LastCompany"]) + "','" + Convert.ToString(Session["LastFinYear"]) + "'," + strProductID + ") as branchopenstock");

                if (dt2.Rows.Count > 0)
                {
                    taxUpdatePanel.JSProperties["cpstock"] = Convert.ToString(Math.Round(Convert.ToDecimal(dt2.Rows[0]["branchopenstock"]), 2));
                }
                else
                {
                    taxUpdatePanel.JSProperties["cpstock"] = "0.00";
                }
            }
            catch (Exception ex)
            {
                taxUpdatePanel.JSProperties["cpstock"] = "0.00";
            }
        }
        protected void taxUpdatePanel_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {

            string performpara = e.Parameter;
            if (performpara.Split('~')[0] == "DelProdbySl")
            {
                DataTable MainTaxDataTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];

                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }

                Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = MainTaxDataTable;
                GetStock(Convert.ToString(performpara.Split('~')[2]));
                DeleteWarehouse(Convert.ToString(performpara.Split('~')[1]));
                DataTable taxDetails = (DataTable)Session["TaxDetails"];
                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["TaxDetails"] = taxDetails;
                }
            }
            else if (performpara.Split('~')[0] == "DeleteAllTax")
            {
                if (hdBasketId.Value == "")
                {
                    CreateDataTaxTable();

                    DataTable taxDetails = (DataTable)Session["TaxDetails"];

                    if (taxDetails != null)
                    {
                        foreach (DataRow dr in taxDetails.Rows)
                        {
                            dr["Amount"] = "0.00";
                        }
                        Session["TaxDetails"] = taxDetails;
                    }
                }
            }

            else
            {
                DataTable MainTaxDataTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];

                DataRow[] deletedRow = MainTaxDataTable.Select("SlNo=" + performpara.Split('~')[1]);
                if (deletedRow.Length > 0)
                {
                    foreach (DataRow dr in deletedRow)
                    {
                        MainTaxDataTable.Rows.Remove(dr);
                    }

                }

                Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = MainTaxDataTable;
                DataTable taxDetails = (DataTable)Session["TaxDetails"];

                if (taxDetails != null)
                {
                    foreach (DataRow dr in taxDetails.Rows)
                    {
                        dr["Amount"] = "0.00";
                    }
                    Session["TaxDetails"] = taxDetails;
                }
            }
        }
        public double ReCalculateTaxAmount(string slno, double amount)
        {
            DataTable MainTaxDataTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
            double totalSum = 0.0;
            //Get The Existing datatable
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "PopulateAllTax");
            DataTable TaxDt = proc.GetTable();

            DataRow[] filterRow = MainTaxDataTable.Select("SlNo=" + slno);

            if (filterRow.Length > 0)
            {
                foreach (DataRow dr in filterRow)
                {
                    if (Convert.ToString(dr["TaxCode"]) != "0")
                    {
                        DataRow[] taxrow = TaxDt.Select("Taxes_ID=" + dr["TaxCode"]);
                        if (taxrow.Length > 0)
                        {
                            if (taxrow[0]["TaxCalculateMethods"] == "A")
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum += (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                            else
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum -= (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                        }
                    }
                    else
                    {
                        DataRow[] taxrow = TaxDt.Select("Taxes_ID=" + dr["AltTaxCode"]);
                        if (taxrow.Length > 0)
                        {
                            if (taxrow[0]["TaxCalculateMethods"] == "A")
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum += (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                            else
                            {
                                dr["Amount"] = (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                                totalSum -= (amount * Convert.ToDouble(dr["Percentage"])) / 100;
                            }
                        }
                    }
                }

            }
            Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = MainTaxDataTable;

            return totalSum;

        }
        public void PopulateGSTCSTVATCombo(string quoteDate)
        {
            string LastCompany = "";
            if (Convert.ToString(Session["LastCompany"]) != null)
            {
                LastCompany = Convert.ToString(Session["LastCompany"]);
            }
            //DataTable dt = new DataTable();
            //dt = objCRMSalesDtlBL.PopulateGSTCSTVATCombo();
            //DataTable DT = oDBEngine.GetDataTable("select cast(td.TaxRates_ID as varchar(5))+'~'+ cast (td.TaxRates_Rate as varchar(25)) 'Taxes_ID',td.TaxRatesSchemeName 'Taxes_Name',th.Taxes_Name as 'TaxCodeName' from Master_Taxes th inner join Config_TaxRates td on th.Taxes_ID=td.TaxRates_TaxCode where (td.TaxRates_Country=0 or td.TaxRates_Country=(select add_country from tbl_master_address where add_cntId ='" + Convert.ToString(Session["LastCompany"]) + "' ))  and th.Taxes_ApplicableFor in ('B','S') and th.TaxTypeCode in('G','V','C')");

            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "LoadGSTCSTVATCombo");
            proc.AddVarcharPara("@S_QuoteAdd_CompanyID", 10, Convert.ToString(LastCompany));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            proc.AddIntegerPara("@branchId", Convert.ToInt32(Session["userbranchID"]));
            DataTable DT = proc.GetTable();
            cmbGstCstVat.DataSource = DT;
            cmbGstCstVat.TextField = "Taxes_Name";
            cmbGstCstVat.ValueField = "Taxes_ID";
            cmbGstCstVat.DataBind();
        }
        public void CreateDataTaxTable()
        {
            DataTable TaxRecord = new DataTable();

            TaxRecord.Columns.Add("SlNo", typeof(System.Int32));
            TaxRecord.Columns.Add("TaxCode", typeof(System.String));
            TaxRecord.Columns.Add("AltTaxCode", typeof(System.String));
            TaxRecord.Columns.Add("Percentage", typeof(System.Decimal));
            TaxRecord.Columns.Add("Amount", typeof(System.Decimal));
            Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxRecord;
        }
        public string GetTaxName(int id)
        {
            string taxName = "";
            string[] arr = oDBEngine.GetFieldValue1("Master_taxes", "Taxes_Name", "Taxes_ID=" + Convert.ToString(id), 1);
            if (arr[0] != "n")
            {
                taxName = arr[0];
            }
            return taxName;
        }
        public DataSet GetQuotationTaxData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "ProductTaxDetails");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public DataSet GetQuotationEditedTaxData()
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "ProductEditedTaxDetails");
            proc.AddVarcharPara("@EstimateCostID", 500, Convert.ToString(Session["EstimateCost_Id"]));
            ds = proc.GetDataSet();
            return ds;
        }
        public double GetTotalTaxAmount(List<TaxDetails> tax)
        {
            double sum = 0;
            foreach (TaxDetails td in tax)
            {
                if (td.Taxes_Name.Substring(td.Taxes_Name.Length - 3, 3) == "(+)")
                    sum += td.Amount;
                else
                    sum -= td.Amount;

            }
            return sum;
        }
        protected void cgridTax_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            string retMsg = "";
            if (e.Parameters.Split('~')[0] == "SaveGST")
            {
                DataTable TaxRecord = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
                int slNo = Convert.ToInt32(HdSerialNo.Value);
                //For GST/CST/VAT
                if (cmbGstCstVat.Value != null)
                {

                    DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='0'");
                    if (finalRow.Length > 0)
                    {
                        finalRow[0]["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                        finalRow[0]["Amount"] = txtGstCstVat.Text;
                        finalRow[0]["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];

                    }
                    else
                    {
                        DataRow newRowGST = TaxRecord.NewRow();
                        newRowGST["slNo"] = slNo;
                        newRowGST["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                        newRowGST["TaxCode"] = "0";
                        newRowGST["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];
                        newRowGST["Amount"] = txtGstCstVat.Text;
                        TaxRecord.Rows.Add(newRowGST);
                    }
                }
                //End Here

                aspxGridTax.JSProperties["cpUpdated"] = "";

                Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxRecord;
            }
            else
            {
                //#region fetch All data For Tax

                DataTable taxDetail = new DataTable();
                DataTable MainTaxDataTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
                DataTable databaseReturnTable = (DataTable)Session["QuotationTaxDetails"];

                //if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 1)
                //    taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");
                //else if (Convert.ToInt32(e.Parameters.Split('~')[1]) == 2)
                //taxDetail = oDBEngine.GetDataTable("select TaxRates_ID as Taxes_ID,TaxRatesSchemeName as Taxes_Name,TaxCalculateMethods,tm.Taxes_Name as taxCodeName,tm.Taxes_ApplicableOn as 'ApplicableOn',isnull((select TaxRatesSchemeName from Config_TaxRates where TaxRates_ID=tm.Taxes_OtherTax),'')  as dependOn from Master_taxes tm inner join Config_TaxRates ct on tm.Taxes_ID=ct.TaxRates_TaxCode where tm.TaxTypeCode not in('G','V','C') and tm.TaxItemlevel='Y'");


                ProcedureExecute proc = new ProcedureExecute("prc_TaxExceptionFind");
                proc.AddVarcharPara("@Action", 500, "SQ");
                proc.AddVarcharPara("@ProductID", 10, Convert.ToString(setCurrentProdCode.Value));
                proc.AddVarcharPara("@ENTITY_ID", 100, hdnCustomerId.Value);
                proc.AddVarcharPara("@Date", 10, dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                proc.AddVarcharPara("@Amount", 100, HdProdGrossAmt.Value);
                proc.AddVarcharPara("@Qty", 100, hdnQty.Value);
                taxDetail = proc.GetTable();



                //ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
                //proc.AddVarcharPara("@Action", 500, "LoadOtherTaxDetails");
                //proc.AddVarcharPara("@ProductID", 10, Convert.ToString(setCurrentProdCode.Value));
                //proc.AddVarcharPara("@S_quoteDate", 10, dt_PLQuote.Date.ToString("yyyy-MM-dd"));
                //taxDetail = proc.GetTable();

                //Get Company Gstin 09032017
                string CompInternalId = Convert.ToString(Session["LastCompany"]);
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);

                //Get BranchStateCode
                string BranchStateCode = "", BranchGSTIN = "";
                DataTable BranchTable = oDBEngine.GetDataTable("select StateCode,branch_GSTIN   from tbl_master_branch branch inner join tbl_master_state st on branch.branch_state=st.id where branch_id=" + Convert.ToString(ddl_Branch.SelectedValue));
                if (BranchTable != null)
                {
                    BranchStateCode = Convert.ToString(BranchTable.Rows[0][0]);
                    BranchGSTIN = Convert.ToString(BranchTable.Rows[0][1]);
                }


                string ShippingState = "";
                //#region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode = BillingShippingControl.GeteShippingStateCode();
                ShippingState = sstateCode;
                //if (ShippingState.Trim() != "")
                //{
                //    ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                //}
                //#region ##### Old Code -- BillingShipping ######
                ////if (chkBilling.Checked)
                ////{
                ////    if (CmbState.Value != null)
                ////    {
                ////        ShippingState = CmbState.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                ////else
                ////{
                ////    if (CmbState1.Value != null)
                ////    {
                ////        ShippingState = CmbState1.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                //#endregion

                //#endregion

                if (ShippingState.Trim() != "" && BranchStateCode != "")
                {


                    if (BranchStateCode == ShippingState)
                    {
                        //Check if the state is in union territories then only UTGST will apply
                        //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU    Lakshadweep              PONDICHERRY
                        if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                        {
                            foreach (DataRow dr in taxDetail.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                                {
                                    dr.Delete();
                                }
                            }

                        }
                        else
                        {
                            foreach (DataRow dr in taxDetail.Rows)
                            {
                                if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                                {
                                    dr.Delete();
                                }
                            }
                        }
                        taxDetail.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                            {
                                dr.Delete();
                            }
                        }
                        taxDetail.AcceptChanges();

                    }


                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (compGstin[0].Trim() == "" && BranchGSTIN == "")
                {
                    foreach (DataRow dr in taxDetail.Rows)
                    {
                        if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                        {
                            dr.Delete();
                        }
                    }
                    taxDetail.AcceptChanges();
                }

                //Check If any TaxScheme Set Against that Product Then update there rate 22-03-2017 and rate
                string[] schemeIDViaProdID = oDBEngine.GetFieldValue1("master_sproducts", "isnull(sProduct_TaxSchemeSale,0)sProduct_TaxSchemeSale", "sProducts_ID='" + Convert.ToString(setCurrentProdCode.Value) + "'", 1);
                //&& schemeIDViaProdID[0] != ""
                if (schemeIDViaProdID.Length > 0)
                {

                    if (taxDetail.Select("Taxes_ID='" + schemeIDViaProdID[0] + "'").Length > 0)
                    {
                        foreach (DataRow dr in taxDetail.Rows)
                        {
                            if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST")
                            {
                                if (Convert.ToString(dr["Taxes_ID"]).Trim() != schemeIDViaProdID[0].Trim())
                                {
                                    //dr["TaxRates_Rate"] = 0;
                                }

                            }
                        }
                    }
                }



                int slNo = Convert.ToInt32(HdSerialNo.Value);

                //Get Gross Amount and Net Amount 
                decimal ProdGrossAmt = Convert.ToDecimal(HdProdGrossAmt.Value);
                decimal ProdNetAmt = Convert.ToDecimal(HdProdNetAmt.Value);

                List<TaxDetails> TaxDetailsDetails = new List<TaxDetails>();

                //Debjyoti 09032017
                decimal totalParcentage = 0;
                foreach (DataRow dr in taxDetail.Rows)
                {
                    if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "UTGST")
                    {
                        totalParcentage += Convert.ToDecimal(dr["TaxRates_Rate"]);
                    }
                }

                if (e.Parameters.Split('~')[0] == "New")
                {
                    foreach (DataRow dr in taxDetail.Rows)
                    {
                        TaxDetails obj = new TaxDetails();
                        obj.Taxes_ID = Convert.ToInt32(dr["Taxes_ID"]);
                        obj.taxCodeName = Convert.ToString(dr["taxCodeName"]);
                        obj.TaxField = Convert.ToString(dr["TaxRates_Rate"]);
                        obj.Amount = 0.0;

                        //#region set calculated on
                        //Check Tax Applicable on and set to calculated on
                        if (Convert.ToString(dr["ApplicableOn"]) == "G")
                        {
                            obj.calCulatedOn = ProdGrossAmt;
                        }
                        else if (Convert.ToString(dr["ApplicableOn"]) == "N")
                        {
                            obj.calCulatedOn = ProdNetAmt;
                        }
                        else
                        {
                            obj.calCulatedOn = 0;
                        }
                        //End Here
                        //#endregion

                        //Debjyoti 09032017
                        //if (Convert.ToString(ddl_AmountAre.Value) == "2")
                        //{
                        //    if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                        //    {
                        //        if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                        //        {
                        //            decimal finalCalCulatedOn = 0;
                        //            decimal backProcessRate = (1 + (totalParcentage / 100));
                        //            finalCalCulatedOn = obj.calCulatedOn / backProcessRate;
                        //            obj.calCulatedOn = finalCalCulatedOn;
                        //        }
                        //    }
                        //}

                        if (Convert.ToString(dr["TaxCalculateMethods"]) == "A")
                        {
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(+)";

                        }
                        else
                        {
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(-)";
                        }

                        obj.Amount = Convert.ToDouble(obj.calCulatedOn * (Convert.ToDecimal(obj.TaxField) / 100));




                        DataRow[] filtr = MainTaxDataTable.Select("TaxCode ='" + obj.Taxes_ID + "' and slNo=" + Convert.ToString(slNo));
                        if (filtr.Length > 0)
                        {
                            obj.Amount = Convert.ToDouble(filtr[0]["Amount"]);
                            if (obj.Taxes_ID == 0)
                            {
                                //   obj.TaxField = GetTaxName(Convert.ToInt32(Convert.ToString(filtr[0]["AltTaxCode"])));
                                aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtr[0]["AltTaxCode"]);
                            }
                            else
                                obj.TaxField = Convert.ToString(filtr[0]["Percentage"]);
                        }

                        TaxDetailsDetails.Add(obj);
                    }
                }
                else
                {
                    string keyValue = e.Parameters.Split('~')[0];

                    DataTable TaxRecord = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];


                    foreach (DataRow dr in taxDetail.Rows)
                    {
                        TaxDetails obj = new TaxDetails();
                        obj.Taxes_ID = Convert.ToInt32(dr["Taxes_ID"]);
                        obj.taxCodeName = Convert.ToString(dr["taxCodeName"]);

                        if (Convert.ToString(dr["TaxCalculateMethods"]) == "A")
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(+)";
                        else
                            obj.Taxes_Name = Convert.ToString(dr["Taxes_Name"]) + "(-)";
                        //chinmoy edited below code for edit mode billing/shipping change
                        //obj.TaxField = "";
                        if (Convert.ToDecimal(dr["TaxRates_Rate"]) == 0)
                        {
                            obj.TaxField = "";
                        }
                        else
                        {
                            obj.TaxField = Convert.ToString(dr["TaxRates_Rate"]);
                        }
                        //End
                        obj.Amount = 0.0;

                        //#region set calculated on
                        //Check Tax Applicable on and set to calculated on
                        if (Convert.ToString(dr["ApplicableOn"]) == "G")
                        {
                            obj.calCulatedOn = ProdGrossAmt;
                        }
                        else if (Convert.ToString(dr["ApplicableOn"]) == "N")
                        {
                            obj.calCulatedOn = ProdNetAmt;
                        }
                        else
                        {
                            obj.calCulatedOn = 0;
                        }
                        //End Here
                        //#endregion




                        //Debjyoti 09032017
                        //if (Convert.ToString(ddl_AmountAre.Value) == "2")
                        //{
                        //    if (Convert.ToString(ddl_VatGstCst.Value) == "0~0~X")
                        //    {
                        //        if (Convert.ToString(dr["TaxTypeCode"]).Trim() == "IGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "CGST" || Convert.ToString(dr["TaxTypeCode"]).Trim() == "SGST")
                        //        {
                        //            decimal finalCalCulatedOn = 0;
                        //            decimal backProcessRate = (1 + (totalParcentage / 100));
                        //            finalCalCulatedOn = obj.calCulatedOn / backProcessRate;
                        //            obj.calCulatedOn = finalCalCulatedOn;
                        //        }
                        //    }
                        //}


                        //chinmoy added below code for edit mode billing/shipping change start
                        if (Convert.ToDecimal(dr["TaxRates_Rate"]) != 0)
                        {
                            obj.Amount = Convert.ToDouble(obj.calCulatedOn * (Convert.ToDecimal(obj.TaxField) / 100));
                        }

                        //End

                        DataRow[] filtronexsisting1 = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                        if (filtronexsisting1.Length > 0)
                        {
                            if (obj.Taxes_ID == 0)
                            {
                                obj.TaxField = "0";
                            }
                            else
                            {
                                obj.TaxField = Convert.ToString(filtronexsisting1[0]["Percentage"]);
                            }
                            obj.Amount = Convert.ToDouble(filtronexsisting1[0]["Amount"]);
                        }
                        else
                        {
                            //#region checkingFordb


                            //DataRow[] filtr = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" +Convert.ToString( Session["EstimateCost_Id"] )+ " and ProductTax_TaxTypeId=" + obj.Taxes_ID);
                            //if (filtr.Length > 0)
                            //{
                            //    obj.Amount = Convert.ToDouble(filtr[0]["ProductTax_Amount"]);
                            //    if (obj.Taxes_ID == 0)
                            //    {
                            //        //obj.TaxField = GetTaxName();
                            //        obj.TaxField = "0";
                            //    }
                            //    else
                            //    {
                            //        obj.TaxField = Convert.ToString(filtr[0]["ProductTax_Percentage"]);
                            //    }


                            //    DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                            //    if (filtronexsisting.Length > 0)
                            //    {
                            //        filtronexsisting[0]["Amount"] = obj.Amount;
                            //        if (obj.Taxes_ID == 0)
                            //        {
                            //            filtronexsisting[0]["Percentage"] = 0;
                            //        }
                            //        else
                            //        {
                            //            filtronexsisting[0]["Percentage"] = obj.TaxField;
                            //        }

                            //    }
                            //    else
                            //    {

                            //        DataRow taxRecordNewRow = TaxRecord.NewRow();
                            //        taxRecordNewRow["SlNo"] = slNo;
                            //        taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                            //        taxRecordNewRow["AltTaxCode"] = "0";
                            //        taxRecordNewRow["Percentage"] = obj.TaxField;
                            //        taxRecordNewRow["Amount"] = obj.Amount;

                            //        TaxRecord.Rows.Add(taxRecordNewRow);
                            //    }

                            //}
                            //else
                            //{
                            //    DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                            //    if (filtronexsisting.Length > 0)
                            //    {
                            //        DataRow taxRecordNewRow = TaxRecord.NewRow();
                            //        taxRecordNewRow["SlNo"] = slNo;
                            //        taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                            //        taxRecordNewRow["AltTaxCode"] = "0";
                            //        taxRecordNewRow["Percentage"] = 0.0;
                            //        taxRecordNewRow["Amount"] = "0";

                            //        TaxRecord.Rows.Add(taxRecordNewRow);
                            //    }
                            //}




                            //#endregion


                            DataRow[] filtronexsisting = TaxRecord.Select("TaxCode=" + obj.Taxes_ID + " and SlNo=" + Convert.ToString(slNo));
                            if (filtronexsisting.Length > 0)
                            {
                                DataRow taxRecordNewRow = TaxRecord.NewRow();
                                taxRecordNewRow["SlNo"] = slNo;
                                taxRecordNewRow["TaxCode"] = obj.Taxes_ID;
                                taxRecordNewRow["AltTaxCode"] = "0";
                                taxRecordNewRow["Percentage"] = 0.0;
                                taxRecordNewRow["Amount"] = "0";

                                TaxRecord.Rows.Add(taxRecordNewRow);
                            }

                        }
                        TaxDetailsDetails.Add(obj);

                        //      DataRow[] filtrIndex = databaseReturnTable.Select("ProductTax_ProductId ='" + keyValue + "' and ProductTax_QuoteId=" + Session["EstimateCost_Id"] + " and ProductTax_TaxTypeId=0");
                        DataRow[] filtrIndex = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                        if (filtrIndex.Length > 0)
                        {
                            aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(filtrIndex[0]["AltTaxCode"]);
                        }
                    }
                    Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxRecord;

                }
                //New Changes 170217
                //GstCode should fetch everytime
                DataRow[] finalFiltrIndex = MainTaxDataTable.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode=0");
                if (finalFiltrIndex.Length > 0)
                {
                    aspxGridTax.JSProperties["cpComboCode"] = Convert.ToString(finalFiltrIndex[0]["AltTaxCode"]);
                }

                for (var i = 0; i < TaxDetailsDetails.Count; i++)
                {
                    decimal _Amount = Convert.ToDecimal(TaxDetailsDetails[i].Amount);
                    decimal _calCulatedOn = Convert.ToDecimal(TaxDetailsDetails[i].calCulatedOn);

                    TaxDetailsDetails[i].Amount = GetRoundOfValue(_Amount);
                    TaxDetailsDetails[i].calCulatedOn = Convert.ToDecimal(GetRoundOfValue(_calCulatedOn));
                }

                aspxGridTax.JSProperties["cpJsonData"] = createJsonForDetails(taxDetail);

                retMsg = Convert.ToString(GetTotalTaxAmount(TaxDetailsDetails));
                aspxGridTax.JSProperties["cpUpdated"] = "ok~" + retMsg;

                TaxDetailsDetails = setCalculatedOn(TaxDetailsDetails, taxDetail);
                aspxGridTax.DataSource = TaxDetailsDetails;
                aspxGridTax.DataBind();

                //#endregion
            }
        }
        public string createJsonForDetails(DataTable lstTaxDetails)
        {
            List<TaxSetailsJson> jsonList = new List<TaxSetailsJson>();
            TaxSetailsJson jsonObj;
            int visIndex = 0;
            foreach (DataRow taxObj in lstTaxDetails.Rows)
            {
                jsonObj = new TaxSetailsJson();

                jsonObj.SchemeName = Convert.ToString(taxObj["Taxes_Name"]);
                jsonObj.vissibleIndex = visIndex;
                jsonObj.applicableOn = Convert.ToString(taxObj["ApplicableOn"]);
                if (jsonObj.applicableOn == "G" || jsonObj.applicableOn == "N")
                {
                    jsonObj.applicableBy = "None";
                }
                else
                {
                    jsonObj.applicableBy = Convert.ToString(taxObj["dependOn"]);
                }
                visIndex++;
                jsonList.Add(jsonObj);
            }

            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Serialize(jsonList);
        }
        public List<TaxDetails> setCalculatedOn(List<TaxDetails> gridSource, DataTable taxDt)
        {
            foreach (TaxDetails taxObj in gridSource)
            {
                DataRow[] dependOn = taxDt.Select("dependOn='" + taxObj.Taxes_Name.Replace("(+)", "").Replace("(-)", "") + "'");
                if (dependOn.Length > 0)
                {
                    foreach (DataRow dr in dependOn)
                    {
                        //  List<TaxDetails> dependObj = gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])).ToList();
                        foreach (var setCalObj in gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])))
                        {
                            setCalObj.calCulatedOn = Convert.ToDecimal(taxObj.Amount);
                        }
                    }

                }

            }
            return gridSource;
        }
        public double GetRoundOfValue(decimal Value)
        {
            double RoundOfValue = 0;

            if (Value > 0)
            {
                decimal _Value = Convert.ToDecimal(Value);
                _Value = Math.Round(Value, 2);

                RoundOfValue = Convert.ToDouble(_Value);
            }

            return RoundOfValue;
        }
        protected void aspxGridTax_CellEditorInitialize(object sender, DevExpress.Web.ASPxGridViewEditorEventArgs e)
        {

            if (e.Column.FieldName == "Taxes_Name")
            {
                e.Editor.ReadOnly = true;
            }
            if (e.Column.FieldName == "taxCodeName")
            {
                e.Editor.ReadOnly = true;
            }
            if (e.Column.FieldName == "calCulatedOn")
            {
                e.Editor.ReadOnly = true;
            }
            //else if (e.Column.FieldName == "Amount")
            //{
            //    e.Editor.ReadOnly = true;
            //}
            else
            {
                e.Editor.ReadOnly = false;
            }
        }
        protected void aspxGridTax_HtmlRowCreated(object sender, ASPxGridViewTableRowEventArgs e)
        {

        }
        protected void taxgrid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {

            int slNo = Convert.ToInt32(HdSerialNo.Value);
            DataTable TaxRecord = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];
            foreach (var args in e.UpdateValues)
            {

                string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
                decimal Percentage = 0;

                Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);

                decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);
                string TaxCode = "0";
                if (!Convert.ToString(args.Keys[0]).Contains('~'))
                {
                    TaxCode = Convert.ToString(args.Keys[0]);
                }



                DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='" + TaxCode + "'");
                if (finalRow.Length > 0)
                {
                    finalRow[0]["Percentage"] = Percentage;
                    // finalRow[0]["TaxCode"] = args.NewValues["TaxField"]; 
                    finalRow[0]["Amount"] = Amount;

                    finalRow[0]["TaxCode"] = args.Keys[0];
                    finalRow[0]["AltTaxCode"] = "0";

                }
                else
                {
                    DataRow newRow = TaxRecord.NewRow();
                    newRow["slNo"] = slNo;
                    newRow["Percentage"] = Percentage;
                    newRow["TaxCode"] = TaxCode;
                    newRow["AltTaxCode"] = "0";
                    newRow["Amount"] = Amount;
                    TaxRecord.Rows.Add(newRow);
                }


            }

            //For GST/CST/VAT
            if (cmbGstCstVat.Value != null)
            {

                DataRow[] finalRow = TaxRecord.Select("SlNo=" + Convert.ToString(slNo) + " and TaxCode='0'");
                if (finalRow.Length > 0)
                {
                    finalRow[0]["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                    finalRow[0]["Amount"] = txtGstCstVat.Text;
                    finalRow[0]["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];

                }
                else
                {
                    DataRow newRowGST = TaxRecord.NewRow();
                    newRowGST["slNo"] = slNo;
                    newRowGST["Percentage"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[1];
                    newRowGST["TaxCode"] = "0";
                    newRowGST["AltTaxCode"] = Convert.ToString(cmbGstCstVat.Value).Split('~')[0];
                    newRowGST["Amount"] = txtGstCstVat.Text;
                    TaxRecord.Rows.Add(newRowGST);
                }
            }
            //End Here


            Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxRecord;


            DataRow[] finalRowGSTTotal = TaxRecord.Select("SlNo=" + Convert.ToString(slNo));
            if (finalRowGSTTotal.Length > 0)
            {
                DataTable TaxTable = finalRowGSTTotal.CopyToDataTable();
                DataSet dsInst = new DataSet();
                //  SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);		
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
                SqlCommand cmd = new SqlCommand("prc_PosSalesInvoice", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetTotalGSTVALUE");
                cmd.Parameters.AddWithValue("@TaxDetail", TaxTable);
                SqlDataAdapter Adap = new SqlDataAdapter();
                Adap.SelectCommand = cmd;
                Adap.Fill(dsInst);
                cmd.Dispose();
                con.Dispose();
                if (dsInst.Tables[0].Rows.Count > 0)
                {
                    aspxGridTax.JSProperties["cpTotalGST"] = dsInst.Tables[0].Rows[0][0];
                    aspxGridTax.JSProperties["cpGSTType"] = dsInst.Tables[0].Rows[0][1];
                }
            }

            //#region oldpart


            //DataTable taxdtByProductCode = new DataTable();
            //if (Session["ProdTax" + "_" + Convert.ToString(HdSerialNo.Value)] == null)
            //{

            //    taxdtByProductCode.TableName = "ProdTax"  + "_" + Convert.ToString(HdSerialNo.Value);


            //    taxdtByProductCode.Columns.Add("TaxCode", typeof(System.String));
            //    taxdtByProductCode.Columns.Add("TaxCodeDescription", typeof(System.String));
            //    taxdtByProductCode.Columns.Add("Percentage", typeof(System.Decimal));
            //    taxdtByProductCode.Columns.Add("Amount", typeof(System.Decimal));
            //    DataRow dr;
            //    foreach (var args in e.UpdateValues)
            //    {
            //        dr = taxdtByProductCode.NewRow();
            //        string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
            //        decimal Percentage = 0;
            //        if (TaxCodeDes == "GST/CST/VAT")
            //        {
            //            Percentage = Convert.ToDecimal(Convert.ToString(args.NewValues["TaxField"]).Split('~')[1]);
            //        }
            //        else
            //        {
            //            Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);
            //        }
            //        decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);

            //        dr["TaxCodeDescription"] = TaxCodeDes;
            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            dr["TaxCode"] = "0~" + Convert.ToString(args.NewValues["TaxField"]).Split('~')[0];
            //        }
            //        else
            //        {
            //            dr["TaxCode"] = args.Keys[0];
            //        }
            //        dr["Percentage"] = Percentage;
            //        dr["Amount"] = Amount;

            //        taxdtByProductCode.Rows.Add(dr);
            //    }
            //}
            //else
            //{
            //    taxdtByProductCode = (DataTable)Session["ProdTax"  +"_"+ Convert.ToString(HdSerialNo.Value)];

            //    foreach (var args in e.UpdateValues)
            //    {
            //        DataRow[] filtr ;

            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            filtr = taxdtByProductCode.Select("TaxCode like '%0~%'"); ;
            //        }
            //        else
            //        {
            //            filtr = taxdtByProductCode.Select("TaxCode='" + args.Keys[0]+"'");
            //        }

            //        if (filtr.Length > 0)
            //        {
            //            string TaxCodeDes = Convert.ToString(args.NewValues["Taxes_Name"]);
            //        filtr[0]["TaxCodeDescription"] = TaxCodeDes;
            //        if (Convert.ToString(args.Keys[0]) == "0")
            //        {
            //            filtr[0]["TaxCode"] = "0~" + Convert.ToString(args.NewValues["TaxField"]).Split('~')[0];
            //        }
            //        else
            //        {
            //            filtr[0]["TaxCode"] = args.Keys[0];
            //        }

            //        decimal Percentage = 0;
            //        if (TaxCodeDes == "GST/CST/VAT")
            //        {
            //            Percentage = Convert.ToDecimal(Convert.ToString(args.NewValues["TaxField"]).Split('~')[1]);
            //        }
            //        else
            //        {
            //            Percentage = Convert.ToDecimal(args.NewValues["TaxField"]);
            //        }
            //        decimal Amount = Convert.ToDecimal(args.NewValues["Amount"]);
            //        filtr[0]["Percentage"] = Percentage;
            //        filtr[0]["Amount"] = Amount;

            //        }
            //    }


            //}

            //#endregion
            //  Session[taxdtByProductCode.TableName] = taxdtByProductCode;

        }




        protected void cmbGstCstVat_Callback(object sender, CallbackEventArgsBase e)
        {
            DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("yyyy-MM-dd"));

            PopulateGSTCSTVATCombo(quoteDate.ToString("yyyy-MM-dd"));
            CreateDataTaxTable();
            //DataTable taxTableItemLvl = (DataTable)Session["FinalTaxRecord"];
            //foreach (DataRow dr in taxTableItemLvl.Rows)
            //    dr.Delete();
            //taxTableItemLvl.AcceptChanges();
            //Session["FinalTaxRecord"] = taxTableItemLvl;
        }
        protected void cmbGstCstVatcharge_Callback(object sender, CallbackEventArgsBase e)
        {
            Session["TaxDetails"] = null;
            DateTime quoteDate = Convert.ToDateTime(dt_PLQuote.Date.ToString("yyyy-MM-dd"));
            PopulateChargeGSTCSTVATCombo(quoteDate.ToString("yyyy-MM-dd"));
        }
        public DataTable getAllTaxDetails(DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable FinalTable = new DataTable();
            FinalTable.Columns.Add("SlNo", typeof(System.Int32));
            FinalTable.Columns.Add("TaxCode", typeof(System.String));
            FinalTable.Columns.Add("AltTaxCode", typeof(System.String));
            FinalTable.Columns.Add("Percentage", typeof(System.Decimal));
            FinalTable.Columns.Add("Amount", typeof(System.Decimal));

            //for insert
            foreach (var args in e.InsertValues)
            {
                string Slno = Convert.ToString(args.NewValues["SrlNo"]);
                DataRow existsRow;
                if (Session["ProdTax_" + Slno] != null)
                {
                    DataTable sessiontable = (DataTable)Session["ProdTax_" + Slno];
                    foreach (DataRow dr in sessiontable.Rows)
                    {
                        existsRow = FinalTable.NewRow();

                        existsRow["SlNo"] = Slno;
                        if (Convert.ToString(dr["taxCode"]).Contains("~"))
                        {
                            existsRow["TaxCode"] = "0";
                            existsRow["AltTaxCode"] = Convert.ToString(dr["taxCode"]).Split('~')[1];
                        }
                        else
                        {
                            existsRow["TaxCode"] = Convert.ToString(dr["taxCode"]);
                            existsRow["AltTaxCode"] = "0";
                        }

                        existsRow["Percentage"] = Convert.ToString(dr["Percentage"]);
                        existsRow["Amount"] = Convert.ToString(dr["Amount"]);

                        FinalTable.Rows.Add(existsRow);
                    }
                    Session.Remove("ProdTax_" + Slno);
                }
            }

            return FinalTable;
        }
        protected void taxgrid_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void taxgrid_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            e.Cancel = true;
        }
        protected void taxgrid_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            e.Cancel = true;
        }
        public void DeleteTaxDetails(string SrlNo)
        {
            DataTable TaxDetailTable = new DataTable();
            if (Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] != null)
            {
                TaxDetailTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];

                var rows = TaxDetailTable.Select("SlNo ='" + SrlNo + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                TaxDetailTable.AcceptChanges();

                Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxDetailTable;
            }
        }
        public void UpdateTaxDetails(string oldSrlNo, string newSrlNo)
        {
            DataTable TaxDetailTable = new DataTable();
            if (Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] != null)
            {
                TaxDetailTable = (DataTable)Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)];

                for (int i = 0; i < TaxDetailTable.Rows.Count; i++)
                {
                    DataRow dr = TaxDetailTable.Rows[i];
                    string Product_SrlNo = Convert.ToString(dr["SlNo"]);
                    if (oldSrlNo == Product_SrlNo)
                    {
                        dr["SlNo"] = newSrlNo;
                    }
                }
                TaxDetailTable.AcceptChanges();

                Session["FinalTaxRecord" + Convert.ToString(uniqueId.Value)] = TaxDetailTable;
            }
        }
        public string createJsonForChargesTax(DataTable lstTaxDetails)
        {
            List<TaxSetailsJson> jsonList = new List<TaxSetailsJson>();
            TaxSetailsJson jsonObj;
            int visIndex = 0;
            foreach (DataRow taxObj in lstTaxDetails.Rows)
            {
                jsonObj = new TaxSetailsJson();

                jsonObj.SchemeName = Convert.ToString(taxObj["Taxes_Name"]);
                jsonObj.vissibleIndex = visIndex;
                jsonObj.applicableOn = "G";//Convert.ToString(taxObj["ApplicableOn"]);
                if (jsonObj.applicableOn == "G" || jsonObj.applicableOn == "N")
                {
                    jsonObj.applicableBy = "None";
                }
                else
                {
                    jsonObj.applicableBy = Convert.ToString(taxObj["dependOn"]);
                }
                visIndex++;
                jsonList.Add(jsonObj);
            }

            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return oSerializer.Serialize(jsonList);
        }
        public List<Tax> setChargeCalculatedOn(List<Tax> gridSource, DataTable taxDt)
        {
            foreach (Tax taxObj in gridSource)
            {
                DataRow[] dependOn = taxDt.Select("dependOn='" + taxObj.TaxName.Replace("(+)", "").Replace("(-)", "").Trim() + "'");
                if (dependOn.Length > 0)
                {
                    foreach (DataRow dr in dependOn)
                    {
                        //  List<TaxDetails> dependObj = gridSource.Where(r => r.Taxes_Name.Replace("(+)", "").Replace("(-)", "") == Convert.ToString(dependOn[0]["Taxes_Name"])).ToList();
                        foreach (var setCalObj in gridSource.Where(r => r.TaxName.Replace("(+)", "").Replace("(-)", "").Trim() == Convert.ToString(dependOn[0]["Taxes_Name"]).Replace("(+)", "").Replace("(-)", "").Trim()))
                        {
                            setCalObj.calCulatedOn = Convert.ToDecimal(taxObj.Amount);
                        }
                    }

                }

            }
            return gridSource;
        }
        public void PopulateChargeGSTCSTVATCombo(string quoteDate)
        {
            string LastCompany = "";
            if (Convert.ToString(Session["LastCompany"]) != null)
            {
                LastCompany = Convert.ToString(Session["LastCompany"]);
            }
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 500, "LoadChargeGSTCSTVATCombo");
            proc.AddVarcharPara("@S_QuoteAdd_CompanyID", 10, Convert.ToString(LastCompany));
            proc.AddVarcharPara("@S_quoteDate", 10, quoteDate);
            proc.AddIntegerPara("@branchId", Convert.ToInt32(Session["userbranchID"]));
            DataTable DT = proc.GetTable();
            cmbGstCstVatcharge.DataSource = DT;
            cmbGstCstVatcharge.TextField = "Taxes_Name";
            cmbGstCstVatcharge.ValueField = "Taxes_ID";
            cmbGstCstVatcharge.DataBind();
        }

        //#region Tax Class

        public class TaxSetailsJson
        {
            public string SchemeName { get; set; }
            public int vissibleIndex { get; set; }
            public string applicableOn { get; set; }
            public string applicableBy { get; set; }
        }
        public class TaxDetails
        {
            public int Taxes_ID { get; set; }
            public string Taxes_Name { get; set; }
            public double Amount { get; set; }
            public string TaxField { get; set; }
            public string taxCodeName { get; set; }
            public decimal calCulatedOn { get; set; }

        }
        public class MultiUOMPacking
        {
            public decimal packing_quantity { get; set; }
            public decimal sProduct_quantity { get; set; }

            public Int32 AltUOMId { get; set; }
        }

        public class SalesInquiry
        {
            public string SrlNo { get; set; }
            public string OrderID { get; set; }
            public string ProductID { get; set; }
            public string Description { get; set; }
            public string Quantity { get; set; }
            public string UOM { get; set; }
            public string Warehouse { get; set; }
            public string StockQuantity { get; set; }
            public string StockUOM { get; set; }
            public string SalePrice { get; set; }
            public string Discount { get; set; }
            public string Amount { get; set; }
            public string TaxAmount { get; set; }
            public string TotalAmount { get; set; }
            public Int64 Quotation_No { get; set; }
            public string Quotation_Num { get; set; }
            public string Key_UniqueId { get; set; }
            public string Product_Shortname { get; set; }
            public string ProductName { get; set; }
            public string QuoteDetails_Id { get; set; }
            public int TaxAmountType { get; set; }

            // Rev  Manis 24428
            public string Order_AltQuantity { get; set; }
            public string Order_AltUOM { get; set; }
            // End  Manis 24428


        }
        class taxCode
        {
            public string Taxes_ID { get; set; }
            public string Taxes_Name { get; set; }
        }

        //#endregion

        //#endregion

        //#region Approval Section

        //#region PrePopulated Data If Page is not Post Back Section Start
        public void SetFinYearCurrentDate()
        {
            dt_PLQuote.EditFormatString = objConverter.GetDateFormat("Date");
            string fDate = null;

            //DateTime dt = DateTime.ParseExact("3/31/2016", "MM/dd/yyy", CultureInfo.InvariantCulture);
            string[] FinYEnd = Convert.ToString(Session["FinYearEnd"]).Split(' ');
            string FinYearEnd = FinYEnd[0];

            DateTime date3 = DateTime.ParseExact(FinYearEnd, "M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            ForJournalDate = Convert.ToString(date3);

            //ForJournalDate =Session["FinYearEnd"].ToString();
            int month = oDBEngine.GetDate().Month;
            int date = oDBEngine.GetDate().Day;
            int Year = oDBEngine.GetDate().Year;

            if (date3 < oDBEngine.GetDate().Date)
            {
                fDate = Convert.ToString(Convert.ToDateTime(ForJournalDate).Month) + "/" + Convert.ToString(Convert.ToDateTime(ForJournalDate).Day) + "/" + Convert.ToString(Convert.ToDateTime(ForJournalDate).Year);
            }
            else
            {
                fDate = Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Month) + "/" + Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Day) + "/" + Convert.ToString(Convert.ToDateTime(oDBEngine.GetDate().Date).Year);
            }

            dt_PLQuote.Value = DateTime.ParseExact(fDate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        public void GetFinacialYearBasedQouteDate()
        {
            String finyear = "";
            string setdate = null;
            if (Session["LastFinYear"] != null)
            {
                finyear = Convert.ToString(Session["LastFinYear"]).Trim();
                DataTable dtFinYear = objSlaesActivitiesBL.GetFinacialYearBasedQouteDate(finyear);
                if (dtFinYear != null && dtFinYear.Rows.Count > 0)
                {
                    Session["FinYearStartDate"] = Convert.ToString(dtFinYear.Rows[0]["finYearStartDate"]);
                    Session["FinYearEndDate"] = Convert.ToString(dtFinYear.Rows[0]["finYearEndDate"]);
                    if (Session["FinYearStartDate"] != null)
                    {
                        dt_PLQuote.MinDate = Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"]));
                    }
                    if (Session["FinYearEndDate"] != null)
                    {
                        dt_PLQuote.MaxDate = Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"]));
                    }
                    if (oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {

                    }
                    else if (oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date >= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {
                        setdate = Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Month) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Day) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearEndDate"]).Year);
                        dt_PLQuote.Value = DateTime.ParseExact(setdate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        //dt_PLQuote.Value = DateTime.ParseExact(Convert.ToString(Session["FinYearStartDate"]), @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else if (oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearStartDate"])) && oDBEngine.GetDate().Date <= Convert.ToDateTime(Convert.ToString(Session["FinYearEndDate"])))
                    {
                        setdate = Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Month) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Day) + "/" + Convert.ToString(Convert.ToDateTime(Session["FinYearStartDate"]).Year);
                        dt_PLQuote.Value = DateTime.ParseExact(setdate, @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                        //dt_PLQuote.Value = DateTime.ParseExact(Convert.ToString(Session["FinYearEndDate"]), @"M/d/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            //dt_PLQuote.Value = Convert.ToDateTime(oDBEngine.GetDate().ToString());
        }
        public void GetAllDropDownDetailForSalesQuotation(string userbranch)
        {
            //#region Schema Drop Down Start
            DataSet dst = new DataSet();
            dst = objSlaesActivitiesBL.GetAllDropDownDetailForSalesQuotation(userbranch);

            string strCompanyID = Convert.ToString(Session["LastCompany"]);
            string strBranchID = Convert.ToString(Session["userbranchID"]);
            string FinYear = Convert.ToString(Session["LastFinYear"]);
            string userbranchHierarchy = Convert.ToString(Session["userbranchHierarchy"]);

            DataTable Schemadt = objSlaesActivitiesBL.GetNumberingSchema(strCompanyID, userbranchHierarchy, FinYear, "163", "Y");
            if (Schemadt != null && Schemadt.Rows.Count > 0)
            {
                ddl_numberingScheme.DataTextField = "SchemaName";
                ddl_numberingScheme.DataValueField = "Id";
                ddl_numberingScheme.DataSource = Schemadt;
                ddl_numberingScheme.DataBind();
            }
            //#endregion Schema Drop Down Start

            //#region Branch Drop Down Start
            if (dst.Tables[1] != null && dst.Tables[1].Rows.Count > 0)
            {
                ddl_Branch.DataTextField = "branch_description";
                ddl_Branch.DataValueField = "branch_id";
                ddl_Branch.DataSource = dst.Tables[1];
                ddl_Branch.DataBind();
                //ddl_Branch.Items.Insert(0, new ListItem("Select", "0"));
            }
            if (Session["userbranchID"] != null)
            {
                if (ddl_Branch.Items.Count > 0)
                {
                    int branchindex = 0;
                    int cnt = 0;
                    foreach (ListItem li in ddl_Branch.Items)
                    {
                        if (li.Value == Convert.ToString(Session["userbranchID"]))
                        {
                            cnt = 1;
                            break;
                        }
                        else
                        {
                            branchindex += 1;
                        }
                    }
                    if (cnt == 1)
                    {
                        ddl_Branch.SelectedIndex = branchindex;
                    }
                    else
                    {
                        ddl_Branch.SelectedIndex = cnt;
                    }
                }
            }

            //#endregion Branch Drop Down End

            //#region Saleman DropDown Start
            //if (dst.Tables[2] != null && dst.Tables[2].Rows.Count > 0)
            //{
            //    ddl_SalesAgent.DataTextField = "Name";
            //    ddl_SalesAgent.DataValueField = "cnt_internalId";
            //    ddl_SalesAgent.DataSource = dst.Tables[2];
            //    ddl_SalesAgent.DataBind();
            //}
            //ddl_SalesAgent.Items.Insert(0, new ListItem("Select", "0"));
            //#endregion Saleman DropDown End

            //#region Currency Drop Down Start

            if (dst.Tables[3] != null && dst.Tables[3].Rows.Count > 0)
            {
                //ddl_Currency.DataTextField = "Currency_Name";
                //ddl_Currency.DataValueField = "Currency_ID";
                //ddl_Currency.DataSource = dst.Tables[3];
                //ddl_Currency.DataBind();
            }
            int currencyindex = 1;
            int no = 0;
            //if (Session["LocalCurrency"] != null)
            //{
            //    if (ddl_Currency.Items.Count > 0)
            //    {
            //        string[] ActCurrency = new string[] { };
            //        string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
            //        ActCurrency = currency.Split('~');
            //        if (Convert.ToString(ActCurrency[0]) == "1")
            //        {
            //            txt_Rate.ClientEnabled = false;
            //        }
            //        foreach (ListItem li in ddl_Currency.Items)
            //        {
            //            if (li.Value == Convert.ToString(ActCurrency[0]))
            //            {
            //                //ddl_Currency.Items.Remove(li);
            //                no = 1;
            //                break;
            //            }
            //            else
            //            {
            //                currencyindex += 1;
            //            }
            //        }
            //    }
            //    ddl_Currency.Items.Insert(0, new ListItem("Select", "0"));
            //    if (no == 1)
            //    {
            //        ddl_Currency.SelectedIndex = currencyindex;
            //    }
            //    else
            //    {
            //        ddl_Currency.SelectedIndex = no;
            //    }
            //}

            //#endregion Currency Drop Down End

            //#region TaxGroupType DropDown Start
            if (dst.Tables[4] != null && dst.Tables[4].Rows.Count > 0)
            {
                //ddl_AmountAre.TextField = "taxGrp_Description";
                //ddl_AmountAre.ValueField = "taxGrp_Id";
                //ddl_AmountAre.DataSource = dst.Tables[4];
                //ddl_AmountAre.DataBind();
            }
            //#endregion TaxGroupType DropDown Start
        }

        //#endregion PrePopulated Data If Page is not Post Back Section End

        //#region PrePopulated Data in Page Load Due to use Searching Functionality Section Start
        public void PopulateCustomerDetail()
        {
            if (Session["CustomerDetail"] == null)
            {
                DataTable dtCustomer = new DataTable();
                dtCustomer = objSlaesActivitiesBL.PopulateCustomerDetail();

                if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                {
                    //lookup_Customer.DataSource = dtCustomer; //Abhisek
                    //lookup_Customer.DataBind();              //Abhisek
                    Session["CustomerDetail"] = dtCustomer;
                }
            }
            else
            {
                //lookup_Customer.DataSource = (DataTable)Session["CustomerDetail"];  //Abhisek
                //lookup_Customer.DataBind();                                         //Abhisek                          
            }

        }
        //#endregion PrePopulated Data in Page Load Due to use Searching Functionality Section End

        //#region Header Portion Detail of the Page By Sam

        //#region Check Billing and Shipping Address

        [WebMethod]
        public static int CheckCustomerBillingShippingAddress(string Customerid)
        {
            int addressStatus = 0;
            try
            {
                CRMSalesDtlBL objCRMSalesDtlBL = new CRMSalesDtlBL();
                addressStatus = objCRMSalesDtlBL.CheckCustomerBillingShippingAddress(Customerid);
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return addressStatus;
        }

        //#endregion Check Billing and Shipping Address

        [WebMethod]
        public static bool CheckUniqueCode(string QuoteNo)
        {
            bool flag = false;
            BusinessLogicLayer.GenericMethod oGenericMethod = new BusinessLogicLayer.GenericMethod();
            try
            {
                MShortNameCheckingBL objShortNameChecking = new MShortNameCheckingBL();
                flag = objShortNameChecking.CheckUnique(QuoteNo, "0", "Quotation");
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
            return flag;
        }




        [WebMethod]
        public static string GetCurrentConvertedRate(string CurrencyId)
        {

            string[] ActCurrency = new string[] { };

            string CompID = "";
            if (HttpContext.Current.Session["LastCompany"] != null)
            {
                CompID = Convert.ToString(HttpContext.Current.Session["LastCompany"]);


            }
            string currentRate = "";
            if (HttpContext.Current.Session["LocalCurrency"] != null)
            {
                string currency = Convert.ToString(HttpContext.Current.Session["LocalCurrency"]);
                ActCurrency = currency.Split('~');
                int BaseCurrencyId = Convert.ToInt32(ActCurrency[0]);
                int ConvertedCurrencyId = Convert.ToInt32(CurrencyId);
                SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
                DataTable dt = objSlaesActivitiesBL.GetCurrentConvertedRate(BaseCurrencyId, ConvertedCurrencyId, CompID);
                if (dt != null && dt.Rows.Count > 0)
                {
                    currentRate = Convert.ToString(dt.Rows[0]["SalesRate"]);
                    return currentRate;
                }
            }
            return null;

        }
        protected void cmbContactPerson_Callback(object sender, CallbackEventArgsBase e)
        {
            Session["QuotationAddressDtl"] = null;
            Session["BillingAddressLookup"] = null;
            Session["ShippingAddressLookup"] = null;
            string WhichCall = e.Parameter.Split('~')[0];
            if (WhichCall == "BindContactPerson")
            {
                string InternalId = Convert.ToString(Convert.ToString(e.Parameter.Split('~')[1]));
                PopulateContactPersonOfCustomer(InternalId);
            }
        }
        //Rev Rajdip
        [WebMethod(EnableSession = true)]
        public static object GetSalesManAgent(string SearchKey, string CustomerId)
        {
            List<SalesManAgntModel> listSalesMan = new List<SalesManAgntModel>();
            string Mode = Convert.ToString(HttpContext.Current.Session["key_QutId"]);
            if (HttpContext.Current.Session["userid"] != null)
            {

                //  BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                DataTable dtexistcheckfrommaptable = oDBEngine.GetDataTable("SELECT * FROM tbl_Salesman_Entitymap WHERE CustomerId=(Select cnt_id from tbl_master_contact WHERE cnt_internalId='" + CustomerId + "' )");
                if (dtexistcheckfrommaptable.Rows.Count > 0)
                {
                    if (Mode != "ADD")
                    {
                        DataTable cust = new DataTable();
                        ProcedureExecute proc = new ProcedureExecute("prc_GetQuotationmappedSalesMan");
                        proc.AddVarcharPara("@CustomerID", 500, CustomerId);
                        proc.AddVarcharPara("@SearchKey", 500, SearchKey);
                        proc.AddVarcharPara("@Action", 500, "Edit");
                        proc.AddVarcharPara("@EstimateCostID", 500, Mode);
                        cust = proc.GetTable();

                        listSalesMan = (from DataRow dr in cust.Rows
                                        select new SalesManAgntModel()
                                        {
                                            id = dr["SalesmanId"].ToString(),
                                            Na = dr["Name"].ToString()
                                        }).ToList();
                    }
                    else
                    {
                        DataTable cust = new DataTable();
                        ProcedureExecute proc = new ProcedureExecute("prc_GetQuotationmappedSalesMan");
                        proc.AddVarcharPara("@Action", 500, "Add");
                        proc.AddVarcharPara("@CustomerID", 500, CustomerId);
                        proc.AddVarcharPara("@SearchKey", 500, SearchKey);

                        cust = proc.GetTable();

                        listSalesMan = (from DataRow dr in cust.Rows
                                        select new SalesManAgntModel()
                                        {
                                            id = dr["SalesmanId"].ToString(),
                                            Na = dr["Name"].ToString()
                                        }).ToList();

                    }

                }
                else
                {

                    DataTable cust = oDBEngine.GetDataTable("select top 10 cnt_id ,Name from v_GetAllSalesManAgent where  Name like '%" + SearchKey + "%'");


                    listSalesMan = (from DataRow dr in cust.Rows
                                    select new SalesManAgntModel()
                                    {
                                        id = dr["cnt_id"].ToString(),
                                        Na = dr["Name"].ToString()
                                    }).ToList();

                }

            }

            return listSalesMan;
        }
        public class GetSalesMan
        {
            public int Id { get; set; }
            public string Name { get; set; }

            //  public string Ifexists { get; set; }

        }
        [WebMethod]
        public static object MappedSalesManOnetoOne(string Id)
        {

            DataTable dtContactPerson = new DataTable();
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            List<GetSalesMan> GetSalesMan = new List<GetSalesMan>();
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_GetOneToOnemappedCustomer");
            proc.AddVarcharPara("@CustomerID", 500, Id);
            dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    GetSalesMan.Add(new GetSalesMan
                    {
                        Id = Convert.ToInt32(dt.Rows[i]["SalesmanId"]),
                        Name = Convert.ToString(dt.Rows[i]["Name"])
                    });
                }
            }
            return GetSalesMan;
        }
        //End Rev Rajdip

        [WebMethod]
        public static object GetEntityType(string Id)
        {
            string output = "O";
            DataTable dtEntity = new DataTable();
            DBEngine obj = new DBEngine();

            dtEntity = obj.GetDataTable("select ISNULL(CNT_TAX_ENTITYTYPE,'O') from tbl_master_contact where cnt_internalId='" + Convert.ToString(Id) + "'");

            if (dtEntity != null && dtEntity.Rows.Count > 0)
            {
                output = Convert.ToString(dtEntity.Rows[0][0]);
            }

            return output;

        }


        public class SalesManAgntModel
        {
            public string id { get; set; }
            public string Na { get; set; }
        }
        protected void PopulateContactPersonOfCustomer(string InternalId)
        {
            string ContactPerson = "";
            DataTable dtContactPerson = new DataTable();
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            dtContactPerson = objSlaesActivitiesBL.PopulateContactPersonOfCustomer(InternalId);
            if (dtContactPerson != null && dtContactPerson.Rows.Count > 0)
            {
                cmbContactPerson.TextField = "contactperson";
                cmbContactPerson.ValueField = "add_id";
                cmbContactPerson.DataSource = dtContactPerson;
                cmbContactPerson.DataBind();
                foreach (DataRow dr in dtContactPerson.Rows)
                {
                    if (Convert.ToString(dr["Isdefault"]) == "True")
                    {
                        ContactPerson = Convert.ToString(dr["add_id"]);
                        break;
                    }
                }
                cmbContactPerson.SelectedItem = cmbContactPerson.Items.FindByValue(ContactPerson);

            }
            //dtContactPerson = objSlaesActivitiesBL.PopulateContactPersonForCustomerOrLead(InternalId);
            //oGenericMethod = new BusinessLogicLayer.GenericMethod();

            //DataTable dtCmb = new DataTable();
            //dtCmb = oGenericMethod.GetDataTable("Select city_id,city_name From tbl_master_city Where state_id=" + stateID + " Order By city_name");//+ " Order By state "
            //AspxHelper oAspxHelper = new AspxHelper();
            //if (dtCmb.Rows.Count > 0)
            //{
            //    //CmbState.Enabled = true;
            //    // oAspxHelper.Bind_Combo(CmbCity, dtCmb, "city_name", "city_id", 0);
            //}
            //else
            //{
            //    //CmbCity.Enabled = false;
            //}
        }
        protected void ddl_VatGstCst_Callback(object sender, CallbackEventArgsBase e)
        {
            string type = e.Parameter.Split('~')[0];
            PopulateGSTCSTVAT(type);
        }
        protected void PopulateGSTCSTVAT(string type)
        {
            DataTable dtGSTCSTVAT = new DataTable();
            SlaesActivitiesBL objSlaesActivitiesBL = new SlaesActivitiesBL();
            if (type == "2")
            {
                dtGSTCSTVAT = objSlaesActivitiesBL.PopulateGSTCSTVAT(dt_PLQuote.Date.ToString("yyyy-MM-dd"));

                //#region Delete Igst,Cgst,Sgst respectively

                string CompInternalId = Convert.ToString(Session["LastCompany"]);
                string BranchId = Convert.ToString(Session["userbranchID"]);
                string[] compGstin = oDBEngine.GetFieldValue1("tbl_master_company", "cmp_gstin", "cmp_internalid='" + CompInternalId + "'", 1);
                string[] branchGstin = oDBEngine.GetFieldValue1("tbl_master_branch", "isnull(branch_GSTIN,'')branch_GSTIN", "branch_id='" + BranchId + "'", 1);
                String GSTIN = "";

                if (branchGstin[0].Trim() != "")
                {
                    GSTIN = branchGstin[0].Trim();
                }
                else
                {
                    if (compGstin.Length > 0)
                    {
                        GSTIN = compGstin[0].Trim();
                    }
                }

                string ShippingState = "";

                //#region ##### Added By : Samrat Roy -- For BillingShippingUserControl ######
                string sstateCode = BillingShippingControl.GetShippingStateId();
                ShippingState = sstateCode;
                //if (ShippingState.Trim() != "")
                //{
                //    ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                //}
                //#region ##### Old Code -- BillingShipping ######
                ////if (chkBilling.Checked)
                ////{
                ////    if (CmbState.Value != null)
                ////    {
                ////        ShippingState = CmbState.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                ////else
                ////{
                ////    if (CmbState1.Value != null)
                ////    {
                ////        ShippingState = CmbState1.Text;
                ////        ShippingState = ShippingState.Substring(ShippingState.IndexOf("(State Code:")).Replace("(State Code:", "").Replace(")", "");
                ////    }
                ////}
                //#endregion

                //#endregion

                if (ShippingState.Trim() != "" && GSTIN.Trim() != "")
                {

                    if (GSTIN.Substring(0, 2) == ShippingState)
                    {
                        //Check if the state is in union territories then only UTGST will apply
                        //   Chandigarh     Andaman and Nicobar Islands    DADRA & NAGAR HAVELI    DAMAN & DIU     Lakshadweep              PONDICHERRY
                        if (ShippingState == "4" || ShippingState == "26" || ShippingState == "25" || ShippingState == "35" || ShippingState == "31" || ShippingState == "34")
                        {
                            foreach (DataRow dr in dtGSTCSTVAT.Rows)
                            {
                                if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S")
                                {
                                    dr.Delete();
                                }
                            }

                        }
                        else
                        {
                            foreach (DataRow dr in dtGSTCSTVAT.Rows)
                            {
                                if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U")
                                {
                                    dr.Delete();
                                }
                            }
                        }
                        dtGSTCSTVAT.AcceptChanges();
                    }
                    else
                    {
                        foreach (DataRow dr in dtGSTCSTVAT.Rows)
                        {
                            if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "C" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U")
                            {
                                dr.Delete();
                            }
                        }
                        dtGSTCSTVAT.AcceptChanges();

                    }


                }

                //If Company GSTIN is blank then Delete All CGST,UGST,IGST,CGST
                if (GSTIN.Trim() == "")
                {
                    foreach (DataRow dr in dtGSTCSTVAT.Rows)
                    {
                        if (Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "C" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "S" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "U" || Convert.ToString(dr["TaxRates_GSTtype"]).Trim() == "I")
                        {
                            dr.Delete();
                        }
                    }
                    dtGSTCSTVAT.AcceptChanges();
                }


                //#endregion


                if (dtGSTCSTVAT != null && dtGSTCSTVAT.Rows.Count > 0)
                {
                    ddl_VatGstCst.TextField = "Taxes_Name";
                    ddl_VatGstCst.ValueField = "Taxes_ID";
                    ddl_VatGstCst.DataSource = dtGSTCSTVAT;
                    ddl_VatGstCst.DataBind();
                    ddl_VatGstCst.SelectedIndex = 0;
                }
            }
            else
            {
                ddl_VatGstCst.DataSource = null;
                ddl_VatGstCst.DataBind();
            }
        }
        public int Sendmail_SalesQuotation(string Output)
        {

            int stat = 0;

            Employee_BL objemployeebal = new Employee_BL();
            DataTable dt2 = new DataTable();
            dt2 = objemployeebal.GetSystemsettingmail("Show Email in Quotation");
            if (Convert.ToString(dt2.Rows[0]["Variable_Value"]) == "Yes")
            {


                ExceptionLogging mailobj = new ExceptionLogging();
                EmailSenderHelperEL emailSenderSettings = new EmailSenderHelperEL();
                DataTable dt_EmailConfig = new DataTable();
                DataTable dt_EmailConfigpurchase = new DataTable();

                DataTable dt_Emailbodysubject = new DataTable();
                QuotationEmailTags fetchModel = new QuotationEmailTags();
                string Subject = "";
                string Body = "";
                string emailTo = "";

                int MailStatus = 0;


                //var customerid = lookup_Customer.GridView.GetRowValues(lookup_Customer.GridView.FocusedRowIndex, lookup_Customer.KeyFieldName).ToString(); //Abhisek
                var customerid = hdnCustomerId.Value;

                //   var customerid = cmbContactPerson.Value.ToString();

                dt_EmailConfig = objemployeebal.Getemailids(customerid);
                // string FilePath = ConfigurationManager.AppSettings["ReportPOpdf"].ToString() + "PO-Default-" + Output + ".pdf";
                //string FilePath = Server.MapPath("~/Reports/RepxReportDesign/PurchaseOrder/DocDesign/PDFFiles/" + "PO-Default-" + Output + ".pdf");
                //string FileName = FilePath;


                string FilePath = string.Empty;
                string path = System.Web.HttpContext.Current.Server.MapPath("~");
                string path1 = string.Empty;
                string DesignPath = "";
                string fullpath = Server.MapPath("~");

                if (ConfigurationManager.AppSettings["IsDevelopedZone"] != null)
                {
                    FilePath = Server.MapPath("~/Reports/Reports/RepxReportDesign/Proforma/DocDesign/PDFFiles/" + "Quotation-" + Output + ".pdf");
                }
                else
                {
                    FilePath = Server.MapPath("~/Reports/RepxReportDesign/Proforma/DocDesign/PDFFiles/" + "Quotation-" + Output + ".pdf");
                }
                FilePath = FilePath.Replace("ERP.UI\\", "");





                string FileName = FilePath;
                if (dt_EmailConfig.Rows.Count > 0)
                {
                    emailTo = Convert.ToString(dt_EmailConfig.Rows[0]["eml_email"]);
                    //  emailTo = "sayan.dutta@indusnet.co.in";
                    dt_Emailbodysubject = objemployeebal.Getemailtemplates("12");  //for purchase order

                    if (dt_Emailbodysubject.Rows.Count > 0)
                    {
                        Body = Convert.ToString(dt_Emailbodysubject.Rows[0]["body"]);
                        Subject = Convert.ToString(dt_Emailbodysubject.Rows[0]["subjct"]);

                        dt_EmailConfigpurchase = objemployeebal.Getemailtagsforpurchase(Output, "QuotationEmailTags");  //For Purchase Order Get all Tags Value

                        if (dt_EmailConfigpurchase.Rows.Count > 0)
                        {

                            fetchModel = DbHelpers.ToModel<QuotationEmailTags>(dt_EmailConfigpurchase);

                            Body = Employee_BL.GetFormattedString<QuotationEmailTags>(fetchModel, Body);
                            Subject = Employee_BL.GetFormattedString<QuotationEmailTags>(fetchModel, Subject);


                        }

                        emailSenderSettings = mailobj.GetEmailSettingsforAllreport(emailTo, "", "", FilePath, Body, Subject);

                        if (emailSenderSettings.IsSuccess)
                        {
                            string Message = "";
                            // checkmailId = new SendEmailUL().CheckMailIdExistence(emailSenderSettings.ModelCast<EmailSenderEL>());
                            //if (checkmailId == true)
                            //{
                            EmailSenderEL obj2 = new EmailSenderEL();
                            stat = SendEmailUL.sendMailInHtmlFormat(emailSenderSettings.ModelCast<EmailSenderEL>(), out Message);
                            //}


                        }
                    }
                }



            }
            return stat;
        }

        protected void EntityServerModeDataProjectQuotation_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {

            e.KeyExpression = "Proj_Id";

            //string connectionString = ConfigurationManager.ConnectionStrings["crmConnectionString"].ConnectionString;
            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);

            ERPDataClassesDataContext dc = new ERPDataClassesDataContext(connectionString);

            string Userid = Convert.ToString(HttpContext.Current.Session["userid"]);
            BusinessLogicLayer.DBEngine BEngine = new BusinessLogicLayer.DBEngine();



            // Mantis Issue 24976
            //var q = from d in dc.V_ProjectLists
            //        where d.ProjectStatus == "Approved" && d.ProjBracnchid == Convert.ToInt64(ddl_Branch.SelectedValue) && d.CustId == Convert.ToString(hdnCustomerId.Value)
            //        orderby d.Proj_Id descending
            //        select d;

            //e.QueryableSource = q;

            CommonBL cbl = new CommonBL();
            string ISProjectIndependentOfBranch = cbl.GetSystemSettingsResult("AllowProjectIndependentOfBranch");

            if (ISProjectIndependentOfBranch == "No")
            {
                var q = from d in dc.V_ProjectLists
                        where d.ProjectStatus == "Approved" && d.ProjBracnchid == Convert.ToInt64(ddl_Branch.SelectedValue) && d.CustId == Convert.ToString(hdnCustomerId.Value)
                        orderby d.Proj_Id descending
                        select d;

                e.QueryableSource = q;
            }
            else
            {
                var q = from d in dc.V_ProjectLists
                        where d.ProjectStatus == "Approved" && d.CustId == Convert.ToString(hdnCustomerId.Value)
                        orderby d.Proj_Id descending
                        select d;

                e.QueryableSource = q;
            }
            // End of Mantis Issue 24976


        }

        public void bindHierarchy()
        {
            DataTable hierarchydt = oDBEngine.GetDataTable("select 0 as ID ,'Select' as H_Name union select ID,H_Name from V_HIERARCHY");
            if (hierarchydt != null && hierarchydt.Rows.Count > 0)
            {
                ddlHierarchy.DataTextField = "H_Name";
                ddlHierarchy.DataValueField = "ID";
                ddlHierarchy.DataSource = hierarchydt;
                ddlHierarchy.DataBind();
            }
        }
        //#endregion

        [WebMethod]
        public static string SetSessionPacking(string list)
        {
            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            List<ProductQuantity> finalResult = jsSerializer.Deserialize<List<ProductQuantity>>(list);
            //var result = JsonConvert.DeserializeObject<ProductQuantity>(list);
            HttpContext.Current.Session["SessionPackingDetails"] = finalResult;
            return null;

        }

        [WebMethod]
        public static object DocWiseSimilarProjectCheck(string quote_Id)
        {
            string returnValue = "0";
            if (HttpContext.Current.Session["userid"] != null)
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                //quote_Id = quote_Id.Replace("'", "''");

                DataTable dtMainAccount = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("[Prc_ProjectQuotation_Details]");
                proc.AddVarcharPara("@Action", 100, "GetSimilarProjectCheck");
                proc.AddVarcharPara("@ProductList", 4000, quote_Id);
                proc.AddVarcharPara("@ReturnValue", 50, "0", QueryParameterDirection.Output);
                proc.RunActionQuery();
                returnValue = Convert.ToString(proc.GetParaValue("@ReturnValue"));

            }
            return returnValue;

        }
        [WebMethod]
        public static object SetProjectCode(Int64 OrderId, string TagDocType)
        {
            List<ProjectDocumentDetails> Detail = new List<ProjectDocumentDetails>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                ProcedureExecute proc = new ProcedureExecute("Prc_ProjectQuotation_Details");
                proc.AddVarcharPara("@Action", 500, "ProjectSalesInquirytaggingProjectdata");
                proc.AddBigIntegerPara("@Order_Id", OrderId);
                //proc.AddVarcharPara("@TagDocType", 500, TagDocType);
                DataTable address = proc.GetTable();



                Detail = (from DataRow dr in address.Rows
                          select new ProjectDocumentDetails()

                          {
                              ProjectId = Convert.ToInt64(dr["ProjectId"]),
                              ProjectCode = Convert.ToString(dr["ProjectCode"])
                          }).ToList();
                return Detail;

            }
            return null;

        }


        [WebMethod]
        public static int Duplicaterevnumbercheck(string RevNo, string Order)
        {
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
            int returnVal = 0;
            CRMSalesOrderDtlBL objCRMSalesOrderDtlBL = new CRMSalesOrderDtlBL();
            DataTable dtrev = new DataTable();
            DataTable OrderNumber = new DataTable();
            OrderNumber = oDBEngine.GetDataTable("select ISNULL(EstimateCost_Number,'') EstimateCost_Number from trans_EstimateCostSheet  where EstimateCost_Id='" + Order + "'");
            string orderno;
            if (OrderNumber.Rows.Count > 0)
            {
                orderno = Convert.ToString(OrderNumber.Rows[0]["EstimateCost_Number"]);
            }
            else
            {
                orderno = "";
            }
            //dtrev = objCRMSalesOrderDtlBL.DuplicateRevisionNumber(RevNo, Convert.ToString(OrderNumber.Rows[0]["Order_Number"]));	
            //dtrev = objCRMSalesOrderDtlBL.DuplicateRevisionNumber(RevNo, Convert.ToString(OrderNumber.Rows[0]["Order_Number"]));	
            dtrev = oDBEngine.GetDataTable("select isnull(ECS_RevisionNo,'')  ECS_RevisionNo from trans_EstimateCostSheet where  EstimateCost_Number='" + orderno + "'");
            //if (dtrev.Rows.Count > 0)	
            //{	
            //    returnVal = 1;	
            //}	
            if (dtrev.Rows.Count > 0)
            {
                for (int i = 0; i < dtrev.Rows.Count; i++)
                {
                    if (dtrev.Rows[i]["ECS_RevisionNo"].ToString() == RevNo)
                    {
                        returnVal = 1;
                    }
                }
            }
            return returnVal;
        }
        [WebMethod]
        public static string SetApproveReject(string ApproveRemarks, int ApproveRejStatus, string OrderId)
        {
            string returnValue = "";
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 200, "SalesQuotationApproveRejectDetails");
            proc.AddVarcharPara("@Project_ApproveRejectREmarks", 5000, ApproveRemarks);
            proc.AddIntegerPara("@Project_ApproveStatus", ApproveRejStatus);
            proc.AddVarcharPara("@Order_Id", 10, OrderId);
            proc.AddVarcharPara("@ReturnValue", 50, "0", QueryParameterDirection.Output);
            proc.RunActionQuery();
            returnValue = Convert.ToString(proc.GetParaValue("@ReturnValue"));
            return returnValue;

        }
        [WebMethod]
        public static object GetApproveRejectStatus(string OrderId)
        {
            int status = 0;

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("prc_EstimateCostSheetEditDetails");
            proc.AddVarcharPara("@Action", 200, "GetSalesEstimateCostSheetApproveDetails");
            proc.AddVarcharPara("@EstimateCostID", 10, OrderId);
            dt = proc.GetTable();
            status = Convert.ToInt32(dt.Rows[0]["ECS_ApproveStatus"]);
            return status;
        }
        [WebMethod]
        public static string DeleteTaxForShipPartyChange(string UniqueVal)
        {
            DataTable dt = new DataTable();
            if (HttpContext.Current.Session["FinalTaxRecord" + Convert.ToString(UniqueVal)] != null)
            {
                dt = (DataTable)HttpContext.Current.Session["FinalTaxRecord" + Convert.ToString(UniqueVal)];
                dt.Rows.Clear();
                //HttpContext.Current.Session["FinalTaxRecord"]=null;
                HttpContext.Current.Session["FinalTaxRecord" + Convert.ToString(UniqueVal)] = dt;
            }


            return null;

        }

    }
    
    public class ProductsDEtailsQuantity
    {
        public Int64 productid { get; set; }
        public Int32 slno { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal packing { get; set; }

        public Int32 PackingUom { get; set; }
        public Int32 PackingSelectUom { get; set; }
    }
   

}
