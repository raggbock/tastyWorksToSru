namespace IbkrToSru;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using IbkrToSru.TastyModels;

public static class Csv
{
    public static ImmutableArray<Execution> ReadIbkr(string csv)
    {
        if (csv.Contains("Trades,Data,Order,Stocks,", StringComparison.Ordinal))
        {
            return ReadExecutions(csv, ',').ToImmutableArray();
        }

        if (csv.Contains("Trades\tData\tOrder\tStocks\t", StringComparison.Ordinal))
        {
            return ReadExecutions(csv, '\t').ToImmutableArray();
        }

        throw new FormatException("Expected comma or tab separated IBKR csv.");

        static IEnumerable<Execution> ReadExecutions(string csv, char terminator)
        {
            var position = 0;
            while (ReadLine(csv, out var line, ref position))
            {
                if (TryRead(line, terminator) is { } execution)
                {
                    yield return execution;
                }
            }

            static Execution? TryRead(ReadOnlySpan<char> line, char terminator)
            {
                // Trades,Header,DataDiscriminator,Asset Category,Currency,Symbol,Date/Time,Quantity,T. Price,C. Price,Proceeds,Comm/Fee,Basis,Realized P/L,MTM P/L,Code
                // Trades,Data,Order,Stocks,USD,APPS,\"2021-10-05, 10:46:40\",13,74.06,73.56,-962.78,-1,963.78,0,-6.5,O;P
                var position = 0;
                if (SkipKnown(line, "Trades", terminator, ref position) &&
                    SkipKnown(line, "Data", terminator, ref position) &&
                    SkipKnown(line, "Order", terminator, ref position) &&
                    SkipKnown(line, "Stocks", terminator, ref position))
                {
                    if (ReadString(line, terminator, out var currency, ref position) &&
                        ReadString(line, terminator, out var symbol, ref position) &&
                        ReadTime(line, terminator, out var time, ref position) &&
                        ReadDouble(line, terminator, out var quantity, ref position) &&
                        ReadDouble(line, terminator, out var price, ref position) &&
                        ReadDouble(line, terminator, out _, ref position) &&
                        ReadDouble(line, terminator, out var proceeds, ref position) &&
                        ReadDouble(line, terminator, out var fee, ref position) &&
                        ReadDouble(line, terminator, out var basis, ref position) &&
                        ReadDouble(line, terminator, out var pnl, ref position) &&
                        ReadString(line, terminator, out _, ref position))
                    {
                        return new Execution(
                            Currency: currency,
                            Symbol: symbol,
                            Time: time,
                            Quantity: (int)quantity,
                            Price: price,
                            Proceeds: proceeds,
                            Fee: fee,
                            Basis: basis,
                            Pnl: pnl);
                    }

                    throw new FormatException("Error reading execution");
                }

                return null;
            }
        }
    }

    public static ImmutableArray<Execution> ReadTastyTrade(string csvPath)
    {
        List<Execution> executions = new List<Execution>();
        using (var reader = new StreamReader(csvPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<TastyClassMap>();
            var records = csv.GetRecords<TastyCSV>();

            foreach (var record in records)
            {
                var execution = new Execution(
                    "USD",
                    record.SYMBOL,
                    record.OPEN_DATE,
                    record.QUANTITY,
                    DoubleParse(record.num1099_COST),
                    DoubleParse(record.num1099_PROCEEDS),
                    record.OPEN_DATE < new DateTime(2022, 02, 01) ? 1.12 * record.QUANTITY : 1.13 * record.QUANTITY,
                    DoubleParse(record.NO_WS_COST),
                    DoubleParse(record.TOTAL_GAINLOSS, false));

                executions.Add(execution);
            }
        }

        static double DoubleParse(string currencyString, bool removeMinus = true)
        {
            if (currencyString == null)
            {
                return 0;
            }

            // remove any non-numeric characters except for the decimal point
            currencyString = new string(currencyString.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
            double currencyValue = 0;

            // parse the string as a double

            if (double.TryParse(currencyString.Replace(".", ","), out currencyValue))
            {
                if (removeMinus)
                {
                    if (currencyString.StartsWith("-"))
                    {
                        currencyValue *= -1;
                    }
                }

                return currencyValue;
            }

            // check if the value was negative and adjust if necessary
            return 0;
        }

        return executions.ToImmutableArray();

        /*if (csv.Contains("TAX_YEAR,SUBLOT_ID,SECNO,CUSIP,SYMBOL,SEC_DESCR", StringComparison.Ordinal))
        {
            return ReadExecutions(csv, ',').ToImmutableArray();
        }

        static IEnumerable<Execution> ReadExecutions(string csv, char terminator)
        {
            var position = 0;
            while (ReadLine(csv, out var line, ref position))
            {
                if (TryRead(line, terminator) is { } execution)
                {
                    yield return execution;
                }
            }
        }

        static Execution? TryRead(ReadOnlySpan<char> line, char terminator)
        {
            // TAX_YEAR,SUBLOT_ID,SECNO,CUSIP,SYMBOL,SEC_DESCR,SEC_TYPE,SEC_SUBTYPE,SUBACCOUNT_TYPE,OPEN_TRAN_ID,CLOSE_TRAN_ID - SEQNO,OPEN_DATE,CLOSE_DATE,CLOSE_EVENT,DISPOSAL_METHOD,QUANTITY,LONG_SHORT_IND,NO_WS_COST,NO_WS_PROCEEDS,NO_WS_GAINLOSS,WS_COST_ADJ,WS_PROC_ADJ,WS_LOSS_ID - SEQNO,1099_ACQ_DATE,1099_DISP_DATE,1099_COST,1099_PROCEEDS,GROSS_NET_IND,TOTAL_GAINLOSS,ORDINARY_GAINLOSS,1099_DISALLOWED_LOSS,1099_MARKET_DISCOUNT,8949_GAINLOSS,8949_CODE,HOLDING_DATE,TERM,COVERED_IND,8949_BOX,1099_1256_CY_REALIZED,1099_1256_PY_UNREALIZED,1099_1256_CY_UNREALIZED,1099_1256_AGGREGATE
            // 2022,CS0000buFRNUVr8SeX,8SLRJQ1_R2022 - 02 - 01,"",AMC-- - 220128P00013500,PUT AMC    01 / 28 / 22    13.50 AMC ENTERTAINMENT HOLDINGS INC, Option, EquityOption, Cash,220128ELVSW4UP30YRRNOF000,220128ELILB06ZAWNM3UZB000 - 1,2022 - 01 - 28,2022 - 01 - 28,Sell,FIFO,1.00000000,L,$26.12,$11.87,-$14.25,$0.00,$0.00,,2022 - 01 - 28,2022 - 01 - 28,$26.12,$11.87,G,-$14.25,$0.00,$0.00,$0.00,-$14.25,,2022 - 01 - 28,ShortTerm,C,A,,,,
            var position = 0;
            if (ReadTime(line, terminator, out var time, ref position))
            {
                if (ReadString(line, terminator, out var action, ref position) &&
                    ReadInt(line, terminator, out var quantity, ref position) &&
                    ReadDouble(line, terminator, out var price, ref position) &&
                    ReadString(line, terminator, out var symbol, ref position) &&
                    ReadString(line, terminator, out _, ref position) &&
                    ReadString(line, terminator, out _, ref position))
                {
                    return new Execution(
                            Currency: currency,
                            Symbol: symbol,
                            Time: time,
                            Quantity: (int)quantity,
                            Price: price,
                            Proceeds: proceeds,
                            Fee: fee,
                            Basis: basis,
                            Pnl: pnl);
                }

                throw new FormatException("Error reading execution");
            }

            return null;
        }*/
    }

    public static ImmutableArray<BuyOrSell> ReadTradorvate(string csv)
    {
        if (csv.Contains("Timestamp,B/S,Quantity,Price,Contract,Product,Product Description", StringComparison.Ordinal))
        {
            return ReadExecutions(csv, ',').ToImmutableArray();
        }

        throw new FormatException("Expected comma or tab separated Tradorvate csv.");

        static IEnumerable<BuyOrSell> ReadExecutions(string csv, char terminator)
        {
            var position = 0;
            while (ReadLine(csv, out var line, ref position))
            {
                if (TryRead(line, terminator) is { } execution)
                {
                    yield return execution;
                }
            }

            static BuyOrSell? TryRead(ReadOnlySpan<char> line, char terminator)
            {
                // Timestamp,B/S,Quantity,Price,Contract,Product,Product Description
                // 2021-09-16 19:05, Sell,1,4449.00,ESZ1,ES,E-Mini S&P 500
                var position = 0;
                if (ReadTime(line, terminator, out var time, ref position))
                {
                    if (ReadString(line, terminator, out var action, ref position) &&
                        ReadInt(line, terminator, out var quantity, ref position) &&
                        ReadDouble(line, terminator, out var price, ref position) &&
                        ReadString(line, terminator, out var symbol, ref position) &&
                        ReadString(line, terminator, out _, ref position) &&
                        ReadString(line, terminator, out _, ref position))
                    {
                        return action switch
                        {
                            " Buy" => new BuyOrSell("USD", symbol, time, quantity, price),
                            " Sell" => new BuyOrSell("USD", symbol, time, -quantity, price),
                            _ => throw new NotSupportedException("Unknown action expected buy or sell."),
                        };
                    }

                    throw new FormatException("Error reading execution");
                }

                return null;
            }
        }
    }

    private static bool ReadLine(ReadOnlySpan<char> csv, out ReadOnlySpan<char> line, ref int position)
    {
        if (csv.IsEmpty ||
            position >= csv.Length - 1)
        {
            line = default;
            return false;
        }

        for (var i = position; i < csv.Length; i++)
        {
            switch (csv[i])
            {
                case '\r'
                    when i < csv.Length - 1 &&
                         csv[i + 1] == '\n':
                    line = csv[position..i];
                    position = i + 2;
                    return true;
                case '\r':
                    line = csv[position..i];
                    position = i + 1;
                    return true;
                case '\n':
                    line = csv[position..i];
                    position = i + 1;
                    return true;
            }
        }

        line = csv[position..];
        position = csv.Length;
        return true;
    }

    private static bool ReadInt(ReadOnlySpan<char> csv, char terminator, out int result, ref int position) =>
        int.TryParse(ValueSpan(csv, terminator, ref position), NumberStyles.Number, CultureInfo.InvariantCulture, out result);

    private static bool ReadDouble(ReadOnlySpan<char> csv, char terminator, out double result, ref int position) =>
        double.TryParse(ValueSpan(csv, terminator, ref position), NumberStyles.Number, CultureInfo.InvariantCulture, out result);

    private static bool ReadString(ReadOnlySpan<char> csv, char terminator, [NotNullWhen(true)] out string? result, ref int position)
    {
        result = ValueSpan(csv, terminator, ref position).ToString();
        return true;
    }

    private static bool ReadTime(ReadOnlySpan<char> csv, char terminator, out DateTime result, ref int position) =>
        DateTime.TryParse(ValueSpan(csv, terminator, ref position), out result);

    private static bool SkipKnown(ReadOnlySpan<char> csv, string text, char terminator, ref int position) =>
        ValueSpan(csv, terminator, ref position).Equals(text, StringComparison.Ordinal);

    private static void Skip(ReadOnlySpan<char> csv, char c, ref int position)
    {
        while (position < csv.Length)
        {
            if (csv[position] != c)
            {
                return;
            }

            position++;
        }
    }

    private static ReadOnlySpan<char> ValueSpan(ReadOnlySpan<char> csv, char terminator, ref int position)
    {
        var temp = position;
        if (ReadQuoted(csv, terminator, out var inner, ref temp))
        {
            position = temp;
            return inner;
        }

        for (var i = position; i < csv.Length; i++)
        {
            if (csv[i] == terminator)
            {
                var result = csv[position..i];
                position = i + 1;
                if (terminator == ' ')
                {
                    Skip(csv, ' ', ref position);
                }

                return result;
            }

            if (i == csv.Length - 1)
            {
                var result = csv[position..];
                position = csv.Length;
                return result;
            }
        }

        throw new FormatException("Could not read value at position");

        static bool ReadQuoted(ReadOnlySpan<char> csv, char terminator, out ReadOnlySpan<char> inner, ref int position)
        {
            if (position < csv.Length &&
                csv[position] == '"')
            {
                for (var i = position + 1; i < csv.Length; i++)
                {
                    if (csv[i] == '"' &&
                        i < csv.Length - 1 &&
                        csv[i + 1] == terminator)
                    {
                        inner = csv[(position + 1)..i];
                        position = i + 2;
                        if (terminator == ' ')
                        {
                            Skip(csv, ' ', ref position);
                        }

                        return true;
                    }
                }
            }

            inner = default;
            return false;
        }
    }
}
