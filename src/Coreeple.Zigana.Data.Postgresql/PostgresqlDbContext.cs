using Coreeple.Zigana.Data.Abstractions;
using Dapper;
using Npgsql;
using System.Data;

namespace Coreeple.Zigana.Data.Postgresql;
public class PostgresqlDbContext(string connectionString, string schema = "public") : IDbContext
{
    public IDbConnection CreateConnection() => new NpgsqlConnection(connectionString);

    public void Migration()
    {
        using var connection = CreateConnection();

        var sql = $"""
            CREATE SCHEMA IF NOT EXISTS {schema};
        
            CREATE TABLE IF NOT EXISTS {schema}."Apis"
            (
                "Id" uuid NOT NULL,
                "Path" character varying COLLATE pg_catalog."default" NOT NULL,
                "Name" character varying COLLATE pg_catalog."default",
                "Description" character varying COLLATE pg_catalog."default",
                "Defs" json,
                "IsActive" boolean NOT NULL DEFAULT false,
                CONSTRAINT "Apis_pkey" PRIMARY KEY ("Id"),
                CONSTRAINT "Apis_Path_key" UNIQUE ("Path")
            );

            CREATE TABLE IF NOT EXISTS {schema}."Endpoints"
            (
                "Id" uuid NOT NULL,
                "ApiId" uuid NOT NULL,
                "Path" character varying COLLATE pg_catalog."default" NOT NULL,
                "Actions" json,
                "Response" json,
                "Method" character varying COLLATE pg_catalog."default" NOT NULL,
                "IsActive" boolean NOT NULL DEFAULT false,
                CONSTRAINT "Endpoints_pkey" PRIMARY KEY ("Id"),
                CONSTRAINT "Endpoints_Path_Method_key" UNIQUE ("Path", "Method")
            );

            CREATE TABLE IF NOT EXISTS {schema}."EndpointRequestTransactions"
            (
                "Id" uuid NOT NULL,
                "EndpointId" uuid NOT NULL,
                "Name" character varying COLLATE pg_catalog."default" NOT NULL,
                "Status" character varying COLLATE pg_catalog."default" NOT NULL,
                "Date" timestamp without time zone NOT NULL,
                "Message" text COLLATE pg_catalog."default",
                CONSTRAINT "EndpointRequestTransactions_pkey" PRIMARY KEY ("Id", "EndpointId", "Name")
            );
        """;

        connection.Execute(sql);
    }
}
