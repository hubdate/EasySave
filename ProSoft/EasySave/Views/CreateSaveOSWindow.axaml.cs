using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EasySave.ViewModels;

namespace EasySave.Views
{
    public partial class CreateSaveOsView : UserControl
    {
        public ObservableCollection<TableItem> tableData { get; set; }

        public CreateSaveOsView()
        {
            InitializeComponent();
            tableData = new ObservableCollection<TableItem>();
            DataContext = new CreateSaveOsViewModel(); // Créez une instance de CreateSaveOsViewModel
        }

        private void OnOptionButtonClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ((CreateSaveOsViewModel)DataContext).TableVisibility = true;

            // Initialiser la collection avec des données factices (à remplacer par vos propres données)
            tableData.Clear(); // Efface les données existantes, si présentes
            tableData.Add(new TableItem { Column1 = "Donnée 1A", Column2 = "Donnée 1B" });
            tableData.Add(new TableItem { Column1 = "Donnée 2A", Column2 = "Donnée 2B" });
            // ... Ajoutez autant d'éléments que nécessaire
        }
    }
}
