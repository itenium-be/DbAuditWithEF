using System.Data.Common;
using System.Data;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace DbAuditWithEF;

public class TestContainersTests
{
    [Fact]
    public async Task ConnectionStateReturnsOpen()
    {
        var mssqlContainer = new MsSqlBuilder()
            .WithPassword("yourStrong(!)Password")
            .Build();

        await mssqlContainer.StartAsync();
        await using DbConnection connection = new SqlConnection(mssqlContainer.GetConnectionString());

        await connection.OpenAsync();

        Assert.Equal(ConnectionState.Open, connection.State);

        await mssqlContainer.DisposeAsync();
    }

    //[Fact]
    //public async Task ExecScriptReturnsSuccessful()
    //{
    //    const string scriptContent = "SELECT 1;";

    //    var execResult = await msSqlContainer.ExecScriptAsync(scriptContent)
    //        .ConfigureAwait(true);

    //    Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    //    Assert.Empty(execResult.Stderr);
    //}
}
