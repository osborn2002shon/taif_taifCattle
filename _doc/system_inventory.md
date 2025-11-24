# 系統總覽文件

本文件彙整專案的軟體材料清單（SBOM）、主要程式目錄與檔案清冊，以及資料庫 Schema 重點，並提供中文說明以利追蹤。

## CSV 版本匯出

| CSV 檔案 | 內容摘要 |
| --- | --- |
| `_doc/sbom.csv` | NuGet 套件、版本與中文用途。 |
| `_doc/directories.csv` | 主要目錄與中文說明。 |
| `_doc/system_programs.csv` | 系統程式與 ASPX 頁面清冊（含中文）。 |
| `_doc/schema_full.csv` | Schema 全欄位（函式、資料表欄位、型態、中文描述）。 |

以下仍保留 Markdown 表格以供快速瀏覽，CSV 可直接下載開啟或匯入其他系統。

## SBOM 表

| 套件 (Package) | 版本 | 主要用途（中文） |
| --- | --- | --- |
| BouncyCastle.Cryptography | 2.4.0 | 密碼學演算法與金鑰處理功能庫。 |
| Enums.NET | 5.0.0 | 提供列舉型別的擴充操作與格式化。 |
| ExtendedNumerics.BigDecimal | 2025.1001.2.129 | 提供高精度十進位數值運算。 |
| MathNet.Numerics.Signed | 5.0.0 | 數值分析與統計運算工具。 |
| Microsoft.CodeDom.Providers.DotNetCompilerPlatform | 2.0.1 | ASP.NET 編譯器提供者，支援 Roslyn。 |
| Microsoft.IO.RecyclableMemoryStream | 3.0.1 | 針對記憶體串流的再利用以減少 GC 壓力。 |
| Newtonsoft.Json | 13.0.4 | JSON 解析與序列化。 |
| NPOI | 2.7.5 | Excel 讀寫與匯出。 |
| NSax | 1.0.2 | 流式 XML 解析工具。 |
| SharpZipLib | 1.4.2 | ZIP 等壓縮格式處理。 |
| SixLabors.Fonts | 1.0.1 | 字型載入與測量。 |
| SixLabors.ImageSharp | 2.1.11 | 影像處理與轉檔。 |
| System.Buffers | 4.5.1 | 緩衝區池化與配置工具。 |
| System.Memory | 4.5.5 | Span/Memory 型別支援。 |
| System.Numerics.Vectors | 4.5.0 | 向量化數值運算基礎型別。 |
| System.Runtime.CompilerServices.Unsafe | 6.0.0 | 低階非安全型別操作支援。 |
| System.Security.Cryptography.Xml | 8.0.2 | XML 簽章與加解密。 |
| System.Text.Encoding.CodePages | 5.0.0 | 額外的編碼頁面支援。 |
| System.Threading.Tasks.Extensions | 4.5.2 | 非同步程式設計的擴充型別。 |
| ZString | 2.6.0 | 高效能字串組裝。 |

> 來源：`packages.config`。

## 系統程式目錄（含中文翻譯）

| 目錄 | 說明（中文） |
| --- | --- |
| `/App_Code` | 共用程式碼庫，包含基礎類別、資料存取與業務邏輯。 |
| `/App_Code/Basic` | 基礎工具（帳號、資料庫存取、郵件、報表、Excel 匯出等）。 |
| `/pages` | 網站的主要功能頁面與管理介面。 |
| `/_css`, `/_js`, `/_fonts`, `/_img` | 靜態資源（樣式、腳本、字型、圖片）。 |
| `/_uc` | User Control 元件。 |
| `_doc` | 文件與匯入範本。 |
| `Web.config`, `Web.Debug.config`, `Web.Release.config` | 站台設定與環境組態。 |

## 系統程式清冊（含中文翻譯）

| 檔案/區段 | 說明（中文） |
| --- | --- |
| `App_Code/Basic/Account.vb` | 帳號權限與驗證相關工具。 |
| `App_Code/Basic/Base.vb`、`Base_Convert.vb` | 共用基底類別與型別轉換輔助。 |
| `App_Code/Basic/Control.vb` | Web 控制項輔助方法。 |
| `App_Code/Basic/DataAccess_MSSQL.vb` | MSSQL 資料存取封裝。 |
| `App_Code/Basic/ExcelExporter.vb` | 匯出 Excel 的通用工具（NPOI 依賴）。 |
| `App_Code/Basic/Mail.vb` | 郵件傳送工具。 |
| `App_Code/Basic/Report.vb` | 報表產製輔助。 |
| `App_Code/Cattle.vb` | 牛隻資料處理與核心業務邏輯。 |
| `App_Code/Cattle_Control.vb` | 與牛隻控制流程相關的輔助模組。 |
| `App_Code/Farm.vb` | 牧場資料處理邏輯。 |
| `App_Code/WebService.vb` | WebService 端點及相關封裝。 |
| `CPW.aspx` / `CPW.aspx.vb` | 變更密碼頁面與後置程式。 |
| `FPW.aspx` / `FPW.aspx.vb` | 忘記密碼頁面與後置程式。 |
| `Login.aspx` / `Login.aspx.vb` | 登入頁面與後置程式。 |
| `WebService/CattleCulling.aspx` | Web Service 介面：牛隻淘汰（對應 code-behind）。 |
| `WebService/CattleHistory.aspx` | Web Service 介面：牛隻歷程查詢。 |
| `WebService/CattleUninsured.aspx` | Web Service 介面：未投保牛隻查詢。 |
| `WebService/Verify.aspx` | Web Service 介面：驗證服務。 |
| `pages/Data/CattleManage.aspx` | 資料維護：牛隻管理列表。 |
| `pages/Data/CattleManage_Detail.aspx` | 資料維護：牛隻明細編輯。 |
| `pages/Data/CattleManage_Batch.aspx` | 資料維護：牛隻批次處理。 |
| `pages/Data/FarmManage.aspx` | 資料維護：牧場管理列表。 |
| `pages/Data/FarmManage_Batch.aspx` | 資料維護：牧場批次處理。 |
| `pages/Data/HisManage_Batch.aspx` | 資料維護：歷程批次匯入。 |
| `pages/Data/HisEndManage_Batch.aspx` | 資料維護：歷程結案批次匯入。 |
| `pages/Analysis/StaticsCattle.aspx` | 分析報表：牛隻統計。 |
| `pages/Analysis/StaticsFarm.aspx` | 分析報表：牧場統計。 |
| `pages/Analysis/StaticsCityCattle.aspx` | 分析報表：縣市別牛隻統計。 |
| `pages/Analysis/StaticsNationCattle.aspx` | 分析報表：全國牛隻統計。 |
| `pages/System/AccountManage.aspx` | 系統管理：帳號管理。 |
| `pages/System/MyAccount.aspx` | 系統管理：我的帳號設定。 |
| `pages/System/WebServiceLog.aspx` | 系統管理：Web Service 呼叫紀錄。 |
| `pages/System/Permission.aspx` | 系統管理：權限設定。 |
| `pages/System/Config.aspx` | 系統管理：系統組態設定。 |
| `pages/System/MilkSetting.aspx` | 系統管理：乳量與相關設定。 |
| `pages/System/UserLog.aspx` | 系統管理：使用者操作紀錄。 |
| `pages/sample/sample.aspx` | 範例頁面。 |
| `WebService/` | 供外部系統呼叫的 Web Service 介面。 |
| `_doc/batch/*` | 批次匯入的 Excel 範本與說明檔。 |

## Schema 全欄位明細（含中文翻譯）

以下將 `SQL/schema.sql` 的物件逐一列出所有欄位、型態與中文解釋，便於對照版本與後續維護。

### 函式

| 物件 | 說明（中文） |
| --- | --- |
| `makeSecret(plainText)` | 以 PassPhrase `TaIFT` 將文字加密並回傳 VARBINARY。 |
| `showSecret(cipher)` | 以同 PassPhrase 解密 VARBINARY 並回傳 NVARCHAR。 |

### 資料表

#### Cattle_List（牛隻基本資料）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| cattleID | int, identity | 主鍵，牛隻流水號。 |
| cattleTypeID | int | 牛隻類型（對應 Cattle_TypeCattle）。 |
| tagNo | nvarchar(50) | 耳標號碼。 |
| tagMemo | nvarchar(max) | 耳標備註。 |
| birthYear | int | 出生年（民國/西元依系統慣例）。 |
| cattleMemo | nvarchar(max) | 其他牛隻備註。 |
| insertType | nvarchar(max) | 建立來源描述。 |
| insertDateTime | datetime | 建立時間。 |
| insertAccountID | int | 建立人員帳號 ID。 |
| updateDateTime | datetime | 最近更新時間。 |
| updateAccountID | int | 最近更新人員帳號 ID。 |
| removeDateTime | datetime | 下架時間（NULL 表示仍在用）。 |
| removeAccountID | int | 下架人員帳號 ID。 |

#### Cattle_History（牛隻歷程）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| hisID | int, identity | 主鍵，歷程流水號。 |
| cattleID | int | 對應牛隻主檔。 |
| hisTypeID | int | 歷程類型（對應 Cattle_TypeHistory）。 |
| dataDate | date | 事件日期。 |
| farmID | int | 牧場 ID（可為 NULL）。 |
| plantID | int | 化製場 ID（可為 NULL）。 |
| slauID | int | 屠宰場 ID（可為 NULL）。 |
| memo | nvarchar(max) | 歷程備註。 |
| insertType | nvarchar(max) | 建立來源描述。 |
| insertDateTime | datetime | 建立時間。 |
| insertAccountID | int | 建立人員帳號 ID。 |
| removeDateTime | datetime | 下架時間。 |
| removeAccountID | int | 下架人員帳號 ID。 |

#### Cattle_TypeCattle（牛隻類型）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| cattleTypeID | int, identity | 主鍵，牛隻類型 ID。 |
| groupOrder | int | 類型群組排序。 |
| groupName | nvarchar(50) | 群組名稱。 |
| typeName | nvarchar(50) | 類型名稱。 |
| isActive | bit | 是否啟用。 |

#### Cattle_TypeHistory（歷程類型）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| hisTypeID | int, identity | 主鍵，歷程類型 ID。 |
| groupName | nvarchar(50) | 群組名稱。 |
| typeName | nvarchar(50) | 類型名稱。 |
| memo | nvarchar(max) | 備註。 |

#### Data_DataExchangeLog（資料交換紀錄）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| logID | int, identity | 主鍵，交換紀錄 ID。 |
| logDateTime | datetime | 記錄時間。 |
| dataSourceName | nvarchar(max) | 資料來源名稱。 |
| apiName | nvarchar(max) | API 名稱。 |
| dataCount | int | 資料筆數。 |
| triggerType | nvarchar(50) | 觸發方式（排程/手動）。 |
| queryMemo | nvarchar(max) | 查詢條件/摘要。 |
| isError | bit | 是否發生錯誤。 |
| errorString | nvarchar(max) | 錯誤訊息。 |
| ip | nvarchar(max) | 呼叫來源 IP。 |

#### Data_FarmMissingImport（缺漏牧場匯入）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| missingID | int, identity | 主鍵，缺漏紀錄 ID。 |
| dataSource | nvarchar(50) | 資料來源代碼。 |
| serialNo | nvarchar(64) | 外部序號。 |
| farmCode | nvarchar(50) | 牧場代碼。 |
| insertDateTime | datetime, default getdate() | 建立時間。 |

#### Data_Moa_Farm（MOA 牧場資料）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| autoID | int, identity | 主鍵，自動編號。 |
| FarmID | nvarchar(max) | MOA 牧場代碼。 |
| FarmName | nvarchar(max) | 牧場名稱。 |
| FarmCounty | nvarchar(max) | 縣市。 |
| FarmDist | nvarchar(max) | 行政區。 |
| FarmAddress | nvarchar(max) | 地址。 |
| Farmer | nvarchar(max) | 飼養戶姓名。 |
| AnimalDataString | nvarchar(max) | 動物數量描述。 |
| isEmpty | nvarchar(max) | 是否為空白資料標記。 |
| insertDateTime | datetime | 建立時間。 |
| updateType | nvarchar(50) | 更新類型。 |
| updateFarmAutoID | int | 來源/被取代資料的 autoID。 |
| updateDateTime | datetime | 更新時間。 |
| isListData | bit | 是否來自清單資料。 |
| isUpdateDone | bit | 是否已更新完成。 |

#### Data_Moa_FarmLog（MOA 匯入紀錄）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| logID | int, identity | 主鍵，匯入紀錄 ID。 |
| event | nvarchar(50) | 事件名稱。 |
| type | nvarchar(50) | 類型（例如 import/update）。 |
| interfaceDate | date | 介接日期。 |
| interfaceFarmID | nvarchar(50) | 介接牧場代碼。 |
| dataCount | int | 資料筆數。 |
| insertDateTime | datetime | 建立時間。 |

#### Data_Naif_EarTag（NAIF 耳標核發）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| dataID | int, identity | 主鍵，耳標資料 ID。 |
| issuanceDate | date | 核發日期。 |
| earTagNo | nvarchar(50) | 耳標號碼。 |
| govName | nvarchar(50) | 核發單位。 |
| farmCode | nvarchar(50) | 農場代碼。 |
| farmName | nvarchar(max) | 農場名稱。 |
| owner | nvarchar(max) | 飼養戶。 |
| ownerTel | nvarchar(max) | 飼養戶電話。 |
| city | nvarchar(50) | 縣市。 |
| area | nvarchar(50) | 行政區。 |
| address | nvarchar(max) | 地址。 |
| insertDateTime | datetime | 建立時間。 |
| convertDateTime | datetime | 轉入系統時間。 |

#### Data_Naif_SlauData（NAIF 屠宰資料）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| dataID | int, identity | 主鍵，屠宰資料 ID。 |
| slauID | int | 屠宰場代碼。 |
| slauName | nvarchar(max) | 屠宰場名稱。 |
| slauDate | date | 屠宰日期。 |
| typeName | nvarchar(50) | 牛隻類型。 |
| tagNo | nvarchar(50) | 耳標號碼。 |
| brandNo | nvarchar(50) | 烙印號。 |
| slauTraceCode | nvarchar(50) | 屠宰追溯碼。 |
| dataStatus | nvarchar(50) | 資料狀態。 |
| jouDate | date | 歷程日期。 |
| farmCode | nvarchar(50) | 來源牧場代碼。 |
| farmName | nvarchar(max) | 來源牧場名稱。 |
| owner | nvarchar(max) | 飼養戶。 |
| ownerTel | nvarchar(max) | 飼養戶電話。 |
| city | nvarchar(50) | 縣市。 |
| area | nvarchar(50) | 行政區。 |
| address | nvarchar(max) | 地址。 |
| insertDateTime | datetime | 建立時間。 |
| convertDateTime | datetime | 轉入系統時間。 |

#### List_Farm（牧場清單）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| farmID | int, identity | 主鍵，牧場 ID。 |
| farmName | nvarchar(max) | 牧場名稱。 |
| farmCode | nvarchar(max) | 牧場代碼。 |
| owner | nvarchar(max) | 飼養戶。 |
| ownerID | nvarchar(max) | 飼養戶身分證/統編。 |
| ownerTel | nvarchar(max) | 飼養戶電話。 |
| twID | int | 所在行政區（對應 System_Taiwan）。 |
| address | nvarchar(max) | 地址。 |
| animalCount | nvarchar(max) | 牧場動物數量描述。 |
| memo | nvarchar(max) | 備註。 |
| insertType | nvarchar(max) | 建立來源描述。 |
| insertDateTime | datetime | 建立時間。 |
| insertAccountID | int | 建立人員帳號 ID。 |
| updateDateTime | datetime | 最近更新時間。 |
| updateAccountID | int | 最近更新人員帳號 ID。 |
| removeDateTime | datetime | 下架時間。 |
| removeAccount | int | 下架人員帳號 ID。 |

#### List_RenderingPlant（化製場清單）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| plantID | int, identity | 主鍵，化製場 ID。 |
| plantCode | nvarchar(50) | 化製場代碼。 |
| plantName | nvarchar(max) | 化製場名稱。 |
| owner | nvarchar(max) | 負責人。 |
| twID | int | 行政區 ID。 |
| address | nvarchar(max) | 地址。 |
| isActive | bit | 是否啟用。 |
| memo | nvarchar(max) | 備註。 |

#### List_Slaughterhouse（屠宰場清單）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| slauID | int, identity | 主鍵，屠宰場 ID。 |
| baphiqCode | nvarchar(50) | 防檢局代碼。 |
| slauName | nvarchar(max) | 屠宰場全名。 |
| slauName_short | nvarchar(max) | 屠宰場簡稱。 |
| twID | int | 行政區 ID。 |
| address | nvarchar(max) | 地址。 |
| ownerName | nvarchar(max) | 業者名稱。 |
| ownerTel | nvarchar(max) | 業者電話。 |
| contactName | nvarchar(max) | 聯絡人。 |
| contactTel | nvarchar(max) | 聯絡電話。 |
| contactMobile | nvarchar(max) | 聯絡手機。 |
| fax | nvarchar(max) | 傳真。 |
| isActive | bit | 是否啟用。 |
| memo | nvarchar(max) | 備註。 |

#### System_Config（系統設定）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| configID | int, identity | 主鍵，設定 ID。 |
| passwordMinLength | int | 密碼最小長度。 |
| requireUppercase | bit | 是否要求大寫。 |
| requireLowercase | bit | 是否要求小寫。 |
| requireNumbers | bit | 是否要求數字。 |
| requireSymbols | bit | 是否要求符號。 |
| passwordMaxAge | int | 密碼最長有效天數。 |
| passwordHistoryCount | int | 密碼歷史不得重複次數。 |
| passwordMinAge | int | 密碼最短使用天數。 |
| maxFailAttempts | int | 最多失敗登入次數。 |
| lockoutDuration | int | 鎖定時長（分鐘）。 |
| updateDateTime | datetime | 更新時間。 |
| updateUserID | int | 更新人員帳號 ID。 |

#### System_Menu（功能選單）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| menuID | int, identity | 主鍵，選單 ID。 |
| groupName | nvarchar(50) | 選單群組名稱。 |
| menuName | nvarchar(50) | 選單項目名稱。 |
| menuURL | nvarchar(max) | 目標 URL。 |
| iconClass | nvarchar(50) | 圖示樣式（CSS class）。 |
| orderBy_group | int | 群組排序。 |
| orderBy_menu | int | 群組內排序。 |
| isActive | bit | 是否啟用。 |
| isShow | bit | 是否顯示。 |
| memo | nvarchar(max) | 備註。 |

#### System_MenuAu（選單授權對應）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| auTypeID | int | 授權類型 ID（對應 System_UserAuType）。 |
| menuID | int | 選單 ID（對應 System_Menu）。 |

#### System_MilkSetting（乳量設定）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| age | int | 乳量年齡（月/週齡，依系統慣例）。 |
| milkProduction | decimal(4,1) | 對應年齡的乳量。 |
| remark | nvarchar(max) | 備註。 |
| updateDateTime | datetime | 更新時間。 |
| updateUserID | int | 更新人員帳號 ID。 |

#### System_Taiwan（行政區）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| twID | int, identity | 主鍵，行政區 ID。 |
| cityID | int | 縣市代碼。 |
| city | nvarchar(50) | 縣市名稱。 |
| area | nvarchar(50) | 鄉鎮市區名稱。 |

#### System_UserAccount（使用者帳號）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| accountID | int, identity | 主鍵，帳號 ID。 |
| auTypeID | int | 授權類型。 |
| slauID | int | 所屬屠宰場 ID（可空）。 |
| govID | int | 主管機關 ID（可空）。 |
| farmID | int | 所屬牧場 ID（可空）。 |
| account | nvarchar(50) | 帳號名稱。 |
| password | nvarchar(max) | 密碼（加密後存放）。 |
| name | nvarchar(50) | 使用者姓名。 |
| email | nvarchar(max) | 電子郵件。 |
| unit | nvarchar(max) | 單位/部門。 |
| mobile | nvarchar(max) | 行動電話。 |
| memo | nvarchar(max) | 備註。 |
| isActive | bit | 是否啟用。 |
| isEmailVerified | bit | 電子郵件是否驗證。 |
| emailVerifiedDateTime | datetime | 驗證時間。 |
| lastLoginDateTime | datetime | 最近登入時間。 |
| insertDateTime | datetime | 建立時間。 |
| insertAccountID | int | 建立人員帳號 ID。 |
| updateDateTime | datetime | 最近更新時間。 |
| updateAccountID | int | 最近更新人員帳號 ID。 |
| removeDateTime | datetime | 停用時間。 |
| removeAccountID | int | 停用人員帳號 ID。 |
| lastUpdatePWDateTime | datetime | 最近更改密碼時間。 |

#### System_UserAuType（授權類型）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| auTypeID | int, identity | 主鍵，授權類型 ID。 |
| auTypeName | nvarchar(50) | 授權類型名稱。 |
| isActive | bit | 是否啟用。 |
| memo | nvarchar(max) | 備註。 |

#### System_UserCPWLog（變更密碼紀錄）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| cpwID | int, identity | 主鍵，變更紀錄 ID。 |
| accountID | int | 目標帳號 ID。 |
| exAccountID | int | 操作者帳號 ID。 |
| pwdHash | varchar(64) | 更改後的密碼雜湊。 |
| changeType | nvarchar(50) | 變更類型（自助/管理者）。 |
| changeDateTime | datetime | 變更時間。 |

#### System_UserFPWLog（忘記密碼紀錄）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| fpwID | int, identity | 主鍵，忘記密碼請求 ID。 |
| accountID | int | 目標帳號 ID。 |
| email | nvarchar(200) | 收件者 Email。 |
| resetToken | nvarchar(max) | 重設 token。 |
| requestTime | datetime | 請求時間。 |
| changeTime | datetime | 完成重設時間。 |
| isUsed | bit | 是否已使用。 |

#### System_UserLog（登入/操作紀錄）

| 欄位 | 型態 | 說明（中文） |
| --- | --- | --- |
| logID | int, identity | 主鍵，紀錄 ID。 |
| accountID | int | 相關帳號 ID。 |
| logDateTime | datetime | 記錄時間。 |
| IP | nvarchar(max) | 操作來源 IP。 |
| logItem | nvarchar(50) | 事件項目（如 Login）。 |
| logType | nvarchar(50) | 事件類型（如 Success/Fail）。 |
| memo | nvarchar(max) | 詳細描述。 |

> 如 Schema 更新，請同步於此表補充欄位與中文描述。
