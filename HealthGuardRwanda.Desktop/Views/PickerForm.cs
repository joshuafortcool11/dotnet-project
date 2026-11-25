namespace HealthGuardRwanda.Desktop.Views;

public sealed class PickerForm : Form
{
    private readonly ListBox _list;

    public PickerForm(string title, List<(Guid Id, string Label)> items)
    {
        Text = title;
        Width = 620;
        Height = 420;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;

        _list = new ListBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Consolas", 10)
        };
        foreach (var item in items)
            _list.Items.Add(item);
        if (_list.Items.Count > 0)
            _list.SelectedIndex = 0;

        var ok = new Button { Text = "Use selected id", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 36 };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Bottom, Height = 36 };

        Controls.Add(_list);
        Controls.Add(ok);
        Controls.Add(cancel);
        AcceptButton = ok;
        CancelButton = cancel;
    }

    public string? SelectedId => (_list.SelectedItem is ValueTuple<Guid, string> pair)
        ? pair.Item1.ToString()
        : null;
}
