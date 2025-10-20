Imports Microsoft.VisualBasic

Namespace taifCattle

    ''' <summary>
    ''' Class 控制項相關函式庫
    ''' </summary>
    Public Class Control

        '※ 系統資料從什麼時候開始

#Region "Enum // Structure"

        ''' <summary>
        ''' stru CityAreaInfo
        ''' </summary>
        ''' <remarks></remarks>
        Structure stru_CityAreaInfo
            Property twID As Integer
            Property cityID As Integer
            Property cityName As String
            Property areaName As String
        End Structure

        ''' <summary>
        ''' enum 上傳的檔案格式分類
        ''' </summary>
        Enum enum_fileType
            img
            normalFile
            docOnly
            xlsOnly
            pdfOnly
        End Enum

        ''' <summary>
        ''' stru 上傳檔案的詳細資訊
        ''' </summary>
        Structure stru_UploadInfo

            ''' <summary>
            ''' 檔名(含附檔名)
            ''' </summary>
            ''' <returns></returns>
            Property fileName As Object

            ''' <summary>
            ''' 副檔名(含.)
            ''' </summary>
            ''' <returns></returns>
            Property fileExtension As Object

            ''' <summary>
            ''' 新檔名(含副檔名)
            ''' </summary>
            ''' <returns></returns>
            Property fileName_new As Object

            ''' <summary>
            ''' 相對路徑(無用 預留)
            ''' </summary>
            ''' <returns></returns>
            Property filePath_relative As Object

            ''' <summary>
            ''' 絕對路徑(無用 預留)
            ''' </summary>
            ''' <returns></returns>
            Property filePath_absolute As Object

            ''' <summary>
            ''' 實體路徑(存檔用不含檔案名, D:\aa\bb\)
            ''' </summary>
            ''' <returns></returns>
            Property filePath_physical As Object

            ''' <summary>
            ''' 虛擬路徑(顯示用不含檔案名, ~\aa\bb\)
            ''' </summary>
            ''' <returns></returns>
            Property filePath_virtual As Object

        End Structure

        ''' <summary>
        ''' enum 上傳的檔案類型
        ''' </summary>
        Enum enum_UploadFunctionType

            '※ 範例，實際使用時需修改
            公告檔案

            使用者檔案
        End Enum

#End Region

        ''' <summary>
        ''' 產生DDL控制項：年+月(組)
        ''' </summary>
        ''' <param name="ddl_year"></param>
        ''' <param name="ddl_month"></param>
        ''' <remarks></remarks>
        Sub BindDropDownList_yearmonth(ddl_year As DropDownList, ddl_month As DropDownList, Optional yearBeg As Integer = 2020)
            BindDropDownList_year(ddl_year, yearBeg)
            BindDropDownList_month(ddl_month)
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：年
        ''' </summary>
        ''' <param name="ddl_year"></param>
        ''' <remarks></remarks>
        Sub BindDropDownList_year(ddl_year As DropDownList, Optional yearBeg As Integer = 2020, Optional isNeedAll As Boolean = False)
            ddl_year.Items.Clear()
            If isNeedAll = True Then
                ddl_year.Items.Add(New ListItem("*年份不拘", "%"))
            End If
            For i = yearBeg To Today.Year + 1
                ddl_year.Items.Add(New ListItem(i & "年", i))
            Next
            ddl_year.SelectedValue = Today.Year
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：月
        ''' </summary>
        ''' <param name="ddl_month"></param>
        ''' <remarks></remarks>
        Sub BindDropDownList_month(ddl_month As DropDownList, Optional isNeedAll As Boolean = False)
            ddl_month.Items.Clear()
            If isNeedAll = True Then
                ddl_month.Items.Add(New ListItem("*月份不拘", "%"))
            End If
            For i = 1 To 12
                ddl_month.Items.Add(New ListItem(CStr(i).PadLeft(2, "0") & "月", i))
            Next
            ddl_month.SelectedValue = Today.Month
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：縣市
        ''' </summary>
        ''' <param name="ddl_city"></param>
        ''' <param name="isNeedAll"></param>
        ''' <remarks></remarks>
        Sub BindDropDownList_city(ddl_city As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String = "select distinct cityID,city from System_Taiwan where cityID <= 21 order by cityID "
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using
            ddl_city.Items.Clear()
            If isNeedAll = True Then
                ddl_city.Items.Add(New ListItem("*縣市不拘", "%"))
            End If
            For Each item As Data.DataRow In dt.Rows
                ddl_city.Items.Add(New ListItem(item("city"), item("cityID")))
            Next
        End Sub

        ''' <summary>
        ''' 產生CBL控制項：縣市
        ''' </summary>
        ''' <param name="cbl_city"></param>
        Sub BindCheckBoxList_city(cbl_city As CheckBoxList, Optional isNeedNoCity As Boolean = False)
            Dim sqlString As String = "select distinct cityID,city from System_Taiwan where cityID <= 21 order by cityID "
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using
            cbl_city.Items.Clear()
            If isNeedNoCity Then
                cbl_city.Items.Add(New ListItem("無縣市", "-1"))
            End If
            For Each item As Data.DataRow In dt.Rows
                cbl_city.Items.Add(New ListItem(item("city"), item("cityID")))
            Next
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：鄉鎮
        ''' </summary>
        ''' <param name="ddl_area"></param>
        ''' <param name="cityID"></param>
        ''' <param name="isNeedAll"></param>
        ''' <remarks></remarks>
        Sub BindDropDownList_area(ddl_area As DropDownList, cityID As String, isNeedAll As Boolean)
            Dim sqlString As String = "select area,twID from System_Taiwan where cityID like @cityID order by twID "
            Dim para As New Data.SqlClient.SqlParameter("cityID", cityID)
            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para)
            End Using
            ddl_area.Items.Clear()

            If cityID = "%" Then
                ddl_area.Items.Add(New ListItem("*鄉鎮不拘", "%"))
            Else
                If isNeedAll = True Then
                    ddl_area.Items.Add(New ListItem("*鄉鎮不拘", "%"))
                End If
                For Each item As Data.DataRow In dt.Rows
                    ddl_area.Items.Add(New ListItem(item("area"), item("twID")))
                Next
            End If

        End Sub

        ''' <summary>
        ''' 產生DDL控制項：屠宰場
        ''' </summary>
        ''' <param name="ddl_slau"></param>
        ''' <param name="isNeedAll"></param>
        ''' <param name="isActive"></param>
        ''' <param name="isFullName"></param>
        ''' <param name="cityID"></param>
        Sub BindDropDownList_slauList(ddl_slau As DropDownList, isNeedAll As Boolean, Optional isActive As String = "1", Optional isFullName As Boolean = False, Optional cityID As Integer = -1)
            Dim sqlString As String =
                <xml sql="
                    select slauID, slauName, isnull(slauName_short,slauName) 'slauName_short', city, area from List_Slaughterhouse
                    left join System_Taiwan on List_Slaughterhouse.twID = System_Taiwan.twID
                    where isActive like @isActive and System_Taiwan.cityID = isNull(@cityID, System_Taiwan.cityID)
                    order by List_Slaughterhouse.twID
                "></xml>.FirstAttribute.Value
            Dim dt As New Data.DataTable
            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("isActive", isActive))
            If cityID < 1 Then
                para.Add(New Data.SqlClient.SqlParameter("cityID", DBNull.Value))
            Else
                para.Add(New Data.SqlClient.SqlParameter("cityID", cityID))
            End If
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString, para.ToArray())
            End Using

            ddl_slau.Items.Clear()

            If dt.Rows.Count > 1 Then
                ddl_slau.Items.Add(New ListItem("*屠宰場不拘", "%"))
            ElseIf dt.Rows.Count = 0 Then
                ddl_slau.Items.Add(New ListItem("轄內無屠宰場資料", -9999))
            End If

            For Each item As Data.DataRow In dt.Rows
                Select Case isFullName
                    Case True
                        ddl_slau.Items.Add(New ListItem(item("slauName"), item("slauID")))
                    Case False
                        ddl_slau.Items.Add(New ListItem(item("city") & "：" & item("slauName_short"), item("slauID")))
                End Select

            Next
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：牛隻種類
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_cattleType(ddl As DropDownList, isNeedAll As Boolean, Optional isOnlyNeedMilk As Boolean = False)
            Dim sqlString As String =
                <xml sql="
                    select cattleTypeID, typeName from System_CattleType
                    where isActive = 1 @isOnlyNeedMilk
                    order by cattleTypeID
                "></xml>.FirstAttribute.Value

            '2021/10 擴充新增乳牛處理
            Select Case isOnlyNeedMilk
                Case False
                    sqlString = sqlString.Replace("@isOnlyNeedMilk", "")
                Case True
                    sqlString = sqlString.Replace("@isOnlyNeedMilk", " and typeName like '乳%' ")
            End Select

            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*牛隻種類不拘", "%"))
            End If
            For Each item As Data.DataRow In dt.Rows
                ddl.Items.Add(New ListItem(item("typeName"), item("cattleTypeID")))
            Next
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：耳標狀態
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_tagStatus(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String =
                <xml sql="
                    select tagStatusID, tagStatusName from System_CattleStatus
                    order by tagStatusID
                "></xml>.FirstAttribute.Value

            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*耳標狀態不拘", "%"))
            End If
            For Each item As Data.DataRow In dt.Rows
                ddl.Items.Add(New ListItem(item("tagStatusName"), item("tagStatusID")))
            Next
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：縣市政府
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="cityID"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_govMain(ddl As DropDownList, cityID As String, isNeedAll As Boolean)
            Dim dt As Data.DataTable = Get_GovInfo("縣市政府", cityID)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*縣市政府不拘", "%"))
            End If
            If dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    ddl.Items.Add(New ListItem(dt.Rows(i)("govName"), dt.Rows(i)("govID") & "_" & dt.Rows(i)("cityID")))
                Next
            Else
                ddl.Items.Add(New ListItem("*無縣市政府", -1))
            End If
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：鄉鎮區公所
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="cityID"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_govSub(ddl As DropDownList, cityID As String, isNeedAll As Boolean)
            Dim dt As Data.DataTable = Get_GovInfo("鄉鎮區公所", cityID)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*鄉鎮區公所不拘", "%"))
            End If
            If dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    ddl.Items.Add(New ListItem(dt.Rows(i)("govName"), dt.Rows(i)("govID") & "_" & dt.Rows(i)("cityID")))
                Next
            Else
                ddl.Items.Add(New ListItem("*無鄉鎮區公所", -1))
            End If

        End Sub
        ''' <summary>
        ''' 產生DDL控制項：飼養狀態
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_animalStatus(ddl As DropDownList, isNeedAll As Boolean)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*飼養狀態不拘", "%"))
            End If

            ddl.Items.Add(New ListItem("一貫場"))
            ddl.Items.Add(New ListItem("架子牛場"))
            ddl.Items.Add(New ListItem("肥育牛場"))
            ddl.Items.Add(New ListItem("其他"))
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：畜牧場類型
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_farmType(ddl As DropDownList, isNeedAll As Boolean)
            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*畜牧場類型不拘", "%"))
            End If

            ddl.Items.Add(New ListItem("一般畜牧場"))
            ddl.Items.Add(New ListItem("個人畜牧場"))
        End Sub

        ''' <summary>
        ''' 產生DDL控制項：帳號權限
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_auType(ddl As DropDownList, isNeedAll As Boolean)
            Dim sqlString As String =
                <xml sql="
                    select auTypeID,auTypeName,isActive,memo
                    from System_UserAuType
                    order by auTypeID
                "></xml>.FirstAttribute.Value

            Dim dt As New Data.DataTable
            Using da As New DataAccess.MS_SQL
                dt = da.GetDataTable(sqlString)
            End Using

            ddl.Items.Clear()
            If isNeedAll = True Then
                ddl.Items.Add(New ListItem("*帳號權限不拘", "%"))
            End If
            For Each item As Data.DataRow In dt.Rows
                ddl.Items.Add(New ListItem(item("auTypeName"), item("auTypeID")))
            Next
        End Sub

        ''' <summary>
        ''' 產生 DDL 控制項：操作項目
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_UserLogItem(ddl As DropDownList, isNeedAll As Boolean)

            ddl.Items.Clear()

            ' 若需要「全部」選項
            If isNeedAll Then
                ddl.Items.Add(New ListItem("*操作項目不拘", "%"))
            End If

            ' 依照 Enum 定義順序產生項目
            For Each logType As taifCattle.Base.enum_UserLogItem In [Enum].GetValues(GetType(taifCattle.Base.enum_UserLogItem))
                ddl.Items.Add(New ListItem(logType.ToString(), CInt(logType).ToString()))
            Next

        End Sub

        ''' <summary>
        ''' 產生 DDL 控制項：操作類型
        ''' </summary>
        ''' <param name="ddl"></param>
        ''' <param name="isNeedAll"></param>
        Sub BindDropDownList_UserLogType(ddl As DropDownList, isNeedAll As Boolean)

            ddl.Items.Clear()

            ' 若需要「全部」選項
            If isNeedAll Then
                ddl.Items.Add(New ListItem("*操作類型不拘", "%"))
            End If

            ' 依照 Enum 定義順序產生項目
            For Each logType As taifCattle.Base.enum_UserLogType In [Enum].GetValues(GetType(taifCattle.Base.enum_UserLogType))
                ddl.Items.Add(New ListItem(logType.ToString(), CInt(logType).ToString()))
            Next

        End Sub


        ''' <summary>
        ''' 取得指定縣市的縣市政府／鄉鎮區公所清單
        ''' </summary>
        ''' <param name="govType"></param>
        ''' <param name="cityID"></param>
        ''' <returns></returns>
        Function Get_GovInfo(govType As String, cityID As String) As Data.DataTable
            Dim sqlString As String =
                <xml sql="
                    select * from System_GovList
                    where govType like @govType and cityID like @cityID
                    order by twID
                "></xml>.FirstAttribute.Value

            Dim para As New List(Of Data.SqlClient.SqlParameter)
            para.Add(New Data.SqlClient.SqlParameter("govType", govType))
            para.Add(New Data.SqlClient.SqlParameter("cityID", cityID))

            Using da As New DataAccess.MS_SQL
                Return da.GetDataTable(sqlString, para.ToArray)
            End Using
        End Function

        ''' <summary>
        ''' 取得指定縣市與鄉鎮資料
        ''' </summary>
        ''' <param name="cityID"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Get_CityAreaInfo(cityID As String) As Dictionary(Of Integer, List(Of stru_CityAreaInfo))
            Dim dicResult As New Dictionary(Of Integer, List(Of stru_CityAreaInfo))
            Dim sqlString As String =
                "select * from System_Taiwan where cityID < 20 and cityID like @cityID order by cityID,twID "
            Dim para As New Data.SqlClient.SqlParameter("cityID", cityID)
            Using da As New DataAccess.MS_SQL

                Dim dt As Data.DataTable = da.GetDataTable(sqlString, para)
                Dim linq_city = (From row In dt.Rows Select cID = CInt(row("cityID")), cName = CStr(row("city"))).Distinct()
                For Each itemCity In linq_city
                    Dim key As Integer = itemCity.cID
                    Dim listCityAreaInfo As New List(Of stru_CityAreaInfo)
                    Dim linq_area =
                        From row In dt.Rows
                        Select
                        twID = CInt(row("twID")), cID = CInt(row("cityID")),
                        cName = CStr(row("city")), aName = CStr(row("area"))
                        Where cID = itemCity.cID

                    For Each itemDetail In linq_area
                        Dim cityAreaInfo As New stru_CityAreaInfo
                        cityAreaInfo.twID = itemDetail.twID
                        cityAreaInfo.cityID = itemDetail.cID
                        cityAreaInfo.cityName = itemDetail.cName
                        cityAreaInfo.areaName = itemDetail.aName
                        listCityAreaInfo.Add(cityAreaInfo)
                    Next
                    dicResult.Add(key, listCityAreaInfo)
                Next
                Return dicResult
            End Using
        End Function

        ''' <summary>
        ''' 取得鄉鎮公所所屬的縣市政府GovID
        ''' </summary>
        ''' <param name="govID"></param>
        ''' <returns></returns>
        Function Get_GovMainID(govID As Integer) As Integer
            'Dim sqlString As String =
            '    "select govID from System_GovList " &
            '    "where cityID = (select cityID from System_GovList where govID = @govID) and govType = '縣市政府'"
            Dim sqlString As String = "select govMainID from View_Gov where govID = @govID"
            Dim para As New Data.SqlClient.SqlParameter("govID", govID)
            Using da As New DataAccess.MS_SQL
                Return da.ExecuteScalar(sqlString, para)
            End Using
        End Function

        ''' <summary>
        ''' 檢查檔案格式是否正確
        ''' </summary>
        ''' <param name="fu"></param>
        ''' <param name="fileMB">檔案大小限制，預設1MB</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Check_isFileFormatCorrect(ByVal fu As FileUpload, ByVal fileType As enum_fileType, Optional fileMB As Integer = 1) As taifCattle.Base.stru_checkResult
            Dim fcr As New taifCattle.Base.stru_checkResult
            Dim fileBytes As Integer = fileMB * 1024 * 1024
            fcr.isPass = False
            fcr.msg = "檔案格式不正確，或檔案大小超過 " & fileMB & " MB！"
            '檢查檔案大小
            '1048576, 1MB
            If fu.PostedFile.ContentLength > fileBytes Then
                '超過1MB
            Else
                '檢查是否為指定檔案格式
                Dim extension As String = System.IO.Path.GetExtension(fu.FileName).ToLower
                Dim allowedExtension As String() = Nothing
                Select Case fileType
                    Case enum_fileType.normalFile
                        allowedExtension = {
                            ".jpg", ".jpeg", ".gif", ".png", ".bmp",
                            ".doc", ".docx", ".odt", ".xls", ".xlsx", ".ods",
                            ".pdf", ".zip", ".rar", ".7z", ".txt"}
                    Case enum_fileType.img
                        allowedExtension = {".jpg", ".jpeg", ".gif", ".png", ".bmp"}
                    Case enum_fileType.xlsOnly
                        allowedExtension = {".xls", ".xlsx", ".ods"}
                    Case enum_fileType.docOnly
                        allowedExtension = {".doc", ".docx", ".odt"}
                    Case enum_fileType.pdfOnly
                        allowedExtension = {".pdf"}
                End Select
                If allowedExtension.Contains(extension) = True Then
                    fcr.msg = ""
                    fcr.isPass = True
                End If
            End If
            Return fcr
        End Function

        ''' <summary>
        ''' 取得上傳檔案的詳細資訊(儲存位置)
        ''' </summary>
        ''' <param name="uft"></param>
        ''' <param name="fu"></param>
        ''' <param name="accountID"></param>
        ''' <param name="sufStr">後綴字詞</param>
        ''' <returns></returns>
        Function Get_UploadInfo(fu As FileUpload, uft As enum_UploadFunctionType, accountID As Integer, Optional sufStr As String = "") As stru_UploadInfo
            Dim folder As String = ""
            Dim preStr As String = ""
            Select Case uft
                Case enum_UploadFunctionType.公告檔案
                    folder = "Document\UserFiles\"
                    preStr = accountID & "_"

                Case enum_UploadFunctionType.使用者檔案
                    folder = "Document\ISOFiles\"
                    preStr = System.IO.Path.GetFileNameWithoutExtension(fu.FileName) & "_"

            End Select

            Dim uploadInfo As New stru_UploadInfo
            If fu.HasFile Then
                uploadInfo.fileName = fu.FileName
                uploadInfo.fileExtension = System.IO.Path.GetExtension(fu.FileName).ToLower
                uploadInfo.fileName_new = preStr & Now.ToString("yyyyMMddHHmmss") & sufStr & System.IO.Path.GetExtension(fu.FileName).ToLower
                uploadInfo.filePath_physical = System.Web.HttpContext.Current.Request.PhysicalApplicationPath & folder
                uploadInfo.filePath_virtual = "~\" & folder
                If System.IO.Directory.Exists(uploadInfo.filePath_physical) = False Then
                    System.IO.Directory.CreateDirectory(uploadInfo.filePath_physical)
                End If
            Else
                uploadInfo.fileName = DBNull.Value
                uploadInfo.fileExtension = DBNull.Value
                uploadInfo.fileName_new = DBNull.Value
                uploadInfo.filePath_physical = DBNull.Value
                uploadInfo.filePath_virtual = DBNull.Value
            End If
            Return uploadInfo
        End Function

        ''' <summary>
        ''' 檢查EMAIL格式是否正確
        ''' </summary>
        ''' <param name="mailString"></param>
        ''' <returns></returns>
        Function Check_RegMail(mailString As String) As Boolean
            'https://docs.microsoft.com/zh-tw/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
            Dim emailRegString_MSDN As String =
                "^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                "(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$"
            Return Regex.IsMatch(mailString, emailRegString_MSDN, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250))
        End Function

    End Class

End Namespace