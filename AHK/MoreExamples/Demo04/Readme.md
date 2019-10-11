This script shows a simple solution to force the filling in of forms before a status transition is allowed.

Request: [How can EPDM Prompt for ECN form During Transition? What kind of Transition action i have to create or add to fullfill the ECO requirement During transition?](https://forum.solidworks.com/thread/216707)

This example opens a document template for a specific transition and waits until it is closed. The transition will be 
aborted if the document was not edited.

The target file will be saved in the directory **destFolder** using the pattern **original filename.PDM version.docx**.

The values in the section **Replace these variables with yours** have to be adapted. Use this query to get the destination 
state ids for the variable **destinationStateIDs** from the data base: 
```sql
Select [StatusID], [Name] From [dbo].[Status]
```

The script can be extended by the default information from the passed INI file with no problems, e.g. to users or groups.
