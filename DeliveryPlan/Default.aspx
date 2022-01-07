<%@ Page Title="" Language="C#" MasterPageFile="~/Navbar.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DeliveryPlan.Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .title p {
            margin: 15px 0 5px 0;
            font-weight: bold;
            font-size: 16px;
        }

        .detail h2 {
            margin: 5px 0 15px 0;
            font-weight: bold;
        }

        .box-shadow {
            border-radius: 6px;
            box-shadow: rgba(0, 0, 0, 0.18) 0px 2px 4px;
        }
    </style>
    <div class="container">
        <h2 class="text-center">Delivery Plan</h2>
        <hr />
        
    </div>
</asp:Content>
