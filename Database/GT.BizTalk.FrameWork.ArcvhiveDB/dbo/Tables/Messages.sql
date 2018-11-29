CREATE TABLE [dbo].[Messages] (
    [MessageId]      UNIQUEIDENTIFIER NOT NULL,
    [Body]           VARBINARY (MAX)  NULL,
    [Property]       XML              NULL,
    [Size]           BIGINT           CONSTRAINT [DF_Messages_Size] DEFAULT ((0)) NOT NULL,
    [CreatedOn]      DATE             CONSTRAINT [DF_Messages_CreatedOn] DEFAULT (getdate()) NOT NULL,
    [BodyText]       VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED ([MessageId] ASC)
);

