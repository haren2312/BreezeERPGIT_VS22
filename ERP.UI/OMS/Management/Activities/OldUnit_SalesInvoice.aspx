﻿<%--=======================================================Revision History=====================================================    
    1.0   Pallab    V2.0.38   15-05-2023      26128: Add Second Hand Sales module design modification & check in small device
=========================================================End Revision History===================================================--%>

<%@ Page Title="Sales Invoice" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="OldUnit_SalesInvoice.aspx.cs" Inherits="ERP.OMS.Management.Activities.OldUnit_SalesInvoice" %>
<%@ Register Src="~/OMS/Management/Activities/UserControls/ucPaymentDetails.ascx" TagPrefix="PayD" TagName="ucPaymentDetails" %>
<%@ Register Src="~/OMS/Management/Activities/UserControls/BillingShippingControl.ascx" TagPrefix="ucBS" TagName="BillingShippingControl" %>
<%@ Register Src="~/OMS/Management/Activities/UserControls/VehicleDetailsControl.ascx" TagPrefix="uc1" TagName="VehicleDetailsControl" %>
<%@ Register Src="~/OMS/Management/Activities/UserControls/TermsConditionsControl.ascx" TagPrefix="uc2" TagName="TermsConditionsControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../Tax%20Details/Js/TaxDetailsItemlevel.js" type="text/javascript"></script>

    <style type="text/css">
        #grid_DXMainTable > tbody > tr > td:last-child, #productLookUp_DDD_gv_DXMainTable > tbody > tr > td:nth-child(2) {
            display: none !important;
        }

        .inline {
            display: inline !important;
        }

        .dxtcLite_PlasticBlue.dxtc-top > .dxtc-content {
            overflow: visible !important;
        }

        .abs {
            position: absolute;
            right: -20px;
            top: 10px;
        }


        .fa.fa-exclamation-circle:before {
            font-family: FontAwesome;
        }

        .tp2 {
            right: -18px;
            top: 7px;
            position: absolute;
        }

        #txtCreditLimit_EC {
            position: absolute;
        }

        #grid_DXStatus span > a {
            display: none;
        }

        #aspxGridTax_DXEditingErrorRow0 {
            display: none;
        }

        .horizontal-images.content li {
            float: left;
        }

        #rdl_SaleInvoice {
            margin-top: 3px;
        }

            #rdl_SaleInvoice > tbody > tr > td {
                padding-right: 5px;
            }
    </style>
    <script>
        (function (global) {

            if (typeof (global) === "undefined") {
                throw new Error("window is undefined");
            }

            var _hash = "!";
            var noBackPlease = function () {
                global.location.href += "#";

                // making sure we have the fruit available for juice (^__^)
                global.setTimeout(function () {
                    global.location.href += "!";
                }, 50);
            };

            global.onhashchange = function () {
                if (global.location.hash !== _hash) {
                    global.location.hash = _hash;
                }
            };

            global.onload = function () {
                noBackPlease();

                // disables backspace on page except on input fields and textarea..
                document.body.onkeydown = function (e) {
                    var elm = e.target.nodeName.toLowerCase();
                    if (e.which === 8 && (elm !== 'input' && elm !== 'textarea')) {
                        e.preventDefault();
                    }
                    // stopping event bubbling up the DOM tree..
                    e.stopPropagation();
                };
            }

        })(window);

        var isCtrl = false;
        //document.onkeyup = function (e) {
        //    if (event.keyCode == 17) {
        //        isCtrl = false;
        //    }
        //    else if (event.keyCode == 27) {
        //        btnCancel_Click();
        //    }
        //}

        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }

        document.onkeydown = function (e) {
            if (event.keyCode == 78 && event.altKey == true && getUrlVars().req != "V") { //run code for Alt + n -- ie, Save & New  
                StopDefaultAction(e);
                Save_ButtonClick();
            }
            else if (event.keyCode == 88 && event.altKey == true && getUrlVars().req != "V") { //run code for Ctrl+X -- ie, Save & Exit!     
                StopDefaultAction(e);
                SaveExit_ButtonClick();
            }
            else if (event.keyCode == 79 && event.altKey == true && getUrlVars().req != "V") { //run code for alt + o -- ie, Save & Exit!     
                StopDefaultAction(e);
                if (page.GetActiveTabIndex() == 1) {
                    fnSaveBillingShipping();
                }
            }
            else if (event.keyCode == 77 && event.altKey == true && getUrlVars().req != "V") { //run code for alt + m -- ie, TC
                $('#TermsConditionseModal').modal({
                    show: 'true'
                });
            }
            else if (event.keyCode == 69 && event.altKey == true && getUrlVars().req != "V") { //run code for alt + e -- ie, TC
                if (($("#TermsConditionseModal").data('bs.modal') || {}).isShown) {
                    StopDefaultAction(e);
                    SaveTermsConditionData();
                }
            }
            else if (event.keyCode == 76 && event.altKey == true && getUrlVars().req != "V") { //run code for alt + l -- ie, TC
                StopDefaultAction(e);
                calcelbuttonclick();
            }
            else {
                //do nothing
            }
        }

        //transporter
        document.onkeyup = function (e) {
            if (event.altKey == true && getUrlVars().req != "V") {
                switch (event.keyCode) {
                    case 83:
                        if (($("#exampleModal").data('bs.modal') || {}).isShown) {
                            SaveVehicleControlData();
                        }
                        break;
                    case 67:
                        modalShowHide(0);
                        break;
                    case 82:
                        modalShowHide(1);
                        $('body').on('shown.bs.modal', '#exampleModal', function () {
                            $('input:visible:enabled:first', this).focus();
                        })
                        break;
                        //case 78:
                        //    StopDefaultAction(e);
                        //    Save_ButtonClick();
                        //    break;
                        //case 88:
                        //    StopDefaultAction(e);
                        //    SaveExit_ButtonClick();
                        //    break;
                    case 120:
                        StopDefaultAction(e);
                        SaveExit_ButtonClick();
                        break;
                    case 84:
                        StopDefaultAction(e);
                        Save_TaxesClick();
                        break;
                    case 85:
                        OpenUdf();
                        break;
                }
            }
        }

        function StopDefaultAction(e) {
            if (e.preventDefault) { e.preventDefault() }
            else { e.stop() };

            e.returnValue = false;
            e.stopPropagation();
        }
    </script>

    <%--Debu Section--%>
    <script type="text/javascript">
        function RecalCulateTaxTotalAmountInline() {
            var totalInlineTaxAmount = 0;
            for (var i = 0; i < taxJson.length; i++) {
                cgridTax.batchEditApi.StartEdit(i, 3);
                var totLength = cgridTax.GetEditor("Taxes_Name").GetText().length;
                var sign = cgridTax.GetEditor("Taxes_Name").GetText().substring(totLength - 3);
                if (sign == '(+)') {
                    totalInlineTaxAmount = totalInlineTaxAmount + parseFloat(cgridTax.GetEditor("Amount").GetValue());
                } else {
                    totalInlineTaxAmount = totalInlineTaxAmount - parseFloat(cgridTax.GetEditor("Amount").GetValue());
                }

                cgridTax.batchEditApi.EndEdit();
            }

            totalInlineTaxAmount = totalInlineTaxAmount + parseFloat(ctxtGstCstVat.GetValue());

            ctxtTaxTotAmt.SetValue(totalInlineTaxAmount);
        }

        function ShowTaxPopUp(type) {
            if (type == "IY") {
                $('#ContentErrorMsg').hide();
                $('#content-6').show();


                if (ccmbGstCstVat.GetItemCount() <= 1) {
                    $('.InlineTaxClass').hide();
                } else {
                    $('.InlineTaxClass').show();
                }
                if (cgridTax.GetVisibleRowsOnPage() < 1) {
                    $('.cgridTaxClass').hide();

                } else {
                    $('.cgridTaxClass').show();
                }

                if (ccmbGstCstVat.GetItemCount() <= 1 && cgridTax.GetVisibleRowsOnPage() < 1) {
                    $('#ContentErrorMsg').show();
                    $('#content-6').hide();
                }
            }
            if (type == "IN") {
                $('#ErrorMsgCharges').hide();
                $('#content-5').show();

                if (ccmbGstCstVatcharge.GetItemCount() <= 1) {
                    $('.chargesDDownTaxClass').hide();
                } else {
                    $('.chargesDDownTaxClass').show();
                }
                if (gridTax.GetVisibleRowsOnPage() < 1) {
                    $('.gridTaxClass').hide();

                } else {
                    $('.gridTaxClass').show();
                }

                if (ccmbGstCstVatcharge.GetItemCount() <= 1 && gridTax.GetVisibleRowsOnPage() < 1) {
                    $('#ErrorMsgCharges').show();
                    $('#content-5').hide();
                }
            }
        }

        function gridFocusedRowChanged(s, e) {
            globalRowIndex = e.visibleIndex;

            //var ProductIDColumn = s.GetColumnByField("ProductID");
            //if (!e.rowValues.hasOwnProperty(ProductIDColumn.index))
            //    return;
            //var cellInfo = e.rowValues[ProductIDColumn.index];

            //if (cCmbProduct.FindItemByValue(cellInfo.value) != null) {
            //    cCmbProduct.SetValue(cellInfo.value);
            //}
            //else {
            //    cCmbProduct.SetSelectedIndex(-1);
            //}
        }

        function OnBatchEditEndEditing(s, e) {
            var ProductIDColumn = s.GetColumnByField("ProductID");
            if (!e.rowValues.hasOwnProperty(ProductIDColumn.index))
                return;
            var cellInfo = e.rowValues[ProductIDColumn.index];
            if (cCmbProduct.GetSelectedIndex() > -1 || cellInfo.text != cCmbProduct.GetText()) {
                cellInfo.value = cCmbProduct.GetValue();
                cellInfo.text = cCmbProduct.GetText();
                cCmbProduct.SetValue(null);
            }
        }

        function TaxAmountKeyDown(s, e) {

            if (e.htmlEvent.key == "Enter") {
                s.OnButtonClick(0);
            }
        }

        var taxAmountGlobal;
        function taxAmountGotFocus(s, e) {
            taxAmountGlobal = parseFloat(s.GetValue());
        }
        function taxAmountLostFocus(s, e) {
            var finalTaxAmt = parseFloat(s.GetValue());
            var totAmt = parseFloat(ctxtTaxTotAmt.GetText());
            var totLength = cgridTax.GetEditor("Taxes_Name").GetText().length;
            var sign = cgridTax.GetEditor("Taxes_Name").GetText().substring(totLength - 3);
            if (sign == '(+)') {
                ctxtTaxTotAmt.SetValue(Math.round(totAmt + finalTaxAmt - taxAmountGlobal));
            } else {
                ctxtTaxTotAmt.SetValue(Math.round(totAmt + (finalTaxAmt * -1) - (taxAmountGlobal * -1)));
            }


            SetOtherTaxValueOnRespectiveRow(0, cgridTax.GetEditor("Amount").GetValue(), cgridTax.GetEditor("Taxes_Name").GetText().replace('(+)', '').replace('(-)', ''));
            //Set Running Total
            SetRunningTotal();

            RecalCulateTaxTotalAmountInline();
        }

        function cmbGstCstVatChange(s, e) {

            SetOtherTaxValueOnRespectiveRow(0, 0, gstcstvatGlobalName);
            $('.RecalculateInline').hide();
            var ProdAmt = parseFloat(ctxtprodBasicAmt.GetValue());
            if (s.GetValue().split('~')[2] == 'G') {
                ProdAmt = parseFloat(clblTaxProdGrossAmt.GetValue());
            }
            else if (s.GetValue().split('~')[2] == 'N') {
                ProdAmt = parseFloat(clblProdNetAmt.GetValue());
            }
            else if (s.GetValue().split('~')[2] == 'O') {
                //Check for Other Dependecy
                $('.RecalculateInline').show();
                ProdAmt = 0;
                var taxdependentName = s.GetValue().split('~')[3];
                for (var i = 0; i < taxJson.length; i++) {
                    cgridTax.batchEditApi.StartEdit(i, 3);
                    var gridTaxName = cgridTax.GetEditor("Taxes_Name").GetText();
                    gridTaxName = gridTaxName.substring(0, gridTaxName.length - 3).trim();
                    if (gridTaxName == taxdependentName) {
                        ProdAmt = cgridTax.GetEditor("Amount").GetValue();
                    }
                }
            }
            else if (s.GetValue().split('~')[2] == 'R') {
                ProdAmt = GetTotalRunningAmount();
                $('.RecalculateInline').show();
            }

            GlobalCurTaxAmt = parseFloat(ctxtGstCstVat.GetText());

            var calculatedValue = parseFloat(ProdAmt * ccmbGstCstVat.GetValue().split('~')[1]) / 100;
            ctxtGstCstVat.SetValue(calculatedValue);

            var totAmt = parseFloat(ctxtTaxTotAmt.GetText());
            ctxtTaxTotAmt.SetValue(Math.round(totAmt + calculatedValue - GlobalCurTaxAmt));

            //tax others
            SetOtherTaxValueOnRespectiveRow(0, calculatedValue, ccmbGstCstVat.GetText());
            gstcstvatGlobalName = ccmbGstCstVat.GetText();
        }


        //for tax and charges
        function Onddl_VatGstCstEndCallback(s, e) {
            if (s.GetItemCount() == 1) {
                cddlVatGstCst.SetEnabled(false);
            }
        }
        var GlobalCurChargeTaxAmt;
        var ChargegstcstvatGlobalName;
        function ChargecmbGstCstVatChange(s, e) {

            SetOtherChargeTaxValueOnRespectiveRow(0, 0, ChargegstcstvatGlobalName);
            $('.RecalculateCharge').hide();
            var ProdAmt = parseFloat(ctxtProductAmount.GetValue());

            //Set ProductAmount
            if (s.GetValue().split('~')[2] == 'G') {
                ProdAmt = parseFloat(ctxtProductAmount.GetValue());
            }
            else if (s.GetValue().split('~')[2] == 'N') {
                ProdAmt = parseFloat(clblProdNetAmt.GetValue());
            }
            else if (s.GetValue().split('~')[2] == 'O') {
                //Check for Other Dependecy
                $('.RecalculateCharge').show();
                ProdAmt = 0;
                var taxdependentName = s.GetValue().split('~')[3];
                for (var i = 0; i < taxJson.length; i++) {
                    gridTax.batchEditApi.StartEdit(i, 3);
                    var gridTaxName = gridTax.GetEditor("TaxName").GetText();
                    gridTaxName = gridTaxName.substring(0, gridTaxName.length - 3).trim();
                    if (gridTaxName == taxdependentName) {
                        ProdAmt = gridTax.GetEditor("Amount").GetValue();
                    }
                }
            }
            else if (s.GetValue().split('~')[2] == 'R') {
                $('.RecalculateCharge').show();
                ProdAmt = GetChargesTotalRunningAmount();
            }


            GlobalCurChargeTaxAmt = parseFloat(ctxtGstCstVatCharge.GetText());

            var calculatedValue = parseFloat(ProdAmt * ccmbGstCstVatcharge.GetValue().split('~')[1]) / 100;
            ctxtGstCstVatCharge.SetValue(calculatedValue);

            var totAmt = parseFloat(ctxtQuoteTaxTotalAmt.GetText());
            ctxtQuoteTaxTotalAmt.SetValue(totAmt + calculatedValue - GlobalCurChargeTaxAmt);

            //tax others
            SetOtherChargeTaxValueOnRespectiveRow(0, calculatedValue, ctxtGstCstVatCharge.GetText());
            ChargegstcstvatGlobalName = ctxtGstCstVatCharge.GetText();

            //set Total Amount
            ctxtTotalAmount.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(ctxtProductNetAmount.GetValue()));
        }




        function GetChargesTotalRunningAmount() {
            var runningTot = parseFloat(ctxtProductNetAmount.GetValue());
            for (var i = 0; i < chargejsonTax.length; i++) {
                gridTax.batchEditApi.StartEdit(i, 3);
                runningTot = runningTot + parseFloat(gridTax.GetEditor("Amount").GetValue());
                gridTax.batchEditApi.EndEdit();
            }

            return runningTot;
        }

        function chargeCmbtaxClick(s, e) {
            GlobalCurChargeTaxAmt = parseFloat(ctxtGstCstVatCharge.GetText());
            ChargegstcstvatGlobalName = s.GetText();
        }

        var GlobalCurTaxAmt = 0;
        var rowEditCtrl;
        var globalRowIndex;
        var globalTaxRowIndex;
        function GetVisibleIndex(s, e) {
            globalRowIndex = e.visibleIndex;
        }
        function GetTaxVisibleIndex(s, e) {
            globalTaxRowIndex = e.visibleIndex;
        }
        function cmbtaxCodeindexChange(s, e) {
            if (cgridTax.GetEditor("Taxes_Name").GetText() == "GST/CST/VAT") {

                var taxValue = s.GetValue();

                if (taxValue == null) {
                    taxValue = 0;
                    GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                    cgridTax.GetEditor("Amount").SetValue(0);
                    ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) - GlobalCurTaxAmt));
                }


                var isValid = taxValue.indexOf('~');
                if (isValid != -1) {
                    var rate = parseFloat(taxValue.split('~')[1]);
                    var ProdAmt = parseFloat(ctxtprodBasicAmt.GetValue());

                    GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());


                    cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * rate) / 100);
                    ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * rate) / 100) - GlobalCurTaxAmt));
                    GlobalCurTaxAmt = 0;
                }
                else {
                    s.SetText("");
                }

            } else {
                var ProdAmt = parseFloat(ctxtprodBasicAmt.GetValue());

                if (s.GetValue() == null) {
                    s.SetValue(0);
                }

                if (!isNaN(parseFloat(ProdAmt * s.GetText()) / 100)) {

                    GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                    cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * s.GetText()) / 100);

                    ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) - GlobalCurTaxAmt));
                    GlobalCurTaxAmt = 0;
                } else {
                    s.SetText("");
                }
            }

        }

        function SetOtherTaxValueOnRespectiveRow(idx, amt, name) {
            for (var i = 0; i < taxJson.length; i++) {
                if (taxJson[i].applicableBy == name) {
                    cgridTax.batchEditApi.StartEdit(i, 3);
                    cgridTax.GetEditor('calCulatedOn').SetValue(amt);

                    var totLength = cgridTax.GetEditor("Taxes_Name").GetText().length;
                    var taxNameWithSign = cgridTax.GetEditor("TaxField").GetText();
                    var sign = cgridTax.GetEditor("Taxes_Name").GetText().substring(totLength - 3);
                    var ProdAmt = parseFloat(cgridTax.GetEditor("calCulatedOn").GetValue());
                    var s = cgridTax.GetEditor("TaxField");
                    if (sign == '(+)') {
                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * s.GetText()) / 100);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) - GlobalCurTaxAmt));
                        GlobalCurTaxAmt = 0;
                    }
                    else {

                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue((parseFloat(ProdAmt * s.GetText()) / 100) * -1);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + ((parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) * -1) - (GlobalCurTaxAmt * -1)));
                        GlobalCurTaxAmt = 0;
                    }




                }
            }
            //return;
            cgridTax.batchEditApi.EndEdit();

        }



        function SetOtherChargeTaxValueOnRespectiveRow(idx, amt, name) {
            name = name.substring(0, name.length - 3).trim();
            for (var i = 0; i < chargejsonTax.length; i++) {
                if (chargejsonTax[i].applicableBy == name) {
                    gridTax.batchEditApi.StartEdit(i, 3);
                    gridTax.GetEditor('calCulatedOn').SetValue(amt);

                    var totLength = gridTax.GetEditor("TaxName").GetText().length;
                    var taxNameWithSign = gridTax.GetEditor("Percentage").GetText();
                    var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
                    var ProdAmt = parseFloat(gridTax.GetEditor("calCulatedOn").GetValue());
                    var s = gridTax.GetEditor("Percentage");
                    if (sign == '(+)') {
                        GlobalCurTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
                        gridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * s.GetText()) / 100);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) - GlobalCurTaxAmt));
                        GlobalCurTaxAmt = 0;
                    }
                    else {

                        GlobalCurTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
                        gridTax.GetEditor("Amount").SetValue((parseFloat(ProdAmt * s.GetText()) / 100) * -1);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + ((parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) * -1) - (GlobalCurTaxAmt * -1)));
                        GlobalCurTaxAmt = 0;
                    }




                }
            }
            //return;
            gridTax.batchEditApi.EndEdit();
        }



        function txtPercentageLostFocus(s, e) {

            //var ProdAmt = parseFloat(ctxtprodBasicAmt.GetValue());
            var ProdAmt = parseFloat(cgridTax.GetEditor("calCulatedOn").GetValue());
            if (s.GetText().trim() != '') {

                if (!isNaN(parseFloat(ProdAmt * s.GetText()) / 100)) {
                    //Checking Add or less
                    var totLength = cgridTax.GetEditor("Taxes_Name").GetText().length;
                    var taxNameWithSign = cgridTax.GetEditor("TaxField").GetText();
                    var sign = cgridTax.GetEditor("Taxes_Name").GetText().substring(totLength - 3);
                    if (sign == '(+)') {
                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * s.GetText()) / 100);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) - GlobalCurTaxAmt));
                        GlobalCurTaxAmt = 0;
                    }
                    else {

                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue((parseFloat(ProdAmt * s.GetText()) / 100) * -1);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + ((parseFloat(ProdAmt * parseFloat(s.GetText())) / 100) * -1) - (GlobalCurTaxAmt * -1)));
                        GlobalCurTaxAmt = 0;
                    }
                    SetOtherTaxValueOnRespectiveRow(0, cgridTax.GetEditor("Amount").GetValue(), cgridTax.GetEditor("Taxes_Name").GetText().replace('(+)', '').replace('(-)', ''));

                    //Call for Running Total
                    SetRunningTotal();

                } else {
                    s.SetText("");
                }
            }

            RecalCulateTaxTotalAmountInline();
        }

        function SetRunningTotal() {
            var runningTot = parseFloat(clblProdNetAmt.GetValue());
            for (var i = 0; i < taxJson.length; i++) {
                cgridTax.batchEditApi.StartEdit(i, 3);
                if (taxJson[i].applicableOn == "R") {
                    cgridTax.GetEditor("calCulatedOn").SetValue(runningTot);
                    var totLength = cgridTax.GetEditor("Taxes_Name").GetText().length;
                    var taxNameWithSign = cgridTax.GetEditor("TaxField").GetText();
                    var sign = cgridTax.GetEditor("Taxes_Name").GetText().substring(totLength - 3);
                    var ProdAmt = parseFloat(cgridTax.GetEditor("calCulatedOn").GetValue());
                    var thisRunningAmt = 0;
                    if (sign == '(+)') {
                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * cgridTax.GetEditor("TaxField").GetValue()) / 100);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + (parseFloat(ProdAmt * parseFloat(cgridTax.GetEditor("TaxField").GetValue())) / 100) - GlobalCurTaxAmt));
                        GlobalCurTaxAmt = 0;
                    }
                    else {

                        GlobalCurTaxAmt = parseFloat(cgridTax.GetEditor("Amount").GetValue());
                        cgridTax.GetEditor("Amount").SetValue((parseFloat(ProdAmt * cgridTax.GetEditor("TaxField").GetValue()) / 100) * -1);

                        ctxtTaxTotAmt.SetValue(Math.round(parseFloat(ctxtTaxTotAmt.GetValue()) + ((parseFloat(ProdAmt * parseFloat(cgridTax.GetEditor("TaxField").GetValue())) / 100) * -1) - (GlobalCurTaxAmt * -1)));
                        GlobalCurTaxAmt = 0;
                    }
                    SetOtherTaxValueOnRespectiveRow(0, cgridTax.GetEditor("Amount").GetValue(), cgridTax.GetEditor("Taxes_Name").GetText().replace('(+)', '').replace('(-)', ''));
                }
                runningTot = runningTot + parseFloat(cgridTax.GetEditor("Amount").GetValue());
                cgridTax.batchEditApi.EndEdit();
            }
        }

        function GetTotalRunningAmount() {
            var runningTot = parseFloat(clblProdNetAmt.GetValue());
            for (var i = 0; i < taxJson.length; i++) {
                cgridTax.batchEditApi.StartEdit(i, 3);
                runningTot = runningTot + parseFloat(cgridTax.GetEditor("Amount").GetValue());
                cgridTax.batchEditApi.EndEdit();
            }

            return runningTot;
        }



        var gstcstvatGlobalName;
        function CmbtaxClick(s, e) {
            GlobalCurTaxAmt = parseFloat(ctxtGstCstVat.GetText());
            gstcstvatGlobalName = s.GetText();
        }


        function txtTax_TextChanged(s, i, e) {
            cgridTax.batchEditApi.StartEdit(i, 2);
            var ProdAmt = parseFloat(ctxtprodBasicAmt.GetValue());
            cgridTax.GetEditor("Amount").SetValue(parseFloat(ProdAmt * s.GetText()) / 100);
        }

        function taxAmtButnClick(s, e) {
            if (e.buttonIndex == 0) {

                if (cddl_AmountAre.GetValue() != null) {
                    var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";

                    if (ProductID.trim() != "") {

                        document.getElementById('setCurrentProdCode').value = ProductID.split('||')[0];
                        document.getElementById('HdSerialNo').value = grid.GetEditor('SrlNo').GetText();
                        ctxtTaxTotAmt.SetValue(0);
                        ccmbGstCstVat.SetSelectedIndex(0);
                        $('.RecalculateInline').hide();
                        caspxTaxpopUp.Show();
                        //Set Product Gross Amount and Net Amount

                        var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
                        var SpliteDetails = ProductID.split("||@||");
                        var strMultiplier = SpliteDetails[7];
                        var strFactor = SpliteDetails[8];
                        var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";
                        //var strRate = "1";
                        var strStkUOM = SpliteDetails[4];
                        // var strSalePrice = SpliteDetails[6];
                        var strSalePrice = (grid.GetEditor('SalePrice').GetValue() != null) ? grid.GetEditor('SalePrice').GetValue() : "";
                        if (strRate == 0) {
                            strRate = 1;
                        }

                        var AvgOldUnitAmount = parseFloat(grid.GetEditor("ProdAvgvalue").GetValue());

                        var StockQuantity = strMultiplier * QuantityValue;
                        var Amount = DecimalRoundoff((QuantityValue * strFactor * (strSalePrice / strRate)) - (AvgOldUnitAmount * QuantityValue),2);
                        clblTaxProdGrossAmt.SetText(Amount);
                        clblProdNetAmt.SetText(DecimalRoundoff(grid.GetEditor('Amount').GetValue() - (AvgOldUnitAmount * QuantityValue), 2));
                        document.getElementById('HdProdGrossAmt').value = Amount;
                        document.getElementById('HdProdNetAmt').value = DecimalRoundoff(grid.GetEditor('Amount').GetValue() - (AvgOldUnitAmount * QuantityValue),2);

                        //End Here

                        //Set Discount Here
                        if (parseFloat(grid.GetEditor('Discount').GetValue()) > 0) {
                            var discount = Math.round((Amount * grid.GetEditor('Discount').GetValue() / 100)).toFixed(2);
                            clblTaxDiscount.SetText(discount);
                        }
                        else {
                            clblTaxDiscount.SetText('0.00');
                        }
                        //End Here 


                        //Checking is gstcstvat will be hidden or not
                        if (cddl_AmountAre.GetValue() == "2") {
                            $('.GstCstvatClass').hide();
                            $('.gstGrossAmount').show();
                            clblTaxableGross.SetText("(Taxable)");
                            clblTaxableNet.SetText("(Taxable)");
                            $('.gstNetAmount').show();
                            //Set Gross Amount with GstValue
                            //Get The rate of Gst
                            var gstRate = parseFloat(cddlVatGstCst.GetValue().split('~')[1]);
                            if (gstRate) {
                                if (gstRate != 0) {
                                    var gstDis = (gstRate / 100) + 1;
                                    if (cddlVatGstCst.GetValue().split('~')[2] == "G") {
                                        $('.gstNetAmount').hide();
                                        clblTaxProdGrossAmt.SetText(Math.round(Amount / gstDis).toFixed(2));
                                        document.getElementById('HdProdGrossAmt').value = Math.round(Amount / gstDis).toFixed(2);
                                        clblGstForGross.SetText(Math.round(Amount - parseFloat(document.getElementById('HdProdGrossAmt').value)).toFixed(2));
                                        clblTaxableNet.SetText("");
                                    }
                                    else {
                                        $('.gstGrossAmount').hide();
                                        clblProdNetAmt.SetText(Math.round(grid.GetEditor('Amount').GetValue() / gstDis).toFixed(2));
                                        document.getElementById('HdProdNetAmt').value = Math.round(grid.GetEditor('Amount').GetValue() / gstDis).toFixed(2);
                                        clblGstForNet.SetText(Math.round(grid.GetEditor('Amount').GetValue() - parseFloat(document.getElementById('HdProdNetAmt').value)).toFixed(2));
                                        clblTaxableGross.SetText("");
                                    }
                                }


                            } else {
                                $('.gstGrossAmount').hide();
                                $('.gstNetAmount').hide();
                                clblTaxableGross.SetText("");
                                clblTaxableNet.SetText("");
                            }
                        }
                        else if (cddl_AmountAre.GetValue() == "1") {
                            $('.GstCstvatClass').show();
                            $('.gstGrossAmount').hide();
                            $('.gstNetAmount').hide();
                            clblTaxableGross.SetText("");
                            clblTaxableNet.SetText("");

                            var shippingStCode = '';

                            ////###### Added By : Samrat Roy ##########
                            //Get Customer Shipping StateCode
                            var shippingStCode = '';

                            var shippingStCode = '';
                            shippingStCode = cbsSCmbState.GetText();
                            shippingStCode = shippingStCode.substring(shippingStCode.lastIndexOf('(')).replace('(State Code:', '').replace(')', '').trim();

                            ////// ###########  Old Code #####################
                            ////if (cchkBilling.GetValue()) {
                            ////    shippingStCode = CmbState.GetText();
                            ////}
                            ////else {
                            ////    shippingStCode = CmbState1.GetText();
                            ////}
                            ////shippingStCode = shippingStCode.substring(shippingStCode.lastIndexOf('(')).replace('(State Code:', '').replace(')', '').trim();

                            ////###### END : Samrat Roy : END ########## 

                            //Debjyoti 09032017
                            if (shippingStCode.trim() != '') {
                                for (var cmbCount = 1; cmbCount < ccmbGstCstVat.GetItemCount() ; cmbCount++) {
                                    //Check if gstin is blank then delete all tax
                                    if (ccmbGstCstVat.GetItem(cmbCount).value.split('~')[5] != "") {

                                        if (ccmbGstCstVat.GetItem(cmbCount).value.split('~')[5] == shippingStCode) {

                                            //if its state is union territories then only UTGST will apply
                                            if (shippingStCode == "4" || shippingStCode == "26" || shippingStCode == "25" || shippingStCode == "35" || shippingStCode == "31" || shippingStCode == "34") {
                                                if (ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'I' || ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'C' || ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'S') {
                                                    ccmbGstCstVat.RemoveItem(cmbCount);
                                                    cmbCount--;
                                                }
                                            }
                                            else {
                                                if (ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'I' || ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'U') {
                                                    ccmbGstCstVat.RemoveItem(cmbCount);
                                                    cmbCount--;
                                                }
                                            }
                                        } else {
                                            if (ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'S' || ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'C' || ccmbGstCstVat.GetItem(cmbCount).value.split('~')[4] == 'U') {
                                                ccmbGstCstVat.RemoveItem(cmbCount);
                                                cmbCount--;
                                            }
                                        }
                                    } else {
                                        //remove tax because GSTIN is not define
                                        ccmbGstCstVat.RemoveItem(cmbCount);
                                        cmbCount--;
                                    }
                                }
                            }




                        }
                        //End here

                        if (globalRowIndex > -1) {
                            cgridTax.PerformCallback(grid.keys[globalRowIndex] + '~' + cddl_AmountAre.GetValue());
                        } else {

                            cgridTax.PerformCallback('New~' + cddl_AmountAre.GetValue());
                            //Set default combo
                            cgridTax.cpComboCode = grid.GetEditor('ProductID').GetValue().split('||@||')[9];
                        }

                        ctxtprodBasicAmt.SetValue(grid.GetEditor('Amount').GetValue());
                    } else {
                        grid.batchEditApi.StartEdit(globalRowIndex, 13);
                    }
                }
            }
        }
        function taxAmtButnClick1(s, e) {
            console.log(grid.GetFocusedRowIndex());
            rowEditCtrl = s;
        }

        function BatchUpdate() {

            //cgridTax.batchEditApi.StartEdit(0, 1);

            //if (cgridTax.GetEditor("TaxField").GetText().indexOf('.') == -1) {
            //    cgridTax.GetEditor("TaxField").SetText(cgridTax.GetEditor("TaxField").GetText().trim() + '.00');
            //} else {
            //    cgridTax.GetEditor("TaxField").SetText(cgridTax.GetEditor("TaxField").GetText().trim() + '0');
            //}
            if (cgridTax.GetVisibleRowsOnPage() > 0) {
                cgridTax.UpdateEdit();
            }
            else {
                cgridTax.PerformCallback('SaveGST');
            }
            return false;
        }

        var taxJson;
        function cgridTax_EndCallBack(s, e) {
            //cgridTax.batchEditApi.StartEdit(0, 1);
            $('.cgridTaxClass').show();

            cgridTax.StartEditRow(0);


            //check Json data
            if (cgridTax.cpJsonData) {
                if (cgridTax.cpJsonData != "") {
                    taxJson = JSON.parse(cgridTax.cpJsonData);
                    cgridTax.cpJsonData = null;
                }
            }
            //End Here

            if (cgridTax.cpComboCode) {
                if (cgridTax.cpComboCode != "") {
                    if (cddl_AmountAre.GetValue() == "1") {
                        var selectedIndex;
                        for (var i = 0; i < ccmbGstCstVat.GetItemCount() ; i++) {
                            if (ccmbGstCstVat.GetItem(i).value.split('~')[0] == cgridTax.cpComboCode) {
                                selectedIndex = i;
                            }
                        }
                        if (selectedIndex && ccmbGstCstVat.GetItem(selectedIndex) != null) {
                            ccmbGstCstVat.SetValue(ccmbGstCstVat.GetItem(selectedIndex).value);
                        }
                        cmbGstCstVatChange(ccmbGstCstVat);
                        cgridTax.cpComboCode = null;
                    }
                }
            }

            if (cgridTax.cpUpdated.split('~')[0] == 'ok') {
                ctxtTaxTotAmt.SetValue(Math.round(cgridTax.cpUpdated.split('~')[1]));
                var gridValue = parseFloat(cgridTax.cpUpdated.split('~')[1]);
                var ddValue = parseFloat(ctxtGstCstVat.GetValue());
                ctxtTaxTotAmt.SetValue(Math.round(gridValue + ddValue));
                cgridTax.cpUpdated = "";
            }

            else {
                var totAmt = ctxtTaxTotAmt.GetValue();
                cgridTax.CancelEdit();
                caspxTaxpopUp.Hide();
                grid.batchEditApi.StartEdit(globalRowIndex, 13);

                grid.GetEditor("TaxAmount").SetValue(totAmt); 
                var totalNetAmount = parseFloat(totAmt) + parseFloat(grid.GetEditor("Amount").GetValue());
                 

                var OldTotalAmount = parseFloat(grid.GetEditor("TotalAmount").GetValue());
                if (cddl_AmountAre.GetValue() == "1")
                    grid.GetEditor("TotalAmount").SetValue(totalNetAmount);
                else if (cddl_AmountAre.GetValue() == "2") {
                    var diffamount = parseFloat(grid.GetEditor("TotalAmount").GetValue()) - totalNetAmount;
                    grid.GetEditor("Amount").SetValue(parseFloat(grid.GetEditor("Amount").GetValue()) + diffamount);
                }
            }

            if (cgridTax.GetVisibleRowsOnPage() == 0) {
                $('.cgridTaxClass').hide();
                ccmbGstCstVat.Focus();
            }
            //Debjyoti Check where any Gst Present or not
            // If Not then hide the hole section

            SetRunningTotal();
            ShowTaxPopUp("IY");
            RecalCulateTaxTotalAmountInline();
        }

        function recalculateTax() {
            cmbGstCstVatChange(ccmbGstCstVat);
        }
        function recalculateTaxCharge() {
            ChargecmbGstCstVatChange(ccmbGstCstVatcharge);
        }

    </script>
    <%--Debu Section End--%>


    <%--Sam Section Start--%>
    <script type="text/javascript">
        $(document).ready(function () {
            cProductsPopup.Hide();
            if (GetObjectID('hdnCustomerId').value == null || GetObjectID('hdnCustomerId').value == '') {
                page.GetTabByName('Billing/Shipping').SetEnabled(false);
            }
            $('#ApprovalCross').click(function () {

                window.parent.popup.Hide();
                window.parent.cgridPendingApproval.Refresh()();
            })
        })
        var globalQty;
        var globalSalePrice;
        var globalDisc;
        var globalAmt;

        function UniqueCodeCheck() {

            var QuoteNo = ctxt_PLQuoteNo.GetText();
            if (QuoteNo != '') {
                var CheckUniqueCode = false;
                $.ajax({
                    type: "POST",
                    url: "OldUnit_SalesInvoice.aspx/CheckUniqueCode",
                    data: JSON.stringify({ QuoteNo: QuoteNo }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {
                        CheckUniqueCode = msg.d;
                        if (CheckUniqueCode == true) {
                            //jAlert('Please enter unique PI/Quotation number');
                            $('#duplicateQuoteno').attr('style', 'display:block');
                            ctxt_PLQuoteNo.SetValue('');
                            ctxt_PLQuoteNo.Focus();
                        }
                        else {
                            $('#duplicateQuoteno').attr('style', 'display:none');
                        }
                    }
                });
            }
        }
        function CloseGridLookup() {
            gridLookup.ConfirmCurrentSelection();
            gridLookup.HideDropDown();
            gridLookup.Focus();
        }
        function GetContactPerson(e) {
            if (gridquotationLookup.GetValue() != null) {
                jConfirm('Documents tagged are to be automatically De-selected. Confirm ?', 'Confirmation Dialog', function (r) {

                    if (r == true) {
                        var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                        if (key != null && key != '') {

                            cContactPerson.PerformCallback('BindContactPerson~' + key);
                            page.GetTabByName('Billing/Shipping').SetEnabled(true);

                            var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                            var startDate = new Date();
                            startDate = tstartdate.GetValueString();

                            if (type != "") {
                                cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + '%' + '~' + type);
                            }

                            var componentType = gridquotationLookup.GetValue();//gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());
                            if (componentType != null && componentType != '') {
                                grid.PerformCallback('GridBlank');
                                $('#<%=hdnPageStatus.ClientID %>').val('update');
                            }

                            //###### Added By : Samrat Roy ##########
                            //cchkBilling.SetChecked(false);
                            //cchkShipping.SetChecked(false);
                            //page.SetActiveTabIndex(1);
                            //$('.dxeErrorCellSys').addClass('abc');
                            //$('.crossBtn').hide();
                            //page.GetTabByName('General').SetEnabled(false);

                            LoadCustomerAddress(key, $('#ddl_Branch').val(), 'SI');
                            GetObjectID('hdnCustomerId').value = key;
                            if ($('#hfBSAlertFlag').val() == "1") {
                                jConfirm('Wish to View/Select Billing and Shipping details?', 'Confirmation Dialog', function (r) {
                                    if (r == true) {
                                        page.SetActiveTabIndex(1);
                                        cbsSave_BillingShipping.Focus();
                                        page.tabs[0].SetEnabled(false);
                                        $("#divcross").hide();
                                    }
                                });
                            }
                            else {
                                page.SetActiveTabIndex(1);
                                cbsSave_BillingShipping.Focus();
                                page.tabs[0].SetEnabled(false);
                                $("#divcross").hide();
                            }
                            //###### END : Samrat Roy : END ########## 


                            GetObjectID('hdnAddressDtl').value = '0';
                        }
                    }
                });
            }
            else {
                var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                if (key != null && key != '') {

                    cContactPerson.PerformCallback('BindContactPerson~' + key);
                    page.GetTabByName('Billing/Shipping').SetEnabled(true);

                    var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                    var startDate = new Date();
                    startDate = tstartdate.GetValueString();

                    if (type != "") {
                        cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + '%' + '~' + type);
                    }

                    var componentType = gridquotationLookup.GetValue();//gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());
                    if (componentType != null && componentType != '') {
                        grid.PerformCallback('GridBlank');
                        $('#<%=hdnPageStatus.ClientID %>').val('update');
                    }

                    //###### Added By : Samrat Roy ##########
                    //cchkBilling.SetChecked(false);
                    //cchkShipping.SetChecked(false);
                    //page.SetActiveTabIndex(1);
                    //$('.dxeErrorCellSys').addClass('abc');
                    //$('.crossBtn').hide();
                    //page.GetTabByName('General').SetEnabled(false);

                    LoadCustomerAddress(key, $('#ddl_Branch').val(), 'SI');
                    GetObjectID('hdnCustomerId').value = key;
                    if ($('#hfBSAlertFlag').val() == "1") {
                        jConfirm('Wish to View/Select Billing and Shipping details?', 'Confirmation Dialog', function (r) {
                            if (r == true) {
                                page.SetActiveTabIndex(1);
                                cbsSave_BillingShipping.Focus();
                                page.tabs[0].SetEnabled(false);
                                $("#divcross").hide();
                            }
                        });
                    }
                    else {
                        page.SetActiveTabIndex(1);
                        cbsSave_BillingShipping.Focus();
                        page.tabs[0].SetEnabled(false);
                        $("#divcross").hide();
                    }
                    //###### END : Samrat Roy : END ########## 
                }
            }
        }
        $(document).ready(function () {
            var schemaid = $('#ddl_numberingScheme').val();
            if (schemaid != null) {
                if (schemaid == '') {
                    ctxt_PLQuoteNo.SetEnabled(false);
                }
            }
            $('#ddl_numberingScheme').change(function () {
                var NoSchemeTypedtl = $(this).val();
                var NoSchemeType = NoSchemeTypedtl.toString().split('~')[1];
                var quotelength = NoSchemeTypedtl.toString().split('~')[2];

                var fromdate = NoSchemeTypedtl.toString().split('~')[5];
                var todate = NoSchemeTypedtl.toString().split('~')[6];

                var dt = new Date();

                tstartdate.SetDate(dt);

                if (dt < new Date(fromdate)) {
                    tstartdate.SetDate(new Date(fromdate));
                }

                if (dt > new Date(todate)) {
                    tstartdate.SetDate(new Date(todate));
                }




                tstartdate.SetMinDate(new Date(fromdate));
                tstartdate.SetMaxDate(new Date(todate));

                var branchID = (NoSchemeTypedtl.toString().split('~')[3] != null) ? NoSchemeTypedtl.toString().split('~')[3] : "";
                if (branchID != "") {
                    document.getElementById('ddl_Branch').value = branchID;
                    $('#HdSelectedBranch').val(branchID);
                }
                loadMainAccountByBranchIdForPayDet(branchID);
                cddlCashBank.PerformCallback();

                //ctxt_PLQuoteNo.SetMaxLength(quotelength);
                if (NoSchemeType == '1') {
                    ctxt_PLQuoteNo.SetText('Auto');
                    ctxt_PLQuoteNo.SetEnabled(false);
                    //ctxt_PLQuoteNo.SetClientEnabled(false);

                    tstartdate.Focus();
                }
                else if (NoSchemeType == '0') {
                    ctxt_PLQuoteNo.SetEnabled(true);
                    ctxt_PLQuoteNo.GetInputElement().maxLength = quotelength;
                    //ctxt_PLQuoteNo.SetClientEnabled(true);
                    ctxt_PLQuoteNo.SetText('');
                    ctxt_PLQuoteNo.Focus();
                }
                else if (NoSchemeType == '2') {
                    ctxt_PLQuoteNo.SetText('Datewise');
                    ctxt_PLQuoteNo.SetEnabled(false);
                    //ctxt_PLQuoteNo.SetClientEnabled(false);

                    tstartdate.Focus();
                }
                else {
                    ctxt_PLQuoteNo.SetText('');
                    ctxt_PLQuoteNo.SetEnabled(false);
                    //ctxt_PLQuoteNo.SetClientEnabled(true);
                }
            });

            $('#ddl_Currency').change(function () {

                var CurrencyId = $(this).val();
                var LocalCurrency = '<%=Session["LocalCurrency"]%>';
                var basedCurrency = LocalCurrency.split("~")[0];
                // var Currency_ID = $("#ddl_Currency").val();
                //  alert(basedCurrency);
                if ($("#ddl_Currency").val() == basedCurrency) {
                    ctxt_Rate.SetValue("");
                    ctxt_Rate.SetEnabled(false);
                }
                else {
                    if (basedCurrency != CurrencyId) {
                        if (LocalCurrency != null) {
                            if (CurrencyId != '0') {
                                $.ajax({
                                    type: "POST",
                                    url: "OldUnit_SalesInvoice.aspx/GetCurrentConvertedRate",
                                    data: "{'CurrencyId':'" + CurrencyId + "'}",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (msg) {
                                        var currentRate = msg.d;
                                        if (currentRate != null) {

                                            ctxt_Rate.SetValue(currentRate);
                                        }
                                        else {
                                            ctxt_Rate.SetValue('1');
                                        }
                                        ReBindGrid_Currency();
                                    }
                                });
                            }
                            else {
                                ctxt_Rate.SetValue("1");
                                ReBindGrid_Currency();
                            }
                        }
                    }
                    else {
                        ctxt_Rate.SetValue("1");
                        ReBindGrid_Currency();
                    }
                    ctxt_Rate.SetEnabled(true);
                }
            });
        });

        function SetFocusonDemand(e) {
            var key = cddl_AmountAre.GetValue();
            if (key == '1' || key == '3') {
                if (grid.GetVisibleRowsOnPage() == 1) {
                    grid.batchEditApi.StartEdit(-1, 2);
                }
            }
            else if (key == '2') {
                cddlVatGstCst.Focus();
            }

        }

        function PopulateGSTCSTVAT(e) {
            var key = cddl_AmountAre.GetValue();
            //deleteAllRows();

            if (key == 1) {

                grid.GetEditor('TaxAmount').SetEnabled(true);
                cddlVatGstCst.SetEnabled(false);
                //cddlVatGstCst.PerformCallback('1');
                cddlVatGstCst.SetSelectedIndex(0);
                cbtn_SaveRecords.SetVisible(true);
                grid.GetEditor('ProductID').Focus();
                if (grid.GetVisibleRowsOnPage() == 1) {
                    grid.batchEditApi.StartEdit(-1, 2);
                }

            }
            else if (key == 2) {
                grid.GetEditor('TaxAmount').SetEnabled(true);

                cddlVatGstCst.SetEnabled(true);
                cddlVatGstCst.PerformCallback('2');
                cddlVatGstCst.Focus();
                cbtn_SaveRecords.SetVisible(true);
            }
            else if (key == 3) {

                grid.GetEditor('TaxAmount').SetEnabled(false);

                //cddlVatGstCst.PerformCallback('3');
                cddlVatGstCst.SetSelectedIndex(0);
                cddlVatGstCst.SetEnabled(false);
                cbtn_SaveRecords.SetVisible(false);
                if (grid.GetVisibleRowsOnPage() == 1) {
                    grid.batchEditApi.StartEdit(-1, 2);
                }


            }

        }

        //Date Function Start

        function Startdate(s, e) {
            grid.batchEditApi.EndEdit();
            var frontRow = 0;
            var backRow = -1;
            var IsProduct = "";
            for (var i = 0; i <= grid.GetVisibleRowsOnPage() ; i++) {
                var frontProduct = (grid.batchEditApi.GetCellValue(backRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(backRow, 'ProductID')) : "";
                var backProduct = (grid.batchEditApi.GetCellValue(frontRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(frontRow, 'ProductID')) : "";

                if (frontProduct != "" || backProduct != "") {
                    IsProduct = "Y";
                    break;
                }

                backRow--;
                frontRow++;
            }


            var t = s.GetDate();
            ccmbGstCstVat.PerformCallback(t);
            ccmbGstCstVatcharge.PerformCallback(t);
            ctaxUpdatePanel.PerformCallback('DeleteAllTax');
            if (IsProduct == "Y") {
                $('#<%=hdfIsDelete.ClientID %>').val('D');
                $('#<%=HdUpdateMainGrid.ClientID %>').val('True');
                grid.UpdateEdit();
            }

            if (t == "")
            { $('#MandatorysDate').attr('style', 'display:block'); }
            else { $('#MandatorysDate').attr('style', 'display:none'); }
        }
        function Enddate(s, e) {

            var t = s.GetDate();
            if (t == "")
            { $('#MandatoryEDate').attr('style', 'display:block'); }
            else { $('#MandatoryEDate').attr('style', 'display:none'); }



            var sdate = tstartdate.GetValue();
            var edate = tenddate.GetValue();

            var startDate = new Date(sdate);
            var endDate = new Date(edate);

            if (startDate > endDate) {

                flag = false;
                $('#MandatoryEgSDate').attr('style', 'display:block');
            }
            else { $('#MandatoryEgSDate').attr('style', 'display:none'); }
        }

        //Date Function End

        // Popup Section

        function ShowCustom() {

            cPopup_wareHouse.Show();


        }

        // Popup Section End

    </script>
    <%--Sam Section End--%>

    <%--Sudip--%>
    <script>
        var IsProduct = "";
        var currentEditableVisibleIndex;
        var preventEndEditOnLostFocus = false;
        var lastProductID;
        var setValueFlag;

        function GridCallBack() {
            grid.PerformCallback('Display');
        }

        function ReBindGrid_Currency() {
            var frontRow = 0;
            var backRow = -1;
            var IsProduct = "";
            for (var i = 0; i <= grid.GetVisibleRowsOnPage() ; i++) {
                var frontProduct = (grid.batchEditApi.GetCellValue(backRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(backRow, 'ProductID')) : "";
                var backProduct = (grid.batchEditApi.GetCellValue(frontRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(frontRow, 'ProductID')) : "";

                if (frontProduct != "" || backProduct != "") {
                    IsProduct = "Y";
                    break;
                }

                backRow--;
                frontRow++;
            }

            if (IsProduct == "Y") {
                $('#<%=hdfIsDelete.ClientID %>').val('D');
                grid.UpdateEdit();
                grid.PerformCallback('CurrencyChangeDisplay');
            }
        }

        function ProductsCombo_SelectedIndexChanged(s, e) {
            pageheaderContent.style.display = "block";
            cddl_AmountAre.SetEnabled(false);

            var tbDescription = grid.GetEditor("Description");
            var tbUOM = grid.GetEditor("UOM");
            var tbSalePrice = grid.GetEditor("SalePrice");
            //var tbStkUOM = grid.GetEditor("StockUOM");
            //var tbStockQuantity = grid.GetEditor("StockQuantity");

            //var ProductID = (cCmbProduct.GetValue() != null) ? cCmbProduct.GetValue() : "0";
            //var strProductName = (cCmbProduct.GetText() != null) ? cCmbProduct.GetText() : "";
            var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var ProductID = (grid.GetEditor('ProductID').GetValue() != null) ? grid.GetEditor('ProductID').GetValue() : "0";
            var SpliteDetails = ProductID.split("||@||");
            var strProductID = SpliteDetails[0];
            var strDescription = SpliteDetails[1];
            var strUOM = SpliteDetails[2];
            var strStkUOM = SpliteDetails[4];
            var strSalePrice = SpliteDetails[6];
            strProductName = strDescription;

            var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
            var IsPackingActive = SpliteDetails[10];
            var Packing_Factor = SpliteDetails[11];
            var Packing_UOM = SpliteDetails[12];

            var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";
            if (strRate == 0) {
                strSalePrice = strSalePrice;
            }
            else {
                strSalePrice = strSalePrice / strRate;
            }

            tbDescription.SetValue(strDescription);
            tbUOM.SetValue(strUOM);
            tbSalePrice.SetValue(strSalePrice);

            grid.GetEditor("Quantity").SetValue("0.00");
            grid.GetEditor("Discount").SetValue("0.00");
            grid.GetEditor("Amount").SetValue("0.00");
            grid.GetEditor("TaxAmount").SetValue("0.00");
            grid.GetEditor("TotalAmount").SetValue("0.00");

            var ddlbranch = $("[id*=ddl_Branch]");
            var strBranch = ddlbranch.find("option:selected").text();

            $('#<%= lblStkQty.ClientID %>').text("0.00");
            $('#<%= lblStkUOM.ClientID %>').text(strStkUOM);
            $('#<%= lblProduct.ClientID %>').text(strProductName);
            $('#<%= lblbranchName.ClientID %>').text(strBranch);

            if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
                $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
                divPacking.style.display = "block";
            } else {
                divPacking.style.display = "none";
            }
            //divPacking.style.display = "none";

            //lblbranchName lblProduct
            //tbStkUOM.SetValue(strStkUOM);
            //tbStockQuantity.SetValue("0");
            //Debjyoti
            ctaxUpdatePanel.PerformCallback('DelProdbySl~' + grid.GetEditor("SrlNo").GetValue());
            //cacpAvailableStock.PerformCallback(strProductID);
        }
        function cmbContactPersonEndCall(s, e) {
            if (cContactPerson.cpDueDate != null) {
                var DeuDate = cContactPerson.cpDueDate;
                var myDate = new Date(DeuDate);

                var invoiceDate = new Date();
                var datediff = Math.round((myDate - invoiceDate) / (1000 * 60 * 60 * 24));

                ctxtCreditDays.SetValue(datediff);

                cdt_SaleInvoiceDue.SetDate(myDate);
                cContactPerson.cpDueDate = null;
            }

            if (cContactPerson.cpTotalDue != null) {
                var TotalDue = cContactPerson.cpTotalDue;
                var TotalCustDue = "";
                if (TotalDue >= 0) {
                    TotalCustDue = TotalDue + ' Db';
                    document.getElementById('<%=lblTotalDues.ClientID %>').style.color = "black";
                }
                else {
                    TotalDue = TotalDue * (-1);
                    TotalCustDue = TotalDue + ' Cr';
                    document.getElementById('<%=lblTotalDues.ClientID %>').style.color = "red";
                }

                document.getElementById('<%=lblTotalDues.ClientID %>').innerHTML = TotalCustDue;
                pageheaderContent.style.display = "block";
                divDues.style.display = "block";
                cContactPerson.cpTotalDue = null;
            }
        }
        function CreditDays_TextChanged(s, e) {
            var CreditDays = ctxtCreditDays.GetValue();

            var today = new Date();
            var newdate = new Date();
            newdate.setDate(today.getDate() + Math.round(CreditDays));

            cdt_SaleInvoiceDue.SetDate(newdate);
        }

        function OnEndCallback(s, e) {
            var value = document.getElementById('hdnRefreshType').value;
            document.getElementById('hdnRefreshType').value = "";

            //Debjyoti Check grid needs to be refreshed or not
            if ($('#<%=HdUpdateMainGrid.ClientID %>').val() == 'True') {
                $('#<%=HdUpdateMainGrid.ClientID %>').val('False');
                grid.PerformCallback('DateChangeDisplay');
            }

            if (grid.cpComponent) {
                if (grid.cpComponent == 'true') {
                    grid.cpComponent = null;
                    OnAddNewClick();
                }
            }

            LoadingPanel.Hide();
            if (grid.cpinsert == 'UDFMandatory') {
                OnAddNewClick();
                grid.cpinsert = null;
                jAlert("UDF is set as Mandatory. Please enter values.", "Alert", function () { OpenUdf(); });
            }
            else if (grid.cpSaveSuccessOrFail == "TCMandatory") {
                OnAddNewClick_AtSaveTime();
                grid.cpSaveSuccessOrFail = null;
                jAlert("Terms and Condition is set as Mandatory. Please enter values.", "Alert", function () { $("#TermsConditionseModal").modal('show'); });
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "transporteMandatory") {
                OnAddNewClick_AtSaveTime();
                grid.cpSaveSuccessOrFail = null;
                jAlert("Transporter is set as Mandatory. Please enter values.", "Alert", function () { $("#exampleModal").modal('show'); });
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "outrange") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Can Not Add More Quotation Number as Quotation Scheme Exausted.<br />Update The Scheme and Try Again');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "DueDateLess") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Due date must be greater than Today');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "duplicate") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Can Not Save as Duplicate Quotation Numbe No. Found');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "quantityTagged") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Proforma is tagged in Sale Order. So, Quantity of selected products cannot be less than Ordered Quantity.');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "errorInsert") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Please try after sometime.');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "nullAmount") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('total amount cant not be zero(0).');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "nullQuantity") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Please fill Quantity');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "nullCredit") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Credit Days must be greater than Zero(0)');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "duplicateProduct") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;
                jAlert('Can not Duplicate Product in the Quotation List.');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "minSalePriceMust") {
                OnAddNewClick();
                jAlert('Sale Price Should be equal or higher than Min Sale Price');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "MRPLess") {
                OnAddNewClick();
                jAlert('Sale Price Should be equal or less than MRP');
                grid.cpSaveSuccessOrFail = '';
            }
            else if (grid.cpSaveSuccessOrFail == "checkWarehouse") {
                OnAddNewClick();
                grid.cpSaveSuccessOrFail = null;

                var SrlNo = grid.cpProductSrlIDCheck;
                var msg = "Product Sales Quantity must be equal to Warehouse Quantity for SL No. " + SrlNo;
                jAlert(msg);
                grid.cpSaveSuccessOrFail = '';
            }
            else {
                var Quote_Number = grid.cpQuotationNo;
                var Quote_ID = grid.cpQuotationID;

                grid.cpQuotationNo = null;
                grid.cpQuotationID = null;

                var Quote_Msg = "Sales Invoice(Second Hand Sales) No. '" + Quote_Number + "' saved.";

                if (value == "E") {
                    if (grid.cpApproverStatus == "approve") {
                        window.parent.popup.Hide();
                        window.parent.cgridPendingApproval.PerformCallback();
                    }
                    else if (grid.cpApproverStatus == "rejected") {
                        window.parent.popup.Hide();
                        window.parent.cgridPendingApproval.PerformCallback();
                    }
                    else {
                        if (Quote_Number != "") {
                            //var strconfirm = alert(Quote_Msg);
                            //if (strconfirm == true) {
                            //    window.location.assign("OldUnit_SalesInvoiceList.aspx");
                            //}
                            //else {
                            //    window.location.assign("OldUnit_SalesInvoiceList.aspx");
                            //}
                           // window.open("../../Reports/REPXReports/RepxReportViewer.aspx?Previewrpt=SalesInvoice~D&modulename=Invoice&id=" + Quote_ID + '&PrintOption=1', '_blank')
                            jAlert(Quote_Msg, 'Alert Dialog: [SalesInvoice]', function (r) {
                                if (r == true) {
                                    var CashBank = (cddlCashBank.GetValue() != null) ? cddlCashBank.GetValue() : "";

                                    if (CashBank != "") {
                                        var URL = "CustomerReceiptPayment.aspx?key=ADD&IsTagged=Y&PageStatus=E&ComponentType=I&ComponentID=" + Quote_ID; // ComponentType => I : Invoice ; B : POS Bucket
                                        capcReciptPopup.SetContentUrl(URL);
                                        capcReciptPopup.Show();
                                    }
                                    else {
                                        window.location.assign("OldUnit_SalesInvoiceList.aspx");
                                    }
                                }
                            });
                        }
                        else {
                            window.location.assign("OldUnit_SalesInvoiceList.aspx");
                        }
                    }

                }
                else if (value == "N") {
                    if (grid.cpApproverStatus == "approve") {
                        window.parent.popup.Hide();
                        window.parent.cgridPendingApproval.PerformCallback();
                    }
                    else {
                        if (Quote_Number != "") {
                            //var strconfirm = confirm(Quote_Msg);
                            //if (strconfirm == true) {
                            //    window.location.assign("OldUnit_SalesInvoice.aspx?key=ADD");
                            //}
                            //else {
                            //    window.location.assign("OldUnit_SalesInvoice.aspx?key=ADD");
                            //}
                            //window.open("../../Reports/REPXReports/RepxReportViewer.aspx?Previewrpt=SalesInvoice~D&modulename=Invoice&id=" + Quote_ID + '&PrintOption=1', '_blank')
                            jAlert(Quote_Msg, 'Alert Dialog: [SalesInvoice]', function (r) {
                                if (r == true) {
                                    var CashBank = (cddlCashBank.GetValue() != null) ? cddlCashBank.GetValue() : "";

                                    if (CashBank != "") {
                                        var URL = "CustomerReceiptPayment.aspx?key=ADD&IsTagged=Y&PageStatus=E&ComponentType=I&ComponentID=" + Quote_ID; // ComponentType => I : Invoice ; B : POS Bucket
                                        capcReciptPopup.SetContentUrl(URL);
                                        capcReciptPopup.Show();
                                    }
                                    else {
                                        window.location.assign("OldUnit_SalesInvoice.aspx?key=ADD");
                                    }
                                }
                            });
                        }
                        else {
                            window.location.assign("OldUnit_SalesInvoice.aspx?key=ADD");
                        }
                    }
                }
                else {
                    var pageStatus = document.getElementById('hdnPageStatus').value;
                    if (pageStatus == "first") {
                        OnAddNewClick();
                        grid.batchEditApi.EndEdit();
                        // it has been commented by sam on 04032017 due to set focus from server side start
                        //document.getElementById("<%=ddl_numberingScheme.ClientID%>").focus();
                        // above part has been commented by sam on 04032017 due to set focus from server side start

                        $('#<%=hdnPageStatus.ClientID %>').val('');

                        var LocalCurrency = '<%=Session["LocalCurrency"]%>';
                        var basedCurrency = LocalCurrency.split("~");
                        if ($("#ddl_Currency").val() == basedCurrency[0]) {
                            ctxt_Rate.SetEnabled(false);
                        }
                    }
                    else if (pageStatus == "update") {
                        OnAddNewClick();
                        $('#<%=hdnPageStatus.ClientID %>').val('');
                        //document.getElementById("ddlInventory").disabled = true;

                        var LocalCurrency = '<%=Session["LocalCurrency"]%>';
                        var basedCurrency = LocalCurrency.split("~");
                        if ($("#ddl_Currency").val() == basedCurrency[0]) {
                            ctxt_Rate.SetEnabled(false);
                        }
                    }
                    else if (pageStatus == "Quoteupdate") {
                        grid.StartEditRow(0);
                        $('#<%=hdnPageStatus.ClientID %>').val('');
                    }
                    else if (pageStatus == "delete") {
                        grid.StartEditRow(0);
                        $('#<%=hdnPageStatus.ClientID %>').val('');
                    }
    }
}

    if (gridquotationLookup.GetValue() != null) {
        grid.GetEditor('ComponentNumber').SetEnabled(false);
        grid.GetEditor('ProductName').SetEnabled(false);
        grid.GetEditor('Description').SetEnabled(false);
    }
    cProductsPopup.Hide();
}

function Save_ButtonClick() {
    LoadingPanel.Show();

    flag = true;
    $('#<%=hfControlData.ClientID %>').val($('#hfControlSaveData').val());
    grid.batchEditApi.EndEdit();
    // Quote no validation Start
    var QuoteNo = ctxt_PLQuoteNo.GetText();
    if (QuoteNo == '' || QuoteNo == null) {
        LoadingPanel.Hide();
        $('#MandatorysQuoteno').attr('style', 'display:block');
        flag = false;
    }
    else {
        $('#MandatorysQuoteno').attr('style', 'display:none');
    }
    // Quote no validation End


    // Quote Date validation Start
    var sdate = tstartdate.GetValue();
    var edate = tenddate.GetValue();

    var startDate = new Date(sdate);
    var endDate = new Date(edate);
    if (sdate == null || sdate == "") {
        LoadingPanel.Hide();
        flag = false;
        $('#MandatorysDate').attr('style', 'display:block');
    }
    else { $('#MandatorysDate').attr('style', 'display:none'); }
    if (sdate == "") {
        LoadingPanel.Hide();
        flag = false;
        $('#MandatoryEDate').attr('style', 'display:block');
    }
    else {
        $('#MandatoryEDate').attr('style', 'display:none');
        //if (startDate > endDate) {

        //    flag = false;
        //    $('#MandatoryEgSDate').attr('style', 'display:block');
        //}
        //else { $('#MandatoryEgSDate').attr('style', 'display:none'); }
    }
    // Quote Date validation End

    // Quote Customer validation Start
    var customerId = GetObjectID('hdnCustomerId').value
    if (customerId == '' || customerId == null) {
        LoadingPanel.Hide();

        $('#MandatorysCustomer').attr('style', 'display:block');
        flag = false;
    }
    else {
        $('#MandatorysCustomer').attr('style', 'display:none');
    }

    if (ctxtCreditDays.GetValue() == 0) {
        LoadingPanel.Hide();
        jAlert("Credit Days must be greater than Zero(0)");
        flag = false;
    }

    // Quote Customer validation End
    var amtare = cddl_AmountAre.GetValue();
    if (amtare == '2') {
        var taxcodeid = cddlVatGstCst.GetValue();
        if (taxcodeid == '' || taxcodeid == null) {
            $('#Mandatorytaxcode').attr('style', 'display:block');
            flag = false;
        }
        else {
            $('#Mandatorytaxcode').attr('style', 'display:none');
        }
    }

    var frontRow = 0;
    var backRow = -1;
    var IsProduct = "";
    for (var i = 0; i <= grid.GetVisibleRowsOnPage() ; i++) {
        var frontProduct = (grid.batchEditApi.GetCellValue(backRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(backRow, 'ProductID')) : "";
        var backProduct = (grid.batchEditApi.GetCellValue(frontRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(frontRow, 'ProductID')) : "";

        if (frontProduct != "" || backProduct != "") {
            IsProduct = "Y";
            break;
        }

        backRow--;
        frontRow++;
    }

    if (flag != false) {
        if (IsProduct == "Y") {
            //divSubmitButton.style.display = "none";
            var customerval = (gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex()) != null) ? gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex()) : "";
            $('#<%=hdfLookupCustomer.ClientID %>').val(customerval);

            $('#<%=hdnRefreshType.ClientID %>').val('N');
            $('#<%=hdfIsDelete.ClientID %>').val('I');
            grid.batchEditApi.EndEdit();
            SelectAllData();
            grid.UpdateEdit();
        }
        else {
            LoadingPanel.Hide();
            jAlert('Cannot Save. You must enter atleast one Product to save this entry.');
        }
    }
}

function SaveExit_ButtonClick() {
    LoadingPanel.Show();

    flag = true;
    $('#<%=hfControlData.ClientID %>').val($('#hfControlSaveData').val());
    grid.batchEditApi.EndEdit();
    // Quote no validation Start
    var QuoteNo = ctxt_PLQuoteNo.GetText();
    if (QuoteNo == '' || QuoteNo == null) {
        LoadingPanel.Hide();
        $('#MandatorysQuoteno').attr('style', 'display:block');
        flag = false;
    }
    else {
        $('#MandatorysQuoteno').attr('style', 'display:none');
    }
    // Quote no validation End


    // Quote Date validation Start
    var sdate = tstartdate.GetValue();
    var edate = tenddate.GetValue();

    var startDate = new Date(sdate);
    var endDate = new Date(edate);
    if (sdate == null || sdate == "") {
        LoadingPanel.Hide();
        flag = false;
        $('#MandatorysDate').attr('style', 'display:block');
    }
    else { $('#MandatorysDate').attr('style', 'display:none'); }
    //if (edate == null || sdate == "") {
    if (sdate == "") {
        LoadingPanel.Hide();
        flag = false;
        $('#MandatoryEDate').attr('style', 'display:block');
    }
    else {
        //$('#MandatoryEDate').attr('style', 'display:none');
        //if (startDate > endDate) {

        //    flag = false;
        //    $('#MandatoryEgSDate').attr('style', 'display:block');
        //}
        //else { $('#MandatoryEgSDate').attr('style', 'display:none'); }
    }


    // Quote Date validation End

    // Quote Customer validation Start
    var customerId = GetObjectID('hdnCustomerId').value
    if (customerId == '' || customerId == null) {
        LoadingPanel.Hide();
        $('#MandatorysCustomer').attr('style', 'display:block');
        flag = false;
    }
    else {
        $('#MandatorysCustomer').attr('style', 'display:none');
    }
    // Quote Customer validation End

    var amtare = cddl_AmountAre.GetValue();
    if (amtare == '2') {
        var taxcodeid = cddlVatGstCst.GetValue();
        if (taxcodeid == '' || taxcodeid == null) {
            LoadingPanel.Hide();
            $('#Mandatorytaxcode').attr('style', 'display:block');
            flag = false;
        }
        else {
            $('#Mandatorytaxcode').attr('style', 'display:none');
        }
    }

    if (ctxtCreditDays.GetValue() == 0) {
        LoadingPanel.Hide();
        jAlert("Credit Days must be greater than Zero(0)");
        flag = false;
    }

    var frontRow = 0;
    var backRow = -1;
    var IsProduct = "";
    for (var i = 0; i <= grid.GetVisibleRowsOnPage() ; i++) {
        var frontProduct = (grid.batchEditApi.GetCellValue(backRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(backRow, 'ProductID')) : "";
        var backProduct = (grid.batchEditApi.GetCellValue(frontRow, 'ProductID') != null) ? (grid.batchEditApi.GetCellValue(frontRow, 'ProductID')) : "";

        if (frontProduct != "" || backProduct != "") {
            IsProduct = "Y";
            break;
        }

        backRow--;
        frontRow++;
    }

    if (flag != false) {
        if (IsProduct == "Y") {
            //divSubmitButton.style.display = "none";
            var customerval = (gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex()) != null) ? gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex()) : "";
            $('#<%=hdfLookupCustomer.ClientID %>').val(customerval);

            $('#<%=hdnRefreshType.ClientID %>').val('E');
            $('#<%=hdfIsDelete.ClientID %>').val('I');
            grid.batchEditApi.EndEdit();
            SelectAllData();
            grid.UpdateEdit();
        }
        else {
            LoadingPanel.Hide();
            jAlert('Cannot Save. You must enter atleast one Product to save this entry.');
        }
    }
}
        function QuantityTextChange(s, e) {

            if (globalQty == s.GetValue())
                return;


    pageheaderContent.style.display = "block";
    var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
    var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
    var key = gridquotationLookup.GetValue();// gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());

    if (ProductID != null) {
        var SpliteDetails = ProductID.split("||@||");
        var strMultiplier = SpliteDetails[7];
        var strFactor = SpliteDetails[8];
        var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";

        if (key != null && key != '') {
            var IsComponentProduct = SpliteDetails[15];
            var ComponentProduct = SpliteDetails[16];
            var TotalQty = (grid.GetEditor('TotalQty').GetText() != null) ? grid.GetEditor('TotalQty').GetText() : "0";
            var BalanceQty = (grid.GetEditor('BalanceQty').GetText() != null) ? grid.GetEditor('BalanceQty').GetText() : "0";
            var CurrQty = 0;

            BalanceQty = parseFloat(BalanceQty);
            TotalQty = parseFloat(TotalQty);
            QuantityValue = parseFloat(QuantityValue);

            if (TotalQty > QuantityValue) {
                CurrQty = BalanceQty + (TotalQty - QuantityValue);
            }
            else {
                CurrQty = BalanceQty - (QuantityValue - TotalQty);
            }

            if (CurrQty < 0) {
                grid.GetEditor("TotalQty").SetValue(TotalQty);
                grid.GetEditor("Quantity").SetValue(TotalQty);
                var OrdeMsg = 'Balance Quantity of selected Product from tagged document. <br/>Cannot enter quantity more than balance quantity.';
                grid.batchEditApi.EndEdit();
                jAlert(OrdeMsg, 'Alert Dialog: [Balace Quantity ]', function (r) {
                    grid.batchEditApi.StartEdit(globalRowIndex, 6);
                });
                return false;
            }
            else {
                grid.GetEditor("TotalQty").SetValue(QuantityValue);
                grid.GetEditor("BalanceQty").SetValue(CurrQty);
            }
        }
        else {
            grid.GetEditor("TotalQty").SetValue(QuantityValue);
            grid.GetEditor("BalanceQty").SetValue(QuantityValue);
        }

        var strProductID = SpliteDetails[0];
        var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
        var ddlbranch = $("[id*=ddl_Branch]");
        var strBranch = ddlbranch.find("option:selected").text();

        var strStkUOM = SpliteDetails[4];
        var strSalePrice = SpliteDetails[6];

        if (strRate == 0) {
            strRate = 1;
        }

        var StockQuantity = strMultiplier * QuantityValue;
        var Amount = QuantityValue * strFactor * (strSalePrice / strRate);

        $('#<%= lblStkQty.ClientID %>').text(StockQuantity);
        $('#<%= lblStkUOM.ClientID %>').text(strStkUOM);
        $('#<%= lblProduct.ClientID %>').text(strProductName);
        $('#<%= lblbranchName.ClientID %>').text(strBranch);

        var IsPackingActive = SpliteDetails[10];
        var Packing_Factor = SpliteDetails[11];
        var Packing_UOM = SpliteDetails[12];
        var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

        if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
            $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
            divPacking.style.display = "block";
        } else {
            divPacking.style.display = "none";
        }

        //var tbStockQuantity = grid.GetEditor("StockQuantity");
        //tbStockQuantity.SetValue(StockQuantity);

        var IsLinkedProduct = (grid.GetEditor('IsLinkedProduct').GetText() != null) ? grid.GetEditor('IsLinkedProduct').GetText() : "";
        if (IsLinkedProduct != "Y") {
            var tbAmount = grid.GetEditor("Amount");
            tbAmount.SetValue(Amount);

            var tbTotalAmount = grid.GetEditor("TotalAmount");
            tbTotalAmount.SetValue(Amount);

            DiscountTextChange(s, e);
        }

        //cacpAvailableStock.PerformCallback(strProductID);
    }
    else {
        jAlert('Select a product first.');
        grid.GetEditor('Quantity').SetValue('0');
        grid.GetEditor('ProductID').Focus();
    }
}

/// Code Added By Sam on 23022017 after make editable of sale price field Start

        function SalePriceTextChange(s, e) {
            if (globalSalePrice == s.GetValue())
                return;


    pageheaderContent.style.display = "block";
    var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
    var Saleprice = (grid.GetEditor('SalePrice').GetValue() != null) ? grid.GetEditor('SalePrice').GetValue() : "0";
    var ProductID = grid.GetEditor('ProductID').GetValue();
    if (ProductID != null) {
        var SpliteDetails = ProductID.split("||@||");

        console.log(SpliteDetails);
        if (parseFloat(s.GetValue()) < parseFloat(SpliteDetails[17])) {
            jAlert("Sale price cannot be lesser than Min Sale Price locked as: " + parseFloat(Math.round(Math.abs(parseFloat(SpliteDetails[17])) * 100) / 100).toFixed(2), "Alert", function () {
                grid.batchEditApi.StartEdit(globalRowIndex, 10);
                return;
            });
            s.SetValue(parseFloat(SpliteDetails[6]));
            return;
        }


        //if (parseFloat(SpliteDetails[18]) != 0 && parseFloat(s.GetValue()) > parseFloat(SpliteDetails[18])) {
        //    jAlert("Sale price cannot be greater than MRP locked as: " + parseFloat(Math.round(Math.abs(parseFloat(SpliteDetails[18])) * 100) / 100).toFixed(2), "Alert", function () {
        //        grid.batchEditApi.StartEdit(globalRowIndex, 10);
        //        return;
        //    });
        //    s.SetValue(parseFloat(SpliteDetails[6]));
        //    return;
        //}


        var strMultiplier = SpliteDetails[7];
        var strFactor = SpliteDetails[8];
        var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";
        //var strRate = "1";
        var strStkUOM = SpliteDetails[4];
        //var strSalePrice = SpliteDetails[6];

        var strProductID = SpliteDetails[0];
        var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
        var ddlbranch = $("[id*=ddl_Branch]");
        var strBranch = ddlbranch.find("option:selected").text();

        if (strRate == 0) {
            strRate = 1;
        }

        var StockQuantity = strMultiplier * QuantityValue;
        var Discount = (grid.GetEditor('Discount').GetValue() != null) ? grid.GetEditor('Discount').GetValue() : "0";

        var Amount = QuantityValue * strFactor * (Saleprice / strRate);
        var amountAfterDiscount = parseFloat(Amount) - ((parseFloat(Discount) * parseFloat(Amount)) / 100);

        var IsLinkedProduct = (grid.GetEditor('IsLinkedProduct').GetText() != null) ? grid.GetEditor('IsLinkedProduct').GetText() : "";
        if (IsLinkedProduct != "Y") {
            var tbAmount = grid.GetEditor("Amount");
            tbAmount.SetValue(amountAfterDiscount);

            var tbTotalAmount = grid.GetEditor("TotalAmount");
            tbTotalAmount.SetValue(amountAfterDiscount);

            $('#<%= lblProduct.ClientID %>').text(strProductName);
            $('#<%= lblbranchName.ClientID %>').text(strBranch);

            var IsPackingActive = SpliteDetails[10];
            var Packing_Factor = SpliteDetails[11];
            var Packing_UOM = SpliteDetails[12];
            var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

            if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
                $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
                divPacking.style.display = "block";
            } else {
                divPacking.style.display = "none";
            }
        }

        DiscountTextChange(s, e);
        //cacpAvailableStock.PerformCallback(strProductID);
    }
    else {
        jAlert('Select a product first.');
        grid.GetEditor('SalePrice').SetValue('0');
        grid.GetEditor('ProductID').Focus();
    }
}


/// Code Above Added By Sam on 23022017 after make editable of sale price field End


        function DiscountGotFocus(s, e) {
            globalDisc = s.GetValue();
        }

        function DiscountTextChange(s, e) {

            if (globalDisc == s.GetValue())
                return;

    //var Amount = (grid.GetEditor('Amount').GetValue() != null) ? grid.GetEditor('Amount').GetValue() : "0";
    var Discount = (grid.GetEditor('Discount').GetValue() != null) ? grid.GetEditor('Discount').GetValue() : "0";

    var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
    var IsLinkedProduct = (grid.GetEditor('IsLinkedProduct').GetText() != null) ? grid.GetEditor('IsLinkedProduct').GetText() : "";
    var ProductID = grid.GetEditor('ProductID').GetValue();
    if (ProductID != null) {
        var SpliteDetails = ProductID.split("||@||");
        var strFactor = SpliteDetails[8];
        var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";
        var strSalePrice = (grid.GetEditor('SalePrice').GetValue() != null) ? grid.GetEditor('SalePrice').GetValue() : "0";
        if (strSalePrice == '0') {
            strSalePrice = SpliteDetails[6];
        }
        if (strRate == 0) {
            strRate = 1;
        }
        var Amount = QuantityValue * strFactor * (strSalePrice / strRate);

        var amountAfterDiscount = parseFloat(Amount) - ((parseFloat(Discount) * parseFloat(Amount)) / 100);

        if (IsLinkedProduct != "Y") {
            var tbAmount = grid.GetEditor("Amount");
            tbAmount.SetValue(amountAfterDiscount);
        }

        var IsPackingActive = SpliteDetails[10];
        var Packing_Factor = SpliteDetails[11];
        var Packing_UOM = SpliteDetails[12];
        var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

        if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
            $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
            divPacking.style.display = "block";
        } else {
            divPacking.style.display = "none";
        }

        if (IsLinkedProduct != "Y") {
            var tbTotalAmount = grid.GetEditor("TotalAmount");
            tbTotalAmount.SetValue(amountAfterDiscount);
        }
        else {
            var tbDiscount = grid.GetEditor("Discount");
            tbDiscount.SetValue("0");

            var tbTotalAmount = grid.GetEditor("TotalAmount");
            tbTotalAmount.SetValue("0");

            var tbAmount = grid.GetEditor("Amount");
            tbAmount.SetValue("0");
        }

        var ShippingStateCode = cbsSCmbState.GetValue();
        var TaxType = "";
        if (cddl_AmountAre.GetValue() == "1") {
            TaxType = "E";
        }
        else if (cddl_AmountAre.GetValue() == "2") {
            TaxType = "I";
        }

        caluculateAndSetGSTforOldUnit(grid.GetEditor("Amount"), grid.GetEditor("TaxAmount"), grid.GetEditor("TotalAmount"), SpliteDetails[19], Amount, amountAfterDiscount, TaxType, ShippingStateCode, $('#ddl_Branch').val(), grid.GetEditor("ProdAvgvalue").GetValue() * grid.GetEditor('Quantity').GetValue());
    }
    else {
        jAlert('Select a product first.');
        grid.GetEditor('Discount').SetValue('0');
        grid.GetEditor('ProductID').Focus();
    }

    //Debjyoti 
    //grid.GetEditor('TaxAmount').SetValue(0);
    ctaxUpdatePanel.PerformCallback('DelQtybySl~' + grid.GetEditor("SrlNo").GetValue());
}
 
        function ProductAmountTextChange(s, e) {

            if (globalAmt == s.GetValue())
                return;

    var Amount = (grid.GetEditor('Amount').GetValue() != null) ? grid.GetEditor('Amount').GetValue() : "0";
    var TaxAmount = (grid.GetEditor('TaxAmount').GetValue() != null) ? grid.GetEditor('TaxAmount').GetValue() : "0";

    var tbTotalAmount = grid.GetEditor("TotalAmount");
    tbTotalAmount.SetValue(Amount + TaxAmount);

    ////////////////// For Tax

    var ProductID = grid.GetEditor('ProductID').GetValue();
    var SpliteDetails = ProductID.split("||@||");

    var ShippingStateCode = cbsSCmbState.GetValue();
    var TaxType = "";
    if (cddl_AmountAre.GetValue() == "1") {
        TaxType = "E";
    }
    else if (cddl_AmountAre.GetValue() == "2") {
        TaxType = "I";
    }

    caluculateAndSetGSTforOldUnit(grid.GetEditor("Amount"), grid.GetEditor("TaxAmount"), grid.GetEditor("TotalAmount"), SpliteDetails[19], Amount, Amount, TaxType, ShippingStateCode, $('#ddl_Branch').val(), grid.GetEditor("ProdAvgvalue").GetValue() * grid.GetEditor('Quantity').GetValue());
}

function AddBatchNew(s, e) {
    var ProductIDValue = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";

    var globalRow_Index = 0;
    if (globalRowIndex > 0) {
        globalRow_Index = globalRowIndex + 1;
    }
    else {
        globalRow_Index = globalRowIndex - 1;
    }


    var keyCode = ASPxClientUtils.GetKeyCode(e.htmlEvent);
    if (keyCode === 13) {
        if (ProductIDValue != "") {
            //var noofvisiblerows = grid.GetVisibleRowsOnPage();
            //var i;
            //var cnt = 2;

            grid.batchEditApi.EndEdit();

            grid.AddNewRow();
            grid.SetFocusedRowIndex();
            var noofvisiblerows = grid.GetVisibleRowsOnPage();

            var tbQuotation = grid.GetEditor("SrlNo");
            tbQuotation.SetValue(noofvisiblerows);

            grid.batchEditApi.StartEdit(globalRow_Index, 2);
            //grid.batchEditApi.StartEdit(-1, 1);
        }
    }
}
function OnAddNewClick() {
    if (gridquotationLookup.GetValue() == null) {
        grid.AddNewRow();

        var noofvisiblerows = grid.GetVisibleRowsOnPage(); // all newly created rows have -ve index -1 , -2 etc
        var tbQuotation = grid.GetEditor("SrlNo");
        tbQuotation.SetValue(noofvisiblerows);
    }
    else {
        QuotationNumberChanged();
        grid.StartEditRow(0);
    }
}
function OnAddNewClick_AtSaveTime() {
    if (gridquotationLookup.GetValue() == null) {
        grid.AddNewRow();

        var noofvisiblerows = grid.GetVisibleRowsOnPage(); // all newly created rows have -ve index -1 , -2 etc
        var tbQuotation = grid.GetEditor("SrlNo");
        tbQuotation.SetValue(noofvisiblerows);
    }
    else {
        grid.batchEditApi.StartEdit(0, 5);
    }
}




function Save_TaxClick() {

    if (gridTax.GetVisibleRowsOnPage() > 0) {
        gridTax.UpdateEdit();
    }
    else {
        gridTax.PerformCallback('SaveGst');
    }
    cPopup_Taxes.Hide();
}

var Warehouseindex;
function OnCustomButtonClick(s, e) {
    if (e.buttonID == 'CustomDelete') {
        var SrlNo = grid.batchEditApi.GetCellValue(e.visibleIndex, 'SrlNo');
        grid.batchEditApi.EndEdit();

        $('#<%=hdnRefreshType.ClientID %>').val('');
        $('#<%=hdnDeleteSrlNo.ClientID %>').val(SrlNo);
        var noofvisiblerows = grid.GetVisibleRowsOnPage();

        if (gridquotationLookup.GetValue() != null) {
            var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
            var messege = "";
            if (type == "QO") {
                messege = "Cannot Delete using this button as the Proforma is linked with this Sale Invoice.<br /> Click on Plus(+) sign to Add or Delete Product from last column !";
            }
            else if (type == "SO") {
                messege = "Cannot Delete using this button as the Sales Order is linked with this Sale Invoice.<br /> Click on Plus(+) sign to Add or Delete Product from last column !";
            }
            else if (type == "SC") {
                messege = "Cannot Delete using this button as the Sales Challan is linked with this Sale Invoice.<br /> Click on Plus(+) sign to Add or Delete Product from last column !";
            }

            jAlert(messege, 'Alert Dialog: [Delete Challan Products]', function (r) {
            });
        }
        else {
            if (noofvisiblerows != "1") {
                grid.DeleteRow(e.visibleIndex);

                $('#<%=hdfIsDelete.ClientID %>').val('D');
                grid.UpdateEdit();
                grid.PerformCallback('Display');

                $('#<%=hdnPageStatus.ClientID %>').val('delete');
                //grid.batchEditApi.StartEdit(-1, 2);
                //grid.batchEditApi.StartEdit(0, 2);
            }
        }
    }
    else if (e.buttonID == 'AddNew') {

        var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
        var SpliteDetails = ProductID.split("||@||");

        var IsComponentProduct = SpliteDetails[15];
        var ComponentProduct = SpliteDetails[16];

        if (IsComponentProduct == "Y") {
            var messege = "Selected product is defined with components.<br/> Would you like to proceed with components (" + ComponentProduct + ") ?";
            jConfirm(messege, 'Confirmation Dialog', function (r) {
                if (r == true) {
                    grid.GetEditor("IsComponentProduct").SetValue("Y");
                    $('#<%=hdfIsDelete.ClientID %>').val('C');

                    grid.UpdateEdit();
                    grid.PerformCallback('Display~fromComponent');
                    //grid.batchEditApi.StartEdit(globalRowIndex, 3);
                }
                else {
                    OnAddNewClick();

                    //setTimeout(function () {
                    //    grid.batchEditApi.StartEdit(globalRowIndex, 3);
                    //}, 500);
                    //return false;
                }
            });
            document.getElementById('popup_ok').focus();
        }
        else {
            if (ProductID != "") {
                OnAddNewClick();

                //setTimeout(function () {
                //    grid.batchEditApi.StartEdit(globalRowIndex, 3);
                //}, 500);
                //return false;
            }
            else {
                grid.batchEditApi.StartEdit(e.visibleIndex, 2);
            }
        }
    }
    else if (e.buttonID == 'CustomWarehouse') {
        var index = e.visibleIndex;
        grid.batchEditApi.StartEdit(index, 2)
        var inventoryType = (document.getElementById("ddlInventory").value != null) ? document.getElementById("ddlInventory").value : "";

        if (inventoryType == "C" || inventoryType == "Y" || inventoryType == "S") {
            Warehouseindex = index;

            var SrlNo = (grid.GetEditor('SrlNo').GetValue() != null) ? grid.GetEditor('SrlNo').GetValue() : "";
            var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
            //var StkQuantityValue = (grid.GetEditor('StockQuantity').GetValue() != null) ? grid.GetEditor('StockQuantity').GetValue() : "0";

            $("#spnCmbWarehouse").hide();
            $("#spnCmbBatch").hide();
            $("#spncheckComboBox").hide();
            $("#spntxtQuantity").hide();

            if (ProductID != "" && parseFloat(QuantityValue) != 0) {
                var SpliteDetails = ProductID.split("||@||");
                var strProductID = SpliteDetails[0];
                var strDescription = SpliteDetails[1];
                var strUOM = SpliteDetails[2];
                var strStkUOM = SpliteDetails[4];
                var strMultiplier = SpliteDetails[7];
                var strProductName = strDescription;
                //var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "";
                var StkQuantityValue = QuantityValue * strMultiplier;
                var Ptype = SpliteDetails[14];
                $('#<%=hdfProductType.ClientID %>').val(Ptype);

                document.getElementById('<%=lblProductName.ClientID %>').innerHTML = strProductName;
                document.getElementById('<%=txt_SalesAmount.ClientID %>').innerHTML = QuantityValue;
                document.getElementById('<%=txt_SalesUOM.ClientID %>').innerHTML = strUOM;
                document.getElementById('<%=txt_StockAmount.ClientID %>').innerHTML = StkQuantityValue;
                document.getElementById('<%=txt_StockUOM.ClientID %>').innerHTML = strStkUOM;

                $('#<%=hdfProductID.ClientID %>').val(strProductID);
                $('#<%=hdfProductSerialID.ClientID %>').val(SrlNo);
                $('#<%=hdfProductSerialID.ClientID %>').val(SrlNo);
                $('#<%=hdnProductQuantity.ClientID %>').val(QuantityValue);
                //cacpAvailableStock.PerformCallback(strProductID);

                if (Ptype == "W") {
                    div_Warehouse.style.display = 'block';
                    div_Batch.style.display = 'none';
                    div_Serial.style.display = 'none';
                    div_Quantity.style.display = 'block';
                    cCmbWarehouse.PerformCallback('BindWarehouse');
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "block");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "B") {
                    div_Warehouse.style.display = 'none';
                    div_Batch.style.display = 'block';
                    div_Serial.style.display = 'none';
                    div_Quantity.style.display = 'block';
                    cCmbBatch.PerformCallback('BindBatch~' + "0");
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "block");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "S") {
                    div_Warehouse.style.display = 'none';
                    div_Batch.style.display = 'none';
                    div_Serial.style.display = 'block';
                    div_Quantity.style.display = 'none';
                    checkListBox.PerformCallback('BindSerial~' + "0" + '~' + "0");
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "none");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "WB") {
                    div_Warehouse.style.display = 'block';
                    div_Batch.style.display = 'block';
                    div_Serial.style.display = 'none';
                    div_Quantity.style.display = 'block';
                    cCmbWarehouse.PerformCallback('BindWarehouse');
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "block");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "WS") {
                    div_Warehouse.style.display = 'block';
                    div_Batch.style.display = 'none';
                    div_Serial.style.display = 'block';
                    div_Quantity.style.display = 'none';
                    cCmbWarehouse.PerformCallback('BindWarehouse');
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "none");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "WBS") {
                    div_Warehouse.style.display = 'block';
                    div_Batch.style.display = 'block';
                    div_Serial.style.display = 'block';
                    div_Quantity.style.display = 'none';
                    cCmbWarehouse.PerformCallback('BindWarehouse');
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "none");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else if (Ptype == "BS") {
                    div_Warehouse.style.display = 'none';
                    div_Batch.style.display = 'block';
                    div_Serial.style.display = 'block';
                    div_Quantity.style.display = 'none';
                    cCmbBatch.PerformCallback('BindBatch~' + "0");
                    cGrdWarehouse.PerformCallback('Display~' + SrlNo);
                    $("#ADelete").css("display", "none");//Subhabrata
                    SelectedWarehouseID = "0";
                    cPopup_Warehouse.Show();
                }
                else {
                    //jAlert("No Warehouse or Batch or Serial is actived !", 'Alert Dialog: [SalesInvoice]', function (r) {
                    //    if (r == true) {
                    //        grid.batchEditApi.StartEdit(index, 8);
                    //    }
                    //});

                    jAlert("No Warehouse or Batch or Serial is actived !");
                }
            }
            else if (ProductID != "" && parseFloat(QuantityValue) == 0) {
                //jAlert("Please enter Quantity !", 'Alert Dialog: [SalesInvoice]', function (r) {
                //    if (r == true) {
                //        grid.batchEditApi.StartEdit(index, 8);
                //    }
                //});

                jAlert("Please enter Quantity !");
            }
        }
        else {
            //jAlert("You have selected Non-Inventory Item, so You cannot updated Stock.", 'Alert Dialog: [SalesInvoice]', function (r) {
            //    if (r == true) {
            //        grid.batchEditApi.StartEdit(index, 8);
            //    }
            //});

            jAlert("You have selected Non-Inventory Item, so You cannot updated Stock.");
        }
    }
}

function FinalWarehouse() {
    cGrdWarehouse.PerformCallback('WarehouseFinal');
}

function closeWarehouse(s, e) {
    e.cancel = false;
    cGrdWarehouse.PerformCallback('WarehouseDelete');
    $('#abpl').popover('hide');//Subhabrata
}

function OnWarehouseEndCallback(s, e) {
    var Ptype = document.getElementById('hdfProductType').value;

    if (cGrdWarehouse.cpIsSave == "Y") {
        cPopup_Warehouse.Hide();
        grid.batchEditApi.StartEdit(Warehouseindex, 5);
    }
    else if (cGrdWarehouse.cpIsSave == "N") {
        jAlert('Sales Quantity must be equal to Warehouse Quantity.');
    }
    else {
        if (document.getElementById("myCheck").checked == true) {
            if (IsPostBack == "N") {
                checkListBox.PerformCallback('BindSerial~' + PBWarehouseID + '~' + PBBatchID);

                IsPostBack = "";
                PBWarehouseID = "";
                PBBatchID = "";
            }

            if (Ptype == "W" || Ptype == "WB") {
                cCmbWarehouse.Focus();
            }
            else if (Ptype == "B") {
                cCmbBatch.Focus();
            }
            else {
                ctxtserial.Focus();
            }
        }
        else {
            if (Ptype == "W" || Ptype == "WB" || Ptype == "WS" || Ptype == "WBS") {
                cCmbWarehouse.Focus();
            }
            else if (Ptype == "B" || Ptype == "BS") {
                cCmbBatch.Focus();
            }
            else if (Ptype == "S") {
                checkComboBox.Focus();
            }
        }
    }
}

var SelectWarehouse = "0";
var SelectBatch = "0";
var SelectSerial = "0";
var SelectedWarehouseID = "0";

function CallbackPanelEndCall(s, e) {
    if (cCallbackPanel.cpEdit != null) {
        var strWarehouse = cCallbackPanel.cpEdit.split('~')[0];
        var strBatchID = cCallbackPanel.cpEdit.split('~')[1];
        var strSrlID = cCallbackPanel.cpEdit.split('~')[2];
        var strQuantity = cCallbackPanel.cpEdit.split('~')[3];

        SelectWarehouse = strWarehouse;
        SelectBatch = strBatchID;
        SelectSerial = strSrlID;

        cCmbWarehouse.PerformCallback('BindWarehouse');
        cCmbBatch.PerformCallback('BindBatch~' + strWarehouse);
        checkListBox.PerformCallback('EditSerial~' + strWarehouse + '~' + strBatchID + '~' + strSrlID);

        cCmbWarehouse.SetValue(strWarehouse);
        ctxtQuantity.SetValue(strQuantity);
    }
}

function acpAvailableStockEndCall(s, e) {
    if (cacpAvailableStock.cpstock != null) {
        divAvailableStk.style.display = "block";
        divpopupAvailableStock.style.display = "block";

        var AvlStk = cacpAvailableStock.cpstock + " " + document.getElementById('<%=lblStkUOM.ClientID %>').innerHTML;
        document.getElementById('<%=lblAvailableStk.ClientID %>').innerHTML = AvlStk;
        document.getElementById('<%=lblAvailableStock.ClientID %>').innerHTML = cacpAvailableStock.cpstock;
        document.getElementById('<%=lblAvailableStockUOM.ClientID %>').innerHTML = document.getElementById('<%=lblStkUOM.ClientID %>').innerHTML;

        cCmbWarehouse.cpstock = null;
    }
}
function ctaxUpdatePanelEndCall(s, e) {
    if (ctaxUpdatePanel.cpstock != null) {
        divAvailableStk.style.display = "block";
        divpopupAvailableStock.style.display = "block";

        var AvlStk = ctaxUpdatePanel.cpstock + " " + document.getElementById('<%=lblStkUOM.ClientID %>').innerHTML;
        document.getElementById('<%=lblAvailableStk.ClientID %>').innerHTML = AvlStk;
        document.getElementById('<%=lblAvailableStock.ClientID %>').innerHTML = ctaxUpdatePanel.cpstock;
        document.getElementById('<%=lblAvailableStockUOM.ClientID %>').innerHTML = document.getElementById('<%=lblStkUOM.ClientID %>').innerHTML;

        ctaxUpdatePanel.cpstock = null;
        grid.batchEditApi.StartEdit(globalRowIndex, 5);
        return false;
    }
}

function CmbWarehouseEndCallback(s, e) {
    if (SelectWarehouse != "0") {
        cCmbWarehouse.SetValue(SelectWarehouse);
        SelectWarehouse = "0";
    }
    else {
        cCmbWarehouse.SetEnabled(true);
    }
}

function CmbBatchEndCall(s, e) {
    if (SelectBatch != "0") {
        cCmbBatch.SetValue(SelectBatch);
        SelectBatch = "0";
    }
    else {
        cCmbBatch.SetEnabled(true);
    }
}

function listBoxEndCall(s, e) {
    if (SelectSerial != "0") {
        var values = [SelectSerial];
        checkListBox.SelectValues(values);
        UpdateSelectAllItemState();
        UpdateText();
        //checkListBox.SetValue(SelectWarehouse);
        SelectSerial = "0";
        cCmbBatch.SetEnabled(false);
        cCmbWarehouse.SetEnabled(false);
    }
}

function Save_TaxesClick() {
    grid.batchEditApi.EndEdit();
    var noofvisiblerows = grid.GetVisibleRowsOnPage(); // all newly created rows have -ve index -1 , -2 etc
    var i, cnt = 1;
    var sumAmount = 0, sumTaxAmount = 0, sumDiscount = 0, sumNetAmount = 0, sumDiscountAmt = 0;

    cnt = 1;
    for (i = -1 ; cnt <= noofvisiblerows ; i--) {
        var Amount = (grid.batchEditApi.GetCellValue(i, 'Amount') != null) ? (grid.batchEditApi.GetCellValue(i, 'Amount')) : "0";
        var TaxAmount = (grid.batchEditApi.GetCellValue(i, 'TaxAmount') != null) ? (grid.batchEditApi.GetCellValue(i, 'TaxAmount')) : "0";
        var Discount = (grid.batchEditApi.GetCellValue(i, 'Discount') != null) ? (grid.batchEditApi.GetCellValue(i, 'Discount')) : "0";
        var NetAmount = (grid.batchEditApi.GetCellValue(i, 'TotalAmount') != null) ? (grid.batchEditApi.GetCellValue(i, 'TotalAmount')) : "0";
        var sumDiscountAmt = ((parseFloat(Discount) * parseFloat(Amount)) / 100);

        sumAmount = sumAmount + parseFloat(Amount);
        sumTaxAmount = sumTaxAmount + parseFloat(TaxAmount);
        sumDiscount = sumDiscount + parseFloat(sumDiscountAmt);
        sumNetAmount = sumNetAmount + parseFloat(NetAmount);

        cnt++;
    }

    if (sumAmount == 0 && sumTaxAmount == 0 && Discount == 0) {
        cnt = 1;
        for (i = 0 ; cnt <= noofvisiblerows ; i++) {
            var Amount = (grid.batchEditApi.GetCellValue(i, 'Amount') != null) ? (grid.batchEditApi.GetCellValue(i, 'Amount')) : "0";
            var TaxAmount = (grid.batchEditApi.GetCellValue(i, 'TaxAmount') != null) ? (grid.batchEditApi.GetCellValue(i, 'TaxAmount')) : "0";
            var Discount = (grid.batchEditApi.GetCellValue(i, 'Discount') != null) ? (grid.batchEditApi.GetCellValue(i, 'Discount')) : "0";
            var NetAmount = (grid.batchEditApi.GetCellValue(i, 'TotalAmount') != null) ? (grid.batchEditApi.GetCellValue(i, 'TotalAmount')) : "0";
            var sumDiscountAmt = ((parseFloat(Discount) * parseFloat(Amount)) / 100);

            sumAmount = sumAmount + parseFloat(Amount);
            sumTaxAmount = sumTaxAmount + parseFloat(TaxAmount);
            sumDiscount = sumDiscount + parseFloat(sumDiscountAmt);
            sumNetAmount = sumNetAmount + parseFloat(NetAmount);

            cnt++;
        }
    }

    //Debjyoti 
    document.getElementById('HdChargeProdAmt').value = sumAmount;
    document.getElementById('HdChargeProdNetAmt').value = sumNetAmount;
    //End Here

    ctxtProductAmount.SetValue(Math.round(sumAmount).toFixed(2));
    ctxtProductTaxAmount.SetValue(Math.round(sumTaxAmount).toFixed(2));
    ctxtProductDiscount.SetValue(Math.round(sumDiscount).toFixed(2));
    ctxtProductNetAmount.SetValue(Math.round(sumNetAmount).toFixed(2));
    clblChargesTaxableGross.SetText("");
    clblChargesTaxableNet.SetText("");

    //Checking is gstcstvat will be hidden or not
    if (cddl_AmountAre.GetValue() == "2") {

        $('.lblChargesGSTforGross').show();
        $('.lblChargesGSTforNet').show();

        //Set Gross Amount with GstValue
        //Get The rate of Gst
        var gstRate = parseFloat(cddlVatGstCst.GetValue().split('~')[1]);
        if (gstRate) {
            if (gstRate != 0) {
                var gstDis = (gstRate / 100) + 1;
                if (cddlVatGstCst.GetValue().split('~')[2] == "G") {
                    $('.lblChargesGSTforNet').hide();
                    ctxtProductAmount.SetText(Math.round(sumAmount / gstDis).toFixed(2));
                    document.getElementById('HdChargeProdAmt').value = Math.round(sumAmount / gstDis).toFixed(2);
                    clblChargesGSTforGross.SetText(Math.round(sumAmount - parseFloat(document.getElementById('HdChargeProdAmt').value)).toFixed(2));
                    clblChargesTaxableGross.SetText("(Taxable)");

                }
                else {
                    $('.lblChargesGSTforGross').hide();
                    ctxtProductNetAmount.SetText(Math.round(sumNetAmount / gstDis).toFixed(2));
                    document.getElementById('HdChargeProdNetAmt').value = Math.round(sumNetAmount / gstDis).toFixed(2);
                    clblChargesGSTforNet.SetText(Math.round(sumNetAmount - parseFloat(document.getElementById('HdChargeProdNetAmt').value)).toFixed(2));
                    clblChargesTaxableNet.SetText("(Taxable)");
                }
            }

        } else {
            $('.lblChargesGSTforGross').hide();
            $('.lblChargesGSTforNet').hide();
        }
    }
    else if (cddl_AmountAre.GetValue() == "1") {
        $('.lblChargesGSTforGross').hide();
        $('.lblChargesGSTforNet').hide();

        //Debjyoti 09032017
        for (var cmbCount = 1; cmbCount < ccmbGstCstVatcharge.GetItemCount() ; cmbCount++) {
            if (ccmbGstCstVatcharge.GetItem(cmbCount).value.split('~')[5] == '19') {
                if (ccmbGstCstVatcharge.GetItem(cmbCount).value.split('~')[4] == 'I') {
                    ccmbGstCstVatcharge.RemoveItem(cmbCount);
                    cmbCount--;
                }
            } else {
                if (ccmbGstCstVatcharge.GetItem(cmbCount).value.split('~')[4] == 'S' || ccmbGstCstVatcharge.GetItem(cmbCount).value.split('~')[4] == 'C') {
                    ccmbGstCstVatcharge.RemoveItem(cmbCount);
                    cmbCount--;
                }
            }
        }






    }
    //End here





    //Set Total amount
    ctxtTotalAmount.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(ctxtProductNetAmount.GetValue()));

    gridTax.PerformCallback('Display');
    //Checking is gstcstvat will be hidden or not
    if (cddl_AmountAre.GetValue() == "2") {
        $('.chargeGstCstvatClass').hide();
    }
    else if (cddl_AmountAre.GetValue() == "1") {
        $('.chargeGstCstvatClass').show();
    }
    //End here
    $('.RecalculateCharge').hide();
    cPopup_Taxes.Show();
    gridTax.StartEditRow(0);
}

var chargejsonTax;
function OnTaxEndCallback(s, e) {
    GetPercentageData();
    $('.gridTaxClass').show();
    if (gridTax.GetVisibleRowsOnPage() == 0) {
        $('.gridTaxClass').hide();
        ccmbGstCstVatcharge.Focus();
    }
    else {
        gridTax.StartEditRow(0);
    }
    //check Json data
    if (gridTax.cpJsonChargeData) {
        if (gridTax.cpJsonChargeData != "") {
            chargejsonTax = JSON.parse(gridTax.cpJsonChargeData);
            gridTax.cpJsonChargeData = null;
        }
    }

    //Set Total Charges And total Amount
    if (gridTax.cpTotalCharges) {
        if (gridTax.cpTotalCharges != "") {
            ctxtQuoteTaxTotalAmt.SetValue(gridTax.cpTotalCharges);
            ctxtTotalAmount.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(ctxtProductNetAmount.GetValue()));
            gridTax.cpTotalCharges = null;
        }
    }

    SetChargesRunningTotal();
    ShowTaxPopUp("IN");
}

function GetPercentageData() {
    var Amount = ctxtProductAmount.GetValue();
    var GlobalTaxAmt = 0;
    var noofvisiblerows = gridTax.GetVisibleRowsOnPage(); // all newly created rows have -ve index -1 , -2 etc
    var i, cnt = 1;
    var sumAmount = 0, totalAmount = 0;
    for (i = 0 ; cnt <= noofvisiblerows ; i++) {
        var totLength = gridTax.batchEditApi.GetCellValue(i, 'TaxName').length;
        var sign = gridTax.batchEditApi.GetCellValue(i, 'TaxName').substring(totLength - 3);
        var DisAmount = (gridTax.batchEditApi.GetCellValue(i, 'Amount') != null) ? (gridTax.batchEditApi.GetCellValue(i, 'Amount')) : "0";

        if (sign == '(+)') {
            sumAmount = sumAmount + parseFloat(DisAmount);
        }
        else {
            sumAmount = sumAmount - parseFloat(DisAmount);
        }

        cnt++;
    }

    totalAmount = (parseFloat(Amount)) + (parseFloat(sumAmount));
    // ctxtTotalAmount.SetValue(totalAmount);
}



function PercentageTextChange(s, e) {
    //var Amount = ctxtProductAmount.GetValue();
    var Amount = gridTax.GetEditor("calCulatedOn").GetValue();
    var GlobalTaxAmt = 0;
    //var Percentage = (gridTax.GetEditor('Percentage').GetValue() != null) ? gridTax.GetEditor('Percentage').GetValue() : "0";
    var Percentage = s.GetText();
    var totLength = gridTax.GetEditor("TaxName").GetText().length;
    var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
    Sum = ((parseFloat(Amount) * parseFloat(Percentage)) / 100);

    if (sign == '(+)') {
        GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
        gridTax.GetEditor("Amount").SetValue(Sum);
        ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(Sum) - GlobalTaxAmt);
        //ctxtTotalAmount.SetValue(parseFloat(Amount) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue())); 
        ctxtTotalAmount.SetValue(parseFloat(ctxtProductNetAmount.GetValue()) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
        GlobalTaxAmt = 0;
    }
    else {
        GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
        gridTax.GetEditor("Amount").SetValue(Sum);
        ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) - parseFloat(Sum) + GlobalTaxAmt);
        //ctxtTotalAmount.SetValue(parseFloat(Amount) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue())); 
        ctxtTotalAmount.SetValue(parseFloat(ctxtProductNetAmount.GetValue()) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
        GlobalTaxAmt = 0;
    }

    SetOtherChargeTaxValueOnRespectiveRow(0, Sum, gridTax.GetEditor("TaxName").GetText());
    SetChargesRunningTotal();

    RecalCulateTaxTotalAmountCharges();
}

function RecalCulateTaxTotalAmountCharges() {
    var totalTaxAmount = 0;
    for (var i = 0; i < chargejsonTax.length; i++) {
        gridTax.batchEditApi.StartEdit(i, 3);
        var totLength = gridTax.GetEditor("TaxName").GetText().length;
        var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
        if (sign == '(+)') {
            totalTaxAmount = totalTaxAmount + parseFloat(gridTax.GetEditor("Amount").GetValue());
        } else {
            totalTaxAmount = totalTaxAmount - parseFloat(gridTax.GetEditor("Amount").GetValue());
        }

        gridTax.batchEditApi.EndEdit();
    }

    totalTaxAmount = totalTaxAmount + parseFloat(ctxtGstCstVatCharge.GetValue());

    ctxtQuoteTaxTotalAmt.SetValue(Math.round(totalTaxAmount));
    ctxtTotalAmount.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(ctxtProductNetAmount.GetValue()));
}

//Set Running Total for Charges And Tax 
function SetChargesRunningTotal() {
    var runningTot = parseFloat(ctxtProductNetAmount.GetValue());
    for (var i = 0; i < chargejsonTax.length; i++) {
        gridTax.batchEditApi.StartEdit(i, 3);
        if (chargejsonTax[i].applicableOn == "R") {
            gridTax.GetEditor("calCulatedOn").SetValue(runningTot);
            var totLength = gridTax.GetEditor("TaxName").GetText().length;
            var taxNameWithSign = gridTax.GetEditor("Percentage").GetText();
            var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
            var ProdAmt = parseFloat(gridTax.GetEditor("calCulatedOn").GetValue());
            var Amount = gridTax.GetEditor("calCulatedOn").GetValue();
            var GlobalTaxAmt = 0;

            var Percentage = gridTax.GetEditor("Percentage").GetText();
            var totLength = gridTax.GetEditor("TaxName").GetText().length;
            var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
            Sum = ((parseFloat(Amount) * parseFloat(Percentage)) / 100);

            if (sign == '(+)') {
                GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
                gridTax.GetEditor("Amount").SetValue(Sum);
                ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + parseFloat(Sum) - GlobalTaxAmt);
                ctxtTotalAmount.SetValue(parseFloat(Amount) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
                //ctxtTotalAmount.SetText(parseFloat(ctxtTotalAmount.GetValue()) + parseFloat(Sum) - GlobalTaxAmt);
                GlobalTaxAmt = 0;
            }
            else {
                GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
                gridTax.GetEditor("Amount").SetValue(Sum);
                ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) - parseFloat(Sum) + GlobalTaxAmt);
                ctxtTotalAmount.SetValue(parseFloat(Amount) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
                //ctxtTotalAmount.SetText(parseFloat(ctxtTotalAmount.GetValue()) - parseFloat(Sum) + GlobalTaxAmt);
                GlobalTaxAmt = 0;
            }

            SetOtherChargeTaxValueOnRespectiveRow(0, Sum, gridTax.GetEditor("TaxName").GetText());


        }
        runningTot = runningTot + parseFloat(gridTax.GetEditor("Amount").GetValue());
        gridTax.batchEditApi.EndEdit();
    }
}

/////////////////// QuotationTaxAmountTextChange By Sam on 23022017
var taxAmountGlobalCharges;
function QuotationTaxAmountGotFocus(s, e) {
    taxAmountGlobalCharges = parseFloat(s.GetValue());
}


function QuotationTaxAmountTextChange(s, e) {
    //var Amount = ctxtProductAmount.GetValue();
    var Amount = gridTax.GetEditor("calCulatedOn").GetValue();
    var GlobalTaxAmt = 0;
    //var Percentage = (gridTax.GetEditor('Percentage').GetValue() != null) ? gridTax.GetEditor('Percentage').GetValue() : "0";
    //var Percentage = s.GetText();
    var totLength = gridTax.GetEditor("TaxName").GetText().length;
    var sign = gridTax.GetEditor("TaxName").GetText().substring(totLength - 3);
    //Sum = ((parseFloat(Amount) * parseFloat(Percentage)) / 100);

    if (sign == '(+)') {
        GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
        //gridTax.GetEditor("Amount").SetValue(Sum);
        ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) + GlobalTaxAmt - taxAmountGlobalCharges);
        ctxtTotalAmount.SetValue(parseFloat(ctxtProductNetAmount.GetValue()) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
        GlobalTaxAmt = 0;
        SetOtherChargeTaxValueOnRespectiveRow(0, s.GetValue(), gridTax.GetEditor("TaxName").GetText());
    }
    else {
        GlobalTaxAmt = parseFloat(gridTax.GetEditor("Amount").GetValue());
        //gridTax.GetEditor("Amount").SetValue(Sum);
        ctxtQuoteTaxTotalAmt.SetValue(parseFloat(ctxtQuoteTaxTotalAmt.GetValue()) - GlobalTaxAmt + taxAmountGlobalCharges);
        ctxtTotalAmount.SetValue(parseFloat(ctxtProductNetAmount.GetValue()) + parseFloat(ctxtQuoteTaxTotalAmt.GetValue()));
        GlobalTaxAmt = 0;
        SetOtherChargeTaxValueOnRespectiveRow(0, s.GetValue(), gridTax.GetEditor("TaxName").GetText());
    }

    RecalCulateTaxTotalAmountCharges();

}



////////////

var AmountOldValue;
var AmountNewValue;

function AmountTextChange(s, e) {
    AmountLostFocus(s, e);
    var RecieveValue = (grid.GetEditor('Amount').GetValue() != null) ? parseFloat(grid.GetEditor('Amount').GetValue()) : "0";
}

function AmountLostFocus(s, e) {
    AmountNewValue = s.GetText();
    var indx = AmountNewValue.indexOf(',');

    if (indx != -1) {
        AmountNewValue = AmountNewValue.replace(/,/g, '');
    }
    if (AmountOldValue != AmountNewValue) {
        changeReciptTotalSummary();
    }
}

function AmountGotFocus(s, e) {
    AmountOldValue = s.GetText();
    globalAmt = s.GetValue();
    var indx = AmountOldValue.indexOf(',');
    if (indx != -1) {
        AmountOldValue = AmountOldValue.replace(/,/g, '');
    }
}

function changeReciptTotalSummary() {
    var newDif = AmountOldValue - AmountNewValue;
    var CurrentSum = ctxtSumTotal.GetText();
    var indx = CurrentSum.indexOf(',');
    if (indx != -1) {
        CurrentSum = CurrentSum.replace(/,/g, '');
    }

    ctxtSumTotal.SetValue(parseFloat(CurrentSum - newDif));
}

function CmbWarehouse_ValueChange() {
    var WarehouseID = cCmbWarehouse.GetValue();
    var type = document.getElementById('hdfProductType').value;

    if (type == "WBS" || type == "WB") {
        cCmbBatch.PerformCallback('BindBatch~' + WarehouseID);
    }
    else if (type == "WS") {
        checkListBox.PerformCallback('BindSerial~' + WarehouseID + '~' + "0");
    }
}
function CmbBatch_ValueChange() {
    var WarehouseID = cCmbWarehouse.GetValue();
    var BatchID = cCmbBatch.GetValue();
    var type = document.getElementById('hdfProductType').value;

    if (type == "WBS") {
        checkListBox.PerformCallback('BindSerial~' + WarehouseID + '~' + BatchID);
    }
    else if (type == "BS") {
        checkListBox.PerformCallback('BindSerial~' + "0" + '~' + BatchID);
    }
}
function SaveWarehouse() {
    var WarehouseID = (cCmbWarehouse.GetValue() != null) ? cCmbWarehouse.GetValue() : "0";
    var WarehouseName = cCmbWarehouse.GetText();
    var BatchID = (cCmbBatch.GetValue() != null) ? cCmbBatch.GetValue() : "0";
    var BatchName = cCmbBatch.GetText();
    var SerialID = "";
    var SerialName = "";
    var Qty = ctxtQuantity.GetValue();

    var items = checkListBox.GetSelectedItems();
    var vals = [];
    var texts = [];

    for (var i = 0; i < items.length; i++) {
        if (items[i].index != 0) {
            if (i == 0) {
                SerialID = items[i].value;
                SerialName = items[i].text;
            }
            else {
                if (SerialID == "" && SerialID == "") {
                    SerialID = items[i].value;
                    SerialName = items[i].text;
                }
                else {
                    SerialID = SerialID + '||@||' + items[i].value;
                    SerialName = SerialName + '||@||' + items[i].text;
                }
            }
            //texts.push(items[i].text);
            //vals.push(items[i].value);
        }
    }

    //WarehouseID, BatchID, SerialID, Qty=0.0
    $("#spnCmbWarehouse").hide();
    $("#spnCmbBatch").hide();
    $("#spncheckComboBox").hide();
    $("#spntxtQuantity").hide();

    var Ptype = document.getElementById('hdfProductType').value;
    if ((Ptype == "W" && WarehouseID == "0") || (Ptype == "WB" && WarehouseID == "0") || (Ptype == "WS" && WarehouseID == "0") || (Ptype == "WBS" && WarehouseID == "0")) {
        $("#spnCmbWarehouse").show();
    }
    else if ((Ptype == "B" && BatchID == "0") || (Ptype == "WB" && BatchID == "0") || (Ptype == "WBS" && BatchID == "0") || (Ptype == "BS" && BatchID == "0")) {
        $("#spnCmbBatch").show();
    }
    else if ((Ptype == "W" && Qty == "0.0") || (Ptype == "B" && Qty == "0.0") || (Ptype == "WB" && Qty == "0.0")) {
        $("#spntxtQuantity").show();
    }
    else if ((Ptype == "S" && SerialID == "") || (Ptype == "WS" && SerialID == "") || (Ptype == "WBS" && SerialID == "") || (Ptype == "BS" && SerialID == "")) {
        $("#spncheckComboBox").show();
    }
    else {
        if (document.getElementById("myCheck").checked == true && SelectedWarehouseID == "0") {
            if (Ptype == "W" || Ptype == "WB" || Ptype == "B") {
                cCmbWarehouse.PerformCallback('BindWarehouse');
                cCmbBatch.PerformCallback('BindBatch~' + "");
                checkListBox.PerformCallback('BindSerial~' + "" + '~' + "");
                ctxtQuantity.SetValue("0");
            }
            else {
                IsPostBack = "N";
                PBWarehouseID = WarehouseID;
                PBBatchID = BatchID;
            }
        }
        else {
            cCmbWarehouse.PerformCallback('BindWarehouse');
            cCmbBatch.PerformCallback('BindBatch~' + "");
            checkListBox.PerformCallback('BindSerial~' + "" + '~' + "");
            ctxtQuantity.SetValue("0");
        }
        UpdateText();
        cGrdWarehouse.PerformCallback('SaveDisplay~' + WarehouseID + '~' + WarehouseName + '~' + BatchID + '~' + BatchName + '~' + SerialID + '~' + SerialName + '~' + Qty + '~' + SelectedWarehouseID);
        SelectedWarehouseID = "0";
    }
}

var IsPostBack = "";
var PBWarehouseID = "";
var PBBatchID = "";


$(document).ready(function () {
    $('#ddl_VatGstCst_I').blur(function () {
        if (grid.GetVisibleRowsOnPage() == 1) {
            grid.batchEditApi.StartEdit(-1, 2);
        }
    })
    $('#ddl_AmountAre').blur(function () {
        var id = cddl_AmountAre.GetValue();
        if (id == '1' || id == '3') {
            if (grid.GetVisibleRowsOnPage() == 1) {
                grid.batchEditApi.StartEdit(-1, 2);
            }
        }
    })


});

function deleteAllRows() {
    var frontRow = 0;
    var backRow = -1;
    for (var i = 0; i <= grid.GetVisibleRowsOnPage() + 100 ; i++) {
        grid.DeleteRow(frontRow);
        grid.DeleteRow(backRow);
        backRow--;
        frontRow++;
    }
    OnAddNewClick();
}
function txtserialTextChanged() {
    checkListBox.UnselectAll();
    var SerialNo = (ctxtserial.GetValue() != null) ? (ctxtserial.GetValue()) : "0";

    if (SerialNo != "0") {
        ctxtserial.SetValue("");
        var texts = [SerialNo];
        var values = GetValuesByTexts(texts);

        if (values.length > 0) {
            checkListBox.SelectValues(values);
            UpdateSelectAllItemState();
            UpdateText(); // for remove non-existing texts
            SaveWarehouse();
        }
        else {
            jAlert("This Serial Number does not exists.");
        }
    }
}

function AutoCalculateMandateOnChange(element) {
    $("#spnCmbWarehouse").hide();
    $("#spnCmbBatch").hide();
    $("#spncheckComboBox").hide();
    $("#spntxtQuantity").hide();

    if (document.getElementById("myCheck").checked == true) {
        divSingleCombo.style.display = "block";
        divMultipleCombo.style.display = "none";

        checkComboBox.Focus();
    }
    else {
        divSingleCombo.style.display = "none";
        divMultipleCombo.style.display = "block";

        ctxtserial.Focus();
    }
}

function fn_Deletecity(keyValue) {
    var WarehouseID = (cCmbWarehouse.GetValue() != null) ? cCmbWarehouse.GetValue() : "0";
    var BatchID = (cCmbBatch.GetValue() != null) ? cCmbBatch.GetValue() : "0";

    cGrdWarehouse.PerformCallback('Delete~' + keyValue);
    checkListBox.PerformCallback('BindSerial~' + WarehouseID + '~' + BatchID);
}
function fn_Edit(keyValue) {
    //cGrdWarehouse.PerformCallback('EditWarehouse~' + keyValue);
    SelectedWarehouseID = keyValue;
    cCallbackPanel.PerformCallback('EditWarehouse~' + keyValue);
}
    </script>
    <script type="text/javascript">
        // <![CDATA[
        var textSeparator = ";";
        var selectedChkValue = "";

        function OnListBoxSelectionChanged(listBox, args) {
            if (args.index == 0)
                args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
            UpdateSelectAllItemState();
            UpdateText();

            //Added Subhabrata
            var selectedItems = checkListBox.GetSelectedItems();
            var val = GetSelectedItemsText(selectedItems);
            var strWarehouse = cCmbWarehouse.GetValue();
            var strBatchID = cCmbBatch.GetValue();
            var ProducttId = $("#hdfProductID").val();

            //$.ajax({
            //    type: "POST",
            //    url: "OldUnit_SalesInvoice.aspx/GetSerialId",
            //    data: JSON.stringify({
            //        "id": val,
            //        "wareHouseStr": strWarehouse,
            //        "BatchID": strBatchID,
            //        "ProducttId": ProducttId
            //    }),
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    async: false,//Added By:Subhabrata
            //    success: function (msg) {

            //        var type = msg.d;
            //        if (type == "1") {

            //            return true;
            //        }
            //        else if (type == "0") {
            //            alert("Serial No can be Stock out based on FIFO process.Select the Serial No. shown from Oldest to Newest sequence to proceed");
            //            //listBox.UnselectAll();

            //            var indices = [];
            //            //Added By:Subhabrata
            //            if ((selectedItems.length * 1) == 1) {
            //                indices.push(listBox.GetItem(args.index));
            //                listBox.UnselectIndices(indices[0].text);
            //                UpdateSelectAllItemState();
            //                UpdateText();
            //            }
            //            if (((args.index) * 1) <= (selectedItems.length * 1)) {
            //                for (var i = ((args.index) * 1) ; i <= ((selectedItems.length * 1) + 1) ; i++) {
            //                    indices.push(listBox.GetItem(i));

            //                }
            //            }
            //            else {
            //                indices.push(listBox.GetItem(args.index));
            //                listBox.UnselectIndices(indices[0].text);
            //                UpdateSelectAllItemState();
            //                UpdateText();
            //            }

            //            for (var j = 0; j < indices.length   ; j++) {
            //                listBox.UnselectIndices(indices[j].text);
            //                UpdateSelectAllItemState();
            //                UpdateText();
            //            }
            //        }
            //    }
            //});

            //End
        }
        function UpdateSelectAllItemState() {
            IsAllSelected() ? checkListBox.SelectIndices([0]) : checkListBox.UnselectIndices([0]);
        }
        function IsAllSelected() {
            var selectedDataItemCount = checkListBox.GetItemCount() - (checkListBox.GetItem(0).selected ? 0 : 1);
            return checkListBox.GetSelectedItems().length == selectedDataItemCount;
        }
        function UpdateText() {
            var selectedItems = checkListBox.GetSelectedItems();
            selectedChkValue = GetSelectedItemsText(selectedItems);
            //checkComboBox.SetText(GetSelectedItemsText(selectedItems));
            //checkComboBox.SetText(selectedItems.length + " Items");

            var itemsCount = GetSelectedItemsCount(selectedItems);
            checkComboBox.SetText(itemsCount + " Items");

            var val = GetSelectedItemsText(selectedItems);
            $("#abpl").attr('data-content', val);
        }
        function SynchronizeListBoxValues(dropDown, args) {
            checkListBox.UnselectAll();
            // var texts = dropDown.GetText().split(textSeparator);
            var texts = selectedChkValue.split(textSeparator);

            var values = GetValuesByTexts(texts);
            checkListBox.SelectValues(values);
            UpdateSelectAllItemState();
            UpdateText(); // for remove non-existing texts
        }
        function GetSelectedItemsText(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.join(textSeparator);
        }
        function GetSelectedItemsCount(items) {
            var texts = [];
            for (var i = 0; i < items.length; i++)
                if (items[i].index != 0)
                    texts.push(items[i].text);
            return texts.length;
        }
        function GetValuesByTexts(texts) {
            var actualValues = [];
            var item;
            for (var i = 0; i < texts.length; i++) {
                item = checkListBox.FindItemByText(texts[i]);
                if (item != null)
                    actualValues.push(item.value);
            }
            return actualValues;
        }
        $(function () {
            $('[data-toggle="popover"]').popover();
        })
        // ]]>
    </script>
    <script>
        function ProductsGotFocus(s, e) {
            pageheaderContent.style.display = "block";
            var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            //var ProductID = (cCmbProduct.GetValue() != null) ? cCmbProduct.GetValue() : "0";
            //var strProductName = (cCmbProduct.GetText() != null) ? cCmbProduct.GetText() : "0";

            var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";

            globalQty = QuantityValue;
            globalSalePrice = grid.GetEditor('SalePrice').GetValue();

            var ddlbranch = $("[id*=ddl_Branch]");
            var strBranch = ddlbranch.find("option:selected").text();

            var SpliteDetails = ProductID.split("||@||");
            var strProductID = SpliteDetails[0];
            var strDescription = SpliteDetails[1];
            var strUOM = SpliteDetails[2];
            var strStkUOM = SpliteDetails[4];
            var strSalePrice = SpliteDetails[6];
            var IsPackingActive = SpliteDetails[10];
            var Packing_Factor = SpliteDetails[11];
            var Packing_UOM = SpliteDetails[12];
            var strProductShortCode = SpliteDetails[14];
            var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

            strProductName = strDescription;

            if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
                $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
                divPacking.style.display = "block";
            } else {
                divPacking.style.display = "none";
            }

            $('#<%= lblStkQty.ClientID %>').text(QuantityValue);
            $('#<%= lblStkUOM.ClientID %>').text(strStkUOM);
            $('#<%= lblProduct.ClientID %>').text(strProductName);
            $('#<%= lblbranchName.ClientID %>').text(strBranch);

            //if (ProductID != "0") {
            //   cacpAvailableStock.PerformCallback(strProductID);
            //}
        }
        function ProductsGotFocusFromID(s, e) {
            pageheaderContent.style.display = "block";
            var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            //var ProductID = (cCmbProduct.GetValue() != null) ? cCmbProduct.GetValue() : "0";
            //var strProductName = (cCmbProduct.GetText() != null) ? cCmbProduct.GetText() : "0";

            var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";

            var ddlbranch = $("[id*=ddl_Branch]");
            var strBranch = ddlbranch.find("option:selected").text();

            var SpliteDetails = ProductID.split("||@||");
            var strProductID = SpliteDetails[0];
            var strDescription = SpliteDetails[1];
            var strUOM = SpliteDetails[2];
            var strStkUOM = SpliteDetails[4];
            var strSalePrice = SpliteDetails[6];
            var IsPackingActive = SpliteDetails[10];
            var Packing_Factor = SpliteDetails[11];
            var Packing_UOM = SpliteDetails[12];
            var strProductShortCode = SpliteDetails[14];
            var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

            strProductName = strDescription;

            if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
                $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
                divPacking.style.display = "block";
            } else {
                divPacking.style.display = "none";
            }

            $('#<%= lblStkQty.ClientID %>').text(QuantityValue);
            $('#<%= lblStkUOM.ClientID %>').text(strStkUOM);
            $('#<%= lblProduct.ClientID %>').text(strProductName);
            $('#<%= lblbranchName.ClientID %>').text(strBranch);

            if (ProductID != "0") {
                cacpAvailableStock.PerformCallback(strProductID);
            }
        }

        function ddlInventory_OnChange() {
            cproductLookUp.GetGridView().Refresh();
        }
    </script>

    
    <style>
        .dxgvControl_PlasticBlue td.dxgvBatchEditModifiedCell_PlasticBlue {
            background: #fff !important;
        }

        .popover {
            z-index: 999999;
            max-width: 350px;
        }

            .popover .popover-title {
                margin-top: 0 !important;
                background: #465b9d;
                color: #fff;
            }

        .pdLeft15 {
            padding-left: 15px;
        }

        .mTop {
            margin-top: 10px;
        }

        .mLeft {
            margin-left: 15px;
        }

        .popover .popover-content {
            min-height: 60px;
        }
        /*#grid_DXEditingErrorRow-1 {
            display: none;
        }*/

        /*#grid_DXStatus span > a {
            display: none;
        }

        #gridTax_DXStatus span > a {
            display: none;
        }*/

        #grid_DXStatus {
            display: none;
        }

        #aspxGridTax_DXStatus {
            display: none;
        }

        #gridTax_DXStatus {
            display: none;
        }

        .hideCell {
            display: none;
        }

        .pullleftClass {
            position: absolute;
            right: -3px;
            top: 24px;
        }

        #myCheck {
            transform: translateY(2px);
            -webkit-transform: translateY(2px);
            -moz-transform: translateY(2px);
            margin-right: 5px;
        }
    </style>
    <%--End Sudip--%>

    <style>
        .dxeErrorFrameSys.dxeErrorCellSys {
            position: absolute;
        }

        .dxeButtonEditClearButton_PlasticBlue {
            display: none;
        }

        .mbot5 .col-md-8 {
            margin-bottom: 5px;
        }

        .validclass {
            position: absolute;
            right: -4px;
            top: 20px;
        }

        .mandt {
            position: absolute;
            right: -18px;
            top: 4px;
        }

        #txtProductAmount, #txtProductTaxAmount, #txtProductDiscount {
            font-weight: bold;
        }

        /*#grid, #grid div {
            width: 100% !important;
        }*/
        .crossBtn {
            cursor: pointer;
        }

        #txtTaxTotAmt input, #txtprodBasicAmt input, #txtGstCstVat input {
            text-align: right;
        }

        #grid .dxgvHSDC > div, #grid .dxgvCSD {
            width: 100% !important;
        }
    </style>


    <%--Batch Product Popup Start--%>

    <script>
        function ProductKeyDown(s, e) {
            console.log(e.htmlEvent.key);
            if (e.htmlEvent.key == "Enter") {

                s.OnButtonClick(0);
            }
            if (e.htmlEvent.key == "Tab") {

                s.OnButtonClick(0);
            }
        }

        function ProductButnClick(s, e) {
            if (e.buttonIndex == 0) {

                if (cproductLookUp.Clear()) {
                    cProductpopUp.Show();
                    cproductLookUp.Focus();
                    cproductLookUp.ShowDropDown();
                }
            }
        }

        function ProductSelected(s, e) {
            if (cproductLookUp.GetGridView().GetFocusedRowIndex() == -1) {
                cProductpopUp.Hide();
                grid.batchEditApi.StartEdit(globalRowIndex, 5);
                return;
            }

            var LookUpData = cproductLookUp.GetGridView().GetRowKey(cproductLookUp.GetGridView().GetFocusedRowIndex());
            //var ProductCode = cproductLookUp.GetValue();

            var focusedRow = cproductLookUp.gridView.GetFocusedRowIndex();
            var ProductCode = cproductLookUp.gridView.GetRow(focusedRow).children[1].innerText;

            if (!ProductCode) {
                LookUpData = null;
            }
            cProductpopUp.Hide();

            grid.GetEditor("ProdAvgvalue").SetText("");
            grid.batchEditApi.StartEdit(globalRowIndex);
            grid.GetEditor("ProductID").SetText(LookUpData);
            grid.GetEditor("ProductName").SetText(ProductCode);
            console.log(LookUpData);
            pageheaderContent.style.display = "block";
            cddl_AmountAre.SetEnabled(false);

            var tbDescription = grid.GetEditor("Description");
            var tbUOM = grid.GetEditor("UOM");
            var tbSalePrice = grid.GetEditor("SalePrice");

            //var strProductName = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var ProductID = (grid.GetEditor('ProductID').GetText() != null) ? grid.GetEditor('ProductID').GetText() : "0";
            var SpliteDetails = ProductID.split("||@||");
            var strProductID = SpliteDetails[0];
            var strDescription = SpliteDetails[1];
            var strUOM = SpliteDetails[2];
            var strStkUOM = SpliteDetails[4];
            var strSalePrice = SpliteDetails[6];
            var oldUnitAvgPrice = SpliteDetails[20];

            var QuantityValue = (grid.GetEditor('Quantity').GetValue() != null) ? grid.GetEditor('Quantity').GetValue() : "0";
            var IsPackingActive = SpliteDetails[10];
            var Packing_Factor = SpliteDetails[11];
            var Packing_UOM = SpliteDetails[12];

            var strRate = (ctxt_Rate.GetValue() != null && ctxt_Rate.GetValue() != "0") ? ctxt_Rate.GetValue() : "1";
            if (strRate == 0) {
                strSalePrice = strSalePrice;
            }
            else {
                strSalePrice = strSalePrice / strRate;
            }

            tbDescription.SetValue(strDescription);
            tbUOM.SetValue(strUOM);
            tbSalePrice.SetValue(strSalePrice);

            grid.GetEditor("Quantity").SetValue("0.00");
            grid.GetEditor("Discount").SetValue("0.00");
            grid.GetEditor("Amount").SetValue("0.00");
            grid.GetEditor("TaxAmount").SetValue("0.00");
            grid.GetEditor("TotalAmount").SetValue("0.00");
            grid.GetEditor("ProdAvgvalue").SetValue(oldUnitAvgPrice);


            var ddlbranch = $("[id*=ddl_Branch]");
            var strBranch = ddlbranch.find("option:selected").text();

            $('#<%= lblStkQty.ClientID %>').text("0.00");
            $('#<%= lblStkUOM.ClientID %>').text(strStkUOM);
            $('#<%= lblProduct.ClientID %>').text(strDescription);
            $('#<%= lblbranchName.ClientID %>').text(strBranch);

            var PackingValue = (parseFloat((Packing_Factor * QuantityValue).toString())).toFixed(4) + " " + Packing_UOM;

            if (IsPackingActive == "Y" && (parseFloat(Packing_Factor * QuantityValue) > 0)) {
                $('#<%= lblPackingStk.ClientID %>').text(PackingValue);
                divPacking.style.display = "block";
            } else {
                divPacking.style.display = "none";
            }

            
            
            var IsComponentProduct = SpliteDetails[15];
            var ComponentProduct = SpliteDetails[16];

            document.getElementById("ddlInventory").disabled = true;

            cacpAvailableStock.PerformCallback(strProductID);
            //Debjyoti
            ctaxUpdatePanel.PerformCallback('DelProdbySl~' + grid.GetEditor("SrlNo").GetValue() + '~' + strProductID);
            grid.batchEditApi.StartEdit(globalRowIndex, 5);
        }
        function ProductlookUpKeyDown(s, e) {
            if (e.htmlEvent.key == "Escape") {
                cProductpopUp.Hide();
                grid.batchEditApi.StartEdit(globalRowIndex, 5);
            }
        }
    </script>
    <style>
        #grid_DXMainTable > tbody > tr > td:last-child {
            display: none !important;
        }

        #openlink {
            font-size: 18px;
        }
    </style>
    <%--Batch Product Popup End--%>

    <%--Compnent Tag Start--%>

    <script>
        function ddlBranch_ChangeIndex() {
            if (gridquotationLookup.GetValue() != null) {
                jConfirm('Documents tagged are to be automatically De-selected. Confirm ?', 'Confirmation Dialog', function (r) {
                    if (r == true) {
                        var startDate = new Date();
                        startDate = tstartdate.GetValueString();
                        var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                        var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                        var componentType = gridquotationLookup.GetValue();// gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());

                        cQuotationComponentPanel.PerformCallback('DateCheckOnChanged' + '~' + key + '~' + startDate + '~' + '@');

                        if (key != null && key != '' && type != "") {
                            cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + 'DateCheck' + '~' + type);
                        }

                        if (componentType != null && componentType != '') {
                            grid.PerformCallback('GridBlank');
                            ctaxUpdatePanel.PerformCallback('DeleteAllTax');
                        }
                    }
                });
            }
            else {
                var startDate = new Date();
                startDate = tstartdate.GetValueString();
                var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                var componentType = gridquotationLookup.GetValue();// gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());

                cQuotationComponentPanel.PerformCallback('DateCheckOnChanged' + '~' + key + '~' + startDate + '~' + '@');

                if (key != null && key != '' && type != "") {
                    cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + 'DateCheck' + '~' + type);
                }

                if (componentType != null && componentType != '') {
                    grid.PerformCallback('GridBlank');
                    ctaxUpdatePanel.PerformCallback('DeleteAllTax');
                }
            }
        }
        function DateCheck() {

            if (gridquotationLookup.GetValue() != null) {
                jConfirm('Documents tagged are to be automatically De-selected. Confirm ?', 'Confirmation Dialog', function (r) {
                    if (r == true) {
                        var startDate = new Date();
                        startDate = tstartdate.GetValueString();
                        var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                        var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                        var componentType = gridquotationLookup.GetValue();// gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());

                        cQuotationComponentPanel.PerformCallback('DateCheckOnChanged' + '~' + key + '~' + startDate + '~' + '@');

                        if (key != null && key != '' && type != "") {
                            cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + 'DateCheck' + '~' + type);
                        }

                        if (componentType != null && componentType != '') {
                            grid.PerformCallback('GridBlank');
                            ctaxUpdatePanel.PerformCallback('DeleteAllTax');
                        }
                    }
                });
            }
            else {
                var startDate = new Date();
                startDate = tstartdate.GetValueString();
                var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
                var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";
                var componentType = gridquotationLookup.GetValue();// gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());

                cQuotationComponentPanel.PerformCallback('DateCheckOnChanged' + '~' + key + '~' + startDate + '~' + '@');

                if (key != null && key != '' && type != "") {
                    cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + 'DateCheck' + '~' + type);
                }

                if (componentType != null && componentType != '') {
                    grid.PerformCallback('GridBlank');
                    ctaxUpdatePanel.PerformCallback('DeleteAllTax');
                }
            }
        }
        function componentEndCallBack(s, e) {
            gridquotationLookup.gridView.Refresh();
            if (grid.GetVisibleRowsOnPage() == 0) {
                OnAddNewClick();
            }

            if (cQuotationComponentPanel.cpDetails != null) {
                var details = cQuotationComponentPanel.cpDetails;
                cQuotationComponentPanel.cpDetails = null;

                var SpliteDetails = details.split("~");
                var Reference = SpliteDetails[0];
                var Currency_Id = SpliteDetails[1];
                var SalesmanId = SpliteDetails[2];
                var ExpiryDate = SpliteDetails[3];
                var CurrencyRate = SpliteDetails[4];

                var Type = SpliteDetails[5];
                var CreditDays = SpliteDetails[6];
                var DueDate = SpliteDetails[7];

                if (Type == "SO") {
                    if (DueDate != "" && CreditDays != "") {
                        ctxtCreditDays.SetValue(CreditDays);

                        var Due_Date = new Date(DueDate);
                        cdt_SaleInvoiceDue.SetDate(Due_Date);
                    }
                }

                ctxt_Refference.SetValue(Reference);
                ctxt_Rate.SetValue(CurrencyRate);
                document.getElementById('ddl_Currency').value = Currency_Id;
                document.getElementById('ddl_SalesAgent').value = SalesmanId;

                if (ExpiryDate != "") {
                    var myDate = new Date(ExpiryDate);
                    var invoiceDate = new Date();
                    var datediff = Math.round((myDate - invoiceDate) / (1000 * 60 * 60 * 24));

                    ctxtCreditDays.SetValue(datediff);
                    cdt_SaleInvoiceDue.SetDate(myDate);
                }
            }
        }
        function selectValue() {
            var startDate = new Date();
            startDate = tstartdate.GetValueString();
            var key = gridLookup.GetGridView().GetRowKey(gridLookup.GetGridView().GetFocusedRowIndex());
            var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";

            if (type == "QO") {
                clbl_InvoiceNO.SetText('PI/Quotation Date');
            }
            else if (type == "SO") {
                clbl_InvoiceNO.SetText('Sales Order Date');
            }
            else if (type == "SC") {
                clbl_InvoiceNO.SetText('Sales Challan Date');
            }

            cQuotationComponentPanel.PerformCallback('DateCheckOnChanged' + '~' + key + '~' + startDate + '~' + '@');

            if (key != null && key != '' && type != "") {
                cQuotationComponentPanel.PerformCallback('BindComponentGrid' + '~' + key + '~' + startDate + '~' + 'DateCheck' + '~' + type);
            }

            var componentType = gridquotationLookup.GetValue();//gridquotationLookup.GetGridView().GetRowKey(gridquotationLookup.GetGridView().GetFocusedRowIndex());
            if (componentType != null && componentType != '') {
                grid.PerformCallback('GridBlank');
            }
        }
        function CloseGridQuotationLookup() {
            gridquotationLookup.ConfirmCurrentSelection();
            gridquotationLookup.HideDropDown();
            gridquotationLookup.Focus();
        }
        function QuotationNumberChanged() {
            var quote_Id = gridquotationLookup.gridView.GetSelectedKeysOnPage();//gridquotationLookup.GetValue();
            quote_Id = quote_Id.join();

            var type = ($("[id$='rdl_SaleInvoice']").find(":checked").val() != null) ? $("[id$='rdl_SaleInvoice']").find(":checked").val() : "";

            if (quote_Id != null) {
                var arr = quote_Id.split(',');

                if (arr.length > 1) {
                    if (type == "QO") {
                        ctxt_InvoiceDate.SetText('Multiple Select Quotation Dates');
                    }
                    else if (type == "SO") {
                        ctxt_InvoiceDate.SetText('Multiple Select Order Dates');
                    }
                    else if (type == "SC") {
                        ctxt_InvoiceDate.SetText('Multiple Select Challan Dates');
                    }
                }
                else {
                    if (arr.length == 1) {
                        cComponentDatePanel.PerformCallback('BindComponentDate' + '~' + quote_Id + '~' + type);
                    }
                    else {
                        ctxt_InvoiceDate.SetText('');
                    }
                }
            }
            else { ctxt_InvoiceDate.SetText(''); }

            if (quote_Id != null) {
                cgridproducts.PerformCallback('BindProductsDetails' + '~' + '@');
                cProductsPopup.Show();
            }
        }
        function ChangeState(value) {

            cgridproducts.PerformCallback('SelectAndDeSelectProducts' + '~' + value);
        }
        function PerformCallToGridBind() {

            grid.PerformCallback('BindGridOnQuotation' + '~' + '@');
            cQuotationComponentPanel.PerformCallback('BindComponentGridOnSelection');
            $('#hdnPageStatus').val('Quoteupdate');
            cProductsPopup.Hide();

            //#### added by Samrat Roy for Transporter Control #############

            var quote_Id = gridquotationLookup.gridView.GetSelectedKeysOnPage();
            if ("<%=Convert.ToString(Session["TransporterVisibilty"])%>" == "Yes") {
                callTransporterControl(quote_Id[0], $("#rdl_SaleInvoice").find(":checked").val());
            }
            if (quote_Id.length > 0) {
                BSDocTagging(quote_Id[0], $("#rdl_SaleInvoice").find(":checked").val());
            }
            //#### end : Samrat Roy for Transporter Control :end #############

            //#### added by Sayan Dutta for TC Control #############
            if ($("#btn_TermsCondition").is(":visible")) {
                callTCControl(quote_Id[0], $("#rdl_SaleInvoice").find(":checked").val());
            }
            //#### End : added by Sayan Dutta for TC Control : End #############

            return false;
        }
    </script>
    <%--Compnent Tag End--%>

    <%--Receipt/Payment Popup Start--%>
    <script>
        function ShowReceiptPayment() {
            //uri = "CustomerReceiptPayment.aspx?key=ADD&IsTagged=Y";
            //capcReciptPopup.SetContentUrl(uri);
            //capcReciptPopup.Show();
        }
        $(document).ready(function () {
            $("#openlink").on("click", function () {
                //window.open('../master/Contact_general.aspx?id=ADD', '_blank');
                AddcustomerClick();
            });
        });

        function AddcustomerClick() {
            var url = '/OMS/management/Master/Customer_general.aspx';
            AspxDirectAddCustPopup.SetContentUrl(url);
            AspxDirectAddCustPopup.Show();
        }

        function ParentCustomerOnClose(newCustId) {
            AspxDirectAddCustPopup.Hide();

            cCustomerCallBackPanel.PerformCallback('SetCustomer~' + newCustId);
        }
        function CustomerCallBackPanelEndCallBack() {
            GetContactPerson();
        }

    </script>
    <%--Receipt/Payment Popup End--%>

    <script>
        //Code for UDF Control 
        function OpenUdf() {
            if (document.getElementById('IsUdfpresent').value == '0') {
                jAlert("UDF not define.");
            }
            else {
                var keyVal = document.getElementById('Keyval_internalId').value;
                var url = '/OMS/management/Master/frm_BranchUdfPopUp.aspx?Type=SI&&KeyVal_InternalID=' + keyVal;
                popup.SetContentUrl(url);
                popup.Show();
            }
            return true;
        }
        // End Udf Code
    </script>

    <%--Added By : Samrat Roy -- New Billing/Shipping Section--%>
    <script>
        function disp_prompt(name) {

            if (name == "tab0") {
                gridLookup.Focus();
                //alert(name);
                //document.location.href = "SalesQuotation.aspx?";
            }
            if (name == "tab1") {
                var custID = GetObjectID('hdnCustomerId').value;
                if (custID == null && custID == '') {
                    jAlert('Please select a customer');
                    page.SetActiveTabIndex(0);
                    return;
                }
                else {
                    page.SetActiveTabIndex(1);
                    //fn_PopOpen();
                }
            }
        }

    </script>
    <%--Added By : Samrat Roy -- New Billing/Shipping Section End--%>

    <script>
        function CRP_SaveANDExit_Press() {
            window.location.assign("OldUnit_SalesInvoiceList.aspx");
        }

        function CRP_SaveANDNew_Press() {
            window.location.assign("OldUnit_SalesInvoice.aspx?key=ADD");
        }
    </script>

    <%--Rev 1.0--%>
    <link href="/assests/css/custom/newcustomstyle.css" rel="stylesheet" />
    
    <style>
        select
        {
            z-index: 1;
        }

        /*#grid {
            max-width: 98% !important;
        }*/
        #FormDate , #toDate , #dtTDate , #dt_PLQuote , #dt_PLSales , #dt_SaleInvoiceDue , #dt_BTOut , #dt_refCreditNoteDt
        {
            position: relative;
            z-index: 1;
            background: transparent;
        }

        #FormDate_B-1 , #toDate_B-1 , #dtTDate_B-1 , #dt_PLQuote_B-1 , #dt_PLSales_B-1 , #dt_SaleInvoiceDue_B-1 , #dt_BTOut_B-1 ,
        #dt_refCreditNoteDt_B-1
        {
            background: transparent !important;
            border: none;
            width: 30px;
            padding: 10px !important;
        }

        #FormDate_B-1 #FormDate_B-1Img , #toDate_B-1 #toDate_B-1Img , #dtTDate_B-1 #dtTDate_B-1Img , #dt_PLQuote_B-1 #dt_PLQuote_B-1Img ,
        #dt_PLSales_B-1 #dt_PLSales_B-1Img , #dt_SaleInvoiceDue_B-1 #dt_SaleInvoiceDue_B-1Img , #dt_BTOut_B-1 #dt_BTOut_B-1Img ,
        #dt_refCreditNoteDt_B-1 #dt_refCreditNoteDt_B-1Img
        {
            display: none;
        }

        /*select
        {
            -webkit-appearance: auto;
        }*/

        .calendar-icon
        {
                right: 18px;
                bottom: 6px;
        }
        .padTabtype2 > tbody > tr > td
        {
            vertical-align: bottom;
        }
        #rdl_Salesquotation
        {
            margin-top: 0px;
        }

        .lblmTop8>span, .lblmTop8>label
        {
            margin-top: 0 !important;
        }

        .col-md-2, .col-md-4 {
    margin-bottom: 5px;
}

        .simple-select::after
        {
                top: 34px;
            right: 13px;
        }

        .dxeErrorFrameWithoutError_PlasticBlue.dxeControlsCell_PlasticBlue
        {
            padding: 0;
        }

        .aspNetDisabled
        {
            background: #f3f3f3 !important;
        }

        .backSelect {
    background: #42b39e !important;
}

        #ddlInventory
        {
                -webkit-appearance: auto;
        }

        /*.wid-90
        {
            width: 100%;
        }
        .dxtcLite_PlasticBlue.dxtc-top > .dxtc-content
        {
            width: 97%;
        }*/
        .newLbl
        {
                margin: 3px 0 !important;
        }

        .lblBot > span, .lblBot > label
        {
                margin-bottom: 3px !important;
        }

        .col-md-2 > label, .col-md-2 > span, .col-md-1 > label, .col-md-1 > span
        {
            margin-top: 0px;
            font-size: 14px;
        }

        .col-md-6 span
        {
            font-size: 14px;
        }

        #gridDEstination
        {
            width:99% !important;
        }

        #txtEntity , #txtCustName
        {
            width: 100%;
        }
        .col-md-6 span
        {
            margin-top: 8px !important;
        }

        .rds
        {
            margin-top: 10px !important;
        }

        @media only screen and (max-width: 1380px) and (min-width: 1300px)
        {
            .col-xs-1, .col-xs-2, .col-xs-3, .col-xs-4, .col-xs-5, .col-xs-6, .col-xs-7, .col-xs-8, .col-xs-9, .col-xs-10, .col-xs-11, .col-xs-12, .col-sm-1, .col-sm-2, .col-sm-3, .col-sm-4, .col-sm-5, .col-sm-6, .col-sm-7, .col-sm-8, .col-sm-9, .col-sm-10, .col-sm-11, .col-sm-12, .col-md-1, .col-md-2, .col-md-3, .col-md-4, .col-md-5, .col-md-6, .col-md-7, .col-md-8, .col-md-9, .col-md-10, .col-md-11, .col-md-12, .col-lg-1, .col-lg-2, .col-lg-3, .col-lg-4, .col-lg-5, .col-lg-6, .col-lg-7, .col-lg-8, .col-lg-9, .col-lg-10, .col-lg-11, .col-lg-12
            {
                 padding-right: 10px;
                 padding-left: 10px;
            }
            .simple-select::after
        {
                top: 34px;
            right: 8px;
        }
            .calendar-icon
        {
                right: 14px;
                bottom: 6px;
        }
        }

    </style>
    <%--Rev end 1.0--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <script src="UserControls/Js/ucPaymentDetails.js?var=1.0"></script>
    <%--Rev 1.0: "outer-div-main" class add --%>
    <div class="outer-div-main clearfix">
        <div class="panel-title clearfix">
        <h3 class="pull-left">
            <asp:Label ID="lblHeadTitle" Text="" runat="server"></asp:Label>
            <%--<label>Add Proforma Invoice/ Quotation</label>--%>
        </h3>
        <div id="pageheaderContent" class="pull-right wrapHolder content horizontal-images" style="display: none;">
            <div class="Top clearfix">
                <ul>
                    <li>
                        <div class="lblHolder" id="divDues" style="display: none;">
                            <table>
                                <tr>
                                    <td>Receivable(Dues)</td>
                                </tr>
                                <tr>
                                    <td class="lower">
                                        <asp:Label ID="lblTotalDues" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                    <li>
                        <div class="lblHolder" id="divAvailableStk" style="display: none;">
                            <table>
                                <tr>
                                    <td>Available Stock</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblAvailableStk" runat="server" Text="0.0"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                    <li>
                        <div class="lblHolder" id="divPacking" style="display: none;">
                            <table>
                                <tr>
                                    <td>Packing Quantity</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblPackingStk" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                </ul>
                <ul style="display: none;">
                    <li>
                        <div class="lblHolder">
                            <table>
                                <tr>
                                    <td>Selected Unit</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblbranchName" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                    <li>
                        <div class="lblHolder">
                            <table>
                                <tr>
                                    <td>Selected Product</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblProduct" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                    <li>
                        <div class="lblHolder">
                            <table>
                                <tr>
                                    <td>Stock Quantity</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="lblStkQty" runat="server" Text="0.00"></asp:Label>
                                        <asp:Label ID="lblStkUOM" runat="server" Text=" "></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </li>
                </ul>
            </div>
        </div>
        <div id="ApprovalCross" runat="server" class="crossBtn"><a href=""><i class="fa fa-times"></i></a></div>
        <div id="divcross" runat="server" class="crossBtn"><a href="OldUnit_SalesInvoiceList.aspx"><i class="fa fa-times"></i></a></div>

    </div>
        <div class="form_main">
        <asp:Panel ID="pnl_quotation" runat="server">
            <div class="">
                <dxe:ASPxPageControl ID="ASPxPageControl1" runat="server" ClientInstanceName="page" Width="100%">
                    <TabPages>
                        <dxe:TabPage Name="General" Text="General">
                            <ContentCollection>
                                <dxe:ContentControl runat="server">
                                    <div class="">
                                        <div style=" padding: 4px 0; margin-bottom: 0px; border-radius: 4px; " class="clearfix col-md-12">
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_Inventory" runat="server" Text="Inventory Item?">
                                                </dxe:ASPxLabel>
                                                <asp:DropDownList ID="ddlInventory" CssClass="backSelect" runat="server" Width="100%" TabIndex="1" onchange="ddlInventory_OnChange()">
                                                    <asp:ListItem Text="Inventory Item" Value="Y" />
                                                    <asp:ListItem Text="Non-Inventory Item" Value="N" />
                                                    <asp:ListItem Text="Capital Goods" Value="C" />
                                                    <asp:ListItem Text="Second Hand" Value="S" />
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-md-9">
                                                <div class="row">
                                                    <%--Rev 1.0: "simple-select" class add --%>
                                                    <div class="col-md-3 simple-select" id="divScheme" runat="server">
                                                        <dxe:ASPxLabel ID="lbl_NumberingScheme" runat="server" Text="Numbering Scheme">
                                                        </dxe:ASPxLabel>
                                                        <asp:DropDownList ID="ddl_numberingScheme" runat="server" Width="100%" TabIndex="2">
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-3">
                                                        <dxe:ASPxLabel ID="lbl_SaleInvoiceNo" runat="server" Text="Document Number">
                                                        </dxe:ASPxLabel>
                                                        <dxe:ASPxTextBox ID="txt_PLQuoteNo" runat="server" ClientInstanceName="ctxt_PLQuoteNo" TabIndex="2" Width="100%">
                                                            <ClientSideEvents TextChanged="function(s, e) {UniqueCodeCheck();}" />
                                                        </dxe:ASPxTextBox>

                                                        <span id="MandatorysQuoteno" style="display: none" class="validclass">
                                                            <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory">
                                                        </span>
                                                        <span id="duplicateQuoteno" style="display: none" class="validclass">
                                                            <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Duplicate number">
                                                        </span>

                                                    </div>
                                                    <div class="col-md-3">
                                                        <dxe:ASPxLabel ID="lbl_SaleInvoiceDt" runat="server" Text="Posting Date">
                                                        </dxe:ASPxLabel>
                                                        <dxe:ASPxDateEdit ID="dt_PLQuote" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="tstartdate" TabIndex="3" Width="100%">
                                                            <ButtonStyle Width="13px">
                                                            </ButtonStyle>
                                                            <ClientSideEvents DateChanged="function(s, e) {DateCheck();}" GotFocus="function(s,e){tstartdate.ShowDropDown();}" />
                                                        </dxe:ASPxDateEdit>
                                                        <%--Rev 1.0--%>
                                                        <img src="/assests/images/calendar-icon.png" class="calendar-icon"/>
                                                        <%--Rev end 1.0--%>
                                                    </div>
                                                    <%--Rev 1.0: "simple-select" class add --%>
                                                    <div class="col-md-3 simple-select">
                                                        <dxe:ASPxLabel ID="lbl_Branch" runat="server" Text="Unit">
                                                        </dxe:ASPxLabel>
                                                        <asp:DropDownList ID="ddl_Branch" runat="server" Width="100%" TabIndex="4" onchange="ddlBranch_ChangeIndex()" Enabled="true">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div style="clear: both">
                                            </div>
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_Customer" runat="server" Text="Customer">
                                                </dxe:ASPxLabel>
                                                <i id="openlink" class="fa fa-plus-circle" aria-hidden="true"></i>
                                                <dxe:ASPxCallbackPanel runat="server" ID="CustomerCallBackPanel" ClientInstanceName="cCustomerCallBackPanel" OnCallback="CustomerCallBackPanel_Callback">
                                                    <PanelCollection>
                                                        <dxe:PanelContent runat="server">

                                                            <dxe:ASPxGridLookup ID="lookup_Customer" runat="server" TabIndex="5" ClientInstanceName="gridLookup"
                                                                KeyFieldName="cnt_internalid" Width="100%" TextFormatString="{1}" AutoGenerateColumns="False" DataSourceID="dsCustomer">
                                                                <Columns>
                                                                    <dxe:GridViewDataColumn FieldName="shortname" Visible="true" VisibleIndex="0" Caption="Short Name" Width="200px" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="Name" Visible="true" VisibleIndex="1" Caption="Name" Settings-AutoFilterCondition="Contains" Width="200px">
                                                                        <%-- <Settings AllowAutoFilter="False"></Settings>--%>
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="Type" Visible="true" VisibleIndex="2" Caption="Type" Settings-AutoFilterCondition="Contains" Width="200px">
                                                                        <%-- <Settings AllowAutoFilter="False"></Settings>--%>
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="cnt_internalid" Visible="false" VisibleIndex="3" Settings-AllowAutoFilter="False" Width="200px">
                                                                        <Settings AllowAutoFilter="False"></Settings>
                                                                    </dxe:GridViewDataColumn>
                                                                </Columns>
                                                                <GridViewProperties Settings-VerticalScrollBarMode="Auto">
                                                                    <Templates>
                                                                        <StatusBar>
                                                                            <table class="OptionsTable" style="float: right">
                                                                                <tr>
                                                                                    <td>
                                                                                        <dxe:ASPxButton ID="Close" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridLookup" UseSubmitBehavior="False" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </StatusBar>
                                                                    </Templates>
                                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectSingleRowOnly="True"></SettingsBehavior>
                                                                    <SettingsLoadingPanel Text="Please Wait..." />
                                                                    <Settings ShowFilterRow="True" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                                                                </GridViewProperties>
                                                                <ClientSideEvents TextChanged="function(s, e) { GetContactPerson(e)}" GotFocus="function(s,e){gridLookup.ShowDropDown();}" />
                                                                <ClearButton DisplayMode="Auto">
                                                                </ClearButton>
                                                            </dxe:ASPxGridLookup>

                                                        </dxe:PanelContent>
                                                    </PanelCollection>
                                                    <ClientSideEvents EndCallback="CustomerCallBackPanelEndCallBack" />
                                                </dxe:ASPxCallbackPanel>


                                                <span id="MandatorysCustomer" style="display: none" class="validclass">
                                                    <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                                            </div>
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_ContactPerson" runat="server" Text="Contact Person">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxComboBox ID="cmbContactPerson" runat="server" OnCallback="cmbContactPerson_Callback" ClientSideEvents-EndCallback="cmbContactPersonEndCall" TabIndex="6" Width="100%" ClientInstanceName="cContactPerson" Font-Size="12px">
                                                    <ClientSideEvents EndCallback="cmbContactPersonEndCall" GotFocus="function(s,e){cContactPerson.ShowDropDown();}" />
                                                </dxe:ASPxComboBox>
                                            </div>
                                            <div class="col-md-3 hide">
                                                <asp:RadioButtonList ID="rdl_SaleInvoice" runat="server" RepeatDirection="Horizontal" onchange="return selectValue();">
                                                    <asp:ListItem Text="PI/Quotation" Value="QO"></asp:ListItem>
                                                    <asp:ListItem Text="Order" Value="SO"></asp:ListItem>
                                                    <asp:ListItem Text="Challan" Value="SC"></asp:ListItem>
                                                </asp:RadioButtonList>
                                                <dxe:ASPxCallbackPanel runat="server" ID="ComponentQuotationPanel" ClientInstanceName="cQuotationComponentPanel" OnCallback="ComponentQuotation_Callback">
                                                    <PanelCollection>
                                                        <dxe:PanelContent runat="server">
                                                            <dxe:ASPxGridLookup ID="lookup_quotation" SelectionMode="Multiple" runat="server" TabIndex="7" ClientInstanceName="gridquotationLookup"
                                                                OnDataBinding="lookup_quotation_DataBinding"
                                                                KeyFieldName="ComponentID" Width="100%" TextFormatString="{0}" AutoGenerateColumns="False" MultiTextSeparator=", ">
                                                                <Columns>
                                                                    <dxe:GridViewCommandColumn ShowSelectCheckbox="True" VisibleIndex="0" Width="60" Caption=" " />
                                                                    <dxe:GridViewDataColumn FieldName="ComponentNumber" Visible="true" VisibleIndex="1" Caption="Document Number" Width="180" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="ComponentDate" Visible="true" VisibleIndex="2" Caption="Posting Date" Width="100" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="CustomerName" Visible="true" VisibleIndex="3" Caption="Customer Name" Width="150" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="ReferenceName" Visible="true" VisibleIndex="4" Caption="Reference" Width="150" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                    <dxe:GridViewDataColumn FieldName="BranchName" Visible="true" VisibleIndex="5" Caption="Unit" Width="150" Settings-AutoFilterCondition="Contains">
                                                                        <Settings AutoFilterCondition="Contains" />
                                                                    </dxe:GridViewDataColumn>
                                                                </Columns>
                                                                <GridViewProperties Settings-VerticalScrollBarMode="Auto" SettingsPager-Mode="ShowAllRecords">
                                                                    <Templates>
                                                                        <StatusBar>
                                                                            <table class="OptionsTable" style="float: right">
                                                                                <tr>
                                                                                    <td>
                                                                                        <dxe:ASPxButton ID="Close" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridQuotationLookup" UseSubmitBehavior="False" />
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                        </StatusBar>
                                                                    </Templates>
                                                                    <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" />
                                                                    <SettingsPager Mode="ShowAllRecords">
                                                                    </SettingsPager>
                                                                    <Settings ShowFilterRow="True" ShowFilterRowMenu="true" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                                                                </GridViewProperties>
                                                                <ClientSideEvents ValueChanged="function(s, e) { QuotationNumberChanged();}" GotFocus="function(s,e){gridquotationLookup.ShowDropDown();}" />
                                                            </dxe:ASPxGridLookup>
                                                        </dxe:PanelContent>
                                                    </PanelCollection>
                                                    <ClientSideEvents EndCallback="componentEndCallBack" />
                                                </dxe:ASPxCallbackPanel>
                                            </div>
                                            <div class="col-md-3 hide">
                                                <dxe:ASPxLabel ID="lbl_InvoiceNO" ClientInstanceName="clbl_InvoiceNO" runat="server" Text="Date">
                                                </dxe:ASPxLabel>
                                                <div style="width: 100%; height: 23px; border: 1px solid #e6e6e6;">
                                                    <dxe:ASPxCallbackPanel runat="server" ID="ComponentDatePanel" ClientInstanceName="cComponentDatePanel" OnCallback="ComponentDatePanel_Callback">
                                                        <PanelCollection>
                                                            <dxe:PanelContent runat="server">
                                                                <dxe:ASPxTextBox ID="txt_InvoiceDate" ClientInstanceName="ctxt_InvoiceDate" runat="server" Width="100%" ClientEnabled="false">
                                                                </dxe:ASPxTextBox>
                                                            </dxe:PanelContent>
                                                        </PanelCollection>
                                                    </dxe:ASPxCallbackPanel>
                                                </div>
                                            </div>

                                          

                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_Refference" runat="server" Text="Reference">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxTextBox ID="txt_Refference" ClientInstanceName="ctxt_Refference" runat="server" TabIndex="7" Width="100%">
                                                </dxe:ASPxTextBox>
                                            </div>
                                            <%--Rev 1.0: "simple-select" class add --%>
                                            <div class="col-md-3 simple-select">
                                                <dxe:ASPxLabel ID="ASPxLabel3" runat="server" Text="Salesman/Agents">
                                                </dxe:ASPxLabel>
                                                <asp:DropDownList ID="ddl_SalesAgent" runat="server" Width="100%" TabIndex="8">
                                                </asp:DropDownList>
                                            </div>
                                              <div style="clear: both">
                                            </div>
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="ASPxLabel2" runat="server" Text="Cash/Bank">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxComboBox ID="ddlCashBank" runat="server" ClientIDMode="Static" ClientInstanceName="cddlCashBank" TabIndex="11" Width="100%" OnCallback="ddlCashBank_Callback" ClientEnabled="false">
                                                    <ClientSideEvents GotFocus="function(s,e){cddlCashBank.ShowDropDown();}" />
                                                </dxe:ASPxComboBox>
                                                <%--TextChanged="ShowReceiptPayment" --%>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="row">
                                                    <div class="col-md-6 lblmTop8">
                                                        <dxe:ASPxLabel ID="ASPxLabel4" runat="server" Text="Credit Days">
                                                        </dxe:ASPxLabel>
                                                        <dxe:ASPxTextBox ID="txtCreditDays" ClientInstanceName="ctxtCreditDays" runat="server" TabIndex="12" Width="100%">
                                                            <MaskSettings Mask="<0..999999999>" AllowMouseWheel="false" />
                                                            <ClientSideEvents TextChanged="CreditDays_TextChanged" />
                                                        </dxe:ASPxTextBox>
                                                    </div>
                                                    <div class="col-md-6 lblmTop8">
                                                        <dxe:ASPxLabel ID="lbl_DueDate" runat="server" Text="Due Date">
                                                        </dxe:ASPxLabel>
                                                        <dxe:ASPxDateEdit ID="dt_SaleInvoiceDue" runat="server" EditFormat="Custom" EditFormatString="dd-MM-yyyy" ClientInstanceName="cdt_SaleInvoiceDue" TabIndex="12" Width="100%">
                                                            <ButtonStyle Width="13px">
                                                            </ButtonStyle>
                                                            <ClientSideEvents GotFocus="function(s,e){cdt_SaleInvoiceDue.ShowDropDown();}" />
                                                        </dxe:ASPxDateEdit>
                                                        <%--Rev 1.0--%>
                                                        <img src="/assests/images/calendar-icon.png" class="calendar-icon"/>
                                                        <%--Rev end 1.0--%>
                                                    </div>
                                                </div>
                                            </div> 
                                            <%--Rev 1.0: "simple-select" class add --%>
                                            <div class="col-md-3 simple-select">
                                                <dxe:ASPxLabel ID="lbl_Currency" runat="server" Text="Currency">
                                                </dxe:ASPxLabel>
                                                <asp:DropDownList ID="ddl_Currency" runat="server" Width="100%" TabIndex="13">
                                                </asp:DropDownList>
                                                
                                            </div>
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_Rate" runat="server" Text="Rate">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxTextBox ID="txt_Rate" ClientInstanceName="ctxt_Rate" runat="server" TabIndex="14" Width="100%" Height="28px">
                                                    <MaskSettings Mask="&lt;0..999999999&gt;.&lt;00..9999&gt;" AllowMouseWheel="false" />
                                                    <ClientSideEvents LostFocus="ReBindGrid_Currency" />
                                                </dxe:ASPxTextBox>
                                            </div>
                                              <div style="clear: both">
                                            </div>
                                            <div class="col-md-3">
                                                <dxe:ASPxLabel ID="lbl_AmountAre" runat="server" Text="Amounts are">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxComboBox ID="ddl_AmountAre" runat="server" ClientIDMode="Static" ClientInstanceName="cddl_AmountAre" TabIndex="15" Width="100%">
                                                    <ClientSideEvents SelectedIndexChanged="function(s, e) { PopulateGSTCSTVAT(e)}" LostFocus="function(s, e) { SetFocusonDemand(e)}"
                                                        GotFocus="function(s,e){cddl_AmountAre.ShowDropDown();}" />
                                                </dxe:ASPxComboBox>
                                            </div>
                                            <div class="col-md-3 hide">
                                                <dxe:ASPxLabel ID="lblVatGstCst" runat="server" Text="Select VAT/GST/CST">
                                                </dxe:ASPxLabel>
                                                <dxe:ASPxComboBox ID="ddl_VatGstCst" runat="server" ClientInstanceName="cddlVatGstCst" OnCallback="ddl_VatGstCst_Callback" TabIndex="16" Width="100%">
                                                    <ClientSideEvents EndCallback="Onddl_VatGstCstEndCallback" GotFocus="function(s,e){cddlVatGstCst.ShowDropDown();}" />
                                                </dxe:ASPxComboBox>
                                                <span id="Mandatorytaxcode" style="display: none" class="validclass">
                                                    <img id="1gridHistory_DXPEForm_efnew_DXEFL_DXEditor2_EI" class="dxEditors_edtError_PlasticBlue" src="/DXR.axd?r=1_36-tyKfc" title="Mandatory"></span>
                                            </div> 
                                            <div class="col-md-6">
                                                <dxe:ASPxLabel ID="ASPxLabel8" runat="server" Text="Remarks">
                                                </dxe:ASPxLabel>
                                                <asp:TextBox ID="txtRemarks" runat="server"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div style="clear: both;"></div>
                                        <div class="">
                                            <div style="display: none;">
                                                <a href="javascript:void(0);" onclick="OnAddNewClick()" class="btn btn-primary"><span>Add New</span> </a>
                                            </div>
                                            <div>
                                                <br />
                                            </div>
                                            <dxe:ASPxGridView runat="server" KeyFieldName="QuotationID" OnCustomUnboundColumnData="grid_CustomUnboundColumnData" ClientInstanceName="grid" ID="grid"
                                                Width="100%" SettingsBehavior-AllowSort="false" SettingsBehavior-AllowDragDrop="false" Settings-ShowFooter="false"
                                                OnBatchUpdate="grid_BatchUpdate"
                                                OnCustomCallback="grid_CustomCallback"
                                                OnDataBinding="grid_DataBinding"
                                                OnCellEditorInitialize="grid_CellEditorInitialize"
                                                OnRowInserting="Grid_RowInserting"
                                                OnRowUpdating="Grid_RowUpdating"
                                                OnRowDeleting="Grid_RowDeleting"
                                                OnHtmlRowPrepared="grid_HtmlRowPrepared"
                                                SettingsPager-Mode="ShowAllRecords" Settings-VerticalScrollBarMode="auto" Settings-VerticalScrollableHeight="150">
                                                <SettingsPager Visible="false"></SettingsPager>
                                                <Columns>
                                                    <dxe:GridViewCommandColumn ShowDeleteButton="false" ShowNewButtonInHeader="false" Width="3%" VisibleIndex="0" Caption="#">
                                                        <CustomButtons>
                                                            <dxe:GridViewCommandColumnCustomButton Text=" " ID="CustomDelete" Image-Url="/assests/images/crs.png">
                                                                <Image Url="/assests/images/crs.png">
                                                                </Image>
                                                            </dxe:GridViewCommandColumnCustomButton>
                                                        </CustomButtons>
                                                        <%-- <HeaderCaptionTemplate>
                                                            <dxe:ASPxHyperLink ID="btnNew" runat="server" Text="New" ForeColor="White">
                                                                <ClientSideEvents Click="function (s, e) { OnAddNewClick();}" />
                                                            </dxe:ASPxHyperLink>
                                                        </HeaderCaptionTemplate>--%>
                                                    </dxe:GridViewCommandColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="SrlNo" Caption="Sl" ReadOnly="true" VisibleIndex="1" Width="2%">
                                                        <PropertiesTextEdit>
                                                        </PropertiesTextEdit>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="ComponentNumber" Caption="Document No." VisibleIndex="2" ReadOnly="True" Width="9%">
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataButtonEditColumn FieldName="ProductName" Caption="Product" VisibleIndex="3" Width="14%">
                                                        <PropertiesButtonEdit>
                                                            <ClientSideEvents ButtonClick="ProductButnClick" KeyDown="ProductKeyDown" GotFocus="ProductsGotFocusFromID" />
                                                            <Buttons>
                                                                <dxe:EditButton Text="..." Width="20px">
                                                                </dxe:EditButton>
                                                            </Buttons>
                                                        </PropertiesButtonEdit>
                                                    </dxe:GridViewDataButtonEditColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="Description" Caption="Description" VisibleIndex="4" ReadOnly="True" Width="18%">
                                                        <CellStyle Wrap="True"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="Quantity" Caption="Quantity" VisibleIndex="5" Width="6%" PropertiesTextEdit-MaxLength="14" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesTextEdit DisplayFormatString="0.0000" Style-HorizontalAlign="Right">
                                                            <ClientSideEvents GotFocus="ProductsGotFocus" LostFocus="QuantityTextChange" />
                                                            <MaskSettings AllowMouseWheel="False" Mask="&lt;0..999999999&gt;.&lt;00..9999&gt;" />
                                                            <Style HorizontalAlign="Right">
                                                            </Style>
                                                        </PropertiesTextEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="UOM" Caption="UOM(Sale)" VisibleIndex="6" ReadOnly="true" Width="6%">
                                                        <PropertiesTextEdit>
                                                        </PropertiesTextEdit>
                                                    </dxe:GridViewDataTextColumn>
                                                    <%--Caption="Warehouse"--%>
                                                    <dxe:GridViewCommandColumn VisibleIndex="7" Caption="Stk Details" Width="6%">
                                                        <CustomButtons>
                                                            <dxe:GridViewCommandColumnCustomButton Text=" " ID="CustomWarehouse" Image-Url="/assests/images/warehouse.png" Image-ToolTip="Warehouse">
                                                                <Image ToolTip="Warehouse" Url="/assests/images/warehouse.png">
                                                                </Image>
                                                            </dxe:GridViewCommandColumnCustomButton>
                                                        </CustomButtons>
                                                    </dxe:GridViewCommandColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="StockQuantity" Caption="Stock Qty" VisibleIndex="8" Visible="false">
                                                        <PropertiesTextEdit DisplayFormatString="0.00">
                                                            <MaskSettings AllowMouseWheel="False" Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" />
                                                        </PropertiesTextEdit>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="StockUOM" Caption="Stock UOM" VisibleIndex="9" ReadOnly="true" Visible="false">
                                                        <PropertiesTextEdit>
                                                        </PropertiesTextEdit>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="SalePrice" Caption="Sale Price" VisibleIndex="10" Width="6%" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                            <ClientSideEvents GotFocus="ProductsGotFocus" LostFocus="SalePriceTextChange" />
                                                            <MaskSettings AllowMouseWheel="False" Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" />
                                                            <Style HorizontalAlign="Right">
                                                            </Style>
                                                        </PropertiesTextEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataSpinEditColumn FieldName="Discount" Caption="Disc(%)" VisibleIndex="11" Width="5%" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesSpinEdit MinValue="0" MaxValue="100" AllowMouseWheel="false" DisplayFormatString="0.00" MaxLength="6" Style-HorizontalAlign="Right">
                                                            <SpinButtons ShowIncrementButtons="false"></SpinButtons>
                                                            <ClientSideEvents LostFocus="DiscountTextChange" GotFocus="DiscountGotFocus" />
                                                            <Style HorizontalAlign="Right">
                                                            </Style>
                                                        </PropertiesSpinEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataSpinEditColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="Amount" Caption="Amount" VisibleIndex="12" Width="6%" ReadOnly="true" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                            <MaskSettings AllowMouseWheel="False" Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" />
                                                            <ClientSideEvents LostFocus="ProductAmountTextChange" GotFocus="AmountGotFocus" />
                                                            <Style HorizontalAlign="Right">
                                                            </Style>
                                                        </PropertiesTextEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataButtonEditColumn FieldName="TaxAmount" Caption="Charges" VisibleIndex="13" Width="6%" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesButtonEdit>
                                                            <ClientSideEvents ButtonClick="taxAmtButnClick" GotFocus="taxAmtButnClick1" KeyDown="TaxAmountKeyDown" />
                                                            <Buttons>
                                                                <dxe:EditButton Text="..." Width="20px">
                                                                </dxe:EditButton>
                                                            </Buttons>
                                                        </PropertiesButtonEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataButtonEditColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="TotalAmount" Caption="Net Amount" VisibleIndex="14" Width="6%" ReadOnly="true" HeaderStyle-HorizontalAlign="Right">
                                                        <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                            <MaskSettings AllowMouseWheel="False" Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" />
                                                            <Style HorizontalAlign="Right">
                                                            </Style>
                                                        </PropertiesTextEdit>
                                                        <HeaderStyle HorizontalAlign="Right" />
                                                        <CellStyle HorizontalAlign="Right"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewCommandColumn ShowDeleteButton="false" ShowNewButtonInHeader="false" Width="2.5%" VisibleIndex="15" Caption=" ">
                                                        <CustomButtons>
                                                            <dxe:GridViewCommandColumnCustomButton Text=" " ID="AddNew" Image-Url="/assests/images/add.png">
                                                                <Image Url="/assests/images/add.png">
                                                                </Image>
                                                            </dxe:GridViewCommandColumnCustomButton>
                                                        </CustomButtons>
                                                    </dxe:GridViewCommandColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="ComponentID" Caption="Component ID" VisibleIndex="16" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="TotalQty" Caption="Total Qty" VisibleIndex="18" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="BalanceQty" Caption="Balance Qty" VisibleIndex="17" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="IsComponentProduct" Caption="IsComponentProduct" VisibleIndex="19" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>
                                                    <dxe:GridViewDataTextColumn FieldName="IsLinkedProduct" Caption="IsLinkedProduct" VisibleIndex="20" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>

                                                    <dxe:GridViewDataTextColumn FieldName="ProdAvgvalue" Caption="ProdAvgvalue" VisibleIndex="21" ReadOnly="True" Width="0">
                                                    </dxe:GridViewDataTextColumn>

                                                    <dxe:GridViewDataTextColumn FieldName="ProductID" PropertiesTextEdit-ValidationSettings-ErrorImage-IconID="ghg" Caption="hidden Field Id" VisibleIndex="22" ReadOnly="True" Width="0" PropertiesTextEdit-Height="15px" PropertiesTextEdit-Style-CssClass="abcd">
                                                        <PropertiesTextEdit Height="15px">
                                                            <ValidationSettings>
                                                                <ErrorImage IconID="ghg">
                                                                </ErrorImage>
                                                            </ValidationSettings>
                                                            <Style CssClass="abcd">
                                                            </Style>
                                                        </PropertiesTextEdit>
                                                        <CellStyle Wrap="True" CssClass="abcd"></CellStyle>
                                                    </dxe:GridViewDataTextColumn>

                                                </Columns>
                                                <ClientSideEvents EndCallback="OnEndCallback" CustomButtonClick="OnCustomButtonClick" RowClick="GetVisibleIndex" BatchEditStartEditing="gridFocusedRowChanged" />
                                                <SettingsDataSecurity AllowEdit="true" />
                                                <SettingsEditing Mode="Batch" NewItemRowPosition="Bottom">
                                                    <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="row" />
                                                </SettingsEditing>
                                                <Settings VerticalScrollableHeight="150" VerticalScrollBarMode="Auto" />
                                                <SettingsBehavior ColumnResizeMode="Disabled" />
                                            </dxe:ASPxGridView>
                                        </div>
                                        <div style="clear: both;"></div>

                                           <PayD:ucPaymentDetails runat="server" ID="PaymentDetails" />

                                         <div style="clear: both;"></div>
                                        <br />
                                        <div class="col-md-12" id="divSubmitButton" runat="server">
                                            <asp:Label ID="lbl_quotestatusmsg" runat="server" Text="" Font-Bold="true" ForeColor="Red" Font-Size="Medium"></asp:Label>
                                            <dxe:ASPxButton ID="btn_SaveRecords" ClientInstanceName="cbtn_SaveRecords" runat="server" AutoPostBack="False" Text="Save & N&#818;ew" CssClass="btn btn-success" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                                <ClientSideEvents Click="function(s, e) {Save_ButtonClick();}" />
                                            </dxe:ASPxButton>
                                            <dxe:ASPxButton ID="ASPxButton1" ClientInstanceName="cbtn_SaveRecords" runat="server" AutoPostBack="False" Text="Save & Ex&#818;it" CssClass="btn btn-success" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                                <ClientSideEvents Click="function(s, e) {SaveExit_ButtonClick();}" />
                                            </dxe:ASPxButton>
                                            <%--   <asp:Button ID="ASPxButton2" runat="server" Text="UDF" CssClass="btn btn-primary" OnClientClick="if(OpenUdf()){ return false;}" />--%>
                                            <dxe:ASPxButton ID="ASPxButton2" ClientInstanceName="cbtn_SaveRecords" runat="server" AutoPostBack="False" Text="U&#818;DF" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                                <ClientSideEvents Click="function(s, e) {if(OpenUdf()){ return false}}" />
                                            </dxe:ASPxButton>
                                            <%--  Text="T&#818;axes"--%>
                                            <dxe:ASPxButton ID="ASPxButton3" ClientInstanceName="cbtn_SaveRecords" runat="server" AutoPostBack="False" Text="T&#818;ax & Charges" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1" UseSubmitBehavior="False">
                                                <ClientSideEvents Click="function(s, e) {Save_TaxesClick();}" />
                                            </dxe:ASPxButton>
                                            <uc1:VehicleDetailsControl runat="server" ID="VehicleDetailsControl" />
                                            <asp:HiddenField ID="hfControlData" runat="server" />

                                            <uc2:TermsConditionsControl runat="server" ID="TermsConditionsControl" />
                                            <asp:HiddenField runat="server" ID="hfTermsConditionData" />
                                            <asp:HiddenField runat="server" ID="hfTermsConditionDocType" Value="SI" />
                                            <%-- onclick=""--%>
                                            <%--<a href="javascript:void(0);" id="btnAddNew" runat="server" class="btn btn-primary"><span>[A]ttachment(s)</span></a>--%>
                                            <%--<dxe:ASPxButton ID="ASPxButton4" ClientInstanceName="cbtn_SaveRecords" runat="server" AccessKey="X" AutoPostBack="False" Text="[A]ttachment(s)" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1">
                            <ClientSideEvents Click="function(s, e) {Save_ButtonClick();}" />
                        </dxe:ASPxButton>--%>
                                            <%--<a class="btn btn-primary" href="javascript:void(0);" onclick="fn_PopOpen()"><span>[B]illing/Shipping</span> </a>--%>
                                        </div>
                                    </div>
                                </dxe:ContentControl>
                            </ContentCollection>
                            <%--test generel--%>
                        </dxe:TabPage>
                        <%--<dxe:TabPage Name="[A]ttachment(s)" Visible="false" Text="[A]ttachment(s)">
                            <ContentCollection>
                                <dxe:ContentControl runat="server">
                                </dxe:ContentControl>
                            </ContentCollection>
                        </dxe:TabPage>--%>
                        <dxe:TabPage Name="Billing/Shipping" Text="Billing/Shipping">
                            <ContentCollection>
                                <dxe:ContentControl runat="server">



                                    <%--<dxe:ASPxPopupControl ID="ASPXPopupControl1" runat="server" ContentUrl="AddArea_PopUp.aspx"
                                        CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="popupan" Height="250px"
                                        Width="300px" HeaderText="Add New Area" AllowResize="true" ResizingMode="Postponed" Modal="true">
                                        <ContentCollection>
                                            <dxe:PopupControlContentControl runat="server">
                                            </dxe:PopupControlContentControl>
                                        </ContentCollection>
                                        <HeaderStyle BackColor="Blue" Font-Bold="True" ForeColor="White" />
                                    </dxe:ASPxPopupControl>--%>

                                    <ucBS:BillingShippingControl runat="server" ID="BillingShippingControl" />

                                </dxe:ContentControl>
                            </ContentCollection>
                        </dxe:TabPage>

                    </TabPages>
                    <ClientSideEvents ActiveTabChanged="function(s, e) {
	                                            var activeTab   = page.GetActiveTab();
	                                            var Tab0 = page.GetTab(0);
	                                            var Tab1 = page.GetTab(1);
	                                            var Tab2 = page.GetTab(2);
                                                
                                               if(activeTab == Tab0)
	                                            {
	                                                disp_prompt('tab0');
	                                            }
	                                            if(activeTab == Tab1)
	                                            {
	                                                disp_prompt('tab1');
	                                            }
	                                           else if(activeTab == Tab2)
	                                            {
	                                                disp_prompt('tab2');
	                                            }


	                                            }"></ClientSideEvents>

                </dxe:ASPxPageControl>
            </div>
            <%--SelectCommand="SELECT s.id as ID,s.state as State from tbl_master_state s where (s.countryId = @State) ORDER BY s.state">--%>
            <asp:SqlDataSource ID="CountrySelect" runat="server" 
                SelectCommand="SELECT cou_id, cou_country as Country FROM tbl_master_country order by cou_country"></asp:SqlDataSource>
            <asp:SqlDataSource ID="StateSelect" runat="server" 
                SelectCommand="SELECT s.id as ID,s.state+' (State Code:' +s.StateCode+')' as State from tbl_master_state s where (s.countryId = @State) ORDER BY s.state">

                <SelectParameters>
                    <asp:Parameter Name="State" Type="string" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SelectCity" runat="server" 
                SelectCommand="SELECT c.city_id AS CityId, c.city_name AS City FROM tbl_master_city c where c.state_id=@City order by c.city_name">
                <SelectParameters>
                    <asp:Parameter Name="City" Type="string" />
                </SelectParameters>
            </asp:SqlDataSource>

            <asp:SqlDataSource ID="SelectArea" runat="server" 
                SelectCommand="SELECT area_id, area_name from tbl_master_area where (city_id = @Area) ORDER BY area_name">
                <SelectParameters>
                    <asp:Parameter Name="Area" Type="string" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="SelectPin" runat="server" 
                SelectCommand="select pin_id,pin_code from tbl_master_pinzip where city_id=@City order by pin_code">
                <SelectParameters>
                    <asp:Parameter Name="City" Type="string" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="sqltaxDataSource" runat="server" 
                SelectCommand="select Taxes_ID,Taxes_Name from dbo.Master_Taxes"></asp:SqlDataSource>

            <dxe:ASPxPopupControl ID="ASPXPopupControl" runat="server" ContentUrl="AddArea_PopUp.aspx"
                CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="popupan" Height="250px"
                Width="300px" HeaderText="Add New Area" AllowResize="true" ResizingMode="Postponed" Modal="true">
                <ContentCollection>
                    <dxe:PopupControlContentControl runat="server">
                    </dxe:PopupControlContentControl>
                </ContentCollection>
                <HeaderStyle BackColor="Blue" Font-Bold="True" ForeColor="White" />
            </dxe:ASPxPopupControl>

            <%--Sudip--%>
            <div class="PopUpArea">
                <asp:HiddenField ID="HdChargeProdAmt" runat="server" />
                <asp:HiddenField ID="HdChargeProdNetAmt" runat="server" />
                <%--ChargesTax--%>
                <dxe:ASPxPopupControl ID="Popup_Taxes" runat="server" ClientInstanceName="cPopup_Taxes"
                    Width="900px" Height="300px" HeaderText="Quotation Taxes" PopupHorizontalAlign="WindowCenter"
                    BackColor="white" PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
                    Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
                    ContentStyle-CssClass="pad">
                    <ContentStyle VerticalAlign="Top" CssClass="pad">
                    </ContentStyle>
                    <ContentCollection>
                        <dxe:PopupControlContentControl runat="server">
                            <div class="Top clearfix">
                                <div id="content-5" class="col-md-12  wrapHolder content horizontal-images" style="width: 100%; margin-right: 0;">
                                    <ul>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Gross Amount Total
                                                                <dxe:ASPxLabel ID="ASPxLabel6" runat="server" Text="ASPxLabel" ClientInstanceName="clblChargesTaxableGross"></dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="txtProductAmount" runat="server" Text="ASPxLabel" ClientInstanceName="ctxtProductAmount">
                                                                </dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li class="lblChargesGSTforGross">
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>GST</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="lblChargesGSTforGross" runat="server" Text="ASPxLabel" ClientInstanceName="clblChargesGSTforGross">
                                                                </dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Total Discount</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="txtProductDiscount" runat="server" Text="ASPxLabel" ClientInstanceName="ctxtProductDiscount">
                                                                </dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Total Charges</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="txtProductTaxAmount" runat="server" Text="ASPxLabel" ClientInstanceName="ctxtProductTaxAmount"></dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Net Amount Total
                                                                <dxe:ASPxLabel ID="ASPxLabel7" runat="server" Text="ASPxLabel" ClientInstanceName="clblChargesTaxableNet"></dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="txtProductNetAmount" runat="server" Text="ASPxLabel" ClientInstanceName="ctxtProductNetAmount"></dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li class="lblChargesGSTforNet">
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>GST</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <dxe:ASPxLabel ID="lblChargesGSTforNet" runat="server" Text="ASPxLabel" ClientInstanceName="clblChargesGSTforNet">
                                                                </dxe:ASPxLabel>
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                    </ul>
                                </div>
                                <div class="clear">
                                </div>
                                <%--Error Msg--%>

                                <div class="col-md-8" id="ErrorMsgCharges">
                                    <div class="lblHolder">
                                        <table>
                                            <tbody>
                                                <tr>
                                                    <td>Status
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>Tax Code/Charges Not Defined.
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>

                                </div>

                                <div class="clear">
                                </div>
                                <div class="col-md-12 gridTaxClass" style="">
                                    <dxe:ASPxGridView runat="server" KeyFieldName="TaxID" ClientInstanceName="gridTax" ID="gridTax"
                                        Width="100%" SettingsBehavior-AllowSort="false" SettingsBehavior-AllowDragDrop="false"
                                        Settings-ShowFooter="false" OnCustomCallback="gridTax_CustomCallback" OnBatchUpdate="gridTax_BatchUpdate"
                                        OnRowInserting="gridTax_RowInserting" OnRowUpdating="gridTax_RowUpdating" OnRowDeleting="gridTax_RowDeleting"
                                        OnDataBinding="gridTax_DataBinding">
                                        <Settings VerticalScrollableHeight="150" VerticalScrollBarMode="Auto"></Settings>
                                        <SettingsPager Visible="false"></SettingsPager>
                                        <Columns>
                                            <dxe:GridViewDataTextColumn FieldName="TaxName" Caption="Tax" VisibleIndex="0" Width="40%" ReadOnly="true">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn FieldName="calCulatedOn" Caption="Calculated On" VisibleIndex="0" Width="20%" ReadOnly="true">
                                                <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>

                                            <dxe:GridViewDataTextColumn FieldName="Percentage" Caption="Percentage" VisibleIndex="1" Width="20%">
                                                <PropertiesTextEdit Style-HorizontalAlign="Right" DisplayFormatString="0.00">
                                                    <MaskSettings Mask="<0..999999999999>.<0..99>" AllowMouseWheel="false" />
                                                    <ClientSideEvents LostFocus="PercentageTextChange" />
                                                    <ClientSideEvents />
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn FieldName="Amount" Caption="Amount" VisibleIndex="2" Width="20%">
                                                <PropertiesTextEdit Style-HorizontalAlign="Right" DisplayFormatString="0.00">
                                                    <MaskSettings Mask="<0..999999999999>.<0..99>" AllowMouseWheel="false" />
                                                    <ClientSideEvents LostFocus="QuotationTaxAmountTextChange" GotFocus="QuotationTaxAmountGotFocus" />
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>
                                        </Columns>
                                        <ClientSideEvents EndCallback="OnTaxEndCallback" />
                                        <SettingsDataSecurity AllowEdit="true" />
                                        <SettingsEditing Mode="Batch" NewItemRowPosition="Bottom">
                                            <BatchEditSettings ShowConfirmOnLosingChanges="false" EditMode="row" />
                                        </SettingsEditing>
                                    </dxe:ASPxGridView>
                                </div>
                                <div class="col-md-12">
                                    <table style="" class="chargesDDownTaxClass">
                                        <tr class="chargeGstCstvatClass">
                                            <td style="padding-top: 10px; padding-right: 25px"><span><strong>GST</strong></span></td>
                                            <td style="padding-top: 10px; width: 200px;">
                                                <dxe:ASPxComboBox ID="cmbGstCstVatcharge" ClientInstanceName="ccmbGstCstVatcharge" runat="server" SelectedIndex="0" TabIndex="2"
                                                    ValueType="System.String" Width="100%" EnableSynchronization="True" EnableIncrementalFiltering="True" TextFormatString="{0}"
                                                    OnCallback="cmbGstCstVatcharge_Callback">
                                                    <Columns>
                                                        <dxe:ListBoxColumn FieldName="Taxes_Name" Caption="Tax Component ID" Width="250" />
                                                        <dxe:ListBoxColumn FieldName="TaxCodeName" Caption="Tax Component Name" Width="250" />

                                                    </Columns>
                                                    <ClientSideEvents SelectedIndexChanged="ChargecmbGstCstVatChange"
                                                        GotFocus="chargeCmbtaxClick" />

                                                </dxe:ASPxComboBox>



                                            </td>
                                            <td style="padding-left: 15px; padding-top: 10px; width: 200px;">
                                                <dxe:ASPxTextBox ID="txtGstCstVatCharge" MaxLength="80" ClientInstanceName="ctxtGstCstVatCharge" TabIndex="3" ReadOnly="true" Text="0.00"
                                                    runat="server" Width="100%">
                                                    <MaskSettings Mask="<-999999999..999999999>.<0..99>" AllowMouseWheel="false" />
                                                </dxe:ASPxTextBox>

                                            </td>
                                            <td style="padding-left: 15px; padding-top: 10px">
                                                <input type="button" onclick="recalculateTaxCharge()" class="btn btn-info btn-small RecalculateCharge" value="Recalculate GST" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div class="clear">
                                    <br />
                                </div>



                                <div class="col-sm-3">
                                    <div>
                                        <dxe:ASPxButton ID="btn_SaveTax" ClientInstanceName="cbtn_SaveTax" runat="server" AutoPostBack="False" Text="Save" CssClass="btn btn-primary" meta:resourcekey="btnSaveRecordsResource1">
                                            <ClientSideEvents Click="function(s, e) {Save_TaxClick();}" />
                                        </dxe:ASPxButton>
                                        <dxe:ASPxButton ID="ASPxButton5" ClientInstanceName="cbtn_tax_cancel" runat="server" AutoPostBack="False" Text="Cancel" CssClass="btn btn-danger">
                                            <ClientSideEvents Click="function(s, e) {cPopup_Taxes.Hide();}" />
                                        </dxe:ASPxButton>
                                    </div>
                                </div>

                                <div class="col-sm-9">
                                    <table class="pull-right">
                                        <tr>
                                            <td style="padding-right: 30px"><strong>Total Charges</strong></td>
                                            <td>
                                                <div>
                                                    <dxe:ASPxTextBox ID="txtQuoteTaxTotalAmt" runat="server" Width="100%" ClientInstanceName="ctxtQuoteTaxTotalAmt" Text="0.00" HorizontalAlign="Right" Font-Size="12px" ReadOnly="true">
                                                        <MaskSettings Mask="<-999999999..99999999999999999999>.<0..99>" AllowMouseWheel="false" />
                                                        <%-- <MaskSettings Mask="<0..999999999999g>.<0..99g>" />--%>
                                                    </dxe:ASPxTextBox>
                                                </div>

                                            </td>
                                            <td style="padding-right: 30px; padding-left: 5px"><strong>Total Amount</strong></td>
                                            <td>
                                                <div>
                                                    <dxe:ASPxTextBox ID="txtTotalAmount" runat="server" Width="100%" ClientInstanceName="ctxtTotalAmount" Text="0.00" HorizontalAlign="Right" Font-Size="12px" ReadOnly="true">
                                                        <MaskSettings Mask="<-999999999..99999999999999999999>.<0..99>" AllowMouseWheel="false" />
                                                        <%--<MaskSettings Mask="<0..999999999999g>.<0..99g>" />--%>
                                                    </dxe:ASPxTextBox>
                                                </div>
                                            </td>

                                        </tr>
                                    </table>
                                </div>
                                <div class="col-sm-2" style="padding-top: 8px;">
                                    <span></span>
                                </div>
                                <div class="col-sm-4">
                                </div>
                                <div class="col-sm-2" style="padding-top: 8px;">
                                    <span></span>
                                </div>
                                <div class="col-sm-4">
                                </div>
                            </div>
                        </dxe:PopupControlContentControl>
                    </ContentCollection>
                    <HeaderStyle BackColor="LightGray" ForeColor="Black" />
                </dxe:ASPxPopupControl>
                <dxe:ASPxGridViewExporter ID="exporter" runat="server" Landscape="false" PaperKind="A3" PageHeader-Font-Size="Larger" PageHeader-Font-Bold="true">
                </dxe:ASPxGridViewExporter>
                <dxe:ASPxPopupControl ID="Popup_Warehouse" runat="server" ClientInstanceName="cPopup_Warehouse"
                    Width="900px" HeaderText="Warehouse Details" PopupHorizontalAlign="WindowCenter"
                    BackColor="white" PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
                    Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True"
                    ContentStyle-CssClass="pad">
                    <ClientSideEvents Closing="function(s, e) {
	closeWarehouse(s, e);}" />
                    <ContentStyle VerticalAlign="Top" CssClass="pad">
                    </ContentStyle>
                    <ContentCollection>
                        <dxe:PopupControlContentControl runat="server">
                            <div class="Top clearfix">
                                <div id="content-5" class="pull-right wrapHolder content horizontal-images" style="width: 100%; margin-right: 0px;">
                                    <ul>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Selected Product</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblProductName" runat="server"></asp:Label></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Entered Quantity </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="txt_SalesAmount" runat="server"></asp:Label>
                                                                <asp:Label ID="txt_SalesUOM" runat="server"></asp:Label>

                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li>
                                            <div class="lblHolder" id="divpopupAvailableStock" style="display: none;">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Available Stock</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAvailableStock" runat="server"></asp:Label>
                                                                <asp:Label ID="lblAvailableStockUOM" runat="server"></asp:Label>

                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>
                                        <li style="display: none;">
                                            <div class="lblHolder">
                                                <table>
                                                    <tbody>
                                                        <tr>
                                                            <td>Stock Quantity </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="txt_StockAmount" runat="server"></asp:Label>
                                                                <asp:Label ID="txt_StockUOM" runat="server"></asp:Label></td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </div>
                                        </li>

                                    </ul>
                                </div>

                                <div class="clear">
                                    <br />
                                </div>
                                <div class="clearfix col-md-12" style="background: #f5f4f3; padding: 8px 0; margin-bottom: 15px; border-radius: 4px; border: 1px solid #ccc;">
                                    <div>
                                        <div class="col-md-3" id="div_Warehouse">
                                            <div style="margin-bottom: 5px;">
                                                Warehouse
                                            </div>
                                            <div class="Left_Content" style="">
                                                <dxe:ASPxComboBox ID="CmbWarehouse" EnableIncrementalFiltering="True" ClientInstanceName="cCmbWarehouse" SelectedIndex="0"
                                                    TextField="WarehouseName" ValueField="WarehouseID" runat="server" Width="100%" OnCallback="CmbWarehouse_Callback">
                                                    <ClientSideEvents ValueChanged="function(s,e){CmbWarehouse_ValueChange()}" EndCallback="CmbWarehouseEndCallback"></ClientSideEvents>
                                                </dxe:ASPxComboBox>
                                                <span id="spnCmbWarehouse" class="pullleftClass fa fa-exclamation-circle iconRed" style="color: red; position: absolute; display: none" title="Mandatory"></span>
                                            </div>
                                        </div>
                                        <div class="col-md-3" id="div_Batch">
                                            <div style="margin-bottom: 5px;">
                                                Batch/Lot
                                            </div>
                                            <div class="Left_Content" style="">
                                                <dxe:ASPxComboBox ID="CmbBatch" EnableIncrementalFiltering="True" ClientInstanceName="cCmbBatch"
                                                    TextField="BatchName" ValueField="BatchID" runat="server" Width="100%" OnCallback="CmbBatch_Callback">
                                                    <ClientSideEvents ValueChanged="function(s,e){CmbBatch_ValueChange()}" EndCallback="CmbBatchEndCall"></ClientSideEvents>
                                                </dxe:ASPxComboBox>
                                                <span id="spnCmbBatch" class="pullleftClass fa fa-exclamation-circle iconRed" style="color: red; position: absolute; display: none" title="Mandatory"></span>
                                            </div>
                                        </div>
                                        <div class="col-md-4" id="div_Serial">
                                            <div style="margin-bottom: 5px;">
                                                Serial No &nbsp;&nbsp; (
                                                <input type="checkbox" id="myCheck" name="BarCode" onchange="AutoCalculateMandateOnChange(this)">Barcode )
                                            </div>
                                            <div class="" id="divMultipleCombo">
                                                <%--<dxe:ASPxComboBox ID="CmbSerial" EnableIncrementalFiltering="True" ClientInstanceName="cCmbSerial"
                                                    TextField="SerialName" ValueField="SerialID" runat="server" Width="100%" OnCallback="CmbSerial_Callback">
                                                </dxe:ASPxComboBox>--%>
                                                <dxe:ASPxDropDownEdit ClientInstanceName="checkComboBox" ID="ASPxDropDownEdit1" Width="85%" CssClass="pull-left" runat="server" AnimationType="None">
                                                    <DropDownWindowStyle BackColor="#EDEDED" />
                                                    <DropDownWindowTemplate>
                                                        <dxe:ASPxListBox Width="100%" ID="listBox" ClientInstanceName="checkListBox" SelectionMode="CheckColumn" OnCallback="CmbSerial_Callback"
                                                            runat="server">
                                                            <Border BorderStyle="None" />
                                                            <BorderBottom BorderStyle="Solid" BorderWidth="1px" BorderColor="#DCDCDC" />

                                                            <ClientSideEvents SelectedIndexChanged="OnListBoxSelectionChanged" EndCallback="listBoxEndCall" />
                                                        </dxe:ASPxListBox>
                                                        <table style="width: 100%">
                                                            <tr>
                                                                <td style="padding: 4px">
                                                                    <dxe:ASPxButton ID="ASPxButton4" AutoPostBack="False" runat="server" Text="Close" Style="float: right">
                                                                        <ClientSideEvents Click="function(s, e){ checkComboBox.HideDropDown(); }" />
                                                                    </dxe:ASPxButton>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </DropDownWindowTemplate>
                                                    <ClientSideEvents TextChanged="SynchronizeListBoxValues" DropDown="SynchronizeListBoxValues" GotFocus="function(s, e){ s.ShowDropDown(); }" />
                                                </dxe:ASPxDropDownEdit>
                                                <span id="spncheckComboBox" class="pullleftClass fa fa-exclamation-circle iconRed" style="color: red; position: absolute; display: none" title="Mandatory"></span>
                                                <div class="pull-left">
                                                    <i class="fa fa-commenting" id="abpl" aria-hidden="true" style="font-size: 16px; cursor: pointer; margin: 3px 0 0 5px;" title="Serial No " data-container="body" data-toggle="popover" data-placement="right" data-content=""></i>
                                                </div>
                                            </div>
                                            <div class="" id="divSingleCombo" style="display: none;">
                                                <dxe:ASPxTextBox ID="txtserial" runat="server" Width="85%" ClientInstanceName="ctxtserial" HorizontalAlign="Left" Font-Size="12px" MaxLength="49">
                                                    <ClientSideEvents TextChanged="txtserialTextChanged" />
                                                </dxe:ASPxTextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-3" id="div_Quantity">
                                            <div style="margin-bottom: 2px;">
                                                Quantity
                                            </div>
                                            <div class="Left_Content" style="">
                                                <dxe:ASPxTextBox ID="txtQuantity" runat="server" ClientInstanceName="ctxtQuantity" HorizontalAlign="Right" Font-Size="12px" Width="100%" Height="15px">
                                                    <MaskSettings Mask="&lt;0..999999999&gt;.&lt;00..9999&gt;" IncludeLiterals="DecimalSymbol" />
                                                    <ClientSideEvents TextChanged="function(s, e) {SaveWarehouse();}" />
                                                </dxe:ASPxTextBox>
                                                <span id="spntxtQuantity" class="pullleftClass fa fa-exclamation-circle iconRed" style="color: red; position: absolute; display: none" title="Mandatory"></span>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div>
                                            </div>
                                            <div class="Left_Content" style="padding-top: 14px">
                                                <dxe:ASPxButton ID="btnWarehouse" ClientInstanceName="cbtnWarehouse" Width="50px" runat="server" AutoPostBack="False" UseSubmitBehavior="True" Text="Add" CssClass="btn btn-primary">
                                                    <ClientSideEvents Click="function(s, e) {if(!document.getElementById('myCheck').checked) SaveWarehouse();}" />
                                                </dxe:ASPxButton>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="clearfix">
                                    <dxe:ASPxGridView ID="GrdWarehouse" runat="server" KeyFieldName="SrlNo" AutoGenerateColumns="False"
                                        Width="100%" ClientInstanceName="cGrdWarehouse" OnCustomCallback="GrdWarehouse_CustomCallback" OnDataBinding="GrdWarehouse_DataBinding"
                                        SettingsPager-Mode="ShowAllRecords" Settings-VerticalScrollBarMode="auto" Settings-VerticalScrollableHeight="200" SettingsBehavior-AllowSort="false">
                                        <Columns>
                                            <dxe:GridViewDataTextColumn Caption="Warehouse Name" FieldName="WarehouseName"
                                                VisibleIndex="0">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Available Quantity" FieldName="AvailableQty" Visible="false"
                                                VisibleIndex="1">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Quantity" FieldName="SalesQuantity"
                                                VisibleIndex="2">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Conversion Foctor" FieldName="ConversionMultiplier" Visible="false"
                                                VisibleIndex="3">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Stock Quantity" FieldName="StkQuantity" Visible="false"
                                                VisibleIndex="4">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Balance Stock" FieldName="BalancrStk" Visible="false"
                                                VisibleIndex="5">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Batch/Lot Number" FieldName="BatchNo"
                                                VisibleIndex="6">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Mfg Date" FieldName="MfgDate"
                                                VisibleIndex="7">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Expiry Date" FieldName="ExpiryDate"
                                                VisibleIndex="8">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn Caption="Serial Number" FieldName="SerialNo"
                                                VisibleIndex="9">
                                            </dxe:GridViewDataTextColumn>
                                            <dxe:GridViewDataTextColumn VisibleIndex="10" Width="80px">
                                                <DataItemTemplate>
                                                    <a href="javascript:void(0);" onclick="fn_Edit('<%# Container.KeyValue %>')" title="Delete">
                                                        <img src="../../../assests/images/Edit.png" /></a>
                                                    &nbsp;
                                                        <a href="javascript:void(0);" id="ADelete" onclick="fn_Deletecity('<%# Container.KeyValue %>')" title="Delete">
                                                            <img src="/assests/images/crs.png" /></a>
                                                </DataItemTemplate>
                                            </dxe:GridViewDataTextColumn>
                                        </Columns>
                                        <ClientSideEvents EndCallback="OnWarehouseEndCallback" />
                                        <SettingsPager Visible="false"></SettingsPager>
                                        <SettingsLoadingPanel Text="Please Wait..." />
                                    </dxe:ASPxGridView>
                                </div>
                                <div class="clearfix">
                                    <br />
                                    <div style="align-content: center">
                                        <dxe:ASPxButton ID="btnWarehouseSave" ClientInstanceName="cbtnWarehouseSave" Width="50px" runat="server" AutoPostBack="False" Text="OK" CssClass="btn btn-primary">
                                            <ClientSideEvents Click="function(s, e) {FinalWarehouse();}" />
                                        </dxe:ASPxButton>
                                    </div>
                                </div>
                            </div>
                        </dxe:PopupControlContentControl>
                    </ContentCollection>
                    <HeaderStyle BackColor="LightGray" ForeColor="Black" />
                </dxe:ASPxPopupControl>
            </div>
            <div>
                <asp:HiddenField ID="HdUpdateMainGrid" runat="server" />
                <asp:HiddenField ID="hdfIsDelete" runat="server" />
                <asp:HiddenField ID="hdfLookupCustomer" runat="server" />
                <asp:HiddenField ID="hdfProductID" runat="server" />
                <asp:HiddenField ID="hdfProductType" runat="server" />
                <asp:HiddenField ID="hdfProductSerialID" runat="server" />
                <asp:HiddenField ID="hdnProductQuantity" runat="server" />
                <asp:HiddenField ID="hdnRefreshType" runat="server" />
                <asp:HiddenField ID="hdnPageStatus" runat="server" />
                <asp:HiddenField ID="hdnDeleteSrlNo" runat="server" />
                <%--Subhra--%>
                <asp:HiddenField ID="hdntab2" runat="server"></asp:HiddenField>
            </div>

            <dxe:ASPxCallbackPanel runat="server" ID="CallbackPanel" ClientInstanceName="cCallbackPanel" OnCallback="CallbackPanel_Callback">
                <PanelCollection>
                    <dxe:PanelContent runat="server">
                    </dxe:PanelContent>
                </PanelCollection>
                <ClientSideEvents EndCallback="CallbackPanelEndCall" />
            </dxe:ASPxCallbackPanel>

            <dxe:ASPxCallbackPanel runat="server" ID="acpAvailableStock" ClientInstanceName="cacpAvailableStock" OnCallback="acpAvailableStock_Callback">
                <PanelCollection>
                    <dxe:PanelContent runat="server">
                    </dxe:PanelContent>
                </PanelCollection>
                <ClientSideEvents EndCallback="acpAvailableStockEndCall" />
            </dxe:ASPxCallbackPanel>

            <%--End Sudip--%>

            <asp:HiddenField ID="hdnCustomerId" runat="server" />
            <asp:HiddenField ID="hdnAddressDtl" runat="server" />
            <%--Debu Section--%>

            <%--Batch Product Popup Start--%>

            <dxe:ASPxPopupControl ID="ProductpopUp" runat="server" ClientInstanceName="cProductpopUp"
                CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Height="400"
                Width="700" HeaderText="Select Product" AllowResize="true" ResizingMode="Postponed" Modal="true">
                <HeaderTemplate>
                    <span>Select Product</span>
                </HeaderTemplate>
                <ContentCollection>
                    <dxe:PopupControlContentControl runat="server">
                        <label><strong>Search By product Name</strong></label>
                        <span style="color: red;">[Press ESC key to Cancel]</span>
                        <dxe:ASPxGridLookup ID="productLookUp" runat="server"   ClientInstanceName="cproductLookUp" OnDataBinding="productLookUp_DataBinding"
                            KeyFieldName="Products_ID" Width="800" TextFormatString="{0}" MultiTextSeparator=", " ClientSideEvents-TextChanged="ProductSelected" ClientSideEvents-KeyDown="ProductlookUpKeyDown">
                            <Columns>
                                <dxe:GridViewDataColumn FieldName="Products_Description" Caption="Name" Width="240">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="Products_Name" Caption="Name" Width="0">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="IsInventory" Caption="Inventory" Width="80">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="HSNSAC" Caption="HSN/SAC" Width="100">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="ClassCode" Caption="Class" Width="220">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="BrandName" Caption="Brand" Width="140">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                                <dxe:GridViewDataColumn FieldName="sProducts_isInstall" Caption="Installation Reqd." Width="120" Visible="false">
                                    <Settings AutoFilterCondition="Contains" />
                                </dxe:GridViewDataColumn>
                            </Columns>
                            <GridViewProperties Settings-VerticalScrollBarMode="Auto">

                                <Templates>
                                    <StatusBar>
                                        <table class="OptionsTable" style="float: right">
                                            <tr>
                                                <td>
                                                    <%--  <dxe:ASPxButton ID="ASPxButton1" runat="server" AutoPostBack="false" Text="Close" ClientSideEvents-Click="CloseGridLookup" />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </StatusBar>
                                </Templates>
                                <Settings ShowFilterRow="True" ShowFilterRowMenu="true" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                            </GridViewProperties>
                        </dxe:ASPxGridLookup>

                    </dxe:PopupControlContentControl>
                </ContentCollection>
                <HeaderStyle BackColor="Blue" Font-Bold="True" ForeColor="White" />
            </dxe:ASPxPopupControl>

              

            <%--InlineTax--%>

            <dxe:ASPxPopupControl ID="aspxTaxpopUp" runat="server" ClientInstanceName="caspxTaxpopUp"
                Width="850px" HeaderText="Select Tax" PopupHorizontalAlign="WindowCenter"
                PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
                Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
                <HeaderTemplate>
                    <span style="color: #fff"><strong>Select Tax</strong></span>
                    <dxe:ASPxImage ID="ASPxImage3" runat="server" ImageUrl="/assests/images/closePop.png" Cursor="pointer" CssClass="popUpHeader pull-right">
                        <ClientSideEvents Click="function(s, e){ 
                                                            cgridTax.CancelEdit();
                                                            caspxTaxpopUp.Hide();
                                                        }" />
                    </dxe:ASPxImage>
                </HeaderTemplate>
                <ContentCollection>
                    <dxe:PopupControlContentControl runat="server">
                        <asp:HiddenField runat="server" ID="setCurrentProdCode" />
                        <asp:HiddenField runat="server" ID="HdSerialNo" />
                        <asp:HiddenField runat="server" ID="HdProdGrossAmt" />
                        <asp:HiddenField runat="server" ID="HdProdNetAmt" />
                        <div id="content-6">
                            <div class="col-sm-3">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>Gross Amount
                                                    <dxe:ASPxLabel ID="ASPxLabel1" runat="server" Text="(Taxable)" ClientInstanceName="clblTaxableGross"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dxe:ASPxLabel ID="lblTaxProdGrossAmt" runat="server" Text="" ClientInstanceName="clblTaxProdGrossAmt"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <div class="col-sm-3 gstGrossAmount">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>GST</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dxe:ASPxLabel ID="lblGstForGross" runat="server" Text="" ClientInstanceName="clblGstForGross"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <div class="col-sm-3">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>Discount</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dxe:ASPxLabel ID="lblTaxDiscount" runat="server" Text="" ClientInstanceName="clblTaxDiscount"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>


                            <div class="col-sm-3">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>Net Amount
                                                    <dxe:ASPxLabel ID="ASPxLabel5" runat="server" Text="(Taxable)" ClientInstanceName="clblTaxableNet"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dxe:ASPxLabel ID="lblProdNetAmt" runat="server" Text="" ClientInstanceName="clblProdNetAmt"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <div class="col-sm-2 gstNetAmount">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>GST</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <dxe:ASPxLabel ID="lblGstForNet" runat="server" Text="" ClientInstanceName="clblGstForNet"></dxe:ASPxLabel>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                        </div>

                        <%--Error Message--%>
                        <div id="ContentErrorMsg">
                            <div class="col-sm-8">
                                <div class="lblHolder">
                                    <table>
                                        <tbody>
                                            <tr>
                                                <td>Status
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Tax Code/Charges Not defined.
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <table style="width: 100%;">
                            <tr>
                                <td colspan="2"></td>
                            </tr>

                            <tr>
                                <td colspan="2"></td>
                            </tr>
                            <tr style="display: none">
                                <td><span><strong>Product Basic Amount</strong></span></td>
                                <td>
                                    <dxe:ASPxTextBox ID="txtprodBasicAmt" MaxLength="80" ClientInstanceName="ctxtprodBasicAmt" TabIndex="1" ReadOnly="true"
                                        runat="server" Width="50%">
                                        <MaskSettings Mask="<0..999999999>.<0..99>" AllowMouseWheel="false" />
                                    </dxe:ASPxTextBox>
                                </td>
                            </tr>
                            <tr class="cgridTaxClass">
                                <td colspan="3">
                                    <dxe:ASPxGridView runat="server" OnBatchUpdate="taxgrid_BatchUpdate" KeyFieldName="Taxes_ID" ClientInstanceName="cgridTax" ID="aspxGridTax"
                                        Width="100%" SettingsBehavior-AllowSort="false" SettingsBehavior-AllowDragDrop="false" SettingsPager-Mode="ShowAllRecords" OnCustomCallback="cgridTax_CustomCallback"
                                        Settings-ShowFooter="false" AutoGenerateColumns="False" OnCellEditorInitialize="aspxGridTax_CellEditorInitialize" OnHtmlRowCreated="aspxGridTax_HtmlRowCreated"
                                        OnRowInserting="taxgrid_RowInserting" OnRowUpdating="taxgrid_RowUpdating" OnRowDeleting="taxgrid_RowDeleting">
                                        <Settings VerticalScrollableHeight="150" VerticalScrollBarMode="Auto"></Settings>
                                        <SettingsBehavior AllowDragDrop="False" AllowSort="False"></SettingsBehavior>
                                        <SettingsPager Visible="false"></SettingsPager>
                                        <Columns>
                                            <dxe:GridViewDataTextColumn VisibleIndex="1" FieldName="Taxes_Name" ReadOnly="true" Caption="Tax Component ID">
                                            </dxe:GridViewDataTextColumn>

                                            <dxe:GridViewDataTextColumn VisibleIndex="2" FieldName="taxCodeName" ReadOnly="true" Caption="Tax Component Name">
                                            </dxe:GridViewDataTextColumn>

                                            <dxe:GridViewDataTextColumn VisibleIndex="3" FieldName="calCulatedOn" ReadOnly="true" Caption="Calculated On">
                                                <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                    <MaskSettings Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" AllowMouseWheel="false" />
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>
                                            <%--<dxe:GridViewDataComboBoxColumn Caption="percentage" FieldName="TaxField" VisibleIndex="2">
                                                <PropertiesComboBox ClientInstanceName="cTaxes_Name" ValueField="Taxes_ID" TextField="Taxes_Name" DropDownStyle="DropDown">
                                                    <ClientSideEvents SelectedIndexChanged="cmbtaxCodeindexChange"
                                                        GotFocus="CmbtaxClick" />
                                                </PropertiesComboBox>
                                            </dxe:GridViewDataComboBoxColumn>--%>
                                            <dxe:GridViewDataTextColumn Caption="Percentage" FieldName="TaxField" VisibleIndex="4">
                                                <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                    <ClientSideEvents LostFocus="txtPercentageLostFocus" GotFocus="CmbtaxClick" />
                                                    <MaskSettings Mask="<0..999999999999>.<0..99>" AllowMouseWheel="false" />
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>

                                            <dxe:GridViewDataTextColumn VisibleIndex="5" FieldName="Amount" Caption="Amount" ReadOnly="true">
                                                <PropertiesTextEdit DisplayFormatString="0.00" Style-HorizontalAlign="Right">
                                                    <ClientSideEvents LostFocus="taxAmountLostFocus" GotFocus="taxAmountGotFocus" />
                                                    <MaskSettings Mask="&lt;0..999999999&gt;.&lt;00..99&gt;" AllowMouseWheel="false" />
                                                </PropertiesTextEdit>
                                                <CellStyle Wrap="False" HorizontalAlign="Right"></CellStyle>
                                            </dxe:GridViewDataTextColumn>
                                        </Columns>
                                        <%--  <SettingsPager Mode="ShowAllRecords"></SettingsPager>--%>
                                        <SettingsDataSecurity AllowEdit="true" />
                                        <SettingsEditing Mode="Batch">
                                            <BatchEditSettings EditMode="row" />
                                        </SettingsEditing>
                                        <ClientSideEvents EndCallback=" cgridTax_EndCallBack " RowClick="GetTaxVisibleIndex" />

                                    </dxe:ASPxGridView>

                                </td>
                            </tr>
                            <tr>
                                <td colspan="4">
                                    <table class="InlineTaxClass">
                                        <tr class="GstCstvatClass" style="">
                                            <td style="padding-top: 10px; padding-bottom: 15px; padding-right: 25px"><span><strong>GST</strong></span></td>
                                            <td style="padding-top: 10px; padding-bottom: 15px;">
                                                <dxe:ASPxComboBox ID="cmbGstCstVat" ClientInstanceName="ccmbGstCstVat" runat="server" SelectedIndex="-1" TabIndex="2"
                                                    ValueType="System.String" Width="100%" EnableSynchronization="True" EnableIncrementalFiltering="True" TextFormatString="{0}"
                                                    ClearButton-DisplayMode="Always" OnCallback="cmbGstCstVat_Callback">
                                                    <Columns>
                                                        <dxe:ListBoxColumn FieldName="Taxes_Name" Caption="Tax Component ID" Width="250" />
                                                        <dxe:ListBoxColumn FieldName="TaxCodeName" Caption="Tax Component Name" Width="250" />

                                                    </Columns>
                                                    <ClientSideEvents SelectedIndexChanged="cmbGstCstVatChange"
                                                        GotFocus="CmbtaxClick" />
                                                </dxe:ASPxComboBox>
                                            </td>
                                            <td style="padding-left: 15px; padding-top: 10px; padding-bottom: 15px; padding-right: 25px">
                                                <dxe:ASPxTextBox ID="txtGstCstVat" MaxLength="80" ClientInstanceName="ctxtGstCstVat" TabIndex="3" ReadOnly="true" Text="0.00"
                                                    runat="server" Width="100%">
                                                    <MaskSettings Mask="<-999999999..999999999>.<0..99>" AllowMouseWheel="false" />
                                                </dxe:ASPxTextBox>


                                            </td>
                                            <td>
                                                <input type="button" onclick="recalculateTax()" class="btn btn-info btn-small RecalculateInline" value="Recalculate GST" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <div class="pull-left">
                                        <asp:Button ID="Button1" runat="server" Text="Ok" TabIndex="5" CssClass="btn btn-primary mTop" OnClientClick="return BatchUpdate();" Width="85px" />
                                        <asp:Button ID="Button2" runat="server" Text="Cancel" TabIndex="5" CssClass="btn btn-danger mTop" Width="85px" OnClientClick="cgridTax.CancelEdit(); caspxTaxpopUp.Hide(); return false;" />
                                    </div>
                                    <table class="pull-right">
                                        <tr>
                                            <td style="padding-top: 10px; padding-right: 5px"><strong>Total Charges</strong></td>
                                            <td>
                                                <dxe:ASPxTextBox ID="txtTaxTotAmt" MaxLength="80" ClientInstanceName="ctxtTaxTotAmt" Text="0.00" ReadOnly="true"
                                                    runat="server" Width="100%" CssClass="pull-left mTop">
                                                    <MaskSettings Mask="<-999999999..999999999>.<0..99>" AllowMouseWheel="false" />
                                                </dxe:ASPxTextBox>

                                            </td>
                                        </tr>
                                    </table>


                                    <div class="clear"></div>
                                </td>
                            </tr>

                        </table>
                    </dxe:PopupControlContentControl>
                </ContentCollection>
                <ContentStyle VerticalAlign="Top" CssClass="pad"></ContentStyle>
                <HeaderStyle BackColor="LightGray" ForeColor="Black" />
            </dxe:ASPxPopupControl>
            <%--End debjyoti 22-12-2016--%>
            <dxe:ASPxCallbackPanel runat="server" ID="taxUpdatePanel" ClientInstanceName="ctaxUpdatePanel" OnCallback="taxUpdatePanel_Callback">
                <PanelCollection>
                    <dxe:PanelContent runat="server">
                    </dxe:PanelContent>
                </PanelCollection>
                <ClientSideEvents EndCallback="ctaxUpdatePanelEndCall" />
            </dxe:ASPxCallbackPanel>

            <%--Debu Section End--%>
        </asp:Panel>
    </div>
    </div>
    <div style="display: none">
        <dxe:ASPxDateEdit ID="dt_PlQuoteExpiry" runat="server" Date="" Width="100%" EditFormatString="dd-MM-yyyy" ClientInstanceName="tenddate" TabIndex="4">
            <ClientSideEvents DateChanged="Enddate" />
        </dxe:ASPxDateEdit>
    </div>
    <%--Compnent Tag Start--%>

    <dxe:ASPxPopupControl ID="ASPxProductsPopup" runat="server" ClientInstanceName="cProductsPopup"
        Width="900px" HeaderText="Select Tax" PopupHorizontalAlign="WindowCenter"
        PopupVerticalAlign="WindowCenter" CloseAction="CloseButton"
        Modal="True" ContentStyle-VerticalAlign="Top" EnableHierarchyRecreation="True">
        <HeaderTemplate>
            <strong><span style="color: #fff">Select Products</span></strong>
            <dxe:ASPxImage ID="ASPxImage3" runat="server" ImageUrl="/assests/images/closePop.png" Cursor="pointer" CssClass="popUpHeader pull-right">
                <ClientSideEvents Click="function(s, e){ 
                                                            cProductsPopup.Hide();
                                                        }" />
            </dxe:ASPxImage>
        </HeaderTemplate>
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
                <div style="padding: 7px 0;">
                    <input type="button" value="Select All Products" onclick="ChangeState('SelectAll')" class="btn btn-primary"></input>
                    <input type="button" value="De-select All Products" onclick="ChangeState('UnSelectAll')" class="btn btn-primary"></input>
                    <input type="button" value="Revert" onclick="ChangeState('Revart')" class="btn btn-primary"></input>
                </div>
                <dxe:ASPxGridView runat="server" KeyFieldName="ComponentDetailsID" ClientInstanceName="cgridproducts" ID="grid_Products"
                    Width="100%" SettingsBehavior-AllowSort="false" SettingsBehavior-AllowDragDrop="false" SettingsPager-Mode="ShowAllRecords" OnCustomCallback="cgridProducts_CustomCallback"
                    Settings-ShowFooter="false" AutoGenerateColumns="False" Settings-VerticalScrollableHeight="300" Settings-VerticalScrollBarMode="Visible">
                    <SettingsBehavior AllowDragDrop="False" AllowSort="False"></SettingsBehavior>
                    <SettingsPager Visible="false"></SettingsPager>
                    <Columns>
                        <dxe:GridViewCommandColumn ShowSelectCheckbox="True" Width="40" Caption=" " VisibleIndex="0" />
                        <dxe:GridViewDataTextColumn VisibleIndex="1" FieldName="SrlNo" Width="40" ReadOnly="true" Caption="Sl. No.">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="2" FieldName="ComponentNumber" Width="120" ReadOnly="true" Caption="Number">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="3" FieldName="ProductID" ReadOnly="true" Caption="Product" Width="0">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="4" FieldName="ProductsName" ReadOnly="true" Width="100" Caption="Product Name">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="5" FieldName="ProductDescription" Width="200" ReadOnly="true" Caption="Product Description">
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn Caption="Bal Quantity" FieldName="Quantity" Width="70" VisibleIndex="6">
                            <PropertiesTextEdit>
                                <MaskSettings Mask="<0..999999999999>.<0..99>" AllowMouseWheel="false" />
                            </PropertiesTextEdit>
                        </dxe:GridViewDataTextColumn>
                        <dxe:GridViewDataTextColumn VisibleIndex="7" FieldName="ComponentID" ReadOnly="true" Caption="ComponentID" Width="0">
                        </dxe:GridViewDataTextColumn>
                    </Columns>
                    <SettingsDataSecurity AllowEdit="true" />
                </dxe:ASPxGridView>
                <div class="text-center">
                    <asp:Button ID="Button3" runat="server" Text="OK" CssClass="btn btn-primary  mLeft mTop" OnClientClick="return PerformCallToGridBind();" />
                </div>
            </dxe:PopupControlContentControl>
        </ContentCollection>
        <ContentStyle VerticalAlign="Top" CssClass="pad"></ContentStyle>
        <HeaderStyle BackColor="LightGray" ForeColor="Black" />
    </dxe:ASPxPopupControl>

    <%--Compnent Tag End--%>

    <%-- Customer Payment & Recipt --%>

    <dxe:ASPxPopupControl ID="apcReciptPopup" runat="server"
        CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="capcReciptPopup" Height="630px"
        Width="1200px" HeaderText="Customer Receipt/Payment" Modal="true" AllowResize="true" ResizingMode="Postponed">
        <HeaderTemplate>
            <span>Customer Receipt/Payment</span>
        </HeaderTemplate>
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
            </dxe:PopupControlContentControl>
        </ContentCollection>
    </dxe:ASPxPopupControl>

    <%-- Customer Payment & Recipt --%>

    <asp:SqlDataSource runat="server" ID="dsCustomer" 
        SelectCommand="prc_CRMSalesInvoice_Details" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:Parameter Type="String" Name="Action" DefaultValue="PopulateCustomerDetail" />
        </SelectParameters>
    </asp:SqlDataSource>

    <%-- Customer Payment & Recipt --%>

    <%-- UDF Module Start --%>
    <dxe:ASPxPopupControl ID="ASPXPopupControl2" runat="server"
        CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="popup" Height="630px"
        Width="600px" HeaderText="Add/Modify UDF" Modal="true" AllowResize="true" ResizingMode="Postponed">
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
            </dxe:PopupControlContentControl>
        </ContentCollection>
    </dxe:ASPxPopupControl>
    <asp:HiddenField runat="server" ID="IsUdfpresent" />
    <asp:HiddenField runat="server" ID="Keyval_internalId" />
    <%-- UDF Module End--%>

    <%--Customer Popup--%>
    <dxe:ASPxPopupControl ID="DirectAddCustPopup" runat="server"
        CloseAction="CloseButton" PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" ClientInstanceName="AspxDirectAddCustPopup" Height="750px"
        Width="1020px" HeaderText="Add New Customer" Modal="true" AllowResize="true" ResizingMode="Postponed">
        <HeaderTemplate>
            <span>Add New Customer</span>
        </HeaderTemplate>
        <ContentCollection>
            <dxe:PopupControlContentControl runat="server">
            </dxe:PopupControlContentControl>
        </ContentCollection>
    </dxe:ASPxPopupControl>

    <%-- TaxDetails HiddenField Field --%>
    <asp:HiddenField runat="server" ID="HDItemLevelTaxDetails" />
    <asp:HiddenField runat="server" ID="HDHSNCodewisetaxSchemid" />
    <asp:HiddenField runat="server" ID="HDBranchWiseStateTax" />
    <asp:HiddenField runat="server" ID="HDStateCodeWiseStateIDTax" />
    <%-- TaxDetails HiddenField Field --%>

    <dxe:ASPxLoadingPanel ID="LoadingPanel" runat="server" ClientInstanceName="LoadingPanel" ContainerElementID="divSubmitButton"
        Modal="True">
    </dxe:ASPxLoadingPanel>
</asp:Content>