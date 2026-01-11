using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Services;
using Moq;
using Xunit;

namespace KontoApi.Tests;

public class StatementParserTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly StatementParser _parser;

    public StatementParserTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _parser = new StatementParser(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ParseLineAsync_WhenCategoryKnown_UsesIt()
    {
        // Arrange
        var line = "01.01.2024 GROCERIES 100.00 RUB";
        var expectedCategory = new Category("Groceries");
        
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string name, CancellationToken _) => 
                name.Contains("GROCERIES") ? expectedCategory : null);

        // Act
        var result = await _parser.ParseLineAsync(line, CancellationToken.None);

        // Assert
        Assert.Equal(expectedCategory.Id, result.CategoryId);
        // "RUB" is treated as part of the description because the parser only recognizes symbols (₽, $, €) as currency tokens, not "RUB" text.
        Assert.Equal("GROCERIES RUB", result.Description.Trim());
    }

    [Fact]
    public async Task ParseLineAsync_WhenCategoryUnknown_UsesUncategorized()
    {
        // Arrange
        var line = "01.01.2024 WEIRD_TRANSACTION 500.00 RUB";
        var uncategorized = new Category("Uncategorized");

        // Mock: First call (weird name) returns null
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("WEIRD_TRANSACTION", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Mock: Second call ("Uncategorized") returns the default category
        _categoryRepositoryMock
            .Setup(r => r.GetByNameAsync("Uncategorized", It.IsAny<CancellationToken>()))
            .ReturnsAsync(uncategorized);

        // Act
        var result = await _parser.ParseLineAsync(line, CancellationToken.None);

        // Assert
        Assert.Equal(uncategorized.Id, result.CategoryId);
        // Ensure it didn't use the garbage description as the category name lookup key effectively
        _categoryRepositoryMock.Verify(r => r.GetByNameAsync("Uncategorized", It.IsAny<CancellationToken>()), Times.Once);
    }
}
