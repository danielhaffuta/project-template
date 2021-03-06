﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminForm.aspx.cs" Inherits="ReshitScheduler.AdminForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel = "stylesheet" href = "/css/page.css" />
     <link rel = "stylesheet" href = "/css/AdminForm.css" />
     <script src="/js/jquery.min.js"></script>
    <title>admin page</title>
   
</head>
<body dir ="rtl">
       <header>
          
              <img src="/media/reshitLogo.gif" alt="Alternate Text" id="logo"/>
              <span id="title"> 
                    שלום  <br /> 
                    <asp:Label ID="AdminName" runat="server" Text="Label"></asp:Label>
                    <button class="hamburger">&#9776;</button>
                   <button class="cross">&#735;</button>
               </span>
             
        </header> 

      <div class="menu">
      <ul>
          <li><a href="#" onclick="form1()"> ערוך טבלאות</a></li>
          <li><a href="#" onclick="classShow()">מערכת שעות</a></li>
          <li><a href="#" onclick="disconnect()">התנתק</a></li>
          
      </ul>
    </div> 

     <form id="allOptions" runat="server">
           <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
            </asp:ScriptManager>   
         <div class="options" id="editTables" dir ="rtl">
    
 
                   <label>בחר טבלה לעריכה:</label>
                 <br />
                  <asp:dropdownlist ID="courseEdit" runat="server" AutoPostBack="True" 
                 onselectedindexchanged="itemSelected">
                </asp:dropdownlist>
            </div>
         <div class="options" id="classShow">
             <label>בחר כיתה להצגה</label>

         </div>
     </form>
        <footer>
        <p>
           <img src="/media/arr_logo.png" alt="Alternate Text" />  <b>אתר זה אחלה</b>
        </p>
    </footer>
</body>
</html>
     <script type="text/javascript" src="/js/AdminForm.js"></script>
