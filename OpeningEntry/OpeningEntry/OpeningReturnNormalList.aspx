﻿<%@ Page Title="" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" CodeBehind="OpeningReturnNormalList.aspx.cs" Inherits="OpeningEntry.OpeningEntry.OpeningReturnNormalList_aspx" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
     Namespace="DevExpress.Data.Linq" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>

        .fwidth{width:350px !important;}

           .padTabtype2 > tbody > tr > td {
            padding: 0px 5px;
        }

            .padTabtype2 > tbody > tr > td > label {
                margin-bottom: 0 !important;
                margin-right: 15px;
            }
      
    </style>
     <%--Subhra--%>
    <script>
        var ReturnId = 0;
        function onPrintJv(id) {
            // debugger;
            ReturnId = id;
            cSelectPanel.cpSuccess = "";
            cDocumentsPopup.Show();
            cCmbDesignName.SetSelectedIndex(0);
            cSelectPanel.PerformCallback('Bindalldesignes');
            $('#btnOK').focus();
        }

        function PerformCallToGridBind() {
            cSelectPanel.PerformCallback('Bindsingledesign');
            cDocumentsPopup.Hide();
            return false;
        }
        function cSelectPanelEndCall(s, e) {
            debugger;
            if (cSelectPanel.cpSuccess != "") {
                var TotDocument = cSelectPanel.cpSuccess.split(',');
                var reportName = cCmbDesignName.GetValue();
                var module = 'Sales_Return';
                if (TotDocument.length > 0) {
                    for (var i = 0; i < TotDocument.length; i++) {
                        if (TotDocument[i] != "") {
                            window.open("../../Reports/REPXReports/RepxReportViewer.aspx?Previewrpt=" + reportName + '&modulename=' + module + '&id=' + ReturnId + '&PrintOption=' + TotDocument[i], '_blank')
                        }
                    }
                }
            }
            if (cSelectPanel.cpSuccess == "") {
                if (cSelectPanel.cpChecked != "") {
                    jAlert('Please check Original For Recipient and proceed.');
                }
                CselectOriginal.SetCheckState('UnChecked');
                CselectDuplicate.SetCheckState('UnChecked');
                CselectTriplicate.SetCheckState('UnChecked');
                cCmbDesignName.SetSelectedIndex(0);
            }
        }


    </script>
    <%--Subhra--%>
    <script>

        function OnclickViewAttachment(obj) {
            //var URL = '/OMS/Management/Activities/SalesReturn_Document.aspx?idbldng=' + obj + '&type=SalesReturn';
            var URL = '/OMS/Management/Activities/EntriesDocuments.aspx?idbldng=' + obj + '&type=NormalSaleReturn';
            window.location.href = URL;
        }
        document.onkeydown = function (e) {
            if (event.keyCode == 18) isCtrl = true;


            if (event.keyCode == 65 && isCtrl == true) { //run code for alt+a -- ie, Add

                StopDefaultAction(e);
                OnAddButtonClick();
            }

        }

        function StopDefaultAction(e) {
            if (e.preventDefault) { e.preventDefault() }
            else { e.stop() };

            e.returnValue = false;
            e.stopPropagation();
        }
        function OnAddButtonClick() {
            var url = 'ReturnNormal.aspx?key=' + 'ADD';
            window.location.href = url;
        }

        function OnMoreInfoClick(keyValue) {
            var ActiveUser = '<%=Session["userid"]%>'
            if (ActiveUser != null) {
                $.ajax({
                    type: "POST",
                    url: "ReturnNormalList.aspx/GetEditablePermission",
                    data: "{'ActiveUser':'" + ActiveUser + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        var status = msg.d;
                        var url = 'ReturnNormal.aspx?key=' + keyValue + '&Permission=' + status + '&type=SRN';
                        window.location.href = url;
                    }
                });
            }
        }

        ////##### coded by Samrat Roy - 17/04/2017 - ref IssueLog(P Order - 70) 
        ////Add an another param to define request type 
        function OnViewClick(keyValue) {
            var url = '/OMS/Management/Activities/ReturnNormal.aspx?key=' + keyValue + '&req=V' + '&type=SRN';
            window.location.href = url;
        }

        function OnClickDelete(keyValue) {
            jConfirm('Confirm delete?', 'Confirmation Dialog', function (r) {
                if (r == true) {
                    cGrdSalesReturn.PerformCallback('Delete~' + keyValue);
                }
            });
        }
        function OnEndCallback(s, e) {

            if (cGrdSalesReturn.cpDelete != null) {
                jAlert(cGrdSalesReturn.cpDelete);

                cGrdSalesReturn.cpDelete = null;
                cGrdSalesReturn.Refresh();
                //  window.location.href = "ReturnNormalList.aspx";
            }
        }
        //function OnClickDelete(keyValue) {
        //    jConfirm('Confirm delete?', 'Confirmation Dialog', function (r) {
        //        if (r == true) {
        //            cGrdQuotation.PerformCallback('Delete~' + keyValue);
        //        }
        //    });
        //}

        function OnTransferClick(keyValue) {
            cPopup_BranchTransfer.Show();

            cBranchTransferCallBackPanel.PerformCallback('Edit~' + keyValue);
        }


        function SaveButtonClick() {
            var branch = $("#ddlBranch").val();
            if (branch == "") {
                $("#MandatoryBranch").show();
                return false;
            }
            //var CashBank = cddlCashBank.GetValue();
            //if (CashBank == null) {
            //    $("#MandatoryCashBank").show();
            //    return false;
            //}
            var EditID = document.getElementById('hdnEditID').value;
            //var branch = $("#ddlBranch").val();
            cBranchTransferCallBackPanel.PerformCallback('Save~' + EditID + '~' + branch);
        }
        function BranchTransferEndCallBack() {
            if (cBranchTransferCallBackPanel.cpEdit != null) {
                document.getElementById('hdnEditID').value = cBranchTransferCallBackPanel.cpEdit;
                cBranchTransferCallBackPanel.cpEdit = null;
            }
            if (cBranchTransferCallBackPanel.cpTransfer == "YES") {
                cPopup_BranchTransfer.Hide();
                window.location.href = 'ReturnNormalList.aspx';
                cBranchTransferCallBackPanel.cpTransfer = null;
            }
            if (cBranchTransferCallBackPanel.cpBtnVisible != null && cBranchTransferCallBackPanel.cpBtnVisible != "") {
                cBranchTransferCallBackPanel.cpBtnVisible = null;
                document.getElementById('btnSaveNew').style.display = 'none'
                document.getElementById('tagged').style.display = 'block';
            }
        }

        var isFirstTime = true;

        function AllControlInitilize() {
            if (isFirstTime) {
                //if (localStorage.getItem('ReturnList_FromDate')) {
                //    var fromdatearray = localStorage.getItem('ReturnList_FromDate').split('-');
                //    var fromdate = new Date(fromdatearray[0], parseFloat(fromdatearray[1]) - 1, fromdatearray[2], 0, 0, 0, 0);
                //    cFormDate.SetDate(fromdate);
                //}

                //if (localStorage.getItem('ReturnList_ToDate')) {
                //    var todatearray = localStorage.getItem('ReturnList_ToDate').split('-');
                //    var todate = new Date(todatearray[0], parseFloat(todatearray[1]) - 1, todatearray[2], 0, 0, 0, 0);
                //    ctoDate.SetDate(todate);
                //}

                //if (localStorage.getItem('ReturnList_Branch')) {
                //    if (ccmbBranchfilter.FindItemByValue(localStorage.getItem('ReturnList_Branch'))) {
                //        ccmbBranchfilter.SetValue(localStorage.getItem('ReturnList_Branch'));
                //    }

                //}

                //if ($("#LoadGridData").val() == "ok")
                //    updateGridByDate();
                isFirstTime = false;
            }
        }

        function updateGridByDate() {
            if (cFormDate.GetDate() == null) {
                jAlert('Please select from date.', 'Alert', function () { cFormDate.Focus(); });
            }
            else if (ctoDate.GetDate() == null) {
                jAlert('Please select to date.', 'Alert', function () { ctoDate.Focus(); });
            }
            else if (ccmbBranchfilter.GetValue() == null) {
                jAlert('Please select Branch.', 'Alert', function () { ccmbBranchfilter.Focus(); });
            }
            else {
                localStorage.setItem("ReturnList_FromDate", cFormDate.GetDate().format('yyyy-MM-dd'));
                localStorage.setItem("ReturnList_ToDate", ctoDate.GetDate().format('yyyy-MM-dd'));
                localStorage.setItem("ReturnList_Branch", ccmbBranchfilter.GetValue());
                $("#hfFromDate").val(cFormDate.GetDate().format('yyyy-MM-dd'));
                $("#hfToDate").val(ctoDate.GetDate().format('yyyy-MM-dd'));
                $("#hfBranchID").val(ccmbBranchfilter.GetValue());
                $("#hfIsFilter").val("Y");
                cGrdSalesReturn.Refresh();
                //cGrdSalesReturn.PerformCallback('FilterGridByDate~' + cFormDate.GetDate().format('yyyy-MM-dd') + '~' + ctoDate.GetDate().format('yyyy-MM-dd') + '~' + ccmbBranchfilter.GetValue())
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="panel-heading">
        <div class="panel-title">
            <h3>Opening Sale Return Normal</h3>
        </div>
    </div>
    <div class="form_main">
        <div class="clearfix">
             <% if (rights.CanAdd)
                                   { %>
          <%--  <a href="javascript:void(0);" onclick="OnAddButtonClick()" class="btn btn-primary"><span><u>A</u>dd New</span> </a>--%>
            <%} %>

            <% if (rights.CanExport)
                                               { %>
         <%--   <asp:DropDownList ID="drdExport" runat="server" CssClass="btn btn-sm btn-primary" OnSelectedIndexChanged="cmbExport_SelectedIndexChanged" AutoPostBack="true" OnChange="if(!AvailableExportOption()){return false;}">
                <asp:ListItem Value="0">Export to</asp:ListItem>
                <asp:ListItem Value="1">PDF</asp:ListItem>
                <asp:ListItem Value="2">XLS</asp:ListItem>
                <asp:ListItem Value="3">RTF</asp:ListItem>
                <asp:ListItem Value="4">CSV</asp:ListItem>
            </asp:DropDownList>--%>
             <% } %>
            
              <table class="padTabtype2 pull-right" id="gridFilter">
                        <tr>
                            <td>
                                <label>From Date</label></td>
                            <td>
                                <dxe:ASPxDateEdit ID="FormDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="cFormDate" Width="100%">
                                    <buttonstyle width="13px">
                        </buttonstyle>
                                </dxe:ASPxDateEdit>
                            </td>
                            <td>
                                <label>To Date</label>
                            </td>
                            <td>
                                <dxe:ASPxDateEdit ID="toDate" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="ctoDate" Width="100%">
                                    <buttonstyle width="13px">
                        </buttonstyle>
                                </dxe:ASPxDateEdit>
                            </td>
                            <td>Unit</td>
                            <td>
                                <dxe:ASPxComboBox ID="cmbBranchfilter" runat="server" ClientInstanceName="ccmbBranchfilter" Width="100%">
                                </dxe:ASPxComboBox>
                            </td>
                            <td>
                                <input type="button" value="Show" class="btn btn-primary" onclick="updateGridByDate()" />
                            </td>

                        </tr>

                    </table>
        </div>
    </div>
     <div class="GridViewArea">
        <dxe:ASPxGridView ID="GrdSalesReturn" runat="server" KeyFieldName="SrlNo" AutoGenerateColumns="False"  
            Width="100%" ClientInstanceName="cGrdSalesReturn" OnCustomCallback="GrdSalesReturn_CustomCallback" SettingsBehavior-AllowFocusedRow="true" 
            
         DataSourceID="EntityServerModeDataSource"   SettingsDataSecurity-AllowEdit="false" SettingsDataSecurity-AllowInsert="false" SettingsDataSecurity-AllowDelete="false"  Setting-HorizontalScrollBarMode="Auto" >
            <SettingsSearchPanel Visible="True" Delay="5000" />
            <Columns>
                <dxe:GridViewDataTextColumn Caption="Document Number" FieldName="SalesReturnNo"
                    VisibleIndex="0" FixedStyle="Left"  Width="140px" >
                    <CellStyle CssClass="gridcellleft" Wrap="true" >
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>

                <dxe:GridViewDataTextColumn VisibleIndex="1" FieldName="Return_Date" Caption="Posting Date" SortOrder="Descending">
                                               <CellStyle Wrap="False" CssClass="gridcellleft"></CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                                               <EditFormSettings Visible="True"></EditFormSettings>
                                           </dxe:GridViewDataTextColumn>
                 <%--<dxe:GridViewDataTextColumn Caption="Date" FieldName="Return_Date"
                    VisibleIndex="1" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>--%>

                 <dxe:GridViewDataTextColumn Caption="Invoice Number(s)" FieldName="Invoice"
                    VisibleIndex="2"  Width="130px">
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>
                   <dxe:GridViewDataTextColumn Caption="Date(s)" FieldName="InvoiceDate"
                    VisibleIndex="3" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                       <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Customer" FieldName="CustomerName"
                    VisibleIndex="3" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn Caption="Amount" FieldName="Amount"
                    VisibleIndex="4" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                     <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>               
               
                 <dxe:GridViewDataTextColumn Caption="Unit" FieldName="Branch"
                    VisibleIndex="5" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>   
                 <dxe:GridViewDataTextColumn VisibleIndex="6" Caption="Assigned To Branch" FieldName="branchToBranch" Width="130px">
                <CellStyle Wrap="False" CssClass="gridcellleft"></CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
            </dxe:GridViewDataTextColumn>
                
                 <dxe:GridViewDataTextColumn Caption="Place of Supply[GST]" FieldName="PlaceOfSupply" VisibleIndex="7" Width="100px">
                  
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>
                 <dxe:GridViewDataTextColumn Caption="Entered By" FieldName="Return_CreateUser"
                    VisibleIndex="8" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                     <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>   

                  <dxe:GridViewDataTextColumn Caption="Last Update On" FieldName="Return_CreateDateTime"
                    VisibleIndex="9" >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                      <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>   
                  <dxe:GridViewDataTextColumn Caption="Updated By" FieldName="Return_ModifyUser"
                    VisibleIndex="10"  >
                    <CellStyle CssClass="gridcellleft" Wrap="true">
                    </CellStyle>
                      <Settings AllowAutoFilterTextInputTimer="False" />
                    <Settings AutoFilterCondition="Contains" />
                </dxe:GridViewDataTextColumn>    
                  <dxe:GridViewDataTextColumn VisibleIndex="11" Caption="Status" FieldName="PStatus">
                <CellStyle Wrap="False" CssClass="gridcellleft"></CellStyle>
                      <Settings AllowAutoFilterTextInputTimer="False" />
            </dxe:GridViewDataTextColumn>
                <dxe:GridViewDataTextColumn HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="center" VisibleIndex="17"  Width="180px">
                    <DataItemTemplate>
                         <% if (rights.CanView)
                            { %>
                        <a href="javascript:void(0);" onclick="OnViewClick('<%#Eval("Return_Id")%>')" class="pad" title="View">
                            <img src="../../../assests/images/doc.png" /></a>
                           <% } %>
                        <% if (rights.CanEdit)
                                       { %>
           <%--             <a href="javascript:void(0);" onclick="OnMoreInfoClick('<%#Eval("Return_Id")%>')" class="pad" title="Edit">
                            <img src="../../../assests/images/info.png" /></a>--%>
                        <%} %>
                        <% if (rights.CanDelete)
                                       { %>
                        <%--<a href="javascript:void(0);" onclick="OnClickDelete('<%#Eval("Return_Id")%>')" class="pad" title="Delete">
                            <img src="../../../assests/images/Delete.png" /></a>--%>
                        <%} %>
                        <%-- <a href="javascript:void(0);" onclick="OnClickCopy('<%# Container.KeyValue %>')" class="pad" title="Copy ">
                            <i class="fa fa-copy"></i></a>--%>
                      <%--   <% if (rights.CanEdit)
                                       { %>
                        <a href="javascript:void(0);" onclick="OnClickStatus('<%# Container.KeyValue %>')" class="pad" title="Status">
                            <img src="../../../assests/images/verified.png" /></a><%} %>--%>
                         <% if (rights.CanView)
                                       { %>
                        <%--<a href="javascript:void(0);" onclick="OnclickViewAttachment('<%#Eval("Return_Id")%>')" class="pad" title="View Attachment">
                            <img src="../../../assests/images/attachment.png" />
                        </a>--%>
                        <%} %>

                          <% if (rights.CanPrint)
                           { %>
                      <%--  <a href="javascript:void(0);" onclick="onPrintJv('<%#Eval("Return_Id")%>')" class="pad" title="print">
                            <img src="../../../assests/images/Print.png" />
                        </a>--%>
                        <%} %>

                           <% if (rights.CanAssignbranch)
                                                                      { %>
<%--                           <a href="javascript:void(0);" class="pad" title="Branch Assignment" id="a_Assignment" onclick="OnTransferClick('<%#Eval("Return_Id")%>')"  >
                            <span class="fa fa-truck out"></span>
                       </a>--%>
                        <%} %>
                         
                 
             
                    </DataItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <CellStyle HorizontalAlign="Center"></CellStyle>
                    <Settings AllowAutoFilterTextInputTimer="False" />
                    <HeaderTemplate><span>Actions</span></HeaderTemplate>
                    <EditFormSettings Visible="False"></EditFormSettings>
                </dxe:GridViewDataTextColumn>
            </Columns>
             <SettingsContextMenu Enabled="true"></SettingsContextMenu>
            <ClientSideEvents EndCallback="OnEndCallback" />
           <%-- <SettingsPager NumericButtonCount="20" PageSize="10" ShowSeparators="True" Mode="ShowPager">
                <FirstPageButton Visible="True">
                </FirstPageButton>
                <LastPageButton Visible="True">
                </LastPageButton>
            </SettingsPager>--%>
            <settingspager pagesize="10">
                            <PageSizeItemSettings Visible="true" ShowAllItem="false" Items="10,50,100,150,200"/>
                              </settingspager>
            <%--<SettingsSearchPanel Visible="True" />--%>
            <Settings ShowGroupPanel="True" ShowStatusBar="Hidden" HorizontalScrollBarMode="Visible" ShowFilterRow="true" ShowFilterRowMenu="true" />
            <SettingsLoadingPanel Text="Please Wait..." />
        </dxe:ASPxGridView>


          <dx:LinqServerModeDataSource ID="EntityServerModeDataSource" runat="server" OnSelecting="EntityServerModeDataSource_Selecting"
            ContextTypeName="ERPDataClassesDataContext" TableName="v_SalesReturnNormalList" />
        <asp:HiddenField ID="hiddenedit" runat="server" />
    </div>
    <div style="display: none">
        <dxe:ASPxGridViewExporter ID="exporter" GridViewID="GrdSalesReturn" runat="server" Landscape="false" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
        </dxe:ASPxGridViewExporter>
    </div>

    

      <div class="PopUpArea">
        <dxe:ASPxPopupControl ID="Popup_BranchTransfer" runat="server" ClientInstanceName="cPopup_BranchTransfer"
            Width="450px" HeaderText="Branch Assignment" PopupHorizontalAlign="WindowCenter"
            BackColor="white" Height="200px" PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <dxe:ASPxCallbackPanel runat="server" ID="BranchTransferCallBackPanel" ClientInstanceName="cBranchTransferCallBackPanel" OnCallback="BranchTransferCallBackPanel_Callback">
                        <PanelCollection>
                            <dxe:PanelContent runat="server">
                                <div style="background: #f5f4f3; padding: 5px 0 5px 0; margin-bottom: 10px; border-radius: 4px; border: 1px solid #ccc;">
                                    <div class="Top clearfix">
                                        <div class="col-md-6">
                                            <label>Assigned To Unit<span style="color: red">*</span></label>
                                            <div>
                                                <asp:DropDownList ID="ddlBranch" runat="server" DataSourceID="dsBranch" 
                                                    DataTextField="BANKBRANCH_NAME" DataValueField="BANKBRANCH_ID" Width="100%">
                                                </asp:DropDownList>
                                                <span id="MandatoryBranch" class="iconBranch pullleftClass fa fa-exclamation-circle iconRed " style="color: red; position: absolute; display: none"
                                                    title="Mandatory"></span>
                                            </div>
                                        </div>
                                        <%--<div class="col-md-6" id="tdCashBankLabel">
                                            <label>Cash/Bank <span style="color: red">*</span></label>
                                            <dxe:ASPxComboBox ID="ddlCashBank" runat="server" ClientIDMode="Static" ClientInstanceName="cddlCashBank" Width="100%" OnCallback="ddlCashBank_Callback">
                                                
                                            </dxe:ASPxComboBox>
                                            <span id="MandatoryCashBank" class="iconCashBank pullleftClass fa fa-exclamation-circle iconRed " style="color: red; position: absolute; display: none" title="Mandatory"></span>
                                        </div>--%>
                                        <div class="clear"></div>
                                        <div class="col-md-12 lblmTop8">
                                            <label>Remarks </label>
                                            <div>
                                                <asp:TextBox ID="txtNarration" runat="server" MaxLength="500"
                                                    TextMode="MultiLine"
                                                    Width="100%" Height="80px" ></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <table style="width: 100%;">
                                    <tr>
                                        <td style="padding: 5px 0;">
                                            <div class="text-center">
                                                <dxe:ASPxButton ID="btnSaveNew" ClientInstanceName="cbtnSaveNew" runat="server"
                                                    AutoPostBack="false" CssClass="btn btn-primary" TabIndex="0" Text="Save & C&#818;lose"
                                                    UseSubmitBehavior="False">
                                                    <ClientSideEvents Click="function(s, e) {SaveButtonClick();}" />
                                                </dxe:ASPxButton>
                                                  <b><span id="tagged" style="display:none;color: red">Advance Already Adjusted. Cannot Modify data</span></b>
                                            </div>

                                        </td>
                                    </tr>
                                </table>
                            </dxe:PanelContent>
                        </PanelCollection>
                        <ClientSideEvents EndCallback="BranchTransferEndCallBack" />
                    </dxe:ASPxCallbackPanel>
                </dxe:PopupControlContentControl>
            </ContentCollection>
            <HeaderStyle BackColor="LightGray" ForeColor="Black" />
        </dxe:ASPxPopupControl>

    </div>
      <div id="DivDataSource">
        <asp:SqlDataSource ID="dsBranch" runat="server" 
            ConflictDetection="CompareAllValues"
            SelectCommand=""></asp:SqlDataSource>
    </div>
      <asp:HiddenField ID="hdnEditID" runat="server" />

     <%--SUBHRA--%>
    <div class="PopUpArea">
        <dxe:ASPxPopupControl ID="ASPxPopupControl1" runat="server" ClientInstanceName="cDocumentsPopup"
            Width="350px" HeaderText="Select Design(s)" PopupHorizontalAlign="WindowCenter"
            PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
            Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
            <ContentCollection>
                <dxe:PopupControlContentControl runat="server">
                    <dxe:ASPxCallbackPanel runat="server" ID="SelectPanel" ClientInstanceName="cSelectPanel" OnCallback="SelectPanel_Callback" ClientSideEvents-EndCallback="cSelectPanelEndCall">
                        <PanelCollection>
                            <dxe:PanelContent runat="server">
                                <dxe:ASPxCheckBox ID="selectOriginal" Text="Original For Recipient" runat="server" ToolTip="Select Original" 
                                    ClientInstanceName="CselectOriginal">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxCheckBox ID="selectDuplicate" Text="Duplicate For Transporter" runat="server" ToolTip="Select Duplicate" 
                                    ClientInstanceName="CselectDuplicate">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxCheckBox ID="selectTriplicate" Text="Triplicate For Supplier" runat="server" ToolTip="Select Triplicate"
                                    ClientInstanceName="CselectTriplicate">
                                </dxe:ASPxCheckBox>
                                <dxe:ASPxComboBox ID="CmbDesignName" ClientInstanceName="cCmbDesignName" runat="server" ValueType="System.String" Width="100%" EnableSynchronization="True">
                                </dxe:ASPxComboBox>
                                <div class="text-center pTop10">
                                    <dxe:ASPxButton ID="btnOK" ClientInstanceName="cbtnOK" runat="server" AutoPostBack="False" Text="OK" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                        <ClientSideEvents Click="function(s, e) {return PerformCallToGridBind();}" />
                                    </dxe:ASPxButton>
                                </div>
                            </dxe:PanelContent>
                        </PanelCollection>
                    </dxe:ASPxCallbackPanel>
                </dxe:PopupControlContentControl>
            </ContentCollection>
        </dxe:ASPxPopupControl>
    </div>

     <dxe:ASPxGlobalEvents ID="GlobalEvents" runat="server">
        <ClientSideEvents ControlsInitialized="AllControlInitilize" />
    </dxe:ASPxGlobalEvents>

     <div>
        <asp:HiddenField ID="hfIsFilter" runat="server" />
        <asp:HiddenField ID="hfFromDate" runat="server" />
        <asp:HiddenField ID="hfToDate" runat="server" />
        <asp:HiddenField ID="hfBranchID" runat="server" />
    </div>

</asp:Content>


