using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Common.Models;
using KontoApi.Domain;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using KontoApi.Application.Features.Categories.Queries.GetCategoryById;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KontoApi.Tests")]

namespace KontoApi.Infrastructure.Services;


public partial class StatementParser(ICategoryRepository categoryRepository) : IStatementParser
{
    private static readonly Regex DateRegex = MyDateRegex();
    private static readonly Regex AmountRegex = MyAmountRegex();
    private static readonly Regex CurrencyRegex = MyCurrencyRegex();

    public bool Supports(string fileName)
        => fileName.EndsWith(".csv") || fileName.EndsWith(".pdf");

    public async Task<List<ParsedTransaction>> ParseAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        var transactions = new List<ParsedTransaction>();

        using var pdf = PdfDocument.Open(fileStream);
        foreach (var page in pdf.GetPages())
        {
            var lines = GetLines(page);

            foreach (var line in lines)
            {
                var transaction = await ParseLineAsync(line, cancellationToken);
                if (transaction.Amount != 0)
                {
                    transactions.Add(transaction);
                }
            }
        }
        return transactions;
    }

    private static List<string> GetLines(Page page)
    {
        var words = page.GetWords().ToList();
        if (words.Count == 0) return [];

        var lines = new List<List<Word>>();
        var sortedWords = words.OrderByDescending(w => w.BoundingBox.Bottom).ToList();

        var currentLine = new List<Word> { sortedWords[0] };
        lines.Add(currentLine);

        var lastY = sortedWords[0].BoundingBox.Bottom;

        for (var i = 1; i < sortedWords.Count; i++)
        {
            var word = sortedWords[i];

            if (Math.Abs(word.BoundingBox.Bottom - lastY) < 5)
                currentLine.Add(word);
            else
            {
                currentLine = [word];
                lines.Add(currentLine);
                lastY = word.BoundingBox.Bottom;
            }
        }

        return lines
            .Select(line => string
            .Join(" ", line.OrderBy(w => w.BoundingBox.Left)
            .Select(w => w.Text)))
            .ToList();
    }

    internal async Task<ParsedTransaction> ParseLineAsync(string line, CancellationToken cancellationToken)
    {
        var dateTime = DateTime.MinValue;
        decimal amount = 0;
        var currency = "RUB";
        var parsedDescription = "";
        var transactionType = TransactionType.Expense;
        var ruCulture = CultureInfo.GetCultureInfo("ru-RU");

        var dateMatch = DateRegex.Match(line);
        if (!dateMatch.Success)
        {
            return new ParsedTransaction(default, 0, "RUB", TransactionType.Expense, null, Guid.Empty);
        }

        var formats = new[] { "dd.MM.yyyy", "dd.MM.yy" };
        if (!DateTime.TryParseExact(dateMatch.Value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return new ParsedTransaction(default, 0, "RUB", TransactionType.Expense, null, Guid.Empty);
        }
        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        var amountMatches = MyRegex().Matches(line);
        foreach (Match match in amountMatches)
        {
            var rawAmount = match.Value;
            var cleanAmount = rawAmount.Replace(" ", "").Replace("\u00A0", "").Replace(",", ".");
            if (decimal.TryParse(cleanAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                amount = parsed;
                break;
            }
        }

        if (line.Contains('$') || line.Contains("USD")) currency = "USD";
        else if (line.Contains('€') || line.Contains("EUR")) currency = "EUR";

        var textProcessing = line.Replace(dateMatch.Value, "");
        foreach (Match match in amountMatches)
        {
            textProcessing = textProcessing.Replace(match.Value, "");
        }
        textProcessing = Regex.Replace(textProcessing, @"[\$\u20bd\u20ac]", "");

        parsedDescription = Regex.Replace(textProcessing, @"\s+", " ").Trim();

        if (line.Contains('+') || parsedDescription.Contains("Пополнение", StringComparison.OrdinalIgnoreCase))
            transactionType = TransactionType.Income;
        else if (parsedDescription.Contains("Перевод", StringComparison.OrdinalIgnoreCase))
            transactionType = TransactionType.Transfer;
        else
            transactionType = TransactionType.Expense;

        if (string.IsNullOrWhiteSpace(parsedDescription))
            parsedDescription = "Uncategorized";

        var categoryName = "";
        if (await categoryRepository.ExistsByNameAsync(parsedDescription, cancellationToken))
        {
            categoryName = parsedDescription;
        }
        else
        {
            categoryName = "Прочие расходы";
        }

        var category = await categoryRepository.GetByNameAsync(categoryName, cancellationToken);
        var categoryId = category.Id;

        return new ParsedTransaction(
            dateTime,
            amount,
            currency,
            transactionType,
            parsedDescription,
            categoryId
        );
    }

    [GeneratedRegex(@"(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.(\d{4}|\d{2})", RegexOptions.Compiled)]
    private static partial Regex MyDateRegex();

    [GeneratedRegex(@"-?\d+([.,]\d+)?", RegexOptions.Compiled)]
    private static partial Regex MyAmountRegex();

    [GeneratedRegex(@"-?\d+([.,]\d+)?\s*[\u20BD\u0024\u20AC]", RegexOptions.Compiled)]
    private static partial Regex MyCurrencyRegex();

    [GeneratedRegex(@"(?<=\s|^)[-+]?[\d\s]*\d[.,]\d{2}(?=\s|$)")]
    private static partial Regex MyRegex();
}
