This script shows a simple solution for the reworking of legacy data, to which new variables have to be assigned depending on existing variables.

When the files are checked out, the system checks whether a valid value has already been assigned to the new variable. If not, the 'Legacy data' checkbox on the Data Card is assigned the value true. Otherwise nothing is done.

When the files will be checked in, the new variable will be assigned the new value if necessary.

Request: [DataCard](https://forum.solidworks.com/thread/238727)

Prerequisite: A checkbox on the data card to which the variable 'legacyData' is assigned with the variable type 'Yes or No'.
