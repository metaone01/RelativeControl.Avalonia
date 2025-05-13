using System;
using System.Globalization;
using Avalonia.Utilities;
using static System.Char;

namespace RelativeControl.Avalonia;

// Copied from Avalonia
internal ref struct SpanStringTokenizer {
    private const char DefaultSeparatorChar = ',';

    private readonly ReadOnlySpan<char> _s;
    private readonly int _length;
    private readonly char _separator;
    private readonly string? _exceptionMessage;
    private readonly IFormatProvider _formatProvider;
    private int _index;
    private int _tokenLength;

    public SpanStringTokenizer(string s, IFormatProvider formatProvider, string? exceptionMessage = null) : this(
        s.AsSpan(),
        GetSeparatorFromFormatProvider(formatProvider),
        exceptionMessage) {
        _formatProvider = formatProvider;
    }

    public SpanStringTokenizer(string s, char separator = DefaultSeparatorChar, string? exceptionMessage = null) : this(
        s.AsSpan(),
        separator,
        exceptionMessage) { }

    public SpanStringTokenizer(ReadOnlySpan<char> s, IFormatProvider formatProvider, string? exceptionMessage = null) :
        this(s, GetSeparatorFromFormatProvider(formatProvider), exceptionMessage) {
        _formatProvider = formatProvider;
    }

    public SpanStringTokenizer(
        ReadOnlySpan<char> s,
        char separator = DefaultSeparatorChar,
        string? exceptionMessage = null) {
        _s                = s;
        _length           = s.Length;
        _separator        = separator;
        _exceptionMessage = exceptionMessage;
        _formatProvider   = CultureInfo.InvariantCulture;
        _index            = 0;
        CurrentTokenIndex = -1;
        _tokenLength      = 0;

        while (_index < _length && IsWhiteSpace(_s[_index]))
            _index++;
    }

    public int CurrentTokenIndex { get; private set; }

    public string? CurrentToken => CurrentTokenIndex < 0 ? null : _s.Slice(CurrentTokenIndex, _tokenLength).ToString();

    public ReadOnlySpan<char> CurrentTokenSpan =>
        CurrentTokenIndex < 0 ? ReadOnlySpan<char>.Empty : _s.Slice(CurrentTokenIndex, _tokenLength);

    public void Dispose() {
        if (_index != _length)
            throw GetFormatException();
    }

    public bool TryReadDouble(out double result, char? separator = null) {
        if (TryReadSpan(out ReadOnlySpan<char> stringResult, separator) &&
            stringResult.TryParseDouble(NumberStyles.Float, _formatProvider, out result))
            return true;

        result = 0;
        return false;
    }

    public double ReadDouble(char? separator = null) {
        if (!TryReadDouble(out double result, separator))
            throw GetFormatException();

        return result;
    }

    public bool TryReadSpan(out ReadOnlySpan<char> result, char? separator = null) {
        bool success = TryReadToken(separator ?? _separator);
        result = CurrentTokenSpan;
        return success;
    }

    public ReadOnlySpan<char> ReadSpan(char? separator = null) {
        if (!TryReadSpan(out ReadOnlySpan<char> result, separator))
            throw GetFormatException();

        return result;
    }

    private bool TryReadToken(char separator) {
        CurrentTokenIndex = -1;

        if (_index >= _length)
            return false;

        int index  = _index;
        var length = 0;

        while (_index < _length) {
            char c = _s[_index];

            if (IsWhiteSpace(c) || c == separator)
                break;

            _index++;
            length++;
        }

        SkipToNextToken(separator);

        CurrentTokenIndex = index;
        _tokenLength      = length;

        if (_tokenLength < 1)
            throw GetFormatException();

        return true;
    }

    private void SkipToNextToken(char separator) {
        if (_index >= _length)
            return;
        char c = _s[_index];

        if (c != separator && !IsWhiteSpace(c))
            throw GetFormatException();

        var length = 0;

        while (_index < _length) {
            c = _s[_index];

            if (c == separator) {
                length++;
                _index++;

                if (length > 1)
                    throw GetFormatException();
            } else {
                if (!IsWhiteSpace(c))
                    break;

                _index++;
            }
        }

        if (length > 0 && _index >= _length)
            throw GetFormatException();
    }

    private FormatException GetFormatException() {
        return _exceptionMessage != null ? new FormatException(_exceptionMessage) : new FormatException();
    }

    private static char GetSeparatorFromFormatProvider(IFormatProvider provider) {
        char c = DefaultSeparatorChar;

        NumberFormatInfo formatInfo = NumberFormatInfo.GetInstance(provider);
        if (formatInfo.NumberDecimalSeparator.Length > 0 && c == formatInfo.NumberDecimalSeparator[0])
            c = ';';

        return c;
    }
}