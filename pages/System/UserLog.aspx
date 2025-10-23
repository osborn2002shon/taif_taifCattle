<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/_mp/mp_default.master" CodeBehind="UserLog.aspx.vb" Inherits="taifCattle.UserLog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder_path" runat="server">
    <i class="fas fa-history"></i>使用者操作紀錄
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder_title" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder_content" runat="server">
    <div class="queryBox">
        <div class="queryBox-header">
            操作紀錄查詢與列表
        </div>
        <div class="queryBox-body">
        <div class="row">
            <div class="col">
                <label>操作時間(起)</label>
                <input type="date" id="dateBeg" name="dateBeg" class="form-control" value="<%=Property_Query_dateBeg.ToString("yyyy-MM-dd") %>">
            </div>
            <div class="col">
                <label>操作日期(訖)</label>
                <input type="date" id="dateEnd" name="dateEnd" class="form-control" value="<%=Property_Query_dateEnd.ToString("yyyy-MM-dd") %>">
            </div>
            <div class="col">
                <label>操作項目</label>
                <asp:DropDownList ID="DropDownList_userLogItem" runat="server" class="form-select"></asp:DropDownList>
            </div>
        </div>
        <div class="row">
            <div class="col">
                <label>操作類型</label>
                <asp:DropDownList ID="DropDownList_userLogType" runat="server" class="form-select"></asp:DropDownList>
            </div>
            <div class="col">
                <label>關鍵字</label>
                <div class="input-group">
                     <asp:TextBox ID="TextBox_keyWord" runat="server" CssClass="form-control" placeHolder="請輸入姓名、帳號、IP"></asp:TextBox>
                    <span class="input-group-text" onclick="clearControl('<%= TextBox_keyWord.ClientID %>')" style="cursor: pointer;"><i class="fa-solid fa-xmark"></i></span>
                </div>
            </div>
                
        </div>
            <div class="row">
                <div class="col text-center">
                    <asp:LinkButton ID="LinkButton_query" runat="server" CssClass="btn btn-primary">
                        <i class="fas fa-search"></i>查詢
                    </asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="queryBox-footer"></div>
    </div>
    <div class="row m-0 mt-3 mb-3 align-items-center">
        <div class="col p-0">
           
            <asp:LinkButton ID="LinkButton_excel" runat="server" CssClass="btn btn-outline-success">下載列表</asp:LinkButton>
        </div>
         <div class="col p-0 text-end">
             共 <asp:Label ID="Label_recordCount" runat="server" Text="0"></asp:Label> 筆
         </div>
    </div>
    <div class="table-responsive gv-tb">
  
        <asp:GridView ID="GridView_userLog" runat="server" CssClass="gv" AutoGenerateColumns="false" 
            AllowPaging="true" PageSize="10" ShowHeaderWhenEmpty="true" HeaderStyle-CssClass="text-center">
            <Columns>
                <asp:BoundField DataField="logDateTime" HeaderText="操作時間" 
                    DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" ItemStyle-Width="15%" ItemStyle-CssClass="text-center"/>

                <asp:BoundField DataField="IP" HeaderText="IP位址" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                <asp:BoundField DataField="name" HeaderText="使用者姓名" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                <asp:BoundField DataField="account" HeaderText="帳號名稱" ItemStyle-Width="15%" ItemStyle-CssClass="text-center" />

                <asp:BoundField DataField="logType" HeaderText="操作類型" ItemStyle-Width="10%" ItemStyle-CssClass="text-center"/>

                <asp:BoundField DataField="logItem" HeaderText="操作項目" ItemStyle-Width="15%" ItemStyle-CssClass="text-center"/>

                <asp:BoundField DataField="memo" HeaderText="操作內容"
                    HtmlEncode="False" ItemStyle-Width="25%" ItemStyle-Wrap="True" />
          
            </Columns>
             <EmptyDataTemplate>
               <div class="text-danger text-center py-2 fw-bold">
                    目前沒有任何操作紀錄。
                </div>
            </EmptyDataTemplate>
            <PagerStyle HorizontalAlign="Center"/>
        </asp:GridView>
    </div>
     <script type="text/javascript">
         function clearControl(controlId) {
             var textbox = document.getElementById(controlId);
             textbox.value = '';
             textbox.focus(); // 清除後自動聚焦
         }
     </script>
</asp:Content>
