Your [question](https://stackoverflow.com/q/74873611/5438626) is **How to bring rows with search results on top in the datagridview** and your current approach is attempting to use [DataGridView.Sort](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.datagridview.sort?view=windowsdesktop-7.0#system-windows-forms-datagridview-sort(system-collections-icomparer)) to work with the UI control directly. With `DataGridView` it's often easier to achieve your desired outcome by setting the `DataSource` property (in this case, a `BindingList<Appliance>`) and working with that list instead. For starters, the DGV can be completely set up using <10 lines in the `OnLoad` of the mainform. 

    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();
        private BindingList<Appliance> Appliances { get; }  = new BindingList<Appliance>();
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Set up the DGV and add a few appliances.
            dtgridappliance.DataSource = Appliances;            
            Appliances.Add(new Appliance(ApplianceType.Refrigerator, "Maytag" ));
            Appliances.Add(new Appliance(ApplianceType.Refrigerator, "LG" ));
            Appliances.Add(new Appliance(ApplianceType.Microwave, "Amana" ));
            Appliances.Add(new Appliance(ApplianceType.Dishwasher, "Samsung" ));
            Appliances.Add(new Appliance(ApplianceType.Dishwasher, "Whirlpool" ));
            // Format columns
            dtgridappliance
            .Columns[nameof(Appliance.Name)]
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Populate the combo box and add a handler for when the combo box xhanges
            comboBoxSearch
            .Items
            .AddRange(Enum.GetValues(typeof(ApplianceType)).Cast<object>().ToArray());
            comboBoxSearch.SelectionChangeCommitted += onSearchByApplianceType;
        }
    }

Where the class representing a Row of data looks something like this:

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

 ***

 Now you just need a way to compare a given `Appliance` to the type selected in the combo box: 

    private int compareToSelectedType(Appliance appliance)
    {
        return appliance.ApplianceType.Equals(comboBoxSearch.SelectedItem) ?
            0 : 1; // Ones that match will sort before ones that don''t.
    }

 ***

 Now, when the combo box changes, you can remove all the DataGridView items and put them back in a different order.

    private void onSearchByApplianceType(object? sender, EventArgs e)
    {
        // Use System.Linq to sort a new temporary list.
        var tmp = new List<Appliance>(
            Appliances
            .OrderBy(_=>compareToSelectedType(_))
            .ThenBy(_=>_.Name));

        // Remove all items from the DataGridView
        Appliances.Clear();

        // Put them back in with the new order.
        foreach (var appliance in tmp)
        {
            Appliances.Add(appliance);
        }
    }

This is "just one way" to do it, but I [tested](https://github.com/IVSoftware/datagrid-view-sort.git) it and it works. Hope this helps get you closer to your intended outcome.