﻿<%@ Page Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" CodeBehind="VendorAgeingSummary.aspx.cs" Inherits="Reports.Reports.GridReports.VendorAgeingSummary" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Data.Linq" TagPrefix="dx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/SearchPopup.css" rel="stylesheet" />
    <script src="JS/SearchMultiPopup.js"></script>
    <script src="/assests/pluggins/choosen/choosen.min.js"></script>
    <style>
        .colDisable {
        cursor:default !important;
        }
        #pageControl, .dxtc-content {
            overflow: visible !important;
        }

        .btnOkformultiselection {
            border-width: 1px;
            padding: 4px 10px;
            font-size: 13px !important;
            margin-right: 6px;
            }
        
        #MandatoryAssign {
            position: absolute;
            right: -17px;
            top: 6px;
        }

        #MandatorySupervisorAssign {
            position: absolute;
            right: 1px;
            top: 27px;
        }

        .chosen-container.chosen-container-multi,
        .chosen-container.chosen-container-single {
            width: 100% !important;
        }

        .chosen-choices {
            width: 100% !important;
        }

        #ListBoxBranches {
            width: 200px;
        }

        #ListBoxProjects{
            width: 200px;
        }

        .hide {
            display: none;
        }

        .dxtc-activeTab .dxtc-link {
            color: #fff !important;
        }

        .divPad input + label {
            padding: 2px;
        }
        /*rev Pallab*/
        .branch-selection-box .dxlpLoadingPanel_PlasticBlue tbody, .branch-selection-box .dxlpLoadingPanelWithContent_PlasticBlue tbody, #divProj .dxlpLoadingPanel_PlasticBlue tbody, #divProj .dxlpLoadingPanelWithContent_PlasticBlue tbody
        {
            bottom: 0 !important;
        }
        .dxlpLoadingPanel_PlasticBlue tbody, .dxlpLoadingPanelWithContent_PlasticBlue tbody
        {
            position: absolute;
            bottom: 80px;
            background: #fff;
            box-shadow: 1px 1px 10px #0000001c;
            right: 50%;
        }
        /*rev end Pallab*/
    </style>

    <script type="text/javascript">
        //for Esc
        document.onkeyup = function (e) {
            if (event.keyCode == 27 && popupdetails.GetVisible() == true) {
            }
        }
        function popupHide(s, e) {
            popupdetails.Hide();
            Grid.Focus();
            $("#drdExport").val(0);
        }

        function fn_OpenDetails(keyValue) {
            Grid.PerformCallback('Edit~' + keyValue);
        }

        $(function () {
            cBranchComponentPanel.PerformCallback('BindComponentGrid' + '~' + $("#ddlbranchHO").val());
        });

        $(function () {
            cProjectPanel.PerformCallback('BindProjectGrid');
        });


        $(document).ready(function () {
            $("#ListBoxBranches").chosen().change(function () {
                var Ids = $(this).val();
                $('#<%=hdnSelectedBranches.ClientID %>').val(Ids);
                $('#MandatoryActivityType').attr('style', 'display:none');
            })

            $("#ddlbranchHO").change(function () {
                var Ids = $(this).val();
                $('#MandatoryActivityType').attr('style', 'display:none');
                $("#hdnSelectedBranches").val('');
                cBranchComponentPanel.PerformCallback('BindComponentGrid' + '~' + $("#ddlbranchHO").val());
            })

            var ProjectSelection = document.getElementById('hdnProjectSelection').value;
            if (ProjectSelection == "0") {
                $('#divProj').addClass('hidden');
            }
            else {
                $('#divProj').removeClass('hidden');
            }
        })
    </script>
  <%-- For multiselection when click on ok button--%>
    <script type="text/javascript">
        function SetSelectedValues(Id, Name, ArrName) {
            if (ArrName == 'VendorSource') {
                var key = Id;
                if (key != null && key != '') {
                    $('#VendModel').modal('hide');
                    ctxtVendName.SetText(Name);
                    GetObjectID('hdnVendorId').value = key;
                }
                else {
                    ctxtVendName.SetText('');
                    GetObjectID('hdnVendorId').value = '';
                }
            }

        }

    </script>
   <%-- For multiselection when click on ok button--%>

    <%-- Start For multiselection--%>
 
     <script type="text/javascript">

         $(document).ready(function () {
             $('#VendModel').on('shown.bs.modal', function () {
                 $('#txtVendSearch').focus();
             })
         })
         var VendArr = new Array();
         $(document).ready(function () {
             var VendObj = new Object();
             VendObj.Name = "VendorSource";
             VendObj.ArraySource = VendArr;
             arrMultiPopup.push(VendObj);
         })
         function VendorButnClick(s, e) {
             $('#VendModel').modal('show');
         }

         function Vendor_KeyDown(s, e) {
             if (e.htmlEvent.key == "Enter") {
                 $('#VendModel').modal('show');
             }
         }

         function Vendorkeydown(e) {
             var OtherDetails = {}

             if ($.trim($("#txtVendSearch").val()) == "" || $.trim($("#txtVendSearch").val()) == null) {
                 return false;
             }
             OtherDetails.SearchKey = $("#txtVendSearch").val();

             if (e.code == "Enter" || e.code == "NumpadEnter") {

                 var HeaderCaption = [];
                 HeaderCaption.push("Vendor Name");

                 if ($("#txtVendSearch").val() != "") {
                     callonServerM("Services/Master.asmx/GetTransporterVendor", OtherDetails, "VendorTable", HeaderCaption, "dPropertyIndex", "SetSelectedValues", "VendorSource");
                 }
             }
             else if (e.code == "ArrowDown") {
                 if ($("input[dPropertyIndex=0]"))
                     $("input[dPropertyIndex=0]").focus();
             }
         }

         function SetfocusOnseach(indexName) {
             if (indexName == "dPropertyIndex")
                 $('#txtVendSearch').focus();
             else
                 $('#txtVendSearch').focus();
         }
   </script>
      <%-- End For multiselection--%>


    <script type="text/javascript">

        function btn_ShowRecordsClick(e) {
            var BranchSelection = document.getElementById('hdnBranchSelection').value;
            var ProjectSelection = document.getElementById('hdnProjectSelection').value;
            var ProjectSelectionInReport = document.getElementById('hdnProjectSelectionInReport').value;
            e.preventDefault;
            var data = "OnDateChanged";
            $("#drdExport").val(0);
            $("#hfIsVendAgeSummFilter").val("Y");

            //if (ctxtVendName.GetValue() == null & chkallvend.checked == false) {
            if (ctxtVendName.GetValue() == null & Cchkallvend.GetValue() == false) {
                jAlert('Please select Vendor for generate the report.');
            }
            else if (ProjectSelection == "1") {
                if (ProjectSelectionInReport == "Yes" && gridprojectLookup.GetValue() == null) {
                    jAlert('Please select atleast one Project for generate the report.');
                }
                else if (BranchSelection == "Yes" && gridbranchLookup.GetValue() == null) {
                    jAlert('Please select atleast one branch for generate the report.');
                }
                else {
                    cCallbackPanel.PerformCallback(data + '~' + $("#ddlbranchHO").val());
                }
            }
            else {
                //cCallbackPanel.PerformCallback(data + '~' + $("#ddlbranchHO").val());
                if (BranchSelection == "Yes" && gridbranchLookup.GetValue() == null) {
                    jAlert('Please select atleast one branch for generate the report.');
                }
                else {
                    cCallbackPanel.PerformCallback(data + '~' + $("#ddlbranchHO").val());
                }
            }

            var ToDate = (cxdeAsOnDate.GetValue() != null) ? cxdeAsOnDate.GetValue() : "";

            ToDate = GetDateFormat(ToDate);
            document.getElementById('<%=DateRange.ClientID %>').innerHTML = "As On: " + ToDate;
        }

        function GetDateFormat(today) {
            if (today != "") {
                var dd = today.getDate();
                var mm = today.getMonth() + 1; //January is 0!

                var yyyy = today.getFullYear();
                if (dd < 10) {
                    dd = '0' + dd;
                }
                if (mm < 10) {
                    mm = '0' + mm;
                }
                today = dd + '-' + mm + '-' + yyyy;
            }

            return today;
        }

        function CallbackPanelEndCall(s, e) {
            <%--Rev Subhra 24-12-2018   0017670--%>
            document.getElementById('<%=CompBranch.ClientID %>').innerHTML = cCallbackPanel.cpBranchNames
            //End of Subhra
            GridVendAgeing.Refresh();
        }

        function OnContextMenuItemClick(sender, args) {
            if (args.item.name == "ExportToPDF" || args.item.name == "ExportToXLS") {
                args.processOnServer = true;
                args.usePostBack = true;
            } else if (args.item.name == "SumSelected")
                args.processOnServer = true;
        }

        function selectAll() {
            gridbranchLookup.gridView.SelectRows();
        }
        function unselectAll() {
            gridbranchLookup.gridView.UnselectRows();
        }

        function selectAll_Project() {
            gridprojectLookup.gridView.SelectRows();
        }
        function unselectAll_Project() {
            gridprojectLookup.gridView.UnselectRows();
        }
        function CloseGridProjectLookup() {
            gridprojectLookup.ConfirmCurrentSelection();
            gridprojectLookup.HideDropDown();
            gridprojectLookup.Focus();
        }

    </script>
    <script>
        //function VendAll(obj) {
        //    if (obj == 'allvend') {
        //        if (chkallvend.checked == true) {
        //            ctxtVendName.SetText('');
        //            GetObjectID('hdnVendorId').value = '';
        //            document.getElementById("txtVendSearch").value = ""
        //            ctxtVendName.SetEnabled(false);
        //        }
        //        else {
        //            ctxtVendName.SetEnabled(true);
        //        }
        //    }
        //}

        function CheckAllVend(s, e) {
            if (s.GetCheckState() == 'Checked') {
                ctxtVendName.SetText('');
                GetObjectID('hdnVendorId').value = '';
                document.getElementById("txtVendSearch").value = ""
                ctxtVendName.SetEnabled(false);
            }
            else {
                ctxtVendName.SetEnabled(true);
            }
        }

        function CallbackVendAgeing_EndCallback() {
            $("#drdExport").val(0);
        }

        function CloseGridBranchLookup() {
            gridbranchLookup.ConfirmCurrentSelection();
            gridbranchLookup.HideDropDown();
            gridbranchLookup.Focus();
        }

    </script>
     <style>
        .plhead a {
            font-size:16px;
            padding-left:10px;
            position:relative;
            width:100%;
            display:block;
            padding:9px 10px 5px 10px;
            text-decoration:none;
        }
        .plhead a>i {
            position:absolute;
            top:11px;
            right:15px;
        }
        #accordion {
            margin-bottom:10px;
        }
        .companyName>span {
            font-size:18px;
            font-weight:bold;
            margin-bottom:15px;
        }
       
        .plhead a.collapsed .fa-minus-circle{
            display:none;
        }
        .paddingTbl>tbody>tr>td {
            padding-right:20px;
        }
        .marginTop10 {
            margin-top:10px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            if ($('body').hasClass('mini-navbar')) {
                var windowWidth = $(window).width();
                var cntWidth = windowWidth - 90;
                GridVendAgeing.SetWidth(cntWidth);
            } else {
                var windowWidth = $(window).width();
                var cntWidth = windowWidth - 220;
                GridVendAgeing.SetWidth(cntWidth);
            }

            $('.navbar-minimalize').click(function () {
                if ($('body').hasClass('mini-navbar')) {
                    var windowWidth = $(window).width();
                    var cntWidth = windowWidth - 220;
                    GridVendAgeing.SetWidth(cntWidth);
                } else {
                    var windowWidth = $(window).width();
                    var cntWidth = windowWidth - 90;
                    GridVendAgeing.SetWidth(cntWidth);
                }

            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:HiddenField ID="hdnexpid" runat="server" />
    </div>

    <div class="panel-heading">
        <div class="panel-group" id="accordion" role="tablist" aria-multiselectable="true">
          <div class="panel panel-info">
            <div class="panel-heading" role="tab" id="headingOne">
              <h4 class="panel-title plhead">
                <a role="button" data-toggle="collapse" data-parent="#accordion" href="#collapseOne" aria-expanded="true" aria-controls="collapseOne" class="collapsed">
                  <asp:Label ID="RptHeading" runat="Server" Text=""  Style="font-weight: bold;"></asp:Label>
                    <i class="fa fa-plus-circle" ></i>
                    <i class="fa fa-minus-circle"></i>
                </a>
              </h4>
            </div>
            <div id="collapseOne" class="panel-collapse collapse" role="tabpanel" aria-labelledby="headingOne">
              <div class="panel-body">
                        
                    <div  class="companyName">
                        <asp:Label ID="CompName" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <%--Rev Subhra 24-12-2018   0017670--%>
                    <div>
                        <asp:Label ID="CompBranch" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <%--End of Rev--%>
                    <div>
                        <asp:Label ID="CompAdd" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <div>
                        <asp:Label ID="CompOth" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <div>
                        <asp:Label ID="CompPh" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <div>
                        <asp:Label ID="CompAccPrd" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
                    <div>
                        <asp:Label ID="DateRange" runat="Server" Text="" Width="470px" ></asp:Label>
                    </div>
              </div>
            </div>
          </div>
        </div>

        <div>
            
        </div>
        
    </div>

    <div class="form_main">
        <asp:HiddenField runat="server" ID="hdndaily" />
        <asp:HiddenField runat="server" ID="hdtid" />
        <div class="row">
            <div class="col-md-2">
                <label style="color: #b5285f; font-weight: bold;" class="clsTo">Head Branch:</label>
                <div>
                    <asp:DropDownList ID="ddlbranchHO" runat="server" Width="100%"></asp:DropDownList>
                </div>
            </div>
            <div class="col-md-2 branch-selection-box">
                <label style="color: #b5285f; font-weight: bold;" class="clsTo">
                    <asp:Label ID="Label1" runat="Server" Text="Branch : " CssClass="mylabel1"></asp:Label></label>
                <%--<asp:ListBox ID="ListBoxBranches" Visible="false" runat="server" SelectionMode="Multiple" Font-Size="12px" Height="60px" Width="100%" CssClass="mb0 chsnProduct  hide" data-placeholder="--- ALL ---"></asp:ListBox>--%>
                <asp:HiddenField ID="hdnActivityType" runat="server" />

                <dxe:ASPxCallbackPanel runat="server" ID="ComponentBranchPanel" ClientInstanceName="cBranchComponentPanel" OnCallback="Componentbranch_Callback">
                    <panelcollection>
                        <dxe:PanelContent runat="server">
                            <dxe:ASPxGridLookup ID="lookup_branch" SelectionMode="Multiple" runat="server" ClientInstanceName="gridbranchLookup"
                                OnDataBinding="lookup_branch_DataBinding"
                                KeyFieldName="ID" Width="100%" TextFormatString="{1}" AutoGenerateColumns="False" MultiTextSeparator=", ">
                                <Columns>
                                    <dxe:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="60px" Caption=" " />
                                    <dxe:GridViewDataColumn FieldName="branch_code" Visible="true" VisibleIndex="1" width="200px" Caption="Branch code" Settings-AutoFilterCondition="Contains">
                                        <Settings AutoFilterCondition="Contains" />
                                    </dxe:GridViewDataColumn>

                                    <dxe:GridViewDataColumn FieldName="branch_description" Visible="true" VisibleIndex="2" width="200px" Caption="Branch Name" Settings-AutoFilterCondition="Contains">
                                        <Settings AutoFilterCondition="Contains" />
                                    </dxe:GridViewDataColumn>
                                </Columns>
                                <GridViewProperties Settings-VerticalScrollBarMode="Auto" SettingsPager-Mode="ShowAllRecords">
                                    <Templates>
                                        <StatusBar>
                                            <table class="OptionsTable" style="float: right">
                                                <tr>
                                                    <td>
                                                        <%--<div class="hide">--%>
                                                            <dxe:ASPxButton ID="ASPxButton2" runat="server" AutoPostBack="false" Text="Select All" ClientSideEvents-Click="selectAll" UseSubmitBehavior="False" />
                                                      <%--  </div>--%>
                                                        <dxe:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="false" Text="Deselect All" ClientSideEvents-Click="unselectAll" UseSubmitBehavior="False" />                                                        
                                                        <dxe:ASPxButton ID="Close" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridBranchLookup" UseSubmitBehavior="False" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </StatusBar>
                                    </Templates>
                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                    <SettingsPager Mode="ShowPager">
                                    </SettingsPager>

                                    <SettingsPager PageSize="20">
                                        <PageSizeItemSettings Visible="true" ShowAllItem="false" Items="10,20,50,100,150,200" />
                                    </SettingsPager>

                                    <Settings ShowFilterRow="True" ShowFilterRowMenu="true" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                                </GridViewProperties>

                            </dxe:ASPxGridLookup>
                        </dxe:PanelContent>
                    </panelcollection>
                </dxe:ASPxCallbackPanel>

                <asp:HiddenField ID="hdnActivityTypeText" runat="server" />
                <span id="MandatoryActivityType" style="display: none" class="validclass">
                    <img id="3gridHistory_DXPEForm_efnew_DXEFL_DXEditor1112_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                <asp:HiddenField ID="hdnSelectedBranches" runat="server" />
            </div>

            <div class="col-md-2 col-lg-2">
                <label style="color: #b5285f; font-weight: bold;" class="clsFrom">
                    <asp:Label ID="lblFromDate" runat="Server" Text="As On Date : " CssClass="mylabel1"></asp:Label>
                </label>
                <dxe:ASPxDateEdit ID="ASPxAsOnDate" runat="server" EditFormat="custom" DisplayFormatString="dd-MM-yyyy" EditFormatString="dd-MM-yyyy"
                    UseMaskBehavior="True" Width="100%" ClientInstanceName="cxdeAsOnDate">
                    <buttonstyle width="13px">
                    </buttonstyle>
                </dxe:ASPxDateEdit>
            </div>

            <div class="col-md-1">
                <label style="color: #b5285f; font-weight: bold;" class="clsTo">
                    <asp:Label ID="Label2" runat="Server" Text="Days: " CssClass="mylabel1"></asp:Label>
                </label>
                <asp:DropDownList ID="ddldays" runat="server" Width="100%">
                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                    <asp:ListItem Text="0-30 Days" Value="D30"></asp:ListItem>
                    <asp:ListItem Text="31-60 Days" Value="D60"></asp:ListItem>
                    <asp:ListItem Text="61-90 Days" Value="D90"></asp:ListItem>
                    <asp:ListItem Text="91-120 Days" Value="D120"></asp:ListItem>
                    <%--<asp:ListItem Text="120 & Above" Value="D120A"></asp:ListItem>--%>
                    <asp:ListItem Text="121-150 Days" Value="D150"></asp:ListItem>
                    <asp:ListItem Text="151-180 Days" Value="D180"></asp:ListItem>
                    <asp:ListItem Text="181 & Above" Value="D180A"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="col-md-2">
                <span style="margin-bottom: 5px;display: inline-block;"><dxe:ASPxLabel ID="lbl_Vendor" style="color: #b5285f;" runat="server" Text="Vendor:">
                </dxe:ASPxLabel></span>                        
                <dxe:ASPxButtonEdit ID="txtVendName" runat="server" ReadOnly="true" ClientInstanceName="ctxtVendName" Width="100%" TabIndex="5">
                    <Buttons>
                        <dxe:EditButton>
                        </dxe:EditButton>
                    </Buttons>
                    <ClientSideEvents ButtonClick="function(s,e){VendorButnClick();}" KeyDown="function(s,e){Vendor_KeyDown(s,e);}" />
                </dxe:ASPxButtonEdit>
            </div>
            <div class="col-md-1" style="padding:0;padding-top: 21px;">
                <%--<asp:CheckBox runat="server" ID="chkallvend" Checked="false" Text="All Vendor" />--%>
                <dxe:ASPxCheckBox runat="server" ID="chkallvend" Checked="false" Text="All Vendor" ClientInstanceName="Cchkallvend">
                     <ClientSideEvents CheckedChanged="CheckAllVend" />
                </dxe:ASPxCheckBox>
            </div>
            <div class="col-md-2" style="padding:0;padding-top: 21px;">
                <%--<asp:CheckBox runat="server" ID="chkcb" Checked="false" Text="Include Cash/Bank" />--%>
                <td><dxe:ASPxCheckBox runat="server" ID="chkcb" Checked="false" Text="Include Cash/Bank">
                    </dxe:ASPxCheckBox></td>
            </div>
            <div class="clear"></div>
            <div class="col-md-12">
                <div class="col-md-2 lblmTop8" style="padding:0;padding-top: 1px; width:205px;margin-right: 15px;" id="divProj">
                <div style="color: #b5285f; /*font-weight: bold;*/" class="clsTo">
                    <asp:Label ID="lblProj" runat="Server" Text="Project : " CssClass="mylabel1"></asp:Label>
                </div>
                <asp:ListBox ID="ListBoxProjects" Visible="false" runat="server" SelectionMode="Single" Font-Size="12px" Height="60px" Width="100%" CssClass="mb0 chsnProduct  hide" data-placeholder="--- ALL ---"></asp:ListBox>
                <dxe:ASPxCallbackPanel runat="server" ID="ProjectPanel" ClientInstanceName="cProjectPanel" OnCallback="Project_Callback">
                    <PanelCollection>
                        <dxe:PanelContent runat="server">
                            <dxe:ASPxGridLookup ID="lookup_project" SelectionMode="Single" runat="server" ClientInstanceName="gridprojectLookup"
                                OnDataBinding="lookup_project_DataBinding"
                                KeyFieldName="ID" Width="100%" TextFormatString="{1}" AutoGenerateColumns="False" MultiTextSeparator=", ">
                                <Columns>
                                    <dxe:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="60" Caption=" " />
                                    <dxe:GridViewDataColumn FieldName="project_code" Visible="true" VisibleIndex="1" width="200px" Caption="Project code" Settings-AutoFilterCondition="Contains">
                                        <Settings AutoFilterCondition="Contains" />
                                    </dxe:GridViewDataColumn>

                                    <dxe:GridViewDataColumn FieldName="project_name" Visible="true" VisibleIndex="2" width="200px" Caption="Project Name" Settings-AutoFilterCondition="Contains">
                                        <Settings AutoFilterCondition="Contains" />
                                    </dxe:GridViewDataColumn>
                                </Columns>
                                <GridViewProperties Settings-VerticalScrollBarMode="Auto" SettingsPager-Mode="ShowAllRecords">
                                    <Templates>
                                        <StatusBar>
                                            <table class="OptionsTable" style="float: right">
                                                <tr>
                                                    <td>
                                                        <div class="hide">
                                                            <dxe:ASPxButton ID="ASPxButton3" runat="server" AutoPostBack="false" Text="Select All" ClientSideEvents-Click="selectAll_Project" UseSubmitBehavior="False"/>
                                                        </div>
                                                        <dxe:ASPxButton ID="ASPxButton4" runat="server" AutoPostBack="false" Text="Deselect" ClientSideEvents-Click="unselectAll_Project" UseSubmitBehavior="False"/>                                                        
                                                        <dxe:ASPxButton ID="ASPxButton5" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridProjectLookup" UseSubmitBehavior="False" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </StatusBar>
                                    </Templates>
                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                    <SettingsPager Mode="ShowPager">
                                    </SettingsPager>

                                    <SettingsPager PageSize="20">
                                        <PageSizeItemSettings Visible="true" ShowAllItem="false" Items="10,20,50,100,150,200" />
                                    </SettingsPager>

                                    <Settings ShowFilterRow="True" ShowFilterRowMenu="true" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                                </GridViewProperties>
                            </dxe:ASPxGridLookup>
                        </dxe:PanelContent>
                    </PanelCollection>
                </dxe:ASPxCallbackPanel>

                <span id="MandatoryActivityType" style="display: none" class="validclass">
                    <img id="3gridHistory_DXPEForm_efnew_DXEFL_DXEditor1112_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                <asp:HiddenField ID="hdnSelectedProjects" runat="server" />
            </div>
                <div class="col-md-10 marginTop8 divPad"  style="padding-top: 12px; padding-left:0;">
                <table class="paddingTbl">
                <tr>
                    <%--<td><asp:CheckBox runat="server" ID="chkjv" Checked="false" Text="Include Journal" /></td>
                    <td><asp:CheckBox runat="server" ID="chkdncn" Checked="false" Text="Exclude Debit/Credit Note" /></td>--%>
                     <td><dxe:ASPxCheckBox runat="server" ID="chkjv" Checked="false" Text="Include Journal">
                    </dxe:ASPxCheckBox></td>
                    <td><dxe:ASPxCheckBox runat="server" ID="chkdncn" Checked="false" Text="Exclude Debit/Credit Note">
                    </dxe:ASPxCheckBox></td>
                    <td >
                    <button id="btnShow" class="btn btn-success" type="button" onclick="btn_ShowRecordsClick(this);">Show</button>
                    <% if (rights.CanExport)
                        { %> 
                            <asp:DropDownList ID="drdExport" runat="server" CssClass="btn btn-sm btn-primary"
                                OnSelectedIndexChanged="cmbExport_SelectedIndexChanged" AutoPostBack="true" OnChange="if(!AvailableExportOption()){return false;}">
                                <asp:ListItem Value="0">Export to</asp:ListItem>
                               <%-- <asp:ListItem Value="1">PDF</asp:ListItem>
                                <asp:ListItem Value="2">XLSX</asp:ListItem>
                                <asp:ListItem Value="3">RTF</asp:ListItem>
                                <asp:ListItem Value="4">CSV</asp:ListItem>--%>
                                <asp:ListItem Value="1">EXCEL</asp:ListItem>
                                <asp:ListItem Value="2">PDF</asp:ListItem>
                                <asp:ListItem Value="3">CSV</asp:ListItem>
                            </asp:DropDownList>
                        <% } %>
                    </td>
                </tr>
            </table>
            </div>
            </div> 

            <%--<div class="col-md-2" style="padding: 0; padding-top: 20px;">
                <table>
                    <tr>

                        <td style="padding-left: 20px;">
                            <button id="btnShow" class="btn btn-primary" type="button" onclick="btn_ShowRecordsClick(this);">Show</button>
                            <asp:DropDownList ID="drdExport" runat="server" CssClass="btn btn-sm btn-primary"
                                OnSelectedIndexChanged="cmbExport_SelectedIndexChanged" AutoPostBack="true" OnChange="if(!AvailableExportOption()){return false;}">
                                <asp:ListItem Value="0">Export to</asp:ListItem>
                                <asp:ListItem Value="1">PDF</asp:ListItem>
                                <asp:ListItem Value="2">XLSX</asp:ListItem>
                                <asp:ListItem Value="3">RTF</asp:ListItem>
                                <asp:ListItem Value="4">CSV</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>

            </div>--%>
            <div class="clear"></div>


            <div class="col-md-2">
                <label style="margin-bottom: 0">&nbsp</label>
                <div class="">
                </div>
            </div>
        </div>
        <table class="TableMain100">
            <tr>
                <td colspan="2">

                    <div>
                        <dxe:ASPxGridView runat="server" ID="ShowGridVendAgeing" ClientInstanceName="GridVendAgeing" Width="100%" EnableRowsCache="false" AutoGenerateColumns="False" KeyFieldName="SEQ"
                            OnSummaryDisplayText="ShowGridVendAgeing_SummaryDisplayText" Settings-HorizontalScrollBarMode="Visible"
                            DataSourceID="GenerateEntityServerModeDataSource" ClientSideEvents-BeginCallback="CallbackVendAgeing_EndCallback" OnHtmlFooterCellPrepared="ShowGridVendAgeing_HtmlFooterCellPrepared" OnHtmlDataCellPrepared="ShowGridVendAgeing_HtmlDataCellPrepared" >
                            <columns>

                                <dxe:GridViewDataTextColumn Caption="Vendor Name"  FieldName="PARTYNAME" GroupIndex="0"
                                    VisibleIndex="0" >
                                </dxe:GridViewDataTextColumn>
                                <dxe:GridViewDataTextColumn Caption="Unit" FieldName="BRANCH_DESCRIPTION" Width="80%"
                                    VisibleIndex="1" FixedStyle="Left" HeaderStyle-CssClass="colDisable">
                                    <CellStyle CssClass="gridcellleft" Wrap="true">
                                    </CellStyle>
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn VisibleIndex="2" FieldName="DOC_TYPE" Width="70%" Caption="Type" FixedStyle="Left" HeaderStyle-CssClass="colDisable">
                                    <CellStyle HorizontalAlign="Left">
                                    </CellStyle>
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn VisibleIndex="3" FieldName="PROJ_NAME" Width="120%" Caption="Project Name" FixedStyle="Left" HeaderStyle-CssClass="colDisable">
                                    <CellStyle HorizontalAlign="Left">
                                    </CellStyle>
                                </dxe:GridViewDataTextColumn>
                            
                                <dxe:GridViewDataTextColumn FieldName="DOC_AMOUNT" Caption="Doc. Amt." Width="50%" VisibleIndex="4" HeaderStyle-CssClass="colDisable">
                                     <CellStyle HorizontalAlign="Right">
                                     </CellStyle>
                                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                                     <HeaderStyle HorizontalAlign="Right" />
                                </dxe:GridViewDataTextColumn>
                                
                                <dxe:GridViewDataTextColumn FieldName="DAYS30" Caption="0-30 Days" Width="40%" VisibleIndex="5" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>
                                
                                <dxe:GridViewDataTextColumn FieldName="DAYS60" Caption="31-60 Days" Width="40%" VisibleIndex="6" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>
                                
                                <dxe:GridViewDataTextColumn FieldName="DAYS90" Caption="61-90 Days" Width="40%" VisibleIndex="7" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>
                                
                                <dxe:GridViewDataTextColumn FieldName="DAYS120" Caption="91-120 Days" Width="45%" VisibleIndex="8" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn FieldName="DAYS150" Caption="121-150 Days" Width="120px" VisibleIndex="9" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn FieldName="DAYS180" Caption="151-180 Days" Width="120px" VisibleIndex="10" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>
                                
                                <dxe:GridViewDataTextColumn FieldName="DAYS180A" Caption="181 & Above" Width="40%" VisibleIndex="11" HeaderStyle-CssClass="colDisable">
                                      <HeaderStyle HorizontalAlign="Right" />
                                      <CellStyle HorizontalAlign="Right">
                                      </CellStyle>
                                </dxe:GridViewDataTextColumn>

                                <dxe:GridViewDataTextColumn FieldName="CUMBAL_AMOUNT" Caption="Balance" Width="45%" VisibleIndex="12" HeaderStyle-CssClass="colDisable">
                                     <CellStyle HorizontalAlign="Right">
                                     </CellStyle>
                                    <PropertiesTextEdit DisplayFormatString="0.00"></PropertiesTextEdit>
                                     <HeaderStyle HorizontalAlign="Right" />
                                </dxe:GridViewDataTextColumn>                               
                            </columns>

                            <settingsbehavior confirmdelete="true" enablecustomizationwindow="true" enablerowhottrack="true" columnresizemode="Control" />
                            <settings showfooter="true" showgrouppanel="true" showgroupfooter="VisibleIfExpanded" />
                            <settingsediting mode="EditForm" />
                            <settingscontextmenu enabled="true" />
                            <settingsbehavior autoexpandallgroups="true" AllowSort="False"/>
                            <settings showgrouppanel="true" showstatusbar="Visible" showfilterrow="true" />
                            <settingssearchpanel visible="false" />
                            <settingspager pagesize="10">
                                <PageSizeItemSettings Visible="true" ShowAllItem="false" Items="10,50,100" />
                            </settingspager>

                            <totalsummary>
                                <dxe:ASPxSummaryItem FieldName="DOC_TYPE" SummaryType="Custom" Tag="Item_DocType"/>
                                <dxe:ASPxSummaryItem FieldName="DOC_AMOUNT" SummaryType="Custom" Tag="Item_Doc_Amount"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS30" SummaryType="Custom" Tag="Item_Days30"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS60" SummaryType="Custom" Tag="Item_Days60"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS90" SummaryType="Custom" Tag="Item_Days90"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS120" SummaryType="Custom" Tag="Item_Days120"/>
                                 <dxe:ASPxSummaryItem FieldName="DAYS150" SummaryType="Custom" Tag="Item_Days150"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS180" SummaryType="Custom" Tag="Item_Days180"/>
                                <dxe:ASPxSummaryItem FieldName="DAYS180A" SummaryType="Custom" Tag="Item_Days180A"/>
                                <dxe:ASPxSummaryItem FieldName="CUMBAL_AMOUNT" SummaryType="Custom" Tag="Item_BalAmt" >                                   
                                </dxe:ASPxSummaryItem>
                            </totalsummary>

                        </dxe:ASPxGridView>

                        <dx:LinqServerModeDataSource ID="GenerateEntityServerModeDataSource" runat="server" OnSelecting="GenerateEntityServerModeDataSource_Selecting"
                            ContextTypeName="ReportSourceDataContext" TableName="PARTYWISEAGEINGSUMDET_REPORT"></dx:LinqServerModeDataSource>
                    </div>


                </td>
            </tr>
        </table>
    </div>

    <div>
    </div>

    <!--Vendor Modal -->
    <div class="modal fade" id="VendModel" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Vendor Search</h4>
                </div>
                <div class="modal-body">
                    <input type="text" onkeydown="Vendorkeydown(event)" id="txtVendSearch" width="100%" placeholder="Search By Vendor Name" />
                    <div id="VendorTable">
                        <table border='1' width="100%">
                            <tr class="HeaderStyle" style="font-size:small">
                                <th>Select</th>
                                 <th class="hide">id</th>
                                <th>Vendor Name</th>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btnOkformultiselection btn-default"  onclick="DeSelectAll('VendorSource')">Deselect All</button>
                    <button type="button" class="btnOkformultiselection btn-default" data-dismiss="modal" onclick="OKPopup('VendorSource')">OK</button>
                    <%--<button type="button" class="btnOkformultiselection btn-default" data-dismiss="modal">Close</button>--%>
                </div>
            </div>
        </div>
    </div>
    <!--Vendor Modal -->

    <dxe:ASPxGridViewExporter ID="exporter" runat="server" Landscape="true" PaperKind="A4" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
    </dxe:ASPxGridViewExporter>
     <asp:HiddenField ID="hdnVendorId" runat="server" />

    <dxe:ASPxCallbackPanel runat="server" ID="CallbackPanel" ClientInstanceName="cCallbackPanel" OnCallback="CallbackPanel_Callback">
    <PanelCollection>
        <dxe:PanelContent runat="server">
        <asp:HiddenField ID="hfIsVendAgeSummFilter" runat="server" />
        </dxe:PanelContent>
    </PanelCollection>
    <ClientSideEvents EndCallback="CallbackPanelEndCall" />
    </dxe:ASPxCallbackPanel>

    <asp:HiddenField ID="hdnBranchSelection" runat="server" />
    <asp:HiddenField ID="hdnProjectSelection" runat="server" />
    <asp:HiddenField ID="hdnProjectSelectionInReport" runat="server" />
</asp:Content>