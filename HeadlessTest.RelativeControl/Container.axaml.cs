using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace HeadlessTest.RelativeControl;

public class Container : Button {
    public static readonly DirectProperty<Container, bool> IsTemplateAppliedProperty =
        AvaloniaProperty.RegisterDirect<Container, bool>(
            nameof(IsTemplateApplied),
            o => o.IsTemplateApplied,
            (o, v) => o.IsTemplateApplied = v);

    private bool _isTemplateApplied;

    public Container() { TemplateApplied += OnTemplateApplied; }

    public bool IsTemplateApplied {
        get => _isTemplateApplied;
        private set => SetAndRaise(IsTemplateAppliedProperty, ref _isTemplateApplied, value);
    }

    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) { IsTemplateApplied = true; }
}