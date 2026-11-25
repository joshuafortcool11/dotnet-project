using HealthGuardRwanda.Core.Controllers;
using HealthGuardRwanda.Desktop.Features;

namespace HealthGuardRwanda.Desktop.Views;

public sealed class FeaturePanel : UserControl
{
    private readonly Func<string, List<(Guid Id, string Label)>> _lookup;
    private readonly Label _title = new();
    private readonly Label _description = new();
    private readonly TableLayoutPanel _inputs = new();
    private readonly Button _execute = new();
    private readonly Label _message = new();
    private readonly DataGridView _grid = new();
    private readonly Dictionary<string, TextBox> _boxes = new();
    private Feature? _feature;

    public FeaturePanel(Func<string, List<(Guid Id, string Label)>> lookup)
    {
        _lookup = lookup;

        Dock = DockStyle.Fill;
        Padding = new Padding(14);

        _title.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        _title.Dock = DockStyle.Top;
        _title.Height = 30;

        _description.Dock = DockStyle.Top;
        _description.Height = 24;
        _description.ForeColor = Color.DimGray;

        _inputs.Dock = DockStyle.Top;
        _inputs.AutoSize = true;
        _inputs.ColumnCount = 3;
        _inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210));
        _inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 420));
        _inputs.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));

        _execute.Text = "Execute requirement";
        _execute.Height = 38;
        _execute.Width = 190;
        _execute.Dock = DockStyle.Top;
        _execute.BackColor = Color.FromArgb(0, 105, 148);
        _execute.ForeColor = Color.White;
        _execute.FlatStyle = FlatStyle.Flat;
        _execute.Click += ExecuteClick;

        _message.Dock = DockStyle.Top;
        _message.Height = 42;
        _message.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        _message.Padding = new Padding(0, 10, 0, 0);

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 56 };
        bottom.Controls.Add(_message);
        var middle = new Panel { Dock = DockStyle.Top, Height = 60 };
        middle.Controls.Add(_execute);

        Controls.Add(_grid);
        Controls.Add(middle);
        Controls.Add(bottom);

        var header = new Panel { Dock = DockStyle.Top, Height = 62 };
        header.Controls.Add(_description);
        header.Controls.Add(_title);
        _description.Top = 32;
        Controls.Add(header);

        var spacer = new Panel { Dock = DockStyle.Top, Height = 8 };
        Controls.Add(spacer);
        Controls.Add(_inputs);
    }

    public void ShowFeature(Feature feature)
    {
        _feature = feature;
        _title.Text = $"{feature.Id}  -  {feature.Title}";
        _description.Text = feature.Description;
        _boxes.Clear();
        _inputs.Controls.Clear();
        _inputs.RowCount = feature.Parameters.Count;
        _inputs.RowStyles.Clear();

        for (var i = 0; i < feature.Parameters.Count; i++)
        {
            var parameter = feature.Parameters[i];
            _inputs.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            var label = new Label { Text = parameter.Label, TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill };
            var box = new TextBox { Text = parameter.DefaultValue, Dock = DockStyle.Fill, Font = new Font("Consolas", 10) };
            _boxes[parameter.Name] = box;
            _inputs.Controls.Add(label, 0, i);
            _inputs.Controls.Add(box, 1, i);

            if (parameter.Picker is not null)
            {
                var browse = new Button { Text = "Browse...", Dock = DockStyle.Fill };
                var picker = parameter.Picker;
                browse.Click += (_, _) =>
                {
                    using var dialog = new PickerForm($"Choose {parameter.Label}", _lookup(picker));
                    if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedId is not null)
                        box.Text = dialog.SelectedId;
                };
                _inputs.Controls.Add(browse, 2, i);
            }
        }

        _grid.DataSource = null;
        _message.Text = "Ready. Fill the parameters and press Execute.";
        _message.ForeColor = Color.DimGray;
    }

    private void ExecuteClick(object? sender, EventArgs e)
    {
        if (_feature is null)
            return;

        var parameters = _boxes.ToDictionary(pair => pair.Key, pair => pair.Value.Text);
        ServiceResult result;
        try
        {
            result = _feature.Execute(parameters);
        }
        catch (Exception exception)
        {
            _message.Text = $"ERROR: {exception.Message}";
            _message.ForeColor = Color.OrangeRed;
            return;
        }

        var statusText = result.Success ? $"{result.Status} OK" : $"{result.Status} ERROR";
        _message.Text = string.IsNullOrEmpty(result.Message)
            ? statusText
            : $"{statusText}  -  {result.Message}";
        _message.ForeColor = result.Success ? Color.SeaGreen : Color.Crimson;

        _grid.DataSource = GridFormatter.ToTable(result.Data, out var extra);
        if (!string.IsNullOrEmpty(extra))
            _message.Text += "   |   " + extra;
    }
}
