using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace HeadlessTest.RelativeControl;

public class Container : Button {
    private bool _isTemplateApplied;

    public static readonly DirectProperty<Container, bool> IsTemplateAppliedProperty = AvaloniaProperty.RegisterDirect<Container, bool>(
        nameof(IsTemplateApplied),
        o => o.IsTemplateApplied,
        (o, v) => o.IsTemplateApplied = v);

    public bool IsTemplateApplied {
        get => _isTemplateApplied;
        private set => SetAndRaise(IsTemplateAppliedProperty, ref _isTemplateApplied, value);
    }

    public Container() {
        TemplateApplied += OnTemplateApplied;
    }

    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) {
        IsTemplateApplied = true;
    }
}