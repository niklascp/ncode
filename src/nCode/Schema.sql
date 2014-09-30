GO
/****** Object:  Table [dbo].[Global_Templates]    Script Date: 12/07/2007 02:22:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Global_Templates](
	[ID] [uniqueidentifier] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[GlobalSender] [bit] NOT NULL,
	[SenderName] [nvarchar](80) NULL,
	[SenderAddress] [nvarchar](80) NULL,
	[Subject] [nvarchar](255) NULL,
	[ContentHTML] [ntext] NULL,
	[ContentText] [ntext] NULL,
 CONSTRAINT [PK_Global_Templates] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]