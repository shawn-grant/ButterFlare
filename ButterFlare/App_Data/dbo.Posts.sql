CREATE TABLE [dbo].[Posts] (
    [Id]      INT              NOT NULL,
    [UID]     UNIQUEIDENTIFIER NOT NULL,
    [caption] NVARCHAR (MAX)   NOT NULL,
    [image]   VARBINARY (MAX)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

