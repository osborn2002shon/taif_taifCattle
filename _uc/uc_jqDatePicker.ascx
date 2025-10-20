<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uc_jqDatePicker.ascx.vb" Inherits="taifCattle.uc_jqDatePicker" %>
<script type="text/javascript">
    $(function () {
        $('#<%= TextBox_date.ClientID%>').datepicker(
                {
                    dateFormat: "yy/mm/dd",
                    changeYear: true,
                    changeMonth: true,
                    yearRange: "c-2:c+2"
                    //yearRange: "1883:+0"
                }).on("change", function () {
                    if ("<%= AutoPostBack%>" == "True") {
                        $("#<%= Button_ValueChanged.ClientID%>").click()
                    }
                })
        });
</script>
<style type="text/css">
    .hov {
        min-width:100px;
        text-align:center;
    }
    .hov:hover {
        cursor:pointer;
    }
</style>
<asp:TextBox ID="TextBox_date" runat="server" CssClass="hov"></asp:TextBox>
<div style="display:none;">
    <asp:Button runat="server" ID="Button_ValueChanged" />
</div>