﻿using BusinessLogicLayer;
using DevExpress.Web;
using DevExpress.Web.Data;
using EntityLayer.CommonELS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERP.OMS.Management.Activities
{
    public partial class VendorRateDiffAdjustmentInvoice : System.Web.UI.Page
    {
        VendorPaymentAdjustmentBL blLayer = new VendorPaymentAdjustmentBL();
        public EntityLayer.CommonELS.UserRightsForPage rights = new UserRightsForPage();

        DataTable TempTable = new DataTable();
        string adjustmentNumber = "";
        int adjustmentId = 0, ErrorCode = 0;
        string AdjustID = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/Management/Activities/VendorRateDiffAdjustmentInvoiceList.aspx");

                if (Request.QueryString["Key"] != "Add")
                {
                    string AdjId = Request.QueryString["Key"];
                    EditModeExecute(AdjId);
                    hdAddEdit.Value = "Edit";
                    lblHeading.Text = "Modify Adjustment of Documents - Rate difference With Invoice";
                    hdAdjustmentId.Value = AdjId;
                }
                else
                {
                    hdAddEdit.Value = "Add";
                    AddmodeExecuted();
             
                    Session["AdjustmentRateDiffDetailTable"] = null;
                }
                if (Request.QueryString["req"] != null && Request.QueryString["req"] == "V")
                {
                    lblHeading.Text = "View Adjustment of Documents - Rate Difference With Invoice";
                    btn_SaveRecords.Visible = false;
                    btnSaveRecords.Visible = false;
                }  
            }

        }

        private void AddmodeExecuted()
        {

            DataSet allDetails = blLayer.PopulateVendorRateDifferenceAdjustDetails();
            CmbScheme.DataSource = allDetails.Tables[0];
            CmbScheme.ValueField = "Id";
            CmbScheme.TextField = "SchemaName";
            CmbScheme.DataBind();

            ddlBranch.DataSource = allDetails.Tables[1];
            ddlBranch.DataValueField = "branch_id";
            ddlBranch.DataTextField = "branch_description";
            ddlBranch.DataBind();

            DateTime startDate = Convert.ToDateTime(allDetails.Tables[2].Rows[0]["FinYear_StartDate"]);
            DateTime LastDate = Convert.ToDateTime(allDetails.Tables[2].Rows[0]["FinYear_EndDate"]);
            dtTDate.MaxDate = LastDate;
            dtTDate.MinDate = startDate;

            if (DateTime.Now > LastDate)
                dtTDate.Date = LastDate;
            else
                dtTDate.Date = DateTime.Now;

            if (Session["schemavalRateDiffAdj"] != null)
            {
                List<string> SaveNewValues = (Session["schemavalRateDiffAdj"]) as List<string>;
                CmbScheme.Value = SaveNewValues[0];
                CmbScheme.Text = SaveNewValues[1];
                ddlBranch.SelectedValue = SaveNewValues[2];
                ddlBranch.SelectedItem.Text = SaveNewValues[3];
                ddlBranch.Enabled = false;         

            }
            if (Convert.ToString(Session["SaveModeRateDiffAdj"]) == "A")
            {
                dtTDate.Focus();
                txtVoucherNo.ClientEnabled = false;
                txtVoucherNo.Text = "Auto";
            }
            else if (Convert.ToString(Session["SaveModeRateDiffAdj"]) == "M")
            {
                txtVoucherNo.ClientEnabled = true;
                txtVoucherNo.Text = "";
                txtVoucherNo.Focus();

            }
        }


        public void EditModeExecute(string id)
        {
            DataSet EditedDataDetails = blLayer.GetRateDifferenceEditedData(id);
            CmbScheme.DataSource = EditedDataDetails.Tables[0];
            CmbScheme.ValueField = "Id";
            CmbScheme.TextField = "SchemaName";
            CmbScheme.DataBind();

            ddlBranch.DataSource = EditedDataDetails.Tables[1];
            ddlBranch.DataValueField = "branch_id";
            ddlBranch.DataTextField = "branch_description";
            ddlBranch.DataBind();

            DateTime startDate = Convert.ToDateTime(EditedDataDetails.Tables[2].Rows[0]["FinYear_StartDate"]);
            DateTime LastDate = Convert.ToDateTime(EditedDataDetails.Tables[2].Rows[0]["FinYear_EndDate"]);
            dtTDate.MaxDate = LastDate;
            dtTDate.MinDate = startDate;

            DataRow HeaderRow = EditedDataDetails.Tables[3].Rows[0];
            CmbScheme.Text = Convert.ToString(HeaderRow["SchemaName"]);
            CmbScheme.ClientEnabled = false;
            txtVoucherNo.Text = Convert.ToString(HeaderRow["Adjustment_No"]);
            dtTDate.Date = Convert.ToDateTime(HeaderRow["Adjustment_Date"]);
            dtTDate.ClientEnabled = false;
            ddlBranch.SelectedValue = Convert.ToString(HeaderRow["Branch"]);
            ddlBranch.Enabled = false;
            txtVendName.Text = Convert.ToString(HeaderRow["CustName"]);
            txtVendName.ClientEnabled = false;
            hdnVendorId.Value = Convert.ToString(HeaderRow["Vendor_Id"]);
            btntxtDocNo.Text = Convert.ToString(HeaderRow["Adjusted_Doc_no"]);
            btntxtDocNo.ClientEnabled = false;
            hdAdvanceDocNo.Value = Convert.ToString(HeaderRow["Adjusted_doc_id"]);
            DocAmt.Text = Convert.ToString(HeaderRow["Adjusted_DocAmt"]);
            ExchRate.Text = Convert.ToString(HeaderRow["ExchangeRate"]);
            BaseAmt.Text = Convert.ToString(HeaderRow["Adjusted_DocAmt_inBaseCur"]);
            Remarks.Text = Convert.ToString(HeaderRow["Remarks"]);
            OsAmt.Text = Convert.ToString(HeaderRow["Adjusted_DocOSAmt"]);
            AdjAmt.Text = Convert.ToString(HeaderRow["Adjusted_Amount"]);
            hdAdjustmentType.Value = Convert.ToString(HeaderRow["AdvType"]);
            TempTable = EditedDataDetails.Tables[4];
            Session["AdjustmentRateDiffDetailTable"] = EditedDataDetails.Tables[4];
            // grid.DataSource = EditedDataDetails.Tables[4];
            grid.DataBind();
        }


        #region Grid Event
        protected void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            if (e.Column.FieldName != "AdjAmt")
            {
                e.Editor.Enabled = true;
            }
            else
            {
                e.Editor.ReadOnly = false;
            }
        }


        protected void grid_BatchUpdate(object sender, DevExpress.Web.Data.ASPxDataBatchUpdateEventArgs e)
        {
            DataTable AdjustmentTable = new DataTable();
            AdjustmentTable.Columns.Add("DocNo", typeof(string));
            AdjustmentTable.Columns.Add("DocAmt", typeof(decimal));
            AdjustmentTable.Columns.Add("Currency", typeof(string));
            AdjustmentTable.Columns.Add("ExchangeRate", typeof(string));
            AdjustmentTable.Columns.Add("LocalAmt", typeof(decimal));
            AdjustmentTable.Columns.Add("OsAmt", typeof(decimal));
            AdjustmentTable.Columns.Add("AdjAmt", typeof(decimal));
            AdjustmentTable.Columns.Add("RemainingBalance", typeof(decimal));
            AdjustmentTable.Columns.Add("DocumentId", typeof(string));
            AdjustmentTable.Columns.Add("DocumentType", typeof(string));

            DataRow NewRow;
            foreach (var args in e.InsertValues)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(args.NewValues["DocNo"])))
                {
                    NewRow = AdjustmentTable.NewRow();
                    NewRow["DocNo"] = Convert.ToString(args.NewValues["DocNo"]);
                    NewRow["DocAmt"] = Convert.ToDecimal(args.NewValues["DocAmt"]);
                    NewRow["Currency"] = Convert.ToString(args.NewValues["Currency"]);
                    NewRow["ExchangeRate"] = Convert.ToString(args.NewValues["ExchangeRate"]);
                    NewRow["LocalAmt"] = Convert.ToDecimal(args.NewValues["LocalAmt"]);
                    NewRow["OsAmt"] = Convert.ToDecimal(args.NewValues["OsAmt"]);
                    NewRow["AdjAmt"] = Convert.ToDecimal(args.NewValues["AdjAmt"]);
                    NewRow["RemainingBalance"] = Convert.ToDecimal(args.NewValues["RemainingBalance"]);
                    NewRow["DocumentId"] = Convert.ToString(args.NewValues["DocumentId"]);
                    NewRow["DocumentType"] = Convert.ToString(args.NewValues["DocumentType"]);
                    AdjustmentTable.Rows.Add(NewRow);
                }
            }

            foreach (var args in e.UpdateValues)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(args.NewValues["DocNo"])))
                {
                    NewRow = AdjustmentTable.NewRow();
                    NewRow["DocNo"] = Convert.ToString(args.NewValues["DocNo"]);
                    NewRow["DocAmt"] = Convert.ToDecimal(args.NewValues["DocAmt"]);
                    NewRow["Currency"] = Convert.ToString(args.NewValues["Currency"]);
                    NewRow["ExchangeRate"] = Convert.ToString(args.NewValues["ExchangeRate"]);
                    NewRow["LocalAmt"] = Convert.ToDecimal(args.NewValues["LocalAmt"]);
                    NewRow["OsAmt"] = Convert.ToDecimal(args.NewValues["OsAmt"]);
                    NewRow["AdjAmt"] = Convert.ToDecimal(args.NewValues["AdjAmt"]);
                    NewRow["RemainingBalance"] = Convert.ToDecimal(args.NewValues["RemainingBalance"]);
                    NewRow["DocumentId"] = Convert.ToString(args.NewValues["DocumentId"]);
                    NewRow["DocumentType"] = Convert.ToString(args.NewValues["DocumentType"]);
                    AdjustmentTable.Rows.Add(NewRow);
                }
            }



            foreach (var args in e.DeleteValues)
            {
                DataTable AdjTable = (DataTable)Session["AdjustmentRateDiffDetailTable"];
                if (AdjTable != null)
                {
                    string delId = Convert.ToString(args.Keys[0]);
                    DataRow[] AdjTableRow = AdjTable.Select("SrlNo='" + delId + "'");
                    DataRow[] delRow = AdjustmentTable.Select("DocumentId='" + AdjTableRow[0]["DocumentId"] + "' and DocumentType='" + AdjTableRow[0]["DocumentType"] + "'");
                    foreach (DataRow dr in delRow)
                        dr.Delete();

                    AdjustmentTable.AcceptChanges();
                }
            }
            TempTable = AdjustmentTable.Copy();
            RefetchSrlNo();

            blLayer.AddEditRateDifferenceAdjustment(hdAddEdit.Value, CmbScheme.Value.ToString().Split('~')[0], txtVoucherNo.Text, dtTDate.Date.ToString("yyyy-MM-dd"),
                ddlBranch.SelectedValue, hdnVendorId.Value, hdAdvanceDocNo.Value, btntxtDocNo.Text, DocAmt.Text,
                ExchRate.Text, BaseAmt.Text, Remarks.Text, OsAmt.Text, AdjAmt.Text, Convert.ToString(Session["userid"]), ref adjustmentId,
                ref adjustmentNumber, AdjustmentTable, ref ErrorCode, Request.QueryString["Key"], hdAdjustmentType.Value);

            grid.JSProperties["cpadjustmentNumber"] = adjustmentNumber;
            grid.JSProperties["cpErrorCode"] = Convert.ToString(ErrorCode);
            grid.JSProperties["cpadjustmentId"] = Convert.ToString(adjustmentId);
            e.Handled = true;

            #region To Show By Default Cursor after SAVE AND NEW
            if (hdAddEdit.Value == "Add")
            {
                if(HiddenSaveButton.Value != "E")
                {
                    string schemavalue = CmbScheme.Value.ToString();
                    string NumberingScheme = CmbScheme.Text;
                    string BranchID = ddlBranch.SelectedValue;
                    string BranchName = ddlBranch.SelectedItem.Text;

                    List<string> AfterAdd = new List<string> { schemavalue, NumberingScheme, BranchID, BranchName };

                    Session["schemavalRateDiffAdj"] = AfterAdd;

                    string schematype = txtVoucherNo.Text;
                    if (schematype == "Auto")
                    {
                        Session["SaveModeRateDiffAdj"] = "A";
                    }
                    else
                    {
                        Session["SaveModeRateDiffAdj"] = "M";
                    }
                }
                
            }


            #endregion
        }

        private void RefetchSrlNo()
        {
            TempTable.Columns.Add("SrlNo", typeof(string));
            int conut = 1;
            foreach (DataRow dr in TempTable.Rows)
            {
                dr["SrlNo"] = conut;
                conut++;
            }
        }



        protected void grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e)
        {
            grid.JSProperties["cpadjustmentNumber"] = adjustmentNumber;
            grid.JSProperties["cpErrorCode"] = Convert.ToString(ErrorCode);
            grid.JSProperties["cpadjustmentId"] = Convert.ToString(adjustmentId);
        }
        protected void grid_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }
        protected void grid_DataBinding(object sender, EventArgs e)
        {
            grid.DataSource = TempTable;
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
        #endregion

       
    }
}