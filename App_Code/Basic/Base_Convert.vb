Namespace taifCattle

    Partial Public Class Base

        ''' <summary>
        ''' 轉換字串成為MD5加密
        ''' </summary>
        ''' <param name="md5Hash"></param>
        ''' <param name="inputStr"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert_MD5(md5Hash As System.Security.Cryptography.MD5, inputStr As String) As String
            '舊的轉碼方式，於Framework 4.5過時
            'Return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5")

            'Convert the input string to a byte array and compute the hash.
            Dim dataStr As Byte() = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(inputStr))

            'Create a new Stringbuilder to collect the bytes
            'and create a string.
            Dim sBuilder As New StringBuilder()

            'Loop through each byte of the hashed data 
            'and format each one as a hexadecimal string.
            Dim i As Integer
            For i = 0 To dataStr.Length - 1
                sBuilder.Append(dataStr(i).ToString("x2"))
            Next i

            'Return the hexadecimal string.
            Return sBuilder.ToString()
        End Function

        ''' <summary>
        ''' 轉換空值變成指定物件(DBNULL／字串／數值...etc)
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        Function Convert_EmptyToObject(input As String, output As Object) As Object
            Select Case String.IsNullOrEmpty(input)
                Case True
                    Return output
                Case False
                    Return input
            End Select
        End Function

        ''' <summary>
        ''' 轉換DBNULL變成指定／預設文字
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert_DBNullToString(input As Object, Optional output As String = "") As String
            Select Case IsDBNull(input)
                Case True
                    Return output
                Case False
                    Return input
            End Select
        End Function

        ''' <summary>
        ''' 轉換DBNULL變成物件
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert_DBNullToObject(input As Object, output As Object) As Object
            Select Case IsDBNull(input)
                Case True
                    Return output
                Case False
                    Return input
            End Select
            Return Nothing
        End Function

        ''' <summary>
        ''' 轉換日期欄位的DBNULL變成指定格式
        ''' </summary>
        ''' <param name="input"></param>
        ''' <param name="formatStr"></param>
        ''' <param name="output"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Convert_DateNullToString(input As Object, output As Object, Optional formatStr As String = "yyyy/MM/dd") As String
            Select Case IsDBNull(input)
                Case True
                    Return output
                Case False
                    Return CType(input, DateTime).ToString(formatStr)
            End Select
            Return Nothing
        End Function

        ''' <summary>
        ''' 轉換值成指定的Enum型別
        ''' </summary>
        Function Convert_ValueToEnum(enumType As Type, value As Object) As [Enum]
            If [Enum].IsDefined(enumType, value) Then
                Return CType([Enum].Parse(enumType, value), [Enum])
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace
