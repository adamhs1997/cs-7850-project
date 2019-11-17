# CS 7850 Project: Comparison of PINQ to LINQ
Code and data related to the project for WSU CS 7850.

## About
The code in this repository serves to compare LINQ, a query tool built in to the .NET framework, to the Privacy INtegrated Queries (PINQ) prototype described at https://www.microsoft.com/en-us/research/project/privacy-integrated-queries-pinq/. Comparisons are made between syntax of the queries, how to replicate a LINQ query in PINQ, and the way that PINQ alters raw query results to ensure differential privacy is preserved.

## Running this Project
This code is distributed as a Visual Studio solution. To run, clone the entire project to your local computer, then open the LINQAgain solution in Visual Studio.

#### PINQ
This project depends on the PINQ prototype distributed at https://www.microsoft.com/en-us/download/details.aspx?id=52363. For convenience, this has been included with our code. This prototype was written by Frank McSherry, Microsoft Research, 2009.

#### Setting up SQL Server
This code also uses a SQL Server backend to serve as the database to query. You will need to download and install SQL Server to make this work. The Express version is fine. You can download this at https://www.microsoft.com/en-us/sql-server/sql-server-editions-express. You can then import the CSV file data distributed with this repository to the SQL Server. To do so:
1. Open the "SQL Server Import and Export Wizard"
2. Click "Next" past the first screen to get to "Choose a Data Source". From the dropdown, select "Flat File Source". Click "Browse..." to locate the CSV data on your computer. Check the box next to "Column names in the first data row".
3. Click "Next" through the rest of the wizard, accepting all defaults.

#### Testing
You should now have everything you need to run the project. Go back to Visual Studio and ensure that the project will run. Note that there are two files in the project with `main` methods defined.
* KMeansTests.cs: This file compares a standard k-means clustering algorithm to a differential privacy-preserving implementation. Running this file will produce a long output of test results.
* LINQvPINQTests.cs: This file does a comparison of LINQ queries with their corresponding PINQ counterparts. In this file, you can easily see how the syntax varies between these two systems. Running the file will produce a long output demonstrating how the query results differ between LINQ and PINQ.

You will need to comment out one of these `main` functions in order for the code to run, otherwise Visual Studio will not know the entry point for the code. (The KMeansTests `main` function is enabled by default.) Example run outputs can be found in the runs folder of this repository. You can compare your output to these examples to ensure the code is running properly.
