namespace APDL.Tests;

public class UnitTest1
{
    [Fact]
    public void SimpleTest_ShouldPass()
    {
        // Arrange
        var expected = "healthy";

        // Act
        var actual = "healthy";

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SQLite_ConnectionString_Should_Contain_DbName()
    {
        // Arrange
        var connectionString = "Data Source=apdl.db";

        // Assert
        Assert.Contains("apdl", connectionString);
    }
}
