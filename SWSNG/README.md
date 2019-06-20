# SWSNG
**S**olid**W**orks **S**erial **N**o. **G**enerator

An example of a SQL database based serial number generator.

## How to build the serial number generator

**SQL-Server Management Studio**
- Replace **{Path}** with the target directory.
- Replace **{COMPATIBILITY LEVEL}** with the target SQL-Server version, e.g. 110 for [SQL-Server](https://sqlserverbuilds.blogspot.com/) 2012.
  
- Execute the SQL script to create the **database**, **table** and **stored procedure**. 

## How to get the next free serial number

If **GetNextSerialNo** is executed, the next free number will be returned according to the scheme, and the counter will be increased by one.

``` SQL
DECLARE @serialNo NVARCHAR(MAX);
DECLARE @errCode INT;
DECLARE @errMsg NVARCHAR(MAX);

EXEC GetNextSerialNo N'P000001'
	,@serialNo OUTPUT
	,@errCode OUTPUT
	,@errMsg OUTPUT

SELECT @serialNo
	,@errCode
	,@errMsg
``` 
