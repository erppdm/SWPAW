This script shows a simple solution to rename files automatically according to a predefined pattern after they were added 
to the vault.

Request: [PDM - Multiple serial numbers](https://forum.solidworks.com/thread/214067)

This example works correctly in Windows Explorer if only one file has been created in the vault. The name of the directory 
in which the new files are created must match this **^.{7}$** pattern. All files with the extension **.txt** will be renamed.
In production systems, instead of the local hard disk the vault database has to be checked for existing files.

This query returns all filenames in all directories in the vault.
```sql
SELECT (SELECT [VaultName]
	FROM [BiIEpdmVault].[dbo].[ArchiveServers]
	) + T2.[Path] + T0.[filename]
FROM [Documents] T0
INNER JOIN [DocumentsInProjects] T1 ON T0.[DocumentID] = T1.[DocumentID]
INNER JOIN [Projects] T2 ON T2.[ProjectID] = T1.[ProjectID]
```

[SWSNG](https://github.com/erppdm/SWSNG/tree/master/SQL#swsng), a SQL-based Serial No. Generator.
