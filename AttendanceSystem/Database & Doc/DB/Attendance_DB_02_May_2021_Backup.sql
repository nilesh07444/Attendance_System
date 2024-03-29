USE [master]
GO
/****** Object:  Database [db_a72d09_testattendance]    Script Date: 02-05-2021 09:07:07 ******/
CREATE DATABASE [db_a72d09_testattendance]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'db_a72d09_testattendance_Data', FILENAME = N'H:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\db_a72d09_testattendance_DATA.mdf' , SIZE = 8192KB , MAXSIZE = 1024000KB , FILEGROWTH = 10%)
 LOG ON 
( NAME = N'db_a72d09_testattendance_Log', FILENAME = N'H:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\db_a72d09_testattendance_Log.LDF' , SIZE = 3072KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [db_a72d09_testattendance] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [db_a72d09_testattendance].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [db_a72d09_testattendance] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET ARITHABORT OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [db_a72d09_testattendance] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [db_a72d09_testattendance] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET  ENABLE_BROKER 
GO
ALTER DATABASE [db_a72d09_testattendance] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [db_a72d09_testattendance] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [db_a72d09_testattendance] SET  MULTI_USER 
GO
ALTER DATABASE [db_a72d09_testattendance] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [db_a72d09_testattendance] SET DB_CHAINING OFF 
GO
ALTER DATABASE [db_a72d09_testattendance] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [db_a72d09_testattendance] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [db_a72d09_testattendance] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [db_a72d09_testattendance] SET QUERY_STORE = OFF
GO
USE [db_a72d09_testattendance]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
USE [db_a72d09_testattendance]
GO
/****** Object:  Table [dbo].[mst_AdminRole]    Script Date: 02-05-2021 09:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_AdminRole](
	[AdminRoleId] [bigint] NOT NULL,
	[AdminRoleName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_AdminRole] PRIMARY KEY CLUSTERED 
(
	[AdminRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mst_CompanyType]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_CompanyType](
	[CompanyTypeId] [bigint] NOT NULL,
	[CompanyTypeName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_mst_CompanyType] PRIMARY KEY CLUSTERED 
(
	[CompanyTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_AdminUser]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_AdminUser](
	[AdminUserId] [bigint] IDENTITY(1,1) NOT NULL,
	[AdminUserRoleId] [int] NOT NULL,
	[CompanyId] [bigint] NULL,
	[Prefix] [nvarchar](50) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[EmailId] [nvarchar](100) NOT NULL,
	[MobileNo] [nvarchar](20) NOT NULL,
	[AlternateMobileNo] [nvarchar](20) NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[Address] [nvarchar](200) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[AadharCardNo] [nvarchar](20) NOT NULL,
	[PanCardNo] [nvarchar](20) NOT NULL,
	[PanCardPhoto] [nvarchar](100) NOT NULL,
	[UserPhoto] [nvarchar](100) NULL,
	[AadharCardPhoto] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_AdminUser] PRIMARY KEY CLUSTERED 
(
	[AdminUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Attendance]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Attendance](
	[AttendanceId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[AttendanceDate] [datetime] NOT NULL,
	[DayType] [nvarchar](50) NOT NULL,
	[ExtraHours] [decimal](18, 2) NOT NULL,
	[TodayWorkDetail] [nvarchar](500) NULL,
	[TomorrowWorkDetail] [nvarchar](500) NULL,
	[Remarks] [nvarchar](500) NULL,
	[LocationFrom] [nvarchar](200) NULL,
	[Status] [int] NOT NULL,
	[RejectReason] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Attendance] PRIMARY KEY CLUSTERED 
(
	[AttendanceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Company]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Company](
	[CompanyId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyTypeId] [bigint] NOT NULL,
	[CompanyName] [nvarchar](200) NOT NULL,
	[CompanyCode] [nvarchar](100) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[GSTNo] [nvarchar](20) NULL,
	[GSTPhoto] [nvarchar](100) NULL,
	[CompanyPhoto] [nvarchar](100) NOT NULL,
	[CancellationChequePhoto] [nvarchar](100) NOT NULL,
	[RequestStatus] [int] NOT NULL,
	[RejectReason] [nvarchar](500) NULL,
	[FreeAccessDays] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CompanyFollowup]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CompanyFollowup](
	[CompanyFollowupId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[FollowupDate] [datetime] NOT NULL,
	[FollowupText] [nvarchar](500) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[FollowupStatus] [int] NOT NULL,
	[NextDate] [datetime] NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_CompanyFollowup] PRIMARY KEY CLUSTERED 
(
	[CompanyFollowupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CompanyRenewPayment]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CompanyRenewPayment](
	[CompanyRegistrationPaymentId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentFor] [nvarchar](50) NOT NULL,
	[PaymentGatewayResponseId] [nvarchar](500) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[AccessDays] [int] NOT NULL,
	[PackageId] [bigint] NOT NULL,
	[PackageName] [nvarchar](100) NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_CompanyRegistrationPayment] PRIMARY KEY CLUSTERED 
(
	[CompanyRegistrationPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CompanyRequest]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CompanyRequest](
	[CompanyRequestId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyTypeId] [bigint] NOT NULL,
	[CompanyName] [nvarchar](200) NOT NULL,
	[Prefix] [nvarchar](50) NOT NULL,
	[Firstname] [nvarchar](100) NOT NULL,
	[Lastname] [nvarchar](100) NOT NULL,
	[DateOfBirth] [datetime] NOT NULL,
	[EmailId] [nvarchar](100) NOT NULL,
	[MobileNo] [nvarchar](20) NOT NULL,
	[AlternateMobileNo] [nvarchar](20) NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[AadharCardNo] [nvarchar](20) NOT NULL,
	[GSTNo] [nvarchar](20) NULL,
	[PanCardNo] [nvarchar](20) NOT NULL,
	[PanCardPhoto] [nvarchar](100) NOT NULL,
	[AadharCardPhoto] [nvarchar](100) NOT NULL,
	[GSTPhoto] [nvarchar](100) NULL,
	[CompanyPhoto] [nvarchar](100) NOT NULL,
	[CancellationChequePhoto] [nvarchar](100) NOT NULL,
	[RequestStatus] [int] NOT NULL,
	[RejectReason] [nvarchar](500) NULL,
	[CompanyId] [bigint] NULL,
	[RegistrationFee] [decimal](18, 2) NOT NULL,
	[FreeAccessDays] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_CompanyRequest] PRIMARY KEY CLUSTERED 
(
	[CompanyRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CompanySMSPackRenew]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CompanySMSPackRenew](
	[CompanySMSPackRenewId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[SMSPackageId] [bigint] NOT NULL,
	[SMSPackageName] [nvarchar](100) NOT NULL,
	[RenewDate] [datetime] NOT NULL,
	[AccessDays] [int] NOT NULL,
	[PackageExpiryDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_CompanySMSPackRenew] PRIMARY KEY CLUSTERED 
(
	[CompanySMSPackRenewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_DynamicContent]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DynamicContent](
	[DynamicContentId] [bigint] IDENTITY(1,1) NOT NULL,
	[DynamicContentType] [int] NOT NULL,
	[ContentTitle] [nvarchar](100) NOT NULL,
	[ContentDescription] [nvarchar](max) NULL,
	[SeqNo] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
 CONSTRAINT [PK_tbl_DynamicContent] PRIMARY KEY CLUSTERED 
(
	[DynamicContentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Employee]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Employee](
	[EmployeeId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[AdminRoleId] [int] NOT NULL,
	[Prefix] [nvarchar](50) NULL,
	[FirstName] [nvarchar](350) NOT NULL,
	[LastName] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](350) NULL,
	[EmployeeCode] [nvarchar](50) NULL,
	[Password] [nvarchar](150) NOT NULL,
	[MobileNo] [nvarchar](20) NULL,
	[AlternateMobile] [nvarchar](50) NULL,
	[Address] [nvarchar](450) NULL,
	[City] [nvarchar](200) NULL,
	[Designation] [nvarchar](350) NULL,
	[Dob] [datetime] NULL,
	[DateOfJoin] [datetime] NULL,
	[BloodGroup] [nvarchar](50) NULL,
	[WorkingTime] [nvarchar](50) NULL,
	[AdharCardNo] [nvarchar](50) NULL,
	[DateOfIdCardExpiry] [datetime] NULL,
	[Remarks] [nvarchar](600) NULL,
	[ProfilePicture] [nvarchar](100) NULL,
	[EmploymentCategory] [int] NOT NULL,
	[PerCategoryPrice] [decimal](18, 2) NOT NULL,
	[MonthlySalaryPrice] [decimal](18, 2) NULL,
	[ExtraPerHourPrice] [decimal](18, 2) NULL,
	[IsLeaveForward] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsFingerprintEnabled] [bit] NOT NULL,
	[IsFingerprintSaved] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Employee] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_EmployeePayment]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_EmployeePayment](
	[EmployeePaymentId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[PaymentDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PaymentType] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
 CONSTRAINT [PK_tbl_EmployeePayment] PRIMARY KEY CLUSTERED 
(
	[EmployeePaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_EmployeeRating]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_EmployeeRating](
	[EmployeeRatingId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[RateMonth] [int] NOT NULL,
	[RateYear] [int] NOT NULL,
	[BehaviourRate] [decimal](18, 2) NOT NULL,
	[RegularityRate] [decimal](18, 2) NOT NULL,
	[WorkRate] [decimal](18, 2) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
 CONSTRAINT [PK_tbl_EmployeeRating] PRIMARY KEY CLUSTERED 
(
	[EmployeeRatingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Feedback]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Feedback](
	[FeedbackId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[FeedbackType] [int] NOT NULL,
	[FeedbackStatus] [int] NOT NULL,
	[FeedbackText] [nvarchar](1000) NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[SuperAdminFeedbackText] [nvarchar](1000) NULL,
	[IsDeleted] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Feedback] PRIMARY KEY CLUSTERED 
(
	[FeedbackId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Holiday]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Holiday](
	[HolidayId] [bigint] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[HolidayReason] [nvarchar](100) NOT NULL,
	[Remark] [nvarchar](500) NULL,
	[CompanyId] [nvarchar](200) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Holiday] PRIMARY KEY CLUSTERED 
(
	[HolidayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_HomeImage]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_HomeImage](
	[HomeImageId] [bigint] IDENTITY(1,1) NOT NULL,
	[HomeImageName] [nvarchar](100) NOT NULL,
	[HeadingText1] [nvarchar](100) NULL,
	[HeadingText2] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[HomeImageFor] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
 CONSTRAINT [PK_tbl_HomeImage] PRIMARY KEY CLUSTERED 
(
	[HomeImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Leave]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Leave](
	[LeaveId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[NoOfDays] [decimal](18, 2) NOT NULL,
	[LeaveStatus] [int] NOT NULL,
	[LeaveReason] [nvarchar](500) NULL,
	[RejectReason] [nvarchar](500) NULL,
	[CancelledReason] [nvarchar](500) NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Leave] PRIMARY KEY CLUSTERED 
(
	[LeaveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_LoginHistory]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_LoginHistory](
	[LoginHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[LoginDate] [datetime] NOT NULL,
	[LocationFrom] [nvarchar](200) NULL,
	[SiteId] [bigint] NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_LoginHistory] PRIMARY KEY CLUSTERED 
(
	[LoginHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Material]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Material](
	[MaterialId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[MaterialCategoryId] [bigint] NULL,
	[MaterialDate] [datetime] NOT NULL,
	[SiteId] [bigint] NOT NULL,
	[Qty] [decimal](18, 2) NOT NULL,
	[InOut] [int] NOT NULL,
	[Remarks] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Material] PRIMARY KEY CLUSTERED 
(
	[MaterialId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_MaterialCategory]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_MaterialCategory](
	[MaterialCategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[MaterialCategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_MaterialCategory] PRIMARY KEY CLUSTERED 
(
	[MaterialCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Package]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Package](
	[PackageId] [bigint] IDENTITY(1,1) NOT NULL,
	[PackageName] [nvarchar](100) NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[PackageDescription] [nvarchar](500) NULL,
	[AccessDays] [int] NOT NULL,
	[PackageImage] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [bigint] NULL,
 CONSTRAINT [PK_tbl_Package] PRIMARY KEY CLUSTERED 
(
	[PackageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Setting]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Setting](
	[SettingId] [bigint] IDENTITY(1,1) NOT NULL,
	[SiteCompanyFreeAccessDays] [int] NULL,
	[OfficeCompanyFreeAccessDays] [int] NULL,
 CONSTRAINT [PK_tbl_Setting] PRIMARY KEY CLUSTERED 
(
	[SettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Site]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Site](
	[SiteId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[SiteName] [nvarchar](200) NOT NULL,
	[SiteDescription] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Site] PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_SMSPackage]    Script Date: 02-05-2021 09:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_SMSPackage](
	[SMSPackageId] [bigint] IDENTITY(1,1) NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[PackageName] [nvarchar](100) NOT NULL,
	[PackageImage] [nvarchar](100) NULL,
	[PackageAmount] [decimal](18, 2) NOT NULL,
	[AccessDays] [int] NOT NULL,
	[NoOfSMS] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedBy] [bigint] NULL,
	[ModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_SMSPackage] PRIMARY KEY CLUSTERED 
(
	[SMSPackageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (1, N'Super Admin')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (2, N'Company Admin')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (3, N'Employee')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (4, N'Supervisor')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (5, N'Checker')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (6, N'Payer')
INSERT [dbo].[mst_AdminRole] ([AdminRoleId], [AdminRoleName]) VALUES (7, N'Worker')
GO
INSERT [dbo].[mst_CompanyType] ([CompanyTypeId], [CompanyTypeName]) VALUES (1, N'Banking/Office Company')
INSERT [dbo].[mst_CompanyType] ([CompanyTypeId], [CompanyTypeName]) VALUES (2, N'Construction Company')
GO
SET IDENTITY_INSERT [dbo].[tbl_AdminUser] ON 

INSERT [dbo].[tbl_AdminUser] ([AdminUserId], [AdminUserRoleId], [CompanyId], [Prefix], [FirstName], [LastName], [UserName], [Password], [EmailId], [MobileNo], [AlternateMobileNo], [DateOfBirth], [Address], [City], [State], [AadharCardNo], [PanCardNo], [PanCardPhoto], [UserPhoto], [AadharCardPhoto], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, NULL, N'Mr.', N'Nilesh', N'Prajapati', N'admin', N'8V3XxjpeyWU=', N'prajapati.nileshbhai@gmail.com', N'9999999999', NULL, CAST(N'1989-04-18T00:00:00.000' AS DateTime), N'100 foot road', N'Anand', N'Gujarat', N'123456789012', N'123456789012', N'test.jpg', N'test.jpg', N'test.jpg', 1, 0, -1, CAST(N'2021-04-16T00:00:00.000' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_AdminUser] ([AdminUserId], [AdminUserRoleId], [CompanyId], [Prefix], [FirstName], [LastName], [UserName], [Password], [EmailId], [MobileNo], [AlternateMobileNo], [DateOfBirth], [Address], [City], [State], [AadharCardNo], [PanCardNo], [PanCardPhoto], [UserPhoto], [AadharCardPhoto], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 2, 1, N'Mr.', N'Kamlesh', N'Lalwani', N'UN/18042021/1', N'8V3XxjpeyWU=', N'kamlesh@gmail.com', N'9824936252', NULL, CAST(N'1989-04-18T00:00:00.000' AS DateTime), N'Near Krupa Build Gallery,', N'Anand', N'Gujarat', N'123456789012', N'123456789012', N'test.jpg', N'test.jpg', N'test.jpg', 1, 0, 1, CAST(N'2021-04-16T00:00:00.000' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_AdminUser] ([AdminUserId], [AdminUserRoleId], [CompanyId], [Prefix], [FirstName], [LastName], [UserName], [Password], [EmailId], [MobileNo], [AlternateMobileNo], [DateOfBirth], [Address], [City], [State], [AadharCardNo], [PanCardNo], [PanCardPhoto], [UserPhoto], [AadharCardPhoto], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 2, 5, N'Mr', N'Dipak', N'jumde', N'UN/2304YYYY/2', N'rXTyMBCTLXY=', N'dipakjumade@gmail.com', N'9632587410', NULL, CAST(N'1988-08-11T00:00:00.000' AS DateTime), NULL, N'Surat', N'Gujarat', N'987654321012', N'aserv7547h', N'710a0289-9104-46bf-ace9-ed83be1bfe60-download (4).jpg', NULL, N'a11e50b7-d93b-458d-8a9e-aee01b6879e0-download (5).jpg', 1, 0, 1, CAST(N'2021-04-23T10:34:06.000' AS DateTime), 1, CAST(N'2021-04-23T10:34:06.403' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_AdminUser] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Company] ON 

INSERT [dbo].[tbl_Company] ([CompanyId], [CompanyTypeId], [CompanyName], [CompanyCode], [City], [State], [GSTNo], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [FreeAccessDays], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 2, N'Universal Infotech', N'UN/18042021/1', N'Godhra', N'Gujarat', N'123456789012', N'test.jpg', N'test.jpg', N'test.jpg', 1, NULL, 60, 1, 0, -1, CAST(N'2021-04-18T00:00:00.000' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_Company] ([CompanyId], [CompanyTypeId], [CompanyName], [CompanyCode], [City], [State], [GSTNo], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [FreeAccessDays], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, N'Company name', N'UN/2304YYYY/2', N'BARODA', N'GUJARAT', N'4569852147852', N'1c4d9356-afee-46d0-97ae-1ce397225cc2-download (3).jpg', N'cf00d9a7-bdc1-44db-8bc1-d14621ac114c-download (2).jpg', N'd44d72b8-0a23-4f03-a7c9-2167b4a458dc-download (1).jpg', 2, NULL, 90, 1, 0, 1, CAST(N'2021-04-23T10:21:23.487' AS DateTime), 1, CAST(N'2021-04-23T10:21:23.903' AS DateTime))
INSERT [dbo].[tbl_Company] ([CompanyId], [CompanyTypeId], [CompanyName], [CompanyCode], [City], [State], [GSTNo], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [FreeAccessDays], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, N'Company name', N'UN/2304YYYY/3', N'BARODA', N'GUJARAT', N'4569852147852', N'1c4d9356-afee-46d0-97ae-1ce397225cc2-download (3).jpg', N'cf00d9a7-bdc1-44db-8bc1-d14621ac114c-download (2).jpg', N'd44d72b8-0a23-4f03-a7c9-2167b4a458dc-download (1).jpg', 2, NULL, 90, 1, 0, 1, CAST(N'2021-04-23T10:23:16.087' AS DateTime), 1, CAST(N'2021-04-23T10:23:16.473' AS DateTime))
INSERT [dbo].[tbl_Company] ([CompanyId], [CompanyTypeId], [CompanyName], [CompanyCode], [City], [State], [GSTNo], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [FreeAccessDays], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 1, N'Company name', N'UN/2304YYYY/3', N'BARODA', N'GUJARAT', N'4569852147852', N'1c4d9356-afee-46d0-97ae-1ce397225cc2-download (3).jpg', N'cf00d9a7-bdc1-44db-8bc1-d14621ac114c-download (2).jpg', N'd44d72b8-0a23-4f03-a7c9-2167b4a458dc-download (1).jpg', 2, NULL, 90, 1, 0, 1, CAST(N'2021-04-23T10:27:03.880' AS DateTime), 1, CAST(N'2021-04-23T10:27:04.723' AS DateTime))
INSERT [dbo].[tbl_Company] ([CompanyId], [CompanyTypeId], [CompanyName], [CompanyCode], [City], [State], [GSTNo], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [FreeAccessDays], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, 1, N'Dipak jumde', N'UN/2304YYYY/2', N'Surat', N'Gujarat', N'123456789012345', N'48772fa3-0933-4a81-898e-ebe4c943cf56-download (3).jpg', N'ee204e81-3e1a-4eb1-8d27-e399b607bd59-download (2).jpg', N'473ea6aa-09e3-45c1-9c2a-d8f61420be10-download (1).jpg', 2, NULL, 60, 1, 0, 1, CAST(N'2021-04-23T10:32:31.603' AS DateTime), 1, CAST(N'2021-04-23T10:32:32.050' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Company] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_CompanyRenewPayment] ON 

INSERT [dbo].[tbl_CompanyRenewPayment] ([CompanyRegistrationPaymentId], [CompanyId], [Amount], [PaymentFor], [PaymentGatewayResponseId], [StartDate], [EndDate], [AccessDays], [PackageId], [PackageName], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, CAST(1000.00 AS Decimal(18, 2)), N'Renew', N'', CAST(N'2021-04-01T00:00:00.000' AS DateTime), CAST(N'2022-03-31T00:00:00.000' AS DateTime), 365, 2, N'GOld Place', 1, CAST(N'2021-04-23T09:50:13.003' AS DateTime), 1, CAST(N'2021-04-23T09:50:13.003' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_CompanyRenewPayment] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_CompanyRequest] ON 

INSERT [dbo].[tbl_CompanyRequest] ([CompanyRequestId], [CompanyTypeId], [CompanyName], [Prefix], [Firstname], [Lastname], [DateOfBirth], [EmailId], [MobileNo], [AlternateMobileNo], [City], [State], [AadharCardNo], [GSTNo], [PanCardNo], [PanCardPhoto], [AadharCardPhoto], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [CompanyId], [RegistrationFee], [FreeAccessDays], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, N'Company name', N'Mr', N'TESTING', N'CUSTOMER', CAST(N'2000-02-01T00:00:00.000' AS DateTime), N'abcd@gmail.com', N'9876543210', N'9887665555', N'BARODA', N'GUJARAT', N'9876533232323', N'4569852147852', N'arbpjsdsk', N'd569ad91-8063-471d-a913-65de822aa6d5-download (4).jpg', N'053a2d22-85f0-42a5-a99f-298cca66dd4b-download (5).jpg', N'1c4d9356-afee-46d0-97ae-1ce397225cc2-download (3).jpg', N'cf00d9a7-bdc1-44db-8bc1-d14621ac114c-download (2).jpg', N'd44d72b8-0a23-4f03-a7c9-2167b4a458dc-download (1).jpg', 2, NULL, 4, CAST(0.00 AS Decimal(18, 2)), 90, 0, 0, CAST(N'2021-04-21T03:10:36.300' AS DateTime), 1, CAST(N'2021-04-23T10:26:49.270' AS DateTime))
INSERT [dbo].[tbl_CompanyRequest] ([CompanyRequestId], [CompanyTypeId], [CompanyName], [Prefix], [Firstname], [Lastname], [DateOfBirth], [EmailId], [MobileNo], [AlternateMobileNo], [City], [State], [AadharCardNo], [GSTNo], [PanCardNo], [PanCardPhoto], [AadharCardPhoto], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [CompanyId], [RegistrationFee], [FreeAccessDays], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, N'company name1 ', N'Mr', N'TESTING', N'CUSTOMER', CAST(N'2021-04-21T00:00:00.000' AS DateTime), N'abcd@gmail.com', N'9876543210', N'9887665555', N'BARODA', N'GUJARAT', N'987653323232', N'4569852147852', N'arbpj7548e', N'cb1d601e-6981-4dbd-b336-bd6989428e12-download (5).jpg', N'978c60a2-d300-45d9-9815-2391b5494bd9-download (4).jpg', N'df124b57-5ebb-4b3c-8951-21ee81b03b4b-download (2).jpg', N'5c1ac07c-3ef6-474e-be4e-6c355efd9399-download (3).jpg', N'9d6f4a16-aa8e-4862-9428-ff76491d7a53-download (1).jpg', 3, N'Testing entry', NULL, CAST(0.00 AS Decimal(18, 2)), 60, 0, 0, CAST(N'2021-04-22T02:54:12.787' AS DateTime), 1, CAST(N'2021-04-23T10:37:10.300' AS DateTime))
INSERT [dbo].[tbl_CompanyRequest] ([CompanyRequestId], [CompanyTypeId], [CompanyName], [Prefix], [Firstname], [Lastname], [DateOfBirth], [EmailId], [MobileNo], [AlternateMobileNo], [City], [State], [AadharCardNo], [GSTNo], [PanCardNo], [PanCardPhoto], [AadharCardPhoto], [GSTPhoto], [CompanyPhoto], [CancellationChequePhoto], [RequestStatus], [RejectReason], [CompanyId], [RegistrationFee], [FreeAccessDays], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, N'Dipak jumde', N'Mr', N'Dipak', N'jumde', CAST(N'1988-08-11T00:00:00.000' AS DateTime), N'dipakjumade@gmail.com', N'9632587410', NULL, N'Surat', N'Gujarat', N'987654321012', N'123456789012345', N'aserv7547h', N'710a0289-9104-46bf-ace9-ed83be1bfe60-download (4).jpg', N'a11e50b7-d93b-458d-8a9e-aee01b6879e0-download (5).jpg', N'48772fa3-0933-4a81-898e-ebe4c943cf56-download (3).jpg', N'ee204e81-3e1a-4eb1-8d27-e399b607bd59-download (2).jpg', N'473ea6aa-09e3-45c1-9c2a-d8f61420be10-download (1).jpg', 2, NULL, 5, CAST(0.00 AS Decimal(18, 2)), 60, 0, 0, CAST(N'2021-04-23T10:31:24.043' AS DateTime), 1, CAST(N'2021-04-23T10:32:22.977' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_CompanyRequest] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Employee] ON 

INSERT [dbo].[tbl_Employee] ([EmployeeId], [CompanyId], [AdminRoleId], [Prefix], [FirstName], [LastName], [Email], [EmployeeCode], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [EmploymentCategory], [PerCategoryPrice], [MonthlySalaryPrice], [ExtraPerHourPrice], [IsLeaveForward], [IsActive], [IsDeleted], [IsFingerprintEnabled], [IsFingerprintSaved], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, 1, 3, N'Mr', N'Dipak', N'jumade', N'dipakjumade@gmail.com', N'ilkzjl', N'8V3XxjpeyWU=', N'7878070294', NULL, N'Baroda', N'baroda', NULL, CAST(N'1988-08-01T00:00:00.000' AS DateTime), CAST(N'2021-01-01T00:00:00.000' AS DateTime), N'(A+)', N'10 am - 5 pm', N'123456789012', NULL, N'test remark', N'96a20db7-4e3d-4fec-92d9-fe1395c43912-download (5).jpg', 1, CAST(100.00 AS Decimal(18, 2)), CAST(5000.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), 0, 1, 0, 1, 0, 3, CAST(N'2021-04-26T02:42:58.423' AS DateTime), 2, CAST(N'2021-05-01T14:04:43.500' AS DateTime))
INSERT [dbo].[tbl_Employee] ([EmployeeId], [CompanyId], [AdminRoleId], [Prefix], [FirstName], [LastName], [Email], [EmployeeCode], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [EmploymentCategory], [PerCategoryPrice], [MonthlySalaryPrice], [ExtraPerHourPrice], [IsLeaveForward], [IsActive], [IsDeleted], [IsFingerprintEnabled], [IsFingerprintSaved], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, 1, 4, N'Mr', N'Nilesh', N'prajapati', N'nilesh@gmail.com', NULL, N'MQNpmiU9MfQ=', N'7878070944', NULL, N'anand guajarat', N'anand', NULL, CAST(N'1990-01-01T00:00:00.000' AS DateTime), CAST(N'2021-02-01T00:00:00.000' AS DateTime), N'(A+)', N'10 am - 5 pm', N'123456789012', NULL, N'test remark', N'8aca2c3a-71c9-4e9b-b6e8-b9dca59cfe77-download (2).jpg', 2, CAST(250.00 AS Decimal(18, 2)), CAST(10000.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), 1, 1, 0, 0, 0, 3, CAST(N'2021-04-26T02:44:16.507' AS DateTime), 3, CAST(N'2021-04-26T02:45:32.493' AS DateTime))
INSERT [dbo].[tbl_Employee] ([EmployeeId], [CompanyId], [AdminRoleId], [Prefix], [FirstName], [LastName], [Email], [EmployeeCode], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [EmploymentCategory], [PerCategoryPrice], [MonthlySalaryPrice], [ExtraPerHourPrice], [IsLeaveForward], [IsActive], [IsDeleted], [IsFingerprintEnabled], [IsFingerprintSaved], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 1, 3, N'Mr', N'TESTING', N'Employee', N'testing@gmail.com', N'ekqscp', N'Urs+n557kR8=', N'7878070294', NULL, N'BARODA', N'BARODA', NULL, CAST(N'2021-04-20T00:00:00.000' AS DateTime), CAST(N'2021-04-05T00:00:00.000' AS DateTime), N'(A-)', N'10 am - 5 pm', N'123456789012', NULL, N'remark', N'918c635e-43e4-4f07-b91f-46753a28803e-download (4).jpg', 1, CAST(100.00 AS Decimal(18, 2)), NULL, CAST(100.00 AS Decimal(18, 2)), 0, 1, 0, 0, 0, 3, CAST(N'2021-04-26T07:00:20.900' AS DateTime), 3, CAST(N'2021-04-26T07:00:20.900' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Employee] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_EmployeePayment] ON 

INSERT [dbo].[tbl_EmployeePayment] ([EmployeePaymentId], [UserId], [PaymentDate], [Amount], [PaymentType], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1, 2, CAST(N'2021-04-28T00:00:00.000' AS DateTime), CAST(100.00 AS Decimal(18, 2)), 1, 0, CAST(N'2021-04-28T03:08:05.443' AS DateTime), 3, CAST(N'2021-04-28T03:09:34.040' AS DateTime), 3)
INSERT [dbo].[tbl_EmployeePayment] ([EmployeePaymentId], [UserId], [PaymentDate], [Amount], [PaymentType], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (2, 3, CAST(N'2021-04-21T00:00:00.000' AS DateTime), CAST(100.00 AS Decimal(18, 2)), 2, 0, CAST(N'2021-04-28T03:08:26.637' AS DateTime), 3, CAST(N'2021-04-28T03:09:19.967' AS DateTime), 3)
SET IDENTITY_INSERT [dbo].[tbl_EmployeePayment] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Holiday] ON 

INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday 1', NULL, N'1', 1, 1, 3, CAST(N'2021-04-24T06:55:24.103' AS DateTime), 3, CAST(N'2021-04-24T06:55:50.257' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday 1', NULL, N'1', 1, 0, 3, CAST(N'2021-04-24T06:56:07.763' AS DateTime), 3, CAST(N'2021-04-24T06:56:07.763' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, CAST(N'2021-04-27T00:00:00.000' AS DateTime), CAST(N'2021-04-27T00:00:00.000' AS DateTime), N'Holiday 1222', NULL, N'1', 1, 0, 3, CAST(N'2021-04-24T08:00:49.083' AS DateTime), 3, CAST(N'2021-04-24T08:01:48.110' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, CAST(N'2021-04-24T00:00:00.000' AS DateTime), CAST(N'2021-04-26T00:00:00.000' AS DateTime), N'Holiday 3', NULL, N'1', 1, 0, 3, CAST(N'2021-04-24T14:46:44.943' AS DateTime), 3, CAST(N'2021-04-24T14:46:44.943' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'test', NULL, N'1', 1, 0, 3, CAST(N'2021-04-24T18:42:13.157' AS DateTime), 3, CAST(N'2021-04-24T18:42:13.157' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'test', NULL, N'1', 1, 0, 3, CAST(N'2021-04-24T18:42:35.897' AS DateTime), 3, CAST(N'2021-04-24T18:42:35.897' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'HOliday 1 ', N'remarks', N'1', 1, 0, 3, CAST(N'2021-04-25T12:08:42.500' AS DateTime), 3, CAST(N'2021-04-25T12:08:42.500' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'holiday 2 ', N'remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:10:04.367' AS DateTime), 3, CAST(N'2021-04-25T12:10:04.370' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'holiday 3', N'reamrk', N'1', 1, 0, 3, CAST(N'2021-04-25T12:11:33.997' AS DateTime), 3, CAST(N'2021-04-25T12:11:33.997' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (10, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday 5', N'Reamrk', N'1', 1, 0, 3, CAST(N'2021-04-25T12:12:54.363' AS DateTime), 3, CAST(N'2021-04-25T12:12:54.363' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (11, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday 6', N'remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:14:06.160' AS DateTime), 3, CAST(N'2021-04-25T12:14:06.160' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (12, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'holiday7', N'remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:14:52.397' AS DateTime), 3, CAST(N'2021-04-25T12:14:52.397' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (13, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday', N'remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:19:09.033' AS DateTime), 3, CAST(N'2021-04-25T12:19:09.033' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (14, CAST(N'2021-04-25T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'holiday', N'remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:19:53.757' AS DateTime), 3, CAST(N'2021-04-25T12:19:53.757' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (15, CAST(N'2021-05-01T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday', N'Remark', N'1', 1, 1, 3, CAST(N'2021-04-25T12:45:13.110' AS DateTime), 3, CAST(N'2021-04-25T12:45:41.243' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (16, CAST(N'2021-05-01T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'holiday', N'remark', N'1', 1, 1, 3, CAST(N'2021-04-25T12:46:15.493' AS DateTime), 3, CAST(N'2021-04-25T12:46:22.023' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (17, CAST(N'2021-05-01T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Holiday Reason', N'Remark', N'1', 1, 1, 3, CAST(N'2021-04-25T12:47:36.610' AS DateTime), 3, CAST(N'2021-04-25T12:47:42.537' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (18, CAST(N'2021-05-01T00:00:00.000' AS DateTime), CAST(N'2021-04-25T00:00:00.000' AS DateTime), N'Remark', N'Remark', N'1', 1, 1, 3, CAST(N'2021-04-25T12:48:36.820' AS DateTime), 3, CAST(N'2021-04-25T12:48:43.453' AS DateTime))
INSERT [dbo].[tbl_Holiday] ([HolidayId], [StartDate], [EndDate], [HolidayReason], [Remark], [CompanyId], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (19, CAST(N'2021-04-28T00:00:00.000' AS DateTime), CAST(N'2021-05-01T00:00:00.000' AS DateTime), N'Holiday Reason', N'Remark', N'1', 1, 0, 3, CAST(N'2021-04-25T12:51:40.757' AS DateTime), 3, CAST(N'2021-04-25T12:51:40.757' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Holiday] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_HomeImage] ON 

INSERT [dbo].[tbl_HomeImage] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [HomeImageFor], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (2, N'dcc03aee-dc65-4f74-82a3-6c34d93b1a8f-download.jpg', N'Headeing text1', N'Heading text 2', 1, 1, CAST(N'2021-04-19T02:49:12.767' AS DateTime), 1, CAST(N'2021-04-19T13:41:10.283' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_HomeImage] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Leave] ON 

INSERT [dbo].[tbl_Leave] ([LeaveId], [UserId], [StartDate], [EndDate], [NoOfDays], [LeaveStatus], [LeaveReason], [RejectReason], [CancelledReason], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 2, CAST(N'2021-05-02T00:00:00.000' AS DateTime), CAST(N'2021-05-02T00:00:00.000' AS DateTime), CAST(1.00 AS Decimal(18, 2)), 1, N'Testing entry', NULL, NULL, 0, 2, CAST(N'2021-05-01T08:56:37.160' AS DateTime), 2, CAST(N'2021-05-01T08:56:37.160' AS DateTime))
INSERT [dbo].[tbl_Leave] ([LeaveId], [UserId], [StartDate], [EndDate], [NoOfDays], [LeaveStatus], [LeaveReason], [RejectReason], [CancelledReason], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, 2, CAST(N'2021-05-03T00:00:00.000' AS DateTime), CAST(N'2021-05-04T00:00:00.000' AS DateTime), CAST(2.00 AS Decimal(18, 2)), 1, N'Testing entry', NULL, NULL, 0, 2, CAST(N'2021-05-01T08:58:10.300' AS DateTime), 2, CAST(N'2021-05-01T08:58:10.300' AS DateTime))
INSERT [dbo].[tbl_Leave] ([LeaveId], [UserId], [StartDate], [EndDate], [NoOfDays], [LeaveStatus], [LeaveReason], [RejectReason], [CancelledReason], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, 2, CAST(N'2021-05-05T00:00:00.000' AS DateTime), CAST(N'2021-05-05T00:00:00.000' AS DateTime), CAST(1.00 AS Decimal(18, 2)), 1, N'Testing entry', NULL, NULL, 0, 2, CAST(N'2021-05-01T09:39:37.883' AS DateTime), 2, CAST(N'2021-05-01T09:39:37.883' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Leave] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] ON 

INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 2, CAST(N'2021-04-29T02:10:25.233' AS DateTime), N'', 1, 2, CAST(N'2021-04-29T02:10:25.233' AS DateTime), 2, CAST(N'2021-04-29T02:10:25.233' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 2, CAST(N'2021-04-30T02:51:05.160' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-04-30T02:51:05.160' AS DateTime), 2, CAST(N'2021-04-30T02:51:05.163' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 2, CAST(N'2021-05-01T07:06:07.760' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T07:06:07.760' AS DateTime), 2, CAST(N'2021-05-01T07:06:07.760' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (4, 2, CAST(N'2021-05-01T08:34:18.380' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T08:34:18.380' AS DateTime), 2, CAST(N'2021-05-01T08:34:18.380' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (5, 2, CAST(N'2021-05-01T09:06:04.597' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T09:06:04.597' AS DateTime), 2, CAST(N'2021-05-01T09:06:04.597' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (6, 2, CAST(N'2021-05-01T09:18:17.767' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T09:18:17.767' AS DateTime), 2, CAST(N'2021-05-01T09:18:17.767' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (7, 2, CAST(N'2021-05-01T09:39:20.443' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T09:39:20.443' AS DateTime), 2, CAST(N'2021-05-01T09:39:20.443' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (8, 2, CAST(N'2021-05-01T14:02:41.703' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T14:02:41.703' AS DateTime), 2, CAST(N'2021-05-01T14:02:41.703' AS DateTime))
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [EmployeeId], [LoginDate], [LocationFrom], [SiteId], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (9, 2, CAST(N'2021-05-01T14:07:54.583' AS DateTime), N'Baroda Site', 1, 2, CAST(N'2021-05-01T14:07:54.583' AS DateTime), 2, CAST(N'2021-05-01T14:07:54.583' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Material] ON 

INSERT [dbo].[tbl_Material] ([MaterialId], [CompanyId], [MaterialCategoryId], [MaterialDate], [SiteId], [Qty], [InOut], [Remarks], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, 1, CAST(N'2021-05-01T00:00:00.000' AS DateTime), 3, CAST(10.00 AS Decimal(18, 2)), 1, N'Material in', 1, 1, 3, CAST(N'2021-05-01T13:46:00.760' AS DateTime), 3, CAST(N'2021-05-01T13:46:51.113' AS DateTime))
INSERT [dbo].[tbl_Material] ([MaterialId], [CompanyId], [MaterialCategoryId], [MaterialDate], [SiteId], [Qty], [InOut], [Remarks], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, 1, CAST(N'2021-05-01T00:00:00.000' AS DateTime), 2, CAST(20.00 AS Decimal(18, 2)), 2, N'Material out', 1, 0, 3, CAST(N'2021-05-01T13:46:21.173' AS DateTime), 3, CAST(N'2021-05-01T13:46:37.913' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Material] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_MaterialCategory] ON 

INSERT [dbo].[tbl_MaterialCategory] ([MaterialCategoryId], [CompanyId], [MaterialCategoryName], [Description], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 1, N'Cement', N'Cement BIrla', 1, 0, 3, CAST(N'2021-05-01T12:02:03.633' AS DateTime), 3, CAST(N'2021-05-01T12:02:56.700' AS DateTime))
INSERT [dbo].[tbl_MaterialCategory] ([MaterialCategoryId], [CompanyId], [MaterialCategoryName], [Description], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, N'Saliya', N'TMT Saliya sdfgfdg', 1, 1, 3, CAST(N'2021-05-01T12:02:35.907' AS DateTime), 3, CAST(N'2021-05-01T12:03:11.470' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_MaterialCategory] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Package] ON 

INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (1, N'Package 12', CAST(2.00 AS Decimal(18, 2)), N'Descr', 2, N'de9c428d-39dd-4037-92c3-764f95af845b-demo-300x216.png', 1, 1, CAST(N'2021-04-19T15:32:13.860' AS DateTime), 1, CAST(N'2021-04-19T15:45:49.830' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (2, N'Gold place', CAST(10.00 AS Decimal(18, 2)), N'Gold place', 10, N'8f5a15d2-6f4d-442c-b3ed-839fedae2900-demo-300x216.png', 1, 0, CAST(N'2021-04-19T15:46:38.393' AS DateTime), 1, CAST(N'2021-04-19T17:49:56.323' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (3, N'silver', CAST(50.00 AS Decimal(18, 2)), N'selver desc', 5, N'2f315361-3cb3-481f-9a57-c573c346dd3c-download.jpg', 1, 0, CAST(N'2021-04-19T15:47:49.833' AS DateTime), 1, CAST(N'2021-04-19T15:48:20.397' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (4, N'test package', CAST(0.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'5f17b72c-45d7-4bf3-bbef-72c234f54291-b.png', 1, 0, CAST(N'2021-04-22T18:36:24.870' AS DateTime), 1, CAST(N'2021-04-22T18:36:24.870' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (5, N'test package', CAST(500.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'f2e2995c-73f5-4545-b62f-4e9677d7014f-b.png', 1, 1, CAST(N'2021-04-22T18:36:54.447' AS DateTime), 1, CAST(N'2021-04-22T18:59:29.323' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (6, N'test package', CAST(500.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'55f2a324-dcba-4906-ba5b-62dec6c041db-b.png', 1, 1, CAST(N'2021-04-22T18:37:15.703' AS DateTime), 1, CAST(N'2021-04-22T18:48:00.527' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (7, N'test package', CAST(500.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'ac998b12-78b1-4212-bf60-77a7d84e997f-b.png', 1, 1, CAST(N'2021-04-22T18:39:16.670' AS DateTime), 1, CAST(N'2021-04-22T18:39:25.177' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (8, N'test package', CAST(100.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'a828f0e9-7191-468b-8c6e-da9f7eb320d4-download (4).jpg', 1, 0, CAST(N'2021-04-26T06:24:17.877' AS DateTime), 1, CAST(N'2021-04-26T06:26:28.523' AS DateTime), 1)
INSERT [dbo].[tbl_Package] ([PackageId], [PackageName], [Amount], [PackageDescription], [AccessDays], [PackageImage], [IsActive], [IsDeleted], [CreatedDate], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (9, N'Nilesh package demo 1 ', CAST(100.00 AS Decimal(18, 2)), N'this is the testing package', 15, N'bdb7e930-a832-46c9-8d2b-6ba35a872cf0-download (4).jpg', 1, 0, CAST(N'2021-04-26T06:25:00.663' AS DateTime), 1, CAST(N'2021-04-26T06:26:43.993' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[tbl_Package] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Setting] ON 

INSERT [dbo].[tbl_Setting] ([SettingId], [SiteCompanyFreeAccessDays], [OfficeCompanyFreeAccessDays]) VALUES (1, 30, 60)
SET IDENTITY_INSERT [dbo].[tbl_Setting] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Site] ON 

INSERT [dbo].[tbl_Site] ([SiteId], [CompanyId], [SiteName], [SiteDescription], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (1, 0, N'Baroda site', N'Baroda site description', 1, 0, 3, CAST(N'2021-05-01T11:33:17.687' AS DateTime), 3, CAST(N'2021-05-01T11:33:17.687' AS DateTime))
INSERT [dbo].[tbl_Site] ([SiteId], [CompanyId], [SiteName], [SiteDescription], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (2, 1, N'Baroda', N'Baroda site', 1, 0, 3, CAST(N'2021-05-01T11:40:52.380' AS DateTime), 3, CAST(N'2021-05-01T11:43:34.360' AS DateTime))
INSERT [dbo].[tbl_Site] ([SiteId], [CompanyId], [SiteName], [SiteDescription], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [ModifiedBy], [ModifiedDate]) VALUES (3, 1, N'Baroda 1 222', N'Baroda site 12222', 1, 0, 3, CAST(N'2021-05-01T11:41:34.523' AS DateTime), 3, CAST(N'2021-05-01T11:43:46.910' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Site] OFF
GO
USE [master]
GO
ALTER DATABASE [db_a72d09_testattendance] SET  READ_WRITE 
GO
