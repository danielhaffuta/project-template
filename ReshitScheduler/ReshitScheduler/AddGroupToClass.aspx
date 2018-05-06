﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGroupToClass.aspx.cs" Inherits="ReshitScheduler.AddGroupToClass" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no"/>
    <meta http-equiv="x-ua-compatible" content="ie=edge"/>
    <link rel="stylesheet" href="css/bootstrap.min.css"/>
    <title></title>
</head>
<body dir="rtl">
    <form id="form1" runat="server">
    
        <div id="divContainer" class ="container">
            <asp:DropDownList ID="GroupsList" runat="server"></asp:DropDownList>
        </div>
    </form>
    <script src="js/jquery.slim.min.js"></script>
    <script src="js/popper.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
</body>
</html>