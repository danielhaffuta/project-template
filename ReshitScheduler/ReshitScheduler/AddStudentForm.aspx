﻿<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AddStudentForm.aspx.cs" Inherits="ReshitScheduler.AddStudentForm" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel = "stylesheet" href = "/css/Student.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainForm" runat="server">
    <div class="row justify-content-center mt-5">
        <div class="col col-sm-6  bg-info" >
            <div class="form-group form-inline row">
                <label id="lblClasses" for="drpClassesList" class="col-form-label col-sm-3  col-md-4">כיתה:</label>
                <asp:DropDownList ID="drpClassesList" runat="server" OnSelectedIndexChanged="drpClassesList_SelectedIndexChanged"
                     CssClass="form-control col col-sm-9 col-md-8" DataValueField="id"
                     DataTextField="name" AutoPostBack="true"></asp:DropDownList>
            </div>


        </div>
    </div>

    <asp:Panel runat="server" ID="pnlStudents" CssClass="row justify-content-center mt-3">
        <div class="col text-center" >
            <asp:GridView ID="gvStudents" runat="server" AutoGenerateColumns ="false" CssClass="table table-striped table-bordered">
                <Columns>
                    <asp:BoundField DataField="name" HeaderText="שם תלמיד">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:ImageField DataImageUrlField="picture_path" HeaderText="תמונה">
                        <ControlStyle Width="100px" />
                    </asp:ImageField>
                    <asp:BoundField DataField="father_full_name" HeaderText="שם האב">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="mother_full_name" HeaderText="שם האם">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="father_cellphone" HeaderText="טלפון האב">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="mother_cellphone" HeaderText="טלפון האם">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="father_full_name" HeaderText="טלפון בבית">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="parents_email" HeaderText="אי-מייל">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                    <asp:BoundField DataField="settlement" HeaderText="יישוב">
                    <HeaderStyle Font-Bold="True" />
                    </asp:BoundField>
                        
                </Columns>
            </asp:GridView>
        </div>

    </asp:Panel>
    <div class="row justify-content-center mt-3">
        <div class="col col-sm-6  bg-info" >
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">שם התלמיד:</label>
                <asp:TextBox id="txtStudentFirstName" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">שם משפחה:</label>
                <asp:TextBox id="txtStudentLastName" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>



            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">תמונה:</label>

                <asp:FileUpload ID="FileUpload1" runat="server" CssClass="btn " />


            </div>




            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">שם האב:</label>
                <asp:TextBox id="txtFather_full_name" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">שם האם:</label>
                <asp:TextBox id="txtMother_full_name" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">טלפון האב:</label>
                <asp:TextBox id="txtFather_cellphone" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">טלפון האם:</label>
                <asp:TextBox id="txtMother_cellphone" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>

            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">טלפון בבית:</label>
                <asp:TextBox id="txtHome_phone" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>

            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">אי-מייל:</label>
                <asp:TextBox id="txtParents_email" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>
            <div class="form-group form-inline row">
                <label class="col-form-label col-sm-3 col-md-4">יישוב:</label>
                <asp:TextBox id="txtSettlement" runat="server" CssClass="form-control col col-sm-9 col-md-8"></asp:TextBox>
            </div>

            <div class="form-row justify-content-center btn-group-vertical">
                <button  runat="server" onserverclick="BtnAddStudent_Click" class="btn btn-outline-dark">הוסף תלמיד</button>
                <button  runat="server" onserverclick="BtnBack_Click" class="btn btn-outline-dark">חזור</button>
            </div>
         </div>
    </div>
</asp:Content>

