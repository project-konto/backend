using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Common.Models;
using KontoApi.Domain;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using KontoApi.Application.Features.Categories.Queries.GetCategoryById;

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

    private async Task<ParsedTransaction> ParseLineAsync(string line, CancellationToken cancellationToken)
    {
        const string formatData = "dd.MM.yyyy";
        var amountExist = false;
        var lineWithInfo = false;
        var parts = line.Split(' ');
        var dateTime = DateTime.MinValue;
        decimal amount = 0;
        var currency = "RUB";
        var description = "";
        var transactionType = TransactionType.Income;
        var ruCulture = CultureInfo.GetCultureInfo("ru-RU");


        foreach (var part in parts)
        {
            if (Regex.IsMatch(part, DateRegex.ToString()))
            {
                dateTime = DateTime.ParseExact(part, formatData, CultureInfo.InvariantCulture);
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                lineWithInfo = true;
            }

            if (lineWithInfo == false)
                break;

            var amountMatches = MyRegex().Matches(line);
            foreach (Match match in amountMatches)
            {
                var rawAmount = match.Value;
                var cleanAmount = rawAmount.Replace(" ", "").Replace("\u00A0", "");

                if (!decimal.TryParse(cleanAmount, NumberStyles.Number, ruCulture, out var parsedAmount)) continue;
                amount = parsedAmount;
                break;
            }

            if (Regex.IsMatch(part, CurrencyRegex.ToString()))
            {
                currency = part switch
                {
                    "\u20bd" => "RUB",
                    "\u0024" => "USD",
                    "\u20AC" => "EUR",
                    _ => currency
                };
            }

            if (Regex.IsMatch(part, @"\p{L}"))
            {
                description += $" {part}";
            }

            if (line.Contains('+') && !description.Contains("Перевод")) transactionType = TransactionType.Income;
            else if (description.Contains("Перевод")) transactionType = TransactionType.Transfer;
            else transactionType = TransactionType.Expense;
        }

        description = description.Trim();
        if (string.IsNullOrWhiteSpace(description))
            description = "Uncategorized";

        var category = await categoryRepository.GetByNameAsync(description, cancellationToken);
        var categoryId = category.Id;

        var transaction = new ParsedTransaction(
            dateTime,
            amount,
            currency,
            transactionType,
            description,
            categoryId
        );

        return transaction;
    }

    [GeneratedRegex(@"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4}$", RegexOptions.Compiled)]
    private static partial Regex MyDateRegex();
    [GeneratedRegex(@"^-?\d+([.,]\d+)?$", RegexOptions.Compiled)]
    private static partial Regex MyAmountRegex();
    [GeneratedRegex(@"^-?\d+([.,]\d+)?\s*[\u20BD\u0024\u20AC]$", RegexOptions.Compiled)]
    private static partial Regex MyCurrencyRegex();
    [GeneratedRegex(@"(?<=\s|^)[-+]?[\d\s]*\d,\d{2}(?=\s|$)")]
    private static partial Regex MyRegex();
}
