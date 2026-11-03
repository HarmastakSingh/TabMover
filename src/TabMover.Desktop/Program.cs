using System.Drawing;
using System.Text.Json;

namespace TabMover;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}

internal sealed class AppSettings
{
    public bool CloseSourceAfterVerifiedTransfer { get; set; } = true;
    public bool ShowTransferNotifications { get; set; } = true;
    public bool StartWithWindows { get; set; }
    public bool EnableDragOverlay { get; set; } = true;
    public bool AllowSessionStateTransfer { get; set; } = true;
    public bool IncludeCookies { get; set; } = true;
    public bool IncludeLocalStorage { get; set; } = true;
    public bool IncludeSessionStorage { get; set; } = true;
    public bool IncludeScrollPosition { get; set; } = true;
    public bool IncludeFormState { get; set; }
    public bool AllowPrivateWindows { get; set; }
    public string TabPlacement { get; set; } = "Next to active tab";
}

internal sealed class MainForm : Form
{
    private readonly string _settingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "TabMover", "settings.json");
    private AppSettings _settings = new();
    private readonly NotifyIcon _tray;
    private readonly ListBox _activity = new();

    public MainForm()
    {
        Text = "TabMover";
        MinimumSize = new Size(760, 590);
        Size = new Size(900, 690);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(17, 24, 39);
        ForeColor = Color.FromArgb(235, 240, 248);
        Font = new Font("Segoe UI", 9.5F);
        LoadSettings();
        _tray = new NotifyIcon { Icon = SystemIcons.Application, Text = "TabMover", Visible = true, ContextMenuStrip = BuildTrayMenu() };
        _tray.DoubleClick += (_, _) => ShowDashboard();
        BuildUi();
        AddActivity("TabMover 0.3.0-alpha desktop shell started.");
        AddActivity("Transfer engine is not enabled in this alpha.");
    }

    private void ShowDashboard() { Show(); WindowState = FormWindowState.Normal; Activate(); }

    private ContextMenuStrip BuildTrayMenu()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Open TabMover", null, (_, _) => ShowDashboard());
        menu.Items.Add("Pause / Resume", null, (_, _) => AddActivity("Pause/resume is reserved for the transfer coordinator milestone."));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => { _tray.Visible = false; Environment.Exit(0); });
        return menu;
    }

    private void BuildUi()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(22), BackColor = BackColor };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 42));
        Controls.Add(layout);

        var header = new Panel { Dock = DockStyle.Fill };
        header.Controls.Add(new Label { Text = "TabMover", Font = new Font("Segoe UI Semibold", 22), AutoSize = true, Location = new Point(0, 0) });
        header.Controls.Add(new Label { Text = "Status: Setup required · v0.3.0-alpha", AutoSize = true, ForeColor = Color.Khaki, Location = new Point(3, 40) });
        layout.Controls.Add(header, 0, 0);

        var cards = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        cards.Controls.Add(Card("Transfer engine", "Not connected"));
        cards.Controls.Add(Card("Browser integrations", "Not installed"));
        cards.Controls.Add(Card("Distribution", "Installer + portable"));
        layout.Controls.Add(cards, 0, 1);

        var tabs = new TabControl { Dock = DockStyle.Fill };
        tabs.TabPages.Add(TransferTab());
        tabs.TabPages.Add(SessionTab());
        tabs.TabPages.Add(BrowserTab());
        tabs.TabPages.Add(PrivacyTab());
        layout.Controls.Add(tabs, 0, 2);

        var activityPanel = new Panel { Dock = DockStyle.Fill };
        activityPanel.Controls.Add(new Label { Text = "Activity & diagnostics", Dock = DockStyle.Top, Height = 27, Font = new Font("Segoe UI Semibold", 11) });
        _activity.Dock = DockStyle.Fill;
        _activity.BackColor = Color.FromArgb(10, 15, 25);
        _activity.ForeColor = Color.FromArgb(220, 230, 240);
        activityPanel.Controls.Add(_activity);
        layout.Controls.Add(activityPanel, 0, 3);

        var save = new Button { Text = "Save settings", Dock = DockStyle.Bottom, Height = 34 };
        save.Click += (_, _) => { SaveSettings(); AddActivity("Settings saved."); };
        Controls.Add(save);
    }

    private Control Card(string title, string value)
    {
        var p = new Panel { Width = 250, Height = 66, BackColor = Color.FromArgb(31, 41, 55), Margin = new Padding(0, 0, 12, 0) };
        p.Controls.Add(new Label { Text = title, Location = new Point(12, 7), AutoSize = true, ForeColor = Color.LightGray });
        p.Controls.Add(new Label { Text = value, Location = new Point(12, 32), AutoSize = true, Font = new Font("Segoe UI Semibold", 10) });
        return p;
    }

    private TabPage Tab(string name) => new(name) { BackColor = Color.FromArgb(23, 32, 46), ForeColor = ForeColor, Padding = new Padding(16) };
    private Label Heading(string s) => new() { Text = s, AutoSize = true, Font = new Font("Segoe UI Semibold", 12), Padding = new Padding(3, 10, 3, 5) };

    private CheckBox Check(string text, bool value, Action<bool> set)
    {
        var c = new CheckBox { Text = text, Checked = value, AutoSize = true, ForeColor = ForeColor, Padding = new Padding(4, 6, 4, 6) };
        c.CheckedChanged += (_, _) => set(c.Checked);
        return c;
    }

    private FlowLayoutPanel Flow() => new() { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true };

    private TabPage TransferTab()
    {
        var p = Tab("Transfer"); var f = Flow();
        f.Controls.Add(Heading("Transfer behaviour"));
        f.Controls.Add(Check("Close original tab after successful, verified transfer", _settings.CloseSourceAfterVerifiedTransfer, v => _settings.CloseSourceAfterVerifiedTransfer = v));
        f.Controls.Add(Check("Show transfer notifications", _settings.ShowTransferNotifications, v => _settings.ShowTransferNotifications = v));
        f.Controls.Add(Check("Show visual drop-target overlay", _settings.EnableDragOverlay, v => _settings.EnableDragOverlay = v));
        f.Controls.Add(Heading("Destination tab placement"));
        var placement = new ComboBox { Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        placement.Items.AddRange(new object[] { "Next to active tab", "At end of tab strip", "Use browser default" });
        placement.SelectedItem = _settings.TabPlacement;
        placement.SelectedIndexChanged += (_, _) => _settings.TabPlacement = placement.SelectedItem?.ToString() ?? "Next to active tab";
        f.Controls.Add(placement);
        f.Controls.Add(new Label { Text = "On failure, the original tab is always kept open. This safety rule cannot be disabled.", AutoSize = true, ForeColor = Color.Khaki, Padding = new Padding(4, 10, 4, 4) });
        p.Controls.Add(f); return p;
    }

    private TabPage SessionTab()
    {
        var p = Tab("Session data"); var f = Flow();
        f.Controls.Add(Heading("Best-effort session migration"));
        f.Controls.Add(Check("Enable session-state transfer", _settings.AllowSessionStateTransfer, v => _settings.AllowSessionStateTransfer = v));
        f.Controls.Add(Check("Transfer cookies (subject to browser/site restrictions)", _settings.IncludeCookies, v => _settings.IncludeCookies = v));
        f.Controls.Add(Check("Transfer localStorage", _settings.IncludeLocalStorage, v => _settings.IncludeLocalStorage = v));
        f.Controls.Add(Check("Transfer sessionStorage where accessible", _settings.IncludeSessionStorage, v => _settings.IncludeSessionStorage = v));
        f.Controls.Add(Check("Transfer scroll position", _settings.IncludeScrollPosition, v => _settings.IncludeScrollPosition = v));
        f.Controls.Add(Check("Attempt safe form-field restoration (password/payment fields excluded)", _settings.IncludeFormState, v => _settings.IncludeFormState = v));
        f.Controls.Add(new Label { Text = "Live JavaScript state, password vaults, passkeys and all partitioned cookies cannot be universally cloned.", AutoSize = true, MaximumSize = new Size(690, 0), ForeColor = Color.Khaki, Padding = new Padding(4, 12, 4, 4) });
        p.Controls.Add(f); return p;
    }

    private TabPage BrowserTab()
    {
        var p = Tab("Browsers"); var f = Flow();
        f.Controls.Add(Heading("Browser integrations"));
        foreach (var b in new[] { "Brave", "Opera", "Chrome", "Microsoft Edge", "Vivaldi", "Firefox" })
            f.Controls.Add(new Label { Text = b + " — integration package not installed", AutoSize = true, ForeColor = Color.LightGray, Padding = new Padding(4, 6, 4, 6) });
        f.Controls.Add(new Label { Text = "Integration prototypes are included in this source package. They are not yet published browser-store extensions.", AutoSize = true, MaximumSize = new Size(690, 0), ForeColor = Color.Khaki, Padding = new Padding(4, 12, 4, 4) });
        p.Controls.Add(f); return p;
    }

    private TabPage PrivacyTab()
    {
        var p = Tab("Privacy"); var f = Flow();
        f.Controls.Add(Heading("Privacy & safety"));
        f.Controls.Add(Check("Allow private/incognito windows (not recommended)", _settings.AllowPrivateWindows, v => _settings.AllowPrivateWindows = v));
        f.Controls.Add(new Label { Text = "Session payloads must remain in memory. Cookie values, tokens, storage contents and form values must never be written to logs, diagnostics or temporary files.", AutoSize = true, MaximumSize = new Size(690, 0), ForeColor = Color.LightGray, Padding = new Padding(4, 12, 4, 4) });
        p.Controls.Add(f); return p;
    }

    private void AddActivity(string message) { _activity.Items.Add($"{DateTime.Now:HH:mm:ss}  {message}"); }
    private void LoadSettings()
    {
        try { if (File.Exists(_settingsPath)) _settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(_settingsPath)) ?? new(); }
        catch { _settings = new(); }
    }
    private void SaveSettings()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        File.WriteAllText(_settingsPath, JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true }));
    }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing) { e.Cancel = true; Hide(); _tray.ShowBalloonTip(1200, "TabMover is running", "Open the dashboard from the system tray.", ToolTipIcon.Info); }
        else base.OnFormClosing(e);
    }
    protected override void Dispose(bool disposing) { if (disposing) _tray.Dispose(); base.Dispose(disposing); }
}
