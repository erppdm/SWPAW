This script shows a simple solution for forcing the filling in of comments before the status change. Unlike the option offered 
by the PDM, there is no restriction in this solution.

Request: [Require comment for specific transisiton](https://forum.solidworks.com/thread/215419)

The values in the section **Replace these variables with yours** have to be adapted. Use this query to get the destination 
state ids for the variable **destinationStateIDs** from the data base: 
```sql
Select [StatusID], [Name] From [dbo].[Status]
```
The script can be extended by the default information from the passed INI file with no problems, e.g. to users or groups.
