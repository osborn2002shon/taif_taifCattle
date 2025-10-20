USE [master]
GO
/****** Object:  Database [taif_taifCattle]    Script Date: 2025/10/20 下午 12:22:50 ******/
CREATE DATABASE [taif_taifCattle]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'taif_taifCattle', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\taif_taifCattle.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'taif_taifCattle_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\taif_taifCattle_log.ldf' , SIZE = 2304KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [taif_taifCattle] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [taif_taifCattle].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [taif_taifCattle] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [taif_taifCattle] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [taif_taifCattle] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [taif_taifCattle] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [taif_taifCattle] SET ARITHABORT OFF 
GO
ALTER DATABASE [taif_taifCattle] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [taif_taifCattle] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [taif_taifCattle] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [taif_taifCattle] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [taif_taifCattle] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [taif_taifCattle] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [taif_taifCattle] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [taif_taifCattle] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [taif_taifCattle] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [taif_taifCattle] SET  DISABLE_BROKER 
GO
ALTER DATABASE [taif_taifCattle] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [taif_taifCattle] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [taif_taifCattle] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [taif_taifCattle] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [taif_taifCattle] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [taif_taifCattle] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [taif_taifCattle] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [taif_taifCattle] SET RECOVERY FULL 
GO
ALTER DATABASE [taif_taifCattle] SET  MULTI_USER 
GO
ALTER DATABASE [taif_taifCattle] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [taif_taifCattle] SET DB_CHAINING OFF 
GO
ALTER DATABASE [taif_taifCattle] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [taif_taifCattle] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [taif_taifCattle]
GO
/****** Object:  Table [dbo].[Cattle_History]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cattle_History](
	[hisID] [int] IDENTITY(1,1) NOT NULL,
	[cattleID] [int] NOT NULL,
	[hisTypeID] [int] NOT NULL,
	[dataDate] [date] NOT NULL,
	[farmID] [int] NULL,
	[plantID] [int] NULL,
	[slauID] [int] NULL,
	[memo] [nvarchar](max) NULL,
	[insertType] [nvarchar](max) NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Cattle_Journey] PRIMARY KEY CLUSTERED 
(
	[hisID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cattle_List]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cattle_List](
	[cattleID] [int] IDENTITY(1,1) NOT NULL,
	[cattleTypeID] [int] NOT NULL,
	[tagNo] [nvarchar](50) NOT NULL,
	[tagMemo] [nvarchar](max) NULL,
	[birthYear] [int] NULL,
	[cattleMemo] [nvarchar](max) NULL,
	[insertType] [nvarchar](max) NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NOT NULL,
	[updateAccountID] [int] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
 CONSTRAINT [PK_Cattle_List] PRIMARY KEY CLUSTERED 
(
	[cattleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cattle_TypeCattle]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cattle_TypeCattle](
	[cattleTypeID] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NOT NULL,
	[typeName] [nvarchar](50) NOT NULL,
	[isActive] [bit] NOT NULL,
 CONSTRAINT [PK_Cattle_Type_1] PRIMARY KEY CLUSTERED 
(
	[cattleTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cattle_TypeHistory]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cattle_TypeHistory](
	[hisTypeID] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NOT NULL,
	[typeName] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_Cattle_TypeHistory] PRIMARY KEY CLUSTERED 
(
	[hisTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Data_DataExchangeLog]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data_DataExchangeLog](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[logDateTime] [datetime] NOT NULL,
	[dataSourceName] [nvarchar](max) NOT NULL,
	[apiName] [nvarchar](max) NOT NULL,
	[dataCount] [int] NOT NULL,
	[triggerType] [nvarchar](50) NOT NULL,
	[queryMemo] [nvarchar](max) NULL,
	[isError] [bit] NOT NULL,
	[errorString] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_DataExchangeLog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Data_Moa_Farm]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data_Moa_Farm](
	[autoID] [int] IDENTITY(1,1) NOT NULL,
	[FarmID] [nvarchar](max) NULL,
	[FarmName] [nvarchar](max) NULL,
	[FarmCounty] [nvarchar](max) NULL,
	[FarmDist] [nvarchar](max) NULL,
	[FarmAddress] [nvarchar](max) NULL,
	[Farmer] [nvarchar](max) NULL,
	[AnimalDataString] [nvarchar](max) NULL,
	[isEmpty] [nvarchar](max) NULL,
	[insertDateTime] [datetime] NOT NULL,
	[updateType] [nvarchar](50) NULL,
	[updateFarmAutoID] [int] NULL,
	[updateDateTime] [datetime] NULL,
	[isListData] [bit] NOT NULL,
	[isUpdateDone] [bit] NOT NULL,
 CONSTRAINT [PK_List_Farm_Update2] PRIMARY KEY CLUSTERED 
(
	[autoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Data_Moa_FarmLog]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data_Moa_FarmLog](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[event] [nvarchar](50) NOT NULL,
	[type] [nvarchar](50) NOT NULL,
	[interfaceDate] [date] NULL,
	[interfaceFarmID] [nvarchar](50) NULL,
	[dataCount] [int] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_List_Farm_UpdateLog2] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Data_Naif_EarTag]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data_Naif_EarTag](
	[dataID] [int] IDENTITY(1,1) NOT NULL,
	[issuanceDate] [date] NOT NULL,
	[earTagNo] [nvarchar](50) NOT NULL,
	[govName] [nvarchar](50) NULL,
	[farmCode] [nvarchar](50) NULL,
	[farmName] [nvarchar](max) NULL,
	[owner] [nvarchar](max) NULL,
	[ownerTel] [nvarchar](max) NULL,
	[city] [nvarchar](50) NULL,
	[area] [nvarchar](50) NULL,
	[address] [nvarchar](max) NULL,
	[insertDateTime] [datetime] NOT NULL,
	[convertDateTime] [datetime] NULL,
 CONSTRAINT [PK__Data_Nai__923E36856AB21CF4] PRIMARY KEY CLUSTERED 
(
	[dataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Data_Naif_SlauData]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data_Naif_SlauData](
	[dataID] [int] IDENTITY(1,1) NOT NULL,
	[slauID] [int] NOT NULL,
	[slauName] [nvarchar](max) NOT NULL,
	[slauDate] [date] NOT NULL,
	[typeName] [nvarchar](50) NULL,
	[tagNo] [nvarchar](50) NOT NULL,
	[brandNo] [nvarchar](50) NULL,
	[slauTraceCode] [nvarchar](50) NULL,
	[dataStatus] [nvarchar](50) NOT NULL,
	[jouDate] [date] NOT NULL,
	[farmCode] [nvarchar](50) NULL,
	[farmName] [nvarchar](max) NULL,
	[owner] [nvarchar](max) NULL,
	[ownerTel] [nvarchar](max) NULL,
	[city] [nvarchar](50) NULL,
	[area] [nvarchar](50) NULL,
	[address] [nvarchar](max) NULL,
	[insertDateTime] [datetime] NULL,
	[convertDateTime] [datetime] NULL,
 CONSTRAINT [PK_Data_Naif_SlauData] PRIMARY KEY CLUSTERED 
(
	[dataID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[List_Farm]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[List_Farm](
	[farmID] [int] IDENTITY(1,1) NOT NULL,
	[farmName] [nvarchar](max) NOT NULL,
	[farmCode] [nvarchar](max) NOT NULL,
	[owner] [nvarchar](max) NOT NULL,
	[ownerID] [nvarchar](max) NULL,
	[ownerTel] [nvarchar](max) NULL,
	[twID] [int] NOT NULL,
	[address] [nvarchar](max) NOT NULL,
	[animalCount] [nvarchar](max) NULL,
	[memo] [nvarchar](max) NULL,
	[insertType] [nvarchar](max) NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NOT NULL,
	[updateAccountID] [int] NOT NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccount] [int] NULL,
 CONSTRAINT [PK_List_Farm] PRIMARY KEY CLUSTERED 
(
	[farmID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[List_RenderingPlant]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[List_RenderingPlant](
	[plantID] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](50) NOT NULL,
	[owner] [nvarchar](max) NOT NULL,
	[name] [nvarchar](max) NOT NULL,
	[twID] [int] NOT NULL,
	[address] [nvarchar](max) NOT NULL,
	[isActive] [bit] NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_List_RenderingPlant] PRIMARY KEY CLUSTERED 
(
	[plantID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[List_Slaughterhouse]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[List_Slaughterhouse](
	[slauID] [int] IDENTITY(1,1) NOT NULL,
	[baphiqCode] [nvarchar](50) NULL,
	[slauName] [nvarchar](max) NOT NULL,
	[slauName_short] [nvarchar](max) NOT NULL,
	[twID] [int] NOT NULL,
	[address] [nvarchar](max) NULL,
	[ownerName] [nvarchar](max) NOT NULL,
	[ownerTel] [nvarchar](max) NOT NULL,
	[contactName] [nvarchar](max) NOT NULL,
	[contactTel] [nvarchar](max) NULL,
	[contactMobile] [nvarchar](max) NULL,
	[fax] [nvarchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_sys_butcherList] PRIMARY KEY CLUSTERED 
(
	[slauID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Menu]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Menu](
	[menuID] [int] IDENTITY(1,1) NOT NULL,
	[groupName] [nvarchar](50) NOT NULL,
	[menuName] [nvarchar](50) NOT NULL,
	[menuURL] [nvarchar](max) NOT NULL,
	[iconClass] [nvarchar](50) NULL,
	[orderBy_group] [int] NOT NULL,
	[orderBy_menu] [int] NOT NULL,
	[isActive] [bit] NOT NULL,
	[isShow] [bit] NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_System_Menu] PRIMARY KEY CLUSTERED 
(
	[menuID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_MenuAu]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_MenuAu](
	[auTypeID] [int] NOT NULL,
	[menuID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_MilkSetting]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_MilkSetting](
	[age] [int] NOT NULL,
	[milkProduction] [decimal](4, 1) NOT NULL,
	[remark] [nvarchar](max) NULL,
	[updateDateTime] [datetime] NOT NULL,
	[updateUserID] [int] NOT NULL,
 CONSTRAINT [PK_System_MilkSetting] PRIMARY KEY CLUSTERED 
(
	[age] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_Taiwan]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_Taiwan](
	[twID] [int] IDENTITY(1,1) NOT NULL,
	[cityID] [int] NOT NULL,
	[city] [nvarchar](50) NOT NULL,
	[area] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_System_Taiwan] PRIMARY KEY CLUSTERED 
(
	[twID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAccount]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserAccount](
	[accountID] [int] IDENTITY(1,1) NOT NULL,
	[auTypeID] [int] NOT NULL,
	[slauID] [int] NULL,
	[govID] [int] NULL,
	[farmID] [int] NULL,
	[account] [nvarchar](50) NOT NULL,
	[password] [nvarchar](max) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[email] [nvarchar](max) NULL,
	[unit] [nvarchar](max) NULL,
	[mobile] [nvarchar](max) NULL,
	[memo] [nvarchar](max) NULL,
	[isActive] [bit] NOT NULL,
	[insertDateTime] [datetime] NOT NULL,
	[insertAccountID] [int] NOT NULL,
	[updateDateTime] [datetime] NULL,
	[updateAccountID] [int] NULL,
	[removeDateTime] [datetime] NULL,
	[removeAccountID] [int] NULL,
	[lastUpdatePWDateTime] [datetime] NULL,
 CONSTRAINT [PK_System_UserAccountInfo] PRIMARY KEY CLUSTERED 
(
	[accountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserAuType]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserAuType](
	[auTypeID] [int] IDENTITY(1,1) NOT NULL,
	[auTypeName] [nvarchar](50) NOT NULL,
	[isActive] [bit] NOT NULL,
	[memo] [nvarchar](max) NULL,
 CONSTRAINT [PK_Sys_AuType] PRIMARY KEY CLUSTERED 
(
	[auTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserFPWLog]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserFPWLog](
	[fpwID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[email] [nvarchar](200) NULL,
	[resetToken] [nvarchar](max) NOT NULL,
	[requestTime] [datetime] NOT NULL,
	[changeTime] [datetime] NULL,
	[isUsed] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[fpwID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[System_UserLog]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[System_UserLog](
	[logID] [int] IDENTITY(1,1) NOT NULL,
	[accountID] [int] NOT NULL,
	[logDateTime] [datetime] NULL,
	[IP] [nvarchar](max) NOT NULL,
	[logItem] [nvarchar](50) NOT NULL,
	[logType] [nvarchar](50) NOT NULL,
	[memo] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_System_UserLog] PRIMARY KEY CLUSTERED 
(
	[logID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[_View_CattleHistory_Stay]    Script Date: 2025/10/20 下午 12:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_View_CattleHistory_Stay]
AS
	select
		Cattle_History.hisID, 
		Cattle_History.cattleID, 
		Cattle_History.hisTypeID, 
		Cattle_TypeHistory.groupName,
		Cattle_TypeHistory.typeName,
		Cattle_History.dataDate, 
		Cattle_History.farmID, 
		List_Farm.farmName, 
		List_Farm.farmCode, 
		List_Farm.owner, 
		List_Farm.ownerID, 
		List_Farm.ownerTel, 
		List_Farm.twID, 
		System_Taiwan.city,
		System_Taiwan.area,
		List_Farm.address, 
		Cattle_History.memo, 
		Cattle_History.insertType, 
		Cattle_History.insertDateTime, 
		Cattle_History.insertAccountID
	from Cattle_History 
	left join Cattle_TypeHistory on Cattle_History.hisTypeID = Cattle_TypeHistory.hisTypeID
	left join List_Farm on Cattle_History.farmID = List_Farm.farmID
	left join System_Taiwan on List_Farm.twID = System_Taiwan.twID
	where Cattle_History.removeDateTime is null
GO
ALTER TABLE [dbo].[System_UserFPWLog] ADD  DEFAULT (getdate()) FOR [requestTime]
GO
ALTER TABLE [dbo].[System_UserFPWLog] ADD  DEFAULT ((0)) FOR [isUsed]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Cattle_History"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 196
               Right = 314
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "List_Farm"
            Begin Extent = 
               Top = 9
               Left = 371
               Bottom = 196
               Right = 624
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'_View_CattleHistory_Stay'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'_View_CattleHistory_Stay'
GO
USE [master]
GO
ALTER DATABASE [taif_taifCattle] SET  READ_WRITE 
GO
