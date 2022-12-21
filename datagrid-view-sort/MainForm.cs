using System.ComponentModel;

namespace datagrid_view_sort
{
    // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridview.sort?view=windowsdesktop-7.0#system-windows-forms-datagridview-sort(system-collections-icomparer)
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();
        private BindingList<Appliance> Appliances { get; }  = new BindingList<Appliance>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Set up the DGV and add a few appliances.
            dtgridappliance.DataSource = Appliances;
            dtgridappliance.AllowUserToAddRows = false;
            Appliances.Add(new Appliance(ApplianceType.Refrigerator, "Maytag" ));
            Appliances.Add(new Appliance(ApplianceType.Refrigerator, "LG" ));
            Appliances.Add(new Appliance(ApplianceType.Microwave, "Amana" ));
            Appliances.Add(new Appliance(ApplianceType.Dishwasher, "Whirlpool" ));
            Appliances.Add(new Appliance(ApplianceType.Dishwasher, "Samsung" ));
            // Format columns
            dtgridappliance.Columns[nameof(Appliance.Name)].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Populate the combo box and dd a handler for when the combo box changes
            object[] values = Enum.GetValues(typeof(ApplianceType)).Cast<object>().ToArray();
            comboBoxSearch.Items.AddRange(values);
            comboBoxSearch.SelectedIndex = (int)ApplianceType.Refrigerator;
            comboBoxSearch.SelectionChangeCommitted += onSearchByApplianceType;
        }

        private void onSearchByApplianceType(object? sender, EventArgs e)
        {
            // Use System.Linq to sort a new temporary list.
            var tmp = new List<Appliance>(
                Appliances
                .OrderBy(_=>compareToSelectedType(_))
                .ThenBy(_ => _.ApplianceType)
                .ThenBy(_=>_.Name));

            // Remove all items from the DataGridView
            Appliances.Clear();

            // Put them back in with the new order.
            foreach (var appliance in tmp)
            {
                Appliances.Add(appliance);
            }
        }

        private int compareToSelectedType(Appliance appliance)
        {
            return appliance.ApplianceType.Equals(comboBoxSearch.SelectedItem) ?
                0 : 1;
        }
    }
    enum ApplianceType { Dishwasher, Microwave, Refrigerator  }
    class Appliance
    {
        public Appliance(ApplianceType applianceType, string name)
        {
            ApplianceType = applianceType;
            Name = name;
        }
        public ApplianceType ApplianceType { get; private set; }
        public string Name { get; private set; }
    }
}