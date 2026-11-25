using HealthGuardRwanda.Core.Controllers;
using HealthGuardRwanda.Core.Models;
using HealthGuardRwanda.Desktop.Features;

namespace HealthGuardRwanda.Desktop.Views;

public sealed class MainForm : Form
{
    private readonly FeaturePanel _panel;
    private readonly TreeView _tree = new();
    private readonly EnrollmentOfficerController _officers;
    private readonly CitizenController _citizens;
    private readonly HouseholdCategoryController _categories;
    private readonly EnrollmentController _enrollments;
    private readonly CoverageVerificationController _verifications;
    private readonly SeedController _seed;

    public MainForm(
        List<Feature> features,
        EnrollmentOfficerController officers,
        CitizenController citizens,
        HouseholdCategoryController categories,
        EnrollmentController enrollments,
        CoverageVerificationController verifications,
        SeedController seed,
        string connectionInfo)
    {
        _officers = officers;
        _citizens = citizens;
        _categories = categories;
        _enrollments = enrollments;
        _verifications = verifications;
        _seed = seed;

        Text = "HealthGuard Rwanda - Backend + WinForms MVC";
        Width = 1250;
        Height = 800;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 620);

        _panel = new FeaturePanel(Lookup);

        _tree.Dock = DockStyle.Left;
        _tree.Width = 320;
        _tree.Font = new Font("Segoe UI", 10);
        _tree.HideSelection = false;
        _tree.AfterSelect += (_, args) =>
        {
            if (args.Node?.Tag is Feature feature)
                _panel.ShowFeature(feature);
        };

        foreach (var group in features.GroupBy(f => f.Group))
        {
            var groupNode = new TreeNode(group.Key);
            foreach (var feature in group)
                groupNode.Nodes.Add(new TreeNode($"{feature.Id}  {feature.Title}") { Tag = feature });
            _tree.Nodes.Add(groupNode);
            groupNode.Expand();
        }
        if (_tree.Nodes.Count > 0 && _tree.Nodes[0].Nodes.Count > 0)
        {
            _tree.SelectedNode = _tree.Nodes[0].Nodes[0];
            if (_tree.SelectedNode?.Tag is Feature first)
                _panel.ShowFeature(first);
        }

        var header = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.FromArgb(240, 244, 248) };
        var title = new Label
        {
            Text = "HealthGuard Rwanda  |  AUCA Web Technology  |  Iradukunda Joshua - 27967",
            Dock = DockStyle.Left,
            Width = 700,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            Padding = new Padding(14, 0, 0, 0)
        };
        var seedButton = new Button
        {
            Text = "Load seed data",
            Dock = DockStyle.Right,
            Width = 140,
            Height = 36,
            BackColor = Color.SeaGreen,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        seedButton.Click += (_, _) =>
        {
            var result = _seed.Load();
            MessageBox.Show(result.Message, $"Seed data ({result.Status})",
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        };
        header.Controls.Add(title);
        header.Controls.Add(seedButton);

        var status = new StatusStrip();
        status.Items.Add(new ToolStripStatusLabel(connectionInfo));

        Controls.Add(_panel);
        Controls.Add(_tree);
        Controls.Add(header);
        Controls.Add(status);
    }

    private List<(Guid Id, string Label)> Lookup(string picker)
    {
        return picker switch
        {
            "citizen" => Items<Citizen>(_citizens.GetAll(), c => $"{c.FirstName} {c.LastName} | {c.NationalId} | {c.Village}"),
            "enrollmentOfficer" => Items<EnrollmentOfficer>(_officers.GetAll(), o => $"{o.FirstName} {o.LastName} | {o.Email}"),
            "householdCategory" => Items<HouseholdCategory>(_categories.GetAll(), c => c.Name),
            "enrollment" => Items<Enrollment>(_enrollments.GetAll(), e => $"{e.EnrollmentDate:yyyy-MM-dd} | {e.Status} | {e.Citizen.NationalId}"),
            "coveredEnrollment" => Items<Enrollment>(_enrollments.ByStatus(EnrollmentStatus.COVERED), e => $"{e.EnrollmentDate:yyyy-MM-dd} | {e.Status} | {e.Citizen.NationalId}"),
            "coverageVerification" => Items<CoverageVerification>(_verifications.GetAll(), v => $"{v.HealthCenter} | {v.VerifiedDate:yyyy-MM-dd}"),
            _ => new List<(Guid Id, string Label)>()
        };
    }

    private static List<(Guid Id, string Label)> Items<T>(ServiceResult result, Func<T, string> label)
    {
        if (!result.Success || result.Data is not List<T> list)
            return new List<(Guid Id, string Label)>();

        return list.Select(item =>
        {
            var id = (Guid)item!.GetType().GetProperty("Id")!.GetValue(item)!;
            return (id, label(item));
        }).ToList();
    }
}
