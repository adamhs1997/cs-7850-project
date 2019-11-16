# CS 7850 Project: Comparison of PINQ to LINQ
Code and data related to the project for WSU CS 7850.

## About
The code in this repository serves to compare LINQ, a query tool built in to the .NET framework, to the Privacy INtegrated Queries (PINQ) prototype described at https://www.microsoft.com/en-us/research/project/privacy-integrated-queries-pinq/. Comparisons are made between syntax of the queries, how to replicate a LINQ query in PINQ, and the way that PINQ alters raw query results to ensure differential privacy is preserved.

## Running this Project
This code is distributed as a Visual Studio solution. To run, clone the entire project to your local computer, then open the solution in Visual Studio.

#### 1. Adding PINQ
This project depends on the PINQ prototype distributed at https://www.microsoft.com/en-us/download/details.aspx?id=52363. To run our code, you will need to download this project and set the code we distribute from this repository to reference it. To do so, do the following:
1. Unzip the contents of the PINQ file you downloaded
2. Open the LINQAgain solution (the code from this repository) in Visual Studio.
3. There are several projects included in the PINQ download. Our code depends on the PINQ project and the MachineLearning project. Add a reference to both of these projects from the LINQAgain project in Visual Studio. To do this, right-click the name of the solution in Visual Studio Solution Explorer. Choose Add->Existing Project. (See below.) Navigate to the directory where you unzipped the PINQ download. Open the PINQ project, and choose the PINQ.csproj file to add. You may be prompted to do a one-way upgrade on the project file to make this work. Repeat this process to add the MachineLearning.csproj file.
![image](https://user-images.githubusercontent.com/25497193/68962946-cdadbb00-07a3-11ea-9e74-ec4dcbb1f86f.png)
4. So the LINQAgain project can use code from the PINQ and MachineLearning projects, we must add a reference to them in the LINQAgain project. To do so, right-click the LINQAgain project *(not the solution)*, then choose Add->Reference. Check the boxes next to both PINQ and MachineLearning. See screenshots below.
*Opening reference window*
![image](https://user-images.githubusercontent.com/25497193/68996151-45431f00-0864-11ea-95c1-d13c3c30a060.png)
*Referencing the projects*
![image](https://user-images.githubusercontent.com/25497193/68996166-850a0680-0864-11ea-8594-8483e9222775.png)

#### 2. Setting up SQL Server
This code also uses a SQL Server backend to serve as the database to query. You will need to download and install SQL Server to make this work. The Express version is fine. You can download this at https://www.microsoft.com/en-us/sql-server/sql-server-editions-express. You can then import the CSV file data distributed with this repository to the SQL Server. To do so:
1. Open the "SQL Server Import and Export Wizard"
2. Click "Next" past the first screen to get to "Choose a Data Source". From the dropdown, select "Flat File Source". Click "Browse..." to locate the CSV data on your computer.
3. Click "Next" through the rest of the wizard, accepting all defaults.

You should now have everything you need to run the project. Go back to Visual Studio and ensure that the project will run. Note that there are two files in the project with `main` methods defined.
* LINQvPINQTests.cs: This file does a comparison of LINQ queries with their corresponding PINQ counterparts. In this file, you can easily see how the syntax varies between these two systems. Running the file will produce a long output demonstrating how the query results differ between LINQ and PINQ.
* KMeansTests.cs: This file compares a standard k-means clustering algorithm to a differential privacy-preserving implementation. Running this file will produce a long output of test results.
You will need to comment out one of these `main` functions in order for the code to run, otherwise Visual Studio will not know the entry point for the code. Example run outputs can be found in the runs folder of this repository. You can compare your output to these examples to ensure the code is running properly.
