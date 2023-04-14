--Create database
CREATE DATABASE TransactionsDB;
 
--Create table for transactions
CREATE TABLE TransactionsDB.dbo.Transactions (
id INT primary key IDENTITY (1,1),
TransactionID varchar(50) NOT NULL,
Amount money NOT NULL,
CurrencyCode varchar(20) NOT NULL,
Status  varchar(20) NOT NULL,
TransactionDate datetime NOT NULL,
CreatedDate datetime NOT NULL)

