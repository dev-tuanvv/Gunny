<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Tank.Flash._Default" %>

<html>

<head id="Head1" runat="server">
<title>DDTank</title>
<style type="text/css"> 
    html, body	{ height:100%; }
      body
        {
            margin: 0px auto;
            padding: 0px;
            background-image: url(images/bg_all.jpg);
	     background-repeat: no-repeat;
        background-position: center center;
       
    </style>
   
</head>
<body>
    <table style="width: 100%; height: 100%;">
        <tr>
            <td valign="middle" align="center">
              <iframe id="DDT_Game"  scrolling="no" frameborder="0" height="600" width="1000" src="flash/client.aspx?<%=Content %>"></iframe>
            </td>
        </tr>
        </table>

</body>
</html>
