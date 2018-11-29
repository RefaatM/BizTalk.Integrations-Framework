CREATE PROCEDURE [dbo].[InsMessages] 
(
@MessageId UNIQUEIDENTIFIER
,@Body VARBINARY(MAX)
,@BodyText VARCHAR(MAX)
,@Property XML
,@Size BIGINT
)
AS
BEGIN
	INSERT INTO [Messages] (MessageId, Body,BodyText, Property,Size)
	VALUES (@MessageId, @Body,@BodyText, @Property,@Size)
END
