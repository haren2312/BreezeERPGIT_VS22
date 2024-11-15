﻿<%@ Page Title="Document" Language="C#" MasterPageFile="~/OMS/MasterPage/ERP.Master" AutoEventWireup="true" CodeBehind="Vehicle_Document.aspx.cs" Inherits="ERP.OMS.Management.Master.Vehicle_Document" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

          <script type="text/javascript">

              function Show() {
                  var url = "frmAddDocuments.aspx?id=Vehicle_Document.aspx&id1=Vehicle"; //change110517
                  popup.SetContentUrl(url);
                  popup.Show();

              }
              function disp_prompt(name) {
                  if (name == "tab0") {

                      document.location.href = "VehicleAddEdit.aspx?id=" + <%=Session["KeyVal"]%> + "";
              }
              if (name == "tab1") {
                  //alert(name);
                  document.location.href = "Vehicle_Document.aspx";
              }
          }

          function OnDocumentView(obj1, obj2) {
              var docid = obj1;
              var filename;
              var chk = obj2.includes("~");
              if (chk) {
                  filename = obj2.split('~')[1];
              }
              else {
                  filename = obj2.split('/')[2];
              }
              if (filename != '' && filename != null) {
                  var d = new Date();
                  var n = d.getFullYear();
                  var url = '\\OMS\\Management\\Documents\\' + docid + '\\' + n + '\\' + filename;
                  var seturl = '\\OMS\\Management\\DailyTask\\viewImage.aspx?id=' + url;
                  popup.contentUrl = url;
                  popup.Show();
              }
              else {
                  jAlert('File not found.')
              }
          }

          function DeleteRow(keyValue) {
              jConfirm('Confirm delete?', 'Confirmation Dialog', function (r) {
                  if (r == true) {
                      gridDocument.PerformCallback('Delete~' + keyValue);
                      height();
                  }
              });
          }

          function ShowEditForm(KeyValue) {
              var url = 'frmAddDocuments.aspx?id=Vehicle_Document.aspx&id1=HRrecruitmentagent&AcType=Edit&docid=' + KeyValue; //change110517
              popup.SetContentUrl(url);
              //alert (url);
              popup.Show();
          }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="panel-heading">
        <div class="panel-title">
            <h3>Vehicle Document</h3>
            <div class="crossBtn"><a href="Vehicle.aspx"><i class="fa fa-times"></i></a></div>
        </div>
    </div>

    <div class="form_main">
       <table class="TableMain100">
            <tr>
                <td style="text-align: center">
                    <asp:Label ID="lblHeader" runat="server" Font-Bold="True" Font-Size="15px" ForeColor="Navy" Width="819px" Height="18px"></asp:Label>
                </td>
            </tr>
            <tr>
               <td>

                   <dxe:ASPxPageControl ID="ASPxPageControl1" runat="server" ActiveTabIndex="2" ClientInstanceName="page">
                        <TabPages>
                            <dxe:TabPage Text="General" Name="General">
                                <ContentCollection>
                                    <dxe:ContentControl runat="server">
                                    </dxe:ContentControl>
                                </ContentCollection>
                            </dxe:TabPage>
                            <dxe:TabPage Name="Documents" Text="Documents">
                                <ContentCollection>
                                    <dxe:ContentControl runat="server">
                                        <div class="pull-left"><a href="javascript:void(0);" onclick="Show();" class="btn btn-primary"><span>Add New</span> </a></div>
                                        <dxe:ASPxGridView ID="EmployeeDocumentGrid" runat="server" AutoGenerateColumns="False"
                                            ClientInstanceName="gridDocument" KeyFieldName="Id" Width="100%" Font-Size="12px"
                                            OnCustomCallback="EmployeeDocumentGrid_CustomCallback">
                                            <Columns>
                                                <dxe:GridViewDataTextColumn FieldName="Id" ReadOnly="True" VisibleIndex="0" Visible="False">
                                                    <EditFormSettings Visible="False" />
                                                </dxe:GridViewDataTextColumn>
                                                <dxe:GridViewDataTextColumn FieldName="Type" VisibleIndex="0" Caption="Doc. Type"
                                                    Width="25%">
                                                </dxe:GridViewDataTextColumn>
                                                <dxe:GridViewDataTextColumn FieldName="FileName" VisibleIndex="1" Caption="Doc. Name"
                                                    Width="25%">
                                                </dxe:GridViewDataTextColumn>
                                                <dxe:GridViewDataTextColumn FieldName="Src" VisibleIndex="2" Visible="False">
                                                </dxe:GridViewDataTextColumn>
                                                <dxe:GridViewDataTextColumn FieldName="FilePath" ReadOnly="True" VisibleIndex="2"
                                                    Caption="Document Physical Location" Width="25%">
                                                </dxe:GridViewDataTextColumn>
                                                <dxe:GridViewDataHyperLinkColumn Caption="View" FieldName="Src" Visible="false"
                                                    Width="15%">
                                                    <DataItemTemplate>
                                                        <%-- <a href='viewImage.aspx?id=<%#Eval("Src") %>' target="_blank">View</a>--%>
                                                        <a onclick="OnDocumentView('<%#Eval("Src") %>')" style="cursor: pointer;">View</a>
                                                    </DataItemTemplate>
                                                </dxe:GridViewDataHyperLinkColumn>

                                                <dxe:GridViewDataTextColumn VisibleIndex="3" Width="8%" HeaderStyle-HorizontalAlign="Center" CellStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>Action</HeaderTemplate>
                                                    <CellStyle CssClass="gridcellleft">
                                                    </CellStyle>
                                                    <DataItemTemplate>
                                                        <% if (rights.CanView)
                                                           { %>
                                                        <a onclick="OnDocumentView('<%#Eval("doc") %>','<%#Eval("Src") %>')" style="text-decoration: none; cursor: pointer;" title="View" class="pad">
                                                            <img src="../../../assests/images/viewIcon.png" />
                                                        </a><% } %>

                                                        <% if (rights.CanEdit)
                                                           { %>
                                                        <a href="javascript:void(0);" onclick="ShowEditForm('<%# Container.KeyValue %>');" style="text-decoration: none;" title="Edit" class="pad">
                                                            <img src="../../../assests/images/Edit.png" />
                                                        </a><% } %>

                                                        <% if (rights.CanDelete)
                                                           { %>
                                                        <a href="javascript:void(0);" onclick="DeleteRow('<%# Container.KeyValue %>')" style="text-decoration: none;" title="Delete">
                                                            <img src="../../../assests/images/Delete.png" />
                                                        </a><% } %>

                                                       
                                                    </DataItemTemplate>
                                                    <EditFormSettings Visible="False"></EditFormSettings>
                                                </dxe:GridViewDataTextColumn>


                                            </Columns>
                                            <Settings ShowStatusBar="Visible" ShowTitlePanel="True" />
                                            <SettingsEditing Mode="PopupEditForm" PopupEditFormHeight="250px" PopupEditFormHorizontalAlign="Center"
                                                PopupEditFormModal="True" PopupEditFormVerticalAlign="WindowCenter" PopupEditFormWidth="500px"
                                                EditFormColumnCount="1" />
                                            <Styles>
                                                <LoadingPanel ImageSpacing="10px">
                                                </LoadingPanel>
                                                <Header ImageSpacing="5px" SortingImageSpacing="5px">
                                                </Header>
                                            </Styles>
                                            <SettingsText PopupEditFormCaption="Add/Modify Family Relationship" ConfirmDelete="Confirm delete?" />
                                            <SettingsPager NumericButtonCount="20" PageSize="20">
                                            </SettingsPager>
                                            <SettingsBehavior ColumnResizeMode="NextColumn" ConfirmDelete="True" />
                                        </dxe:ASPxGridView>
                                        <dxe:ASPxPopupControl ID="ASPXPopupControl" runat="server" ContentUrl="frmAddDocuments.aspx"
                                            CloseAction="CloseButton" Top="120" Left="300" ClientInstanceName="popup" Height="470px"
                                            Width="900px" HeaderText="Add Document" Modal="true" AllowResize="true" ResizingMode="Postponed">
                                            <ContentCollection>
                                                <dxe:PopupControlContentControl runat="server">
                                                </dxe:PopupControlContentControl>
                                            </ContentCollection>
                                        </dxe:ASPxPopupControl>
                                    </dxe:ContentControl>
                                </ContentCollection>
                            </dxe:TabPage>

                        </TabPages>
                        <ClientSideEvents ActiveTabChanged="function(s, e) {
	                                            var activeTab   = page.GetActiveTab();
	                                            var Tab0 = page.GetTab(0);
	                                            var Tab1 = page.GetTab(1);
	                                      
	                                            if(activeTab == Tab0)
	                                            {
	                                                disp_prompt('tab0');
	                                            }
	                                            if(activeTab == Tab1)
	                                            {
	                                                disp_prompt('tab1');
	                                            }
	                                            
	                                            
	                                            }"></ClientSideEvents>
                        <ContentStyle>
                            <Border BorderColor="#002D96" BorderStyle="Solid" BorderWidth="1px" />
                        </ContentStyle>
                        <LoadingPanelStyle ImageSpacing="6px">
                        </LoadingPanelStyle>
                        <TabStyle Font-Size="12px">
                        </TabStyle>
                    </dxe:ASPxPageControl>


               </td> 
               <td><asp:TextBox ID="txtID" runat="server" Visible="false"></asp:TextBox></td>
            </tr>
       </table>
    </div>

</asp:Content>
